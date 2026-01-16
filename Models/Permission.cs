namespace QuanLyTaiSan.Models
{
    public static class Permissions
    {
        //Asset
        public const string AssetCreate = "Asset.Create";
        public const string AssetAssign = "Asset.Assign";
        public const string AssetDelete = "Asset.Delete";
        public const string AssetUpdate = "Asset.Update";
        public const string AssetGetHistory = "Asset.GetHistory";
        public const string AssetRecall = "Asset.Recall";
        //Super Admin
        public const string GrantPermission = "GrantPermission";
        //Auth
        public const string UserGet = "User.Get";
        public const string UserCreate = "User.Create";
        public const string UserDelete = "User.Delete";
        public const string UserRefreshToken = "User.RefreshToken";
        //Department
        public const string DepartmentGet = "Department.Get";
        public const string DepartmentCreate = "Department.Create";
        public const string DepartmentUpdate = "Department.Update";
        public const string DepartmentDelete = "Department.Delete";
        //Inventory
        public const string InventoryCreate = "Inventory.Create";
        public const string InvetoryUpdate = "Inventory.Update";
        //Report
        public const string ReportGet = "Report.Get";

    }

}
