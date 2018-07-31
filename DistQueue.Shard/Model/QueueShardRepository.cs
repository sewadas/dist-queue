using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistQueue.Shard.Model
{
    public class QueueShardRepository : IQueueShardRepository
    {
        public QueueShardRepository(QueueShardContext context)
        {
            
            shards = new ConcurrentDictionary<string, QueueShard<IQueueShardItem>>(context.Queues.ToDictionary(key => key.Name, value => value));
        }

        public QueueShardRepository(IEnumerable<QueueShard<IQueueShardItem>> queueShards = null)
        {
            if (queueShards == null) shards = new ConcurrentDictionary<string, QueueShard<IQueueShardItem>>();
            else shards = new ConcurrentDictionary<string, QueueShard<IQueueShardItem>>(queueShards.ToDictionary(key => key.Name, value => value));
        }

        public IEnumerable<string> List()
        {
            return shards.Keys;
        }

        public string AddQueue(string queueID)
        {
            if (shards.TryAdd(queueID, new QueueShard<IQueueShardItem>(queueID)) == false) throw new ArgumentException("Cannot add queue");
            return (queueID);
        }

        public string DeleteQueue(string queueID)
        {
            if (shards.TryRemove(queueID, out QueueShard<IQueueShardItem> shard) == false) throw new ArgumentException("Cannot delete queue");
            return shard.Name;
        }

        public string Enqueue(string queueID, byte [] value)
        {
            if (value == null) throw new ArgumentException(nameof(value));
            if (shards.ContainsKey(queueID) == false) throw new ArgumentException("Queue not exist");
            QueueShardItem item = new QueueShardItem(Guid.NewGuid().ToString(), value);
            shards[queueID].Enqueue(item);
            return item.ID;
        }

        public IQueueShardItem Dequeue(string queueID)
        {
            if (shards.ContainsKey(queueID) == false) throw new ArgumentException("Queue not exist");
            return shards[queueID].Dequeue();
        }

        public string DeleteItem(string queueID, string ID)
        {
            if (shards.ContainsKey(queueID) == false) throw new ArgumentException("Queue not exist");
            shards[queueID].Delete(ID);
            return ID;
        }

        private readonly ConcurrentDictionary<string, QueueShard<IQueueShardItem>> shards;
    }
}
