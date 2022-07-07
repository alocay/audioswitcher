using Newtonsoft.Json;

namespace AudioSwitcher
{
    public class PlaybackDevice
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsDefault { get; set; }

        public PlaybackDevice(string id, string name) : this(id, name, true)
        {
        }

        [JsonConstructor]
        public PlaybackDevice(string id, string name, bool isDefault)
        {
            Name = name;
            Id = id;
            IsDefault = isDefault;
        }
    }
}
