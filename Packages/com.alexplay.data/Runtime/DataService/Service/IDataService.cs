using ACS.Data.DataService.Container;

namespace ACS.Data.DataService.Service
{
    public interface IDataService
    {
        public IProgressModelContainer Models { get; set; }
    }
}