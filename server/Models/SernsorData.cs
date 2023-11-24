using System;
using Newtonsoft.Json;

namespace device.Models;
public class SensorData
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("deviceId")]
    public string DeviceId { get; set; }
    [JsonProperty("sensorValue")]
    public int SensorValue { get; set; }


}