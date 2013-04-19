namespace AweEditor
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importVoxelTerrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTerrianModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnRemove = new System.Windows.Forms.Button();
            this.manifestViewer = new System.Windows.Forms.TreeView();
            this.editorViewerControl = new AweEditor.EditorViewerControl();
            this.ttcControlPanel = new AweEditor.Controls.TablessTabControl();
            this.tpNoControls = new System.Windows.Forms.TabPage();
            this.tpTerrainControls = new System.Windows.Forms.TabPage();
            this.btnToggle = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numCamRoll = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numCamPitch = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numCamYaw = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numCamZ = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numCamY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numCamX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tpModelControls = new System.Windows.Forms.TabPage();
            this.tpTextureControls = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.mapTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ttcControlPanel.SuspendLayout();
            this.tpTerrainControls.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCamRoll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamYaw)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCamZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamX)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.importVoxelTerrainToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(190, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.OpenMenuClicked);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveMenuClicked);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsMenuClicked);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(187, 6);
            // 
            // importVoxelTerrainToolStripMenuItem
            // 
            this.importVoxelTerrainToolStripMenuItem.Name = "importVoxelTerrainToolStripMenuItem";
            this.importVoxelTerrainToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.importVoxelTerrainToolStripMenuItem.Text = "Import Voxel Terrain...";
            this.importVoxelTerrainToolStripMenuItem.Click += new System.EventHandler(this.ImportVoxelTerrainMenuClicked);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openToolStripMenuItem.Text = "Import Model...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.ImportModelMenuClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(190, 22);
            this.toolStripMenuItem1.Text = "Import Texture...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ImportTextureClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuClicked);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createTerrianModelToolStripMenuItem,
            this.mapTexturesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // createTerrianModelToolStripMenuItem
            // 
            this.createTerrianModelToolStripMenuItem.Enabled = false;
            this.createTerrianModelToolStripMenuItem.Name = "createTerrianModelToolStripMenuItem";
            this.createTerrianModelToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.createTerrianModelToolStripMenuItem.Text = "Create Terrian Model";
            this.createTerrianModelToolStripMenuItem.Click += new System.EventHandler(this.CreateMeshMenuItemClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 549);
            this.panel1.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnRemove);
            this.splitContainer1.Panel1.Controls.Add(this.manifestViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.editorViewerControl);
            this.splitContainer1.Panel2.Controls.Add(this.ttcControlPanel);
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(792, 549);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.TabIndex = 8;
            // 
            // btnRemove
            // 
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRemove.Location = new System.Drawing.Point(0, 516);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(141, 33);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove Resource";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // manifestViewer
            // 
            this.manifestViewer.Dock = System.Windows.Forms.DockStyle.Top;
            this.manifestViewer.Location = new System.Drawing.Point(0, 0);
            this.manifestViewer.Name = "manifestViewer";
            this.manifestViewer.Size = new System.Drawing.Size(141, 510);
            this.manifestViewer.TabIndex = 0;
            this.manifestViewer.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.manifestViewer_BeforeSelect);
            // 
            // editorViewerControl
            // 
            this.editorViewerControl.CamPitch = 0F;
            this.editorViewerControl.CamPosition = new Microsoft.Xna.Framework.Vector3(0F, 0F, 0F);
            this.editorViewerControl.CamRoll = 0F;
            this.editorViewerControl.CamYaw = 0F;
            this.editorViewerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.editorViewerControl.Location = new System.Drawing.Point(0, 0);
            this.editorViewerControl.Margin = new System.Windows.Forms.Padding(2);
            this.editorViewerControl.Model = null;
            this.editorViewerControl.Name = "editorViewerControl";
            this.editorViewerControl.Paused = false;
            this.editorViewerControl.Size = new System.Drawing.Size(647, 455);
            this.editorViewerControl.TabIndex = 5;
            this.editorViewerControl.TerrianModel = null;
            this.editorViewerControl.Text = "editorViewerControl";
            this.editorViewerControl.Texture = null;
            this.editorViewerControl.VoxelTerrain = null;
            // 
            // ttcControlPanel
            // 
            this.ttcControlPanel.Controls.Add(this.tpNoControls);
            this.ttcControlPanel.Controls.Add(this.tpTerrainControls);
            this.ttcControlPanel.Controls.Add(this.tpModelControls);
            this.ttcControlPanel.Controls.Add(this.tpTextureControls);
            this.ttcControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ttcControlPanel.Location = new System.Drawing.Point(0, 460);
            this.ttcControlPanel.Name = "ttcControlPanel";
            this.ttcControlPanel.SelectedIndex = 0;
            this.ttcControlPanel.Size = new System.Drawing.Size(647, 89);
            this.ttcControlPanel.TabIndex = 6;
            // 
            // tpNoControls
            // 
            this.tpNoControls.BackColor = System.Drawing.SystemColors.Control;
            this.tpNoControls.Location = new System.Drawing.Point(4, 22);
            this.tpNoControls.Name = "tpNoControls";
            this.tpNoControls.Padding = new System.Windows.Forms.Padding(3);
            this.tpNoControls.Size = new System.Drawing.Size(639, 63);
            this.tpNoControls.TabIndex = 0;
            this.tpNoControls.Text = "No Controls";
            // 
            // tpTerrainControls
            // 
            this.tpTerrainControls.BackColor = System.Drawing.SystemColors.Control;
            this.tpTerrainControls.Controls.Add(this.btnToggle);
            this.tpTerrainControls.Controls.Add(this.btnGo);
            this.tpTerrainControls.Controls.Add(this.groupBox2);
            this.tpTerrainControls.Controls.Add(this.groupBox1);
            this.tpTerrainControls.Location = new System.Drawing.Point(4, 22);
            this.tpTerrainControls.Name = "tpTerrainControls";
            this.tpTerrainControls.Padding = new System.Windows.Forms.Padding(3);
            this.tpTerrainControls.Size = new System.Drawing.Size(639, 63);
            this.tpTerrainControls.TabIndex = 1;
            this.tpTerrainControls.Text = "Terrain Controls";
            // 
            // btnToggle
            // 
            this.btnToggle.Location = new System.Drawing.Point(725, 45);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(51, 35);
            this.btnToggle.TabIndex = 3;
            this.btnToggle.Text = "Toggle";
            this.btnToggle.UseVisualStyleBackColor = true;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(725, 8);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(51, 35);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numCamRoll);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numCamPitch);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numCamYaw);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(374, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 50);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera Rotation Controls";
            // 
            // numCamRoll
            // 
            this.numCamRoll.Location = new System.Drawing.Point(264, 20);
            this.numCamRoll.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numCamRoll.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numCamRoll.Name = "numCamRoll";
            this.numCamRoll.Size = new System.Drawing.Size(57, 20);
            this.numCamRoll.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(230, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Roll";
            // 
            // numCamPitch
            // 
            this.numCamPitch.Location = new System.Drawing.Point(152, 20);
            this.numCamPitch.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numCamPitch.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numCamPitch.Name = "numCamPitch";
            this.numCamPitch.Size = new System.Drawing.Size(57, 20);
            this.numCamPitch.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Pitch";
            // 
            // numCamYaw
            // 
            this.numCamYaw.Location = new System.Drawing.Point(40, 20);
            this.numCamYaw.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numCamYaw.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numCamYaw.Name = "numCamYaw";
            this.numCamYaw.Size = new System.Drawing.Size(57, 20);
            this.numCamYaw.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Yaw";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numCamZ);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numCamY);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numCamX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(24, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Position Controls";
            // 
            // numCamZ
            // 
            this.numCamZ.Location = new System.Drawing.Point(248, 20);
            this.numCamZ.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCamZ.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numCamZ.Name = "numCamZ";
            this.numCamZ.Size = new System.Drawing.Size(57, 20);
            this.numCamZ.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(228, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Z";
            // 
            // numCamY
            // 
            this.numCamY.Location = new System.Drawing.Point(137, 20);
            this.numCamY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCamY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numCamY.Name = "numCamY";
            this.numCamY.Size = new System.Drawing.Size(57, 20);
            this.numCamY.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y";
            // 
            // numCamX
            // 
            this.numCamX.Location = new System.Drawing.Point(26, 20);
            this.numCamX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCamX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numCamX.Name = "numCamX";
            this.numCamX.Size = new System.Drawing.Size(57, 20);
            this.numCamX.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "X";
            // 
            // tpModelControls
            // 
            this.tpModelControls.BackColor = System.Drawing.SystemColors.Control;
            this.tpModelControls.Location = new System.Drawing.Point(4, 22);
            this.tpModelControls.Name = "tpModelControls";
            this.tpModelControls.Size = new System.Drawing.Size(639, 63);
            this.tpModelControls.TabIndex = 2;
            this.tpModelControls.Text = "Model Controls";
            // 
            // tpTextureControls
            // 
            this.tpTextureControls.BackColor = System.Drawing.SystemColors.Control;
            this.tpTextureControls.Location = new System.Drawing.Point(4, 22);
            this.tpTextureControls.Name = "tpTextureControls";
            this.tpTextureControls.Size = new System.Drawing.Size(639, 63);
            this.tpTextureControls.TabIndex = 3;
            this.tpTextureControls.Text = "Texture Controls";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(-54, 18);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(121, 97);
            this.treeView1.TabIndex = 7;
            // 
            // mapTexturesToolStripMenuItem
            // 
            this.mapTexturesToolStripMenuItem.Name = "mapTexturesToolStripMenuItem";
            this.mapTexturesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.mapTexturesToolStripMenuItem.Text = "Map Textures";
            //             // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "WinForms Content Loading";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ttcControlPanel.ResumeLayout(false);
            this.tpTerrainControls.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCamRoll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamYaw)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCamZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCamX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importVoxelTerrainToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private EditorViewerControl editorViewerControl;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private Controls.TablessTabControl ttcControlPanel;
        private System.Windows.Forms.TabPage tpNoControls;
        private System.Windows.Forms.TabPage tpTerrainControls;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numCamRoll;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numCamPitch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numCamYaw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numCamZ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCamY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCamX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tpModelControls;
        private System.Windows.Forms.TabPage tpTextureControls;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTerrianModelToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView manifestViewer;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ToolStripMenuItem mapTexturesToolStripMenuItem;

    }
}

