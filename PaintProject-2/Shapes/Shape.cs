using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintProject.Shapes
{
    internal partial class Shape
    {
        public Pen pen { get; set; }
        public int Cx { get; set; }
        public int Cy { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Shape(Pen pen, int Cx, int Cy, int x, int y)
        {
            this.pen = pen;
            this.Cx = Cx;
            this.Cy = Cy;
            this.x = x;
            this.y = y;
        }

        public virtual void Draw(Graphics g)
        {
            
        }
    }
}
