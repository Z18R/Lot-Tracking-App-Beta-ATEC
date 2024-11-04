namespace LotTrackingApp.Models
{
    public class LotTracking
    {
        public string LotCode { get; set; }
        public string LotAlias { get; set; }
        public string CustomerID { get; set; }
        public string ProductID { get; set; }
        public string MaterialID { get; set; }
        public string FlowID { get; set; }
        public string PackageID { get; set; }
        public string LeadTypeID { get; set; }
        public string StatusID { get; set; }
        public string StoreID { get; set; }
        public string StageID { get; set; }
        public string RecipeID { get; set; }
        public int? CurrentQty { get; set; } // Nullable int
        public int? CurrentSequence { get; set; } // Nullable int
        public string StatusCode { get; set; }
        public string StageCode { get; set; }
        public string CustomerCode { get; set; }
        public int? TotalSavedRejects { get; set; } // Nullable int
        public int? TotalSavedAccounts { get; set; } // Nullable int
        public string RecipeCode { get; set; }
        public string DateCodeVerifiedBy { get; set; }
        public string DateCode { get; set; }
        public string POCode { get; set; }
    }
    public class TransactionDetail
    {
        public string LotCode { get; set; }
        public int Sequence { get; set; }
        public string RecipeID { get; set; }
        public int InQty { get; set; }
        public int RejectQty { get; set; }
        public int AccountQty { get; set; }
        public int OutQty { get; set; }
        public string EquipmentID { get; set; }
        public string Remarks { get; set; }
    }
}
