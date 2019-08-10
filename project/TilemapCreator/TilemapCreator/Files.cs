using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilemapCreator {

    public enum SaveExtensionType {
        JSON
    }

    public enum OpenExtensionType {
        TileSet,
        PNG,
        JPG,
        JSON
    }

    public static class Files {
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
