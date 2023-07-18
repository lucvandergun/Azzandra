using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public interface IMouseInterface
    {
        Surface GetSurface();
        
        void Render();
        void Destroy();
    }
}
