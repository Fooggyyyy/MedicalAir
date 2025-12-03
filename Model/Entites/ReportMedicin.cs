namespace MedicalAir.Model.Entites
{
    public class ReportMedicin
    {
        public int Id { get; set; }

        public DateOnly DataStart { get; set; }
        public DateOnly DataEnd { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalUniqueMedicines { get; set; }
        public int TotalMedkit { get; set; }
        public int ExpiredCount { get; set; }
        public int AlmostExpiredCount { get; set; }
        public int ExpiredCountPercent { get; set; }
        public int AlmostExpiredCountPercent { get; set; }

        public ReportMedicin(DateOnly dataStart, DateOnly dataEnd, int totalMedicines, int totalUniqueMedicines, int totalMedkit, int expiredCount, int almostExpiredCount, int expiredCountPercent, int almostExpiredCountPercent)
        {
            DataStart = dataStart;
            DataEnd = dataEnd;
            TotalMedicines = totalMedicines;
            TotalUniqueMedicines = totalUniqueMedicines;
            TotalMedkit = totalMedkit;
            ExpiredCount = expiredCount;
            AlmostExpiredCount = almostExpiredCount;
            ExpiredCountPercent = expiredCountPercent;
            AlmostExpiredCountPercent = almostExpiredCountPercent;
        }

        public ReportMedicin() { }
    }
}
