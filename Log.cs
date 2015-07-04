using System;
using System.IO;
using System.Text;

namespace OitAntenna
{
    public static class Log
    {
        private static StreamWriter writer;

        static Log()
        {
            if (Settings.OutputLogFile)
            {
                string dateTime = DateTime.Now.ToString("yyMMddHHmmss");
                writer = new StreamWriter("log" + dateTime + ".txt", false, Encoding.GetEncoding(Settings.TextEncoding));
            }
        }

        public static void WriteLine(string message, bool addDateTime)
        {
            if (addDateTime)
            {
                WriteLineSub(DateTime.Now.ToString("MM/dd HH:mm ") + message);
            }
            else
            {
                WriteLineSub(message);
            }
        }

        public static void WriteException(Exception e)
        {
            Console.WriteLine(e);
            if (Settings.OutputLogFile)
            {
                writer.WriteLine(e);
                writer.Flush();
            }
        }

        private static void WriteLineSub(string message)
        {
            if (message.Contains("(警告"))
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = color;
            }
            else
            {
                Console.WriteLine(message);
            }
            if (Settings.OutputLogFile)
            {
                writer.WriteLine(message);
                writer.Flush();
            }
        }
    }
}
