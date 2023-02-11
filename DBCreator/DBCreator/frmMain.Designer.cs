namespace DBCreator
{
    partial class frmMain
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
            this.btnAddItems = new System.Windows.Forms.Button();
            this.btnAddChampions = new System.Windows.Forms.Button();
            this.btnAddCoords = new System.Windows.Forms.Button();
            this.btnAddSourceRects = new System.Windows.Forms.Button();
            this.btnAddMouseTargets = new System.Windows.Forms.Button();
            this.lstTables = new System.Windows.Forms.ListBox();
            this.btnCreateWallDecorations = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.btnAddLevels = new System.Windows.Forms.Button();
            this.btnAddTiled = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEditorCoords = new System.Windows.Forms.Button();
            this.dgvDMData = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDMData)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddItems
            // 
            this.btnAddItems.Location = new System.Drawing.Point(235, 4);
            this.btnAddItems.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddItems.Name = "btnAddItems";
            this.btnAddItems.Size = new System.Drawing.Size(227, 63);
            this.btnAddItems.TabIndex = 0;
            this.btnAddItems.Text = "Add / Update Items Table";
            this.btnAddItems.UseVisualStyleBackColor = true;
            this.btnAddItems.Click += new System.EventHandler(this.btnAddItems_Click);
            // 
            // btnAddChampions
            // 
            this.btnAddChampions.Location = new System.Drawing.Point(235, 71);
            this.btnAddChampions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddChampions.Name = "btnAddChampions";
            this.btnAddChampions.Size = new System.Drawing.Size(227, 63);
            this.btnAddChampions.TabIndex = 1;
            this.btnAddChampions.Text = "Add / Update Champions Table";
            this.btnAddChampions.UseVisualStyleBackColor = true;
            this.btnAddChampions.Click += new System.EventHandler(this.btnAddChampions_Click);
            // 
            // btnAddCoords
            // 
            this.btnAddCoords.Location = new System.Drawing.Point(235, 138);
            this.btnAddCoords.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddCoords.Name = "btnAddCoords";
            this.btnAddCoords.Size = new System.Drawing.Size(227, 63);
            this.btnAddCoords.TabIndex = 2;
            this.btnAddCoords.Text = "Add / Update Coordinates Table";
            this.btnAddCoords.UseVisualStyleBackColor = true;
            this.btnAddCoords.Click += new System.EventHandler(this.btnAddCoords_Click);
            // 
            // btnAddSourceRects
            // 
            this.btnAddSourceRects.Location = new System.Drawing.Point(235, 209);
            this.btnAddSourceRects.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddSourceRects.Name = "btnAddSourceRects";
            this.btnAddSourceRects.Size = new System.Drawing.Size(227, 63);
            this.btnAddSourceRects.TabIndex = 3;
            this.btnAddSourceRects.Text = "Add / Update Source Rectangles";
            this.btnAddSourceRects.UseVisualStyleBackColor = true;
            this.btnAddSourceRects.Click += new System.EventHandler(this.btnAddSourceRects_Click);
            // 
            // btnAddMouseTargets
            // 
            this.btnAddMouseTargets.Location = new System.Drawing.Point(235, 280);
            this.btnAddMouseTargets.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddMouseTargets.Name = "btnAddMouseTargets";
            this.btnAddMouseTargets.Size = new System.Drawing.Size(227, 63);
            this.btnAddMouseTargets.TabIndex = 4;
            this.btnAddMouseTargets.Text = "Add / Update Mouse Targets Table";
            this.btnAddMouseTargets.UseVisualStyleBackColor = true;
            this.btnAddMouseTargets.Click += new System.EventHandler(this.btnAddMouseTargets_Click);
            // 
            // lstTables
            // 
            this.lstTables.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstTables.FormattingEnabled = true;
            this.lstTables.ItemHeight = 21;
            this.lstTables.Location = new System.Drawing.Point(2, 138);
            this.lstTables.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(227, 340);
            this.lstTables.TabIndex = 5;
            this.lstTables.Click += new System.EventHandler(this.lstTables_Click);
            // 
            // btnCreateWallDecorations
            // 
            this.btnCreateWallDecorations.Enabled = false;
            this.btnCreateWallDecorations.Location = new System.Drawing.Point(2, 493);
            this.btnCreateWallDecorations.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCreateWallDecorations.Name = "btnCreateWallDecorations";
            this.btnCreateWallDecorations.Size = new System.Drawing.Size(227, 63);
            this.btnCreateWallDecorations.TabIndex = 9;
            this.btnCreateWallDecorations.Text = "Create WallDecorations.coords";
            this.btnCreateWallDecorations.UseVisualStyleBackColor = true;
            this.btnCreateWallDecorations.Click += new System.EventHandler(this.btnCreateWallDecorations_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(3, 4);
            this.btnRebuild.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(227, 63);
            this.btnRebuild.TabIndex = 10;
            this.btnRebuild.Text = "Re-Create with all datafiles";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // btnAddLevels
            // 
            this.btnAddLevels.Location = new System.Drawing.Point(235, 351);
            this.btnAddLevels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddLevels.Name = "btnAddLevels";
            this.btnAddLevels.Size = new System.Drawing.Size(227, 63);
            this.btnAddLevels.TabIndex = 11;
            this.btnAddLevels.Text = "Add / Update Champion Levels";
            this.btnAddLevels.UseVisualStyleBackColor = true;
            this.btnAddLevels.Click += new System.EventHandler(this.btnAddLevels_Click);
            // 
            // btnAddTiled
            // 
            this.btnAddTiled.Location = new System.Drawing.Point(235, 422);
            this.btnAddTiled.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddTiled.Name = "btnAddTiled";
            this.btnAddTiled.Size = new System.Drawing.Size(227, 63);
            this.btnAddTiled.TabIndex = 12;
            this.btnAddTiled.Text = "Add / Update TiledToItem";
            this.btnAddTiled.UseVisualStyleBackColor = true;
            this.btnAddTiled.Click += new System.EventHandler(this.btnAddTiled_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(2, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 63);
            this.label1.TabIndex = 13;
            this.label1.Text = "Database Tables";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEditorCoords
            // 
            this.btnEditorCoords.Location = new System.Drawing.Point(233, 493);
            this.btnEditorCoords.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditorCoords.Name = "btnEditorCoords";
            this.btnEditorCoords.Size = new System.Drawing.Size(227, 63);
            this.btnEditorCoords.TabIndex = 14;
            this.btnEditorCoords.Text = "Add / Update  Editor Coordinates";
            this.btnEditorCoords.UseVisualStyleBackColor = true;
            this.btnEditorCoords.Click += new System.EventHandler(this.btnEditorCoords_Click);
            // 
            // dgvDMData
            // 
            this.dgvDMData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDMData.Location = new System.Drawing.Point(478, 6);
            this.dgvDMData.Name = "dgvDMData";
            this.dgvDMData.RowTemplate.Height = 25;
            this.dgvDMData.Size = new System.Drawing.Size(778, 559);
            this.dgvDMData.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lstTables);
            this.panel1.Controls.Add(this.btnAddItems);
            this.panel1.Controls.Add(this.btnEditorCoords);
            this.panel1.Controls.Add(this.btnAddChampions);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnAddCoords);
            this.panel1.Controls.Add(this.btnAddTiled);
            this.panel1.Controls.Add(this.btnAddSourceRects);
            this.panel1.Controls.Add(this.btnAddLevels);
            this.panel1.Controls.Add(this.btnAddMouseTargets);
            this.panel1.Controls.Add(this.btnRebuild);
            this.panel1.Controls.Add(this.btnCreateWallDecorations);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(469, 562);
            this.panel1.TabIndex = 16;
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1267, 579);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvDMData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "Database Creator";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDMData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnAddItems;
        private Button btnAddChampions;
        private Button btnAddCoords;
        private Button btnAddSourceRects;
        private Button btnAddMouseTargets;
        private ListBox lstTables;
        private Button btnCreateWallDecorations;
        private Button btnRebuild;
        private Button btnAddLevels;
        private Button btnAddTiled;
        private Label label1;
        private Button btnEditorCoords;
        private DataGridView dgvDMData;
        private Panel panel1;
    }
}