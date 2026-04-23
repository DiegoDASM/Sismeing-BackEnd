using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sismeing.Infrestructura.Persistence
{
    public class SupaBaseDBcontext : DbContext
    {
        public SupaBaseDBcontext(DbContextOptions<SupaBaseDBcontext> options) : base(options)
        {
        }
    }
}
