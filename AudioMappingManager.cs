using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace AudioSwitcher
{
    public class AudioMappingManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string MappingFile = "mappings.json";

        public Dictionary<string, PlaybackDevice> Mappings { get; set; }

        public AudioMappingManager()
        {
            //using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            //{
            //    isoStore.DeleteFile(MappingFile);
            //}

            LoadMappings();
        }

        public void AddOrUpdateMapping(string processName, string deviceId, string deviceName)
        {
            if (string.IsNullOrEmpty(processName) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(deviceName))
            {
                log.Error($"Could not add/udpate mapping, expected value was empty: {processName} {deviceId} {deviceName}");
                return;
            }

            string processNameLower = processName.ToLower().Trim();
            CheckAndRemoveKey(processNameLower);
            Mappings.Add(processNameLower, new PlaybackDevice(deviceId, deviceName));

            SaveMappings();
        }

        public void RemoveMapping(string processName)
        {
            if (string.IsNullOrEmpty(processName) || !Mappings.ContainsKey(processName))
            {
                return;
            }

            Mappings.Remove(processName);
        }

        public void SaveMappings()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
                {
                    var stream = isoStore.CreateFile(MappingFile);

                    using (StreamWriter file = new StreamWriter(stream))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, Mappings);
                    }
                }
            } catch (Exception ex)
            {
                log.Error("Error saving the audio mappings", ex);
            }
        }

        public void LoadMappings()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
                {
                    if (!isoStore.FileExists(MappingFile))
                    {
                        Mappings = new Dictionary<string, PlaybackDevice>();
                        return;
                    }

                    var stream = isoStore.OpenFile(MappingFile, FileMode.Open);

                    using (StreamReader file = new StreamReader(stream))
                    {
                        string json = file.ReadToEnd();
                        Mappings = JsonConvert.DeserializeObject<Dictionary<string, PlaybackDevice>>(json);
                    }
                }
            } catch (Exception ex)
            {
                log.Error("Error loading the audio mappings", ex);
            }
        }

        private void CheckAndRemoveKey(string newProcessName)
        {
            if (Mappings.ContainsKey(newProcessName))
            {
                Mappings.Remove(newProcessName);
                return;
            }

            string similarProcessName = null;
            foreach (string processName in Mappings.Keys)
            {
                if (newProcessName.Contains(processName))
                {
                    similarProcessName = processName;
                }
            }

            if (!string.IsNullOrEmpty(similarProcessName))
            {
                Mappings.Remove(similarProcessName);
            }
        }
    }
}
