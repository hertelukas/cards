using cards.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILobbyService _lobbyService;

    public IndexModel(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    public int Id { get; set; }
    public Data.Lobby Lobby { get; private set; }
    public string Username { get; private set; }

    public IActionResult OnGet(int id)
    {
        Id = id;

        // Check whether the user is allowed to access this lobby
        try
        {
            var username = User.Identity?.Name;
            
            try
            {
                Lobby = _lobbyService.GetLobby(Id);
            }
            catch (ArgumentOutOfRangeException)
            {
                return RedirectToPage("Create");
            }

            if (!_lobbyService.HasAccess(Id, username ?? throw new NullReferenceException()))
            {
                return RedirectToPage("Join", new {Id});
            }


            Username = username;

            return Page();
        }
        catch (NullReferenceException)
        {
            return RedirectToPage("/");
        }
    }
}