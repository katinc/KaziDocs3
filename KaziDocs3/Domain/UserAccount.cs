using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaziDocs3.Domain
{
    public class UserAccount
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccountId { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchieved { get; set; }
        public Boolean CanLogin { get; set; }
        public Boolean AccountBlocked { get; set; }
    }
}
