using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProject.Models
{
    public abstract class AbstractType
    {
        public int AbstractTypeID { get; set; }

        public string TypeName { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
