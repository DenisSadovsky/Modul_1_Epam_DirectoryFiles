using System;
using System.IO;
using DirectoryFiles.Events;

namespace DirectoryFiles
{
    public interface IFileSystemProcessingAndFiltering
    {
        ActionType ProcessItemFinded<TItemInfo>(
            TItemInfo itemInfo,
            string filter,
            EventHandler<ItemFindedEvent<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEvent<TItemInfo>> filteredItemFinded,
            Action<EventHandler<ItemFindedEvent<TItemInfo>>, ItemFindedEvent<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo;
    }
}
