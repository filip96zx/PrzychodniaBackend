using Microsoft.AspNetCore.Identity;
using Przychodnia.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; set; }
        public List<Visit> Visits { get; set; }


    }
}
