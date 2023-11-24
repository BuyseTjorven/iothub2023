using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using device.Models;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace Howest.MCT
{
    public class DeviceMessageTrigger
    {

        [FunctionName("DeviceMessageTrigger")]
        public async Task Run([IoTHubTrigger("messages/events", Connection = "EventHubEndpoint")] EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            string cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");

            var data = JsonConvert.DeserializeObject<SensorData>(Encoding.UTF8.GetString(message.Body.Array));
            data.Id = Guid.NewGuid();

            var clientOptions = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            };



            CosmosClient cosmosClient = new(cosmosConnectionString, clientOptions);
            var database = cosmosClient.GetDatabase("iothub");
            var container = database.GetContainer("device");

            await container.CreateItemAsync(data);
        }
    }
}