﻿using System;
using System.IO;

namespace OitAntenna
{
    public static class HtmlUtility
    {
        public static string Escape(string s)
        {
            return s.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;");
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
            writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + Settings.TextEncoding + "\">");
            writer.WriteLine("<meta http-equiv=\"Content-Style-Type\" content=\"text/css\">");
            writer.WriteLine("<title>" + Settings.Title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            BeginMainBox(writer);
            writer.WriteLine("<table><tr><td><img src=\"oitlogo.png\" alt=\"\"></td><td class=\"pagetitle\">" + Settings.Title + "</td></tr></table>");
            EndMainBox(writer);
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

        public static void WriteLargeWindow(TextWriter writer, Category category)
        {
            writer.WriteLine("<span class=\"titlebar\">" + category.Name + "</span>");
            writer.WriteLine("<div class=\"largewindow\">");
            WriteArticleListSub(writer, category, true, true);
            writer.WriteLine("</div>");
        }

        public static void WriteHalfWindow(TextWriter writer, Category category, bool left)
        {
            string boxClass = left ? "leftbox" : "rightbox";
            writer.WriteLine("<div class=\"" + boxClass + "\">");
            writer.WriteLine("<span class=\"titlebar\">" + category.Name + "</span>");
            writer.WriteLine("<div class=\"halfwindow\">");
            WriteArticleListSub(writer, category, true, false);
            writer.WriteLine("</div>");
            writer.WriteLine("</div>");
        }

        public static void WriteArticleList(TextWriter writer, Category category, bool left)
        {
            string boxClass = left ? "leftbox" : "rightbox";
            writer.WriteLine("<div class=\"" + boxClass + "\">");
            writer.WriteLine("<div class=\"category\">" + category.Name + "</div>");
            WriteArticleListSub(writer, category, false, false);
            writer.WriteLine("</div>");
        }

        private static void WriteArticleListSub(TextWriter writer, Category category, bool addDateTime, bool addBlogTitle)
        {
            writer.WriteLine("<table>");

            int currentDay = 0;
            foreach (ArticleBundle bundle in category.ArticleBundles)
            {
                if (addDateTime && bundle.MainArticle.Date.Day != currentDay)
                {
                    writer.Write("<tr><td class=\"date\" colspan=\"2\">" + bundle.MainArticle.Date.ToString("MM/dd") + "</td></tr>");
                    currentDay = bundle.MainArticle.Date.Day;
                }

                writer.Write("<tr>");
                if (addDateTime)
                {
                    writer.Write("<td class=\"time\">" + bundle.MainArticle.Date.ToString("HH:mm") + "</td>");
                }
                writer.Write("<td>");
                int i = 0;
                foreach (Article article in bundle.Articles)
                {
                    if (i == 0)
                    {
                        writer.Write(CreateLink(article.Uri, article.Title, "article", article.Blog.Title));
                    }
                    else
                    {
                        writer.Write(" " + CreateLink(article.Uri, "●", "duplicate", article.Blog.Title));
                    }
                    i++;
                }
                writer.Write("</td>");
                if (addBlogTitle)
                {
                    writer.Write("<td class=\"blogtitle\">" + CreateLink(bundle.MainArticle.Blog.Uri, bundle.MainArticle.Blog.Title, "article") + "</td>");
                }
                writer.WriteLine("</tr>");
            }

            writer.WriteLine("</table>");
        }
    }
}
