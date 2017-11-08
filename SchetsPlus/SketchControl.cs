using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SketchEditor
{
    public class SketchControl : UserControl
    {
        SketchWin sketchWin;
        Sketch sketch;
        Color penColor;
        int penWidth;
        ISketchObject currentObject;
        Font textFont = new Font("Tahoma", 40);

        public Color PenColor {
            get { return penColor; }
        }
        public int PenWidth {
            get { return penWidth; }
        }
        public Sketch Sketch {
            get { return sketch; }
        }
        public ISketchObject CurrentObject {
            get { return currentObject; }
        }
        public Font TextFont {
            get { return textFont; }
        }

        public SketchControl(SketchWin sketchWin) {
            this.sketchWin = sketchWin;
            BorderStyle = BorderStyle.Fixed3D;
            sketch = new Sketch(this);
            Paint += DrawSketch;
            Resize += ResizeSketch;
            ResizeSketch(null, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
        }
        private void DrawSketch(object o, PaintEventArgs pea) {
            sketch.Draw(pea.Graphics);
        }
        private void ResizeSketch(object o, EventArgs ea) {
            sketch.Resize(ClientSize);
            Invalidate();
        }
        public Graphics CreateBitmapGraphics() {
            Graphics g = sketch.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void ClearSketch(object o, EventArgs ea) {
            sketch.Clear();
            Invalidate();
        }
        public void RotateSketch(object o, EventArgs ea) {
            sketch.Resize(new Size(ClientSize.Height, ClientSize.Width));
            sketch.Rotate();
            Invalidate();
        }
        public void ChangePenColor(Color col) {
            penColor = col;
            sketchWin.ChangeColorButtonColor(col);
        }
        public void ChangeColorViaMenu(object obj, EventArgs ea) {
            ChangePenColor(Color.FromName(((ToolStripMenuItem)obj).Text));
        }

        public void ChangePenWidth(int w) {
            penWidth = w;
            sketchWin.ChangeWidthButtonValue(w);
        }
        public void ChangePenWidthFromTrackbar(object obj, EventArgs ea) {
            ChangePenWidth(((TrackBar)obj).Value);
        }

        public void SketchAddObject(ISketchObject obj) {
            sketch.AddObject(obj);
            currentObject = obj;
        }
        public ISketchObject SketchObjectOnLocation(Point p) {
            return sketch.ObjectOnLocation(p);
        }
    }
}
