using System;
using System.Collections.Generic;
using System.IO;
using DirectoryFiles.Events;

namespace DirectoryFiles
{
    class FileSystemVisitor
    {
        string _filter;
        private readonly DirectoryInfo _startDirectory;
        private readonly FileSystemProcessingAndFiltering _fileSystemProcessingAndFiltering;

        public event EventHandler<StartEvent> Start;
        public event EventHandler<FinishEvent> Finish;
        public event EventHandler<ItemFindedEvent<FileInfo>> FileFinded;
        public event EventHandler<ItemFindedEvent<FileInfo>> FilteredFileFinded;
        public event EventHandler<ItemFindedEvent<DirectoryInfo>> DirectoryFinded;
        public event EventHandler<ItemFindedEvent<DirectoryInfo>> FilteredDirectoryFinded;

        public FileSystemVisitor(DirectoryInfo startDirectory, FileSystemProcessingAndFiltering fileSystemProcessingAndFiltering)
        {
            _startDirectory = startDirectory;
            _fileSystemProcessingAndFiltering = fileSystemProcessingAndFiltering;
        }
        public FileSystemVisitor(DirectoryInfo path, FileSystemProcessingAndFiltering fileSystemProcessingAndFiltering, string filter) 
        : this(path, fileSystemProcessingAndFiltering)
        {
            _filter = filter;
        }

        public IEnumerable<FileSystemInfo> GetFileSystemInfoSequence()
        {
            OnEvent(Start, new StartEvent());
            foreach (var fileSystemInfo in BypassFileSystem(_startDirectory, CurrentAction.ContinueSearch))
            {
                yield return fileSystemInfo;
            }
            OnEvent(Finish, new FinishEvent());
        }

        private IEnumerable<FileSystemInfo> BypassFileSystem(DirectoryInfo directory, CurrentAction currentAction)
        {
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                if (fileSystemInfo is FileInfo file)
                {
                    currentAction.Action = ProcessFile(file);
                }

                if (fileSystemInfo is DirectoryInfo dir)
                {
                    currentAction.Action = ProcessDirectory(dir);
                    if (currentAction.Action == ActionType.ContinueSearch)
                    {
                        yield return dir;
                        foreach (var innerInfo in BypassFileSystem(dir, currentAction))
                        {
                            yield return innerInfo;
                        }
                        continue;
                    }
                }

                if (currentAction.Action == ActionType.StopSearch)
                {
                    yield break;
                }

                yield return fileSystemInfo;
            }
        }

        private ActionType ProcessFile(FileInfo file)
        {
            return _fileSystemProcessingAndFiltering
                .ProcessItemFinded(file, _filter, FileFinded, FilteredFileFinded, OnEvent);
        }

        private ActionType ProcessDirectory(DirectoryInfo directory)
        {
            return _fileSystemProcessingAndFiltering
                .ProcessItemFinded(directory, _filter, DirectoryFinded, FilteredDirectoryFinded, OnEvent);
        }

        private void OnEvent<TArgs>(EventHandler<TArgs> someEvent, TArgs args)
        {
            someEvent?.Invoke(this, args);
        }

        private class CurrentAction
        {
            public ActionType Action { get; set; }
            public static CurrentAction ContinueSearch
                => new CurrentAction { Action = ActionType.ContinueSearch };
        }
    }
}
