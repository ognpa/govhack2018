using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace eda.bot.eda
{
    public class EdaClient
    {

        static string edaClientUrl = "http://www.aemo.com.au/aemo/apps/api/report/5MIN";
        static string url1 = "http://ec2-34-209-171-216.us-west-2.compute.amazonaws.com/ato/dtypes";
        static string rootUrl = "http://ec2-34-209-171-216.us-west-2.compute.amazonaws.com";



        public class DTType {
            public string ColName;
            public string ColType; 
            }
          public static async Task<List<DTType>> GetAtoDataTypesAsync(string dataset)
        {

            string requestUrl = rootUrl + $"/{dataset}/dtypes";


            var client = new HttpClient()as HttpClient;
            //client.BaseAddress = new Uri(url1);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(requestUrl); // +  "/ato/dtypes");
            //response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            string str = await response.Content.ReadAsStringAsync();
            var dtTypes = JsonConvert.DeserializeObject(str);
            var ch = (dtTypes as Newtonsoft.Json.Linq.JObject).Children().ToList();

            List<DTType> ddList = new List<DTType>(); 
            foreach (var item in ch)
            {
                ddList.Add(new DTType()
                {
                    ColName = (item as JProperty).Name,
                    ColType = (item as JProperty).Value.ToString()
                }); 
                System.Diagnostics.Debug.Print($"item {item.ToString()}"); 
            }
            return ddList; 

        }

        public static async Task<List<string>> GetPlotAsync(string dataset)
        {
            string requestUrl = rootUrl + $"/{dataset}/graph2.0";
            var client = new HttpClient() as HttpClient;
            var response = await client.GetAsync(requestUrl);

            string str =  await response.Content.ReadAsStringAsync();
            int noPlots = 0;
            int.TryParse(str, out noPlots);

            List<string> responseUris = new List<string>(); 
            for (int i = 1; i <= noPlots; i++)
            {
                responseUris.Add(rootUrl + $"/{dataset}/graph2.0/{i}"); 
            }


            return responseUris; 

        }
    }
}