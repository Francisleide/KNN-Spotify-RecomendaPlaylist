using PlaylistNameSort.Domain.Interfaces;
using PlaylistNameSort.Domain.Models;
using PlaylistNameSort.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlaylistNameSort.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly SpotifyAuthViewModel _spotifyAuthViewModel;
        private readonly ISpotifyApi _spotifyApi;
        List<PlaylistPronta> playlistProntas = new List<PlaylistPronta>();

        public HomeController(SpotifyAuthViewModel spotifyAuthViewModel, ISpotifyApi spotifyApi)
        {
            _spotifyAuthViewModel = spotifyAuthViewModel;
            _spotifyApi = spotifyApi;
        }

        public ActionResult Index()
        {
            ViewBag.AuthUri = _spotifyAuthViewModel.GetAuthUri();

            return View();
        }

        public ActionResult GenerateNameSortList(string access_token, string error)
        {
            if (error != null || error == "access_denied")
                return View("Error");

            if (string.IsNullOrEmpty(access_token))
                return View();

            try
            {
                _spotifyApi.Token = access_token;
                SpotifyService spotifyService = new SpotifyService(_spotifyApi);
                //Get user_id and user displayName
                SpotifyUser spotifyUser = spotifyService.GetUserProfile();
                ViewBag.UserName = spotifyUser.DisplayName;

                //Get user playlists ids
                //   Playlists playlists = spotifyService.GetPlaylists(spotifyUser.UserId);

                //Get all tracks from user
                //   List<string> tracks = spotifyService.GetTracksAndArtistsFromPlaylists(playlists);

                //Generate the new playlist 
                //  List<string> newPlayList = spotifyService.GenerateNewPlaylist(spotifyUser.DisplayName, tracks);

                Tracks tocadasRecentemente = spotifyService.GetRecentlyPlayed();
                List<Audio> metaAudios = spotifyService.GetAudioTracks(tocadasRecentemente);
                // var x = spotifyService.MeuK(metaAudios);
                metaAudios = spotifyService.calcDistancias(metaAudios);

                playlistProntas = spotifyService.Knn(metaAudios);

                string uriCallback = "http:%2F%2Flocalhost:12029%2FHome%2FPost";
                string clientId = "215f619c52da4befaa569f12a2108b41";
                string completo = "https://accounts.spotify.com/en/authorize?client_id=" + clientId +
                     "&response_type=token&redirect_uri=" + uriCallback +
                     "&state=&scope=" + Scope.PLAYLIST_MODIFY_PRIVATE.GetStringAttribute(" ") +
                     "&show_dialog=true";
                ViewBag.AuthUri = completo;

                View("Teste", playlistProntas);
                Post(access_token, error, playlistProntas);
                return View("Teste", playlistProntas);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        public ActionResult Post(string access_token, string error, List<PlaylistPronta> playlistProntas)
        {
            string uriCallback = "http:%2F%2Flocalhost:12029%2FHome%2FPost";
            string clientId = "215f619c52da4befaa569f12a2108b41";
            string completo = "https://accounts.spotify.com/en/authorize?client_id=" + clientId +
                 "&response_type=token&redirect_uri=" + uriCallback +
                 "&state=&scope=" + Scope.PLAYLIST_MODIFY_PRIVATE.GetStringAttribute(" ") +
                 "&show_dialog=true";
            ViewBag.AuthUri = completo;
            if (error != null || error == "access_denied")
                return View("Error");

            if (string.IsNullOrEmpty(access_token))
                return View();

            try
            {
                _spotifyApi.Token = access_token;
                SpotifyService spotifyService = new SpotifyService(_spotifyApi);
                //Get user_id and user displayName
                SpotifyUser spotifyUser = spotifyService.GetUserProfile();
                // ViewBag.UserName = spotifyUser.DisplayName;
                foreach (var playlistPronta in playlistProntas)
                {
                    spotifyService.PostPlays(playlistPronta, spotifyUser.UserId);
                 }
                

                //Get user playlists ids
                //   Playlists plays = spotifyService.GetPlaylists(spotifyUser.UserId);

                //Get all tracks from user
                // List<string> faixas = spotifyService.GetTracksAndArtistsFromPlaylists(plays);

                //  var lista = spotifyService.GetRecentlyPlayed();
                // var plays = spotifyService.GetAudioTracks(lista);

                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }

        }
    }
}
