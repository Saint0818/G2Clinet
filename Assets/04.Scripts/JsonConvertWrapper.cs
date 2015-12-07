using Newtonsoft.Json;

public static class JsonConvertWrapper
{
    public static T DeserializeObject<T>(string jsonText)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new ForceJSONSerializePrivatesResolver()
        };
        return JsonConvert.DeserializeObject<T>(jsonText, settings);
    }
}
