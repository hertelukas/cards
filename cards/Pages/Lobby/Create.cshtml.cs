using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class CreateModel : PageModel
{
    public void OnGet()
    {
        
    }
}