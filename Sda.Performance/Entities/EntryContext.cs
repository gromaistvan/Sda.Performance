using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sda.Performance.Entities
{
    public class EntryContext: DbContext
    {
        public virtual DbSet<Entry> Entries { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public EntryContext(DbContextOptions<EntryContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null) throw new ArgumentNullException(nameof(optionsBuilder));

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);
            var rnd = new Random();
            var entries =
                Enumerable.Range(1, 1000)
                .Select(i => new Entry
                {
                    Id = i,
                    Date = DateTime.Today.AddDays(100.0 * rnd.NextDouble()),
                    Text = Guid.NewGuid().ToString("N"),
                    Number = rnd.Next(),
                })
                .ToArray();
            var items = new List<Item>();
            foreach (var entry in entries)
            {
                items.AddRange(Enumerable.Range(1, rnd.Next(1, 6)).Select(i => new Item
                {
                    Id = entry.Id * 10 + i,
                    EntryId = entry.Id,
                    Active = true,
                    Name = Guid.NewGuid().ToString("N"),
                }));
            }
            modelBuilder.Entity<Entry>().HasData(entries);
            modelBuilder.Entity<Item>().HasData(items.ToArray());
        }
    }
}
