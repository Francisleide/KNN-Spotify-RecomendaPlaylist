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

                Tracks tocadasRecentemente = spotifyService.GetRecentlyPlayed();
                List<Audio> metaAudios = spotifyService.GetAudioTracks(tocadasRecentemente);
                // var x = spotifyService.MeuK(metaAudios);
                metaAudios = spotifyService.calcDistancias(metaAudios);

                playlistProntas = spotifyService.Knn(metaAudios);

                string uriCallback = "http:%2F%2Fplaylistlistknn.azurewebsites.net%2FHome%2FPost";
                string clientId = spotifyUser.UserId;
                string completo = "https://accounts.spotify.com/en/authorize?client_id=" + clientId +
                     "&response_type=token&redirect_uri=" + uriCallback +
                     "&state=&scope=" + Scope.PLAYLIST_MODIFY_PRIVATE.GetStringAttribute(" ") +
                     "&show_dialog=true";
                ViewBag.AuthUri = completo;

                View("Teste", playlistProntas);
                Post(access_token, error, playlistProntas, spotifyUser);
                return View("Teste", playlistProntas);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        public ActionResult Post(string access_token, string error, List<PlaylistPronta> playlistProntas, SpotifyUser spotifyUser)
        {
            string uriCallback = Server.MapPath("~") + "/Home/Post"; 
            string clientId = spotifyUser.UserId;
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
               // SpotifyUser spotifyUser = spotifyService.GetUserProfile();
                 ViewBag.UserName = spotifyUser.DisplayName;
                foreach (var playlistPronta in playlistProntas)
                {
                    spotifyService.PostPlays(playlistPronta, access_token,spotifyUser.UserId);
                 }
                
                return View();
            }
            catch (Exception e)
            {
                return View("Error",e);
            }

        }
    }
}
