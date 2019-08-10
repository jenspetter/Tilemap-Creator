using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TilemapCreator {
    public class SaveFile {
        public string m_Name;

        public int m_TilemapWidth;
        public int m_TilemapHeigh;

        public List<List<int>> m_GridLayers = new List<List<int>>();

        public int m_TileWidth;
        public int m_TileHeight;
    }

    public class Save {

        private SaveFile m_SaveFile;

        public SaveFile SetSaveFile(SaveFile saveFile) {
            m_SaveFile = saveFile;
            return m_SaveFile;
        }

        public SaveFile LoadFromJSON() {
            OpenFileDialog openDialog = Files.OpenFileDialog(OpenExtensionType.JSON);

            if (openDialog.ShowDialog() == true) {
                string input = File.ReadAllText(openDialog.FileName);
                SaveFile file = JsonConvert.DeserializeObject<SaveFile>(input);
                return file;
            }
            return null;
        }

        public void ExportToJSON() {
            SaveFileDialog saveDialog = Files.OpenSaveDialog(SaveExtensionType.JSON);

            if (saveDialog.FileName != "") {
                string output = JsonConvert.SerializeObject(m_SaveFile);
                File.WriteAllText(saveDialog.FileName, output);
            }
        }
    }
}
