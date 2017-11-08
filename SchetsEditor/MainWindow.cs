using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SketchEditor
{
    public class MainWindow : Form
    {
        public MenuStrip menuStrip;
        public ToolStripDropDownItem fileMenu, toolMenu, actionMenu, helpMenu;

        public MainWindow(){
            ClientSize = new Size(800, 600);
            CreateMenuStrip();
            Text = "Schetseditor";
            IsMdiContainer = true;
            MainMenuStrip = menuStrip;
            NewSketchWin(); // Open lege SketchWin bij opstarten
        }

        private void CreateMenuStrip() {
            // Initialiseer menuStrip
            menuStrip = new MenuStrip();
            Controls.Add(menuStrip);

            // Maak dropdown-menu "Bestand"
            fileMenu = new ToolStripMenuItem("Bestand");
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Nieuw", null, NewSketchWin) { ShortcutKeyDisplayString = "Ctrl+N", ShortcutKeys = Keys.Control | Keys.N });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Exporteren als afbeelding...", null, SaveAsImage) { Enabled = false, ShortcutKeyDisplayString = "Ctrl+E", ShortcutKeys = Keys.Control | Keys.E });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Opslaan als project...", null, Save) { Enabled = false, ShortcutKeyDisplayString = "Ctrl+S", ShortcutKeys = Keys.Control | Keys.S });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Project openen...", null, LoadProject) { ShortcutKeyDisplayString = "Ctrl+O", ShortcutKeys = Keys.Control | Keys.O });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Afbeelding openen...", null, LoadImage)  { ShortcutKeyDisplayString = "Ctrl+g", ShortcutKeys = Keys.Control | Keys.O });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Sluiten", null, CloseActiveSketchWin) { Enabled = false, ShortcutKeyDisplayString = "Ctrl+W", ShortcutKeys = Keys.Control | Keys.W });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("SketchPlus Afsluiten", null, Exit) { ShortcutKeyDisplayString = "Ctrl+Q", ShortcutKeys = Keys.Control | Keys.Q });

            toolMenu = new ToolStripMenuItem("Tool");
            toolMenu.DropDownItems.Add(new ToolStripMenuItem("Geen tools beschikbaar", null) { Enabled = false });

            actionMenu = new ToolStripMenuItem("Actie");
            actionMenu.DropDownItems.Add(new ToolStripMenuItem("Geen acties beschikbaar", null) { Enabled = false });

            // Maak dropdown-menu "Help"
            helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add(new ToolStripMenuItem("Over \"Schets\"", null, About));

            menuStrip.Items.AddRange(new ToolStripDropDownItem[] { fileMenu, toolMenu, actionMenu, helpMenu });

            SketchWinMenuItems();
        }

        public void SketchWinMenuItems(bool forceClosed = false) {
            bool atLeastOneSketchWin = MdiChildren.Length > 0 && !forceClosed;
            GetDropDownItemFromMenu(fileMenu, "Exporteren als afbeelding...").Enabled = atLeastOneSketchWin;
            GetDropDownItemFromMenu(fileMenu, "Opslaan als project...").Enabled = atLeastOneSketchWin;
            GetDropDownItemFromMenu(fileMenu, "Sluiten").Enabled = atLeastOneSketchWin;
            GetDropDownItemFromMenu(toolMenu, "Geen tools beschikbaar").Visible = !atLeastOneSketchWin;
            GetDropDownItemFromMenu(actionMenu, "Geen acties beschikbaar").Visible = !atLeastOneSketchWin;
            if (!atLeastOneSketchWin) {
                for (int i = toolMenu.DropDownItems.Count - 1; i > 0; i--)
                    toolMenu.DropDownItems.RemoveAt(i);
                for (int i = actionMenu.DropDownItems.Count - 1; i > 0; i--)
                    actionMenu.DropDownItems.RemoveAt(i);
            }
        }

        public ToolStripMenuItem GetDropDownItemFromMenu(ToolStripDropDownItem menu, string text) {
            foreach (ToolStripMenuItem item in menu.DropDownItems) {
                if (item.Text == text) {
                    return item;
                }
            }
            return null;
        }

        private void About(object o, EventArgs ea) {
            MessageBox.Show("SchetsPlus versie 0.1\nDoor Menno Klunder en Mats Gottenbos\nGebaseerd op Schets versie 1.0 - (c) UU Informatica 2010",
                "Over \"Schets\"",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public void NewSketchWin(object sender = null, EventArgs e = null) {
            ReturnNewSketchWin();
        }
        public SketchWin ReturnNewSketchWin(object sender = null, EventArgs e = null) {
            SketchWin s = new SketchWin(this);
            s.MdiParent = this;
            s.Show();
            SketchWinMenuItems();
            return s;
        }

        public void CloseActiveSketchWin(object sender = null, EventArgs e = null) {
            SketchWin s = ActiveMdiChild as SketchWin;
            try {
                s.Exit(sender, e);
            }
            catch { }
            SketchWinMenuItems();
        }
        private void LoadProject(object sender, EventArgs e)
        {
            SketchWin activeSketchWin = ActiveMdiChild as SketchWin;
            OpenFileDialog d = new OpenFileDialog(); // Selecteer file
            // Instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Project laden...";
            d.Filter = "Sketch files(*.sketch)| *.sketch";
            if (d.ShowDialog() == DialogResult.OK) {
                SketchControl s;
                if (activeSketchWin.SketchControl.Sketch.Objects.Count == 0)
                    s = activeSketchWin.SketchControl;
                else
                    s = ReturnNewSketchWin().SketchControl;
                string filename = d.FileName;
                string ObjectName;
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                {
                    try {
                        System.Diagnostics.Debug.WriteLine("start loop");
                        while ((ObjectName = file.ReadLine()) != null) {
                            // Variabelen inlezen
                            string[] startPoint = file.ReadLine().Split(' ');
                            string[] endPoint = file.ReadLine().Split(' ');
                            string[] brush = file.ReadLine().Split(' ');
                            string text = file.ReadLine();
                            //string rotation = file.ReadLine();
                            Color col = Color.FromArgb(int.Parse(brush[0]), int.Parse(brush[1]), int.Parse(brush[2]));
                            Point p1 = new Point(int.Parse(startPoint[0]), int.Parse(startPoint[1]));
                            Point p2 = new Point(int.Parse(endPoint[0]), int.Parse(endPoint[1]));
                            //var s = sketchControl;

                            ISketchObject obj;
                            switch (ObjectName) // Maak object naar soort
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

                                case ("LineObject"):
                                    obj = new LineObject(s, p1, new SolidBrush(col));
                                    break;

                                case ("PenObject"):
                                    obj = new PenObject(s, p1, new SolidBrush(col));
                                    break;

                                case ("TextObject"):
                                    obj = new TextObject(s, p1, new SolidBrush(col));
                                    obj.AddText(text);
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

                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }

                }
                file.Close();
                s.Invalidate();
                s.Sketch.listChanged = false; //geeft aan dat veranderingen zijn opgeslagen
            }
        }

        private void LoadImage(object sender, EventArgs e)
        {
            SketchWin activeSketchWin = ActiveMdiChild as SketchWin;
            OpenFileDialog d = new OpenFileDialog(); //selecteer file
            // instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Laad afbeelding...";
            d.Filter = "Afbeeldingsbestanden (*.Bmp, .*Png, *.jpg) | *.Bmp; *.Png; *jpg";
            if (d.ShowDialog() == DialogResult.OK)
            {
                SketchControl s;
                if (activeSketchWin.SketchControl.Sketch.Objects.Count == 0)
                    s = activeSketchWin.SketchControl;
                else
                    s = ReturnNewSketchWin().SketchControl;
                string fileName = d.FileName;
                Image i = Image.FromFile(fileName);
                s.Sketch.ImageLoad = i;
                s.Sketch.listChanged = false;//geeft aan dat veranderingen zijn opgeslagen
                s.Sketch.Clear();
                s.Invalidate();
            }
        }

        private void Save(object sender, EventArgs e)
        {
            SketchWin sketchWin = ActiveMdiChild as SketchWin;
            if (sketchWin != null)
                sketchWin.SaveProject();
        }

        private void SaveAsImage(object sender, EventArgs e)
        {
            SketchWin sketchWin = ActiveMdiChild as SketchWin;
            if (sketchWin != null)
                sketchWin.ExportImage();
        }

        private void Exit(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Er zijn niet opgeslagen veranderingen. Weet je zeker dat je wil doorgaan?", "Alert", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }
            Close();
        }
    }
}
