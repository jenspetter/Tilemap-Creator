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

        BitmapImage carBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));
        CroppedBitmap croppedBitmap;
        TileSetElement currentActiveTilesetElement;

        public PaintMode m_PaintMode = PaintMode.Paint;

        public MainWindow() {
            InitializeComponent();

            m_GridManager = new GridManager(canvas1, int.Parse(WidthInput.Text), int.Parse(HeightInput.Text));
            CreateGridRoom(new object(), new RoutedEventArgs());

            m_TileSetManager = new TileSetManager(TileSetCanvas, this);

            AddLayer(new object(), new RoutedEventArgs());
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
                        element.m_Image.Source = croppedBitmap;
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
            element.m_Image.Source = croppedBitmap;

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
            CroppedBitmap bp = (CroppedBitmap)img.Source;

            if (m_PaintMode == PaintMode.Paint) {
                croppedBitmap = bp;

                TileSetElement element = m_TileSetManager.GetTileSetElementFromImage(img);

                if (img != null) {
                    currentActiveTilesetElement = element;
                }
            }
        }

        private void CreateGridRoom(object sender, RoutedEventArgs e) {
            carBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));

            if (canvas1.Children.Count > 0) {
                canvas1.Children.Clear();
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
            Save save = new Save();
            SaveFile file = new SaveFile();

            file.m_Name = "TEST";
            file.m_TilemapWidth = int.Parse(WidthInput.Text);
            file.m_TilemapHeigh = int.Parse(HeightInput.Text);

            for (int i = 0; i < m_GridManager.m_GridLayers.Count; i++) {
                file.m_GridLayers.Add(new List<int>());
                for (int j = 0; j < m_GridManager.m_GridLayers[i].m_GridElements.Count; j++) {
                    file.m_GridLayers[i].Add(m_GridManager.m_GridLayers[i].m_GridElements[j].m_ID);
                }
            }

            file.m_TileWidth = int.Parse(TileWidthInput.Text);
            file.m_TileHeight = int.Parse(TileHeightInput.Text);

            save.SetSaveFile(file);

            save.ExportToJSON();
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
                m_TileSetManager.LoadTileSet(map, int.Parse(TileWidthInput.Text), int.Parse(TileHeightInput.Text));
            }
        }
    }
}
