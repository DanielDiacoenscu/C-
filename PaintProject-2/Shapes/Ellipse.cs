using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintProject.Shapes
{
    internal class Ellipse : Shape
    {
        public Ellipse(Pen pen, int Cx, int Cy, int x, int y) : base(pen, Cx, Cy, x, y)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawEllipse(pen, Cx, Cy, x, y);
        }
    }
}
