namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class AssetResponseModel
    {
        public long id { get; set; }
        public string name { get; set; } = String.Empty;
        public string model { get; set; } = String.Empty;
        public string serial { get; set; } = String.Empty;
        public int user { get; set; }
        public string user_first_name { get; set; } = String.Empty;
        public string user_last_name { get; set; } = String.Empty;
        public int? next_user { get; set; }
        public string purchase_date { get; set; } = String.Empty;
        public ulong warranty { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public string? description { get; set; } = null;
    }
}