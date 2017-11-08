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
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Nieuw", null, NewSketchWin));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Project laden...", null, LoadProject));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Afbeelding laden...", null, LoadImage));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Opslaan als project...", null, Save));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Opslaan als afbeelding...", null, SaveAsImage));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Sluiten", null, CloseActiveSketchWin) { Enabled = false });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("SketchPlus Afsluiten", null, Exit));

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

        private void NewSketchWin(object sender = null, EventArgs e = null) {
            SketchWin s = new SketchWin(this);
            s.MdiParent = this;
            s.Show();
            SketchWinMenuItems();
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
            SketchWin s = ActiveMdiChild as SketchWin;
            s.LoadProject();
        }
        private void LoadImage(object sender, EventArgs e)
        {
            SketchWin s = ActiveMdiChild as SketchWin;
            s.LoadImage();
        }

        private void Save(object sender, EventArgs e)
        {
            SketchWin s = ActiveMdiChild as SketchWin;
            s.Store();
        }

        private void SaveAsImage(object sender, EventArgs e)
        {
            SketchWin s = ActiveMdiChild as SketchWin;
            s.StoreImage();
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
