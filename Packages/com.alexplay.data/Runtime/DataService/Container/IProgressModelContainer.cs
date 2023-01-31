using ACS.Data.DataService.Model;

namespace ACS.Data.DataService.Container
{
    public interface IProgressModelContainer
    {
        public TModel Resolve<TModel>() where TModel : ProgressModel;
    }
}