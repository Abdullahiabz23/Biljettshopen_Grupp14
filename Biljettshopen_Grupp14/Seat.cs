namespace Biljettshopen
{
    public class Seat
    {
        public int Number { get; }
        public SeatType Type { get; }
        public SeatStatus Status { get; set; } = SeatStatus.Ledig;

        public Seat(int number, SeatType type)
        {
            Number = number;
            Type = type;
        }
    }
}
