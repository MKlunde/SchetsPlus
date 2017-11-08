using System;
using System.Collections.Generic;
using System.Drawing;

namespace SketchEditor
{
    public class Sketch
    {
        private List<ISketchObject> objects;
        private Bitmap bitmap;
        private SketchControl s;
        private Image loadedImage; // Houd de achtergrondafbeelding bij

        private bool IsketchlistChanged = false; // Houd bij of er wijzigingen zijn
        
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
        public void AddObject(ISketchObject obj) { // Voeg nieuw object toe aan sketch
            objects.Add(obj);
            IsketchlistChanged = true; // Geeft aan dat er wijzigingen zijn
        }
        public void Draw(Graphics g) { // Teken alle objecten
            Graphics bitmapGraphics = Graphics.FromImage(bitmap);
            bitmapGraphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            if (loadedImage != null) {
                bitmapGraphics.DrawImage(loadedImage, 0, 0);
            }
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
        public void Clear() { // Wis sketch
            objects = new List<ISketchObject>();
        }
        public void Rotate() { // Roteer de sketch
            for (int i = 0; i < objects.Count; i++) {
                objects[i].Rotate();
            }
        }
        public ISketchObject ObjectOnLocation(Point p) { // Check of er een object is op een bepaalde locatie, en geef zo ja het bovenste object dat daar is terug
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
