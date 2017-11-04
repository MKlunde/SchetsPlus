using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SketchEditor
{
    public interface ISketchTool
    {
        void MouseDown(SketchControl s, Point p);
        void MouseDrag(SketchControl s, Point p);
        void MouseUp(SketchControl s, Point p);
        void Letter(SketchControl s, char c);
    }

    public abstract class StartingPointTool : ISketchTool
    {
        protected Point startingPoint,endingPoint;
        protected Brush brush;

        public virtual void MouseDown(SketchControl s, Point p)
        {
            startingPoint = p;
        }
        public virtual void MouseUp(SketchControl s, Point p)
        {
            brush = new SolidBrush(s.PenColor);
        }
        public abstract void MouseDrag(SketchControl s, Point p);
        public abstract void Letter(SketchControl s, char c);
        public abstract void Draw(Graphics g);
    }

    public class TextTool : StartingPointTool
    {
        public override string ToString() { return "tekst"; }

        public override void MouseDrag(SketchControl s, Point p) { }

        public override void Letter(SketchControl s, char c)
        {
            if (c >= 32) {
                Graphics gr = s.CreateBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = gr.MeasureString(tekst, font, this.startingPoint, StringFormat.GenericTypographic);
                gr.DrawString (tekst, font, brush, this.startingPoint, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startingPoint.X += (int)sz.Width;
                s.Invalidate();
            }
        }

        public override void Draw(Graphics g) { }
    }

    public abstract class DualPointTool : StartingPointTool
    {
        public static Rectangle PointsToRectangle(Point p1, Point p2) {
            return new Rectangle(
                new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)),
                new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
            );
        }
        public static Pen CreatePen(Brush b, int width) {
            Pen pen = new Pen(b, width);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MouseDown(SketchControl s, Point p) {
            base.MouseDown(s, p);
            brush = Brushes.Gray;
            ISketchObject obj = CreateObject(s, p, p);
            s.SketchAddObject(obj);
        }
        public override void MouseDrag(SketchControl s, Point p) {
            ChangeCurrentObjectEndingPoint(s, p);
        }
        public override void MouseUp(SketchControl s, Point p) {
            base.MouseUp(s, p);
            ChangeCurrentObjectEndingPoint(s, p);
            FinishCurrentObject(s);
        }
        public override void Letter(SketchControl s, char c) { }

        public virtual void ChangeCurrentObjectEndingPoint(SketchControl s, Point p) {
            s.CurrentObject.ChangeEndingPoint(p);
            s.Invalidate();
        }

        public virtual void FinishCurrentObject(SketchControl s) {
            s.CurrentObject.Finish();
        }
        public abstract ISketchObject CreateObject(SketchControl s, Point p1, Point p2);

        public override void Draw(Graphics g) { }
    }

    public class EllipseTool : DualPointTool
    {
        public override string ToString() { return "ellips"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawEllipse(CreatePen(brush, 3), p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new EllipseObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }

    public class FilledEllipseTool : EllipseTool
    {
        public override string ToString() { return "ellipsV"; }

        //public override void Finish(SketchControl s, Point p1, Point p2)
        //{
            //g.FillEllipse(brush, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new FilledEllipseObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }

    public class RectangleTool : DualPointTool
    {
        public override string ToString() { return "kader"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawRectangle(CreatePen(brush,3), DualPointTool.PointsToRectangle(p1, p2));
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new RectangleObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }
    
    public class FilledRectangleTool : RectangleTool
    {
        public override string ToString() { return "vlak"; }

        //public override void Finished(SketchControl s, Point p1, Point p2)
        //{
            //g.FillRectangle(brush, DualPointTool.PointsToRectangle(p1, p2));
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new FilledRectangleObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }

    public class LineTool : DualPointTool
    {
        public override string ToString() { return "lijn"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawLine(CreatePen(this.brush,3), p1, p2);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new LineObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }

    public class PenTool : LineTool
    {
        public override string ToString() { return "pen"; }

        public override void MouseDrag(SketchControl s, Point p)
        {
            this.MouseUp(s, p);
            this.MouseDown(s, p);
        }

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new PenObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }
    
    public class EraserTool : PenTool
    {
        public override string ToString() { return "gum"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
           // g.DrawLine(CreatePen(Brushes.White, 7), p1, p2);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point p1, Point p2) {
            return new EraserObject(s, p1, p2, new SolidBrush(s.PenColor));
        }
    }
}
