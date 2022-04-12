using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class IndexModel : PageModel
{
    public int Id { get; set; }
    public void OnGet(int id)
    {
        Id = id;
    }
}