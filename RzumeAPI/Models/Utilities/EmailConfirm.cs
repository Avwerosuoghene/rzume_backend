
namespace RzumeAPI.Models
{
    public class EmailConfirm
    {
        public required string Email { get; set; } 

        public bool IsConfirmed { get; set; }

        public bool EmailSent { get; set; }
    }
}