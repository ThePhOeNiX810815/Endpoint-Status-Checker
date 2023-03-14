using Microsoft.Win32;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EndpointChecker
{
    public partial class FontPublisher
    {
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        private static extern int AddFontResource(string lpszFilename);
        public static void SaveAndPublishFont(string resourceFileName)
        {
            string fontTargetFile = Path.Combine(System.Environment.GetFolderPath
                              (System.Environment.SpecialFolder.Fonts), resourceFileName);

            string resourceName = Assembly.GetCallingAssembly().GetName().Name + ".Fonts." + resourceFileName;

            if (!File.Exists(fontTargetFile))
            {
                // SAVE EMBEDDED FONT TO TEMP FILE
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                FileStream fileStream = new FileStream(fontTargetFile, FileMode.CreateNew);
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
                fileStream.Close();

                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontTargetFile);
                var actualFontName = fontCol.Families[0].Name;

                AddFontResource(fontTargetFile);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
                                  actualFontName, resourceFileName, RegistryValueKind.String);
            }           
        }
    }
}
