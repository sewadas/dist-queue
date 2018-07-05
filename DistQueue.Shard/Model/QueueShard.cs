using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistQueue.Shard.Model
{
    public class QueueShard<T> where T : IQueueShardItem
    {
        public QueueShard(string name, TimeSpan? defaultVisibilityTimeout = null, short maxDequeueAttempts = 5)
        {
            queue = new ConcurrentQueue<T>();
            Name = name;
            this.maxDequeueAttempts = maxDequeueAttempts;

            inflightItems = new ConcurrentDictionary<string, T>();

            if (defaultVisibilityTimeout == null) this.defaultVisibilityTimeout = TimeSpan.FromMinutes(30);
            else this.defaultVisibilityTimeout = defaultVisibilityTimeout.Value;
        }

        public void Enqueue(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            queue.Enqueue(item);
        }

        public T Dequeue()
        {
            T item;
            if (queue.TryDequeue(out item))
            {
                item.SetVisibilityTimeout(defaultVisibilityTimeout);
                item.DequeueAttempts++;
                inflightItems.TryAdd(item.ID, item);
            }
            return item;
        }

        public void Delete(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
            inflightItems.TryRemove(name, out T value);
        }

        public void ReclaimVisibility()
        {
            foreach (KeyValuePair<string, T> item in inflightItems.ToArray())
            {
                if (item.Value.DequeueAttempts > maxDequeueAttempts) inflightItems.TryRemove(item.Key, out T value);
                else if (item.Value.IsVisible())
                {
                    queue.Enqueue(item.Value);
                    inflightItems.TryRemove(item.Key, out T value);
                }
            }
        }

        public int Count { get { return queue.Count; } }
        public int InFlightCount { get { return inflightItems.Count; } }
        public bool IsEmpty { get { return queue.IsEmpty; } }
        public string Name { get; private set; }
        public T[] ToArray() { return queue.ToArray(); }

        private ConcurrentQueue<T> queue;
        private ConcurrentDictionary<string, T> inflightItems;
        private TimeSpan defaultVisibilityTimeout;
        private short maxDequeueAttempts;
    }
}
