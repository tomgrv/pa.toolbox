
using System.Collections.Generic;

namespace PA.TileList.Quadrant
{
    public interface IQuadrant<T> : IList<T> where T : ICoordinate
    {
        Quadrant Quadrant { get; }
        IArea Area { get; }
        void SetQuadrant(Quadrant q);
    }
}
