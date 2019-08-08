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
        JPG
    }

    public static class Files {
        public static SaveFileDialog OpenSaveDialog(SaveExtensionType saveType) {
            SaveFileDialog saveDialog = new SaveFileDialog();

            if (saveType == SaveExtensionType.JSON) {
                saveDialog.Filter = "JSON Format|*.json";
                saveDialog.Title = "Save JSON Format file";
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
            return null;
        }
    }
}
