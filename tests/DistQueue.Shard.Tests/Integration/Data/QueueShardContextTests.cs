using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DistQueue.Shard.Model.Tests
{
    public class QueueShardContextTests
    {
        [Fact]
        public void Test_Initialize_NullOrEmptyStoragePath()
        {
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings()
            {
                StoragePath = null
            });
            Assert.Throws<ArgumentException>(() => new QueueShardContext(settings));
            settings = Options.Create<QueueShardSettings>(new QueueShardSettings()
            {
                StoragePath = string.Empty
            });
            Assert.Throws<ArgumentException>(() => new QueueShardContext(settings));
        }

        [Fact]
        public void Test_Load_QueuesWhenStorageEmpty()
        {
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());
            settings.Value.StoragePath += "\\emptyStorage";
            Directory.CreateDirectory(settings.Value.StoragePath);
            Assert.Empty(new QueueShardContext(settings).Queues);
        }

        [Fact]
        public void Test_Load_InvalidFilename()
        {
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());

            var ctx = new QueueShardContext(settings);
            Assert.Throws<ArgumentException>(() => ctx.Load("invalidFileName"));
        }

        [Fact]
        public void Test_Load_NullOrEmptyFileName()
        {
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());
            var ctx = new QueueShardContext(settings);
            Assert.Throws<ArgumentNullException>(() => ctx.Load(null));
            Assert.Throws<ArgumentNullException>(() => ctx.Load(string.Empty));
        }

        [Fact]
        public void Test_Load_QueuesWhenStorageCorrupted()
        {
            File.Delete("testCorrupt.bin");
            File.WriteAllText("testCorrupt.bin", "corruptedContent");
            
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());
            var ctx = new QueueShardContext(settings);
            Assert.Null(ctx.Queues.Find(o => string.Equals(o.Name, "testCorrupt")));
            Assert.True(File.Exists("testCorrupt.bin.bak"));
        }

        [Fact]
        public void Test_Save_And_Load_Queue()
        {
            File.Delete("test.bin");
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());
            var ctx = new QueueShardContext(settings);
            ctx.Save(new QueueShard<IQueueShardItem>("test", queueItems: new List<QueueShardItem>() { new QueueShardItem("testItem", Encoding.UTF8.GetBytes("testString")) }));

            ctx = new QueueShardContext(settings);
            Assert.Equal("test", ctx.Queues[0].Name);
        }

        [Fact]
        public void Test_Save_Queue_WhenNullOrEmpty()
        {
            var settings = Options.Create<QueueShardSettings>(new QueueShardSettings());
            var ctx = new QueueShardContext(settings);
            Assert.Throws<ArgumentNullException>(() => ctx.Save(null));
        }
    }
}
