using System.Drawing;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace Ephemera.MusicLib.Test
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            btnGo = new System.Windows.Forms.Button();
            txtViewer = new Ephemera.NBagOfUis.TextViewer();
            SuspendLayout();
            // 
            // btnGo
            // 
            btnGo.Location = new Point(228, 29);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(86, 48);
            btnGo.TabIndex = 0;
            btnGo.Text = "Go Go Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += Go_Click;
            // 
            // txtViewer
            // 
            txtViewer.Location = new Point(52, 134);
            txtViewer.Name = "txtViewer";
            txtViewer.Size = new System.Drawing.Size(753, 242);
            txtViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtViewer.MaxText = 5000;
            txtViewer.Prompt = "";
            txtViewer.TabIndex = 58;
            txtViewer.WordWrap = true;
            txtViewer.TabIndex = 2;
            // 
            // MainForm
            // 
            ClientSize = new Size(836, 499);
            Controls.Add(txtViewer);
            Controls.Add(btnGo);
            Name = "MainForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnGo;
        private NBagOfUis.TextViewer txtViewer;
    }
}
