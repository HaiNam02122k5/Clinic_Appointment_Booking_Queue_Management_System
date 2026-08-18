using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Contracts
{
    public class UserDetailDto : UserSummaryDto
    {
        public string Address { get; set; }
    }
}
