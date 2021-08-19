using System;
using System.Collections.Generic;
using System.Text;

namespace botspace
{
    public class BotMessage
    {
        public int Id { get; set; }        
        public string MessageType { get; set; }       
        public DateTime CreatedDate { get; set; }
        public string ContactNumber { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public User user { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public long telegramID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
