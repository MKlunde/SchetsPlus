using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SketchEditor
{   public class SketchControl : UserControl
    {   private Sketch sketch;
        private Color penColor;

        public Color PenColor
        { get { return penColor; }
        }
        public Sketch Sketch
        { get { return sketch;   }
        }
        public SketchControl()
        {   this.BorderStyle = BorderStyle.Fixed3D;
            this.sketch = new Sketch();
            this.Paint += this.DrawSketch;
            this.Resize += this.ResizeControl;
            this.ResizeControl(null, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void DrawSketch(object o, PaintEventArgs pea)
        {   sketch.Draw(pea.Graphics);
        }
        private void ResizeControl(object o, EventArgs ea)
        {   sketch.Resize(this.ClientSize);
            this.Invalidate();
        }
        public Graphics CreateBitmapGraphics()
        {   Graphics g = sketch.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void ClearSketch(object o, EventArgs ea)
        {   sketch.Clear();
            this.Invalidate();
        }
        public void RotateSketch(object o, EventArgs ea)
        {   sketch.Resize(new Size(this.ClientSize.Height, this.ClientSize.Width));
            sketch.Rotate();
            this.Invalidate();
        }
        public void ChangeColor(object obj, EventArgs ea)
        {   string colorName = ((ComboBox)obj).Text;
            penColor = Color.FromName(colorName);
        }
        public void ChangeColorViaMenu(object obj, EventArgs ea)
        {   string colorName = ((ToolStripMenuItem)obj).Text;
            penColor = Color.FromName(colorName);
        }
    }
}
