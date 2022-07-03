using System.ComponentModel.DataAnnotations;

namespace Musmetaniac.Web.Common.Models
{
    public class RecentTracksRequestModel
    {
        [Required]
        public string Username { get; set; }

        public int? Limit { get; set; }
    }
}
