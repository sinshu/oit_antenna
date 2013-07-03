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
            Log.WriteLine(Settings.Title + "起動", true);
            OitAntennaApplication app = new OitAntennaApplication();
            Log.WriteLine("初期化完了", true);
            app.OutputHtml();
            Log.WriteLine("巡回開始", true);
            app.Crawl();
        }
    }
}
