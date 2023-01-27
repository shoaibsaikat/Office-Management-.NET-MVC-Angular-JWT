namespace Office_Management_.NET_MVC_Angular_JWT.ResponseModels
{
    public class InventoryResponseModel
    {
        public int id { get; set; }
        public string name { get; set; } = String.Empty;
        public string unit { get; set; } = String.Empty;
        public uint count { get; set; }
        public string description { get; set; } = String.Empty;
    }
}
