using DirectoryFiles;
using NUnit.Framework;
using System.IO;
using Moq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace Modul_1_Epam_DirectoryFiles.Tests
{
    [TestClass]
    public class Modul_1_Epam_DirectoryFilesTests
    {
        private IFileSystemProcessingAndFiltering _strategy;
        private Mock<FileSystemInfo> _fileSystemInfoMock;

        [SetUp]
        public void TestInit()
        {
            _strategy = new FileSystemProcessingAndFiltering();
            _fileSystemInfoMock = new Mock<FileSystemInfo>();
        }
        private void OnEvent<TArgs>(EventHandler<TArgs> someEvent, TArgs args)
        {
            someEvent?.Invoke(this, args);
        }

        [Test]
        public void ItemFindedCall()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            _strategy.ProcessItemFinded(fileSystemInfo, null, (s, e) => delegatesCallCount++, null, OnEvent);

            Assert.AreEqual(1, delegatesCallCount);
        }
        [Test]
        public void FilteredItemFindedCall()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            _strategy.ProcessItemFinded(
                fileSystemInfo, "i", (s, e) => delegatesCallCount++, (s, e) => delegatesCallCount++, OnEvent);

            Assert.AreEqual(1, delegatesCallCount);
        }

        [Test]
        public void ItemNotPassFilter()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            _strategy.ProcessItemFinded(
                fileSystemInfo, null, (s, e) => delegatesCallCount++, (s, e) => delegatesCallCount++, OnEvent);

            Assert.AreEqual(1, delegatesCallCount);
        }

        [Test]
        public void ItemFinded_ContinueSearchAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;

            var action = _strategy.ProcessItemFinded(fileSystemInfo, null, (s, e) => { }, null, OnEvent);

            Assert.AreEqual(ActionType.ContinueSearch, action);
        }

        [Test]
        public void FilteredItemFinded_ContinueSearchAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;

            var action = _strategy.ProcessItemFinded(
                fileSystemInfo, "i", (s, e) => { }, (s, e) => { }, OnEvent);

            Assert.AreEqual(ActionType.ContinueSearch, action);
        }

        [Test]
        public void FindedItemSkipped_SkipElementAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            var action = _strategy.ProcessItemFinded(
                fileSystemInfo, "i", (s, e) =>
                {
                    delegatesCallCount++;
                    e.ActionType = ActionType.SkipElement;
                }, (s, e) => delegatesCallCount++, OnEvent);

            Assert.AreEqual(ActionType.SkipElement, action);
            Assert.AreEqual(1, delegatesCallCount);
        }

        [Test]
        public void FindedItemStopped_StopSearchAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            var action = _strategy.ProcessItemFinded(
                fileSystemInfo, "i", (s, e) =>
                {
                    delegatesCallCount++;
                    e.ActionType = ActionType.StopSearch;
                }, (s, e) => delegatesCallCount++, OnEvent);

            Assert.AreEqual(ActionType.StopSearch, action);
            Assert.AreEqual(1, delegatesCallCount);
        }
    }
}
