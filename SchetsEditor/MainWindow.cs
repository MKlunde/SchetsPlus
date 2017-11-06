using System;
using System.Drawing;
using System.Windows.Forms;

namespace SketchEditor
{
    public class MainWindow : Form
    {
        public MenuStrip menuStrip;
        public ToolStripDropDownItem fileMenu, helpMenu;

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
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Sluiten", null, CloseActiveSketchWin) { Enabled = false });
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("SketchPlus Afsluiten", null, Exit));
            Console.WriteLine(fileMenu.DropDownItems.Count);

            // Maak dropdown-menu "Help"
            helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add(new ToolStripMenuItem("Over \"Schets\"", null, About));

            menuStrip.Items.AddRange(new ToolStripDropDownItem[] { fileMenu, helpMenu });

            SketchWinMenuItems();
        }
        public void SketchWinMenuItems() {
            bool atLeastOneWindow = MdiChildren.Length > 0;
            GetDropDownItemFromMenu(fileMenu, "Sluiten").Enabled = atLeastOneWindow;
        }
        public ToolStripMenuItem GetDropDownItemFromMenu(ToolStripDropDownItem menu, string text) {
            foreach (ToolStripMenuItem item in menu.DropDownItems) {
                Console.WriteLine(item.Text);
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
        private void Exit(object sender, EventArgs e) {
            Close();
        }
    }
}
