#nullable disable

using System.ComponentModel.DataAnnotations;
using cards.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace cards.Pages.Lobby;

[Authorize]
public class JoinModel : PageModel
{
    private readonly ILobbyService _lobbyService;
    private readonly ILogger<JoinModel> _logger;

    public JoinModel(ILobbyService lobbyService, ILogger<JoinModel> logger)
    {
        _lobbyService = lobbyService;
        _logger = logger;
    }

    [BindProperty] public InputModel Input { get; set; }

    public int Id { get; set; }

    [TempData] public string ErrorMessage { get; set; }

    public void OnGet(int id)
    {
        Id = id;
    }

    public IActionResult OnPost()
    {
        try
        {
            var username = User.Identity?.Name;
            NotificationMessageModel error;

            switch (_lobbyService.JoinLobby(Input.Id, Input.Password, username ?? throw new NullReferenceException()))
            {
                case Data.Response.Success:
                    if (!_lobbyService.GetLobby(Input.Id).HasStarted) return RedirectToPage("Index", new {Input.Id});

                    _logger.LogInformation("{Username} tried to join {LobbyId}, but lobby already started",
                        username, Input.Id);
                    error = new NotificationMessageModel(NotificationMessageModel.Level.Danger,
                        "Lobby already started");
                    break;
                case Data.Response.NotFound:
                    error =
                        new NotificationMessageModel(NotificationMessageModel.Level.Danger, "Lobby not found");
                    break;
                case Data.Response.InvalidPassword:
                    error =
                        new NotificationMessageModel(NotificationMessageModel.Level.Danger, "Invalid password");
                    break;
                default:
                    error =
                        new NotificationMessageModel(NotificationMessageModel.Level.Danger, "Something went wrong");
                    break;
            }

            ErrorMessage = JsonConvert.SerializeObject(error);
            Console.WriteLine(ErrorMessage);
            return RedirectToPage("/Lobby/Join");
        }
        catch (NullReferenceException)
        {
            ErrorMessage =
                JsonConvert.SerializeObject(new NotificationMessageModel(NotificationMessageModel.Level.Danger,
                    "Something went wrong"));
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