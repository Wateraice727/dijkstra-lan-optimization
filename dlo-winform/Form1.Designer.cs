namespace dlo_winform;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        groupBox1 = new GroupBox();
        button4 = new Button();
        button2 = new Button();
        start_button = new Button();
        groupBox3 = new GroupBox();
        cmbSampleGraphs = new ComboBox();
        buttonPrev = new Button();
        button5 = new Button();
        groupBox2 = new GroupBox();
        checkBoxEditWeight = new CheckBox();
        checkBox1 = new CheckBox();
        button3 = new Button();
        buttonRemoveNode = new Button();
        labelStartNode = new Label();
        txtStartNode = new TextBox();
        labelDestNode = new Label();
        txtDestNode = new TextBox();
        txtPacketSize = new TextBox();
        cmbPacketUnit = new ComboBox();
        labelPacketSize = new Label();
        pbxCanvas = new PictureBox();
        txtLog = new TextBox();
        txtEdgeWeightEditor = new TextBox();
        groupBoxCanvas = new GroupBox();
        groupBoxLogs = new GroupBox();
        groupBox4 = new GroupBox();
        groupBox5 = new GroupBox();
        btnGenerate = new Button();
        txtGenEdges = new TextBox();
        txtGenNodes = new TextBox();
        label1 = new Label();
        label2 = new Label();
        groupBox1.SuspendLayout();
        groupBox3.SuspendLayout();
        groupBox2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pbxCanvas).BeginInit();
        groupBoxCanvas.SuspendLayout();
        groupBoxLogs.SuspendLayout();
        groupBox4.SuspendLayout();
        groupBox5.SuspendLayout();
        SuspendLayout();
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(button4);
        groupBox1.Controls.Add(button2);
        groupBox1.Controls.Add(start_button);
        groupBox1.Location = new Point(15, 14);
        groupBox1.Margin = new Padding(3, 2, 3, 2);
        groupBox1.Name = "groupBox1";
        groupBox1.Padding = new Padding(3, 2, 3, 2);
        groupBox1.Size = new Size(206, 84);
        groupBox1.TabIndex = 0;
        groupBox1.TabStop = false;
        groupBox1.Text = "Test Activation";
        // 
        // button4
        // 
        button4.Location = new Point(64, 50);
        button4.Margin = new Padding(3, 2, 3, 2);
        button4.Name = "button4";
        button4.Size = new Size(74, 26);
        button4.TabIndex = 2;
        button4.Text = "Pause";
        button4.UseVisualStyleBackColor = true;
        button4.Click += button4_Click;
        // 
        // button2
        // 
        button2.Location = new Point(110, 20);
        button2.Margin = new Padding(3, 2, 3, 2);
        button2.Name = "button2";
        button2.Size = new Size(74, 26);
        button2.TabIndex = 1;
        button2.Text = "Stop";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // start_button
        // 
        start_button.Location = new Point(19, 20);
        start_button.Margin = new Padding(3, 2, 3, 2);
        start_button.Name = "start_button";
        start_button.Size = new Size(74, 26);
        start_button.TabIndex = 0;
        start_button.Text = "Start";
        start_button.UseVisualStyleBackColor = true;
        start_button.Click += button1_Click;
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(cmbSampleGraphs);
        groupBox3.Controls.Add(buttonPrev);
        groupBox3.Controls.Add(button5);
        groupBox3.Location = new Point(15, 103);
        groupBox3.Margin = new Padding(3, 2, 3, 2);
        groupBox3.Name = "groupBox3";
        groupBox3.Padding = new Padding(3, 2, 3, 2);
        groupBox3.Size = new Size(206, 82);
        groupBox3.TabIndex = 7;
        groupBox3.TabStop = false;
        groupBox3.Text = "Sample graphs";
        // 
        // cmbSampleGraphs
        // 
        cmbSampleGraphs.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSampleGraphs.Location = new Point(8, 19);
        cmbSampleGraphs.Margin = new Padding(3, 2, 3, 2);
        cmbSampleGraphs.Name = "cmbSampleGraphs";
        cmbSampleGraphs.Size = new Size(190, 23);
        cmbSampleGraphs.TabIndex = 4;
        // 
        // buttonPrev
        // 
        buttonPrev.Location = new Point(8, 46);
        buttonPrev.Margin = new Padding(3, 2, 3, 2);
        buttonPrev.Name = "buttonPrev";
        buttonPrev.Size = new Size(92, 26);
        buttonPrev.TabIndex = 5;
        buttonPrev.Text = "Previous";
        buttonPrev.UseVisualStyleBackColor = true;
        buttonPrev.Click += buttonPrev_Click;
        // 
        // button5
        // 
        button5.Location = new Point(105, 46);
        button5.Margin = new Padding(3, 2, 3, 2);
        button5.Name = "button5";
        button5.Size = new Size(93, 26);
        button5.TabIndex = 3;
        button5.Text = "Next sample";
        button5.UseVisualStyleBackColor = true;
        button5.Click += button5_Click;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(checkBoxEditWeight);
        groupBox2.Controls.Add(checkBox1);
        groupBox2.Controls.Add(button3);
        groupBox2.Controls.Add(buttonRemoveNode);
        groupBox2.Controls.Add(labelStartNode);
        groupBox2.Controls.Add(txtStartNode);
        groupBox2.Controls.Add(labelDestNode);
        groupBox2.Controls.Add(txtDestNode);
        groupBox2.Location = new Point(15, 190);
        groupBox2.Margin = new Padding(3, 2, 3, 2);
        groupBox2.Name = "groupBox2";
        groupBox2.Padding = new Padding(3, 2, 3, 2);
        groupBox2.Size = new Size(206, 173);
        groupBox2.TabIndex = 1;
        groupBox2.TabStop = false;
        groupBox2.Text = "Test Parameters";
        // 
        // checkBoxEditWeight
        // 
        checkBoxEditWeight.Location = new Point(15, 20);
        checkBoxEditWeight.Margin = new Padding(3, 2, 3, 2);
        checkBoxEditWeight.Name = "checkBoxEditWeight";
        checkBoxEditWeight.Size = new Size(93, 33);
        checkBoxEditWeight.TabIndex = 7;
        checkBoxEditWeight.Text = "Weight edit";
        checkBoxEditWeight.UseVisualStyleBackColor = true;
        checkBoxEditWeight.CheckedChanged += checkBoxEditWeight_CheckedChanged;
        // 
        // checkBox1
        // 
        checkBox1.Location = new Point(114, 20);
        checkBox1.Margin = new Padding(3, 2, 3, 2);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(131, 33);
        checkBox1.TabIndex = 2;
        checkBox1.Text = "Edge edit";
        checkBox1.UseVisualStyleBackColor = true;
        checkBox1.CheckedChanged += checkBox1_CheckedChanged;
        // 
        // button3
        // 
        button3.Location = new Point(15, 57);
        button3.Margin = new Padding(3, 2, 3, 2);
        button3.Name = "button3";
        button3.Size = new Size(178, 25);
        button3.TabIndex = 0;
        button3.Text = "Add node";
        button3.UseVisualStyleBackColor = true;
        button3.Click += button3_Click;
        // 
        // buttonRemoveNode
        // 
        buttonRemoveNode.Location = new Point(15, 86);
        buttonRemoveNode.Margin = new Padding(3, 2, 3, 2);
        buttonRemoveNode.Name = "buttonRemoveNode";
        buttonRemoveNode.Size = new Size(178, 25);
        buttonRemoveNode.TabIndex = 10;
        buttonRemoveNode.Text = "Remove node";
        buttonRemoveNode.UseVisualStyleBackColor = true;
        buttonRemoveNode.Click += buttonRemoveNode_Click;
        // 
        // labelStartNode
        // 
        labelStartNode.Location = new Point(23, 114);
        labelStartNode.Name = "labelStartNode";
        labelStartNode.Size = new Size(77, 20);
        labelStartNode.TabIndex = 3;
        labelStartNode.Text = "Start Node";
        // 
        // txtStartNode
        // 
        txtStartNode.Location = new Point(105, 114);
        txtStartNode.Margin = new Padding(3, 2, 3, 2);
        txtStartNode.Name = "txtStartNode";
        txtStartNode.Size = new Size(75, 23);
        txtStartNode.TabIndex = 4;
        txtStartNode.TextAlign = HorizontalAlignment.Center;
        // 
        // labelDestNode
        // 
        labelDestNode.Location = new Point(23, 138);
        labelDestNode.Name = "labelDestNode";
        labelDestNode.Size = new Size(77, 20);
        labelDestNode.TabIndex = 5;
        labelDestNode.Text = "Dest Node";
        // 
        // txtDestNode
        // 
        txtDestNode.Location = new Point(105, 138);
        txtDestNode.Margin = new Padding(3, 2, 3, 2);
        txtDestNode.Name = "txtDestNode";
        txtDestNode.Size = new Size(75, 23);
        txtDestNode.TabIndex = 6;
        txtDestNode.TextAlign = HorizontalAlignment.Center;
        // 
        // txtPacketSize
        // 
        txtPacketSize.Location = new Point(97, 20);
        txtPacketSize.Margin = new Padding(3, 2, 3, 2);
        txtPacketSize.Name = "txtPacketSize";
        txtPacketSize.Size = new Size(44, 23);
        txtPacketSize.TabIndex = 1;
        txtPacketSize.Text = "1500";
        txtPacketSize.TextAlign = HorizontalAlignment.Center;
        // 
        // cmbPacketUnit
        // 
        cmbPacketUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPacketUnit.Items.AddRange(new object[] { "bytes", "KB", "MB", "GB" });
        cmbPacketUnit.Location = new Point(144, 20);
        cmbPacketUnit.Margin = new Padding(3, 2, 3, 2);
        cmbPacketUnit.Name = "cmbPacketUnit";
        cmbPacketUnit.Size = new Size(50, 23);
        cmbPacketUnit.TabIndex = 2;
        // 
        // labelPacketSize
        // 
        labelPacketSize.Location = new Point(15, 20);
        labelPacketSize.Name = "labelPacketSize";
        labelPacketSize.Size = new Size(77, 20);
        labelPacketSize.TabIndex = 0;
        labelPacketSize.Text = "Packet size";
        // 
        // pbxCanvas
        // 
        pbxCanvas.BackColor = Color.White;
        pbxCanvas.Location = new Point(13, 22);
        pbxCanvas.Margin = new Padding(3, 2, 3, 2);
        pbxCanvas.Name = "pbxCanvas";
        pbxCanvas.Size = new Size(560, 315);
        pbxCanvas.TabIndex = 3;
        pbxCanvas.TabStop = false;
        // FIX: Tự động co giãn theo viền GroupBox
        pbxCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pbxCanvas.Click += pictureBox1_Click;
        // 
        // txtLog
        // 
        txtLog.Location = new Point(13, 19);
        txtLog.Margin = new Padding(3, 2, 3, 2);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(560, 147);
        txtLog.TabIndex = 4;
        // FIX: Tự động co giãn theo viền GroupBox
        txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // txtEdgeWeightEditor
        // 
        txtEdgeWeightEditor.Location = new Point(0, 0);
        txtEdgeWeightEditor.Margin = new Padding(3, 2, 3, 2);
        txtEdgeWeightEditor.Name = "txtEdgeWeightEditor";
        txtEdgeWeightEditor.Size = new Size(44, 23);
        txtEdgeWeightEditor.TabIndex = 99;
        txtEdgeWeightEditor.Visible = false;
        txtEdgeWeightEditor.TextChanged += txtEdgeWeightEditor_TextChanged;
        txtEdgeWeightEditor.KeyPress += txtEdgeWeightEditor_KeyPress;
        txtEdgeWeightEditor.LostFocus += txtEdgeWeightEditor_LostFocus;
        // 
        // groupBoxCanvas
        // 
        groupBoxCanvas.Controls.Add(pbxCanvas);
        groupBoxCanvas.Location = new Point(226, 14);
        groupBoxCanvas.Margin = new Padding(3, 2, 3, 2);
        groupBoxCanvas.Name = "groupBoxCanvas";
        groupBoxCanvas.Padding = new Padding(3, 2, 3, 2);
        groupBoxCanvas.Size = new Size(586, 349);
        groupBoxCanvas.TabIndex = 5;
        groupBoxCanvas.TabStop = false;
        groupBoxCanvas.Text = "Simulation diagram";
        // FIX: Tự neo 4 góc để giãn ra Full HD
        groupBoxCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // groupBoxLogs
        // 
        groupBoxLogs.Controls.Add(txtLog);
        groupBoxLogs.Location = new Point(226, 368);
        groupBoxLogs.Margin = new Padding(3, 2, 3, 2);
        groupBoxLogs.Name = "groupBoxLogs";
        groupBoxLogs.Padding = new Padding(3, 2, 3, 2);
        groupBoxLogs.Size = new Size(586, 169);
        groupBoxLogs.TabIndex = 6;
        groupBoxLogs.TabStop = false;
        groupBoxLogs.Text = "Logs";
        // FIX: Tự neo 3 góc để giãn ngang và ôm đáy màn hình
        groupBoxLogs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // groupBox4
        // 
        groupBox4.Controls.Add(labelPacketSize);
        groupBox4.Controls.Add(txtPacketSize);
        groupBox4.Controls.Add(cmbPacketUnit);
        groupBox4.Location = new Point(15, 482);
        groupBox4.Margin = new Padding(3, 2, 3, 2);
        groupBox4.Name = "groupBox4";
        groupBox4.Padding = new Padding(3, 2, 3, 2);
        groupBox4.Size = new Size(206, 52);
        groupBox4.TabIndex = 14;
        groupBox4.TabStop = false;
        groupBox4.Text = "Packet Settings";
        // 
        // groupBox5
        // 
        groupBox5.Controls.Add(btnGenerate);
        groupBox5.Controls.Add(txtGenEdges);
        groupBox5.Controls.Add(txtGenNodes);
        groupBox5.Controls.Add(label1);
        groupBox5.Controls.Add(label2);
        groupBox5.Location = new Point(15, 368);
        groupBox5.Name = "groupBox5";
        groupBox5.Size = new Size(206, 109);
        groupBox5.TabIndex = 100;
        groupBox5.TabStop = false;
        groupBox5.Text = "Random Generator";
        // 
        // btnGenerate
        // 
        btnGenerate.Location = new Point(6, 80);
        btnGenerate.Name = "btnGenerate";
        btnGenerate.Size = new Size(187, 23);
        btnGenerate.TabIndex = 7;
        btnGenerate.Text = "Generate";
        btnGenerate.UseVisualStyleBackColor = true;
        // 
        // txtGenEdges
        // 
        txtGenEdges.Location = new Point(93, 48);
        txtGenEdges.Name = "txtGenEdges";
        txtGenEdges.Size = new Size(100, 23);
        txtGenEdges.TabIndex = 5;
        txtGenEdges.TextAlign = HorizontalAlignment.Center;
        txtGenEdges.TextChanged += textBox1_TextChanged;
        // 
        // txtGenNodes
        // 
        txtGenNodes.Location = new Point(93, 19);
        txtGenNodes.Name = "txtGenNodes";
        txtGenNodes.Size = new Size(100, 23);
        txtGenNodes.TabIndex = 6;
        txtGenNodes.TextAlign = HorizontalAlignment.Center;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(6, 22);
        label1.Name = "label1";
        label1.Size = new Size(52, 15);
        label1.TabIndex = 5;
        label1.Text = "Node(s):";
        label1.Click += label1_Click;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(6, 51);
        label2.Name = "label2";
        label2.Size = new Size(49, 15);
        label2.TabIndex = 6;
        label2.Text = "Edge(s):";
        label2.Click += label2_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(826, 540);
        Controls.Add(groupBox5);
        Controls.Add(groupBoxCanvas);
        Controls.Add(groupBoxLogs);
        Controls.Add(txtEdgeWeightEditor);
        Controls.Add(groupBox3);
        Controls.Add(groupBox2);
        Controls.Add(groupBox4);
        Controls.Add(groupBox1);
        Margin = new Padding(3, 2, 3, 2);
        Name = "Form1";
        Text = "Dijkstra LAN Engine";
        Load += Form1_Load;
        groupBox1.ResumeLayout(false);
        groupBox3.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pbxCanvas).EndInit();
        groupBoxCanvas.ResumeLayout(false);
        groupBoxLogs.ResumeLayout(false);
        groupBoxLogs.PerformLayout();
        groupBox4.ResumeLayout(false);
        groupBox4.PerformLayout();
        groupBox5.ResumeLayout(false);
        groupBox5.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button buttonRemoveNode;
    private System.Windows.Forms.TextBox txtPacketSize;
    private System.Windows.Forms.ComboBox cmbPacketUnit;
    private System.Windows.Forms.Label labelPacketSize;
    private System.Windows.Forms.CheckBox checkBoxEditWeight;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.PictureBox pbxCanvas;
    private System.Windows.Forms.ComboBox cmbSampleGraphs;
    private System.Windows.Forms.Button buttonPrev;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Label labelStartNode;
    private System.Windows.Forms.TextBox txtStartNode;
    private System.Windows.Forms.Label labelDestNode;
    private System.Windows.Forms.TextBox txtDestNode;
    private System.Windows.Forms.TextBox txtLog;
    private System.Windows.Forms.TextBox txtEdgeWeightEditor;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button start_button;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBoxCanvas;
    private System.Windows.Forms.GroupBox groupBoxLogs;
    private System.Windows.Forms.GroupBox groupBox4;

    #endregion

    private GroupBox groupBox5;
    private Label label1;
    private Label label2;
    private TextBox txtGenEdges;
    private TextBox txtGenNodes;
    private Button btnGenerate;
}