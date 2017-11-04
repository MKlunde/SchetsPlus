using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SketchEditor
{
    public class SketchWin : Form
    {   
        MenuStrip menuStrip;
        SketchControl sketchControl;
        ISketchTool currentTool;
        Panel panel;
        bool mouseDown;
        ResourceManager resourceManager = new ResourceManager(
                "SchetsEditor.Properties.Resources",
                Assembly.GetExecutingAssembly()
        );

        private void ResizeWin(object o, EventArgs ea)
        {
            sketchControl.Size = new Size (
                this.ClientSize.Width  - 70,
                this.ClientSize.Height - 50);
            panel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void ToolMenu_click(object obj, EventArgs ea)
        {
            this.currentTool = (ISketchTool)((ToolStripMenuItem)obj).Tag;
        }

        private void ToolButton_click(object obj, EventArgs ea)
        {
            this.currentTool = (ISketchTool)((RadioButton)obj).Tag;
        }

        private void Exit(object obj, EventArgs ea)
        {
            this.Close();
        }

        public SketchWin()
        {
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

            this.ClientSize = new Size(700, 500);
            currentTool = theTools[0];

            sketchControl = new SketchControl();
            sketchControl.Location = new Point(64, 10);
            sketchControl.MouseDown += (object o, MouseEventArgs mea) => {
                mouseDown = true;
                currentTool.MouseDown(sketchControl, mea.Location); 
            };
            sketchControl.MouseMove += (object o, MouseEventArgs mea) => {
                if (mouseDown)
                    currentTool.MouseDrag(sketchControl, mea.Location); 
            };
            sketchControl.MouseUp += (object o, MouseEventArgs mea) => {
                if (mouseDown)
                    currentTool.MouseUp(sketchControl, mea.Location);
                mouseDown = false;
            };
            sketchControl.KeyPress += (object o, KeyPressEventArgs kpea) => {
                currentTool.Letter(sketchControl, kpea.KeyChar); 
            };
            this.Controls.Add(sketchControl);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.CreateFileMenu();
            this.CreateToolMenu(theTools);
            this.CreateActionMenu(theColors);
            this.CreateToolButtons(theTools);
            this.CreateActionButtons(theColors);
            this.Resize += this.ResizeWin;
            this.ResizeWin(null, null);
        }

        private void CreateFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.Exit);
            menuStrip.Items.Add(menu);
        }

        private void CreateToolMenu(ICollection<ISketchTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISketchTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourceManager.GetObject(tool.ToString());
                item.Click += this.ToolMenu_click;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void CreateActionMenu(String[] kleuren)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
            menu.DropDownItems.Add("Clear", null, sketchControl.ClearSketch );
            menu.DropDownItems.Add("Roteer", null, sketchControl.RotateSketch );
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, sketchControl.ChangeColorViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
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
                b.Click += this.ToolButton_click;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void CreateActionButtons(String[] kleuren)
        {   
            panel = new Panel();
            panel.Size = new Size(600, 24);
            this.Controls.Add(panel);
            
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
