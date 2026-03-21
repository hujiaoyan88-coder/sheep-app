using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sheep.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;



namespace sheep.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }



        [BindProperty]
        public required string Name { get; set; }

        [BindProperty]
        public required string Color { get; set; }

        // ���O�C�����[�U�[�̗r�����Ԃ�
        public List<SheepEntity> Sheeps { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            Sheeps = await _db.Sheeps
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }




        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);

            int sheepCount = await _db.Sheeps.CountAsync(s => s.UserId == userId);

            if (sheepCount >= 15)
            {
                TempData["ErrorMessage"] = "羊は15匹まで登録できます。";
                return RedirectToPage();
            }

            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Color))
            {
                // ���O�̏d���`�F�b�N
                bool exists = await _db.Sheeps.AnyAsync(s => s.UserId == userId && s.Name == Name);
                if (exists)
                {
                    TempData["ErrorMessage"] = "この名前の羊はすでに登録されています。";
                    return RedirectToPage();
                }

                var sheep = new SheepEntity
                {
                    Name = Name,
                    Color = Color,
                    UserId = userId
                };

                _db.Sheeps.Add(sheep);
                await _db.SaveChangesAsync();
            }

            // Name �܂��� Color ����ł�A�K���y�[�W��Ԃ�
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportPdfAsync()
        {
            var userId = _userManager.GetUserId(User);

            var sheeps = await _db.Sheeps
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (sheeps.Count == 0)
            {
                TempData["ErrorMessage"] = "出力する羊がいません。";
                return RedirectToPage();
            }

            // PDF出力
            var outputPath = Path.Combine(Path.GetTempPath(), "sheep_list.pdf");

            try
            {
                // ここでPDFを生成
                PdfHelper.ExportSheepListToPdf(outputPath, sheeps);
            }
            catch (Exception ex)
            {
                // 例外発生時はメッセージを画面に表示
                TempData["ErrorMessage"] = "PDF出力中にエラーが発生しました: " + ex.Message;
                return RedirectToPage();
            }

            // 成功時はPDFをダウンロード
            return File(System.IO.File.ReadAllBytes(outputPath), "application/pdf", "sheep_list.pdf");
        }



    }

}




