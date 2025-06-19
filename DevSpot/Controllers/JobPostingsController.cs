using DevSpot.Constants;
using DevSpot.Models;
using DevSpot.Repositories;
using DevSpot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSpot.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly IRepository<JobPosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public JobPostingsController(
            IRepository<JobPosting> repository,
            UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Проверяем, вошёл ли пользователь как работодатель
            if (User.IsInRole(Roles.Employer))
            {
                // 1. Загружаем ВСЕ вакансии из базы (пока без фильтрации)
                var allJobPostings = await _repository.GetAllAsync();

                // 2. Получаем ID текущего вошедшего пользователя
                var userId = _userManager.GetUserId(User);

                // 3. Фильтруем вакансии: оставляем только те, где UserId совпадает с текущим пользователем
                var filteredJobPostings = allJobPostings.Where(jp => jp.UserId == userId);

                // 4. Возвращаем отфильтрованные вакансии в представление
                return View(filteredJobPostings);
            }
            // Если пользователь НЕ работодатель (например, админ или соискатель),
            // просто возвращаем все вакансии
            var jobPostings = await _repository.GetAllAsync();

            return View(jobPostings);
        }

        [Authorize(Roles ="Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(JobPostingViewModel jobPostingVm)
        {
            if (ModelState.IsValid)
            {
                var jobPosting = new JobPosting
                {
                    Title = jobPostingVm.Title,
                    Description = jobPostingVm.Description,
                    Company = jobPostingVm.Company,
                    Location = jobPostingVm.Location,
                    UserId = _userManager.GetUserId(User)
                };

                await _repository.AddAsync(jobPosting);

                return RedirectToAction(nameof(Index));
            }

            return View(jobPostingVm);
        }

        // JobPosting/Delete/5
        [HttpDelete]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var jobPosting = await _repository.GetByIdAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if(User.IsInRole(Roles.Admin) == false && jobPosting.UserId != userId)
            {
                return Forbid();
            }

            await _repository.DeleteAsync(id);

            return Ok();
        }
    }
}
