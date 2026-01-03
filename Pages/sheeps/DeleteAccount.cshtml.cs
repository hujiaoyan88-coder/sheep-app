using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sheep.Data;


public class DeleteAccountModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public DeleteAccountModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context) // ← ★ここが重要
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Index");
        }

        await _signInManager.SignOutAsync();


        // 🐏 このユーザーの羊を削除
        var sheeps = _context.Sheeps.Where(s => s.UserId == user.Id);
        _context.Sheeps.RemoveRange(sheeps);
        await _context.SaveChangesAsync();

        // 👤 ユーザー削除
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            // ログ出力など
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }

        return RedirectToPage("/Index");
    }
}

