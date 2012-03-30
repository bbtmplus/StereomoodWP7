
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.BackgroundAudio;
using Stereomood.Json;
using Song = StereomoodPlaybackAgent.Song;


namespace Stereomood
{

    public class CurrentItemCollections
    {
        private static CurrentItemCollections instance;

        public Tag currentMood;
        public Song currentSong;
        public int currentTrackNumber;

        private Dictionary<string, List<AudioTrack>> tracksForTagDictionary;
        private Dictionary<string, List<Song>> songsForTagDictionary;
        public List<AudioTrack> audioTracks;
        public List<Song> searchResult;
        public List<Tag> selectedTags;
        public List<Tag> topTags;
        public List<Tag> favorites;

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
    }
}