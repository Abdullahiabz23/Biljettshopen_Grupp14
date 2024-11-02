using System;
using System.Collections.Generic;
using System.Linq;

namespace Biljettshopen
{
    public class Event
    {
        public string Name { get; }
        public DateTime EventDateTime { get; }
        public DateTime TicketReleaseDateTime { get; }  // Nytt fält för biljettsläpp
        public List<Seat> Seats { get; }
        public int TicketPrice { get; }
        public string Arena { get; }
        public int AvailableTickets => Seats.Count(s => s.Status == SeatStatus.Ledig);

        public Event(string name, DateTime eventDateTime, DateTime ticketReleaseDateTime, int ticketPrice, string arena)
        {
            Name = name;
            EventDateTime = eventDateTime;
            TicketReleaseDateTime = ticketReleaseDateTime;  // Sätt biljettsläppstid
            TicketPrice = ticketPrice;
            Arena = arena;
            Seats = new List<Seat>();

            for (int i = 1; i <= 50; i++)
            {
                Seats.Add(new Seat(i, SeatType.Fällstol));
            }
            for (int i = 51; i <= 100; i++)
            {
                Seats.Add(new Seat(i, SeatType.Bänk));
            }
        }

        public void ShowSeats()
        {
            Console.WriteLine($"\nTillgängliga platser för evenemanget '{Name}' i Arena: {Arena} på {EventDateTime.ToString("f")}");
            Console.WriteLine("---------------------------");

            Console.WriteLine($"Fällstolar: {TicketPrice} SEK");
            foreach (var seat in Seats.Where(s => s.Type == SeatType.Fällstol))
            {
                Console.WriteLine($"Plats {seat.Number:D2}: Typ - {seat.Type}, Status - {seat.Status}");
            }

            Console.WriteLine($"\nBänkstolar: {TicketPrice * 2} SEK");
            foreach (var seat in Seats.Where(s => s.Type == SeatType.Bänk))
            {
                Console.WriteLine($"Plats {seat.Number:D2}: Typ - {seat.Type}, Status - {seat.Status}");
            }

            Console.WriteLine("---------------------------");
        }

        public bool ReserveSelectedSeats(List<int> seatNumbers, Action<int> timeoutAction)
        {
            var selectedSeats = Seats.Where(s => seatNumbers.Contains(s.Number) && s.Status == SeatStatus.Ledig).ToList();

            if (selectedSeats.Count < seatNumbers.Count)
            {
                Console.WriteLine("En eller flera av de valda platserna är inte tillgängliga. Försök igen med andra platser.");
                return false;
            }

            var reservedSeatNumbers = new List<int>();

            foreach (var seat in selectedSeats)
            {
                seat.Status = SeatStatus.Reserverad;
                reservedSeatNumbers.Add(seat.Number);

                System.Timers.Timer timer = new System.Timers.Timer(600000); // 10 minuter
                timer.Elapsed += (sender, e) => timeoutAction(seat.Number);
                timer.AutoReset = false;
                timer.Start();
            }

            Console.WriteLine($"Plats {string.Join(", ", reservedSeatNumbers)} är reserverad. Du har 10 minuter att slutföra köpet.");
            return true;
        }

        public void CompletePurchase(List<int> seatNumbers, User user)
        {
            foreach (var seatNumber in seatNumbers)
            {
                var seat = Seats.FirstOrDefault(s => s.Number == seatNumber && s.Status == SeatStatus.Reserverad);
                if (seat != null)
                {
                    seat.Status = SeatStatus.Upptagen;
                    string ticketInfo = $"{Name} - Plats {seat.Number} ({seat.Type})";
                    user.AddBookedTicket(ticketInfo);
                    Console.WriteLine($"Köp slutfört för plats {seat.Number}.");
                }
            }
        }

        public void CancelSeat(int seatNumber)
        {
            var seat = Seats.FirstOrDefault(s => s.Number == seatNumber && s.Status == SeatStatus.Upptagen);
            if (seat != null)
            {
                seat.Status = SeatStatus.Ledig;
            }
        }
    }
}

