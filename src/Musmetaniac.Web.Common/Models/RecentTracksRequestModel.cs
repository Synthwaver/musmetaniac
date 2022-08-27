#nullable disable warnings

namespace Musmetaniac.Web.Common.Models
{
    public class RecentTracksRequestModel
    {
        public string Username { get; set; }
        public int? Limit { get; set; }
    }
}
