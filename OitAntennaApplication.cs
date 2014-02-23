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
        private int cycleCount;

        private static Random random = new Random();

        public OitAntennaApplication()
        {
            Log.WriteLine("RSSリストの読み込み", true);
            IList<string> rawRssList = TextUtility.ReadLines(Settings.RssListFileName);
            CheckDuplicate(rawRssList);
            Log.WriteLine("RSSの重複なし", false);

            categories = CreateCategoriesFromRawRssList(rawRssList);

            blogs = GetRandomizedBlogList(categories.Values);
            Log.WriteLine("総ブログ数: " + blogs.Count, true);

            reloadIntervalMs = 1000 * 60 * Settings.ReloadIntervalMinutes / blogs.Count;
            Log.WriteLine("巡回間隔: " + (reloadIntervalMs / 1000.0).ToString("0.0") + "秒", false);

            cycleCount = 0;
        }

        public void Run()
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
                            Log.WriteLine(article.Title + " (" + article.ID + ")", false);
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

                cycleCount++;
                if (cycleCount == Settings.ReloadOrderShuffleInterval)
                {
                    blogs = GetRandomizedBlogList(categories.Values);
                    Log.WriteLine("ブログの更新順序をシャッフル", true);

                    DumpDebugData(categories.Values);
                    Log.WriteLine("デバッグ情報を出力", true);

                    cycleCount = 0;
                }
            }
        }

        public void OutputHtml()
        {
            using (StreamWriter writer = new StreamWriter(Settings.HtmlFileName, false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                HtmlUtility.BeginHtml(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteLargeWindow(writer, categories["VIPとか色々"]);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteHalfWindow(writer, categories["ニュース"], true);
                HtmlUtility.WriteHalfWindow(writer, categories["アニメ・漫画・ラノベ"], false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.BeginMainBox(writer);
                HtmlUtility.WriteArticleList(writer, categories["SS"], Settings.OtherCategoryNumArticles, true);
                HtmlUtility.WriteArticleList(writer, categories["画像"], Settings.OtherCategoryNumArticles, false);
                HtmlUtility.EndMainBox(writer);

                HtmlUtility.EndHtml(writer);
            }
        }

        private static IDictionary<string, Category> CreateCategoriesFromRawRssList(IEnumerable<string> rawRssList)
        {
            Dictionary<string, Category> categories = new Dictionary<string, Category>();
            string currentCategoryName = null;
            List<string> rssUris = new List<string>();
            foreach (string line in rawRssList)
            {
                if (line[0] == '[')
                {
                    if (currentCategoryName != null)
                    {
                        categories.Add(currentCategoryName, new Category(currentCategoryName, rssUris));
                        rssUris.Clear();
                    }
                    currentCategoryName = line.Substring(1, line.Length - 2);
                }
                else
                {
                    rssUris.Add(line);
                }
            }
            categories.Add(currentCategoryName, new Category(currentCategoryName, rssUris));
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

        private static void DumpDebugData(ICollection<Category> categories)
        {
            string date = DateTime.Now.ToString("yyMMddHHmmss");
            using (StreamWriter writer = new StreamWriter("blogdump" + date + ".txt", false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                foreach (Category category in categories)
                {
                    writer.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                    writer.WriteLine();
                    writer.WriteLine(category.Name);
                    writer.WriteLine();
                    writer.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                    foreach (Blog blog in category.Blogs)
                    {
                        writer.WriteLine("================================================================================");
                        writer.WriteLine(blog.Title);
                        writer.WriteLine("================================================================================");
                        foreach (Article article in blog.Articles)
                        {
                            writer.WriteLine(article.Uri + " (" + article.Date.ToString("yy/MM/dd HH:mm:ss") + ")");
                            writer.WriteLine(article.ID + ": " + article.Title);
                        }
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter("catedump" + date + ".txt", false, Encoding.GetEncoding(Settings.TextEncoding)))
            {
                foreach (Category category in categories)
                {
                    writer.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                    writer.WriteLine();
                    writer.WriteLine(category.Name);
                    writer.WriteLine();
                    writer.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                    foreach (ArticleBundle bundle in category.ArticleBundles)
                    {
                        foreach (Article article in bundle.Articles)
                        {
                            writer.WriteLine(article.Uri + " (" + article.Date.ToString("yy/MM/dd HH:mm:ss") + ")");
                            writer.WriteLine(article.ID + ": " + article.Title);
                        }
                        writer.WriteLine("--------------------------------------------------------------------------------");
                    }
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
