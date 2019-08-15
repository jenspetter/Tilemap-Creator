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

namespace TilemapCreator {
    public class GridElement {
        public int m_GridID;
        public int m_GridLayer;
        public bool m_Visible;

        public double m_PositionX;
        public double m_PositionY;

        public int m_ID;
        public Image m_Image;
    }

    public class GridLayer {
        public string m_Name;
        public bool m_Visible;
        public List<GridElement> m_GridElements = new List<GridElement>();

        public GridLayer(string name) {
            m_Name = name;
            m_Visible = true;
        }
    }

    public class GridManager {
        private Canvas m_GridCanvas;
        private StackPanel m_GridLayerStackPanel;

        private PaintManager m_PaintManager;
        private TileSetManager m_TileSetManager;
        public List<GridLayer> m_GridLayers = new List<GridLayer>();

        public int m_LayerIndex;

        public int m_GridWidth;
        public int m_GridHeight;

        public GridManager(Canvas gridCanvas, TileSetManager tilesetManager, PaintManager paintManager, StackPanel stackPanel, int gridWidth, int gridHeight) {
            m_GridCanvas = gridCanvas;
            m_GridLayerStackPanel = stackPanel;
            m_TileSetManager = tilesetManager;
            m_PaintManager = paintManager;
            m_LayerIndex = -1;
            m_GridWidth = gridWidth;
            m_GridHeight = gridHeight;
        }

        public GridLayer AddGridLayer(GridLayer layer) {
            m_GridLayers.Add(layer);
            return layer;
        }

        public void SetVisibilityToLayer(int layerIndex, bool visible) {
            m_GridLayers[layerIndex].m_Visible = visible;

            for (int i = 0; i < m_GridLayers[layerIndex].m_GridElements.Count; i++) {
                if (m_GridLayers[layerIndex].m_GridElements[i].m_Visible) {
                    if (visible) {
                        m_GridLayers[layerIndex].m_GridElements[i].m_Image.Opacity = 0;
                    }
                    else {
                        m_GridLayers[layerIndex].m_GridElements[i].m_Image.Opacity = 1;
                    }
                }
            }
        }

        public GridElement GetGridElementInDirectionFromGridElement(GridElement elementToCountFrom, Direction direction) {
            int count = elementToCountFrom.m_GridID;
            bool canReturn = true;

            switch (direction) {
                case Direction.Up:
                    count = count - m_GridWidth;
                    break;
                case Direction.Right:
                    count = count + 1;

                    int yTimes = count / m_GridWidth;
                    int count2 = count - (yTimes * m_GridWidth);

                    if (count2 == 0) {
                        canReturn = false;
                    }

                    break;
                case Direction.Down:
                    count = count + m_GridWidth;
                    break;
                case Direction.Left:
                    count = count - 1;

                    int yTimes2 = count / m_GridWidth;
                    int count22 = count - (yTimes2 * m_GridWidth);

                    if (count22 == (m_GridWidth - 1)) {
                        canReturn = false;
                    }

                    break;
            }

            //Down or top extent of grid itself
            if (!canReturn || count < 0 || count > (m_GridLayers[elementToCountFrom.m_GridLayer].m_GridElements.Count - 1)) {
                return null;
            }

            

            return m_GridLayers[elementToCountFrom.m_GridLayer].m_GridElements[count];
        }

        public GridElement GetGridElementOnPositionFromLayer(int layer, double x, double y) {
            for (int i = 0; i < m_GridLayers[layer].m_GridElements.Count; i++) {
                if (m_GridLayers[layer].m_GridElements[i].m_PositionX == x &&
                    m_GridLayers[layer].m_GridElements[i].m_PositionY == y) {
                    return m_GridLayers[layer].m_GridElements[i];
                }
            }
            return null;
        }

        public GridElement AddGridElement(GridElement element) {
            m_GridLayers[m_LayerIndex].m_GridElements.Add(element);
            return element;
        }

        public UIElementCollection GetElements() {
            return m_GridCanvas.Children;
        }

        public void PaintGridElementBasedOnID(GridElement element, int id) {
            if (id == -1) {
                element.m_Image.Opacity = 0;
                element.m_ID = -1;
                element.m_Visible = false;
            }
            else {
                element.m_Image.Opacity = 1;
                element.m_Visible = true;
                element.m_ID = id;
                element.m_Image.Source = m_TileSetManager.GetTileSetElementFromID(id).m_CroppedImage;
            }
        }

        public void GenerateGrid() {
            BitmapImage noTileBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));

            if (m_GridCanvas.Children.Count > 0) {
                m_GridCanvas.Children.Clear();
            }
            if (m_GridLayers.Count > 0) {
                m_GridLayers.Clear();
            }
            if (m_GridLayerStackPanel.Children.Count > 0) {
                m_GridLayerStackPanel.Children.Clear();
            }

