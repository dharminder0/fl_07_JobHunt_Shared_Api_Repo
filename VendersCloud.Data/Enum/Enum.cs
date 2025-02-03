namespace VendersCloud.Data.Enum
{
    public class Enum
    {
        public enum RoleType
        {
            Vendor=1,
            Client=2
        }

        public enum BillingType
        {
            Monthly=1,
            Annually=2
        }
        
        public enum Visibility
        {
            Limited=1,
            Empaneled=2,
            Public=3
        }

        public enum RequirementsStatus
        {
            Open=1,
            OnHold=2,
            Closed=3
        }
        public enum LocationType
        {
            Onsite = 1,
            Hybrid = 2,
            Remote = 3
        }
    }
}
