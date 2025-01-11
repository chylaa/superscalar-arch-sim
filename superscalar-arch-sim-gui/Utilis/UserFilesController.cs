using System;
using System.IO;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Utilis
{
    internal static class UserFilesController
    {
        public const string DataTextFilter = "Data/Text Files (*.data;*.text)|*.data;*.text";
        public const string XmlFilter = "XML Files (*.xml)|*.xml";
        public const string TextFilter = "Text Files (*.txt)|*.txt";
        public const string ElfFilter = "ELF Files (*.elf)|*.elf";
        public const string AssemblyFilter = "Assembly Files (*.s)|*.s";
        public const string CoreSettingsFilesFilter = "Core settings (*.scs)|*.scs";
        public const string AllFilesFilter = "All Files (*.*)|*.*";

        public static string ShortDateTimeNowFilename
            => DateTime.Now.ToString("ddMMyyyy_HHmmss");

        public static string ContactFilters(params string[] filters)
            => string.Join("|", filters);

        public static bool SaveFile(string filePath, byte[] data, bool showerr = true)
        {
            try
            {
                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch (Exception ex)
            {
                if (showerr)
                    MessageBox.Show($"Error saving file: {ex.Message}", "Bytes write error");
                return false;
            }
        }
        public static bool SaveFile(string filePath, string data, bool showerr = true, bool append = false)
        {
            try
            {
                if (append)
                    File.AppendAllText(filePath, data);
                else
                    File.WriteAllText(filePath, data);
                return true;
            }
            catch (Exception ex)
            {
                if (showerr)
                    MessageBox.Show($"Error saving file: {ex.Message}", "Text file save error");
                return false;
            }
        }
        public static byte[] LoadFileBytes(string filePath, bool showerr = true)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                if (showerr)
                    MessageBox.Show($"Error reading file: {ex.Message}", "Bytes read error");
                return null;
            }
        }
        public static string LoadFileText(string filePath, bool showerr = true)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                if (showerr)
                    MessageBox.Show($"Error reading file: {ex.Message}", "Text file read error");
                return null;
            }
        }
        private static string AskForFilePath(FileDialog dialog, string filter)
        {
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }
        public static string AskForOpenFilePath(string filter)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                return AskForFilePath(dialog, filter);
            }
        }
        public static string AskForSaveFilePath(string filename, string filter)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
                dialog.DefaultExt = Path.HasExtension(filename) ? Path.GetExtension(filename) : string.Empty;
                dialog.FileName = filename ?? ShortDateTimeNowFilename;
                dialog.Filter = filter;
                return AskForFilePath(dialog, filter);
            }
        }
        public static string AskForFolder()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return null;
        }
    }
}
