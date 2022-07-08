using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class InstRef
    {
        public int ID { get; private set; } = -1;
        public Instance Instance { get; set; }
        public Entity Combatant => Instance as Entity;

        public static int GetSaveID(InstRef instRef) => instRef?.ID ?? -1;
        public static InstRef Load(int id)
        {
            // Loads an InstRef from the given instance id. Will add the correct 'Instance' reference after having loaded all instances.
            if (id < 0) return null;

            var instRef = new InstRef(id);
            LoadedReferences.Add(instRef);
            return instRef;
        }
        public static List<InstRef> LoadedReferences = new List<InstRef>();

        public InstRef(Instance inst)
        {
            Instance = inst;
            ID = inst.ID;
        }

        public InstRef(int id)
        {
            ID = id;
        }

        public bool Exists() => Instance?.Level.GetInstanceByID(ID) != null;

        public override string ToString() => "[ID: " + ID + ", Type: " + Instance?.GetType().Name + "]";
    }
}
