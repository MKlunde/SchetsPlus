using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SketchEditor
{
    /* Example: public DualPointTool(SketchControl s, Point startingPoint):base(s, startingPoint) { } */

    public interface ISketchObject
    {
        void ChangeEndingPoint(Point p);
        void AddText(String text);
        void Finish();
        void Draw(Graphics g);
    }

    public abstract class StartingPointObject : ISketchObject
    {
        protected SketchControl s;
        protected Point startingPoint, endingPoint;
        protected Brush brush, finishedBrush, unfinishedBrush;
        protected String text;
        protected bool finished = false;

        public StartingPointObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            this.s = s;
            this.startingPoint = startingPoint;
            finishedBrush = brush;
            unfinishedBrush = new SolidBrush(Color.FromArgb(150, brush.Color.R, brush.Color.G, brush.Color.B));
        }

        public void ChangeEndingPoint(Point p) {
            endingPoint = p;
        }

        public void AddText(String text) {
            this.text += text;
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

    public class TextObject : StartingPointObject
    {
        public TextObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            Font font = new Font("Tahoma", 40);
            g.DrawString(text, font, brush, startingPoint, StringFormat.GenericTypographic);
        }
    }

    public abstract class DualPointObject : StartingPointObject
    {
        public DualPointObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }
    }

    public class EllipseObject : DualPointObject
    {
        public EllipseObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawEllipse(new Pen(brush, 3), startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }
    public class FilledEllipseObject : EllipseObject
    {
        public FilledEllipseObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.FillEllipse(brush, startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }

    }

    public class RectangleObject : DualPointObject
    {
        public RectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawRectangle(new Pen(brush, 3), startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }
    public class FilledRectangleObject : RectangleObject
    {
        public FilledRectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.FillRectangle(brush, startingPoint.X, startingPoint.Y, endingPoint.X - startingPoint.X, endingPoint.Y - startingPoint.Y);
        }
    }

    public class LineObject : DualPointObject
    {
        public LineObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawLine(new Pen(brush, 3), startingPoint, endingPoint);
        }
    }

    public class PenObject : LineObject
    {
        public PenObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }
    }

    public class EraserObject : PenObject
    {
        public EraserObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            brush = Brushes.White;
            g.DrawLine(new Pen(brush, 3), startingPoint, endingPoint);
        }
    }
}
