using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sheep.Data;                     // ApplicationDbContext の名前空間
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SheepEntity = sheep.Pages;


namespace sheep.Pages.sheeps
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            // ログインユーザーの羊だけ削除
            var sheep = await _db.Sheeps.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (sheep == null)
                return NotFound();
            

            _db.Sheeps.Remove(sheep);
            await _db.SaveChangesAsync();

            return new OkResult(); // JS 側の fetch で成功判定
        }
    }
}
