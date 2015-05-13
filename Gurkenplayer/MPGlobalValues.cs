using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurkenplayer
{
    public static class MPGlobalValues
    {
        static bool isConfigurationFinished = false;

        public static bool IsConfigurationFinished
        {
            get { return MPGlobalValues.isConfigurationFinished; }
            set { MPGlobalValues.isConfigurationFinished = value; }
        }
    }
}