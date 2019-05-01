using GK.Structs;
using System.Collections.Generic;

namespace GK.Interfaces
{
    public interface IDrawable3D
    {
        List<List<Vertex3D>> GetShapes();
    }
}
