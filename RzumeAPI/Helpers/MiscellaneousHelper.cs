using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RzumeAPI.Data;

namespace RzumeAPI.Helpers
{


    public class MiscellaneousHelper
    {
        private ApplicationDbContext _db;

        public MiscellaneousHelper(ApplicationDbContext db)
        {
            _db = db;
        }

            public static  int GenerateOtp()
        {
            Random generator = new Random();
            int generatedOtp = generator.Next(0, 1000000);
            return generatedOtp;
        }


        public  bool IsUniqueUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return true;
            }

            return false;
        }
    }
}