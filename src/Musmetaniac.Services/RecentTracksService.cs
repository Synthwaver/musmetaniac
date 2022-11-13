using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Musmetaniac.Common.Exceptions;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Services.Exceptions;
using Musmetaniac.Services.LastFmApi;
using Musmetaniac.Services.LastFmApi.Models.Track;
using Musmetaniac.Services.LastFmApi.Models.User;

namespace Musmetaniac.Services
{
    public interface IRecentTracksService
    {
        Task<IReadOnlyCollection<Track>> GetRecentTracks(string username, int? limit = null, DateTime? from = null);
    }

    public class RecentTracksService : IRecentTracksService
    {
        private const int MinRecentTracksToRequest = 1;
        private const int MaxRecentTracksToRequest = 50;

        private readonly ServiceAppSettings _serviceAppSettings;

        public RecentTracksService(ServiceAppSettings serviceAppSettings)
        {
            _serviceAppSettings = serviceAppSettings;
        }

        public async Task<IReadOnlyCollection<Track>> GetRecentTracks(string username, int? limit = null, DateTime? from = null)
        {
            if (username.IsNullOrEmpty())
                throw new BusinessException("Username is required.");

            if (limit.HasValue && (limit.Value < MinRecentTracksToRequest || limit.Value > MaxRecentTracksToRequest))
                throw new BusinessException($"Limit must be in range {MinRecentTracksToRequest}-{MaxRecentTracksToRequest}.");

            using var lastFmApiClient = new LastFmApiClient(_serviceAppSettings.LastFmApiKey);

            GetRecentTracksModel recentTracksModel;
            try
            {
                recentTracksModel = await lastFmApiClient.User.GetRecentTracksAsync(new GetRecentTracksRequestModel
                {
                    Username = username,
                    Limit = limit,
                    From = from,
                });
            }
            catch (LastFmApiRequestException exception)
            {
                if (exception.Message.Contains("User not found"))
                    throw new BusinessException("User not found.");

                throw;
            }

            var tracks = recentTracksModel.Tracks.Select(t => new Track
            {
                Name = t.Name,
                ArtistName = t.Artist.Name,
                AlbumName = t.Album.Name,
                Url = t.Url,
                IsPlayingNow = t.IsPlayingNow,
                ScrobbledAt = t.ScrobbledAt,
            }).ToArray();

            var topTagsModelTasks = tracks.Select(t => lastFmApiClient.Track.GetTopTagsAsync(new GetTopTagsRequestModel
            {
                Artist = t.ArtistName,
                Track = t.Name,
            })).ToArray();

            var topTagsModels = await Task.WhenAll(topTagsModelTasks);

            for (var i = 0; i < tracks.Length; i++)
            {
                tracks[i].Tags = topTagsModels[i].Tags.Select(t => new Track.Tag
                {
                    Name = t.Name,
                    Url = t.Url,
                }).ToArray();
            }

            return tracks;
        }
    }
}
