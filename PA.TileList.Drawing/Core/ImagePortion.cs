using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PA.TileList.Drawing
{
    public class ImagePortion<T>
        where T: ICoordinate
    {
        public T Item { get; private set; }
        
        public RectangleF Portion { get; private set; }
       
        public ImagePortion(T item, RectangleF portion)
        {
            this.Item = item;
            this.Portion = portion;
        }
    }
}
