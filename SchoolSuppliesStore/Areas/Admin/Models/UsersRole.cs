using Microsoft.AspNetCore.Identity;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Models
{
    public class UsersRole
    {
        public ApplicationUser User {get; set;}
        public IEnumerable<IdentityRole> UserRoles{get;set;}
    }
}