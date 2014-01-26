﻿using System;
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
            foreach (ArticleBundle bundle in articleBundleSet)
            {
                if (bundle.MainArticle.Blog != article.Blog && AreSame(bundle.MainArticle, article))
                {
                    bundle.Bundle(article);
                    return;
                }
            }
            articleBundleSet.Add(new ArticleBundle(article));

            while (articleBundleSet.Count > maxNumArticleBundles)
            {
                articleBundleSet.Remove(articleBundleSet.Max);
            }
        }

        private static bool AreSame(Article x, Article y)
        {
            int length = Math.Max(x.NormalizedTitle.Length, y.NormalizedTitle.Length);
            if (length < 5)
            {
                return false;
            }
            double d = (double)TextUtility.GetLevenshteinDistance(x.NormalizedTitle, y.NormalizedTitle) / length;
            if (d < Settings.StringDistanceThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
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
