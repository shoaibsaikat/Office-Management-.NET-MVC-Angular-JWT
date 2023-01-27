using System;
using System.Collections.Generic;

namespace Office_Management_.NET_MVC_Angular_JWT.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            Requisitions = new HashSet<Requisition>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public uint Count { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual ICollection<Requisition> Requisitions { get; set; }
    }
}
