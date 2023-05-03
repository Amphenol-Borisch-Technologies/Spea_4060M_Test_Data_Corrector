using System;
using System.Windows.Forms;

namespace Spea_4060M_Test_Data_Corrector {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Spea_4060M_Test_Data_Corrector());
        }
    }
}
