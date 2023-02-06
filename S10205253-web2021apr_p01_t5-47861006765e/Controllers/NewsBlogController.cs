using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    public class NewsBlogController : Controller
    {
        public async Task<ActionResult> Index()
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://newsapi.org/v2/top-headlines");
            HttpResponseMessage response = await client.GetAsync("?country=sg&apiKey=bb90fad2aea145c8bd8e164e8b486d1c");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var newsArticles = JsonConvert.DeserializeObject<News>(data);
                return View(newsArticles);
            }
            else
            {
                return View();
            }

            //        var client = new HttpClient();
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Get,
            //            RequestUri = new Uri("https://newsapi.org/v2/top-headlines?country=us&apiKey=bb90fad2aea145c8bd8e164e8b486d1c"),
            ////            Headers =
            ////{
            ////                { "x-rapidapi-key", "7ef7505eeemsh4b7ae28b990ec32p10a9e5jsnf404942f045e" },
            ////                { "x-rapidapi-host", "InstagramdimashirokovV1.p.rapidapi.com" },
            ///

            ////},
            //        };
            //        using (var response = await client.SendAsync(request))
            //        {
            //            response.EnsureSuccessStatusCode();
            //            var body = await response.Content.ReadAsStringAsync();
            // var newsArticles = JsonConvert.DeserializeObject<News>(body);
            //            return View(newsArticles);
            //        }

        }

        public ActionResult AboutUs()
        {
            return View();
        }

    }

}
