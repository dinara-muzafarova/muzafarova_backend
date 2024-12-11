using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using muzafarova_backend.Models;

namespace muzafarova_backend.Data
{
    public class muzafarova_backendContext : DbContext
    {
        public muzafarova_backendContext (DbContextOptions<muzafarova_backendContext> options)
            : base(options)
        {
        }

        public DbSet<muzafarova_backend.Models.flight> flight { get; set; } = default!;
        public DbSet<muzafarova_backend.Models.passenger> passenger { get; set; } = default!;
        public DbSet<muzafarova_backend.Models.booking> booking { get; set; } = default!;
    }
}
