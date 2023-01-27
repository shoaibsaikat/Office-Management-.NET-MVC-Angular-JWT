using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Office_Management_.NET_MVC_Angular_JWT.Models
{
    public class AssetHistory
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }
        public long AssetId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public virtual Asset Asset { get; set; } = null!;
        public virtual User FromUser { get; set; } = null!;
        public virtual User ToUser { get; set; } = null!;
    }
}
