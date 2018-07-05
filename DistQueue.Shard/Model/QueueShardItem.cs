using System;
using System.Collections.Generic;
using System.Text;

namespace DistQueue.Shard.Model
{
    public class QueueShardItem : IQueueShardItem
    {
        public QueueShardItem(string name, byte[] content)
        {
            this.name = name;
            this.content = content;
        }

        public bool IsVisible()
        {
            if (visibilityTimeout == null) return true;
            else return DateTime.UtcNow.Ticks > visibilityTimeout;
        }

        public void SetVisibilityTimeout(TimeSpan visibilityTimeout)
        {
            this.visibilityTimeout = DateTime.UtcNow.Ticks + visibilityTimeout.Ticks;
        }

        public string ID { get => name; }

        public int DequeueAttempts { get; set; }

        public byte [] Content { get { return content; } }

        private readonly string name;
        private readonly byte[] content;
        private long? visibilityTimeout;
    }
}
