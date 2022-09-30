using System;

#nullable disable warnings

namespace Musmetaniac.Services.LastFmApi.Models.User
{
    public class GetRecentTracksRequestModel
    {
        public string Username { get; set; }
        public int? Limit { get; set; }
        public DateTime? From { get; set; }
    }
}
