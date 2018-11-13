using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sda.Performance.Entities;

namespace Sda.Performance
{
    public class Startup
    {
        public const string Server =
            @"irma-db\poszeidon";
            //@"(LocalDB)\MSSQLLocalDB";

        public const string Database = "Sda.Performance";

        public const string ConnectionString =
            "Data Source=" + Server +
            ";Initial Catalog=" + Database +
            ";Trusted_Connection=True;MultipleActiveResultSets=True";

        static void InitializeDatabase(IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<EntryContext>())
            {
                if (context.Database.IsInMemory()) return;
                context.Database.Migrate();
            }
        }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddDbContext<EntryContext>(opt => opt.UseInMemoryDatabase(Database));
            services.AddDbContext<EntryContext>(opt =>
                opt.UseSqlServer(ConnectionString, provOpt => provOpt.CommandTimeout(600)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));

            InitializeDatabase(app);
            app.UseMvc();
        }
    }
}
