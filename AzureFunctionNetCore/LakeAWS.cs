using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Gdo.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Amazon.Glue;
using System.Net.Http.Headers;
using System.Net;

namespace Meetup.Function
{
    public static class LakeAWS
    {


        [FunctionName("GlueCatalog-ListDatabases")]
        public static async Task<IActionResult> GetGlueDatabases([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AwsCred awsCred = JsonConvert.DeserializeObject<AwsCred>(requestBody);
                AmazonGlueClient c = new AmazonGlueClient(awsCred.awsAccessKeyId, awsCred.awsSecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(awsCred.region));
                var databases = await c.GetDatabasesAsync(new Amazon.Glue.Model.GetDatabasesRequest());
                return new OkObjectResult(databases.DatabaseList);
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.StackTrace);
            }
           
        }
        [FunctionName("GlueCatalog-ListTables")]
        public static async Task<IActionResult> GetGlueTables([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            ILogger log)
        {
            try
            {
                string database = req.Query["database"];
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AwsCred awsCred = JsonConvert.DeserializeObject<AwsCred>(requestBody);
                AmazonGlueClient c = new AmazonGlueClient(awsCred.awsAccessKeyId, awsCred.awsSecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(awsCred.region));

                var _tableRequest = new Amazon.Glue.Model.GetTablesRequest();
                _tableRequest.DatabaseName = database;
                var tables = await c.GetTablesAsync(_tableRequest);
                return new OkObjectResult(tables.TableList);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }

        }
    }
}
