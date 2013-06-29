﻿using System;
using System.IO;
using System.Text;

namespace OitAntenna
{
    public static class Log
    {
        private static StreamWriter writer;

        static Log()
        {
            string dateTime = DateTime.Now.ToString("yyMMddHHmmss");
            writer = new StreamWriter("log" + dateTime + ".txt", false, Encoding.GetEncoding(Settings.TextEncoding));
        }

        public static void WriteLine(string message, bool addDateTime)
        {
            if (addDateTime)
            {
                WriteLineSub(DateTime.Now.ToString("MM/dd HH:mm ") + message);
            }
            else
            {
                WriteLineSub(DateTime.Now.ToString("            ") + message);
            }
        }

        public static void WriteException(Exception e)
        {
            Console.WriteLine(e);
            writer.WriteLine(e);
            writer.Flush();
        }

        private static void WriteLineSub(string message)
        {
            Console.WriteLine(message);
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}
