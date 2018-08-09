using System;
using System.Linq;
using DistQueue.Shard.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.IO;

namespace DistQueue.Shard.Controllers.Tests
{
    public class QueuesControllerTests
    {
        [Fact]
        public void Test_Get_EmptyQueueList()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>()));
            var result = ctrl.Get();
            var model = Assert.IsType<OkObjectResult>(result);
            var collection = Assert.IsAssignableFrom<IEnumerable<string>>(model.Value);
            Assert.Empty(collection);
        }

        [Fact]
        public void Test_Get_QueueList()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test") }));
            var result = ctrl.Get();
            var model = Assert.IsType<OkObjectResult>(result);
            var collection = Assert.IsAssignableFrom<IEnumerable<string>>(model.Value);
            Assert.NotEmpty(collection);
        }

        [Fact]
        public void Test_Post_NullQueue()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>()));
            Assert.IsType<BadRequestObjectResult>(ctrl.Post(null));
        }

        [Fact]
        public void Test_Post_Success()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>()));
            var result = ctrl.Post("newQueue");
            var model = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("newQueue", model.Value.ToString());
        }

        [Fact]
        public void Test_Delete_NullQueue()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository());
            Assert.IsType<BadRequestObjectResult>(ctrl.Delete(null));
        }

        [Fact]
        public void Test_Delete_Success()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test0") }));
            var result = ctrl.Delete("test0");
            var model = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("test0", model.Value);
        }

        [Fact]
        public void Test_Put_NullQueue()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository());
            byte[] value = new byte[50];
            new Random().NextBytes(value);
            var result = ctrl.Put(null, Convert.ToBase64String(value));
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Test_Put_NullItem()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test0") }));
            var result = ctrl.Put("test0", null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Test_Put_Success()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test0") }));
            byte[] value = new byte[50];
            new Random().NextBytes(value);
            var result = ctrl.Put("test0", Convert.ToBase64String(value));
            var model = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<string>(model.Value);
        }

        [Fact]
        public void Test_Get_NullQueue()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test0") }));
            var result = ctrl.Get(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Test_Get_Success()
        {
            QueueController ctrl = new QueueController(new QueueShardRepository(new List<QueueShard<IQueueShardItem>>() { new QueueShard<IQueueShardItem>("test0") }));
            byte[] value = new byte[50];
            new Random().NextBytes(value);
            var result = ctrl.Put("test0", Convert.ToBase64String(value));
            var model = Assert.IsType<OkObjectResult>(result);
            var resultDeq = ctrl.Get("test0");
            var modelDeq = Assert.IsType<OkObjectResult>(resultDeq);
            Assert.IsAssignableFrom<IQueueShardItem>(modelDeq.Value);
        }
    }
}
