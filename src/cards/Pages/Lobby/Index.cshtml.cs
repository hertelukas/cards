using cards.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

#nullable disable

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
    public string JoinLink { get; private set; }

    [TempData] public string ErrorMessage { get; set; }

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
                ErrorMessage = JsonConvert.SerializeObject(new NotificationMessageModel(
                    NotificationMessageModel.Level.Danger,
                    "You are not allowed to access this lobby"));
                return RedirectToPage("Join", new {Id});
            }

            Username = username;
            JoinLink = Url.Page("Join", null, new {id}, Request.Scheme);

            Console.WriteLine("Link: " + JoinLink);
            return Page();
        }
        catch (NullReferenceException)
        {
            return RedirectToPage("/");
        }
    }
}