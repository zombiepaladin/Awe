using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace MinecraftImportTesting
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = ContentPath();

            fileDialog.Title = "Select File";

            fileDialog.Filter = "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ProcessMinecraftFile(fileDialog.FileName, Path.ChangeExtension(fileDialog.FileName, ".out"));
            }
        }

        public static void ProcessMinecraftFile(string input, string output)
        {
            //First Error checking
            if (!File.Exists(input))
                throw new ArgumentException("Input file must be a minecraft map file.");

            //First lets just throw out the bits
            Stream inputStream = File.OpenRead(input);
            Stream outputStream = File.Create(output);

            int buffer = 0;
            while ((buffer = inputStream.ReadByte()) != -1)
            {
                outputStream.WriteByte((byte)buffer);
            }
        }

        private static string ContentPath()
        {
            // Default to the directory which contains our content files.
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string relativePath = Path.Combine(assemblyLocation, "../../../../Content");
            string contentPath = Path.GetFullPath(relativePath);
            return contentPath;
        }
    }
}
