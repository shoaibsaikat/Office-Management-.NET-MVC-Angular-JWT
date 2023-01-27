namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class LeaveResponseModel
    {
        public int id { get; set; }
        public string title { get; set; } = String.Empty;
        public int user { get; set; }
        public string? user_first_name { get; set; } = String.Empty;
        public string? user_last_name { get; set; } = String.Empty;
        public DateTime creation_date { get; set; }
        public int approver { get; set; }
        public bool? approved { get; set; } = false;
        public DateTime? approve_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public uint day_count { get; set; }
        public string? comment { get; set; }
    }
}