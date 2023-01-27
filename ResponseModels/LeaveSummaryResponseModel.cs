namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class LeaveSummaryResponseModel
    {
        public int user { get; set; }
        public string? first_name { get; set; } = String.Empty;
        public string? last_name { get; set; } = String.Empty;
        public long? days { get; set; }
    }
}