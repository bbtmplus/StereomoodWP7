
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.BackgroundAudio;
using TuneYourMood.Json;
using StereomoodPlaybackAgent;
using Song = StereomoodPlaybackAgent.Song;


namespace TuneYourMood
{

    public class CurrentItemCollections
    {
        private static CurrentItemCollections instance;

        public Tag currentMood;
        public Song currentSong;
        public int currentTrackNumber;

        private Dictionary<string, List<Song>> songsForTagDictionary;
        public List<AudioTrack> audioTracks;
        public List<Song> searchResult;
        public List<Tag> selectedTags;
        public List<Tag> topTags;
        public List<Tag> favorites;

        private IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public Dictionary<string, List<Song>> getSongsForTagDictionary()
        {
            return songsForTagDictionary = songsForTagDictionary ?? new Dictionary<string, List<Song>>();
        }

        public static CurrentItemCollections Instance()
        {
            return instance = instance ?? new CurrentItemCollections();
        }

        public AudioTrack convertSongToAudioTrack(Song song)
        {
            return new AudioTrack(song.audio_url,
                song.title,
                song.artist,
                song.album,
                song.image_url);
        }

        public List<AudioTrack> convertSongsToAudioTracks(List<Song> songs)
        {
            return songs.Select(song => new AudioTrack(song.audio_url,
                song.title,
                song.artist,
                song.album,
                song.image_url)).ToList();
        }

        internal void SaveApplicationState()
        {
            StorageUtility.writeObjectToFile(isf, "currentMood.txt", currentMood);
            StorageUtility.writeObjectToFile(isf, "currentSong.txt", currentSong);
            StorageUtility.writeListToFile(isf, "selectedTags.txt", selectedTags);
            StorageUtility.writeListToFile(isf, "topTags.txt", topTags);
            StorageUtility.writeObjectToFile(isf, "songsForTagDictionary.txt", songsForTagDictionary);
        }

        internal void LoadApplicationState()
        {
            currentMood = StorageUtility.readObjectFromFile<Tag>(isf, "currentMood.txt");
            currentSong = StorageUtility.readObjectFromFile<Song>(isf, "currentSong.txt");
            selectedTags = StorageUtility.readListFromFile<Tag>(isf, "selectedTags.txt");
            topTags = StorageUtility.readListFromFile<Tag>(isf, "topTags.txt");
            songsForTagDictionary = StorageUtility.readObjectFromFile<Dictionary<string, List<Song>>>(isf, "songsForTagDictionary.txt");
        }
    }
}