namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class AccountResponseModel
    {
        public int id { get; set; }
        public string? username { get; set; } = null;
        public string? first_name { get; set; } = null;
        public string? last_name { get; set; } = null;
        public string? email { get; set; } = null;
        public int? manager_id { get; set; } = null;
        public bool can_distribute_inventory { get; set; }
        public bool can_approve_inventory { get; set; }
        public bool can_approve_leave { get; set; }
        public bool can_manage_asset { get; set; }
        public string? refresh_token { get; set; } = null;
        public string? access_token { get; set; } = null;
    }
}
