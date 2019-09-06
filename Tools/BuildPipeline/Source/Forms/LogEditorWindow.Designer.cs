namespace BuildPipeline.Forms
{
	partial class LogEditorWindow
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
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.closeBtn = new System.Windows.Forms.Button();
			this.saveBtn = new System.Windows.Forms.Button();
			this.changeset = new System.Windows.Forms.TextBox();
			this.versionDisplay = new System.Windows.Forms.Label();
			this.versionLabel = new System.Windows.Forms.Label();
			this.header = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			this.groupBox.Anchor =
				((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
														System.Windows.Forms.AnchorStyles.Bottom) |
														System.Windows.Forms.AnchorStyles.Left) |
														System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox.Controls.Add(this.closeBtn);
			this.groupBox.Controls.Add(this.saveBtn);
			this.groupBox.Controls.Add(this.changeset);
			this.groupBox.Controls.Add(this.versionDisplay);
			this.groupBox.Controls.Add(this.versionLabel);
			this.groupBox.Controls.Add(this.header);
			this.groupBox.Location = new System.Drawing.Point(7, 12);
			this.groupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox.Name = "groupBox";
			this.groupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox.Size = new System.Drawing.Size(764, 438);
			this.groupBox.TabIndex = 0;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Changeset Editor";
			this.closeBtn.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom |
														System.Windows.Forms.AnchorStyles.Left) |
														System.Windows.Forms.AnchorStyles.Right)));
			this.closeBtn.Location = new System.Drawing.Point(383, 357);
			this.closeBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(130, 75);
			this.closeBtn.TabIndex = 5;
			this.closeBtn.Text = "close";
			this.closeBtn.UseVisualStyleBackColor = true;
			this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
			this.saveBtn.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom |
														System.Windows.Forms.AnchorStyles.Left) |
														System.Windows.Forms.AnchorStyles.Right)));
			this.saveBtn.Location = new System.Drawing.Point(209, 357);
			this.saveBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.saveBtn.Name = "saveBtn";
			this.saveBtn.Size = new System.Drawing.Size(130, 75);
			this.saveBtn.TabIndex = 4;
			this.saveBtn.Text = "save";
			this.saveBtn.UseVisualStyleBackColor = true;
			this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
			this.changeset.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left |
														System.Windows.Forms.AnchorStyles.Right)));
			this.changeset.Location = new System.Drawing.Point(8, 78);
			this.changeset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.changeset.Multiline = true;
			this.changeset.Name = "changeset";
			this.changeset.Size = new System.Drawing.Size(748, 273);
			this.changeset.TabIndex = 3;
			this.versionDisplay.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
														System.Windows.Forms.AnchorStyles.Left) |
														System.Windows.Forms.AnchorStyles.Right)));
			this.versionDisplay.Location = new System.Drawing.Point(607, 44);
			this.versionDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.versionDisplay.Name = "versionDisplay";
			this.versionDisplay.Size = new System.Drawing.Size(149, 27);
			this.versionDisplay.TabIndex = 2;
			this.versionDisplay.Text = "...";
			this.versionDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.versionLabel.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
														System.Windows.Forms.AnchorStyles.Left) |
														System.Windows.Forms.AnchorStyles.Right)));
			this.versionLabel.Location = new System.Drawing.Point(546, 44);
			this.versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(52, 27);
			this.versionLabel.TabIndex = 1;
			this.versionLabel.Text = "Version :";
			this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.header.Location = new System.Drawing.Point(8, 44);
			this.header.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.header.Name = "header";
			this.header.Size = new System.Drawing.Size(122, 29);
			this.header.TabIndex = 0;
			this.header.Text = "changeset :";
			this.header.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 462);
			this.Controls.Add(this.groupBox);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "LogEditorWindow";
			this.Text = "ChangesetEditor";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Label versionDisplay;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label header;
		private System.Windows.Forms.TextBox changeset;
		private System.Windows.Forms.Button saveBtn;
		private System.Windows.Forms.Button closeBtn;
		private System.Windows.Forms.GroupBox groupBox;
	}
}