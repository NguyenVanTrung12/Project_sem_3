namespace Project_sem_3.Models
{
    public class ReportViewModel
    {

        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string SubjectName { get; set; }
        public double TotalMark { get; set; }
        public DateTime SubmitDate { get; set; }
        public bool Passed { get; set; }

    }
}
