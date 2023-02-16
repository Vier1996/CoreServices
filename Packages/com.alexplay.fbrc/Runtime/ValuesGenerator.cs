#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text;
using ACS.FBRC.StaticData;
using UnityEngine;

namespace ACS.FBRC
{
    internal static class ValuesGenerator
    {
        private static readonly StringBuilder s_resultSource = new StringBuilder();
        private static FBRCConfig s_config;

        public static void Generate(FBRCConfig data)
        {
            s_config = data;
            s_resultSource.Clear();
            string path = $"{Application.dataPath}{data.GeneratedFilePath}/FBRCValues.cs";
            AppendHeader();
            InsertProperties(data);
            AppendFooter();
            System.IO.File.WriteAllText(path, s_resultSource.ToString());
        }

        private static void InsertProperties(FBRCConfig data)
        {
            foreach (FBRemoteConfigValue value in data.Values)
            {
                InsertSummary(value);
                switch (value.Type)
                {
                    case ValType.Bool:
                        InsertBool(value); 
                        break;
                    case ValType.Long:
                        InsertLong(value);
                        break;
                    case ValType.Double:
                        InsertDouble(value);
                        break;
                    case ValType.String:
                        InsertString(value);
                        break;
                    case ValType.Json:
                        InsertString(value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void InsertSummary(FBRemoteConfigValue value)
        {
            s_resultSource.Append("\t/// <summary>\n");
            s_resultSource.Append($"\t/// {value.Description}\n");
            s_resultSource.Append("\t/// </summary>\n");
        }

        private static void InsertBool(FBRemoteConfigValue value) => 
            s_resultSource.Append($"\tpublic static bool {GetName(value.Name)} => s_remoteConfig.GetValue(\"{value.Name}\").BooleanValue;\n");

        private static void InsertLong(FBRemoteConfigValue value) => 
            s_resultSource.Append($"\tpublic static long {GetName(value.Name)} => s_remoteConfig.GetValue(\"{value.Name}\").LongValue;\n");

        private static void InsertDouble(FBRemoteConfigValue value) => 
            s_resultSource.Append($"\tpublic static double {GetName(value.Name)} => s_remoteConfig.GetValue(\"{value.Name}\").DoubleValue;\n");

        private static void InsertString(FBRemoteConfigValue value) => 
            s_resultSource.Append($"\tpublic static string {GetName(value.Name)} => s_remoteConfig.GetValue(\"{value.Name}\").StringValue;\n");


        private static string GetName(string name)
        {
            if (s_config.SnakeToPascal == false) return name;

            var words = name.Split(new[] { "_",}, StringSplitOptions.RemoveEmptyEntries);
            words = words
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();
            return string.Join(string.Empty, words);
        }

        private static void AppendHeader()
        {
            s_resultSource.Append("public static class FBRCValues\n{\n");
            s_resultSource.Append("\tprivate static readonly Firebase.RemoteConfig.FirebaseRemoteConfig s_remoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;\n");
        }

        private static void AppendFooter() => 
            s_resultSource.Append("}");
    }
}
#endif