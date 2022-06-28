using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelloWorld.Pages
{
    public class ListMyClaimsModel : PageModel
    {
        private readonly ILogger<ListMyClaimsModel> _logger;

        public ListMyClaimsModel(ILogger<ListMyClaimsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}