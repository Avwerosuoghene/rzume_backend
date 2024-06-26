using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RzumeAPI.Models
{
    public class UserEmailOptions
    {
        public List<string> ToEmails { get; set; } =new List<string>();

        public string Body { get; set; } 

        public string Subject { get; set; } 

        public List<KeyValuePair<string, string>> Placeholders { get; set; } = new List<KeyValuePair<string, string>>();
    }
}