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
        protected SketchControl s;
        protected Point startingPoint, endingPoint;
        protected Brush brush, finishedBrush, unfinishedBrush;
        protected bool finished = false;

        public SketchObject(SketchControl s, Point p1, Point p2, SolidBrush brush) {
            this.s = s;
            startingPoint = p1;
            endingPoint = p2;
            finishedBrush = brush;
            unfinishedBrush = new SolidBrush(Color.FromArgb(100, brush.Color.R, brush.Color.G, brush.Color.B));
        }

        public void ChangeEndingPoint(Point p) {
            endingPoint = p;
        }

        public void Finish() {
            finished = true;
        }

        public virtual void Draw(Graphics g) {
            if (finished) {
                brush = finishedBrush;
            } else {
                brush = unfinishedBrush;
            }
        }
    }

    public class TextObject : SketchObject
    {
        public TextObject(SketchControl s, Point p, SolidBrush brush) :base(s, p, new Point(0, 0), brush) { }
    }

    public class EllipseObject : SketchObject
    {
        public EllipseObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawEllipse(new Pen(brush, 3), startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }
    public class FilledEllipseObject : EllipseObject
    {
        public FilledEllipseObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.FillEllipse(brush, startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }

    }

    public class RectangleObject : SketchObject
    {
        public RectangleObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }
    }
    public class FilledRectangleObject : RectangleObject
    {
        public FilledRectangleObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }
    }

    public class LineObject : SketchObject
    {
        public LineObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }
    }

    public class PenObject : SketchObject
    {
        public PenObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }
    }

    public class EraserObject : SketchObject
    {
        public EraserObject(SketchControl s, Point p1, Point p2, SolidBrush brush) :base(s, p1, p2, brush) { }
    }
}
