namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class RequisitionResponseModel
    {
        public int id { get; set; }
        public string title { get; set; } = String.Empty;
        public int user_id { get; set; }
        public string user_name { get; set; } = String.Empty;
        public string item_name { get; set; } = String.Empty;
        public string unit { get; set; } = String.Empty;
        public uint amount { get; set; }
        public uint total { get; set; }
        public int approver_id { get; set; }
        public string approver_name { get; set; } = String.Empty;
        public int? distributor_id { get; set; }
        public string? distributor_name { get; set; }
        public bool? approved { get; set; }
        public bool? distributed { get; set; }
        public string? comment { get; set; }
        public DateTime date { get; set; }
    }
}
