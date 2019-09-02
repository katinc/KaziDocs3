using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaziDocs3.Domain
{
    public class Account
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool AccounBlocked { get; set; }
        public string UserAccountId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
