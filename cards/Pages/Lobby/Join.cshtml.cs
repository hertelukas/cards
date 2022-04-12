using System.ComponentModel.DataAnnotations;
using cards.Data;
using cards.Data.Game.Decks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class JoinModel : PageModel
{
    private readonly ILobbyService _lobbyService;

    public JoinModel(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    [BindProperty] public InputModel Input { get; set; }

    public int Id { get; set; }

    public void OnGet(int id)
    {
        Id = id;
    }

    public IActionResult OnPost()
    {
        try
        {
            var username = User.Identity?.Name;
            return _lobbyService.JoinLobby(Input.Id, Input.Password, username ?? throw new NullReferenceException())
                switch
                {
                    Data.Response.Success => RedirectToPage("Index", new {Input.Id}),
                    _ => Page()
                };
        }
        catch (NullReferenceException)
        {
            return Page();
        }
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Lobby ID")]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password for joining the lobby")]
        public string Password { get; set; }
    }
}