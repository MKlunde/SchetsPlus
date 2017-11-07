using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Drawing.Imaging;
using System.Xml.Serialization;

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
            sketchControl.Size = new Size (
                ClientSize.Width  - 70,
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
            Close();
        }
        
        private void SketchWin_FormClosed(object sender, FormClosedEventArgs e) {
            parentWindow.SketchWinMenuItems(true);
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
            foreach (ISketchTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourceManager.GetObject(tool.ToString());
                item.Click += ToolMenu_click;
                parentWindow.toolMenu.DropDownItems.Add(item);
            }
        }

        private void CreateActionMenu(String[] kleuren)
        {
            parentWindow.actionMenu.DropDownItems.Add("Clear", null, sketchControl.ClearSketch );
            parentWindow.actionMenu.DropDownItems.Add("Roteer", null, sketchControl.RotateSketch );
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, sketchControl.ChangeColorViaMenu);
            parentWindow.actionMenu.DropDownItems.Add(submenu);
        }

        private void CreateToolButtons(ICollection<ISketchTool> tools)
        {
            int t = 0;
            foreach (ISketchTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
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
            d.Filter = "Xml files (*.Xml)|*.Xml";
            if (d.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filename = d.FileName;
                    System.Diagnostics.Debug.WriteLine("Filename= " + filename);
                    XmlSerializer reader = new XmlSerializer(typeof(List<ISketchObject>));
                    StreamReader file = new StreamReader(filename);
                    this.sketchControl.Sketch.Objects = (List<ISketchObject>)reader.Deserialize(file);
                    file.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("test load failed");
                }
            }
        }
        public void Store()
        {
            SaveFileDialog d = new SaveFileDialog(); //selecteer file
            // instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Opslaan als project...";
            d.Filter = "Xml files (*.Xml)|*.Xml";
            if (d.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filename = d.FileName;
                    System.Diagnostics.Debug.WriteLine("Filename= " + filename);
                    XmlSerializer ser = new XmlSerializer(typeof(List<ISketchObject>));
                    TextWriter writer = new StreamWriter(filename);
                    var b = this.sketchControl.Sketch.Objects;
                    ser.Serialize(writer, b);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("test store failed");
                }
            }
            /*
            using (StreamWriter sw = File.CreateText(filename))
            {//Open een writer naar gekozen bestand
                //File.WriteAllText(filename, String.Empty);// maak het bestand leeg
                foreach (var obj in this.sketchControl.Sketch.Objects) //Selecteer elk object
                {

                    sw.WriteLine(obj); //Schrijf het object naar een bestand
                    sw.WriteLine(obj.Name); //Schrijf het object naar een bestand

                }
                sw.Close();
            }*/
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
                try
                {
                    string fileName = d.FileName;
                    File.Create(fileName);//Maak een nieuwe file aan  
                    Bitmap img = this.sketchControl.Sketch.Bitmap;//Get bitmap
                    //var format = System.Drawing.Imaging.ImageFormat.Jpeg;//default
                    /*switch (System.IO.Path.GetExtension(fileName))
                     {
                    case (".png"):
                    format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                   case (".jpg"):
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                   break;
                   case (".Bmp"):
                   format = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                   default:
                   fileName = fileName + ".jpg"; //geef een extensie aan nieuwe files
                   System.Diagnostics.Debug.WriteLine("default called");
                    break;
                   }   */
                    if (!System.IO.Path.HasExtension(fileName) || System.IO.Path.GetExtension(fileName) != "jpg") 
                        fileName = fileName + ".jpg";
                    img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("test saveAsImage failed");
                }
            }
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
