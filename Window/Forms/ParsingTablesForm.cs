using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Parser;

namespace ParsingTables.Forms
{
    public partial class ParsingTablesForm : Form
    {
        public ParsingTablesForm() { InitializeComponent(); }

        protected override void OnPaint(PaintEventArgs entPaint)
        {
            var rectangle = new Rectangle(0, 0, base.Width, base.Height + 30); //base.ClientRectangle;
            var linearGradBrush = new LinearGradientBrush(rectangle, Color.Yellow, Color.Pink,
                                                          LinearGradientMode.Vertical);

            entPaint.Graphics.FillRectangle(linearGradBrush, rectangle);
            base.OnPaint(entPaint);
        }

        void BrowseGrammar_Click(object sender, EventArgs ent)
        {
            var dlgOpenFile = new OpenFileDialog
                              {
                                  Title = "Grammar File Dialog",
                                  InitialDirectory = Directory.GetCurrentDirectory(),
                                  Filter = "Grammar files (*.gmr)|*.gmr"
                                  //,FilterIndex = 1
                                  //,RestoreDirectory = true
                              };

            if (DialogResult.OK == dlgOpenFile.ShowDialog()) txtGrammarFile.Text = dlgOpenFile.FileName;
        }

        void BrowseParser_Click(object sender, EventArgs ent)
        {
            var dlgFolderBrowse = new FolderBrowserDialog
                                  {
                                      ShowNewFolderButton = true,
                                      SelectedPath = Directory.GetCurrentDirectory()
                                  };
            
            if (DialogResult.OK == dlgFolderBrowse.ShowDialog()) txtParserFile.Text = dlgFolderBrowse.SelectedPath + @"\Output.prs";
        }

        void BtnParse_Click(object sender, EventArgs ent)
        {
            foreach (Control control in Controls)
            {
                if (!(control is TextBox)) continue;

                control.Focus();
                if (!Validate()) return;
            }

            try
            {
                var parser = new CLRParser(txtGrammarFile.Text.Trim());

                using (var stream = new FileStream(txtParserFile.Text.Trim(), FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Grammar >>>");
                    writer.WriteLine(parser.Grammar);
                    writer.WriteLine(Parser.Parser.SEPARATOR);
                    if (chkEntities.Checked)
                    {
                        writer.WriteLine("Grammar Entities >>>");
                        writer.WriteLine(parser.EntityCol);
                        writer.WriteLine(Parser.Parser.SEPARATOR);
                    }
                    if (chkFirstFollow.Checked)
                    {
                        writer.WriteLine("First & Follow >>>");
                        writer.WriteLine(parser.FirstnFollow());
                    }
                    if (chkGotoTable.Checked)
                    {
                        writer.WriteLine("Closures >>>");
                        writer.WriteLine(parser.ClouresGoToTable());
                    }
                    
                    if (chkSLRTable.Checked) writer.WriteLine("SLR Table >>>");
                    //writer.WriteLine(parser.LALRTable());
                    writer.Close();
                }
            }
            catch (FileNotFoundException expNoFile)
            {
                MessageBox.Show(expNoFile.Message);
            }
            catch (IOException expIO)
            {
                MessageBox.Show(expIO.Message);
            }
            catch (FormatException expFormat)
            {
                MessageBox.Show(expFormat.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        void TxtPath_Validating(object sender, CancelEventArgs entCancel)
        {
            if (!(sender is TextBox)) return;

            var txtBox = sender as TextBox;
            if (!String.IsNullOrEmpty(txtBox.Text.Trim())) return;

            errorProvider.SetError(txtBox, "Enter Path : " + txtBox.Tag);
            entCancel.Cancel = true;
        }

        void TxtPath_Validated(object sender, EventArgs ent)
        {
            if (!(sender is TextBox)) return;

            var txtBox = sender as TextBox;
            errorProvider.SetError(txtBox, "");
        }

        void BtnExit_Click(object sender, EventArgs ent) { Close(); }
    }
}