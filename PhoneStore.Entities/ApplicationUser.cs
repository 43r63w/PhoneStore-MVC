﻿
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string? Name { get; set; }

        public string? StreetAdreess { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }


    }
}
