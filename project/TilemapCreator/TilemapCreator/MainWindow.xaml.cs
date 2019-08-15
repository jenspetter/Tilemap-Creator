using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace TilemapCreator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        ///All manager and upper classes needed for this program to function properly
        private PaintManager m_PaintManager;
        private GridManager m_GridManager;
        private TileSetManager m_TileSetManager;
        private Save m_SaveClass;
        
        /// <summary>
        /// Main constructor of program
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            ///Setting all manager and upper classes to be a new instance
            m_PaintManager = new PaintManager();
            m_TileSetManager = new TileSetManager();
            m_GridManager = new GridManager();
            m_SaveClass = new Save();

            ///Initializing all manager and upper classes
            m_PaintManager.Init(m_GridManager, m_TileSetManager);
            m_TileSetManager.Init(TileSetCanvas);
            m_GridManager.Init(canvas1, m_TileSetManager, 
                m_PaintManager, LayerStackPanel, 
                int.Parse(WidthInput.Text), int.Parse(HeightInput.Text
                ));

            //Calling click create room function to make a room by default when program starts up
            CreateGridRoom(new object(), new RoutedEventArgs());
        }

        #region Grid UI functions

        /// <summary>
        /// Creates grid and also adds first layer to the grid
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        private void CreateGridRoom(object sender, RoutedEventArgs e) {
            m_GridManager.GenerateGrid();
            m_GridManager.AddLayer();
        }

        /// <summary>
        /// Adds layer to the grid
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        public void AddLayer(object sender, RoutedEventArgs e) {
            m_GridManager.AddLayer();
        }

        #endregion

        #region Paint mode setting UI functions

        /// <summary>
        /// Sets paint mode to paint mode paint
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        private void SetPaintModePaint(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Paint);
        }

        /// <summary>
        /// Sets paint mode to paint mode erase
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        private void SetPaintModeErase(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Erase);
        }

        /// <summary>
        /// Sets paint mode to paint mode fill
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        private void SetPaintModeFill(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.Fill);
        }

        /// <summary>
        /// Sets paint mode to paint mode delete fill
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event arguments</param>
        private void SetPaintModeDeleteFill(object sender, RoutedEventArgs e) {
            m_PaintManager.SetNewPaintMode(PaintMode.DeleteFill);
        }

        #endregion

        /// <summary>
        /// UI button function to export a Tilemap
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
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

        /// <summary>
        /// UI button function to load a Tilemap from disk
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
        private void ButtonLoadTileSetFromDisk_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDialog = Files.OpenFileDialog(OpenExtensionType.TileSet);

            if (openDialog.ShowDialog() == true) {
                m_TileSetManager.LoadTileSet(openDialog.FileName, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));
            }
        }

        /// <summary>
        /// UI restricting to only number input
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Text composition event arguments</param>
        private void OnlyNumberInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// UI button function to open a Tilemap
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
        private void ButtonOpenDataClick(object sender, RoutedEventArgs e) {
            SaveFile file = m_SaveClass.LoadFromJSON();

            //Tilemap configurations
            WidthInput.Text = file.m_TilemapWidth.ToString();
            HeightInput.Text = file.m_TilemapHeigh.ToString();
            m_GridManager.GenerateGrid();

            //Tileset editor
            m_TileSetManager.LoadTileSet(file.m_LoadedTilesetPath, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));

            //Layer editor
            for (int i = 0; i < file.m_GridLayers.Count; i++) {
                m_GridManager.AddLayer();
                Console.WriteLine("ADDING LAYER");
            }

            //Grid editor
            for (int i = 0; i < m_GridManager.m_GridLayers.Count; i++) {
                for (int j = 0; j < m_GridManager.m_GridLayers[i].m_GridElements.Count; j++) {
                    
                    double x = Canvas.GetLeft(m_GridManager.m_GridLayers[i].m_GridElements[j].m_Image);
                    double y = Canvas.GetTop(m_GridManager.m_GridLayers[i].m_GridElements[j].m_Image);

                    GridElement element = m_GridManager.GetGridElementOnPositionFromLayer(i, x, y);

                    m_PaintManager.ColorGridElementBasedOnID(element, file.m_GridLayers[i][j]);
                }
            }
        }

        /// <summary>
        /// UI Button function that opens a portfolio link
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
        private void ButtonPortfolioClick(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("http://www.jenspetter.nl");
        }

        /// <summary>
        /// UI Button function that opens a portfolio link
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
        private void ButtonLinkedinClick(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/jens-petter-97a0a0a9");
        }

        /// <summary>
        /// UI button function to create a new Tilemap
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Routed event arguments</param>
        private void ButtonNewTilemapClick(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("This opens a new fresh Tilemap, do you want to save your old Tilemap?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //Depending on the user choosing to build a new Tilemap, do different functionality
            if (result == MessageBoxResult.Yes)  {
                ButtonExportDataClick(new object(), new RoutedEventArgs());
            }
            else {
                m_GridManager.GenerateGrid();
            }
        }
    }
}
