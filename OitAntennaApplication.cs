using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace OitAntenna
{
    public class OitAntennaApplication
    {
        private IDictionary<string, Category> categories;
        private IList<Blog> blogs;
        private int reloadIntervalMs;

        private static Random random = new Random();

        public OitAntennaApplication()
        {
            Log.WriteLine(Settings.Title + "起動", true);

            Log.WriteLine("RSSリストの読み込み", true);
            IList<string> rawRssList = TextUtility.ReadLines(Settings.RssListFileName);
            CheckDuplicate(rawRssList);
            Log.WriteLine("RSSの重複なし", false);

            categories = CreateCategoriesFromRawRssList(rawRssList);

            blogs = GetRandomizedBlogList(categories.Values);
            Log.WriteLine("総ブログ数: " + blogs.Count, true);

            reloadIntervalMs = 1000 * 60 * Settings.ReloadIntervalMinutes / blogs.Count;
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
            string mainCategory;
            string subCategory1;
            string subCategory2;
            DateTime now = DateTime.Now;
            if (5 <= now.Hour && now.Hour <= 9)
            {
                mainCategory = "ニュース";
                subCategory1 = "VIPとか色々";
                subCategory2 = "アニメ・漫画・ラノベ";
            }
            else
            {
                mainCategory = "VIPとか色々";
                subCategory1 = "ニュース";
                subCategory2 = "アニメ・漫画・ラノベ";
            }

            using (StreamWriter writer = new StreamWriter(Settings.HtmlFileName, false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                HtmlUtility.BeginHtml(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteLargeWindow(writer, categories[mainCategory]);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteHalfWindow(writer, categories[subCategory1], true);
                HtmlUtility.WriteHalfWindow(writer, categories[subCategory2], false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteArticleList(writer, categories["SS"], true);
                HtmlUtility.WriteArticleList(writer, categories["画像"], false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.EndHtml(writer);
            }
        }

        private static IDictionary<string, Category> CreateCategoriesFromRawRssList(IList<string> rawRssList)
        {
            Dictionary<string, Category> categories = new Dictionary<string, Category>();
            string categoryName = null;
            int maxNumArticles = 0;
            List<string> rssUris = new List<string>();
            foreach (string line in rawRssList)
            {
                if (line[0] == '[')
                {
                    if (categoryName != null)
                    {
                        categories.Add(categoryName, new Category(categoryName, maxNumArticles, rssUris));
                        rssUris.Clear();
                    }
                    string[] temp = line.Substring(1, line.Length - 2).Split(',');
                    categoryName = temp[0];
                    maxNumArticles = int.Parse(temp[1]);
                }
                else
                {
                    rssUris.Add(line);
                }
            }
            categories.Add(categoryName, new Category(categoryName, maxNumArticles, rssUris));
            return categories;
        }

        private static void CheckDuplicate(ICollection<string> rssUris)
        {
            SortedSet<string> set = new SortedSet<string>();
            foreach (string rssUri in rssUris)
            {
                if (!set.Add(rssUri))
                {
                    throw new Exception("RSS[" + rssUri + "]が重複している");
                }
            }
        }

        private static IList<Blog> GetRandomizedBlogList(ICollection<Category> categories)
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
