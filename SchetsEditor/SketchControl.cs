using System;
using System.Collections.Generic;
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
        ISketchObject currentObject;
        Font textFont = new Font("Tahoma", 40);

        public Color PenColor {
            get { return penColor; }
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
            Resize += ResizeControl;
            ResizeControl(null, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
        }
        private void DrawSketch(object o, PaintEventArgs pea) {
            sketch.Draw(pea.Graphics);
        }
        private void ResizeControl(object o, EventArgs ea) {
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
        public void ChangeColor(Color col) {
            penColor = col;
            sketchWin.ChangeColorButtonColor(col);
        }
        public void ChangeColorFromComboBox(object obj, EventArgs ea) {
            ChangeColor(Color.FromName(((ComboBox)obj).Text));
        }
        public void ChangeColorViaMenu(object obj, EventArgs ea) {
            ChangeColor(Color.FromName(((ToolStripMenuItem)obj).Text));
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
