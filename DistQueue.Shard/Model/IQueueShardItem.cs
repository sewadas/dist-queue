using System;
using System.Collections.Generic;
using System.Text;

namespace DistQueue
{
    public interface IQueueShardItem
    {
        bool IsVisible();
        void SetVisibilityTimeout(TimeSpan visiblityTimeout);
        string ID { get; }
        int DequeueAttempts { get; set; }
    }
}
