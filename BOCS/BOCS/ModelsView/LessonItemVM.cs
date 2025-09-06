
namespace BOCS.ModelsView
{
    public class LessonItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string YoutubeId { get; set; } = "";
        public int SortOrder { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
