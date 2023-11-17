using System.Text;
using device.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

var connectionString = "HostName=dureiothub.azure-devices.net;DeviceId=pctjorven;SharedAccessKey=MEilWKqs9WY0+KtgQ/UKJjIH5sfLfpY46AIoTCvyWFI=";

using var deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

await deviceClient.SetReceiveMessageHandlerAsync(ReceiveMessage, null);

async Task ReceiveMessage(Message message, object userContext)
{
    var messageData = Encoding.ASCII.GetString(message.GetBytes());
    Console.WriteLine("Received message: {0}", messageData);
    await deviceClient.CompleteAsync(message);
}
//// open connection explicitly
await deviceClient.OpenAsync();



while (true)
{
    await SendMessage();
    Thread.Sleep(5000);
}


async Task SendMessage()
{
    SensorData sensorData = new SensorData();
    sensorData.SensorValue = new Random().Next(0, 100);
    var json = JsonConvert.SerializeObject(sensorData);
    var message = new Message(Encoding.UTF8.GetBytes(json));

    await deviceClient.SendEventAsync(message);

    Console.WriteLine("A single message is sent");
}