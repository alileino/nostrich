using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using WesternLib;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace LevelEditor
{

    static class Program
    {
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
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                Console.ReadKey();
            }
        }
    }
}
