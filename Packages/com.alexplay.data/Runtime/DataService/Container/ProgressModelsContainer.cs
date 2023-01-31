using System;
using System.Collections.Generic;
using ACS.Data.DataService.Model;

namespace ACS.Data.DataService.Container
{
    public class ProgressModelsContainer : IProgressModelContainer
    {
        private List<ProgressModel> _models;
        public ProgressModelsContainer(List<ProgressModel> models) => _models = models;

        public TModel Resolve<TModel>() where TModel : ProgressModel
        {
            ProgressModel model = null;
            Type modelType = null;
            Type demandedType = typeof(TModel);
            
            for (int i = 0; i < _models.Count; i++)
            {
                model = _models[i];
                modelType = model.GetType();
                
                if(modelType == demandedType)
                    return (TModel)model;
            }

            throw new ArgumentException($"Model with type of {demandedType} not present in container.");
        }
        
        public List<ProgressModel> GetAll() => _models;
    }
}
