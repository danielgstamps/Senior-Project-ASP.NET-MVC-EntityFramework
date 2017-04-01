using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class TypeTwo : AbstractType
    {
        public ICollection<Category> Categories { get; set; }

        public int TypeID { get; set; }

        public string TypeName { get; set; }
    }
}