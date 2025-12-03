namespace MedicalAir.Model.Entites
{
    public class HistoryUpMedicin
    {
        public int Id { get; set; }

        public int Count { get; set; }
        public string? Name { get; set; }
        public DateOnly UpData { get; set; }
        public DateOnly EndData { get; set; }
        public bool IsValid { get; set; }

        public List<Medicin>? Medicins { get; set; }

        public HistoryUpMedicin(int count, string? name, DateOnly upData, DateOnly endData, bool isValid)
        {
            Count = count;
            Name = name;
            UpData = upData;
            EndData = endData;
            IsValid = isValid;
        }

        public HistoryUpMedicin() { }
    }
}
