using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneProject.DAL;

namespace CapstoneProjectTests.Models
{
    public class InMemoryGenericRepository<TEntity> : GenericRepository<TEntity> where TEntity : class
    {
        public InMemoryGenericRepository(DataContext context) : base(context)
        {
        }
    }
}
