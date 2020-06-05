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

namespace Gdo.Function
{
    public static class LakeAWS
    {


        [FunctionName("UploadAzureBlobFileToAWS")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Entering the function");
            HttpClient client = new HttpClient();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            FlowQuery flowQuery = JsonConvert.DeserializeObject<FlowQuery>(requestBody);
            client.DefaultRequestHeaders.Add("x-api-key",flowQuery.destination.xapikey);

            //Retrieving companion flow id from source flow definition
            flowQuery.destination.contract.flow = new Flow { id = flowQuery.source.companion.data_schema.name};
            

            BlobServiceClient blobServiceClient = new BlobServiceClient(flowQuery.source.connectionString);
            
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(flowQuery.source.container);

            BlobClient blobclient = containerClient.GetBlobClient(flowQuery.source.filePath);
            
            

            
            //Retrieve Signed URL
             log.LogInformation("Retrieving the signed URL from AWS");
             var response = await RetrieveSignedUrl(client ,flowQuery);
            log.LogInformation("Signed URL retrieved");
            //If we don't succeed to retrieve the signed URL 
            if(response.StatusCode != HttpStatusCode.OK)
            {
                SignedUrlErrorContent signedUrlErrorContent = await response.Content.ReadAsAsync<SignedUrlErrorContent>();
                if(response.StatusCode  == HttpStatusCode.BadRequest && signedUrlErrorContent.message.Contains("flow must be created"))
                {
                    response = await InitFlow(client,flowQuery);
                    if(response.StatusCode != HttpStatusCode.Created)
                    {
                        return new BadRequestObjectResult(response.Content.ReadAsAsync<object>());
                    }
                    else
                    {
                        response = await RetrieveSignedUrl(client,flowQuery);
                    }
                }
                else
                {
                    return new BadRequestObjectResult(signedUrlErrorContent);
                }
                
            }
            
            var signedUrlContent = await response.Content.ReadAsAsync<SignedUrlContent>();
            log.LogInformation("Loading blob");

            //Upload Blob
            BlobDownloadInfo download = await blobclient.DownloadAsync();
            
            using (MemoryStream downloadMemoryStream = new MemoryStream())
            {
                //Evolution needed in order to paginate the result to avoid high memory consumtion
                await download.Content.CopyToAsync(downloadMemoryStream);
                downloadMemoryStream.Position=0;
                var sc = new StreamContent(downloadMemoryStream);
                sc.Headers.ContentType = new MediaTypeHeaderValue(signedUrlContent.signedUrlHeaders.contentType);
                sc.Headers.Add("x-amz-meta-companion",signedUrlContent.signedUrlHeaders.xAmzMetaCompanion);
                sc.Headers.Add("x-amz-server-side-encryption",signedUrlContent.signedUrlHeaders.xAmzServerSideEncryption);
                sc.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id",signedUrlContent.signedUrlHeaders.xAmzServerSideEncryptionAwsKmsKeyId);
                sc.Headers.Add("x-amz-tagging",signedUrlContent.signedUrlHeaders.xAmzTagging);
                client.DefaultRequestHeaders.Add("host",signedUrlContent.signedUrlHeaders.host);
                //sc.Headers.Add("host",signedUrlContent.signedUrlHeaders.host);
                //"http://localhost:7071/api/test"
                log.LogInformation("Sending blob to AWS");
                response = await client.PutAsync(signedUrlContent.url,sc);
                log.LogInformation("Blob sended to AWS");
                downloadMemoryStream.Close();
                
            }  

            
            
            return new OkObjectResult(response.Content.ReadAsAsync<Object>());
        }
        

        public static async Task<HttpResponseMessage> RetrieveSignedUrl(HttpClient client, FlowQuery flowQuery)
        {
            //Retrieve Signed URL
            string signatureUrl = flowQuery.destination.apiurl + "/" + flowQuery.destination.stage + "/api/v1/jobs";

            var content = new StringContent(JsonConvert.SerializeObject(flowQuery.destination.contract));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

             return await client.PostAsync(signatureUrl, 
                content
                ); 
        }
        public static async Task<HttpResponseMessage> InitFlow(HttpClient client, FlowQuery flowQuery)
        {
            //Retrieve Signed URL
            string signatureUrl = flowQuery.destination.apiurl + "/" + flowQuery.destination.stage + "/api/v1/flows/"+ flowQuery.source.companion.data_schema.name;
            
            var titi = JsonConvert.SerializeObject(flowQuery.source.companion);
            var content = new StringContent(JsonConvert.SerializeObject(flowQuery.source.companion));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

             return await client.PutAsync(signatureUrl, 
                content
                ); 
        }   

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
