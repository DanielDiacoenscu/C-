using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintProject.Shapes;

namespace PaintProject.Shapes
{
    internal class Line : Shape
    {
        public Line(Pen pen, int Cx, int Cy, int x, int y) : base(pen, Cx, Cy, x, y)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawLine(pen, Cx, Cy, x, y);
        }
    }
}