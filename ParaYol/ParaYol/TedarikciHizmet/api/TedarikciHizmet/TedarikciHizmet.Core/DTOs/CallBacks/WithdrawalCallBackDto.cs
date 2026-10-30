namespace TedarikciHizmet.Core.DTOs.CallBacks
{
    public class WithdrawalCallBackDto
    {
        public string userID { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
        public decimal verifiedAmount { get; set; }
        public string transaction_id { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public string type { get; set; }
        public string payment_method_id { get; set; }
        public string hash { get; set; }
    }
}
