using System;
using System.Collections.Generic;

namespace OitAntenna
{
    public class ArticleBundleHolder
    {
        private int maxNumArticleBundles;
        private SortedSet<ArticleBundle> articleBundleSet;

        public ArticleBundleHolder(int maxNumArticleBundles)
        {
            this.maxNumArticleBundles = maxNumArticleBundles;
            articleBundleSet = new SortedSet<ArticleBundle>(ArticleBundle.MainArticleDateOrder);
        }

        public void Add(Article article)
        {
            ArticleBundle targetBundle = null;
            foreach (ArticleBundle bundle in articleBundleSet)
            {
                if (bundle.MainArticle.Blog != article.Blog && AreSame(bundle.MainArticle, article))
                {
                    targetBundle = bundle;
                    break;
                }
            }
            if (targetBundle != null)
            {
                articleBundleSet.Remove(targetBundle);
                targetBundle.Bundle(article);
                articleBundleSet.Add(targetBundle);
            }
            else
            {
                articleBundleSet.Add(new ArticleBundle(article));
                while (articleBundleSet.Count > maxNumArticleBundles)
                {
                    articleBundleSet.Remove(articleBundleSet.Max);
                }
            }
        }

        private static bool AreSame(Article x, Article y)
        {
            int length = Math.Max(x.NormalizedTitle.Length, y.NormalizedTitle.Length);
            if (length < 5)
            {
                return false;
            }

            double d1 = 1 - (double)Math.Min(x.NormalizedTitle.Length, y.NormalizedTitle.Length)
                / Math.Max(x.NormalizedTitle.Length, y.NormalizedTitle.Length);
            if (d1 >= Settings.StringDistanceThreshold)
            {
                return false;
            }

            double d2 = (double)TextUtility.GetLevenshteinDistance(x.NormalizedTitle, y.NormalizedTitle) / length;
            if (d2 >= Settings.StringDistanceThreshold)
            {
                return false;
            }

            return true;
        }

        public ICollection<ArticleBundle> ArticleBundles
        {
            get
            {
                return articleBundleSet;
            }
        }
    }
}
