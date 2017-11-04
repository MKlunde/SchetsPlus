using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SketchEditor
{
    /* Example: public DualPointTool(SketchControl s, Point startingPoint):base(s, startingPoint) { } */

    public interface ISketchObject
    {
        void ChangeEndingPoint(Point p);
        void Finish();
        void Draw(Graphics g);
    }

    public abstract class SketchObject : ISketchObject
    {
        protected Point startingPoint, endingPoint;
        protected Brush finishedBrush = Brushes.Black;
        protected Brush unfinishedBrush = Brushes.Gray;
        protected Brush brush;
        protected bool finished = false;

        public SketchObject(Point p1, Point p2) {
            startingPoint = p1;
            endingPoint = p2;
        }

        public void ChangeEndingPoint(Point p) {
            endingPoint = p;
        }

        public void Finish() {
            finished = true;
        }

        public virtual void Draw(Graphics g) {
            if (finished)
                brush = finishedBrush;
            else
                brush = unfinishedBrush;
        }
    }

    public class TextObject : SketchObject
    {
        public TextObject(Point p1) :base(p1, new Point(0, 0)) {

        }
    }

    public class EllipseObject : SketchObject
    {
        public EllipseObject(Point p1, Point p2) :base(p1, p2) {

        }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawEllipse(new Pen(brush, 3), startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }
    public class FilledEllipseObject : EllipseObject
    {
        public FilledEllipseObject(Point p1, Point p2) :base(p1, p2) {

        }
    }

    public class RectangleObject : SketchObject
    {
        public RectangleObject(Point p1, Point p2) :base(p1, p2) {

        }
    }
    public class FilledRectangleObject : RectangleObject
    {
        public FilledRectangleObject(Point p1, Point p2) :base(p1, p2) {

        }
    }

    public class LineObject : SketchObject
    {
        public LineObject(Point p1, Point p2) :base(p1, p2) {

        }
    }

    public class PenObject : SketchObject
    {
        public PenObject(Point p1, Point p2) :base(p1, p2) {

        }
    }

    public class EraserObject : SketchObject
    {
        public EraserObject(Point p1, Point p2) :base(p1, p2) {

        }
    }
}
