using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DistQueue.Shard.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace DistQueue.Shard.Controllers
{
    [Route("api/v1/[controller]")]
    public class QueueController : Controller
    {
        private readonly IQueueShardRepository _queueRepository;

        public QueueController(IQueueShardRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        // GET api/v1/queue
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_queueRepository.List());
        }

        // POST api/v1/queue
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            if (string.IsNullOrEmpty(value)) return BadRequest("Invalid parameter " + nameof(@value));

            return Ok(_queueRepository.AddQueue(value));
        }

        // DELETE api/v1/queue/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid parameter " + nameof(id));

            return Ok(_queueRepository.DeleteQueue(id));
        }

        // PUT api/v1/queue/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]string content)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid parameter " + nameof(@id));
            
            if (content == null || content.Any() == false) return BadRequest("Invalid content");
            return Ok(_queueRepository.Enqueue(id, Convert.FromBase64String(content)));
        }

        // GET api/v1/queue/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid parameter " + nameof(@id));

            return Ok(_queueRepository.Dequeue(id));
        }

        // DELETE api/v1/queue/5/1
        [HttpDelete("{queue}/{id}")]
        public void DeleteItem(string queue, string id)
        {
            //TODO: delete dequeued element from queue
            throw new NotImplementedException();
        }
    }
}
