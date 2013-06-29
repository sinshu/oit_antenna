using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace OitAntenna
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OitAntennaApplication app = new OitAntennaApplication();
            app.OutputHtml();
            app.AutoF5();
        }
    }
}
