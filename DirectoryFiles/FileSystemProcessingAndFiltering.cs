using System;
using System.IO;
using DirectoryFiles.Events;

namespace DirectoryFiles
{
    public class FileSystemProcessingAndFiltering : IFileSystemProcessingAndFiltering
    {
        public ActionType ProcessItemFinded<TItemInfo>(
            TItemInfo itemInfo,
            string filter,
            EventHandler<ItemFindedEvent<TItemInfo>> itemFinded,
            EventHandler<ItemFindedEvent<TItemInfo>> filteredItemFinded,
            Action<EventHandler<ItemFindedEvent<TItemInfo>>, ItemFindedEvent<TItemInfo>> eventEmitter)
            where TItemInfo : FileSystemInfo
        {
            ItemFindedEvent<TItemInfo> args = new ItemFindedEvent<TItemInfo>
            {
                FindedItem = itemInfo,
                ActionType = ActionType.ContinueSearch
            };
            eventEmitter(itemFinded, args);

            if (args.ActionType != ActionType.ContinueSearch || filter == null)
            {
                return args.ActionType;
            }

            if (filter != null)
            {
                args = new ItemFindedEvent<TItemInfo>
                {
                    FindedItem = itemInfo,
                    ActionType = ActionType.ContinueSearch
                };

                if (itemInfo.Name != null && !itemInfo.Name.Contains(filter))
                {
                    eventEmitter(filteredItemFinded, args);
                } 

                return args.ActionType;
            }

            return ActionType.SkipElement;
        }
    }
}
