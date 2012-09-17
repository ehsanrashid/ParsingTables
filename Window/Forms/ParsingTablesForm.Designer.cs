using System.Windows.Forms;
namespace ParsingTables.Forms
{
    partial class ParsingTablesForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParsingTablesForm));
            this.txtGrammarFile = new System.Windows.Forms.TextBox();
            this.txtParserFile = new System.Windows.Forms.TextBox();
            this.lblGrammarFile = new System.Windows.Forms.Label();
            this.lblParserFile = new System.Windows.Forms.Label();
            this.browseGrammar = new System.Windows.Forms.Button();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkSLRTable = new System.Windows.Forms.CheckBox();
            this.chkGotoTable = new System.Windows.Forms.CheckBox();
            this.chkFirstFollow = new System.Windows.Forms.CheckBox();
            this.chkEntities = new System.Windows.Forms.CheckBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.browseParser = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnExit = new System.Windows.Forms.Button();
            this.grpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGrammarFile
            // 
            this.txtGrammarFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGrammarFile.Location = new System.Drawing.Point(83, 20);
            this.txtGrammarFile.Name = "txtGrammarFile";
            this.txtGrammarFile.Size = new System.Drawing.Size(190, 21);
            this.txtGrammarFile.TabIndex = 1;
            this.txtGrammarFile.Tag = "Grammar Filename";
            this.txtGrammarFile.Validated += new System.EventHandler(this.txtPath_Validated);
            this.txtGrammarFile.Validating += new System.ComponentModel.CancelEventHandler(this.txtPath_Validating);
            // 
            // txtParserFile
            // 
            this.txtParserFile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParserFile.Location = new System.Drawing.Point(83, 56);
            this.txtParserFile.Name = "txtParserFile";
            this.txtParserFile.Size = new System.Drawing.Size(190, 21);
            this.txtParserFile.TabIndex = 4;
            this.txtParserFile.Tag = "Parser Filename";
            this.txtParserFile.Validated += new System.EventHandler(this.txtPath_Validated);
            this.txtParserFile.Validating += new System.ComponentModel.CancelEventHandler(this.txtPath_Validating);
            // 
            // lblGrammarFile
            // 
            this.lblGrammarFile.AutoSize = true;
            this.lblGrammarFile.BackColor = System.Drawing.Color.Transparent;
            this.lblGrammarFile.Location = new System.Drawing.Point(8, 25);
            this.lblGrammarFile.Name = "lblGrammarFile";
            this.lblGrammarFile.Size = new System.Drawing.Size(76, 13);
            this.lblGrammarFile.TabIndex = 0;
            this.lblGrammarFile.Text = "Grammer File :";
            // 
            // lblParserFile
            // 
            this.lblParserFile.AutoSize = true;
            this.lblParserFile.BackColor = System.Drawing.Color.Transparent;
            this.lblParserFile.Location = new System.Drawing.Point(20, 61);
            this.lblParserFile.Name = "lblParserFile";
            this.lblParserFile.Size = new System.Drawing.Size(64, 13);
            this.lblParserFile.TabIndex = 3;
            this.lblParserFile.Text = "Parser File :";
            // 
            // browseGrammar
            // 
            this.browseGrammar.AutoSize = true;
            this.browseGrammar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.browseGrammar.BackColor = System.Drawing.Color.Transparent;
            this.browseGrammar.CausesValidation = false;
            this.browseGrammar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.browseGrammar.Location = new System.Drawing.Point(279, 20);
            this.browseGrammar.Name = "browseGrammar";
            this.browseGrammar.Size = new System.Drawing.Size(29, 23);
            this.browseGrammar.TabIndex = 2;
            this.browseGrammar.Tag = "";
            this.browseGrammar.Text = "...";
            this.browseGrammar.UseVisualStyleBackColor = false;
            this.browseGrammar.Click += new System.EventHandler(this.browseGrammar_Click);
            // 
            // grpOptions
            // 
            this.grpOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpOptions.BackColor = System.Drawing.Color.Transparent;
            this.grpOptions.CausesValidation = false;
            this.grpOptions.Controls.Add(this.chkSLRTable);
            this.grpOptions.Controls.Add(this.chkGotoTable);
            this.grpOptions.Controls.Add(this.chkFirstFollow);
            this.grpOptions.Controls.Add(this.chkEntities);
            this.grpOptions.Location = new System.Drawing.Point(11, 97);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(216, 65);
            this.grpOptions.TabIndex = 6;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // chkSLRTable
            // 
            this.chkSLRTable.AutoSize = true;
            this.chkSLRTable.CausesValidation = false;
            this.chkSLRTable.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkSLRTable.Location = new System.Drawing.Point(124, 43);
            this.chkSLRTable.Name = "chkSLRTable";
            this.chkSLRTable.Size = new System.Drawing.Size(71, 17);
            this.chkSLRTable.TabIndex = 3;
            this.chkSLRTable.Text = "SLR Table";
            this.chkSLRTable.UseVisualStyleBackColor = true;
            // 
            // chkGotoTable
            // 
            this.chkGotoTable.AutoSize = true;
            this.chkGotoTable.CausesValidation = false;
            this.chkGotoTable.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkGotoTable.Location = new System.Drawing.Point(124, 20);
            this.chkGotoTable.Name = "chkGotoTable";
            this.chkGotoTable.Size = new System.Drawing.Size(76, 17);
            this.chkGotoTable.TabIndex = 2;
            this.chkGotoTable.Text = "Goto Table";
            this.chkGotoTable.UseVisualStyleBackColor = true;
            // 
            // chkFirstFollow
            // 
            this.chkFirstFollow.AutoSize = true;
            this.chkFirstFollow.CausesValidation = false;
            this.chkFirstFollow.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkFirstFollow.Location = new System.Drawing.Point(17, 43);
            this.chkFirstFollow.Name = "chkFirstFollow";
            this.chkFirstFollow.Size = new System.Drawing.Size(88, 17);
            this.chkFirstFollow.TabIndex = 1;
            this.chkFirstFollow.Text = "First && Follow";
            this.chkFirstFollow.UseVisualStyleBackColor = true;
            // 
            // chkEntities
            // 
            this.chkEntities.AutoSize = true;
            this.chkEntities.CausesValidation = false;
            this.chkEntities.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkEntities.Location = new System.Drawing.Point(17, 20);
            this.chkEntities.Name = "chkEntities";
            this.chkEntities.Size = new System.Drawing.Size(59, 17);
            this.chkEntities.TabIndex = 0;
            this.chkEntities.Text = "Entities";
            this.chkEntities.UseVisualStyleBackColor = true;
            // 
            // btnParse
            // 
            this.btnParse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnParse.BackColor = System.Drawing.Color.Transparent;
            this.btnParse.Location = new System.Drawing.Point(233, 104);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(75, 25);
            this.btnParse.TabIndex = 7;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = false;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // browseParser
            // 
            this.browseParser.AutoSize = true;
            this.browseParser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.browseParser.BackColor = System.Drawing.Color.Transparent;
            this.browseParser.CausesValidation = false;
            this.browseParser.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.browseParser.Location = new System.Drawing.Point(279, 56);
            this.browseParser.Name = "browseParser";
            this.browseParser.Size = new System.Drawing.Size(29, 23);
            this.browseParser.TabIndex = 8;
            this.browseParser.Tag = "";
            this.browseParser.Text = "...";
            this.browseParser.UseVisualStyleBackColor = false;
            this.browseParser.Click += new System.EventHandler(this.browseParser_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 500;
            this.errorProvider.ContainerControl = this;
            // 
            // btnExit
            // 
            this.btnExit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.CausesValidation = false;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(233, 137);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // ParsingTablesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.btnExit;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(316, 174);
            this.ControlBox = false;
            this.Controls.Add(this.browseParser);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.txtParserFile);
            this.Controls.Add(this.txtGrammarFile);
            this.Controls.Add(this.lblGrammarFile);
            this.Controls.Add(this.lblParserFile);
            this.Controls.Add(this.browseGrammar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ParsingTablesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Parsing Tables";
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox txtGrammarFile;
        internal System.Windows.Forms.TextBox txtParserFile;
        internal System.Windows.Forms.Label lblGrammarFile;
        internal System.Windows.Forms.Label lblParserFile;
        internal System.Windows.Forms.Button browseGrammar;
        internal System.Windows.Forms.GroupBox grpOptions;
        internal System.Windows.Forms.CheckBox chkSLRTable;
        internal System.Windows.Forms.CheckBox chkGotoTable;
        internal System.Windows.Forms.CheckBox chkFirstFollow;
        internal System.Windows.Forms.CheckBox chkEntities;
        internal System.Windows.Forms.Button btnParse;
        internal Button browseParser;
        private ErrorProvider errorProvider;
        internal Button btnExit;
    }
}

