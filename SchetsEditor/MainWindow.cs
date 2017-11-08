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
            Text = "SchetsPlus";
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
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Project openen...", null, LoadProject) { ShortcutKeyDisplayString = "Ctrl+O", ShortcutKeys = Keys.Control | Keys.O });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Afbeelding importeren...", null, LoadImage) { ShortcutKeyDisplayString = "Ctrl+I", ShortcutKeys = Keys.Control | Keys.I });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Opslaan als project...", null, Save) { Enabled = false, ShortcutKeyDisplayString = "Ctrl+S", ShortcutKeys = Keys.Control | Keys.S });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Exporteren als afbeelding...", null, SaveAsImage) { Enabled = false, ShortcutKeyDisplayString = "Ctrl+E", ShortcutKeys = Keys.Control | Keys.E });
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
        private void LoadProject(object sender, EventArgs e) // Laad projectbestand
        {
            SketchWin activeSketchWin = ActiveMdiChild as SketchWin;
            OpenFileDialog d = new OpenFileDialog(); // Selecteer file
            // Instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Project openen";
            d.Filter = "Sketch-bestanden (*.sketch)|*.sketch";
            if (d.ShowDialog() == DialogResult.OK) {
                SketchControl s;
                if (activeSketchWin == null || activeSketchWin.SketchControl.Sketch.Objects.Count > 0)
                    s = ReturnNewSketchWin().SketchControl;
                else
                    s = activeSketchWin.SketchControl;
                string filename = d.FileName;
                string objectName;
                string lineString;
                StreamReader file = new System.IO.StreamReader(filename);
                {
                    try {
                        while ((lineString = file.ReadLine()) != null) {
                            // Variabelen inlezen
                            string[] line = lineString.Split(' ');
                            objectName = line[0];
                            Point p1 = new Point(int.Parse(line[1]), int.Parse(line[2]));
                            Point p2 = new Point(int.Parse(line[3]), int.Parse(line[4]));
                            Color col = Color.FromArgb(int.Parse(line[5]), int.Parse(line[6]), int.Parse(line[7]));
                            int w = int.Parse(line[8]);
                            string text = line[9];
                            int rotation = int.Parse(line[10]);

                            ISketchObject obj;
                            switch (objectName) // Maak object naar soort
                            {
                                case ("FilledEllipseObject"):
                                    obj = new FilledEllipseObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("EllipseObject"):
                                    obj = new EllipseObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("RectangleObject"):
                                    obj = new RectangleObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("FilledRectangleObject"):
                                    obj = new FilledRectangleObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("LineObject"):
                                    obj = new LineObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("PenObject"):
                                    obj = new PenObject(s, p1, new SolidBrush(col), w);
                                    break;
                                case ("TextObject"):
                                    obj = new TextObject(s, p1, new SolidBrush(col), w);
                                    obj.AddText(text);
                                    break;
                                default:
                                    obj = null;
                                    Console.WriteLine("Fout bij laden object"); // Als er iets mis gaat bij het laden, schrijf dan een melding hiervan in de console
                                    break;
                            }
                            s.SketchAddObject(obj); // Voeg object toe aan object list
                            obj.ChangeEndingPoint(p2); // Zet endingPoint goed
                            obj.Finish(); // Zet finished op true (de gebruiker is het object immers niet meer aan het tekenen)
                        }
                    }

                    catch (Exception ex) {
                        Console.WriteLine("Fout bij laden object"); // Als er iets mis gaat bij het laden, schrijf dan een melding hiervan in de console
                    }

                }
                file.Close();
                s.Invalidate();
                s.Sketch.listChanged = false; // Geef aan dat er geen nieuwe wijzigingen zijn aan de sketch
            }
        }

        private void LoadImage(object sender, EventArgs e) // Laad afbeeldingsbestand
        {
            SketchWin activeSketchWin = ActiveMdiChild as SketchWin;
            OpenFileDialog d = new OpenFileDialog(); // Selecteer file
            // Instellingen dialog
            d.InitialDirectory = "./";
            d.Title = "Afbeelding importeren";
            d.Filter = "Afbeeldingsbestanden (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            if (d.ShowDialog() == DialogResult.OK) {
                SketchControl s;
                if(activeSketchWin == null || activeSketchWin.SketchControl.Sketch.Objects.Count > 0)
                    s = ReturnNewSketchWin().SketchControl;
                else
                    s = activeSketchWin.SketchControl;
                string fileName = d.FileName;
                Image i = Image.FromFile(fileName);
                s.Sketch.Resize(new Size(i.Width, i.Height));
                s.Sketch.ImageLoad = i;
                Bitmap b = new Bitmap(i);
                s.Sketch.listChanged = true; // Geef aan dat er wel nieuwe wijzigingen zijn aan de sketch
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
