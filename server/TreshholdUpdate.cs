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

namespace MCT.Function
{
    public static class UpdateTreshholdDevice
    {
        [FunctionName("UpdateTreshholdDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "update/treshhold/{deviceId}/{treshhold}")] HttpRequest req,
            string deviceId, int treshhold, ILogger log)
        {

            var iotHubAdmin = Environment.GetEnvironmentVariable("IotHubOwner");

            try
            {

                var registeryManager = RegistryManager.CreateFromConnectionString(iotHubAdmin);
                var twin = await registeryManager.GetTwinAsync(deviceId);
                twin.Properties.Desired["treshold"] = treshhold;
                await registeryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
                return new OkObjectResult($"Treshhold {treshhold} updated for device {deviceId}");
            }
            catch (Exception e)
            {
                log.LogInformation($"C# Something went wrong with updating treshhold: {e.Message}");
                return new BadRequestObjectResult($"Something went wrong with updating treshhold {treshhold} for device {deviceId}");
            }
        }
    }
}