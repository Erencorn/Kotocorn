﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotocorn.Core.Services
{
    public class StandardConversions
    {
        public static double CelsiusToFahrenheit(double cel)
        {
            return cel * 1.8f + 32;
        }
    }
}
