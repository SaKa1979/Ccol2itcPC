using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public struct IntergreenEntry
{
    public string entry;
    public bool TGOset;
}

namespace CCOL2iTCPC
{
    static class Program
    {
        public const string define = "#define";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            MainStart mainStart = new MainStart();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 gui = new Form1(mainStart);
            Application.Run(gui);
        }
    }

    public enum faseType { Auto, Fiets, Voetganger, OpenbaarVervoer };
}