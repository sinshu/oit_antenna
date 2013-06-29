﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace OitAntenna
{
    public class Blog
    {
        private Category category;
        private string rssUri;
        private string uri;
        private string title;
        private SortedSet<Article> articleSet;

        internal Blog(Category category, string rssUri)
        {
            this.category = category;
            this.rssUri = rssUri;
            articleSet = new SortedSet<Article>(Article.UriOrder);
            Reload();
        }

        // 糞実装
        public ICollection<Article> Reload()
        {
            XmlDocument document = new XmlDocument();
            document.Load(rssUri);

            uri = document.GetElementsByTagName("link")[0].InnerText;
            title = document.GetElementsByTagName("title")[0].InnerText;

            List<Article> newArticleList = new List<Article>();

            XmlNodeList itemNodes = document.GetElementsByTagName("item");
            foreach (XmlNode itemNode in itemNodes)
            {
                XmlElement itemElement = (XmlElement)itemNode;
                string articleUri = itemElement.GetElementsByTagName("link")[0].InnerText;
                XmlNodeList dateNodes = itemElement.GetElementsByTagName("dc:date");
                if (dateNodes.Count == 0)
                {
                    dateNodes = itemElement.GetElementsByTagName("pubDate");
                }
                DateTime articleDate = DateTime.Parse(dateNodes[0].InnerText);
                string articleTitle = itemElement.GetElementsByTagName("title")[0].InnerText;
                Article newArticle = new Article(this, articleUri, articleDate, articleTitle);
                if (articleSet.Add(newArticle))
                {
                    newArticleList.Add(newArticle);
                }
            }

            category.BlogUpdated(newArticleList);

            while (articleSet.Count > Settings.BlogMaxNumArticles)
            {
                articleSet.Remove(articleSet.Max);
            }

            return newArticleList;
        }

        public Category Category
        {
            get
            {
                return category;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public ICollection<Article> Articles
        {
            get
            {
                return articleSet;
            }
        }

        public Article NewestArticle
        {
            get
            {
                return articleSet.Min;
            }
        }
    }
}
