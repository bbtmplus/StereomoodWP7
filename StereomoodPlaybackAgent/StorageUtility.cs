using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;


namespace StereomoodPlaybackAgent
{
    public class StorageUtility
    {
        private static readonly IsolatedStorageSettings _isolatedStore;

        static StorageUtility()
        {
            _isolatedStore = IsolatedStorageSettings.ApplicationSettings;
        }

        public static bool AddOrUpdateValue(string key, object value)
        {
            bool valueChanged = false;

            try
            {
                if (_isolatedStore.Contains(key))
                {
                    if (_isolatedStore[key] != value)
                    {
                        if (_isolatedStore.Remove(key))
                        {
                            _isolatedStore.Add(key, value);
                            valueChanged = true;
                        }
                    }
                }
                else
                {
                    _isolatedStore.Add(key, value);
                    valueChanged = true;
                }
            }
            catch (Exception)
            {
                //to be implemented 
            }

            if (valueChanged)
            {
                Save();
            }

            return valueChanged;
        }

        public static T GetValueOrDefault<T>(string key)
        {


            T tmpval;
            _isolatedStore.TryGetValue(key, out tmpval);


            return tmpval;
        }

        public static T PickValueOrDefault<T>(string key)
        {
            T value = GetValueOrDefault<T>(key);
            return value;
        }

        public static bool RemoveItem(string key)
        {
            if (_isolatedStore.Contains(key))
            {
                if (_isolatedStore.Remove(key))
                    Save();
            }

            return false;
        }

        private static void Save()
        {
            _isolatedStore.Save();
        }
    }
}
