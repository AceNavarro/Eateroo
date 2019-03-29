using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Eateroo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Phone Number")]
        [ProtectedPersonalData]
        public override string PhoneNumber {
            get
            {
                return base.PhoneNumber;
            }
            set
            {
                base.PhoneNumber = value;
            }
        }

        [ProtectedPersonalData]
        public string Name { get; set; }

        [ProtectedPersonalData]
        public string Street { get; set; }

        [ProtectedPersonalData]
        public string City { get; set; }

        [ProtectedPersonalData]
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        [ProtectedPersonalData]
        public string PostalCode { get; set; }
    }
}
