using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GameSaver
    {
        public Server Server;
        public LevelManager LevelManager;
        public User User;

        public GameSaver(Server server)
        {
            Server = server;
            LevelManager = Server.LevelManager;
            User = Server.User;
        }

        public bool TrySaveGame(string directory, string file)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var path = directory + file;
            var bytes = SaveBytes();
            if (bytes != null)
            {
                //using (var saveFile = File.Create(path)) { } // To close the file before overwriting it!
                File.Create(path).Close();
                File.WriteAllBytes(path, bytes);
                return true;
            }

            return false;
        }
        
        private byte[] SaveBytes()
        {
            Debug.WriteLine("Saving game... (" + Engine.GAME_VERSION + ")" );

            // Save components
            var userBytes = User.ToBytes();
            int userBytesAmt = userBytes.Length;

            var invBytes = User.Inventory.ToBytes();
            int invBytesAmt = invBytes.Length;

            var eqBytes = User.Equipment.ToBytes();
            int eqBytesAmt = eqBytes.Length;

            var statBytes = User.Stats.ToBytes();
            int statBytesAmt = statBytes.Length;

            var worldBytes = LevelManager.ToBytes();
            int worldBytesAmt = worldBytes.Length;

            Debug.WriteLine(" - User: " + userBytesAmt);
            Debug.WriteLine(" - Inventory: " + invBytesAmt);
            Debug.WriteLine(" - Equipment: " + eqBytesAmt);
            Debug.WriteLine(" - Stats: " + statBytesAmt);
            Debug.WriteLine(" - World: " + worldBytesAmt);

            // Setup byte array
            var bytes = new byte[20 + 4 + 5*4 + userBytesAmt + invBytesAmt + eqBytesAmt + statBytesAmt + worldBytesAmt];
            int pos = 0;


            // Main:
            bytes.Insert(pos, GameSaver.GetBytes(Engine.GAME_VERSION));
            pos += 20;
            bytes.Insert(pos, BitConverter.GetBytes(Server.AmtTurns));
            pos += 4;

            // User
            bytes.Insert(pos, BitConverter.GetBytes(userBytesAmt));
            pos += 4;
            bytes.Insert(pos, userBytes);
            pos += userBytesAmt;

            // Inventory
            bytes.Insert(pos, BitConverter.GetBytes(invBytesAmt));
            pos += 4;
            bytes.Insert(pos, invBytes);
            pos += invBytesAmt;

            // Equipment
            bytes.Insert(pos, BitConverter.GetBytes(eqBytesAmt));
            pos += 4;
            bytes.Insert(pos, eqBytes);
            pos += eqBytesAmt;

            // Stats
            bytes.Insert(pos, BitConverter.GetBytes(statBytesAmt));
            pos += 4;
            bytes.Insert(pos, statBytes);
            pos += statBytesAmt;

            // LevelManager
            bytes.Insert(pos, BitConverter.GetBytes(worldBytesAmt));
            pos += 4;
            bytes.Insert(pos, worldBytes);
            pos += worldBytesAmt;

            return bytes;
        }

        public static byte[] GetBytes(string str)
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


        /// <summary>
        /// Save an enumerable of a certain type to a bytes array.
        /// The resulting array is of the following shape (amtOfEntries, n * (objBytesAmt, objBytes)).
        /// Removes any "null" objects first - does not save them.
        /// </summary>
        /// <typeparam name="T">The object type in the list</typeparam>
        /// <param name="list">The list of objects to save</param>
        /// <param name="objSaver">They way to convert each object to bytes</param>
        /// <returns></returns>
        public static byte[] SaveList<T>(IEnumerable<T> list, Func<T, byte[]> objSaver)
        {
            // Remove any null references
            list = list.Where(i => i != null);
            
            int objAmt = list.Count();
            var bytes = BitConverter.GetBytes(objAmt);

            foreach (var obj in list)
            {
                var objBytes = objSaver.Invoke(obj);
                var objBytesAmt = objBytes.Length;
                objBytes = BitConverter.GetBytes(objBytesAmt).Concat(objBytes).ToArray();
                bytes = bytes.Concat(objBytes).ToArray();
            }
            return bytes;
        }
    }
}
