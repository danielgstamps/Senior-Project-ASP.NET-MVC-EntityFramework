using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class TypeTwo : AbstractType
    {
        public int TypeTwoID {
            get { return base.AbstractTypeID; }
            set { base.AbstractTypeID = value; }
        }

        [ForeignKey("TypeTwoID")]
        public AbstractType AbstractType { get; set; }

        public string TypeName {
            get { return base.TypeName; }
            set { base.TypeName = value; }
        }

        public ICollection<Category> Categories {
            get { return base.Categories; }
            set { base.Categories = value; }
        }
    }
}