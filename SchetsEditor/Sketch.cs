using System;
using System.Collections.Generic;
using System.Drawing;

namespace SketchEditor
{
    /*public class Sketch // Old
    {
        private Bitmap bitmap;
        
        public Sketch()
        {
            bitmap = new Bitmap(1, 1);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void Resize(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap newBitmap = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(newBitmap);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = newBitmap;
            }
        }
        public void Draw(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }
        public void Clear()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Rotate()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }*/

    public class Sketch
    {
        private List<ISketchObject> objects;
        private Bitmap bitmap;
        private SketchControl sketchControl;

        public List<ISketchObject> Objects {
            get { return objects; }
        }
        public Sketch(SketchControl sketchControl) {
            this.sketchControl = sketchControl;
            objects = new List<ISketchObject>();
            bitmap = new Bitmap(1, 1);
        }
        public void AddObject(ISketchObject obj) {
            objects.Add(obj);
        }
        public void Draw(Graphics g) {
            Graphics bitmapGraphics = Graphics.FromImage(bitmap);
            bitmapGraphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            Console.Write(objects.Count + ": ");
            for (int i = 0; i < objects.Count; i++) {
                //objects[i].Draw(bitmapGraphics);
                bitmapGraphics.DrawEllipse(new Pen(Color.Black, 3), i%10*50, (int)Math.Floor((double)i/10)*50, 40, 40);
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
                sketchControl.Invalidate();
            }
        }
        public void Clear() {
            objects = new List<ISketchObject>();
        }
        public void Rotate() {
            // ...
        }


        public Graphics BitmapGraphics {
            get { return Graphics.FromImage(new Bitmap(bitmap.Size.Width, bitmap.Size.Height)); }
        }
    }
}
