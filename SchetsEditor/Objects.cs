using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SketchEditor
{
    /* Example: public DualPointTool(SketchControl s, Point startingPoint):base(s, startingPoint) { } */

    public interface ISketchObject
    {
        void Draw(Graphics g);
    }

    public abstract class SketchObject : ISketchObject
    {
        public Point startingPoint, endingPoint;
        protected Brush brush = Brushes.Black;

        public virtual void Draw(Graphics g) { }
    }

    public class TextObject : SketchObject
    {

    }

    public class EllipseObject : SketchObject
    {
        public override void Draw(Graphics g) {
            g.DrawEllipse(new Pen(brush, 3), startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }
    public class FilledEllipseObject : EllipseObject
    {
            
    }

    public class RectangleObject : SketchObject
    {

    }
    public class FilledRectangleObject : RectangleObject
    {

    }

    public class LineObject : SketchObject
    {

    }

    public class PenObject : SketchObject
    {

    }

    public class EraserObject : SketchObject
    {

    }
}
