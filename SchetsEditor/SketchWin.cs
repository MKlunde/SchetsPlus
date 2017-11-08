using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SketchEditor
{
    public class SketchWin : Form
    {
        MainWindow parentWindow;
        MenuStrip menuStrip;
        SketchControl sketchControl;
        ISketchTool currentTool;
        Panel panel;
        bool mouseDown;
        ResourceManager resourceManager = new ResourceManager(
                "SchetsEditor.Properties.Resources",
                Assembly.GetExecutingAssembly()
        );

        private void ResizeWin(object sender, EventArgs e)
        {
            sketchControl.Size = new Size(
                ClientSize.Width - 70,
                ClientSize.Height - 50);
            panel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void ToolMenu_click(object sender, EventArgs e)
        {
            currentTool = (ISketchTool)((ToolStripMenuItem)sender).Tag;
        }

        private void ToolButton_click(object sender, EventArgs e)
        {
            currentTool = (ISketchTool)((RadioButton)sender).Tag;
        }

        public void Exit(object obj, EventArgs e)
        {
            if (this.sketchControl.Sketch.listChanged)
            {
                DialogResult dialogResult = MessageBox.Show("Er zijn niet opgeslagen veranderingen. Weet je zeker dat je wil afsluiten?", "Alert", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Close();
                }
            }
        }

        private void SketchWin_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.sketchControl.Sketch.listChanged)
            {
                DialogResult dialogResult = MessageBox.Show("Er zijn niet opgeslagen veranderingen. Weet je zeker dat je wil afsluiten?", "Alert", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    parentWindow.SketchWinMenuItems(true);
                }
            }
        }

        public SketchWin(MainWindow parentWindow)
        {
            this.parentWindow = parentWindow;

            ISketchTool[] theTools = {
                new PenTool(),
                new LineTool(),
                new RectangleTool(),
                new FilledRectangleTool(),
                new EllipseTool(),
                new FilledEllipseTool(),
                new TextTool(),
                new EraserTool()
            };
            String[] theColors = {
                "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan"
            };

            ClientSize = new Size(700, 500);
            currentTool = theTools[0];

            sketchControl = new SketchControl();
            sketchControl.Location = new Point(64, 10);
            sketchControl.MouseDown += (object sender, MouseEventArgs e) => {
                mouseDown = true;
                currentTool.MouseDown(sketchControl, e.Location);
            };
            sketchControl.MouseMove += (object sender, MouseEventArgs e) => {
                if (mouseDown)
                    currentTool.MouseDrag(sketchControl, e.Location);
            };
            sketchControl.MouseUp += (object sender, MouseEventArgs e) => {
                if (mouseDown)
                    currentTool.MouseUp(sketchControl, e.Location);
                mouseDown = false;
            };
            sketchControl.KeyPress += (object sender, KeyPressEventArgs e) => {
                currentTool.Letter(sketchControl, e.KeyChar);
            };
            Controls.Add(sketchControl);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            Controls.Add(menuStrip);
            CreateFileMenu();
            CreateToolMenu(theTools);
            CreateActionMenu(theColors);
            CreateToolButtons(theTools);
            CreateActionButtons(theColors);
            Resize += ResizeWin;
            ResizeWin(null, null);
            FormClosed += SketchWin_FormClosed;
        }

        private void CreateFileMenu()
        {
            /*ToolStripMenuItem menu = new ToolStripMenuItem("Bestand");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.Exit);
            menuStrip.Items.Add(menu);*/
        }

        private void CreateToolMenu(ICollection<ISketchTool> tools)
        {
            if (parentWindow.MdiChildren.Length == 0) {
                foreach (ISketchTool tool in tools) {
                    Console.WriteLine(parentWindow.MdiChildren.Length);
                    ToolStripItem item = new ToolStripMenuItem();
                    item.Tag = tool;
                    item.Text = tool.ToString();
                    item.Image = (Image)resourceManager.GetObject(tool.ToString());
                    item.Click += ToolMenu_click;
                    parentWindow.toolMenu.DropDownItems.Add(item);
                }
            }
        }

        private void CreateActionMenu(String[] kleuren)
        {
            if (parentWindow.MdiChildren.Length == 0) {
                parentWindow.actionMenu.DropDownItems.Add("Clear", null, sketchControl.ClearSketch);
                parentWindow.actionMenu.DropDownItems.Add("Roteer", null, sketchControl.RotateSketch);
                ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
                foreach (string k in kleuren)
                    submenu.DropDownItems.Add(k, null, sketchControl.ChangeColorViaMenu);
                parentWindow.actionMenu.DropDownItems.Add(submenu);
            }
        }

        private void CreateToolButtons(ICollection<ISketchTool> tools)
        {
            int t = 0;
            foreach (ISketchTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.FlatStyle = FlatStyle.Flat;
                b.Font = new Font("Tahoma", 8);
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 200, 200, 200);
                b.Size = new Size(45, 60);
                b.Location = new Point(10, 10 + t * 60);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourceManager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += ToolButton_click;
                Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        public void LoadProject()
        {
            OpenFileDialog d = new OpenFileDialog(); //selecteer file
            // instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Project laden...";
            d.Filter = "Sketch files(*.sketch)| *.sketch";
            if (d.ShowDialog() == DialogResult.OK)
            {
                string filename = d.FileName;
                string ObjectName;
                this.sketchControl.Sketch.Clear();
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("start loop");
                        while ((ObjectName = file.ReadLine()) != null || ObjectName.Length < 4)
                        {
                            // read variables
                            string[] startPoint = file.ReadLine().Split(' ');
                            string[] endPoint = file.ReadLine().Split(' ');
                            string[] brush = file.ReadLine().Split(' ');
                            string text = file.ReadLine();
                            //string rotation = file.ReadLine();
                            Color col = Color.FromArgb(int.Parse(brush[0]), int.Parse(brush[1]), int.Parse(brush[2]));
                            Point p1 = new Point(int.Parse(startPoint[0]), int.Parse(startPoint[1]));
                            Point p2 = new Point(int.Parse(endPoint[0]), int.Parse(endPoint[1]));
                            var s = this.sketchControl;

                            ISketchObject obj;
                            switch (ObjectName)//creër object naar soort
                            {
                                case ("FilledEllipseObject"):
                                    obj = new FilledEllipseObject(s, p1, new SolidBrush(col));
                                    break;

                                case ("EllipseObject"):
                                    obj = new EllipseObject(s, p1, new SolidBrush(col));
                                    break;

                                case ("RectangleObject"):
                                    obj = new RectangleObject(s, p1, new SolidBrush(col));
                                    break;

                                case ("FilledRectangleObject"):
                                    obj = new FilledRectangleObject(s, p1, new SolidBrush(col));
                                    break;

                                default:
                                    obj = new RectangleObject(s, p1, new SolidBrush(col));
                                    break;
                            }
                            s.SketchAddObject(obj); // Voeg object toe aan object list
                            obj.ChangeEndingPoint(p2); // Zet endingPoint goed
                            obj.Finish(); // Zet finished op true (de gebruiker is het object immers niet meer aan het tekenen)
                        }
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }

                }
                System.Diagnostics.Debug.WriteLine("end loop");
            }
        }
        public void Store()
        {
            SaveFileDialog d = new SaveFileDialog(); //selecteer file
            // instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Opslaan als project...";
            d.Filter = "Sketch files (*.sketch)|*.sketch";
            if (d.ShowDialog() == DialogResult.OK)
            {
                string filename = d.FileName;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    foreach(ISketchObject obj in this.sketchControl.Sketch.Objects)
                    {
                        file.WriteLine(obj.Name);
                        file.WriteLine(obj.StartingPoint);
                        file.WriteLine(obj.EndingPoint);
                        file.WriteLine(obj.Brush);
                        file.WriteLine(obj.Text);
                        //file.WriteLine(obj.rotation);
                    }
                }
                this.sketchControl.Sketch.listChanged = false;//geeft aan dat veranderingen zijn opgeslagen
            }
        }

        public void StoreImage()
        {
            SaveFileDialog d = new SaveFileDialog(); //selecteer file
            // instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Opslaan als afbeelding...";
            d.Filter = "Afbeeldingsbestanden (*.Bmp, .*Png, *.jpg) | *.Bmp; *.Png; *jpg";
            if (d.ShowDialog() == DialogResult.OK)
            {
                string fileName = d.FileName;
                Bitmap img = this.sketchControl.Sketch.Bitmap;//Get bitmap
                switch (System.IO.Path.GetExtension(fileName))//Save op basis van extensie
                {
                    case (".Png"):
                        img.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case (".jpg"):
                        img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case (".Bmp"):
                        img.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    default:
                        fileName = fileName + ".jpg"; //geef een extensie aan nieuwe files
                        img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }
            }
            this.sketchControl.Sketch.listChanged = false;//geeft aan dat veranderingen zijn opgeslagen
        }     

        private void CreateActionButtons(String[] kleuren)
        {   
            panel = new Panel();
            panel.Size = new Size(600, 24);
            Controls.Add(panel);
            
            Button b; Label l; ComboBox cbb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += sketchControl.ClearSketch; 
            panel.Controls.Add(b);
            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += sketchControl.RotateSketch; 
            panel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Penkleur:"; 
            l.Location = new Point(180, 3); 
            l.AutoSize = true;               
            panel.Controls.Add(l);
            
            cbb = new ComboBox(); cbb.Location = new Point(240, 0); 
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += sketchControl.ChangeColor;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            panel.Controls.Add(cbb);
        }
    }
}
