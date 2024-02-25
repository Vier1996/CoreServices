using ACS.Dialog.Dialogs.View;

namespace ACS.Dialog.Dialogs.Arguments
{
    public interface IReceiveArgs<in T> where T : DialogArgs
    {
        public DialogView SetArgs(T args);
    }
}
