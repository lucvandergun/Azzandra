using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Azzandra
{
    public class UserInventory : Inventory
    {
        private readonly User User;

        public UserInventory(User user)
        {
            User = user;
        }
    }
}
