using System;
using System.Collections.Generic;
using System.Linq;

namespace Biljettshopen
{
    public class User
    {
        public string Name { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public List<string> BookedTickets { get; set; } = new List<string>();

        public User(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public override string ToString()
        {
            var tickets = string.Join(";", BookedTickets);
            return $"{Name},{Email},{PhoneNumber},{tickets}";
        }

        public static User FromString(string data)
        {
            var parts = data.Split(',');
            if (parts.Length >= 3)
            {
                var user = new User(parts[0], parts[1], parts[2]);
                if (parts.Length == 4 && !string.IsNullOrEmpty(parts[3]))
                {
                    user.BookedTickets = parts[3].Split(';').ToList();
                }
                return user;
            }
            return null;
        }

        public void AddBookedTicket(string ticketInfo)
        {
            BookedTickets.Add(ticketInfo);
        }
    }
}

