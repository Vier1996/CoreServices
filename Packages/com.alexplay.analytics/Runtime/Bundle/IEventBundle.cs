namespace ACS.Analytics.Bundle
{
    public interface IEventBundle
    {
        public IEventBundle AddParam(string paramName, object paramValue);
        public void TrackOnce();
        public void Track();
    }
}