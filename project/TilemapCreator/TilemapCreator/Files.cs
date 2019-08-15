using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilemapCreator {

    /// <summary>
    /// Save extension enum
    /// </summary>
    public enum SaveExtensionType {
        JSON
    }

    /// <summary>
    /// Open extension enum
    /// </summary>
    public enum OpenExtensionType {
        TileSet,
        PNG,
        JPG,
        JSON
    }

    /// <summary>
    /// Custom static Files class
    /// </summary>
    public static class Files {
        /// <summary>
        /// Static Save function
        /// </summary>
        /// <param name="saveType">Save type extension</param>
        /// <returns></returns>
        public static SaveFileDialog OpenSaveDialog(SaveExtensionType saveType) {
            SaveFileDialog saveDialog = new SaveFileDialog();

            if (saveType == SaveExtensionType.JSON) {
                saveDialog.Title = "Save JSON Format file";
                saveDialog.Filter = "JSON Format|*.json";
                saveDialog.ShowDialog();
                return saveDialog;
            }
            return null;
        }

        /// <summary>
        /// Static open file function
        /// </summary>
        /// <param name="openType">Open type extension</param>
        /// <returns></returns>
        public static OpenFileDialog OpenFileDialog(OpenExtensionType openType) {
            OpenFileDialog openDialog = new OpenFileDialog();

            if (openType == OpenExtensionType.TileSet) {
                openDialog.Title = "Choose a Tileset";
                openDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";
                return openDialog;
            }
            else if (openType == OpenExtensionType.JSON) {
                openDialog.Title = "Open a JSON Tilemap file";
                openDialog.Filter = "JSON Format|*.json";
                return openDialog;
            }
            return null;
        }
    }
}
