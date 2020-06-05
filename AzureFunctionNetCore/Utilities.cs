using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Services.AppAuthentication;

namespace Company.Function
{
    public static class Utilities
    {
        [FunctionName("GetBearerFromMSI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetBearerFromMSI/{resource}")] HttpRequest req,
            ILogger log, string resource)
        {
            try
            {
                var t =await GetAccessTokenAsync(resource);
                return new OkObjectResult(t);
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
        }
        public static async Task<string> GetAccessTokenAsync(string resource)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(resource);
            return accessToken;
        }
    }
}
