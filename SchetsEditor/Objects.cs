using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SketchEditor
{
    /* Example: public DualPointTool(SketchControl s, Point startingPoint):base(s, startingPoint) { } */

    public interface ISketchObject
    {
        void ChangeEndingPoint(Point p);
        void AddText(String text);
        void Finish();
        void Draw(Graphics g);
        bool IsOnLocation(Point p);

        //Declaratie van get en set methoden
        string name { get; }
    }

    public abstract class StartingPointObject : ISketchObject
    {
        protected SketchControl s;
        protected Point startingPoint, endingPoint;
        protected Brush brush, finishedBrush, unfinishedBrush;
        protected String text;
        protected bool finished = false;

        public virtual string name { get { return "StartingPointObject"; } }

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

        protected Rectangle GetRectangle() {
            Rectangle rect = new Rectangle(Math.Min(startingPoint.X, endingPoint.X), Math.Min(startingPoint.Y, endingPoint.Y), Math.Abs(endingPoint.X - startingPoint.X), Math.Abs(endingPoint.Y - startingPoint.Y));
            return rect;
        }

        public virtual void Draw(Graphics g) {
            if (finished) {
                brush = finishedBrush;
            } else {
                brush = unfinishedBrush;
            }
        }

        public abstract bool IsOnLocation(Point p);
    }

    public class TextObject : StartingPointObject
    {
        public override string name { get { return ("TextObject"); } }
        public TextObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            Font font = new Font("Tahoma", 40);
            g.DrawString(text, font, brush, startingPoint, StringFormat.GenericTypographic);
        }

        public override bool IsOnLocation(Point p) {
            return false;
        }
    }

    public abstract class DualPointObject : StartingPointObject
    {
        public DualPointObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }
    }

    public class EllipseObject : DualPointObject
    {
        public override string name { get { return ("EllipseObject"); } }

        public EllipseObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawEllipse(new Pen(brush, 3), GetRectangle());
        }

        public override bool IsOnLocation(Point p) {
            return false;
        }
    }
    public class FilledEllipseObject : EllipseObject
    {
        public override string name { get { return ("FilledEllipseObject"); } }
        public FilledEllipseObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.FillEllipse(brush, GetRectangle());
        }

    }

    public class RectangleObject : DualPointObject
    {
        public override string name { get { return ("RectangleObject"); } }
        public RectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawRectangle(new Pen(brush, 3), GetRectangle());
        }

        public override bool IsOnLocation(Point p) {
            return false;
        }
    }
    public class FilledRectangleObject : RectangleObject
    {
        public override string name { get { return ("FilledRectangleObject"); } }
        public FilledRectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.FillRectangle(brush, GetRectangle());
        }
    }

    public class LineObject : DualPointObject
    {
        public override string name { get { return ("LineObject"); } }
        public LineObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawLine(new Pen(brush, 3), startingPoint, endingPoint);
        }

        public override bool IsOnLocation(Point p) {
            if (finished) {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(startingPoint, endingPoint);
                return path.IsOutlineVisible(p, new Pen(Brushes.Black, 7));
            } else {
                return false;
            }
        }
    }

    public class PenObject : LineObject
    {
        public override string name { get { return ("PenObject"); } }
        public PenObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }
    }

    public class EraserObject : PenObject
    {
        public EraserObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            brush = Brushes.White;
            g.DrawLine(new Pen(brush, 7), startingPoint, endingPoint);
        }
    }
}
