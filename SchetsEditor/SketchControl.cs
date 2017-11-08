﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SketchEditor
{
    public class SketchControl : UserControl
    {
        private Sketch sketch;
        private Color penColor;
        private ISketchObject currentObject;
        private Font textFont = new Font("Tahoma", 40);

        public Color PenColor {
            get { return penColor; }
            set { PenColor = value; }
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

        public SketchControl() {
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
        public void ChangeColor(object obj, EventArgs ea) {
            string colorName = ((ComboBox)obj).Text;
            penColor = Color.FromName(colorName);
        }
        public void ChangeColorViaMenu(object obj, EventArgs ea) {
            string colorName = ((ToolStripMenuItem)obj).Text;
            penColor = Color.FromName(colorName);
        }

        /* Toegevoegd */
        public void SketchAddObject(ISketchObject obj) {
            sketch.AddObject(obj);
            currentObject = obj;
        }
        public ISketchObject SketchObjectOnLocation(Point p) {
            return sketch.ObjectOnLocation(p);
        }
    }
}
