using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessTier.Requests.DepartmentRequest
{
    public class CreateDepartmentRequest
    {
        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Hotline { get; set; }
        public string RoomNumber { get; set; }
    }
}
