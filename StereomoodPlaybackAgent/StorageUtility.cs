using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;

namespace StereomoodPlaybackAgent
{
    public class StorageUtility
    {
        public static void writeStringToFile(IsolatedStorageFile isoStore, string fileName, String s)
        {
            try
            {
                StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore));
                writer.WriteLine(s);
                writer.Close();
            }
            catch (IsolatedStorageException ex)
            {

            }
        }



        public static void writeObjectToFile<T>(IsolatedStorageFile isoStore, string filePath, T item)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(filePath, FileMode.Create,
                                                                               isoStore);
                serializer.WriteObject(isfs, item);
                isfs.Close();
            }
            catch (IsolatedStorageException ex)
            {

            }
        }



        public static void writeListToFile<T>(IsolatedStorageFile isoStore, string filePath, List<T> list)
        {
            try
            {
                if (list != null)
                {
                    ListWrapper<T> listWrapper = new ListWrapper<T> { Elements = new List<T>(list) };
                    var serializer = new DataContractJsonSerializer(typeof(ListWrapper<T>));
                    IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(filePath, FileMode.Create,
                                                                                   isoStore);
                    serializer.WriteObject(isfs, listWrapper);
                    isfs.Close();
                }
            }
            catch (IsolatedStorageException ex)
            {

            }
        }



        public static T readObjectFromFile<T>(IsolatedStorageFile isoStore, string filePath)
        {
            var item = default(T);
            try
            {
                if (isoStore.FileExists(filePath))
                {
                    IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(filePath, FileMode.Open,
                                                                                   isoStore);
                    var serializer = new DataContractJsonSerializer(typeof(T));

                    item = (T)serializer.ReadObject(isfs);
                    isfs.Close();
                }
            }
            catch (IsolatedStorageException ex)
            {

            }

            return item;
        }



        public static void writeSongArrayToFile(IsolatedStorageFile isoStore, Song[] list)
        {
            try
            {
                IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("SongList.txt", FileMode.Create,
                                                                                  isoStore);
                var serializer = new DataContractJsonSerializer(typeof(Song[]));
                serializer.WriteObject(isfs, list);
                isfs.Close();
            }
            catch (IsolatedStorageException ex)
            {

            }
        }

        public static Song[] readSongArrayFromFile(IsolatedStorageFile isoStore)
        {
            Song[] array = new Song[0];
            try
            {
                if (isoStore.FileExists("SongList.txt"))
                {
                    IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("SongList.txt", FileMode.Open,
                                                                                   isoStore);
                    var serializer = new DataContractJsonSerializer(typeof(Song[]));

                    array = (Song[])serializer.ReadObject(isfs);

                    isfs.Close();
                }
            }
            catch (IsolatedStorageException ex)
            {

            }
            return array;
        }



        public static List<T> readListFromFile<T>(IsolatedStorageFile isoStore, string filePath)
        {
            ListWrapper<T> listWrapper = null;
            try
            {
                if (isoStore.FileExists(filePath))
                {
                    IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(filePath, FileMode.Open,
                                                                                   isoStore);
                    var serializer = new DataContractJsonSerializer(typeof(ListWrapper<T>));

                    listWrapper = (ListWrapper<T>)serializer.ReadObject(isfs);
                    if (listWrapper == null)
                    {

                    }
                    isfs.Close();
                }
            }
            catch (IsolatedStorageException ex)
            {
                readListFromFile<T>(isoStore, filePath);
            }

            return (listWrapper != null) ? listWrapper.Elements : null;
        }



        public static String readStringFromFile(IsolatedStorageFile isoStore, string fileName)
        {
            String sb = "0";
            try
            {
                if (isoStore.FileExists(fileName))
                {
                    StreamReader reader =
                        new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore));
                    sb = reader.ReadLine();

                    reader.Close();
                }
            }
            catch (IsolatedStorageException ex)
            {

            }
            return sb != null ? sb.ToString(CultureInfo.InvariantCulture) : null;
        }

    }
}
