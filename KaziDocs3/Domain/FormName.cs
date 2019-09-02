using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaziDocs3.Domain
{
    public class FormName
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string DataTypeId { get; set; }
        public string FormNameId { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccountId { get; set; }
    }
}
