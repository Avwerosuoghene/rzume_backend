
namespace RzumeAPI.Models
{
    public class EmailConfirm
    {
        public string Email { get; set; } 

        public bool IsConfirmed { get; set; }

        public bool EmailSent { get; set; }
    }
}