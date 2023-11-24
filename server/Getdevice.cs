using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Shared;
using System.Data.SqlClient;

namespace MCT.Function
{
    public static class GetIOTDevices
    {
        [FunctionName("GetIOTDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var iotHubAdmin = Environment.GetEnvironmentVariable("IotHubOwner");
            try
            {

                var registeryManager = RegistryManager.CreateFromConnectionString(iotHubAdmin);
                var devices = registeryManager.CreateQuery("SELECT * FROM devices");
                List<Twin> twins = new List<Twin>();
                while (devices.HasMoreResults)
                {
                    var page = await devices.GetNextAsTwinAsync();
                    twins.AddRange(page);
                }

                return new OkObjectResult(twins);
            }
            catch
            {
                return new BadRequestObjectResult("Something went wrong with getting devices");
            }
        }

        [FunctionName("GetDeviceById")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/{deviceId}")] HttpRequest req,
            string deviceId,
            ILogger log)
        {
            log.LogInformation("Getting device...");

            var iotHubConnectionString = Environment.GetEnvironmentVariable("IotHubOwner");

            // Validate deviceId parameter
            if (string.IsNullOrWhiteSpace(deviceId))
                return new BadRequestObjectResult("Device id is required");


            try
            {
                var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
                var twin = await registryManager.GetTwinAsync(deviceId);

                return new OkObjectResult(twin);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error getting device: {ex.Message}");
            }
        }

    }


}