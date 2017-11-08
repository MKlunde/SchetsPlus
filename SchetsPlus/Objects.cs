using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SketchEditor
{
    public interface ISketchObject // Objecten die bij de tools horen
    {
        string ToString();
        void ChangeEndingPoint(Point p);
        void AddText(String text);
        void Rotate();
        void Finish();
        void Draw(Graphics g);
        bool IsOnLocation(Point p);

        // Declaratie van get- en set-methoden
        string Name { get; }
        Point StartingPoint { get; }
        Point EndingPoint { get; }
        SolidBrush Brush { get; }
        int PenWidth { get; }
        string Text { get; }
        int Rotation { get; }
    }

    public abstract class StartingPointObject : ISketchObject
    {
        protected SketchControl s;
        protected Point startingPoint, endingPoint;
        protected SolidBrush brush, finishedBrush, unfinishedBrush;
        protected int penWidth = 0;
        protected String text = "";
        protected int rotation = 0;
        protected bool finished = false;

        // Declareer Get-methoden
        public string Name { get { return GetType().ToString().Replace("SketchEditor.", ""); } }
        public Point StartingPoint { get { return startingPoint; } }
        public Point EndingPoint { get { return endingPoint; } }
        public SolidBrush Brush { get { return brush; } }
        public int PenWidth { get { return penWidth; } }
        public string Text { get { return text; } }
        public int Rotation { get { return rotation; } }

        public override string ToString() {
            return Name + " " + startingPoint.X + " " + startingPoint.Y + " " + endingPoint.X + " " + endingPoint.Y + " " + brush.Color.R + " " + brush.Color.G + " " + brush.Color.B + " " + penWidth + " " + text + " " + rotation;
        }

        public StartingPointObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) {
            this.s = s;
            this.startingPoint = startingPoint;
            finishedBrush = brush;
            unfinishedBrush = new SolidBrush(Color.FromArgb(150, brush.Color.R, brush.Color.G, brush.Color.B));
            this.penWidth = penWidth;
        }

        public void ChangeEndingPoint(Point p) {
            endingPoint = p;
        }

        public void AddText(String text) {
            this.text += text;
            Bitmap testBitmap = new Bitmap(s.ClientSize.Width, s.ClientSize.Height);
            Graphics testGraphics = Graphics.FromImage(testBitmap);
            SizeF sz = testGraphics.MeasureString(this.text, s.TextFont, startingPoint, StringFormat.GenericTypographic);
            endingPoint = new Point(startingPoint.X + (int)sz.Width, startingPoint.Y + (int)sz.Height);
        }

        public void Rotate() {
            rotation += 90;
            rotation %= 360;
        }

        public void Finish() {
            finished = true;
        }

        protected Rectangle GetRectangle() {
            Rectangle rect = new Rectangle(Math.Min(startingPoint.X, endingPoint.X), Math.Min(startingPoint.Y, endingPoint.Y), Math.Abs(endingPoint.X - startingPoint.X), Math.Abs(endingPoint.Y - startingPoint.Y));
            return rect;
        }

        public virtual void BaseDraw(Graphics g) {
            if (finished) {
                brush = finishedBrush;
            } else {
                brush = unfinishedBrush;
            }
            int transformX = 0;
            int transformY = 0;
            if (rotation == 90 || rotation == 180)
                transformX = s.Sketch.Bitmap.Width;
            if (rotation == 180 || rotation == 270) {
                transformY = s.Sketch.Bitmap.Width;
            }
            g.TranslateTransform(transformX, transformY);
            g.RotateTransform(rotation);
        }

        public virtual void Draw(Graphics g) {
            BaseDraw(g);
        }

        public abstract bool IsOnLocation(Point p);
    }

    public class TextObject : StartingPointObject
    {
        public TextObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawString(text, s.TextFont, brush, startingPoint, StringFormat.GenericTypographic);
            //g.DrawRectangle(Pens.Gray, GetRectangle()); // Debug
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            Rectangle rect = GetRectangle();
            if (!rect.Contains(p)) {
                return false;
            }
            Bitmap testBitmap = new Bitmap(rect.Width, rect.Height);
            Graphics testGraphics = Graphics.FromImage(testBitmap);
            testGraphics.DrawString(text, s.TextFont, Brushes.Red, new Point(0,0), StringFormat.GenericTypographic);
            Draw(testGraphics);
            Color col = testBitmap.GetPixel(p.X - rect.X, p.Y - rect.Y);
            return col.ToArgb() == Color.Red.ToArgb();
        }
    }
   
    public abstract class DualPointObject : StartingPointObject
    {
        public DualPointObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }
    }
   
    public class EllipseObject : DualPointObject
    {
        public EllipseObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawEllipse(new Pen(brush, penWidth), GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(GetRectangle());
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, penWidth + 5));
        }
    }
   
    public class FilledEllipseObject : EllipseObject
    {
        public FilledEllipseObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            BaseDraw(g);
            g.FillEllipse(brush, GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(GetRectangle());
            return finished && path.IsVisible(p);
        }
    }
  
    public class RectangleObject : DualPointObject
    {
        public RectangleObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawRectangle(new Pen(brush, penWidth), GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(GetRectangle());
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, penWidth + 5));
        }
    }
   
    public class FilledRectangleObject : RectangleObject
    {
        public FilledRectangleObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            BaseDraw(g);
            g.FillRectangle(brush, GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(GetRectangle());
            return finished && path.IsVisible(p);
        }
    }
   
    public class LineObject : DualPointObject
    {
        public LineObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawLine(new Pen(brush, penWidth), startingPoint, endingPoint);
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(startingPoint, endingPoint);
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, penWidth + 5));
        }
    }
   
    public class PenObject : LineObject
    {
        public PenObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.BaseDraw(g);
            g.DrawLine(new Pen(brush, penWidth), startingPoint, endingPoint);
            g.FillEllipse(brush, startingPoint.X - penWidth / 2, startingPoint.Y - penWidth / 2, penWidth, penWidth);
            g.ResetTransform();
        }
    }
   
    public class EraserObject : PenObject
    {
        public EraserObject(SketchControl s, Point startingPoint, SolidBrush brush, int penWidth) :base(s, startingPoint, brush, penWidth) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            brush = Brushes.White as SolidBrush;
            g.DrawLine(new Pen(brush, penWidth), startingPoint, endingPoint);
            g.ResetTransform();
        }
    }
}
