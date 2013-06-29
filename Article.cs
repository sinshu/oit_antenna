using System;
using System.Collections.Generic;
using System.Text;

namespace OitAntenna
{
    public class Article
    {
        private class DateOrderComparer : IComparer<Article>
        {
            public int Compare(Article x, Article y)
            {
                int cmp = y.date.CompareTo(x.date);
                if (cmp == 0)
                {
                    cmp = y.uri.CompareTo(x.uri);
                }
                return cmp;
            }
        }

        private class UriOrderComparer : IComparer<Article>
        {
            public int Compare(Article x, Article y)
            {
                return y.uri.CompareTo(x.uri);
            }
        }

        private static DateOrderComparer dateOrder = new DateOrderComparer();
        private static UriOrderComparer uriOrder = new UriOrderComparer();

        private Blog blog;
        private string uri;
        private DateTime date;
        private string title;
        private string normalizedTitle;

        internal Article(Blog blog, string uri, DateTime date, string title)
        {
            this.blog = blog;
            this.uri = uri;
            this.date = date;
            this.title = title;
            normalizedTitle = NormalizeTitle(title);
        }

        public static string NormalizeTitle(string title)
        {
            string s = TextUtility.Normalize(title).Replace("w", "");
            StringBuilder sb = new StringBuilder();
            char prev = '\0';
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != prev)
                {
                    sb.Append(s[i]);
                    prev = s[i];
                    count = 1;
                }
                else
                {
                    if (count < 3)
                    {
                        sb.Append(s[i]);
                        count++;
                    }
                }
            }
            return sb.ToString();
        }

        public Blog Blog
        {
            get
            {
                return blog;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public string NormalizedTitle
        {
            get
            {
                return normalizedTitle;
            }
        }

        public static IComparer<Article> DateOrder
        {
            get
            {
                return dateOrder;
            }
        }

        public static IComparer<Article> UriOrder
        {
            get
            {
                return uriOrder;
            }
        }
    }
}
