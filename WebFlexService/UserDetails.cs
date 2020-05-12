using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFlexService
{
    public class UserDetails : System.Web.Services.Protocols.SoapHeader
    {
        public string userName { get; set; }
        public string password { get; set; }

        public bool IsValid()
        {
            //Write the logic to Check the User Details From DataBase  
            //i can chek with some hardcode details UserName=Nitin and Password=Pandit  
            return this.userName == "Bejerman" && this.password == "Case#123.";
            //it'll check the details and will return true or false   
        }
    }        
}