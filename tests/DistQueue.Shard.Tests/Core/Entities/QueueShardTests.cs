using System;
using DistQueue;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DistQueue.Shard.Model.Tests
{
    public class QueueShardTests
    {
        [Fact]
        public void Test_Dequeue_ExceedMaxDequeueAttempts()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test", TimeSpan.FromMilliseconds(1), 2);
            queue.Enqueue(new QueueShardItem("test", new byte[64]));
            Assert.True(queue.Count == 1);
            queue.Dequeue();
            Assert.True(queue.Count == 0);
            queue.ReclaimVisibility();
            Assert.NotNull(queue.Dequeue());
            queue.ReclaimVisibility();
            Assert.Null(queue.Dequeue());
        }

        [Fact]
        public void Test_Dequeue_ConfirmDeletion()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test", TimeSpan.FromMilliseconds(1));
            queue.Enqueue(new QueueShardItem("test", new byte[64]));
            Assert.True(queue.Count == 1);
            var item = queue.Dequeue();
            Assert.NotNull(item);
            Assert.True(queue.Count == 0);
            queue.Delete(item.ID);
            queue.ReclaimVisibility();
            Assert.True(queue.Count == 0);
        }

        [Fact]
        public void Test_Dequeue_Empty()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            Assert.Null(queue.Dequeue());
        }

        [Fact]
        public void Test_Dequeue_WhenItemInvisible()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            queue.Enqueue(new QueueShardItem("test", new byte[64]));
            Assert.NotNull(queue.Dequeue());
            Assert.Null(queue.Dequeue());
        }

        [Fact]
        public void Test_Dequeue_WhenItemVisible()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            queue.Enqueue(new QueueShardItem("test", new byte[64]));
            Assert.NotNull(queue.Dequeue());
        }

        [Fact]
        public void Test_Enqueue_SetVisibilityTimeout_IsVisible()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            queue.Enqueue(new QueueShardItem("test", new byte[64]));
            Assert.True(queue.Count == 1);
        }

        [Fact]
        public void Test_Enqueue_NullItem_ThrowsArgumentNullException()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
        }

        [Fact]
        public void Test_Delete_EmptyName_ThrowsArgumentException()
        {
            QueueShard<QueueShardItem> queue = new QueueShard<QueueShardItem>("test");
            Assert.Throws<ArgumentException>(() => queue.Delete(string.Empty));
        }

        [Fact]
        public void Test_IsVisible_False()
        {
            QueueShardItem queue = new QueueShardItem("test", new byte[64]);
            queue.SetVisibilityTimeout(TimeSpan.FromMinutes(5));
            Assert.False(queue.IsVisible());
        }

        [Fact]
        public void Test_IsVisible_True()
        {
            QueueShardItem queue = new QueueShardItem("test", new byte[64]);
            Assert.True(queue.IsVisible());
        }
    }
}
