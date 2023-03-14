using System;
using System.Linq;
// ReSharper disable once HeapView.BoxingAllocation
namespace Interview._3.Attribution
{
    public class UsingTask
    {
        private SecondEnum _secondEnum;
        
        public UsingTask(FirstEnum firstEnum)
        {
            _secondEnum = GetSecondAttribute<FromFirstToSecondConnectAttribute>(e: firstEnum).SecondEnumValue;
        }

        private T GetSecondAttribute<T>(Enum e) where T : Attribute
        {
            Type type = e.GetType();
            string name = Enum.GetName(type, e);
            
            return type
                .GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }
    }
}