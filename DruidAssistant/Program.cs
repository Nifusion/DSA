using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DruidAssistant
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (string.IsNullOrEmpty(Properties.Settings.Default.SummonsXML))
            {
                //try to find it in local folder
                string local = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summons.xml");
                if (!File.Exists(local))
                {
                    //if not, I'm creating a blank one for you;
                    //you can handle a change later
                    Summons s = new Summons();
                    s.Save(local);
                }
                Properties.Settings.Default.SummonsXML = local;
                Properties.Settings.Default.Save();
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.SpellsXML))
            {
                //try to find it in local folder
                string local = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spells.xml");
                if (!File.Exists(local))
                {
                    //if not, I'm creating a blank one for you;
                    //you can handle a change later
                    Spells s = new Spells();
                    s.Save(local);
                }

                Properties.Settings.Default.SpellsXML = local;
                Properties.Settings.Default.Save();
            }

            Application.Run(new DruidMain());
        }
    }
}
