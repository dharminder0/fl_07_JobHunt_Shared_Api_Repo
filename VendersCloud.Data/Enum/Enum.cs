namespace VendersCloud.Data.Enum
{
    public class Enum
    {
        public enum RoleType
        {
            Vendor=1,
            Client=2,
            Admin=3
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

        public enum MasterListValues
        {
            State=1
        }

        public enum InviteStatus
        {
            Pending = 1,
            Accepted =2,
            Declined=3,
            Archived=4
        }

        public enum ClientStatus
        {
            Active= 1,
            InActive=2,
        }

        public enum BenchAvailability
        {
           Immediate=1,
        }
    }
}
