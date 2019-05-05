using GK.Math3D;
using System.Collections.Generic;

namespace GK.Interfaces
{
    public interface IDrawable3D
    {
        List<Triangle3Df> GetTriangle3Dfs();
    }
}
