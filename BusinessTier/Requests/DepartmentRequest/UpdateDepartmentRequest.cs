using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessTier.Requests.DepartmentRequest
{
    public class UpdateDepartmentRequest
    {
        public string DepartmentName { get; set; }
        public string Hotline { get; set; }
        public string RoomNumber { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
