using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sda.Performance.Entities;

namespace Sda.Performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController: ControllerBase
    {
        static IList<Entry> Update(IList<Entry> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            foreach (var entry in data)
            {
                entry.Text = Guid.NewGuid().ToString("N");
            }
            return data;
        }

        EntryContext EntryContext { get; }

        public EntryController(EntryContext entryContext) => EntryContext = entryContext ?? throw new ArgumentNullException(nameof(entryContext));

        IOrderedQueryable<Entry> Query =>
            EntryContext.Entries
            .Include(e => e.Items)
            .Where(e => e.Text.Contains("a") || e.Text.Contains("b") || e.Text.Contains("c"))
            .Where(e => e.Items.Any(i => i.Active))
            .Where(e => DateTime.Now < e.Date && e.Date < DateTime.Now.AddDays(10.0))
            .OrderBy(d => d.Text);

        // GET api/entry/sync
        [HttpGet("sync")]
        public ActionResult<IList<Entry>> GetSync()
        {
            try
            {
                return new ActionResult<IList<Entry>>(Update(Query.ToList()));
            }
            finally
            {
                EntryContext.SaveChanges();
            }
            
        }

        // GET api/entry/async
        [HttpGet("async")]
        public async Task<ActionResult<IList<Entry>>> GetAsync()
        {
            try
            {
                return new ActionResult<IList<Entry>>(Update(await Query.ToListAsync()));
            }
            finally
            {
                await EntryContext.SaveChangesAsync();
            }
        }
    }
}