            double widthHeight = m_GridCanvas.Width / m_GridWidth;

            int xStep = -1;
            int yStep = 0;

            int size = m_GridWidth * m_GridHeight;

            for (int i = 0; i < size; i++) {
                Image img = new Image();
                img.Source = noTileBitmap;
                img.Stretch = Stretch.Fill;
                img.Width = widthHeight;
                img.Height = widthHeight;
                img.MouseEnter += new MouseEventHandler(m_PaintManager.Image_MouseEnter);
                img.MouseDown += new MouseButtonEventHandler(m_PaintManager.Image_MouseEnter);

                xStep++;

                if (xStep > m_GridWidth - 1) {
                    xStep = 0;
                    yStep++;
                }

                Canvas.SetLeft(img, 0 + (widthHeight * xStep));
                Canvas.SetTop(img, 0 + (widthHeight * yStep));

                m_GridCanvas.Children.Add(img);
            }

            m_LayerIndex = -1;
            m_GridLayers.Add(new GridLayer("Layer 1"));
        }

        public void AddLayer(object sender, RoutedEventArgs e) {
            m_LayerIndex++;

            GridLayer layer = new GridLayer("Layer " + m_LayerIndex.ToString());
            AddGridLayer(layer);

            double widthHeight = m_GridCanvas.Width / m_GridWidth;

            int xStep = -1;
            int yStep = 0;

            int size = m_GridWidth * m_GridHeight;

            for (int i = 0; i < size; i++) {
                GridElement element = new GridElement();

                element.m_GridID = i;
                element.m_ID = -1;
                element.m_GridLayer = m_LayerIndex;

                element.m_Image = new Image();
                element.m_Image.Stretch = Stretch.Fill;
                element.m_Image.Width = widthHeight;
                element.m_Image.Height = widthHeight;
                element.m_Image.MouseEnter += new MouseEventHandler(m_PaintManager.Image_MouseEnter);
                element.m_Image.MouseDown += new MouseButtonEventHandler(m_PaintManager.Image_MouseEnter);

                xStep++;

                if (xStep > m_GridWidth - 1) {
                    xStep = 0;
                    yStep++;
                }

                Canvas.SetLeft(element.m_Image, 0 + (widthHeight * xStep));
                Canvas.SetTop(element.m_Image, 0 + (widthHeight * yStep));

                m_GridCanvas.Children.Add(element.m_Image);

                element.m_PositionX = Canvas.GetLeft(element.m_Image);
                element.m_PositionY = Canvas.GetTop(element.m_Image);

                AddGridElement(element);
            }

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            Button layerButton = new Button();
            layerButton.Content = layer.m_Name;
            layerButton.Click += new RoutedEventHandler(ButtonLayerSelect);

            CheckBox layerCheckbox = new CheckBox();
            layerCheckbox.Content = "Visible";
            layerCheckbox.Click += new RoutedEventHandler(ButtonLayerVisibilityClick);

            sp.Children.Add(layerButton);
            sp.Children.Add(layerCheckbox);
            m_GridLayerStackPanel.Children.Add(sp);
        }

        private void ButtonLayerSelect(object sender, RoutedEventArgs e) {
            Button senderButton = (Button)sender;
            int buttonIndexInStackPanel = 0;

            for (int i = 0; i < m_GridLayerStackPanel.Children.Count; i++) {
                StackPanel LayerStackPanelChild = (StackPanel)m_GridLayerStackPanel.Children[i];

                for (int j = 0; j < LayerStackPanelChild.Children.Count; j++) {
                    Button childButton = (Button)LayerStackPanelChild.Children[i];

                    if (childButton != null && childButton == senderButton) {
                        buttonIndexInStackPanel = i;
                    }
                }
            }

            m_LayerIndex = buttonIndexInStackPanel;
        } 

        private void ButtonLayerVisibilityClick(object sender, RoutedEventArgs e) {
            CheckBox senderButton = (CheckBox)sender;
            int buttonIndexInStackPanel = 0;

            for (int i = 0; i < m_GridLayerStackPanel.Children.Count; i++) {
                StackPanel LayerStackPanelChild = (StackPanel)m_GridLayerStackPanel.Children[i];

                for (int j = 0; j < LayerStackPanelChild.Children.Count; j++) {
                    CheckBox childButton = (CheckBox)LayerStackPanelChild.Children[i];

                    if (childButton != null && childButton == senderButton) {
                        buttonIndexInStackPanel = i;
                    }
                }
            }

            SetVisibilityToLayer(buttonIndexInStackPanel, !m_GridLayers[buttonIndexInStackPanel].m_Visible);
        }
    }
}
