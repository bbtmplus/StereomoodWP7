﻿using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Phone.BackgroundAudio;
using TuneYourMood.Json;
using StereomoodPlaybackAgent;
using Song = StereomoodPlaybackAgent.Song;

namespace TuneYourMood
{

    public class CurrentItemCollections
    {
        private static CurrentItemCollections instance;

        public string currentBackgroundKey = "Reflection";
        public Dictionary<string, BitmapImage> backgroundBrushes = new Dictionary<string, BitmapImage> { {"Dawn",new BitmapImage(new Uri("/Images/Backgrounds/bg1.jpg",UriKind.Relative))},
                                                                     {"Daylight",new BitmapImage(new Uri("/Images/Backgrounds/bg2.jpg",UriKind.Relative))},
                                                                     { "Blue Sky", new BitmapImage(new Uri("/Images/Backgrounds/bg3.jpg",UriKind.Relative))},
                                                                     {"The Web",new BitmapImage(new Uri("/Images/Backgrounds/bg4.jpg",UriKind.Relative))},
                                                                     {"Reflection",new BitmapImage(new Uri("/Images/Backgrounds/bg5.jpg",UriKind.Relative))},
                                                                     {"Evening",new BitmapImage(new Uri("/Images/Backgrounds/bg6.jpg",UriKind.Relative))},
                                                                     {"Clowdy Evening",new BitmapImage(new Uri("/Images/Backgrounds/bg7.jpg",UriKind.Relative))},
                                                                     { "Dusk&Clowds",new BitmapImage(new Uri("/Images/Backgrounds/bg8.jpg",UriKind.Relative))},
                                                                     { "Flower",new BitmapImage(new Uri("/Images/Backgrounds/bg9.jpg",UriKind.Relative))}};

        public Tag favoritesTag;
        public Tag currentMood;
        public int currentTrackNumber;

        private Dictionary<string, Song[]> songsForTagDictionary;
        public Song[] songs;
        public List<Song> searchResult;
        public List<Tag> selectedTags;
        public List<Tag> topTags;
        public Dictionary<string, Song> favoritesDict = new Dictionary<string, Song>();

        private readonly IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public Dictionary<string, Song[]> getSongsForTagDictionary()
        {
            return songsForTagDictionary = songsForTagDictionary ?? new Dictionary<string, Song[]>();
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

        public List<AudioTrack> convertSongsToAudioTracks(Song[] songs)
        {
            if (songs == null)
            {
                return null;
            }
            return songs.Select(song => new AudioTrack(song.audio_url,
                song.title,
                song.artist,
                song.album,
                song.image_url)).ToList();
        }

        internal void SaveApplicationState()
        {

            StorageUtility.writeObjectToFile(isf, "currentBackgroundKey.txt", currentBackgroundKey);
            StorageUtility.writeObjectToFile(isf, "currentMood.txt", currentMood);
            StorageUtility.writeListToFile(isf, "selectedTags.txt", selectedTags);
            StorageUtility.writeListToFile(isf, "topTags.txt", topTags);
            StorageUtility.writeObjectToFile(isf, "songsForTagDictionary.txt", songsForTagDictionary);
            StorageUtility.writeObjectToFile(isf, "favoritesDict.txt", favoritesDict);
        }

        internal void LoadApplicationState()
        {
            string tryGetBG = StorageUtility.readObjectFromFile<string>(isf, "currentBackgroundKey.txt");
            currentBackgroundKey = tryGetBG ?? "Reflection";
            songs = StorageUtility.readSongArrayFromFile(isf);
            currentMood = StorageUtility.readObjectFromFile<Tag>(isf, "currentMood.txt");
            selectedTags = StorageUtility.readListFromFile<Tag>(isf, "selectedTags.txt");
            topTags = StorageUtility.readListFromFile<Tag>(isf, "topTags.txt");
            songsForTagDictionary = StorageUtility.readObjectFromFile<Dictionary<string, Song[]>>(isf, "songsForTagDictionary.txt");
            favoritesDict = StorageUtility.readObjectFromFile<Dictionary<string, Song>>(isf, "favoritesDict.txt");
        }
    }
}