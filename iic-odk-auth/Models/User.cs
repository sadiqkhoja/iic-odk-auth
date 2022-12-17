using System;
namespace iic_odk_auth.Models
{
	public class User
	{
        public String Email { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
        public String AzureObjectId { get; set; }
        public int OdkId { get; set; }
    }
}

