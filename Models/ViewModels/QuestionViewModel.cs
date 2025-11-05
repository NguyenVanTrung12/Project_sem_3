namespace Project_sem_3.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Score { get; set; }

        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }

    public class AnswerViewModel
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }  // có thể ẩn ở client
    }
}
