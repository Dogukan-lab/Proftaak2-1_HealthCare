namespace SimulatorGui
{
    partial class SimForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.speedBox = new System.Windows.Forms.TextBox();
            this.speedSwayCheck = new System.Windows.Forms.CheckBox();
            this.speedSwayBox = new System.Windows.Forms.TextBox();
            this.heartRateSway = new System.Windows.Forms.TextBox();
            this.heartRateSwayCheck = new System.Windows.Forms.CheckBox();
            this.heartRateBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.resistanceBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(223, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 71);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bike Simulator";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(17, 135);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 48);
            this.label2.TabIndex = 1;
            this.label2.Text = "Speed";
            // 
            // speedBox
            // 
            this.speedBox.Location = new System.Drawing.Point(17, 193);
            this.speedBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.speedBox.Name = "speedBox";
            this.speedBox.Size = new System.Drawing.Size(141, 31);
            this.speedBox.TabIndex = 2;
            // 
            // speedSwayCheck
            // 
            this.speedSwayCheck.AutoSize = true;
            this.speedSwayCheck.Location = new System.Drawing.Point(467, 197);
            this.speedSwayCheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.speedSwayCheck.Name = "speedSwayCheck";
            this.speedSwayCheck.Size = new System.Drawing.Size(77, 29);
            this.speedSwayCheck.TabIndex = 3;
            this.speedSwayCheck.Text = "sway";
            this.speedSwayCheck.UseVisualStyleBackColor = true;
            // 
            // speedSwayBox
            // 
            this.speedSwayBox.Location = new System.Drawing.Point(591, 190);
            this.speedSwayBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.speedSwayBox.Name = "speedSwayBox";
            this.speedSwayBox.Size = new System.Drawing.Size(141, 31);
            this.speedSwayBox.TabIndex = 4;
            // 
            // heartRateSway
            // 
            this.heartRateSway.Location = new System.Drawing.Point(591, 412);
            this.heartRateSway.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.heartRateSway.Name = "heartRateSway";
            this.heartRateSway.Size = new System.Drawing.Size(141, 31);
            this.heartRateSway.TabIndex = 4;
            // 
            // heartRateSwayCheck
            // 
            this.heartRateSwayCheck.AutoSize = true;
            this.heartRateSwayCheck.Location = new System.Drawing.Point(467, 418);
            this.heartRateSwayCheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.heartRateSwayCheck.Name = "heartRateSwayCheck";
            this.heartRateSwayCheck.Size = new System.Drawing.Size(77, 29);
            this.heartRateSwayCheck.TabIndex = 3;
            this.heartRateSwayCheck.Text = "sway";
            this.heartRateSwayCheck.UseVisualStyleBackColor = true;
            // 
            // heartRateBox
            // 
            this.heartRateBox.Location = new System.Drawing.Point(17, 415);
            this.heartRateBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.heartRateBox.Name = "heartRateBox";
            this.heartRateBox.Size = new System.Drawing.Size(141, 31);
            this.heartRateBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(17, 357);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 48);
            this.label3.TabIndex = 1;
            this.label3.Text = "Heart rate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(11, 565);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 48);
            this.label4.TabIndex = 1;
            this.label4.Text = "Resistance";
            // 
            // resistanceBox
            // 
            this.resistanceBox.Location = new System.Drawing.Point(11, 623);
            this.resistanceBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resistanceBox.Name = "resistanceBox";
            this.resistanceBox.ReadOnly = true;
            this.resistanceBox.Size = new System.Drawing.Size(141, 31);
            this.resistanceBox.TabIndex = 2;
            // 
            // SimForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 723);
            this.Controls.Add(this.resistanceBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.heartRateBox);
            this.Controls.Add(this.heartRateSwayCheck);
            this.Controls.Add(this.heartRateSway);
            this.Controls.Add(this.speedSwayBox);
            this.Controls.Add(this.speedSwayCheck);
            this.Controls.Add(this.speedBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SimForm";
            this.Text = "Bike Simulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox speedBox;
        private System.Windows.Forms.CheckBox speedSwayCheck;
        private System.Windows.Forms.TextBox speedSwayBox;
        private System.Windows.Forms.TextBox heartRateSway;
        private System.Windows.Forms.CheckBox heartRateSwayCheck;
        private System.Windows.Forms.TextBox heartRateBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox resistanceBox;
    }
}

