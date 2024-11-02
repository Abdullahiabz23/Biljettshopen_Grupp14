using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Biljettshopen
{
    public class TicketBookingSystem
    {
        private List<User> users = new List<User>();
        private User loggedInUser = null;
        private string usersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "users.txt");
        private List<Event> events;

        public TicketBookingSystem()
        {
            LoadUsers();
            events = new List<Event>
            {
            new Event("Halloweenfest", new DateTime(2024, 11, 5, 18, 0, 0), new DateTime(2024, 10, 20, 9, 0, 0), 100, "Arena: Tegel"),
            new Event("Studentfest", new DateTime(2024, 11, 10, 20, 0, 0), new DateTime(2024, 10, 22, 10, 0, 0), 100, "Arena: Trä")
            };
            LoadBookings();
        }

        public void Run()
        {
            ShowWelcomeScreen();
            bool running = true;

            while (running)
            {
                if (loggedInUser == null)
                {
                    Console.WriteLine("\nVälj ett alternativ:");
                    Console.WriteLine("1. Registrera dig");
                    Console.WriteLine("2. Logga in");
                    Console.WriteLine("3. Avsluta programmet");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            RegisterUser();
                            break;
                        case "2":
                            LogIn();
                            break;
                        case "3":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val, försök igen.");
                            break;
                    }
                }
                else
                {
                    ShowUserMenu();
                }
            }
        }

        private void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine("***********************************************");
            Console.WriteLine("             Välkommen till Biljettshopen      ");
            Console.WriteLine("***********************************************");
            Console.WriteLine();

            Console.WriteLine("=============================================");
            Console.WriteLine("         Vår Kommande Evenemang");
            Console.WriteLine("=============================================");
            Console.WriteLine();

            if (events.Count > 0)
            {
                foreach (var ev in events)
                {
                    Console.WriteLine($"- {ev.Name} (Datum: {ev.EventDateTime.ToShortDateString()} | Arena: {ev.Arena})");
                }
            }
            else
            {
                Console.WriteLine("Inga tillgängliga evenemang just nu.");
            }

            Console.WriteLine();
            Console.WriteLine("***********************************************");
            Console.WriteLine("Logga in eller registrera dig för att köpa biljetter, avboka och visa dina biljetter.");
            Console.WriteLine("***********************************************");
            Console.WriteLine();
        }

        private void RegisterUser()
        {
            string name;
            while (true)
            {
                Console.Write("Ange ditt namn: ");
                name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name) && name.Length >= 3 && name.All(char.IsLetter))
                    break;

                Console.WriteLine("Namnet måste vara minst tre tecken, inte tomt och får endast innehålla bokstäver. Försök igen.");
            }

            string email;
            while (true)
            {
                Console.Write("Ange din e-postadress: ");
                email = Console.ReadLine();
                if (IsValidEmail(email))
                    break;
                Console.WriteLine("Ogiltig e-postadress. Försök igen.");
            }

            string phoneNumber;
            while (true)
            {
                Console.Write("Ange ditt telefonnummer: ");
                phoneNumber = Console.ReadLine();
                if (IsValidPhoneNumber(phoneNumber))
                    break;
                Console.WriteLine("Ogiltigt telefonnummer. Ange ett nummer som börjar med '07' och är exakt 10 siffror långt. Försök igen.");
            }

            User newUser = new User(name, email, phoneNumber);
            users.Add(newUser);
            Console.WriteLine("Registrering lyckades. Logga in för att fortsätta.");
            SaveUsers();
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Length == 10 && phoneNumber.StartsWith("07") && phoneNumber.All(char.IsDigit);
        }

        private void LogIn()
        {
            string email;
            while (true)
            {
                Console.Write("Ange din e-postadress: ");
                email = Console.ReadLine();
                if (IsValidEmail(email))
                    break;
                Console.WriteLine("Ogiltig e-postadress. Försök igen.");
            }

            loggedInUser = users.FirstOrDefault(u => u.Email == email);

            if (loggedInUser != null)
            {
                Console.WriteLine($"Inloggning lyckades. Välkommen {loggedInUser.Name}!");
            }
            else
            {
                Console.WriteLine("Ingen användare hittades med den angivna e-postadressen. Kontrollera eller registrera ett konto.");
            }
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
        }

        private void ShowUserMenu()
        {
            Console.WriteLine($"\nVälkommen, {loggedInUser.Name}!");
            Console.WriteLine("Välj ett alternativ:");
            Console.WriteLine("1. Visa kommande evenemang");
            Console.WriteLine("2. Köp biljett till ett evenemang");
            Console.WriteLine("3. Visa bokade biljetter");
            Console.WriteLine("4. Avboka biljett");
            Console.WriteLine("5. Logga ut");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowEvents();
                    break;
                case "2":
                    BookTicket();
                    break;
                case "3":
                    ShowBookedTickets();
                    break;
                case "4":
                    CancelBooking();
                    break;
                case "5":
                    LogOut();
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, försök igen.");
                    break;
            }
        }

        private void ShowEvents()
        {
            Console.WriteLine("Kommande evenemang:");
            foreach (var ev in events)
            {
                Console.WriteLine($"\n{ev.Name} - {ev.EventDateTime.ToString("g")}");
                Console.WriteLine($"Arena: {ev.Arena}");
                Console.WriteLine($"Pris per biljett: {ev.TicketPrice} SEK");
            }
        }

        private void BookTicket()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Du måste vara inloggad för att boka biljetter.");
                return;
            }

            Event chosenEvent = null;
            while (chosenEvent == null)
            {
                Console.WriteLine("Vilken av dessa evenemang vill du köpa biljett för:");
                for (int i = 0; i < events.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {events[i].Name} - {events[i].EventDateTime.ToString("f")} (Biljettsläpp: {events[i].TicketReleaseDateTime.ToString("f")})");
                }

                if (int.TryParse(Console.ReadLine(), out int eventChoice) && eventChoice >= 1 && eventChoice <= events.Count)
                {
                    chosenEvent = events[eventChoice - 1];

                    // Kontrollera om biljetterna är tillgängliga ännu
                    if (DateTime.Now < chosenEvent.TicketReleaseDateTime)
                    {
                        Console.WriteLine($"Biljetterna till {chosenEvent.Name} är inte tillgängliga förrän {chosenEvent.TicketReleaseDateTime.ToString("f")}.");
                        chosenEvent = null; // Återställ för att låta användaren välja igen
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt val, försök igen.");
                }
            }

            chosenEvent.ShowSeats();

            int ticketCount = 0;
            while (ticketCount < 1 || ticketCount > 5)
            {
                Console.WriteLine("Hur många biljetter vill du köpa? (Max 5 personer):");
                if (!int.TryParse(Console.ReadLine(), out ticketCount) || ticketCount < 1 || ticketCount > 5)
                {
                    Console.WriteLine("Ogiltigt antal biljetter. Välj mellan 1 och 5.");
                }
            }

            List<int> seatNumbers = new List<int>();
            bool validSelection = false;

            while (!validSelection)
            {
                Console.WriteLine($"Ange {ticketCount} platsnummer som du vill reservera, separera med komma (ex: 1,2,5):");
                var seatNumbersInput = Console.ReadLine();
                seatNumbers = seatNumbersInput.Split(',').Select(int.Parse).ToList();

                if (seatNumbers.Distinct().Count() != seatNumbers.Count)
                {
                    Console.WriteLine("Du har valt samma platsnummer flera gånger. Välj unika platser.");
                    continue;
                }

                if (seatNumbers.Count != ticketCount)
                {
                    Console.WriteLine($"Felaktigt antal platser valda. Du har valt att köpa {ticketCount} biljetter men markerade {seatNumbers.Count} platser.");
                    continue;
                }

                var selectedSeats = chosenEvent.Seats.Where(s => seatNumbers.Contains(s.Number)).ToList();
                if (selectedSeats.Any(s => s.Status != SeatStatus.Ledig))
                {
                    Console.WriteLine("En eller flera av de valda platserna är upptagna. Välj andra platser.");
                    continue;
                }

                validSelection = true;
            }

            if (chosenEvent.ReserveSelectedSeats(seatNumbers, seatNumber =>
            {
                var seat = chosenEvent.Seats.FirstOrDefault(s => s.Number == seatNumber && s.Status == SeatStatus.Reserverad);
                if (seat != null) seat.Status = SeatStatus.Ledig;
            }))
            {
                Console.WriteLine("Vill du slutföra köpet för de reserverade biljetterna? (j/n)");
                if (Console.ReadLine().ToLower() == "j")
                {
                    int totalCost = seatNumbers.Sum(seatNumber =>
                    {
                        var seat = chosenEvent.Seats.FirstOrDefault(s => s.Number == seatNumber);
                        return seat?.Type == SeatType.Bänk ? chosenEvent.TicketPrice * 2 : chosenEvent.TicketPrice;
                    });

                    ProcessPayment(totalCost);
                    chosenEvent.CompletePurchase(seatNumbers, loggedInUser);
                    SaveUsers();
                }
                else
                {
                    Console.WriteLine("Köp avbrutet. Platserna är nu lediga igen.");
                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = chosenEvent.Seats.FirstOrDefault(s => s.Number == seatNumber && s.Status == SeatStatus.Reserverad);
                        if (seat != null) seat.Status = SeatStatus.Ledig;
                    }
                }
            }
        }


        private void ProcessPayment(int totalCost)
        {
            Console.WriteLine($"Total kostnad: {totalCost} SEK");
            Console.WriteLine("Välj betalningsmetod: 1. Faktura 2. Direktbetalning");

            var paymentChoice = Console.ReadLine();
            IPaymentProcessor paymentProcessor;

            if (paymentChoice == "1")
            {
                paymentProcessor = new InvoicePaymentProcessor();
            }
            else if (paymentChoice == "2")
            {
                paymentProcessor = new DirectPaymentProcessor();
            }
            else
            {
                Console.WriteLine("Ogiltigt val, försök igen.");
                ProcessPayment(totalCost); // Återförsök för att välja betalningsmetod
                return;
            }

            paymentProcessor.ProcessPayment(totalCost);
        }

        private void ShowBookedTickets()
        {
            if (loggedInUser == null || !loggedInUser.BookedTickets.Any())
            {
                Console.WriteLine("Inga bokade biljetter att visa.");
                return;
            }

            Console.WriteLine("Dina bokade biljetter:");
            foreach (var ticket in loggedInUser.BookedTickets)
            {
                Console.WriteLine(ticket);
            }
        }

        private void CancelBooking()
        {
            if (loggedInUser == null || !loggedInUser.BookedTickets.Any())
            {
                Console.WriteLine("Inga bokade biljetter att avboka.");
                return;
            }

            Console.WriteLine("Ange numret på biljetten du vill avboka:");
            for (int i = 0; i < loggedInUser.BookedTickets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {loggedInUser.BookedTickets[i]}");
            }

            if (!int.TryParse(Console.ReadLine(), out int ticketChoice) || ticketChoice < 1 || ticketChoice > loggedInUser.BookedTickets.Count)
            {
                Console.WriteLine("Ogiltigt val, försök igen.");
                return;
            }

            string selectedTicket = loggedInUser.BookedTickets[ticketChoice - 1];
            loggedInUser.BookedTickets.RemoveAt(ticketChoice - 1);
            SaveUsers();

            try
            {
                var parts = selectedTicket.Split('-');
                string eventName = parts[0].Trim();

                var match = Regex.Match(selectedTicket, @"Plats (\d+)");
                if (!match.Success)
                {
                    Console.WriteLine("Kunde inte hitta platsnumret i biljetten. Avbokning misslyckades.");
                    return;
                }
                int seatNumber = int.Parse(match.Groups[1].Value);

                var selectedEvent = events.FirstOrDefault(e => e.Name == eventName);
                if (selectedEvent != null)
                {
                    selectedEvent.CancelSeat(seatNumber);
                    Console.WriteLine($"Biljetten '{selectedTicket}' har avbokats och plats {seatNumber} är nu ledig.");
                }
                else
                {
                    Console.WriteLine("Kunde inte hitta eventet för biljetten. Avbokning misslyckades.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ett fel inträffade vid avbokningen. Kontrollera biljetten och försök igen.");
                Console.WriteLine($"Felmeddelande: {ex.Message}");
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                var lines = File.ReadAllLines(usersFilePath);
                foreach (var line in lines)
                {
                    var user = User.FromString(line);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
            }
        }

        private void SaveUsers()
        {
            var lines = users.Select(u => u.ToString()).ToArray();
            File.WriteAllLines(usersFilePath, lines);
        }

        private void LoadBookings()
        {
            foreach (var user in users)
            {
                foreach (var ticket in user.BookedTickets)
                {
                    var parts = ticket.Split('-');
                    string eventName = parts[0].Trim();
                    var match = Regex.Match(ticket, @"Plats (\d+)");
                    if (match.Success)
                    {
                        int seatNumber = int.Parse(match.Groups[1].Value);
                        var selectedEvent = events.FirstOrDefault(e => e.Name == eventName);
                        var seat = selectedEvent?.Seats.FirstOrDefault(s => s.Number == seatNumber);
                        if (seat != null) seat.Status = SeatStatus.Upptagen;
                    }
                }
            }
        }

        private void LogOut()
        {
            loggedInUser = null;
            Console.WriteLine("Du har loggats ut.");
        }
    }
}
