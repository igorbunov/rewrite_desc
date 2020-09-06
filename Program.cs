using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rewrite
{
    static class Program
    {
        private static Form1 f = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Program.f = new Form1();

                Application.Run(Program.f);
            }
            catch (Exception ex)
            {
                Program.f.highlightTimer.Stop();
                if (Program.f.currentDocumentAutosave != null)
                {
                    Program.f.Save(Program.f.autosaveFilename, false, Program.f.currentDocumentAutosave);                
                }
                
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Системная ошибка");
            }
        }
    }
}
