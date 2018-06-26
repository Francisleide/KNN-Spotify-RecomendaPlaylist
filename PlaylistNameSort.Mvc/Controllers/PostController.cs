using PlaylistNameSort.Domain.Interfaces;
using PlaylistNameSort.Domain.Models;
using PlaylistNameSort.Domain.Services;
using PlaylistNameSort.Mvc.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlaylistNameSort.Mvc.Controllers
{
    public class PostController : Controller
    {
     /*   private readonly SpotifyAuthViewModel _spotifyAuthViewModel;
        private readonly ISpotifyApi _spotifyApi;
        
        // GET: Post
        public PostController(SpotifyAuthViewModel spotifyAuthViewModel, ISpotifyApi spotifyApi)
        {
            _spotifyAuthViewModel = spotifyAuthViewModel;
            _spotifyApi = spotifyApi;
        }
        public ActionResult Post(string access_token, string error)
        {
            
             string uriCallback = "http:%2F%2Flocalhost:12029%2FPost%2FPost";
             string clientId = "215f619c52da4befaa569f12a2108b41";
             string completo = "https://accounts.spotify.com/en/authorize?client_id=" + clientId +
                  "&response_type=token&redirect_uri=" + uriCallback +
                  "&state=&scope=" + Scope.PLAYLIST_MODIFY_PRIVATE.GetStringAttribute(" ") +
                  "&show_dialog=true";
            // ViewBag.AuthUri = completo;
         //   NinjectWebCommon.CreateKernelDois();
            ViewBag.AuthUri  = completo;
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
              //  ViewBag.UserName = spotifyUser.DisplayName;
                //spotifyService.PostPlays()
            return View();
        }*/
    }
}