using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelloWorld.Pages
{
    public class backchannel_logoutModel : PageModel
    {
        public async Task OnPostAsync()
        {
            // For backcchannel logout to work, we will need a collection of all logged in users.
            // this is not supported in this demo.
        }
    }
}
