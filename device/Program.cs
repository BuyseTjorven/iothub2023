using System.Text;
using device.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;


var connectionString = "HostName=dureiothub.azure-devices.net;DeviceId=pctjorven;SharedAccessKey=MEilWKqs9WY0+KtgQ/UKJjIH5sfLfpY46AIoTCvyWFI=";

var treshold = 50;
using var deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

await deviceClient.SetReceiveMessageHandlerAsync(ReceiveMessage, null);

await deviceClient.SetReceiveMessageHandlerAsync(ReceiveMessage, null);
await ForceDeviceTwinRetrieval(deviceClient);
await deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null);

var reportedProperties = new TwinCollection
{
    ["BootTime"] = DateTime.Now,
    ["Battery"] = "45%"
};

await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);

async Task ReceiveMessage(Message message, object userContext)
{
    var messageData = Encoding.ASCII.GetString(message.GetBytes());
    Console.WriteLine("Received message: {0}", messageData);
    await deviceClient.CompleteAsync(message);
}
//// open connection explicitly
await deviceClient.OpenAsync();

async Task ForceDeviceTwinRetrieval(DeviceClient deviceClient)
{
    var twin = await deviceClient.GetTwinAsync();

    await OnDesiredPropertyChanged(twin.Properties.Desired, deviceClient);

    Console.WriteLine("The Devicetwin is forced retrieved");
}


async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
{
    Console.WriteLine("One or more device twin desired properties changed:");
    Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));
    Console.WriteLine(desiredProperties["treshold"]);
    treshold = desiredProperties["treshold"];
    Console.WriteLine($"Treshold is set to {treshold}");

}


while (true)
{
    await SendMessage();
    //Thread.Sleep(5000);
    await Task.Delay(5000);
}


async Task SendMessage()
{

    SensorData sensorData = new()
    {
        SensorValue = new Random().Next(0, 100),
        DeviceId = "pcibe"
    };
    if (sensorData.SensorValue > treshold)
    {


        var jsonData = JsonConvert.SerializeObject(sensorData);
        var bytes = Encoding.UTF8.GetBytes(jsonData);

        var message = new Message(bytes);

        await deviceClient.SendEventAsync(message);

        Console.WriteLine($"A single message is send{sensorData.SensorValue}");
    }
    else
    {
        Console.WriteLine($"No message is send{sensorData.SensorValue}");
    }
}