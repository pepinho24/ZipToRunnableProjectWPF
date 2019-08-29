using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterContextMenuOptions
{
    public class Startup
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to create a Send To shortcut?(y/n)");
            var response = Console.ReadKey();

            if (response.KeyChar.ToString().ToLower() == "y")
            {
                // add shortcut to app in the following folder:
                // C:\Users\<username>\AppData\Roaming\Microsoft\Windows\SendTo

                //path should be 'C:\Users\<username>\AppData\Roaming'
                string appdatapath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string sendtoPath = Path.Combine(appdatapath, @"Microsoft\Windows\SendTo");
                string path = Path.Combine(sendtoPath, @"Add to Runnable Sample.lnk");
                //object shDesktop = (object)"Test shortcut - " + DateTime.Now.ToString("HH-mm-ss");
                try
                {
                    WshShell shell = new WshShell();
                    string shortcutAddress = Directory.GetCurrentDirectory();// @"D:\MyWorkProjects\ZipToRunnableProjectWPF\ZipToRunnableProjectWPF\bin\Debug";
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
                    shortcut.Description = "New shortcut for Recycle Bin";
                    //shortcut.Hotkey = "Ctrl+Shift+N";
                    //shortcut.IconLocation = @"C:\WINDOWS\System32\imageres.dll";
                    shortcut.WorkingDirectory = shortcutAddress;
                    shortcut.TargetPath = Path.Combine(shortcutAddress, "ZipToRunnableProjectWPF.exe");
                    shortcut.Save();

                }
                catch (Exception)
                {
                    Console.WriteLine("Exception occurred");
                    throw;
                }
                Console.WriteLine("Shortcut created successfully");
            }
            ////accessing the CurrentUser root element  
            ////and adding "OurSettings" subkey to the "SOFTWARE" subkey  
            //RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");

            ////storing the values  
            //key.SetValue("Setting1", "This is our setting 1");
            //key.SetValue("Setting2", "This is our setting 2");
            //key.Close();
        }
    }
}
