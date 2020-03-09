using System.IO;

namespace DirectoryFiles.Events
{
    public class ItemFindedEvent<T> : System.EventArgs
        where T : FileSystemInfo
    {
        public T FindedItem { get; set; }
        public ActionType ActionType { get; set; }


    }
}
