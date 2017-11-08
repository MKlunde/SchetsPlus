﻿using System;
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
        void Rotate();
        void Finish();
        void Draw(Graphics g);
        bool IsOnLocation(Point p);

        //Declaratie van get en set methoden
        string Name { get; }
        string StartingPoint { get; }
        string EndingPoint { get; }
        string Brush { get; }
        string Text { get; }
    }

    public abstract class StartingPointObject : ISketchObject
    {
        protected SketchControl s;
        protected Point startingPoint, endingPoint;
        protected SolidBrush brush, finishedBrush, unfinishedBrush;
        protected String text;
        protected int rotation = 0;
        protected bool finished = false;

        //gebruik get methoden
        public virtual string Name { get { return this.GetType().ToString().Replace("SketchEditor.", ""); } }
        public virtual string StartingPoint { get { return startingPoint.X + " " + startingPoint.Y; } }
        public virtual string EndingPoint { get { return endingPoint.X + " " + endingPoint.Y; } }
        public virtual string Brush { get { return brush.Color.R + " " + brush.Color.G + " " + brush.Color.B; } }
        public virtual string Text { get { return text; } }

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
            Bitmap testBitmap = new Bitmap(s.ClientSize.Width, s.ClientSize.Height);
            Graphics testGraphics = Graphics.FromImage(testBitmap);
            SizeF sz = testGraphics.MeasureString(this.text, s.TextFont, startingPoint, StringFormat.GenericTypographic);
            endingPoint = new Point(startingPoint.X + (int)sz.Width, startingPoint.Y + (int)sz.Height);
            //Console.WriteLine(startingPoint.X + " " + (int)sz.Width + " " + startingPoint.Y + " " + (int)sz.Height);
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

        public virtual void Draw(Graphics g) {
            if (finished) {
                brush = finishedBrush;
            } else {
                brush = unfinishedBrush;
            }
            int transformX = 0;
            int transformY = 0;
            if (rotation == 90 || rotation == 180)
                transformX = s.ClientSize.Width;
            if (rotation == 180 || rotation == 270) {
                transformY = s.ClientSize.Width;
            }
            g.TranslateTransform(transformX, transformY);
            g.RotateTransform(rotation);
        }

        public abstract bool IsOnLocation(Point p);
    }

    public class TextObject : StartingPointObject
    {
        public TextObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawString(text, s.TextFont, brush, startingPoint, StringFormat.GenericTypographic);
            //g.DrawRectangle(Pens.Gray, GetRectangle()); // Debug
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            Rectangle rect = GetRectangle();
            if (!rect.Contains(p)) {
                //Console.WriteLine("False: [(" + startingPoint.X + ", " + startingPoint.Y + "), (" + endingPoint.X + ", " + endingPoint.Y + ")], (" + p.X + ", " + p.Y + ")");
                return false;
            }
            Bitmap testBitmap = new Bitmap(rect.Width, rect.Height);
            Graphics testGraphics = Graphics.FromImage(testBitmap);
            testGraphics.DrawString(text, s.TextFont, Brushes.Red, new Point(0,0), StringFormat.GenericTypographic);
            Draw(testGraphics);
            Color col = testBitmap.GetPixel(p.X - rect.X, p.Y - rect.Y);
            //Console.WriteLine(col.ToArgb() == Color.Red.ToArgb());
            return col.ToArgb() == Color.Red.ToArgb();
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
            g.DrawEllipse(new Pen(brush, 3), GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(GetRectangle());
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, 7));
        }
    }
   
    public class FilledEllipseObject : EllipseObject
    {
        public FilledEllipseObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
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
        public RectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawRectangle(new Pen(brush, 3), GetRectangle());
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(GetRectangle());
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, 7));
        }
    }
   
    public class FilledRectangleObject : RectangleObject
    {
        public FilledRectangleObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
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
        public LineObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            g.DrawLine(new Pen(brush, 3), startingPoint, endingPoint);
            g.ResetTransform();
        }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(startingPoint, endingPoint);
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, 7));
        }
    }
   
    public class PenObject : LineObject
    {
        public PenObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override bool IsOnLocation(Point p) {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(startingPoint, endingPoint);
            return finished && path.IsOutlineVisible(p, new Pen(Brushes.Black, 7));
        }
    }
   
    public class EraserObject : PenObject
    {
        public EraserObject(SketchControl s, Point startingPoint, SolidBrush brush) :base(s, startingPoint, brush) { }

        public override void Draw(Graphics g) {
            base.Draw(g);
            brush = Brushes.White as SolidBrush;
            g.DrawLine(new Pen(brush, 7), startingPoint, endingPoint);
            g.ResetTransform();
        }
    }
}
