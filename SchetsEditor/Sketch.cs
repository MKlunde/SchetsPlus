using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SketchEditor
{
    public class Sketch
    {
        private List<ISketchObject> objects;
        private Bitmap bitmap;
        private SketchControl s;
        private Image loadedImage;//Houd de achtergrond tekenig bij

        private bool IsketchlistChanged = false; // Houdt bij of de list veranderd is
        
        public bool listChanged {
            get { return IsketchlistChanged; }
            set { IsketchlistChanged = value; }
        }

        public Image ImageLoad { set { loadedImage = value; } }

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
            IsketchlistChanged = true;// geeft aan dat de ISketchList verandert is
        }
        public void Draw(Graphics g) {
            Graphics bitmapGraphics = Graphics.FromImage(bitmap);
            bitmapGraphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            for (int i = 0; i < objects.Count; i++) {
                objects[i].Draw(bitmapGraphics);
            }
            if (loadedImage != null)
            {
                g.DrawImage(loadedImage, 0, 0);
                bitmap.MakeTransparent(Color.White);//Maak de bitmap doorzichtig waardoor de achtergrond zichtbaar is.
                g.DrawImage(bitmap, 0, 0);
            }
            else
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
            for (int i = 0; i < objects.Count; i++) {
                objects[i].Rotate();
            }
        }
        public ISketchObject ObjectOnLocation(Point p) {
            for (int i = objects.Count-1; i >= 0; i--) {
                if (objects[i].IsOnLocation(p))
                    return objects[i];
            }
            return null;
        }


        public Graphics BitmapGraphics {
            get { return Graphics.FromImage(new Bitmap(bitmap.Size.Width, bitmap.Size.Height)); }
        }
    }
}
