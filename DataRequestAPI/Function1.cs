using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;

namespace DataRequestAPI
{
    public static class Function1
    {
        [FunctionName("YesterdayRequest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("OpenData API request.");

            string url = req.Query["url"];
            string key = req.Query["key"];

            var datas = await RequestCovid(url,key,log);
            if (datas != null)
            {
                var targetDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                var cnt = datas.Where(x => x.���\�� == targetDate).Count();

                return new OkObjectResult(cnt);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        private static async Task<List<CovidModel>> RequestCovid(string url,string key, ILogger log)
        {
            var requrl = $"{url}?apikey={key}";
            var result = await new HttpClient().GetAsync(requrl, HttpCompletionOption.ResponseHeadersRead);
            var jsonString = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode == false)
            {
                log.LogError(jsonString);
                return null;
            }
            var datas = JsonConvert.DeserializeObject<List<CovidModel>>(jsonString);

            return datas;
        }

        public class CovidModel
        {
            public string ���\�� { get; set; }
            public string ���Z�n { get; set; }
            public string �N�� { get; set; }
            public string ���� { get; set; }
        }
    }
}
