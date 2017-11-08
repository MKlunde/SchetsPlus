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
        void Draw(Graphics g);
        ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush);
    }

    public abstract class StartingPointTool : ISketchTool
    {
        protected Point startingPoint,endingPoint;
        protected Brush brush;

        public virtual void MouseDown(SketchControl s, Point p)
        {
            startingPoint = p;
        }
        public abstract void MouseDrag(SketchControl s, Point p);
        public virtual void MouseUp(SketchControl s, Point p)
        {
            brush = new SolidBrush(s.PenColor);
        }
        public abstract void Letter(SketchControl s, char c);
        public abstract void Draw(Graphics g);
        public abstract ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush);
    }

    public class TextTool : StartingPointTool
    {
        public override string ToString() { return "Tekst"; }

        public override void MouseDrag(SketchControl s, Point p) { }

        public override void MouseUp(SketchControl s, Point p) {
            base.MouseUp(s, p);
            ISketchObject obj = CreateObject(s, p, new SolidBrush(s.PenColor));
            obj.Finish();
            s.SketchAddObject(obj);
        }

        public override void Letter(SketchControl s, char c)
        {
            CurrentObjectAddText(s, c.ToString());
        }

        public virtual void CurrentObjectAddText(SketchControl s, string text) {
            s.CurrentObject.AddText(text);
            s.Invalidate();
        }

        public override void Draw(Graphics g) { }

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new TextObject(s, startingPoint, brush, s.PenWidth);
        }
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
            ISketchObject obj = CreateObject(s, p, new SolidBrush(s.PenColor));
            s.SketchAddObject(obj);
            CurrentObjectChangeEndingPoint(s, p);
        }
        public override void MouseDrag(SketchControl s, Point p) {
            CurrentObjectChangeEndingPoint(s, p);
            s.Invalidate();
        }
        public override void MouseUp(SketchControl s, Point p) {
            base.MouseUp(s, p);
            CurrentObjectChangeEndingPoint(s, p);
            FinishCurrentObject(s);
        }
        public override void Letter(SketchControl s, char c) { }

        public virtual void CurrentObjectChangeEndingPoint(SketchControl s, Point p) {
            s.CurrentObject.ChangeEndingPoint(p);
        }

        public virtual void FinishCurrentObject(SketchControl s) {
            s.CurrentObject.Finish();
            s.Invalidate();
        }
        

        public override void Draw(Graphics g) { }
    }

    public class EllipseTool : DualPointTool
    {
        public override string ToString() { return "Ellips"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawEllipse(CreatePen(brush, 3), p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new EllipseObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }

    public class FilledEllipseTool : EllipseTool
    {
        public override string ToString() { return "EllipsV"; }

        //public override void Finish(SketchControl s, Point p1, Point p2)
        //{
            //g.FillEllipse(brush, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new FilledEllipseObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }

    public class RectangleTool : DualPointTool
    {
        public override string ToString() { return "Kader"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawRectangle(CreatePen(brush,3), DualPointTool.PointsToRectangle(p1, p2));
        //}

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new RectangleObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }
    
    public class FilledRectangleTool : RectangleTool
    {
        public override string ToString() { return "Vlak"; }

        //public override void Finished(SketchControl s, Point p1, Point p2)
        //{
            //g.FillRectangle(brush, DualPointTool.PointsToRectangle(p1, p2));
        //}

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new FilledRectangleObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }

    public class LineTool : DualPointTool
    {
        public override string ToString() { return "Lijn"; }

        //public override void BeingDrawn(Graphics g, Point p1, Point p2)
        //{
            //g.DrawLine(CreatePen(this.brush,3), p1, p2);
        //}

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new LineObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }

    public class PenTool : LineTool
    {
        public override string ToString() { return "Pen"; }

        public override void MouseDrag(SketchControl s, Point p)
        {
            MouseUp(s, p);
            MouseDown(s, p);
        }

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            return new PenObject(s, startingPoint, new SolidBrush(s.PenColor), s.PenWidth);
        }
    }
    
    public class EraserTool : PenTool
    {
        public override string ToString() { return "Gum"; }

        public override void MouseDown(SketchControl s, Point p) {
            EraseObjectOnLocation(s, p);
        }
        public override void MouseDrag(SketchControl s, Point p) {
            EraseObjectOnLocation(s, p);
        }
        public override void MouseUp(SketchControl s, Point p) { }

        public override ISketchObject CreateObject(SketchControl s, Point startingPoint, SolidBrush brush) {
            //return new EraserObject(s, startingPoint, new SolidBrush(s.PenColor));
            return null;
        }

        public void EraseObjectOnLocation(SketchControl s, Point p) {
            ISketchObject clickedObject = s.SketchObjectOnLocation(p);
            if (clickedObject != null) {
                s.Sketch.Objects.Remove(clickedObject); // Verwijder object waar met de gum op wordt geklikt
                s.Invalidate();
            }
        }
    }
}
