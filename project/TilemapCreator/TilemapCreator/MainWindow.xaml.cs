using Microsoft.Win32;
using System;
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
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

public enum Direction {
    Up,
    Right,
    Down,
    Left
}

namespace TilemapCreator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public PaintManager m_PaintManager;
        public GridManager m_GridManager;
        public TileSetManager m_TileSetManager;
        public Save m_SaveClass;

        BitmapImage carBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));
        
        public MainWindow() {
            InitializeComponent();

            m_PaintManager = new PaintManager();
            m_TileSetManager = new TileSetManager(TileSetCanvas, this);
            m_GridManager = new GridManager(canvas1, m_TileSetManager, m_PaintManager, LayerStackPanel, int.Parse(WidthInput.Text), int.Parse(HeightInput.Text));
            CreateGridRoom(new object(), new RoutedEventArgs());

            m_SaveClass = new Save();

            m_PaintManager.Init(m_GridManager, m_TileSetManager);
        }

        public void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            Image img = (Image)sender;
            ImageSource source = img.Source;

            if (m_PaintManager.GetActivePaintMode() == PaintMode.Paint) {

                TileSetElement element = m_TileSetManager.GetTileSetElementFromImage(img);

                if (img != null) {
                    m_TileSetManager.m_CurrentSelectedTileSetElement = element;
                }
            }
        }

        private void CreateGridRoom(object sender, RoutedEventArgs e) {
            m_GridManager.GenerateGrid();
            m_GridManager.AddLayer(new object(), new RoutedEventArgs());
        }

        public void AddLayer(object sender, RoutedEventArgs e) {
            m_GridManager.AddLayer(sender, e);
        }

        private void SetPaintModePaint(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Paint);
        }

        private void SetPaintModeErase(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Erase);
        }
        
        private void SetPaintModeFill(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Fill);
        }

        private void ButtonExportDataClick(object sender, RoutedEventArgs e) {
            SaveFile file = new SaveFile();

            file.m_Name = "TEST";
            file.m_TilemapWidth = int.Parse(WidthInput.Text);
            file.m_TilemapHeigh = int.Parse(HeightInput.Text);

            for (int i = 0; i < m_GridManager.m_GridLayers.Count; i++) {
                List<int> addingList = new List<int>();
                file.m_GridLayers.Add(addingList);
                int count = 0;
                for (int j = 0; j < m_GridManager.m_GridLayers[i].m_GridElements.Count; j++) {
                    file.m_GridLayers[i].Add(m_GridManager.m_GridLayers[i].m_GridElements[j].m_ID);
                    count++;
                }

                if (count == 0) {
                    file.m_GridLayers.Remove(addingList);
                }
            }

            file.m_TileWidth = int.Parse(TileWidthInput.Text);
            file.m_TileHeight = int.Parse(TileHeightInput.Text);
            file.m_LoadedTilesetPath = m_TileSetManager.m_LoadedTilesetPath;

            m_SaveClass.SetSaveFile(file);

            m_SaveClass.ExportToJSON();
        }

        private void ButtonLoadTileSetFromDisk_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDialog = Files.OpenFileDialog(OpenExtensionType.TileSet);

            if (openDialog.ShowDialog() == true) {
                BitmapImage map = new BitmapImage(new Uri(openDialog.FileName, UriKind.Absolute));
                m_TileSetManager.LoadTileSet(openDialog.FileName, map, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(map));

                string systemDirectory = System.IO.Directory.GetCurrentDirectory();

                string fileName = System.IO.Path.GetFileName(openDialog.FileName);
                string fileExtension = System.IO.Path.GetExtension(openDialog.FileName);

                System.IO.Directory.CreateDirectory(systemDirectory + "/LocalTilesets/");

                using (var fileStream = new System.IO.FileStream(systemDirectory + "/LocalTilesets/" + fileName + fileExtension, System.IO.FileMode.Create)) {
                    encoder.Save(fileStream);
                }
            }
        }

        private void OnlyNumberInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonOpenDataClick(object sender, RoutedEventArgs e) {
            SaveFile file = m_SaveClass.LoadFromJSON();

            //Tilemap configurations
            WidthInput.Text = file.m_TilemapWidth.ToString();
            HeightInput.Text = file.m_TilemapHeigh.ToString();
            m_GridManager.GenerateGrid();

            //Tileset editor
            BitmapImage map = new BitmapImage(new Uri(file.m_LoadedTilesetPath, UriKind.Absolute));
            m_TileSetManager.LoadTileSet(file.m_LoadedTilesetPath, map, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));

            //Layer editor
            for (int i = 0; i < file.m_GridLayers.Count; i++) {
                m_GridManager.AddLayer(new object(), new RoutedEventArgs());
            }

            //Grid editor
            for (int i = 0; i < m_GridManager.m_GridLayers.Count; i++) {
                for (int j = 0; j < m_GridManager.m_GridLayers[i].m_GridElements.Count; j++) {
                    
                    double x = Canvas.GetLeft(m_GridManager.m_GridLayers[i].m_GridElements[j].m_Image);
                    double y = Canvas.GetTop(m_GridManager.m_GridLayers[i].m_GridElements[j].m_Image);

                    GridElement element = m_GridManager.GetGridElementOnPositionFromLayer(i, x, y);

                    m_GridManager.PaintGridElementBasedOnID(element, file.m_GridLayers[i][j]);
                }
            }
        }

        private void ButtonPortfolioClick(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("http://www.jenspetter.nl");
        }

        private void ButtonLinkedinClick(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/jens-petter-97a0a0a9");
        }

        private void ButtonNewTilemapClick(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("This opens a new fresh Tilemap, do you want to save your old Tilemap?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)  {
                ButtonExportDataClick(new object(), new RoutedEventArgs());
            }
            else {
                //OPEN NEW TILEMAP
            }
        }
    }
}
