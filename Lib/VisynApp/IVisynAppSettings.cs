﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visyn.Public.VisynApp
{
    /// <summary>
    /// Interface IVisynAppSettings
    /// </summary>
    public interface IVisynAppSettings
    {
        //
        bool AreValid { get; }

        void InitializeDefaultSettings(object context);
    }
}