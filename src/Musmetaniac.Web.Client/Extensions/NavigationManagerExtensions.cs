using Microsoft.AspNetCore.Components;

namespace Musmetaniac.Web.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static void UpdateQueryParameter(this NavigationManager self, string name, string value)
        {
            self.NavigateTo(self.GetUriWithQueryParameter(name, value == "" ? null : value));
        }
    }
}
