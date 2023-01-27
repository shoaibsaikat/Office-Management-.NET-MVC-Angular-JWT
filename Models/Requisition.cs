using System;
using System.Collections.Generic;

namespace Office_Management_.NET_MVC_Angular_JWT.Models
{
    public partial class Requisition
    {
        public int Id { get; set; }
        public bool? Approved { get; set; }
        public string Title { get; set; } = null!;
        public uint Amount { get; set; }
        public string? Comment { get; set; }
        public int ApproverId { get; set; }
        public int InventoryId { get; set; }
        public int UserId { get; set; }
        public bool? Distributed { get; set; }
        public int? DistributorId { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? DistributionDate { get; set; }
        public DateTime RequestDate { get; set; }

        public virtual User Approver { get; set; } = null!;
        public virtual User? Distributor { get; set; }
        public virtual Inventory Inventory { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
