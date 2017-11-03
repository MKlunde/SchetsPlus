using System;
using System.Collections.Generic;
using System.Drawing;

namespace SketchEditor
{
    public class Sketch // Old
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
    }

    /*public class Schets // New
    {
        private List<ISchetsTool> objects;

        public Schets() {
            objects = new List<ISchetsTool>();
        }

        public void AddObject(ISchetsTool obj) {
            objects.Add(obj);
        }
    }*/
}
