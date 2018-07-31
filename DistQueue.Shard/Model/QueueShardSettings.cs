using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DistQueue.Shard.Model
{
    public class QueueShardSettings
    {
        public string StoragePath { get; set; } = AppContext.BaseDirectory;
        public TimeSpan VisibilityTimeout { get; set; } = TimeSpan.FromMinutes(30);
        public short MaxDequeueAttempts { get; set; } = 5;
    }
}
