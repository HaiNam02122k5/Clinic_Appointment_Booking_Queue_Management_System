using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Auth.DTOs
{
    public class UserInfo
    {
        public UserInfo(Guid Id, string Username, string FullName)
        {
            this.Id = Id;
            this.Username = Username;
            this.FullName = FullName;
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
    }
}
