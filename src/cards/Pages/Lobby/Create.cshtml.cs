using System.ComponentModel.DataAnnotations;
using cards.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages.Lobby;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ILobbyService _lobbyService;

    public CreateModel(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    [BindProperty] public InputModel Input { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var username = User.Identity?.Name;

            return RedirectToPage("Index",
                new
                {
                    id = _lobbyService.CreateLobby(username ?? throw new NullReferenceException(), Input.Password)
                });
        }
        catch (NullReferenceException)
        {
            return Page();
        }
    }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 2)]
        [Display(Name = "Password for joining the lobby")]
        public string Password { get; set; }
    }
}