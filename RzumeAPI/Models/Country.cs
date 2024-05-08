using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CountryID { get; set; } 

        public  string Name { get; set; } 

        public string Code { get; set; }


    }
}

