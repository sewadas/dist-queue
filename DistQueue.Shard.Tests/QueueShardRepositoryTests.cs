using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DistQueue.Shard.Model.Tests
{
    public class QueueShardRepositoryTests
    {
        [Fact]
        public void Test_List_NotInitialized()
        {
            var repo = new QueueShardRepository(null);
            Assert.False(repo.List().Any());
        }

        [Fact]
        public void Test_List_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.List();
            Assert.True(model.Any());
            Assert.Equal("test", model.First());
        }

        [Fact]
        public void Test_AddQueue_Duplicate()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            Assert.Throws<ArgumentException>(() => repo.AddQueue("test"));
        }

        [Fact]
        public void Test_AddQueue_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.AddQueue("test0");
            Assert.Equal("test0", model);
        }

        [Fact]
        public void Test_DeleteQueue_NotExisting()
        {
            var repo = new QueueShardRepository(null);
            Assert.Throws<ArgumentException>(() => repo.DeleteQueue("test"));
        }

        [Fact]
        public void Test_DeleteQueue_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.DeleteQueue("test");
            Assert.Equal("test", model);
        }

        [Fact]
        public void Test_Enqueue_NotExistingQueue()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            Assert.Throws<ArgumentException>(() => repo.Enqueue("test0", new byte[1024]));
        }

        [Fact]
        public void Test_Enqueue_EmptyData()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            Assert.Throws<ArgumentException>(() => repo.Enqueue("test", null));
        }

        [Fact]
        public void Test_Enqueue_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.Enqueue("test", new byte[1024]);
            Assert.True(Guid.TryParse(model, out Guid result));
        }

        [Fact]
        public void Test_Dequeue_NotExistingQueue()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            Assert.Throws<ArgumentException>(() => repo.Dequeue("test0"));
        }

        [Fact]
        public void Test_Dequeue_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            repo.Enqueue("test", new byte[1024]);
            var model = repo.Dequeue("test");
            Assert.NotNull(model);
        }

        [Fact]
        public void Test_DeleteItem_NotExistingQueue()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.Enqueue("test", new byte[1024]);
            Assert.NotNull(model);
            var modelDeq = repo.Dequeue("test");
            Assert.NotNull(modelDeq);
            Assert.Throws<ArgumentException>(() => repo.DeleteItem("test0", modelDeq.ID));
        }

        [Fact]
        public void Test_DeleteItem_InvalidItem()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.Enqueue("test", new byte[1024]);
            Assert.NotNull(model);
            var modelDeq = repo.Dequeue("test");
            Assert.NotNull(modelDeq);
            Assert.Throws<ArgumentException>(() => repo.DeleteItem("test", null));
        }

        [Fact]
        public void Test_DeleteItem_Success()
        {
            var repo = new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") });
            var model = repo.Enqueue("test", new byte[1024]);
            Assert.NotNull(model);
            var modelDeq = repo.Dequeue("test");
            Assert.NotNull(modelDeq);
            Assert.Equal(repo.DeleteItem("test", modelDeq.ID), modelDeq.ID);
        }
    }
}
