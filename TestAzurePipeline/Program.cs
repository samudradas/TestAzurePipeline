using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Rest;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TestAzurePipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            // String values ----
            // Set variables    
            string tenantID = "e6fe8367-24af-48c2-a0ee-b5f386408512";
            string subscriptionId = "44357e6b-77a0-4b60-a817-27e62ffb6fdd";
            string applicationId = "0da14751-0058-4a91-ae16-b8722b2b74d1";
            string authenticationKey = "CbA5ryJyhbFi4_g/bmM]VM4-QynEOU1p";
            string resourceGroup = "RG-Guardian-Development";
            string region = "Central India";
            string dataFactoryName = "DataReplication-CRMtoODS-Test";
            string pipelineName = "DataReplication_AllegationMasterData";



            // Authenticate and create a data factory management client  
            var context = new AuthenticationContext("https://login.windows.net/" + tenantID);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred) { SubscriptionId = subscriptionId };
            Console.WriteLine("Creating Pipeline run...");
            CreateRunResponse runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(resourceGroup, dataFactoryName, pipelineName).Result.Body;
            client.Pipelines.CreateRun(resourceGroup, dataFactoryName, pipelineName);
            
            
            Console.WriteLine("Pipeline run ID: " + runResponse.RunId);

            Check Status of Azure Pipeline Runs
            // Monitor the Pipeline Run  
            Console.WriteLine("Checking Pipeline Run Status...");
            PipelineRun pipelineRun;
            while (true)
            {
                pipelineRun = client.PipelineRuns.Get(resourceGroup, dataFactoryName, runResponse.RunId);
                Console.WriteLine("Status: " + pipelineRun.Status);
                if (pipelineRun.Status == "InProgress")
                    System.Threading.Thread.Sleep(15000);
                else
                    break;
            }
        }
    }
}
