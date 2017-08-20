using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Visyn.Exceptions;
using Visyn.JetBrains;
using Visyn.Log;
using Visyn.Threads;

namespace Visyn.Io
{
    public class LogExceptionHandler : IExceptionHandler
    {
        public LogExceptionHandler(IOutputDevice outputDevice)
        {
            _output = outputDevice;
            _severity = outputDevice as IOutputDevice<SeverityLevel>;
        }

        private readonly IOutputDevice _output;
        private readonly IOutputDevice<SeverityLevel> _severity;
        #region Implementation of IExceptionHandler

        public bool HandleException(object sender, Exception exception)
        {
            if(_severity != null) _severity.WriteLine($"{sender.GetType().Name} {exception.GetType().Name} {exception.Message}",SeverityLevel.Error);
            else _output.WriteLine($"{sender.GetType().Name} {exception.GetType().Name} {exception.Message}");
            return _output != null;
        }

        #endregion
    }

    public class BackgroundOutputDevice : ProcessQueuedDataTask<Func<object>>, IOutputDevice
    {
        public override string Name { get; } = "Background Output Device";
        public Dispatcher Dispatcher { get; protected set; }
        //public int Count => Queue<>.Count;
        public int DelayIntervalMs { get; protected set; } = 100;
        public int? TaskId { get; protected set; }

        [NotNull]
        protected readonly IOutputDevice OutputDevice;

        private readonly Func<string, string> _process;
        

        /// <summary>
        /// Create background output device from any other IOutputDevice
        /// 
        /// </summary>
        /// <param name="outputDevice">Device to perform output (on background ThreadPool task)</param>
        /// <param name="process">Function that will process input and produce ultimate output.</param>
        protected BackgroundOutputDevice(IOutputDevice outputDevice, Func<string, string> process) 
            : base()
        {
            if (outputDevice == null) throw new NullReferenceException($"{nameof(BackgroundOutputDevice)} {nameof(outputDevice)} must be non-null!");
            OutputDevice = outputDevice;
            _process = process;
            Handler = new LogExceptionHandler(this);
            RateLimitTimeSpan = TimeSpan.FromMilliseconds(250);
        }

        public BackgroundOutputDevice(IOutputDevice outputDevice, Func<string, string> process, Dispatcher dispatcher)
            : this(outputDevice, process)
        {
            Dispatcher = dispatcher;
            
            this.StartTask();
        }
        

        protected string DequeueText()
        {
            var obj = DequeueItem();
            return obj as string;
        }


        protected object DequeueItem() => Dequeue()?.Invoke();

        protected int ProcessString(string output)
        {
            var action = new Action(() => OutputDevice.Write(output));
            if (Dispatcher != null) Dispatcher.BeginInvoke(action);
            else action();
            return 1;
        }

        #region Implementation of IOutputDevice

        public void Write(string text)
        {
            Add(() => _process != null ? _process(text) : text);
        }

        public void WriteLine(string line)
        {
            if (_process != null)
            {
                Add(() => _process(line) + Environment.NewLine);
            }
            else
            {
                Add(() => (line + Environment.NewLine));
            }
        }

        public void Write(Func<string> func)
        {
            if (func == null) return;
            Add(_process != null ? (() => _process(func.Invoke())) : func);//func);
        }
        
        #endregion

        #region Overrides of ProcessQueuedDataTask<Func<object>>

        protected override void ProcessData()
        {
            var count = Count;
            if (count > 0)
            {
                var index = 0;

                var output = DequeueText();

                while (--count > 0)
                {
                    output += DequeueText();
                    if (index++ > 10) break; ;
                }
                ProcessString(output);
            }
            else
            {
                Task.Delay(DelayIntervalMs);
            }
        }

        #endregion
    }
    
}