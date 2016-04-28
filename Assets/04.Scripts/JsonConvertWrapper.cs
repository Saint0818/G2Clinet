using Newtonsoft.Json;

public static class JsonConvertWrapper
{
    private static JsonSerializerSettings jsonSetting = null;

    public static T DeserializeObject<T>(string jsonText)
    {
        if (jsonSetting == null) {
            jsonSetting = new JsonSerializerSettings {
                ContractResolver = new ForceJSONSerializePrivatesResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        return JsonConvert.DeserializeObject<T>(jsonText, jsonSetting);
    }
}
