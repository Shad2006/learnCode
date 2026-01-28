using System;
using System.Windows.Forms;
static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Database2 db = new Database2();
        Application.Run(new Form1());
    }
}