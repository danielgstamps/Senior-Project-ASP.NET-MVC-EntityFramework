using System.Collections.Generic;

namespace CapstoneProject.Models
{
    public class Type
    {
        public int TypeID { get; set; }

        public string TypeName { get; set; }

        public virtual ICollection<Evaluation> Evals { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}