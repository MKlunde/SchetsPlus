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

    public abstract class StartingsPointTool : ISketchTool
    {
        protected Point startingPoint;
        protected Brush brush;

        public virtual void MouseDown(SketchControl s, Point p)
        {   startingPoint = p;
        }
        public virtual void MouseUp(SketchControl s, Point p)
        {   brush = new SolidBrush(s.PenColor);
        }
        public abstract void MouseDrag(SketchControl s, Point p);
        public abstract void Letter(SketchControl s, char c);
    }

    public class TextTool : StartingsPointTool
    {
        public override string ToString() { return "tekst"; }

        public override void MouseDrag(SketchControl s, Point p) { }

        public override void Letter(SketchControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startingPoint, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, brush, 
                                              this.startingPoint, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startingPoint.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class DualPointTool : StartingsPointTool
    {
        public static Rectangle PointsToRectangle(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen CreatePen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MouseDown(SketchControl s, Point p)
        {   base.MouseDown(s, p);
            brush = Brushes.Gray;
        }
        public override void MouseDrag(SketchControl s, Point p)
        {   s.Refresh();
            this.BeingDrawn(s.CreateGraphics(), this.startingPoint, p);
        }
        public override void MouseUp(SketchControl s, Point p)
        {   base.MouseUp(s, p);
            this.Finished(s.MaakBitmapGraphics(), this.startingPoint, p);
            s.Invalidate();
        }
        public override void Letter(SketchControl s, char c)
        {
        }
        public abstract void BeingDrawn(Graphics g, Point p1, Point p2);
        
        public virtual void Finished(Graphics g, Point p1, Point p2)
        {   this.BeingDrawn(g, p1, p2);
        }
    }

    public class EllipseTool : DualPointTool
    {
        public override string ToString() { return "circel"; }

        public override void BeingDrawn(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(CreatePen(brush, 3), p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        }
    }

    public class FilledEllipseTool : EllipseTool
    {
        public override string ToString() { return "circelV"; }

        public override void Finished(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(brush, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        }
    }

    public class RectangleTool : DualPointTool
    {
        public override string ToString() { return "kader"; }

        public override void BeingDrawn(Graphics g, Point p1, Point p2)
        {   g.DrawRectangle(CreatePen(brush,3), DualPointTool.PointsToRectangle(p1, p2));
        }
    }
    
    public class FilledRectangleTool : RectangleTool
    {
        public override string ToString() { return "vlak"; }

        public override void Finished(Graphics g, Point p1, Point p2)
        {   g.FillRectangle(brush, DualPointTool.PointsToRectangle(p1, p2));
        }
    }

    public class LineTool : DualPointTool
    {
        public override string ToString() { return "lijn"; }

        public override void BeingDrawn(Graphics g, Point p1, Point p2)
        {   g.DrawLine(CreatePen(this.brush,3), p1, p2);
        }
    }

    public class PenTool : LineTool
    {
        public override string ToString() { return "pen"; }

        public override void MouseDrag(SketchControl s, Point p)
        {   this.MouseUp(s, p);
            this.MouseDown(s, p);
        }
    }
    
    public class EraserTool : PenTool
    {
        public override string ToString() { return "gum"; }

        public override void BeingDrawn(Graphics g, Point p1, Point p2)
        {   g.DrawLine(CreatePen(Brushes.White, 7), p1, p2);
        }
    }
}
