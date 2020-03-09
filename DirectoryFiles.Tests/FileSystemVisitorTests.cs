﻿using DirectoryFiles;
using NUnit.Framework;
using System.IO;
using Moq;
using System;

namespace Modul_1_Epam_DirectoryFiles.Tests
{
    [TestFixture]
    public class FileSystemVisitorTests
    {
        private IFileSystemProcessingAndFiltering _strategy;
        private Mock<FileSystemInfo> _fileSystemInfoMock;

        [SetUp]
        public void TestInit()
        {
            _strategy = new FileSystemProcessingAndFiltering();
            _fileSystemInfoMock = new Mock<FileSystemInfo>();
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

            Assert.AreEqual(2, delegatesCallCount);
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
        public void FilteredFindedItemSkipped_SkipElementAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            var action = _strategy.ProcessItemFinded(
                fileSystemInfo, "i",
                (s, e) => delegatesCallCount++,
                (s, e) =>
                {
                    delegatesCallCount++;
                    e.ActionType = ActionType.SkipElement;
                }, OnEvent);

            Assert.AreEqual(ActionType.SkipElement, action);
            Assert.AreEqual(2, delegatesCallCount);
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

        [Test]
        public void FilteredFindedItemStopped_StopSearchAction()
        {
            FileSystemInfo fileSystemInfo = _fileSystemInfoMock.Object;
            int delegatesCallCount = 0;

            var action = _strategy.ProcessItemFinded(
                fileSystemInfo, "i",
                (s, e) => delegatesCallCount++,
                (s, e) =>
                {
                    delegatesCallCount++;
                    e.ActionType = ActionType.StopSearch;
                }, OnEvent);

            Assert.AreEqual(ActionType.StopSearch, action);
            Assert.AreEqual(2, delegatesCallCount);
        }

        private void OnEvent<TArgs>(EventHandler<TArgs> someEvent, TArgs args)
        {
            someEvent?.Invoke(this, args);
        }

    }
}
