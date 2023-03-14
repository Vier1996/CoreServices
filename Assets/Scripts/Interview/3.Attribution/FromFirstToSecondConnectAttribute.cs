using System;

namespace Interview._3.Attribution
{
    public class FromFirstToSecondConnectAttribute : Attribute
    {
        public SecondEnum SecondEnumValue { get; }

        public FromFirstToSecondConnectAttribute(SecondEnum secondEnumValue) => 
            SecondEnumValue = secondEnumValue;
    }
}