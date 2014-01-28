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
                    cmp = y.id.CompareTo(x.id);
                }
                return cmp;
            }
        }

        private class IDOrderComparer : IComparer<Article>
        {
            public int Compare(Article x, Article y)
            {
                return y.id.CompareTo(x.id);
            }
        }

        private static DateOrderComparer dateOrder = new DateOrderComparer();
        private static IDOrderComparer idOrder = new IDOrderComparer();

        private Blog blog;
        private string uri;
        private DateTime date;
        private string title;
        private string normalizedTitle;
        private string id;

        internal Article(Blog blog, string uri, DateTime date, string title)
        {
            this.blog = blog;
            this.uri = uri;
            this.date = date;
            this.title = title;
            normalizedTitle = NormalizeTitle(title);
            id = GetIDFromUri(uri);

            if (this.date > DateTime.Now)
            {
                this.date = DateTime.Now;
            }
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

        private static string GetIDFromUri(string uri)
        {
            string[] s = uri.Split('/');
            return s[s.Length - 1];
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

        public static IComparer<Article> IDOrder
        {
            get
            {
                return idOrder;
            }
        }
    }
}
