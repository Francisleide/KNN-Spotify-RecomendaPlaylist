namespace PlaylistNameSort.Domain.Interfaces
{
    public interface ISpotifyApi
    {
        string Token { get; set; }

        T GetSpotifyType<T>(string url);
        T PostSpotifyType<T>(string url, string PToken, string json);
    }
}