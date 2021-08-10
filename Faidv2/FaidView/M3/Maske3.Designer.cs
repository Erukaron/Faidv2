
namespace Faidv2.FaidView.M3
{
    partial class Maske3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Maske3));
            this.dgvSelektion = new System.Windows.Forms.DataGridView();
            this.dgvSelektionAusschliessend = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvSelektionTyp = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvSelektionDaten = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.übernehmenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loeschenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neueWertselektionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neueZeichenselektionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelektion)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSelektion
            // 
            this.dgvSelektion.AllowUserToAddRows = false;
            this.dgvSelektion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSelektion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvSelektionAusschliessend,
            this.dgvSelektionTyp,
            this.dgvSelektionDaten});
            this.dgvSelektion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSelektion.Location = new System.Drawing.Point(0, 24);
            this.dgvSelektion.Name = "dgvSelektion";
            this.dgvSelektion.Size = new System.Drawing.Size(464, 157);
            this.dgvSelektion.TabIndex = 0;
            this.dgvSelektion.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSelektion_CellValidated);
            this.dgvSelektion.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvSelektion_CellValidating);
            this.dgvSelektion.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvSelektion_RowsAdded);
            // 
            // dgvSelektionAusschliessend
            // 
            this.dgvSelektionAusschliessend.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvSelektionAusschliessend.HeaderText = "Ausschließend";
            this.dgvSelektionAusschliessend.Name = "dgvSelektionAusschliessend";
            this.dgvSelektionAusschliessend.Width = 82;
            // 
            // dgvSelektionTyp
            // 
            this.dgvSelektionTyp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvSelektionTyp.HeaderText = "Typ";
            this.dgvSelektionTyp.Name = "dgvSelektionTyp";
            this.dgvSelektionTyp.Width = 31;
            // 
            // dgvSelektionDaten
            // 
            this.dgvSelektionDaten.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvSelektionDaten.HeaderText = "Wert";
            this.dgvSelektionDaten.Name = "dgvSelektionDaten";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.übernehmenToolStripMenuItem,
            this.loeschenToolStripMenuItem,
            this.neueWertselektionToolStripMenuItem,
            this.neueZeichenselektionToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(464, 24);
            this.menuStrip.TabIndex = 1;
            // 
            // übernehmenToolStripMenuItem
            // 
            this.übernehmenToolStripMenuItem.Name = "übernehmenToolStripMenuItem";
            this.übernehmenToolStripMenuItem.ShortcutKeyDisplayString = "F2";
            this.übernehmenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.übernehmenToolStripMenuItem.Size = new System.Drawing.Size(111, 20);
            this.übernehmenToolStripMenuItem.Text = "Übernehmen (F2)";
            this.übernehmenToolStripMenuItem.Click += new System.EventHandler(this.übernehmenToolStripMenuItem_Click);
            // 
            // loeschenToolStripMenuItem
            // 
            this.loeschenToolStripMenuItem.Name = "loeschenToolStripMenuItem";
            this.loeschenToolStripMenuItem.ShortcutKeyDisplayString = "F4";
            this.loeschenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.loeschenToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.loeschenToolStripMenuItem.Text = "Löschen (F4)";
            this.loeschenToolStripMenuItem.Click += new System.EventHandler(this.loeschenToolStripMenuItem_Click);
            // 
            // neueWertselektionToolStripMenuItem
            // 
            this.neueWertselektionToolStripMenuItem.Name = "neueWertselektionToolStripMenuItem";
            this.neueWertselektionToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.neueWertselektionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.neueWertselektionToolStripMenuItem.Size = new System.Drawing.Size(114, 20);
            this.neueWertselektionToolStripMenuItem.Text = "Wertselektion (F5)";
            this.neueWertselektionToolStripMenuItem.Click += new System.EventHandler(this.neueWertselektionToolStripMenuItem_Click);
            // 
            // neueZeichenselektionToolStripMenuItem
            // 
            this.neueZeichenselektionToolStripMenuItem.Name = "neueZeichenselektionToolStripMenuItem";
            this.neueZeichenselektionToolStripMenuItem.ShortcutKeyDisplayString = "F6";
            this.neueZeichenselektionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.neueZeichenselektionToolStripMenuItem.Size = new System.Drawing.Size(131, 20);
            this.neueZeichenselektionToolStripMenuItem.Text = "Zeichenselektion (F6)";
            this.neueZeichenselektionToolStripMenuItem.Click += new System.EventHandler(this.neueZeichenselektionToolStripMenuItem_Click);
            // 
            // Maske3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 181);
            this.Controls.Add(this.dgvSelektion);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 220);
            this.Name = "Maske3";
            this.Text = "Selektion";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelektion)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSelektion;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem übernehmenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loeschenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neueWertselektionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neueZeichenselektionToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvSelektionAusschliessend;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvSelektionTyp;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSelektionDaten;
    }
}