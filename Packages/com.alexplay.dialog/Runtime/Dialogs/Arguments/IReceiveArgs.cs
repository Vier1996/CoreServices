namespace ACS.Dialog.Dialogs.Arguments
{
    public interface IReceiveArgs<in T> where T : DialogArgs
    {
        public void SetArgs(T args);
    }
}
