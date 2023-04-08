using System;
using System.Collections.Generic;
using ACS.Data.DataService.Model;

namespace ACS.Data.DataService.Container
{
    public class ProgressModelsContainer : IProgressModelContainer
    {
        public Dictionary<Type, ProgressModel> Models => _models;
        
        private Dictionary<Type, ProgressModel> _models;
        public ProgressModelsContainer(Dictionary<Type, ProgressModel> models) => _models = models;

        public TModel Resolve<TModel>() where TModel : ProgressModel
        {
            Type demandedType = typeof(TModel);

            if (_models.ContainsKey(demandedType))
                return (TModel)_models[demandedType];
            
            throw new ArgumentException($"Model with type of {demandedType} not present in container.");
        }
    }
}
