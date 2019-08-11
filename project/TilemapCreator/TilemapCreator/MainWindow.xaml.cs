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

public enum PaintMode {
    Paint,
    Fill,
    Erase
}

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
        public GridManager m_GridManager;
        public TileSetManager m_TileSetManager;
        public Save m_SaveClass;

        BitmapImage carBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));
        TileSetElement currentActiveTilesetElement;

        public PaintMode m_PaintMode = PaintMode.Paint;

        public MainWindow() {
            InitializeComponent();

            m_TileSetManager = new TileSetManager(TileSetCanvas, this);
            m_GridManager = new GridManager(canvas1, m_TileSetManager, int.Parse(WidthInput.Text), int.Parse(HeightInput.Text));
            CreateGridRoom(new object(), new RoutedEventArgs());

            m_SaveClass = new Save();
        }

        void Image_MouseEnter(object sender, MouseEventArgs e) {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;

            if (mouseIsDown) {
                Image img = (Image)sender;

                double x = Canvas.GetLeft(img);
                double y = Canvas.GetTop(img);

                GridElement element = m_GridManager.GetGridElementOnPositionFromLayer(m_GridManager.m_LayerIndex, x, y);
                
                if (img != null) {
                    if (m_PaintMode == PaintMode.Erase) {
                        if (element != null) {
                            element.m_Image.Opacity = 0;
                            element.m_ID = -1;
                            element.m_Visible = false;
                            return;
                        }                        
                    }
                    else if (m_PaintMode == PaintMode.Paint) {
                        element.m_Image.Opacity = 1;
                        element.m_Visible = true;
                        element.m_ID = currentActiveTilesetElement.m_ID;
                        element.m_Image.Source = currentActiveTilesetElement.m_CroppedImage;
                    }
                    else if (m_PaintMode == PaintMode.Fill) {
                        ColorGridElement(element);
                    }
                }
            }   
        }

        private void ColorGridElement(GridElement element) {
            element.m_Image.Opacity = 1;
            element.m_Visible = true;
            element.m_ID = currentActiveTilesetElement.m_ID;
            element.m_Image.Source = currentActiveTilesetElement.m_CroppedImage;

            SetFillElements(element);
        }

        private void SetFillElements(GridElement element) {
            GridElement upElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Up);

            if (upElement != null) {
                if (upElement.m_ID == -1) {
                    ColorGridElement(upElement);
                }
            }
            
            GridElement rightElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Right);

            if (rightElement != null) {
                if (rightElement.m_ID == -1) {
                    ColorGridElement(rightElement);
                }
            }
            
            GridElement downElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Down);

            if (downElement != null) {
                if (downElement.m_ID == -1) {
                    ColorGridElement(downElement);
                }
            }
            
            GridElement leftElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Left);

            if (leftElement != null) {
                if (leftElement.m_ID == -1) {
                    ColorGridElement(leftElement);
                }
            }
            
        }

        public void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            Image img = (Image)sender;
            ImageSource source = img.Source;

            if (m_PaintMode == PaintMode.Paint) {

                TileSetElement element = m_TileSetManager.GetTileSetElementFromImage(img);

                if (img != null) {
                    currentActiveTilesetElement = element;
                }
            }
        }

        private void CreateGridRoom(object sender, RoutedEventArgs e) {
            GenerateGrid();
            AddLayer(new object(), new RoutedEventArgs());
        }

        private void GenerateGrid() {
            carBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));

            if (canvas1.Children.Count > 0) {
                canvas1.Children.Clear();
            }
            if (m_GridManager.m_GridLayers.Count > 0) {
                m_GridManager.m_GridLayers.Clear();
            }
            if (LayerStackPanel.Children.Count > 0) {
                LayerStackPanel.Children.Clear();
            }

            double widthHeight = canvas1.Width / int.Parse(WidthInput.Text);

            int xStep = -1;
            int yStep = 0;

            int size = int.Parse(WidthInput.Text) * int.Parse(HeightInput.Text);

            for (int i = 0; i < size; i++) {
                Image img = new Image();
                img.Source = carBitmap;
                img.Stretch = Stretch.Fill;
                img.Width = widthHeight;
                img.Height = widthHeight;
                img.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                img.MouseDown += new MouseButtonEventHandler(Image_MouseEnter);

                xStep++;

                if (xStep > (int.Parse(WidthInput.Text) - 1)) {
                    xStep = 0;
                    yStep++;
                }

                Canvas.SetLeft(img, 0 + (widthHeight * xStep));
                Canvas.SetTop(img, 0 + (widthHeight * yStep));

                canvas1.Children.Add(img);
            }

            m_GridManager.m_LayerIndex = -1;
            m_GridManager.m_GridLayers.Add(new GridLayer("Layer 1"));
        }

        private void SetPaintModePaint(object sender, RoutedEventArgs e) {
            m_PaintMode = PaintMode.Paint;
        }

        private void SetPaintModeErase(object sender, RoutedEventArgs e) {
            m_PaintMode = PaintMode.Erase;
        }
        
        private void SetPaintModeFill(object sender, RoutedEventArgs e) {
            m_PaintMode = PaintMode.Fill;
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

        private void AddLayer(object sender, RoutedEventArgs e) {
            m_GridManager.m_LayerIndex++;

            GridLayer layer = new GridLayer("Layer " + m_GridManager.m_LayerIndex.ToString());
            m_GridManager.AddGridLayer(layer);

            double widthHeight = canvas1.Width / int.Parse(WidthInput.Text);

            int xStep = -1;
            int yStep = 0;

            int size = int.Parse(WidthInput.Text) * int.Parse(HeightInput.Text);

            for (int i = 0; i < size; i++) {
                GridElement element = new GridElement();

                element.m_GridID = i;
                element.m_ID = -1;
                element.m_GridLayer = m_GridManager.m_LayerIndex;

                element.m_Image = new Image();
                element.m_Image.Stretch = Stretch.Fill;
                element.m_Image.Width = widthHeight;
                element.m_Image.Height = widthHeight;
                element.m_Image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                element.m_Image.MouseDown += new MouseButtonEventHandler(Image_MouseEnter);

                xStep++;

                if (xStep > (int.Parse(WidthInput.Text) - 1)) {
                    xStep = 0;
                    yStep++;
                }

                Canvas.SetLeft(element.m_Image, 0 + (widthHeight * xStep));
                Canvas.SetTop(element.m_Image, 0 + (widthHeight * yStep));

                canvas1.Children.Add(element.m_Image);

                element.m_PositionX = Canvas.GetLeft(element.m_Image);
                element.m_PositionY = Canvas.GetTop(element.m_Image);

                m_GridManager.AddGridElement(element);
            }

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;            

            Button layerButton = new Button();
            layerButton.Content = layer.m_Name;
            layerButton.Click += new RoutedEventHandler(ButtonLayerSelect);

            Button layerVisibilityButton = new Button();
            layerVisibilityButton.Content = "Visible";
            layerVisibilityButton.Click += new RoutedEventHandler(ButtonLayerVisibilityClick);

            sp.Children.Add(layerButton);
            sp.Children.Add(layerVisibilityButton);
            LayerStackPanel.Children.Add(sp);
        }

        private void ButtonLayerSelect(object sender, RoutedEventArgs e) {
            Button senderButton = (Button)sender;
            int buttonIndexInStackPanel = 0;

            for (int i = 0; i < LayerStackPanel.Children.Count; i++) {
                StackPanel LayerStackPanelChild = (StackPanel)LayerStackPanel.Children[i];

                for (int j = 0; j < LayerStackPanelChild.Children.Count; j++) {
                    Button childButton = (Button)LayerStackPanelChild.Children[i];

                    if (childButton != null && childButton == senderButton) {
                        buttonIndexInStackPanel = i;
                    }
                }
            }

            m_GridManager.m_LayerIndex = buttonIndexInStackPanel;
        } 

        private void ButtonLayerVisibilityClick(object sender, RoutedEventArgs e) {
            Button senderButton = (Button)sender;
            int buttonIndexInStackPanel = 0;

            for (int i = 0; i < LayerStackPanel.Children.Count; i++) {
                StackPanel LayerStackPanelChild = (StackPanel)LayerStackPanel.Children[i];

                for (int j = 0; j < LayerStackPanelChild.Children.Count; j++) {
                    Button childButton = (Button)LayerStackPanelChild.Children[i];

                    if (childButton != null && childButton == senderButton) {
                        buttonIndexInStackPanel = i;
                    }
                }
            }

            m_GridManager.SetVisibilityToLayer(buttonIndexInStackPanel, !m_GridManager.m_GridLayers[buttonIndexInStackPanel].m_Visible);
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
            GenerateGrid();

            //Tileset editor
            BitmapImage map = new BitmapImage(new Uri(file.m_LoadedTilesetPath, UriKind.Absolute));
            m_TileSetManager.LoadTileSet(file.m_LoadedTilesetPath, map, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));

            //Layer editor
            for (int i = 0; i < file.m_GridLayers.Count; i++) {
                AddLayer(new object(), new RoutedEventArgs());
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
