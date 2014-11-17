namespace aulease.Entities

{

    public partial class Status
    {

        public const int OrderedByUser = 1;
        public const int OrderedByAULease = 2;
        public const int Scanned = 3;
        public const int Prepped = 4;
        public const int Delivered = 5;
        public const int BackInShop = 6;
        public const int Cleaned = 7;
        public const int Diagnosed = 8;
        public const int DataWiped = 9;
        public const int ReadyToShip = 10;
        public const int NewRepair = 11;
        public const int CompletedRepair = 12;
        public const int NewTask = 13;
        public const int CompletedTask = 14;

    }

    public enum Statuses
    {
        OrderedByUser = Status.OrderedByUser,
        OrderedByAULease = Status.OrderedByAULease,
        Scanned = Status.Scanned,
        Prepped = Status.Prepped,
        Delivered = Status.Delivered,
        BackInShop = Status.BackInShop,
        Cleaned = Status.Cleaned,
        Diagnosed = Status.Diagnosed,
        DataWiped = Status.DataWiped,
        ReadyToShip = Status.ReadyToShip,
        NewRepair = Status.NewRepair,
        CompletedRepair = Status.CompletedRepair,
        NewTask = Status.NewTask,
        CompletedTask = Status.CompletedTask
    }
}
