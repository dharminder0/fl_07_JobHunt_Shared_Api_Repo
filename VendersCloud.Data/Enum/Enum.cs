using System.ComponentModel;

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
            Closed=3,
            
        }
        public enum LocationType
        {
            Onsite = 1,
            Hybrid = 2,
            Remote = 3
        }

        public enum MasterListValues
        {
            State=1,
            Technology= 2
        }

        public enum InviteStatus
        {
            [Description("")]
            NotInvited = 0,
            [Description("Invited")]
            Pending = 1,
            [Description("Empaneled")]
            Accepted =2,
            [Description("Declined")]
            Declined =3,
            [Description("Archived")]
            Archived =4
        }

        public enum ClientStatus
        {
            Active= 1,
            InActive=2,
        }
        public enum MemberStatus
        {
            Invited=1,
            Active = 2,
            InActive = 3,
        }

        public enum BenchAvailability
        {
            [Description("Immediate")]
            Immediate = 1,

            [Description("15 Days Notice")]
            Days15Notice = 2,

            [Description("30 Days Notice")]
            Days30Notice = 3,

            [Description("60 Days Notice")]
            Days60Notice = 4,

            [Description("90 Days Notice")]
            Days90Notice = 5,

            [Description("Negotiable")]
            Negotiable = 6,

            [Description("Not Available")]
            NotAvailable = 7,

        }
        public enum RecruitmentStatus
        {
            [Description("New Application")]
            New = 1,

            [Description("Under Review")]
            UnderReview = 2,

            [Description("Shortlisted")]
            Shortlisted = 3,

            [Description("Technical Assessment")]
            TechnicalAssessment = 4,

            [Description("Interview Round 1")]
            InterviewRound1 = 5,

            [Description("Interview Round 2")]
            InterviewRound2 = 6,

            [Description("Interview Round 3")]
            InterviewRound3 = 7,

            [Description("Selected")]
            Selected = 8,

            [Description("Onboarded")]
            Onboarded = 9,

            [Description("Contract Closed")]
            ContractClosed = 10,

            [Description("Rejected")]
            Rejected = 11,

            [Description("On Hold")]
            OnHold = 12,

            [Description("Withdrawn")]
            Withdrawn = 13
        }
      
    }
}
