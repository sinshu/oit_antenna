using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace OitAntenna
{
    public class OitAntennaApplication
    {
        private static Random random = new Random();

        private Category cateVip;
        private Category cateNews;
        private Category cateAnime;
        private Category cateSs;
        private Category cateImage;

        private Blog[] blogs;
        private int reloadIntervalMs;

        public OitAntennaApplication()
        {
            Log.WriteLine(Settings.Title + "起動", true);

            Log.WriteLine("RSSリストの読み込み", true);
            IList<string> rssVip = TextUtility.ReadLines(@"rsslist\vip.txt");
            IList<string> rssNews = TextUtility.ReadLines(@"rsslist\news.txt");
            IList<string> rssAnime = TextUtility.ReadLines(@"rsslist\anime.txt");
            IList<string> rssSs = TextUtility.ReadLines(@"rsslist\ss.txt");
            IList<string> rssImage = TextUtility.ReadLines(@"rsslist\image.txt");

            CheckDuplicate(rssVip, rssNews, rssAnime, rssSs, rssImage);
            Log.WriteLine("RSSリストの重複なし", false);

            cateVip = new Category("VIPとか色々", 300, rssVip);
            cateNews = new Category("ニュース", 300, rssNews);
            cateAnime = new Category("アニメ・漫画・ラノベ", 300, rssAnime);
            cateSs = new Category("SS", 30, rssSs);
            cateImage = new Category("画像", 30, rssImage);

            blogs = GetRandomizedBlogArray(cateVip, cateNews, cateAnime, cateSs, cateImage);
            Log.WriteLine("総ブログ数: " + blogs.Length, false);

            reloadIntervalMs = 1000 * 60 * Settings.ReloadIntervalMinutes / blogs.Length;
            Log.WriteLine("巡回間隔: " + (reloadIntervalMs / 1000.0).ToString("0.0") + "秒", false);
        }

        public void AutoF5()
        {
            while (true)
            {
                foreach (Blog blog in blogs)
                {
                    Thread.Sleep(reloadIntervalMs);

                    ICollection<Article> newArticles = null;
                    try
                    {
                        newArticles = blog.Reload();
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine("ブログ[" + blog.Title + "]の更新に失敗", true);
                        Log.WriteException(e);
                    }

                    if (newArticles != null && newArticles.Count > 0)
                    {
                        Log.WriteLine("ブログ[" + blog.Title + "]から" + newArticles.Count + "件の新着記事", true);
                        foreach (Article article in newArticles)
                        {
                            Log.WriteLine("    " + article.Title, false);
                        }
                        try
                        {
                            OutputHtml();
                        }
                        catch (Exception e)
                        {
                            Log.WriteLine("HTMLファイルの出力に失敗", true);
                            Log.WriteException(e);
                        }
                    }
                }
            }
        }

        public void OutputHtml()
        {
            using (StreamWriter writer = new StreamWriter("index.html", false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                HtmlUtility.BeginHtml(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteLargeWindow(writer, cateVip);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteHalfWindow(writer, cateNews, true);
                HtmlUtility.WriteHalfWindow(writer, cateAnime, false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteArticleList(writer, cateSs, true);
                HtmlUtility.WriteArticleList(writer, cateImage, false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.EndHtml(writer);
            }
        }

        private static void CheckDuplicate(params IList<string>[] lists)
        {
            SortedSet<string> set = new SortedSet<string>();
            foreach (IList<string> list in lists)
            {
                foreach (string rssUri in list)
                {
                    if (!set.Add(rssUri))
                    {
                        throw new Exception("RSS[" + rssUri + "]が重複している");
                    }
                }
            }
        }

        private static Blog[] GetRandomizedBlogArray(params Category[] categories)
        {
            int length = 0;
            foreach (Category category in categories)
            {
                length += category.Blogs.Count;
            }

            Blog[] blogs = new Blog[length];
            {
                int i = 0;
                foreach (Category category in categories)
                {
                    foreach (Blog blog in category.Blogs)
                    {
                        blogs[i] = blog;
                        i++;
                    }
                }
            }

            for (int i = 0; i < blogs.Length; i++)
            {
                int j = random.Next(i, blogs.Length);
                Blog temp = blogs[i];
                blogs[i] = blogs[j];
                blogs[j] = temp;
            }

            return blogs;
        }
    }
}
