using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Interview._1.Reflection
{
    /*public class ReflectionTask : MonoBehaviour
    {
        private List<BaseReferenceModel> _referenceList = new List<BaseReferenceModel>();

        public T AddReference<T>(T referenceModel) where T : BaseReferenceModel
        {
            _referenceList.Add(referenceModel);
            
            return referenceModel;
        }

        [CanBeNull] public T FindReference<T>(int id) where T : BaseReferenceModel
        {
            
            return null;
        }
    }*/
    
    public class ReflectionTask : MonoBehaviour
    {
        private List<BaseReferenceModel> _referenceList = new List<BaseReferenceModel>();

        public T AddReference<T>(T referenceModel) where T : BaseReferenceModel
        {
            _referenceList.Add(referenceModel);
            
            return referenceModel;
        }

        [CanBeNull] public T FindReference<T>(int id) where T : BaseReferenceModel
        {
            foreach (var model in _referenceList)
            {
                PropertyInfo? idPropertyInfo = 
                    model
                        .GetType()
                        .GetProperties()
                        .FirstOrDefault(prop => prop.Name.Equals("CustomID"));

                if (idPropertyInfo == default)
                    throw new ArgumentException("Thats entity doesn't contain property [CustomID]");


                int currentId = (int)(idPropertyInfo.GetValue(model) ?? -1);
            
                if (currentId == id)
                    return (T)model;
            }

            return null;
        }
    }
}






























