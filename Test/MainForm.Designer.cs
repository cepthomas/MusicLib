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
            btnOne = new System.Windows.Forms.Button();
            btnTwo = new System.Windows.Forms.Button();
            txtViewer = new Ephemera.NBagOfUis.TextViewer();
            SuspendLayout();
            // 
            // btnOne
            // 
            btnOne.Location = new Point(228, 29);
            btnOne.Name = "btnOne";
            btnOne.Size = new Size(86, 48);
            btnOne.TabIndex = 0;
            btnOne.Text = "1";
            btnOne.UseVisualStyleBackColor = true;
            btnOne.Click += One_Click;
            // 
            // btnTwo
            // 
            btnTwo.Location = new Point(334, 29);
            btnTwo.Name = "btnTwo";
            btnTwo.Size = new Size(86, 48);
            btnTwo.TabIndex = 1;
            btnTwo.Text = "2";
            btnTwo.UseVisualStyleBackColor = true;
            btnTwo.Click += Two_Click;
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
            Controls.Add(btnTwo);
            Controls.Add(btnOne);
            Name = "MainForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOne;
        private System.Windows.Forms.Button btnTwo;
        private NBagOfUis.TextViewer txtViewer;
    }
}
