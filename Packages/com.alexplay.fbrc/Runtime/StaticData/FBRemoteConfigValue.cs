using System;
using Sirenix.OdinInspector;

namespace ACS.FBRC.StaticData
{
    [Serializable]
    public struct FBRemoteConfigValue
    {
        public string Name;
        public ValType Type;
        public string Description;
        [ShowIf("@Type == ValType.Bool"), LabelText("DefaultValue")]
        public bool BoolValue;
        [ShowIf("@Type == ValType.Long"), LabelText("DefaultValue")]
        public long LongValue;
        [ShowIf("@Type == ValType.Double"), LabelText("DefaultValue")]
        public double DoubleValue;
        [ShowIf("@Type == ValType.String"), LabelText("DefaultValue")]
        public string StringValue;
        [ShowIf("@Type == ValType.Json"), LabelText("DefaultValue")]
        public string JsonValue;
    }
}