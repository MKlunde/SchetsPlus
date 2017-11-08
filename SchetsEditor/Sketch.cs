using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace SketchEditor
{
    public class Sketch
    {
        private List<ISketchObject> objects;
        private Bitmap bitmap;
        private SketchControl s;

        private bool IsketchlistChanged = false;//houdt bij of de list veranderd is
        public event ListChangedEventHandler ListChanged;
        
        public bool listChanged {
            get { return IsketchlistChanged; }
            set { IsketchlistChanged = value; }
        }

        public List<ISketchObject> Objects {
            get { return objects; }
            set { objects = value; }
        }
        public Bitmap Bitmap{
            get {return bitmap; }
        }

        public Sketch(SketchControl s) {
            this.s = s;
            objects = new List<ISketchObject>();
            bitmap = new Bitmap(1, 1);
        }
        public void AddObject(ISketchObject obj) {
            objects.Add(obj);
        }
        public void Draw(Graphics g) {
            Graphics bitmapGraphics = Graphics.FromImage(bitmap);
            bitmapGraphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            for (int i = 0; i < objects.Count; i++) {
                objects[i].Draw(bitmapGraphics);
            }
            g.DrawImage(bitmap, 0, 0);
        }
        public void Resize(Size sz) {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height) {
                Bitmap newBitmap = new Bitmap(
                    Math.Max(sz.Width, bitmap.Size.Width),
                    Math.Max(sz.Height, bitmap.Size.Height)
                );
                bitmap = newBitmap;
                s.Invalidate();
            }
        }
        public void Clear() {
            objects = new List<ISketchObject>();
        }
        public void Rotate() {
            // ...
        }
        public ISketchObject ObjectOnLocation(Point p) {
            ISketchObject result = null;
            for (int i = objects.Count-1; i >= 0; i--) {
                if (objects[i].IsOnLocation(p))
                    result = objects[i];
            }
            return result;
        }


        public Graphics BitmapGraphics {
            get { return Graphics.FromImage(new Bitmap(bitmap.Size.Width, bitmap.Size.Height)); }
        }
    }
}
