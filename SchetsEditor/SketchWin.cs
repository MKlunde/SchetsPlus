using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;

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

        Button clearButton, rotateButton, colorButton;
        Label colorLabel;

        ResourceManager resourceManager = new ResourceManager(
                "SchetsEditor.Properties.Resources",
                Assembly.GetExecutingAssembly()
        );

        public SketchControl SketchControl {
            get { return sketchControl;  }
        }

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
            Close();
        }

        private void SketchWin_FormClosing(object sender, FormClosingEventArgs e) {
            if (sketchControl.Sketch.listChanged) {
                DialogResult dialogResult = MessageBox.Show("Er zijn niet-opgeslagen wijzigingen. Wil je deze opslaan?", "Niet-opgeslagen wijzigingen", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes) {
                    e.Cancel = true;
                    SaveProject();
                }
                if (dialogResult == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            }
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

            sketchControl = new SketchControl(this);
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
            CreateToolMenu(theTools);
            CreateActionMenu(theColors);
            CreateToolButtons(theTools);
            CreateActionButtons(theColors);
            Resize += ResizeWin;
            ResizeWin(null, null);
            FormClosing += SketchWin_FormClosing;

            sketchControl.ChangeColor(Color.Black);
        }

        private void CreateToolMenu(ICollection<ISketchTool> tools)
        {
            if (parentWindow.MdiChildren.Length == 0) {
                foreach (ISketchTool tool in tools) {
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
                b.Cursor = Cursors.Hand;
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
        private void CreateActionButtons(String[] kleuren) {
            panel = new Panel();
            panel.Size = new Size(600, 24);
            Controls.Add(panel);

            clearButton = new Button();
            rotateButton = new Button();
            colorButton = new Button();
            colorLabel = new Label();

            clearButton.FlatStyle = rotateButton.FlatStyle = colorButton.FlatStyle = FlatStyle.Flat;
            clearButton.Cursor = rotateButton.Cursor = colorButton.Cursor = Cursors.Hand;

            clearButton.Text = "Clear";
            clearButton.Location = new Point(0, 0);
            clearButton.Click += sketchControl.ClearSketch;
            panel.Controls.Add(clearButton);

            rotateButton.Text = "Rotate";
            rotateButton.Location = new Point(80, 0);
            rotateButton.Click += sketchControl.RotateSketch;
            panel.Controls.Add(rotateButton);

            colorButton.Text = "Penkleur";
            ChangeColorButtonColor(sketchControl.PenColor);
            colorButton.FlatAppearance.BorderColor = Color.Black;
            colorButton.Location = new Point(180, 0);
            colorButton.Click += ColorButton_click;
            panel.Controls.Add(colorButton);

            /*/*colorLabel.Text = "Penkleur:";
            colorLabel.Location = new Point(180, 3);
            colorLabel.AutoSize = true;
            panel.Controls.Add(colorLabel);

            ComboBox cbb = new ComboBox();
            cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += sketchControl.ChangeColorFromComboBox;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            panel.Controls.Add(cbb);*/
        }

        public void ChangeColorButtonColor(Color col) {
            colorButton.BackColor = col;
            Console.WriteLine(sketchControl.PenColor.GetBrightness());
            if(sketchControl.PenColor.GetBrightness() > 0.4)
                colorButton.ForeColor = Color.Black;
            else
                colorButton.ForeColor = Color.White;
        }

        void ColorButton_click(object sender, EventArgs e) {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = sketchControl.PenColor;
            DialogResult result = colorDialog.ShowDialog();
            if (result == DialogResult.OK) {
                sketchControl.ChangeColor(colorDialog.Color);
            }
        }

        public void SaveProject()
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
                sketchControl.Sketch.listChanged = false; // Geef aan dat er geen nieuwe wijzigingen zijn aan de sketch
            }
        }

        public void ExportImage()
        {
            SaveFileDialog d = new SaveFileDialog(); // Selecteer file
            // Instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Exporteer afbeelding...";
            d.Filter = "Afbeeldingsbestanden (*.Bmp, .*Png, *.jpg) | *.Bmp; *.Png; *jpg";
            if (d.ShowDialog() == DialogResult.OK) {
                string fileName = d.FileName;
                Bitmap img = sketchControl.Sketch.Bitmap; // Get bitmap
                switch (System.IO.Path.GetExtension(fileName)) // Save op basis van extensie
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
                        fileName = fileName + ".png"; // Geef een extensie aan nieuwe files
                        img.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }
            }
        }     
    }
}
