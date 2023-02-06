using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Asg.Models
{
    public class Source
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Article
    {
        public Source source { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string urlToImage { get; set; }
        public DateTime publishedAt { get; set; }
        public string content { get; set; }
    }

    public class News
    {
        public string status { get; set; }
        public int totalResults { get; set; }
        public List<Article> articles { get; set; }
    }
    //public class Articles
    //{
    //    public Sources Source { get; set; }
    //    public string Author { get; set; }
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string Url { get; set; }
    //    public string UrlToImg { get; set; }
    //    public string PublishedAt { get; set; }
    //    public string Content { get; set; }

    //}
    //public class Sources
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }

    //}
    //public class News
    //{
    //    public string Status { get; set; }
    //    public int TotalResults { get; set; }
    //    public List<Articles> Article { get; set; }

    //}
}
