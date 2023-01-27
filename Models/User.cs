using System;
using System.Collections.Generic;

namespace Office_Management_.NET_MVC_Angular_JWT.Models
{
    public partial class User
    {
        public User()
        {
            UserSupervisors = new HashSet<User>();
            AssetNextUsers = new HashSet<Asset>();
            AssetUsers = new HashSet<Asset>();
            AssetHistoryFromUsers = new HashSet<AssetHistory>();
            AssetHistoryToUsers = new HashSet<AssetHistory>();
            RequisitionApprovers = new HashSet<Requisition>();
            RequisitionDistributors = new HashSet<Requisition>();
            RequisitionUsers = new HashSet<Requisition>();
            LeaveApprovers = new HashSet<Leave>();
            LeaveUsers = new HashSet<Leave>();
        }

        public int Id { get; set; }
        public string Password { get; set; } = null!;
        public DateTime? LastLogin { get; set; }
        public bool IsSuperuser { get; set; } = false;
        public string? Username { get; set; } = null!;
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime DateJoined { get; set; } = DateTime.Now;
        public int? SupervisorId { get; set; }
        public bool CanApproveInventory { get; set; } = false;
        public bool CanDistributeInventory { get; set; } = false;
        public bool CanApproveLeave { get; set; } = false;
        public bool CanManageAsset { get; set; } = false;

        public virtual User? Supervisor { get; set; }

        public virtual ICollection<User> UserSupervisors { get; set; }
        public virtual ICollection<Asset> AssetNextUsers { get; set; }
        public virtual ICollection<Asset> AssetUsers { get; set; }
        public virtual ICollection<AssetHistory> AssetHistoryFromUsers { get; set; }
        public virtual ICollection<AssetHistory> AssetHistoryToUsers { get; set; }
        public virtual ICollection<Requisition> RequisitionApprovers { get; set; }
        public virtual ICollection<Requisition> RequisitionDistributors { get; set; }
        public virtual ICollection<Requisition> RequisitionUsers { get; set; }
        public virtual ICollection<Leave> LeaveApprovers { get; set; }
        public virtual ICollection<Leave> LeaveUsers { get; set; }
    }
}
