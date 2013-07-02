using System;

namespace OitAntenna
{
    public static class Settings
    {
        public const string TextEncoding = "UTF-8";
        public const string HtmlFileName = "index.html";
        public const string CssFileName = "style.css";
        public const string RssListFileName = "rsslist.txt";

        public const string Title = "OITあんてな";

        public const int BlogMaxNumArticles = 100;
        public const int ArticleBundleMaxNumArticles = 30;
        public const double StringDistanceThreshold = 0.3;

        public const int ReloadIntervalMinutes = 15;
    }
}
