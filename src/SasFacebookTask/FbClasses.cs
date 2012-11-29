using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAS.FbFriends
{
    public static class FbApp
    {
        /// <summary>
        /// You must get your own application key
        /// from Facebook
        /// </summary>
        public static string ApplicationKey = "???????????????????????????";
    }

    public class FbFriend
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string RelationshipStatus { get; set; }
        public string Link { get; set; }
        public string UserID { get; set; }
        public string BirthdayDay { get; set; }
        public string BirthdayMonth { get; set; }
        public string BirthdayYear { get; set; }
    }

    public class FbSchool
    {
        public string UserID { get; set; }
        public string SchoolID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
    }

    public class FbStatus
    {
        public string UserID { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Status { get; set; }
    }
}
