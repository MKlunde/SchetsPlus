using System;
using System.Drawing;
using System.Windows.Forms;

namespace SketchEditor
{
    public class MainWindow : Form
    {
        public MenuStrip menuStrip;

        public MainWindow(){
            this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.CreateFileMenu();
            this.CreateHelpMenu();
            this.Text = "Schetseditor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
            this.NewSketchWin(); // Open lege SketchWin bij opstarten
        }
        private void CreateFileMenu() {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Bestand");
            menu.DropDownItems.Add("Nieuw", null, this.ManualNewSketchWin);
            menu.DropDownItems.Add("Afsluiten", null, this.Exit);
            menuStrip.Items.Add(menu);
        }
        private void CreateHelpMenu() {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.About);
            menuStrip.Items.Add(menu);
        }
        private void About(object o, EventArgs ea) {
            MessageBox.Show("SchetsPlus versie 0.1\nDoor Menno Klunder en Mats Gottenbos\nGebaseerd op Schets versie 1.0 - (c) UU Informatica 2010",
                "Over \"Schets\"",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void NewSketchWin() {
            SketchWin s = new SketchWin(this);
            s.MdiParent = this;
            s.Show();
        }
        private void ManualNewSketchWin(object sender, EventArgs e) {
            this.NewSketchWin();
        }
        private void Exit(object sender, EventArgs e) {
            this.Close();
        }
    }
}
