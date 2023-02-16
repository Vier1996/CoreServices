using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ACS.FBRC.StaticData
{
    public class FBRCParser
    {
        internal static List<FBRemoteConfigValue> Parse(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is empty");
            string json = System.IO.File.ReadAllText(path);
            var parsedFile = JsonConvert.DeserializeObject<ConfigFile>(json);

            List<FBRemoteConfigValue> parameters = new List<FBRemoteConfigValue>();

            foreach (var item in parsedFile.parameters)
            {
                parameters.Add(GetConfigValue(item));
            }

            return parameters;
        }

        private static FBRemoteConfigValue GetConfigValue(KeyValuePair<string, ConfigItem> item)
        {
            FBRemoteConfigValue result = new FBRemoteConfigValue {Name = item.Key, Description = item.Value.description};
            if (item.Value.defaultValue.TryGetValue("useInAppDefault", out string useInAppDefault) &&
                useInAppDefault == "true")
                return result;
            switch (item.Value.valueType)
            {
                case "BOOLEAN":
                    result.Type = ValType.Bool;
                    result.BoolValue = bool.Parse(item.Value.defaultValue["value"]);
                    break;
                case "NUMBER":
                    result.Type = item.Value.defaultValue["value"].Contains(".") ? ValType.Double : ValType.Long;
                    if (result.Type == ValType.Long)
                        result.LongValue = long.Parse(item.Value.defaultValue["value"]);
                    else
                        result.DoubleValue = double.Parse(item.Value.defaultValue["value"].Replace('.', ','));
                    break;
                case "STRING":
                    result.Type = ValType.String;
                    result.StringValue = item.Value.defaultValue["value"];
                    break;
                case "JSON":
                    result.Type = ValType.Json;
                    result.JsonValue = item.Value.defaultValue["value"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
    
    public struct ConfigFile
    {
        [JsonProperty] public Dictionary<string, ConfigItem> parameters;
        [JsonProperty] public ConfigVersion version;
    }

    public struct ConfigItem
    {
        [JsonProperty] public Dictionary<string, string> defaultValue;
        [JsonProperty] public string description;
        [JsonProperty] public string valueType;
    }

    public struct ConfigVersion
    {
        [JsonProperty] public int versionNumber;
        [JsonProperty] public string updateTime;
        [JsonProperty] public Dictionary<string, string> updateUser;
    }
}