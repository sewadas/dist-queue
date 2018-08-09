using System;
using System.Collections.Generic;
using System.Text;

namespace DistQueue.Shard.Model
{
    [Serializable]
    public class QueueShardItem : IQueueShardItem
    {
        public QueueShardItem(string id, byte[] content, int dequeueAttempts = 0)
        {
            this.id = id;
            this.content = content;
            this.DequeueAttempts = 0;
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

        public string ID { get { return id; } }

        public int DequeueAttempts { get; set; }

        public byte [] Content { get { return content; } }

        private readonly string id;
        private readonly byte[] content;
        private long? visibilityTimeout;
    }
}
