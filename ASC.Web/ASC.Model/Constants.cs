using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Model
{
    public static class Constants
    {
        public enum Roles
        {
            Admin,
            Engineer,
            User
        }

        public enum MasterDataKey
        {
            VehicleType,
            ServiceType,
            ServiceStatus
        }

        public enum ServiceRequestStatus
        {
            New,
            Assigned,
            InProgress,
            Completed,
            Cancelled
        }
    }
}
