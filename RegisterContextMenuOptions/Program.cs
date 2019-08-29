using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterContextMenuOptions
{
    public class Startup
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            //accessing the CurrentUser root element  
            //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");

            //storing the values  
            key.SetValue("Setting1", "This is our setting 1");
            key.SetValue("Setting2", "This is our setting 2");
            key.Close();
        }
    }
}
