using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZipToRunnableProjectWPF.Common;
using Path = System.IO.Path;

namespace ZipToRunnableProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string DestinationFolderPath = UniversalFilePathResolver.ResolvePath(ConfigurationManager.AppSettings["DestinationFolderPath"].ToString());//@"D:\Ticket archives\2019.08.27\ProgressAreaCustomProgress\Test";

        public string TemplateProjectLocation = UniversalFilePathResolver.ResolvePath(ConfigurationManager.AppSettings["TemplateProjectLocation"].ToString());

        public string DefaultSourcePath = @"D:\Ticket archives\2019.08.27\ProgressAreaCustomProgress\ProgressAreaCustomProgress";

        public string OriginalTemplateProjectName = ConfigurationManager.AppSettings["OriginalTemplateProjectName"].ToString();

        public string OriginalTemplateProjectPort = ConfigurationManager.AppSettings["OriginalTemplateProjectPort"].ToString();

        public MainWindow()
        {
            InitializeComponent();
            DestinationPathTextBox.Text = DestinationFolderPath;

            if (StaticVariables.SelectedFiles.Length > 0)
            {
                DefaultSourcePath = Path.GetDirectoryName(StaticVariables.SelectedFiles[0]);
            }

            var projects = new List<TemplateProject>();
            //var sourcePath = Environment.GetCommandLineArgs()[1];
            var sourcePath = DefaultSourcePath;
            var folders = Directory.GetDirectories(sourcePath);
            var files = Directory.GetFiles(sourcePath);

            var selectedfiles = new List<TemplateProject>();
            var selectedfolders = new List<TemplateProject>();
            foreach (var relativepathtempl in folders)
            {
                var proj = new TemplateProject(
                    Path.GetFileName(relativepathtempl),
                    Path.GetFullPath(relativepathtempl),
                    FileOrFolder.Folder);
                projects.Add(proj);
                if (StaticVariables.SelectedFiles.Contains(relativepathtempl))
                {
                    selectedfolders.Add(proj);
                }
            }
            foreach (var relativepathtempl in files)
            {
                var proj = new TemplateProject(
                    Path.GetFileName(relativepathtempl),
                    Path.GetFullPath(relativepathtempl),
                    FileOrFolder.File);
                projects.Add(proj);
                if (StaticVariables.SelectedFiles.Contains(relativepathtempl))
                {
                    selectedfiles.Add(proj);
                }
            }

            ListBoxTest.ItemsSource = projects;
            foreach (var selecteditem in selectedfiles.Concat(selectedfolders))
            {
                ListBoxTest.SelectedItems.Add(selecteditem);
            }
            ListBoxTest.Focus();

        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var newProjectName = NewProjectNameTextBox.Text;
            if (!IsProjectNameValid(newProjectName))
            {
                MessageBox.Show("Please enter a valid project name. Allowed Characters:\n1) Latin letters\n2) Digits\n3) Underscore");
                return;
            }

            var filesToExport = ListBoxTest.SelectedItems;
            var newTarget = PasteTemplate(newProjectName, new DirectoryInfo(DestinationFolderPath));

            foreach (TemplateProject file in filesToExport)
            {
                DirectoryInfo diSource = new DirectoryInfo(file.Path);
                DirectoryInfo diTarget = newTarget;

                CopyAll(diSource, diTarget, file.FileOrFolder);
            }

            MessageBox.Show("Runnable project created successfully");
            // open destination folder 
            Process.Start(DestinationFolderPath);
        }

        private bool IsProjectNameValid(string newProjectName)
        {
            string pattern = @"^[a-zA-Z0-9_]+$";

            Regex regex = new Regex(pattern);

            // Compare a string against the regular expression
            return regex.IsMatch(newProjectName);
        }

        public DirectoryInfo PasteTemplate(string projectName, DirectoryInfo target)
        {
            var templateSource = new DirectoryInfo(TemplateProjectLocation);
            PasteTemplateProject(templateSource, target);
            RenameTemplate(target, projectName);
            return new DirectoryInfo(Path.Combine(target.ToString(), projectName));
        }

        private void RenameTemplate(DirectoryInfo templateSource, string projectName)
        {
            var projectFolder = Path.Combine(templateSource.ToString(), OriginalTemplateProjectName);
            var newProjectFolder = Path.Combine(templateSource.ToString(), projectName);
            Directory.Move(projectFolder, newProjectFolder);

            var projectSlnPath = Path.Combine(templateSource.ToString(), OriginalTemplateProjectName + ".sln");
            var newProjectSlnPath = Path.Combine(templateSource.ToString(), projectName + ".sln");
            File.Move(projectSlnPath, newProjectSlnPath);

            ReconfigureSolution(newProjectSlnPath, projectName);
        }

        private void ReconfigureSolution(string newProjectSlnPath, string newProjectName)
        {
            string text = File.ReadAllText(newProjectSlnPath);
            var rnd = new Random();
            text = text.Replace(OriginalTemplateProjectPort, rnd.Next(10000, 65000).ToString());
            text = text.Replace(OriginalTemplateProjectName, newProjectName);
            File.WriteAllText(newProjectSlnPath, text);
        }

        public static void PasteTemplateProject(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                PasteTemplateProject(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, FileOrFolder fileOrFolder)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            if (fileOrFolder == FileOrFolder.File)
            {
                File.Copy(source.FullName, Path.Combine(target.ToString(), source.Name), true);
                return;
            }
            DirectoryInfo TargetSubDir = target.CreateSubdirectory(source.Name);

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(TargetSubDir.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    TargetSubDir.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, FileOrFolder.Folder);
            }
        }

        private void DestinationPath_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Focused");
        }

        private void DestinationPath_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void ChoosePath_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                // Set the help text description for the FolderBrowserDialog.
                dialog.Description =
                    "Select the directory that you want to use as the default.";

                // Default to the My Documents folder.
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = DestinationFolderPath;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    DestinationPathTextBox.Text = dialog.SelectedPath;
                    DestinationFolderPath = dialog.SelectedPath;
                }
            }
        }
    }
    public enum FileOrFolder
    {
        File,
        Folder
    }
    public class TemplateProject
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public FileOrFolder FileOrFolder { get; set; }

        public TemplateProject(string name, string path, FileOrFolder fileOrFolder)
        {
            this.Name = name;
            this.Path = path;
            this.FileOrFolder = fileOrFolder;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
