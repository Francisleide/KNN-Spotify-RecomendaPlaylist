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

                List<PlaylistPronta> playlistProntas = spotifyService.Knn(metaAudios);

                return View("Teste",playlistProntas);
            }
            catch (Exception)
            {
                return View("Error");
            }
            
        }

        public ActionResult Teste(string access_token, string error)
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
             //   Playlists plays = spotifyService.GetPlaylists(spotifyUser.UserId);

                //Get all tracks from user
               // List<string> faixas = spotifyService.GetTracksAndArtistsFromPlaylists(plays);

                var lista = spotifyService.GetRecentlyPlayed();
                var plays = spotifyService.GetAudioTracks(lista);

                return View(lista);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }
    }
}
