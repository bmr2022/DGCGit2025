using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace etactwebBOT.Services
{
    public class WrenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "sk-aysm3Nq1heSZsQ";  // Replace with your actual API key

        public WrenAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GenerateSqlAsync(string question, string metadataJson)
        {
            var requestBody = new
            {
                projectId = 10028,  // Replace with your Wren AI project ID
                question = question,
                manifestStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(metadataJson))
            };

            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://cloud.getwren.ai/api/v1/generate_sql", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseJson);

            return result.sql;
        }
    }
}
