using System;
using System.Collections.Generic;

namespace OitAntenna
{
    public class ArticleBundle
    {
        private class MainArticleDateOrderComparer : IComparer<ArticleBundle>
        {
            public int Compare(ArticleBundle x, ArticleBundle y)
            {
                int cmp = y.MainArticle.Date.CompareTo(x.MainArticle.Date);
                if (cmp == 0)
                {
                    y.MainArticle.Uri.CompareTo(x.MainArticle.Uri);
                }
                return cmp;
            }
        }

        private static MainArticleDateOrderComparer mainArticleDateOrder = new MainArticleDateOrderComparer();

        private SortedSet<Article> articleSet;

        internal ArticleBundle(Article mainArticle)
        {
            articleSet = new SortedSet<Article>(Article.DateOrder);
            articleSet.Add(mainArticle);
        }

        public void Bundle(Article article)
        {
            if (articleSet.Count < Settings.ArticleBundleMaxNumArticles)
            {
                articleSet.Add(article);
            }
        }

        public Article MainArticle
        {
            get
            {
                return articleSet.Min;
            }
        }

        public ICollection<Article> Articles
        {
            get
            {
                return articleSet;
            }
        }

        public static IComparer<ArticleBundle> MainArticleDateOrder
        {
            get
            {
                return mainArticleDateOrder;
            }
        }
    }
}
