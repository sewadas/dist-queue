using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistQueue.Shard.Model
{
    public interface IQueueShardRepository
    {
        IEnumerable<string> List();
        string AddQueue(string queueID);
        string DeleteQueue(string queueID);
        string Enqueue(string queueID, byte [] value);
        IQueueShardItem Dequeue(string queueID);
        string DeleteItem(string queueID, string ID);
    }
}
