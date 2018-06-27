using Newtonsoft.Json;
using PlaylistNameSort.Domain.Interfaces;
using PlaylistNameSort.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace PlaylistNameSort.Domain.Services
{
    public class SpotifyService
    {
        private ISpotifyApi _spotifyApi;

        public SpotifyService(ISpotifyApi spotifyApi)
        {
            _spotifyApi = spotifyApi;
        }

        public List<string> GetPlaylistsName(string userId)
        {
            Playlists playLists = GetPlaylists(userId);

            List<string> playlistNames = new List<string>();

            foreach (var playlist in playLists.Items)
            {
                playlistNames.Add(playlist.Name);
            }

            return playlistNames;
        }

        public SpotifyUser GetUserProfile()
        {
            string url = "https://api.spotify.com/v1/me";
            SpotifyUser spotifyUser = _spotifyApi.GetSpotifyType<SpotifyUser>(url);
            return spotifyUser;
        }
        public Tracks GetRecentlyPlayed()
        {
            string url = "https://api.spotify.com/v1/me/player/recently-played?limit=50";
            Tracks tracks = _spotifyApi.GetSpotifyType<Tracks>(url);
            return tracks;
        }

        public Playlists GetPlaylists(string userId)
        {
            string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists?", userId);
            Playlists playlists = _spotifyApi.GetSpotifyType<Playlists>(url);

            return playlists;
        }
        public Audio getAudio(string urlTrack)
        {
            string url = string.Format("https://api.spotify.com/v1/audio-features/{0}", urlTrack);
            Audio audio = _spotifyApi.GetSpotifyType<Audio>(url);
            return audio;
        }

        public List<Audio> GetAudioTracks(Tracks tracks)
        {

            List<Audio> metaAudios = new List<Audio>();

            foreach (var track in tracks.Items)
            {
                if (tracks == null)
                    continue;
                string music = track.FullTrack.Name;
                string urlTrack = track.FullTrack.Id;
                Audio audio = getAudio(urlTrack);
                metaAudios.Add(audio);
            }
            return metaAudios;
        }


        //Determinar o valor de K


        public List<Audio> calcDistancias(List<Audio> audios)
        {

            for (int i = 0; i < audios.Count(); i++)
            {
                List<Distancia> distancias = new List<Distancia>();
                Distancia dist;
                var audio = audios[i];
                foreach (var audio1 in audios)
                {
                    dist = new Distancia();
                    dist.Audio = audio1;
                    var distance = Math.Sqrt(Math.Pow(audio1.Danceability - audio.Danceability, 2) +
                              Math.Pow(audio1.Energy - audio.Energy, 2) +
                              Math.Pow(audio1.Liveness - audio.Liveness, 2) +
                              Math.Pow(audio1.Speechiness - audio.Speechiness, 2));
                    dist.DistanciaPara = distance;
                    if (dist.DistanciaPara == 0)
                    {
                        continue;
                    }
                    else
                    {
                        distancias.Add(dist);
                    }

                }
                audio.Distancias = distancias;


            }
            return audios;
        }


        public List<PlaylistPronta> Knn(List<Audio> audios)
        {

            foreach (var audio in audios)
            {
                List<Distancia> dists = audio.Distancias.OrderBy(d => d.DistanciaPara).ToList();
                audio.Distancias = dists;
                string url = string.Format("https://api.spotify.com/v1/tracks/" + audio.Id);
                FullTrack fullTrack = _spotifyApi.GetSpotifyType<FullTrack>(url);
                audio.FullTrack = fullTrack;
            }
            int cont = 0;
            List<PlaylistPronta> playlistProntas = new List<PlaylistPronta>();
            PlaylistPronta playPronta;
            for (int l = 0; l < 2; l++)
            {
                cont++;
                List<Audio> audiosPraPlays = new List<Audio>();
                playPronta = new PlaylistPronta();
                playPronta.Nome = "Minha Playlist" + cont;
                int qMusicas = 0;
                for (int m = 0; m < audios.Count(); m++)
                {
                    int mRepet = 0;

                    for (int n = 0; n < m; n++)
                    {
                        if (audios[l].Distancias[m].Audio != audios[l].Distancias[n].Audio)
                        {
                            continue;
                        }
                        else
                        {
                            mRepet++;
                        }
                    }
                    if (mRepet == 0)
                    {

                        audiosPraPlays.Add(audios[l].Distancias[m].Audio);
                        qMusicas++;
                    }
                    if (qMusicas == 15) break;

                }
                playPronta.audios = audiosPraPlays;
                playlistProntas.Add(playPronta);
            }

            return playlistProntas;
        }
        public Playlist PostPlays(PlaylistPronta playlistPronta, string access_token, string userId)
        {
            Tracks play = new Tracks();
            List<Track> items = new List<Track>();
            string json2 = "{\"uris\":[";
            string pedacoJson2 = "\"spotify:track:";
            string outropedaco = "";
            for (int i=0; i<playlistPronta.audios.Count(); i++) 
            {
                var t = new Track();
                t.FullTrack = playlistPronta.audios[i].FullTrack;
                items.Add(t);
                
                if(i== playlistPronta.audios.Count() - 1)
                {
                    outropedaco = outropedaco + pedacoJson2 + t.FullTrack.Id;
                }
                else
                {
                    outropedaco = outropedaco + pedacoJson2 + t.FullTrack.Id + "\",";
                }       
            }
            
            play.Items = items;
            string url = "https://api.spotify.com/v1/users/" + userId + "/playlists";
            string json = "{\"name\":\"" +playlistPronta.Nome+ "\",\"description\":\"Playlist gerada por algoritmo usando IA\", \"public\": false}";

            Playlist playlist = _spotifyApi.PostSpotifyType<Playlist>(url, access_token, json);
            string url2 = url + "/" + playlist.Id + "/tracks";
            json2 = json2 + outropedaco + "\"]}";
            Tracks tracks = _spotifyApi.PostSpotifyType<Tracks>(url2, access_token, json2);
            return playlist;
        }

    }

}



