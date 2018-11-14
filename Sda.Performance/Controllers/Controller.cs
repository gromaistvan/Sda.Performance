using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sda.Performance.Entities;
using static System.IO.File;

namespace Sda.Performance.Controllers
{
    [Route("api")]
    [ApiController]
    public class Controller: ControllerBase
    {
        EntryContext EntryContext { get; }

        public Controller(EntryContext entryContext) => EntryContext = entryContext ?? throw new ArgumentNullException(nameof(entryContext));

        IOrderedQueryable<Entry> Query(string reference) =>
            EntryContext.Entries
            .Include(e => e.Items)
            .Where(e => string.Compare(e.Text, reference, StringComparison.Ordinal) > 0)
            .Where(e => e.Items.Any(i => i.Active))
            .Where(e => DateTime.Now < e.Date && e.Date < DateTime.Now.AddDays(10.0))
            .OrderBy(d => d.Text);

        // GET api/entry/sync
        [HttpGet("sync")]
        public async Task<ActionResult<IList<Entry>>> GetSync()
        {
            string reference = Guid.NewGuid().ToString("N");
            var data = Query(reference).ToList();
            WriteAllText($@"Files\{reference}.json", JsonConvert.SerializeObject(data));
            return await Task.FromResult(new ActionResult<IList<Entry>>(data));
        }

        // GET api/entry/async
        [HttpGet("async")]
        public async Task<ActionResult<IList<Entry>>> GetAsync()
        {
            string reference = Guid.NewGuid().ToString("N");
            var data = await Query(reference).ToListAsync();
            await WriteAllTextAsync($@"Files\{reference}.json", JsonConvert.SerializeObject(data));
            return new ActionResult<IList<Entry>>(data);
        }
    }
}
