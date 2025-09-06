using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOCS.Models
{
    public class CourseEnrollment
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;

        // Identity user id
        public string StudentId { get; set; } = default!;

        // Admin approve করলে true
        public bool IsApproved { get; set; } = false;

        // পুরোনো/বন্ধ এনরোলমেন্ট hide করার জন্য
        public bool IsArchived { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // new
        public CourseAccessType AccessType { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }

        [MaxLength(64)] public string? TransactionId { get; set; }
        [MaxLength(20)] public string? SenderNumber { get; set; }
        [MaxLength(20)] public string? MobileNumber { get; set; }

        // snapshot of price at the time of enrollment
        public int PriceAtEnrollment { get; set; }
    }
}
