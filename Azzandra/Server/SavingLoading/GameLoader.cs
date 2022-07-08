using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GameLoader
    {
        public Server Server;
        public LevelManager LevelManager;
        public User User;

        public GameLoader(Server server)
        {
            Server = server;
            LevelManager = Server.LevelManager;
            User = Server.User;
        }

        private bool TryLoadBytes(string directory, string file, out byte[] bytes)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                bytes = File.ReadAllBytes(directory + file);
                return true;
            }
            catch (FileNotFoundException)
            {
                bytes = null;
                return false;
            }
        }

        public bool TryLoadGame(string directory, string file)
        {
            if (TryLoadBytes(directory, file, out var bytes))
            {
                LoadGameFromBytes(bytes);
                Server.User.UpdateVisibilityMap();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void LoadGameFromBytes(byte[] bytes)
        {
            int pos = 0;

            // Main: game version
            var saveGameVersion = GameSaver.ToString(bytes, pos);
            Debug.WriteLine("Loading game... (" + saveGameVersion + ")");
            pos += 20;
            var turns = BitConverter.ToInt32(bytes, pos);
            Server.Initialize(turns);
            pos += 4;

            // User
            int userBytesAmt = BitConverter.ToInt32(bytes, pos);
            Debug.WriteLine(" - User: " + userBytesAmt);
            pos += 4;
            var userBytes = new byte[userBytesAmt];
            Array.Copy(bytes, pos, userBytes, 0, userBytesAmt);
            User.Load(userBytes);
            pos += userBytesAmt;

            // Inventory
            int invBytesAmt = BitConverter.ToInt32(bytes, pos);
            Debug.WriteLine(" - Inventory: " + invBytesAmt);
            pos += 4;
            var invBytes = new byte[invBytesAmt];
            Array.Copy(bytes, pos, invBytes, 0, invBytesAmt);
            User.Inventory.Load(invBytes);
            pos += invBytesAmt;

            // Equipment
            int eqBytesAmt = BitConverter.ToInt32(bytes, pos);
            Debug.WriteLine(" - Equipment: " + eqBytesAmt);
            pos += 4;
            var eqBytes = new byte[eqBytesAmt];
            Array.Copy(bytes, pos, eqBytes, 0, eqBytesAmt);
            User.Equipment.Load(eqBytes);
            pos += eqBytesAmt;

            // Stats
            int statBytesAmt = BitConverter.ToInt32(bytes, pos);
            Debug.WriteLine(" - Stats: " + statBytesAmt);
            pos += 4;
            if (statBytesAmt != 0)
            {
                var statBytes = new byte[statBytesAmt];
                Array.Copy(bytes, pos, statBytes, 0, statBytesAmt);
                User.Stats.Load(statBytes);
                pos += statBytesAmt;
            }

            // World / LevelManager
            int worldBytesAmt = BitConverter.ToInt32(bytes, pos);
            Debug.WriteLine(" - World: " + worldBytesAmt);
            pos += 4;
            var worldBytes = new byte[worldBytesAmt];
            Array.Copy(bytes, pos, worldBytes, 0, worldBytesAmt);
            LevelManager.Load(worldBytes);
            pos += worldBytesAmt;
        }

        public static byte[] GetBytes(string str, int length = 20)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToString(byte[] bytes, int pos, int length = 20)
        {
            var str = Encoding.ASCII.GetString(bytes, pos, length);

            int i = str.IndexOf('\0');
            if (i >= 0) str = str.Substring(0, i);

            return str;
        }


        public static Instance LoadInstance(string instTypeID, byte[] bytes)
        {
            int pos = 0;

            var type = InstanceID.GetType(instTypeID);
            //Debug.WriteLine(" Loading inst: id = " + id + ", type = " + type);

            if (type != null && typeof(Instance).IsAssignableFrom(type))
            {
                if (typeof(TargetProjectileMoving).IsAssignableFrom(type))
                    return null;

                // Create instance of type at pos (0,0), then load its data
                var inst = (Instance)Activator.CreateInstance(type, 0, 0);
                inst.Load(bytes, ref pos);

                return inst;
                
                //var inst = (Instance)Activator.CreateInstance(type);
                //inst.Load(bytes, ref pos);
                //return inst;
            }

            return null;
        }
    }
}
