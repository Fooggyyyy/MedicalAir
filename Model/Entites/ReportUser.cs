namespace MedicalAir.Model.Entites
{
    public class ReportUser
    {
        public int Id { get; set; }

        public DateOnly DataStart { get; set; }
        public DateOnly DataEnd { get; set; }
        public int TotalUsers { get; set; }
        public int TotalUsersME { get; set; }
        public int Passed { get; set; }
        public int NotPassed { get; set; }
        public int PassedPercent { get; set; }
        public int NotPassedPercent { get; set; }

        public ReportUser(DateOnly dataStart, DateOnly dataEnd, int totalUsers, int totalUsersME, int passed, int notPassed, int passedPercent, int notPassedPercent)
        {
            DataStart = dataStart;
            DataEnd = dataEnd;
            TotalUsers = totalUsers;
            TotalUsersME = totalUsersME;
            Passed = passed;
            NotPassed = notPassed;
            PassedPercent = passedPercent;
            NotPassedPercent = notPassedPercent;
        }

        public ReportUser() { }
    }
}
