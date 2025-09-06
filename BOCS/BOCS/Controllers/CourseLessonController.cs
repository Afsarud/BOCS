using BOCS.Data;
using BOCS.Models;
using BOCS.ModelsView;
using BOCS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BOCS.Controllers
{
    /// Admin only: manage lessons under a course
    [Authorize(Roles = "Admin")]
    [Route("admin/course-lessons")]
    public class CourseLessonController : Controller
    {
        private readonly AppDbContext _db;
        public CourseLessonController(AppDbContext db) => _db = db;
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var courses = await _db.Courses
                .AsNoTracking()
                .OrderBy(c => c.Title)
                .Select(c => new CourseMiniVM
                {
                    Id = c.Id,
                    Title = c.Title,
                    LessonCount = _db.Lessons.Count(l => l.CourseId == c.Id)
                })
                .ToListAsync();

            return View("Index", courses); 
        }

        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> Manage(int courseId)
        {
            var course = await _db.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            var vm = new LessonManageVM
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                Lessons = await _db.Lessons
                    .Where(l => l.CourseId == courseId)
                    .OrderBy(l => l.SortOrder)
                    .Select(l => new LessonItemVM
                    {
                        Id = l.Id,
                        Title = l.Title,
                        YoutubeId = l.YoutubeId,
                        SortOrder = l.SortOrder,
                        IsPublished = l.IsPublished,
                        CreatedAtUtc = l.CreatedAtUtc
                    })
                    .ToListAsync()
            };

            return View("Manage", vm);
        }

        //[HttpGet("{courseId:int}/create")]
        //public IActionResult Create(int courseId) =>
        //    View("Create", new LessonCreateVM { CourseId = courseId });

        [HttpGet("{courseId:int}/create")]
        public async Task<IActionResult> Create(int courseId)
        {
            ViewBag.Subjects = await _db.Subjects
                .Where(s => s.CourseId == courseId && s.IsPublished)
                .OrderBy(s => s.SortOrder)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Title })
                .ToListAsync();

            return View(new LessonCreateVM { CourseId = courseId });
        }

        [HttpPost("{courseId:int}/create"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, LessonCreateVM vm)
        {
            if (courseId != vm.CourseId) return BadRequest();

            var ytId = YoutubeHelper.ExtractId(vm.YoutubeUrlOrId);
            if (ytId == null)
                ModelState.AddModelError(nameof(vm.YoutubeUrlOrId), "Invalid YouTube URL or ID.");

            if (!ModelState.IsValid) return View("Create", vm);

            _db.Lessons.Add(new CourseLesson
            {
                CourseId = courseId,
                Title = vm.Title,
                YoutubeId = ytId,
                YoutubeUrlRaw = vm.YoutubeUrlOrId,
                SortOrder = vm.SortOrder,
                IsPublished = vm.IsPublished,
                SubjectId = vm.SubjectId  //new added subject
            });

            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "✅ Lesson created.";
            return RedirectToAction(nameof(Manage), new { courseId });
        }

        [HttpGet("{courseId:int}/delete/{id:int}")]
        public async Task<IActionResult> Delete(int courseId, int id)
        {
            var l = await _db.Lessons.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.CourseId == courseId);
            if (l == null) return NotFound();

            return View("Delete", new LessonDeleteVM
            {
                CourseId = courseId,
                Id = id,
                Title = l.Title
            });
        }

        [HttpPost("{courseId:int}/delete/{id:int}"), ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int courseId, int id)
        {
            var l = await _db.Lessons
                .FirstOrDefaultAsync(x => x.Id == id && x.CourseId == courseId);
            if (l == null) return NotFound();

            _db.Lessons.Remove(l);
            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "🗑️ Lesson deleted.";
            return RedirectToAction(nameof(Manage), new { courseId });
        }
    }
}
