using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace openai_semantickernel
{
    public class CognitiveSearchServices
    {
        private static string _cognitiveSearchServiceName = Env.Var("Azure_Cognitive_Services_Name");
        private static string _cognitiveSearchServiceKey = Env.Var("Azure_Cognitive_Services_Subscription_Key");

        static string searchServiceName = _cognitiveSearchServiceName;
        static string searchApiVersion = "2020-06-30-Preview";
        StringBuilder searchUrl = new StringBuilder($"https://{searchServiceName}.search.windows.net/indexes/?api-version={searchApiVersion}");
        static string searchApiKey = _cognitiveSearchServiceKey;

        public async Task<string> getListOfIndexes()
        {
            var clientforListofIndexes = new HttpClient();
            clientforListofIndexes.DefaultRequestHeaders.Add("api-key", searchApiKey);
            clientforListofIndexes.DefaultRequestHeaders.Add("ContentType", "application/json");
            searchUrl = new StringBuilder($"https://{searchServiceName}.search.windows.net/indexes/?api-version={searchApiVersion}");
            HttpResponseMessage responseIndexList = await clientforListofIndexes.GetAsync(searchUrl.ToString());
            string listOfIndexes = await responseIndexList.Content.ReadAsStringAsync();
            string cleanedListofIndexes = getCleanedJsonString(listOfIndexes);

            var jobject = JsonConvert.DeserializeObject(cleanedListofIndexes) as JObject;

            var indexList = (from t in jobject["value"]
                             select new
                             {
                                 name = t["name"].ToString(),
                             }).ToList();

            return string.Join(",", indexList);
        }

        public async Task<string> getSearchResult(string indexName, string searchQuestion, string searchType, string select)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", searchApiKey);
            string searchIndexName = indexName;
            searchUrl = new StringBuilder($"https://{searchServiceName}.search.windows.net/indexes/{indexName}/docs/search?api-version={searchApiVersion}");

            var searchJson = new
            {
                search = searchQuestion,
                queryType = searchType,
                queryLanguage = "en-us", // Required when using semantic search
                top = 10,
                speller = "lexicon",
            };

            // Perform the search in Azure Cognitive Search

            HttpResponseMessage response = await client.PostAsync(searchUrl.ToString(), new StringContent(JsonConvert.SerializeObject(searchJson), Encoding.UTF8, "application/json"));
            string searchResult = await response.Content.ReadAsStringAsync();

            return getCleanedJsonString(searchResult);
        }

        public string getCleanedJsonString(string jsonString)
        {
            string finalresult = "{\"" + jsonString.Substring(jsonString.IndexOf("value"));
            var jsonObject = (JObject)JsonConvert.DeserializeObject(finalresult);

            var replacedjsonobject = jsonObject.ToString().Replace("@search.", "");
            return replacedjsonobject.ToString();
        }
    }
}
