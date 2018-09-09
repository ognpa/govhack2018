using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eda.bot
{
    public class SearchDatasets
    {
        // https://govhack2018search.search.windows.net/indexes/documentdb-index/docs?api-version=2017-11-11&api-key=093625B8319F2E241444DD9861F69BB4&search=energy
        private readonly string searchServiceName = "govhack2018search"; //ConfigurationManager.AppSettings["SearchServiceName"];
        private readonly string queryApiKey = "093625B8319F2E241444DD9861F69BB4";//ConfigurationManager.AppSettings["SearchServiceQueryApiKey"];
        private readonly string searchIndexName = "documentdb-index";


        private static SearchIndexClient _indexClient = null;
        public async Task<DocumentSearchResult> GetAzSearchResultsAsync(string q)
        {
            if (_indexClient == null)
            {
                _indexClient = new SearchIndexClient(searchServiceName, searchIndexName, new SearchCredentials(queryApiKey));
            }

            var searchDocuments = await _indexClient.Documents.SearchAsync(q);

            //foreach (var result in searchDocuments.Results)
            //{
            //    Console.WriteLine($"{result.Score}");
            //}


            return searchDocuments;
        }
    }
}