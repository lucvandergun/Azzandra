using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public interface IAnimation
    {
        Instance Owner { get; }
        void Update();
        Vector2 GetDisposition();
    }
}
