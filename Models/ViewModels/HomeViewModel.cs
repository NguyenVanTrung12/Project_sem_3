
namespace Project_sem_3.Models.ViewModels
{
    public class HomeViewModel 
    {
        public IEnumerable<Subject> Subjects { get; set; } = new List<Subject>();
        public Contact Contact { get; set; } = new Contact();

        // Nếu cần thêm các dữ liệu khác trong ViewBag, bạn có thể thêm luôn:
        public IEnumerable<Banner>? Banner { get; set; }
        public IEnumerable<Manager>? Manager { get; set; }
        public IEnumerable<Blog>? Blog { get; set; }
    }
}
