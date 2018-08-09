using System;
using System.Collections.Generic;
using System.Text;

namespace DistQueue.Shard.Model
{
    public interface IQueueShardItem
    {
        bool IsVisible();
        void SetVisibilityTimeout(TimeSpan visiblityTimeout);
        string ID { get; }
        int DequeueAttempts { get; set; }
        byte[] Content { get; }
    }
}
