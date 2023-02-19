using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ToyBox
{
    public class Functions
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public Functions(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName(nameof(GitHubPrivateRepoFileFetcher))]
        public async Task<IActionResult> GitHubPrivateRepoFileFetcher(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. RequestUri={req.Path}");

            string GitHubUri = req.Query["githuburi"];
            string GitHubAccessToken = req.Query["githubaccesstoken"];

            log.LogInformation($"GitHubPrivateRepoFileFecher function is trying to get file content from {GitHubUri}");

            Encoding outputencoding = Encoding.GetEncoding("ASCII");

            if (GitHubUri == null)
            {
                var errorcontent = new StringContent("Please pass the GitHub raw file content URI (raw.githubusercontent.com) in the request URI string", outputencoding);
                return new BadRequestObjectResult("Please pass the GitHub raw file content URI (raw.githubusercontent.com) in the request URI string");
            }
            else if (GitHubAccessToken == null)
            {
                var errorcontent = new StringContent("Please pass the GitHub personal access token in the request URI string", outputencoding);
                return new BadRequestObjectResult("Please pass the GitHub personal access token in the request URI string");
            }

            string strAuthHeader = "token " + GitHubAccessToken;
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3.raw");
            client.DefaultRequestHeaders.Add("Authorization", strAuthHeader);
            HttpResponseMessage response = await client.GetAsync(GitHubUri);
            var prefecturesJsonString = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(prefecturesJsonString);
        }
    }
}
