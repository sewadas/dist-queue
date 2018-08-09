using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DistQueue.Shard.Model
{
    public class QueueShardContext
    {
        private readonly QueueShardSettings _settings;

        public QueueShardContext(IOptions<QueueShardSettings> settings)
        {
            _settings = settings.Value;
            if (string.IsNullOrEmpty(_settings.StoragePath)) throw new ArgumentException("Invalid Storage Path");
            if (Directory.Exists(_settings.StoragePath) == false) Directory.CreateDirectory(_settings.StoragePath);
        }

        public List<QueueShard<IQueueShardItem>> Queues
        {
            get
            {
                var _queues = new List<QueueShard<IQueueShardItem>>();
                Parallel.ForEach(Directory.EnumerateFiles(_settings.StoragePath, "*.bin",
                new EnumerationOptions() { IgnoreInaccessible = true }), file =>
                {
                    try { _queues.Add(Load(file)); }
                    catch (JsonReaderException)
                    {
                        if (File.Exists($"{file}.bak")) File.Delete($"{file}.bak");
                        File.Move(file, $"{file}.bak");
                    }
                });
                return _queues;
            }
        }

        public void Save(QueueShard<IQueueShardItem> queue)
        {
            if (queue == null) throw new ArgumentNullException();
            File.WriteAllText($"{_settings.StoragePath}\\{queue.Name}.bin", JsonConvert.SerializeObject(queue.ToArray()));
        }

        public QueueShard<IQueueShardItem> Load(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException();
            if (Path.GetFileName(file).EndsWith(".bin") == false) throw new ArgumentException();

            QueueShardItem[] items = JsonConvert.DeserializeObject<QueueShardItem[]>(File.ReadAllText(file));
            return new QueueShard<IQueueShardItem>(Path.GetFileNameWithoutExtension(file), _settings.VisibilityTimeout, _settings.MaxDequeueAttempts, items);
        }
    }
}
