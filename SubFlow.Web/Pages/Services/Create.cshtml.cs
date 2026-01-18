using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SubFlow.Application.Services.Commands;

namespace SubFlow.Web.Pages.Services;

public class CreateModel : PageModel
{
    private readonly CreateServiceCommand _command;

    public CreateModel(CreateServiceCommand command)
    {
        _command = command;
    }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(nameof(Name), "Name is required.");
            return Page();
        }

        await _command.ExecuteAsync(Name.Trim(), ct);
        return RedirectToPage("/Services/Index");
    }
}
