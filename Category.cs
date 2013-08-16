using System;
using System.Collections.Generic;

namespace OitAntenna
{
    public class Category
    {
        private string name;
        private ArticleBundleHolder articleBundleHolder;
        private Blog[] blogs;

        internal Category(string name, ICollection<string> rssUris)
        {
            this.name = name;
            articleBundleHolder = new ArticleBundleHolder(Settings.CategoryMaxNumArticleBundles);
            blogs = new Blog[rssUris.Count];

            Log.WriteLine("カテゴリ[" + name + "]の初期化", true);

            int i = 0;
            foreach (string rssUri in rssUris)
            {
                Log.WriteLine("RSS[" + rssUri + "]を取得", false);
                blogs[i] = new Blog(this, rssUri);
                Log.WriteLine("ブログ[" + blogs[i].Title + "]を確認", false);

                if (DateTime.Now - blogs[i].NewestArticle.Date >= TimeSpan.FromDays(30))
                {
                    int m = (int)((DateTime.Now - blogs[i].NewestArticle.Date).TotalDays / 30);
                    Log.WriteLine("(警告)ブログ[" + blogs[i].Title + "]は" + m + "ヵ月以上更新されていない", false);
                }

                i++;
            }
        }

        internal void BlogUpdated(ICollection<Article> newArticles)
        {
            foreach (Article article in newArticles)
            {
                articleBundleHolder.Add(article);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ICollection<Blog> Blogs
        {
            get
            {
                return blogs;
            }
        }

        public ICollection<ArticleBundle> ArticleBundles
        {
            get
            {
                return articleBundleHolder.ArticleBundles;
            }
        }
    }
}
