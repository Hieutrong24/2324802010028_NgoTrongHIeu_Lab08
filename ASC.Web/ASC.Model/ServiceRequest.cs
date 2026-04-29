using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Model
{
    public class ServiceRequest : BaseEntity
    {
        public string? RequestedBy { get; set; }

        public string? VehicleName { get; set; }

        public string? VehicleType { get; set; }

        public string? ServiceType { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? AssignedTo { get; set; }
    }
}
