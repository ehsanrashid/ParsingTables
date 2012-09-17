using System;
using System.Windows.Forms;

using ParsingTables.Forms;

namespace ParsingTables
{
    public static class StartUp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ParsingTablesForm());

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
    }
}