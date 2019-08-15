using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace TilemapCreator {
    /// <summary>
    /// Save file class, used to export to JSON
    /// </summary>
    public class SaveFile {
        public string m_Name;

        public int m_TilemapWidth;
        public int m_TilemapHeigh;

        public List<List<int>> m_GridLayers = new List<List<int>>();

        public int m_TileWidth;
        public int m_TileHeight;

        public string m_LoadedTilesetPath;
    }

    /// <summary>
    /// Save class
    /// </summary>
    public class Save {
        //Only one SaveFile at one time
        private SaveFile m_SaveFile;

        /// <summary>
        /// Sets the Save File
        /// </summary>
        /// <param name="saveFile">SaveFile to set</param>
        /// <returns></returns>
        public SaveFile SetSaveFile(SaveFile saveFile) {
            m_SaveFile = saveFile;
            return m_SaveFile;
        }

        /// <summary>
        /// Loads from JSON into a SaveFile object
        /// </summary>
        /// <returns>SaveFile object</returns>
        public SaveFile LoadFromJSON() {
            OpenFileDialog openDialog = Files.OpenFileDialog(OpenExtensionType.JSON);

            if (openDialog.ShowDialog() == true) {
                string input = File.ReadAllText(openDialog.FileName);
                SaveFile file = JsonConvert.DeserializeObject<SaveFile>(input);
                return file;
            }
            return null;
        }

        /// <summary>
        /// Exports a SaveFile object to JSON
        /// </summary>
        public void ExportToJSON() {
            SaveFileDialog saveDialog = Files.OpenSaveDialog(SaveExtensionType.JSON);

            if (saveDialog.FileName != "") {
                string output = JsonConvert.SerializeObject(m_SaveFile);
                File.WriteAllText(saveDialog.FileName, output);
            }
        }
    }
}
