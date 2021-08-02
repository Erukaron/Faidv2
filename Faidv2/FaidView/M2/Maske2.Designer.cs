
namespace Faidv2.FaidView.M2
{
    partial class Maske2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Maske2));
            this.comboModus = new System.Windows.Forms.ComboBox();
            this.comboBewegung = new System.Windows.Forms.ComboBox();
            this.labelDatum = new System.Windows.Forms.Label();
            this.dateAnlage = new System.Windows.Forms.DateTimePicker();
            this.dateDatum = new System.Windows.Forms.DateTimePicker();
            this.labelWert = new System.Windows.Forms.Label();
            this.labelKommentar = new System.Windows.Forms.Label();
            this.richTextBoxKommentar = new System.Windows.Forms.RichTextBox();
            this.buttonAbbrechen = new System.Windows.Forms.Button();
            this.buttonSpeichern = new System.Windows.Forms.Button();
            this.labelErstellt = new System.Windows.Forms.Label();
            this.labelEuro = new System.Windows.Forms.Label();
            this.textBoxWert = new CustomControls.DecimalTextBox();
            this.comboBoxPeriode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboModus
            // 
            this.comboModus.FormattingEnabled = true;
            this.comboModus.Location = new System.Drawing.Point(82, 12);
            this.comboModus.Name = "comboModus";
            this.comboModus.Size = new System.Drawing.Size(200, 21);
            this.comboModus.TabIndex = 0;
            // 
            // comboBewegung
            // 
            this.comboBewegung.Enabled = false;
            this.comboBewegung.FormattingEnabled = true;
            this.comboBewegung.Location = new System.Drawing.Point(82, 39);
            this.comboBewegung.Name = "comboBewegung";
            this.comboBewegung.Size = new System.Drawing.Size(200, 21);
            this.comboBewegung.TabIndex = 1;
            this.comboBewegung.Visible = false;
            // 
            // labelDatum
            // 
            this.labelDatum.AutoSize = true;
            this.labelDatum.Location = new System.Drawing.Point(13, 98);
            this.labelDatum.Name = "labelDatum";
            this.labelDatum.Size = new System.Drawing.Size(41, 13);
            this.labelDatum.TabIndex = 2;
            this.labelDatum.Text = "Datum:";
            // 
            // dateAnlage
            // 
            this.dateAnlage.Enabled = false;
            this.dateAnlage.Location = new System.Drawing.Point(82, 66);
            this.dateAnlage.Name = "dateAnlage";
            this.dateAnlage.Size = new System.Drawing.Size(200, 20);
            this.dateAnlage.TabIndex = 3;
            // 
            // dateDatum
            // 
            this.dateDatum.Location = new System.Drawing.Point(82, 92);
            this.dateDatum.Name = "dateDatum";
            this.dateDatum.Size = new System.Drawing.Size(200, 20);
            this.dateDatum.TabIndex = 4;
            // 
            // labelWert
            // 
            this.labelWert.AutoSize = true;
            this.labelWert.Location = new System.Drawing.Point(13, 119);
            this.labelWert.Name = "labelWert";
            this.labelWert.Size = new System.Drawing.Size(33, 13);
            this.labelWert.TabIndex = 5;
            this.labelWert.Text = "Wert:";
            // 
            // labelKommentar
            // 
            this.labelKommentar.AutoSize = true;
            this.labelKommentar.Location = new System.Drawing.Point(13, 145);
            this.labelKommentar.Name = "labelKommentar";
            this.labelKommentar.Size = new System.Drawing.Size(63, 13);
            this.labelKommentar.TabIndex = 7;
            this.labelKommentar.Text = "Kommentar:";
            // 
            // richTextBoxKommentar
            // 
            this.richTextBoxKommentar.Location = new System.Drawing.Point(82, 142);
            this.richTextBoxKommentar.Name = "richTextBoxKommentar";
            this.richTextBoxKommentar.Size = new System.Drawing.Size(200, 46);
            this.richTextBoxKommentar.TabIndex = 6;
            this.richTextBoxKommentar.Text = "";
            // 
            // buttonAbbrechen
            // 
            this.buttonAbbrechen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAbbrechen.Location = new System.Drawing.Point(186, 194);
            this.buttonAbbrechen.Name = "buttonAbbrechen";
            this.buttonAbbrechen.Size = new System.Drawing.Size(96, 23);
            this.buttonAbbrechen.TabIndex = 8;
            this.buttonAbbrechen.Text = "Abbrechen (F6)";
            this.buttonAbbrechen.UseVisualStyleBackColor = true;
            this.buttonAbbrechen.Click += new System.EventHandler(this.buttonAbbrechen_Click);
            // 
            // buttonSpeichern
            // 
            this.buttonSpeichern.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSpeichern.Location = new System.Drawing.Point(82, 194);
            this.buttonSpeichern.Name = "buttonSpeichern";
            this.buttonSpeichern.Size = new System.Drawing.Size(96, 23);
            this.buttonSpeichern.TabIndex = 7;
            this.buttonSpeichern.Text = "Speichern (F2)";
            this.buttonSpeichern.UseVisualStyleBackColor = true;
            this.buttonSpeichern.Click += new System.EventHandler(this.buttonSpeichern_Click);
            // 
            // labelErstellt
            // 
            this.labelErstellt.AutoSize = true;
            this.labelErstellt.Location = new System.Drawing.Point(13, 72);
            this.labelErstellt.Name = "labelErstellt";
            this.labelErstellt.Size = new System.Drawing.Size(41, 13);
            this.labelErstellt.TabIndex = 11;
            this.labelErstellt.Text = "Erstellt:";
            // 
            // labelEuro
            // 
            this.labelEuro.AutoSize = true;
            this.labelEuro.Location = new System.Drawing.Point(190, 119);
            this.labelEuro.Name = "labelEuro";
            this.labelEuro.Size = new System.Drawing.Size(13, 13);
            this.labelEuro.TabIndex = 12;
            this.labelEuro.Text = "€";
            // 
            // textBoxWert
            // 
            this.textBoxWert.Location = new System.Drawing.Point(82, 116);
            this.textBoxWert.Name = "textBoxWert";
            this.textBoxWert.Size = new System.Drawing.Size(100, 20);
            this.textBoxWert.TabIndex = 5;
            this.textBoxWert.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBoxPeriode
            // 
            this.comboBoxPeriode.FormattingEnabled = true;
            this.comboBoxPeriode.Location = new System.Drawing.Point(82, 39);
            this.comboBoxPeriode.Name = "comboBoxPeriode";
            this.comboBoxPeriode.Size = new System.Drawing.Size(200, 21);
            this.comboBoxPeriode.TabIndex = 2;
            // 
            // Maske2
            // 
            this.AcceptButton = this.buttonSpeichern;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAbbrechen;
            this.ClientSize = new System.Drawing.Size(290, 225);
            this.Controls.Add(this.comboBoxPeriode);
            this.Controls.Add(this.textBoxWert);
            this.Controls.Add(this.labelEuro);
            this.Controls.Add(this.labelErstellt);
            this.Controls.Add(this.buttonSpeichern);
            this.Controls.Add(this.buttonAbbrechen);
            this.Controls.Add(this.richTextBoxKommentar);
            this.Controls.Add(this.labelKommentar);
            this.Controls.Add(this.labelWert);
            this.Controls.Add(this.dateDatum);
            this.Controls.Add(this.dateAnlage);
            this.Controls.Add(this.labelDatum);
            this.Controls.Add(this.comboBewegung);
            this.Controls.Add(this.comboModus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(306, 264);
            this.MinimumSize = new System.Drawing.Size(306, 264);
            this.Name = "Maske2";
            this.Text = "Erfassung";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboModus;
        private System.Windows.Forms.ComboBox comboBewegung;
        private System.Windows.Forms.Label labelDatum;
        private System.Windows.Forms.DateTimePicker dateAnlage;
        private System.Windows.Forms.DateTimePicker dateDatum;
        private System.Windows.Forms.Label labelWert;
        private System.Windows.Forms.Label labelKommentar;
        private System.Windows.Forms.RichTextBox richTextBoxKommentar;
        private System.Windows.Forms.Button buttonAbbrechen;
        private System.Windows.Forms.Button buttonSpeichern;
        private System.Windows.Forms.Label labelErstellt;
        private System.Windows.Forms.Label labelEuro;
        private CustomControls.DecimalTextBox textBoxWert;
        private System.Windows.Forms.ComboBox comboBoxPeriode;
    }
}