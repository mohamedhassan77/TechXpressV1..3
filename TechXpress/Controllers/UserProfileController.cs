using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "User")]

    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _repository;

        public UserProfileController(IUserProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var userProfiles = await _repository.GetAllAsync();
            return View(userProfiles);
        }

        public async Task<IActionResult> Details(string id)
        {
            var userProfile = await _repository.GetByIdAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            return View(userProfile);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(userProfile);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProfile);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userProfile = await _repository.GetByIdAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserProfile userProfile)
        {
            if (id != userProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(userProfile);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProfile);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var userProfile = await _repository.GetByIdAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            return View(userProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userProfile = await _repository.GetByIdAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(userProfile);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
