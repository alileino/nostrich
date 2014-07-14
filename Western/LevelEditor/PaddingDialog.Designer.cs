namespace LevelEditor
{
    partial class PaddingDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._leftTextBox = new System.Windows.Forms.TextBox();
            this._rightTextBox = new System.Windows.Forms.TextBox();
            this._topTextBox = new System.Windows.Forms.TextBox();
            this._bottomTextBox = new System.Windows.Forms.TextBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Bottom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Top";
            // 
            // _leftTextBox
            // 
            this._leftTextBox.Location = new System.Drawing.Point(54, 47);
            this._leftTextBox.Name = "_leftTextBox";
            this._leftTextBox.Size = new System.Drawing.Size(78, 20);
            this._leftTextBox.TabIndex = 4;
            this._leftTextBox.Text = "0";
            // 
            // _rightTextBox
            // 
            this._rightTextBox.Location = new System.Drawing.Point(138, 47);
            this._rightTextBox.Name = "_rightTextBox";
            this._rightTextBox.Size = new System.Drawing.Size(82, 20);
            this._rightTextBox.TabIndex = 5;
            this._rightTextBox.Text = "0";
            // 
            // _topTextBox
            // 
            this._topTextBox.Location = new System.Drawing.Point(90, 22);
            this._topTextBox.Name = "_topTextBox";
            this._topTextBox.Size = new System.Drawing.Size(93, 20);
            this._topTextBox.TabIndex = 6;
            this._topTextBox.Text = "0";
            // 
            // _bottomTextBox
            // 
            this._bottomTextBox.Location = new System.Drawing.Point(90, 73);
            this._bottomTextBox.Name = "_bottomTextBox";
            this._bottomTextBox.Size = new System.Drawing.Size(93, 20);
            this._bottomTextBox.TabIndex = 7;
            this._bottomTextBox.Text = "0";
            // 
            // _okButton
            // 
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(183, 113);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 8;
            this._okButton.Text = "Confirm";
            this._okButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(102, 113);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // PaddingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 141);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._bottomTextBox);
            this.Controls.Add(this._topTextBox);
            this.Controls.Add(this._rightTextBox);
            this.Controls.Add(this._leftTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PaddingDialog";
            this.Text = "PaddingDialog";
            this.Load += new System.EventHandler(this.PaddingDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _leftTextBox;
        private System.Windows.Forms.TextBox _rightTextBox;
        private System.Windows.Forms.TextBox _topTextBox;
        private System.Windows.Forms.TextBox _bottomTextBox;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}