using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class JoinModel : PageModel
{
    public void OnGet()
    {
        
    }
}