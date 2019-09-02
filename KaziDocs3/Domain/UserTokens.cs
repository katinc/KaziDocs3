using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaziDocs3.Domain
{
    public class UserTokens
    {
        [Key]
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
