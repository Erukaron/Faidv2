
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelektion)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSelektion
            // 
            this.dgvSelektion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSelektion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvSelektionAusschliessend,
            this.dgvSelektionTyp,
            this.dgvSelektionDaten});
            this.dgvSelektion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSelektion.Location = new System.Drawing.Point(0, 0);
            this.dgvSelektion.Name = "dgvSelektion";
            this.dgvSelektion.Size = new System.Drawing.Size(464, 181);
            this.dgvSelektion.TabIndex = 0;
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
            this.dgvSelektionTyp.Width = 39;
            // 
            // dgvSelektionDaten
            // 
            this.dgvSelektionDaten.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvSelektionDaten.HeaderText = "Wert";
            this.dgvSelektionDaten.Name = "dgvSelektionDaten";
            // 
            // Maske3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 181);
            this.Controls.Add(this.dgvSelektion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 220);
            this.Name = "Maske3";
            this.Text = "Selektion";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelektion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSelektion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvSelektionAusschliessend;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvSelektionTyp;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSelektionDaten;
    }
}