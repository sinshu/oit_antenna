using System;
using System.IO;

namespace OitAntenna
{
    public static class HtmlUtility
    {
        private const string daysOfWeek = "日月火水木金土";

        public static string Escape(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        public static string CreateLink(string uri, string name)
        {
            return "<a href=\"" + Escape(uri) + "\" target=\"_blank\">" + Escape(name) + "</a>";
        }

        public static string CreateLink(string uri, string name, string linkClass)
        {
            return "<a href=\"" + Escape(uri) + "\" target=\"_blank\" class=\"" + linkClass + "\">" + Escape(name) + "</a>";
        }

        public static string CreateLink(string uri, string name, string linkClass, string toolTip)
        {
            return "<a href=\"" + Escape(uri) + "\" target=\"_blank\" class=\"" + linkClass + "\" title=\"" + Escape(toolTip) + "\">" + Escape(name) + "</a>";
        }

        public static void BeginHtml(TextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            writer.WriteLine("<html lang=\"ja\">");
            writer.WriteLine("<head>");
            writer.WriteLine("<meta name=\"robots\" content=\"noindex, nofollow, noarchive\">");
            writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + Settings.TextEncoding + "\">");
            writer.WriteLine("<meta http-equiv=\"Content-Style-Type\" content=\"text/css\">");
            writer.WriteLine("<title>" + Settings.Title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            writer.WriteLine("<link rel=\"shortcut icon\" href=\"favicon.ico\" type=\"image/vnd.microsoft.icon\">");
            writer.WriteLine("<link rel=\"icon\" href=\"favicon.ico\" type=\"image/vnd.microsoft.icon\">");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            WriteTitle(writer);
        }

        public static void EndHtml(TextWriter writer)
        {
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        public static void BeginMainBox(TextWriter writer)
        {
            writer.WriteLine("<div class=\"mainbox\">");
        }

        public static void EndMainBox(TextWriter writer)
        {
            writer.WriteLine("</div>");
        }

        public static void WriteTitle(TextWriter writer)
        {
            DateTime now = DateTime.Now;
            BeginMainBox(writer);
            writer.WriteLine("<table>");
            writer.WriteLine("<tr><td><img src=\"oitlogo.png\" alt=\"\"></td><td class=\"pagetitle\">" + Settings.Title + "</td></tr>");
            writer.WriteLine("<tr><td class=\"lastupdate\" colspan=\"2\">最終更新日：" + now.ToString("yyyy/MM/dd") + "(" + daysOfWeek[(int)now.DayOfWeek] + ") " + now.ToString("HH:mm:ss") + "</td></tr>");
            writer.WriteLine("</table>");
            EndMainBox(writer);
        }

        public static void WriteLargeWindow(TextWriter writer, Category category)
        {
            writer.WriteLine("<span class=\"titlebar\">" + category.Name + "</span>");
            writer.WriteLine("<div class=\"largewindow\">");
            WriteArticleListSub(writer, category, Settings.MainCategoryNumArticles, true, true);
            writer.WriteLine("</div>");
        }

        public static void WriteHalfWindow(TextWriter writer, Category category, bool left)
        {
            string boxClass = left ? "leftbox" : "rightbox";
            writer.WriteLine("<div class=\"" + boxClass + "\">");
            writer.WriteLine("<span class=\"titlebar\">" + category.Name + "</span>");
            writer.WriteLine("<div class=\"halfwindow\">");
            WriteArticleListSub(writer, category, Settings.SubCategoryNumArticles, true, false);
            writer.WriteLine("</div>");
            writer.WriteLine("</div>");
        }

        public static void WriteArticleList(TextWriter writer, Category category, int numArticles, bool left)
        {
            string boxClass = left ? "leftbox" : "rightbox";
            writer.WriteLine("<div class=\"" + boxClass + "\">");
            writer.WriteLine("<div class=\"category\">" + category.Name + "</div>");
            WriteArticleListSub(writer, category, numArticles, false, false);
            writer.WriteLine("</div>");
        }

        private static void WriteArticleListSub(TextWriter writer, Category category, int numArticles, bool addDateTime, bool addBlogTitle)
        {
            writer.WriteLine("<table class=\"articlelist\">");

            int i = 0;
            int day = 0;
            foreach (ArticleBundle bundle in category.ArticleBundles)
            {
                if (addDateTime && bundle.MainArticle.Date.Day != day)
                {
                    writer.WriteLine("<tr><td class=\"date\" colspan=\"2\">" + bundle.MainArticle.Date.ToString("MM/dd") + "</td></tr>");
                    day = bundle.MainArticle.Date.Day;
                }

                writer.Write("<tr>");
                if (addDateTime)
                {
                    writer.Write("<td class=\"time\">" + bundle.MainArticle.Date.ToString("HH:mm") + "</td>");
                }
                writer.Write("<td>");
                int j = 0;
                foreach (Article article in bundle.Articles)
                {
                    if (j == 0)
                    {
                        string title = article.Title;
                        if (title.Length == 0)
                        {
                            title = "無題";
                        }
                        writer.Write(CreateLink(article.Uri, title, "article", article.Blog.Title));
                    }
                    else
                    {
                        writer.Write(" " + CreateLink(article.Uri, "●", "duplicate", article.Blog.Title));
                    }
                    j++;
                }
                writer.Write("</td>");
                if (addBlogTitle)
                {
                    writer.Write("<td class=\"blogtitle\">" + CreateLink(bundle.MainArticle.Blog.Uri, bundle.MainArticle.Blog.Title, "article") + "</td>");
                }
                writer.WriteLine("</tr>");

                i++;
                if (i >= numArticles)
                {
                    break;
                }
            }

            writer.WriteLine("</table>");
        }
    }
}
