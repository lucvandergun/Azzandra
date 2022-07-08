using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Settings
    {
        // === SAVED ATTRIBUTES === \\

        [JsonProperty("default_window_width")]
        public int DefaultWindowWidth = Engine.DEFAULT_SCREEN_WIDTH;
        [JsonProperty("default_window_height")]
        public int DefaultWindowHeight = Engine.DEFAULT_SCREEN_HEIGHT;

        [JsonProperty("auto_scaling")]
        public bool AutoScaling = true;
        [JsonProperty("remember_window_size")]
        public bool RememberClientSize = false;
        //[JsonProperty("is_cheat_mode")]
        //public bool IsCheatMode = false;
        //[JsonProperty("is_light_mode")]
        //public bool IsLightMode = false;
        [JsonProperty("is_debug_mode")]
        public bool IsDebugMode = false;
        [JsonProperty("display_input")]
        public bool DisplayInput = true;
        [JsonProperty("slide_diagonals")]
        public bool SlidingDiagonals = true;
        [JsonProperty("re_queueing")]
        public bool ReQueueing = true;


        /// <summary>
        /// Creates a client settings object with default values
        /// </summary>
        public Settings() { }

        public static bool TryLoad(string directory, string file, out Settings cs)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                cs = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(directory + file));
                return true;
            }
            catch (FileNotFoundException)
            {
                cs = null;
                return false;
            }
        }

        public bool Save(string directory, string file)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                if (!File.Exists(directory + file))
                    File.Create(directory + file).Close();

                var serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(directory + file, serialized);

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
    }
}
