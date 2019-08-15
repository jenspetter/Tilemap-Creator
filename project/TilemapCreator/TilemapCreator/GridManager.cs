using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TilemapCreator {
    /// <summary>
    /// Direction enum
    /// </summary>
    public enum Direction {
        Up,
        Right,
        Down,
        Left
    }

    /// <summary>
    /// Grid element class
    /// </summary>
    public class GridElement {
        public int m_GridID;
        public int m_GridLayer;
        public bool m_Visible;

        public double m_PositionX;
        public double m_PositionY;

        public int m_ID;
        public Image m_Image;

        public GridElement() {
            m_Visible = true;
        }
    }

    /// <summary>
    /// Grid layer class
    /// </summary>
    public class GridLayer {
        public string m_Name;
        public bool m_Visible;
        public List<GridElement> m_GridElements = new List<GridElement>();

        public GridLayer(string name) {
            m_Name = name;
            m_Visible = true;
        }
    }

    /// <summary>
    /// Manages all the functionality happening on the drawing grid directly
    /// </summary>
    public class GridManager {
        ///References to various important UI elements
        private Canvas m_GridCanvas;
        private StackPanel m_GridLayerStackPanel;

        ///References to other important managers or upper classes
        private PaintManager m_PaintManager;
        private TileSetManager m_TileSetManager;

        ///All grid layers
        public List<GridLayer> m_GridLayers = new List<GridLayer>();
        public int m_LayerIndex;

        ///Grid width and height
        public int m_GridWidth;
        public int m_GridHeight;

        //All grid layer select items
        private List<StackPanel> m_LayerSelectStackPanels = new List<StackPanel>();
        //All grid layer visibility items
        private List<Image> m_LayerVisibilityImages = new List<Image>();
        //Current active layer stack panel
        private StackPanel m_CurrentActiveLayerStackPanel = new StackPanel();

        /// <summary>
        /// Init function for this GridManager class
        /// </summary>
        /// <param name="gridCanvas">Reference to grid canvas to set</param>
        /// <param name="tilesetManager">Reference to the TilesetManager to set</param>
        /// <param name="paintManager">Reference to the PaintManager to set</param>
        /// <param name="stackPanel">Reference to the grid layer stack panel to set</param>
        /// <param name="gridWidth">Grid canvas width to set</param>
        /// <param name="gridHeight">Grid canvas height to set</param>
        public void Init(Canvas gridCanvas, TileSetManager tilesetManager, PaintManager paintManager, StackPanel stackPanel, int gridWidth, int gridHeight) {
            m_GridCanvas = gridCanvas;
            m_GridLayerStackPanel = stackPanel;
            m_TileSetManager = tilesetManager;
            m_PaintManager = paintManager;
            m_LayerIndex = -1;
            m_GridWidth = gridWidth;
            m_GridHeight = gridHeight;
        }

        /// <summary>
        /// Adds a grid layer
        /// </summary>
        /// <param name="layer">Name for the GridLayer to set</param>
        /// <returns></returns>
        public GridLayer AddGridLayer(GridLayer layer) {
            m_GridLayers.Add(layer);
            return layer;
        }

        /// <summary>
        /// Sets the visibility of a layer
        /// </summary>
        /// <param name="layerIndex">Layer index to set</param>
        /// <param name="visible">Visibility bool</param>
        public void SetVisibilityToLayer(int layerIndex, bool visible) {
            m_GridLayers[layerIndex].m_Visible = visible;

            ///The 2 bitmaps for visually representing if a layer is visible or not
            BitmapImage layerVisibleBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/LayerVisible.png", UriKind.Absolute));
            BitmapImage layerNotVisibleBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/LayerNotVisible.png", UriKind.Absolute));

            if (visible) {
                m_LayerVisibilityImages[layerIndex].Source = layerVisibleBitmap;
            }
            else {
                m_LayerVisibilityImages[layerIndex].Source = layerNotVisibleBitmap;
            }

            //Based on the give visiblity bool,
            //set different opacities of an image in the asked layer to change visibility of
            for (int i = 0; i < m_GridLayers[layerIndex].m_GridElements.Count; i++) {
                if (m_GridLayers[layerIndex].m_GridElements[i].m_Visible) {
                    if (visible) {
                        m_GridLayers[layerIndex].m_GridElements[i].m_Image.Opacity = 1;
                    }
                    else {
                        m_GridLayers[layerIndex].m_GridElements[i].m_Image.Opacity = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a GridElemeent in a direction from another grid element
        /// </summary>
        /// <param name="elementToCountFrom">Grid element to count from</param>
        /// <param name="direction">Direction to look in</param>
        /// <returns></returns>
        public GridElement GetGridElementInDirectionFromGridElement(GridElement elementToCountFrom, Direction direction) {
            int count = elementToCountFrom.m_GridID;
            bool canReturn = true;

            //Based on the give GridElement ID and grid dimensions we try to find the needed Grid Element
            switch (direction) {
                //Up
                case Direction.Up:
                    count = count - m_GridWidth;
                    break;

                //Right
                case Direction.Right:
                    count = count + 1;

                    int yTimes = count / m_GridWidth;
                    int count2 = count - (yTimes * m_GridWidth);

                    if (count2 == 0) {
                        canReturn = false;
                    }

                    break;

                //Down
                case Direction.Down:
                    count = count + m_GridWidth;
                    break;

                //Left
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

        /// <summary>
        /// Get a grid element in a layer based on a position
        /// </summary>
        /// <param name="layer">Layer to search Grid Element in</param>
        /// <param name="x">X location to look for</param>
        /// <param name="y">Y location to look for</param>
        /// <returns></returns>
        public GridElement GetGridElementOnPositionFromLayer(int layer, double x, double y) {
            for (int i = 0; i < m_GridLayers[layer].m_GridElements.Count; i++) {
                if (m_GridLayers[layer].m_GridElements[i].m_PositionX == x &&
                    m_GridLayers[layer].m_GridElements[i].m_PositionY == y) {
                    return m_GridLayers[layer].m_GridElements[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Adds Grid Element to active layer
        /// </summary>
        /// <param name="element">Grid Element to add</param>
        /// <returns></returns>
        public GridElement AddGridElement(GridElement element) {
            m_GridLayers[m_LayerIndex].m_GridElements.Add(element);
            return element;
        }

        /// <summary>
        /// Returns all canvas grid children
        /// </summary>
        /// <returns>ALl canvas grid children</returns>
        public UIElementCollection GetElements() {
            return m_GridCanvas.Children;
        }

        public void GenerateGrid() {
            //When needed resetting grid
            if (m_GridCanvas.Children.Count > 0) {
                m_GridCanvas.Children.Clear();
            }
            if (m_GridLayers.Count > 0) {
                m_GridLayers.Clear();
            }
            if (m_GridLayerStackPanel.Children.Count > 0) {
                m_GridLayerStackPanel.Children.Clear();
            }

            m_TileSetManager.ResetTileSet();
            m_LayerSelectStackPanels.Clear();
            m_LayerVisibilityImages.Clear();

            double widthHeight = m_GridCanvas.Width / m_GridWidth;

            //Loop indexes
            int xStep = -1;
            int yStep = 0;

            int size = m_GridWidth * m_GridHeight;

            //Bitmap image for base layer
            BitmapImage noTileBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/NoTile.png", UriKind.Absolute));

            for (int i = 0; i < size; i++) {
                //Creating and defining grid image
                Image img = new Image();
                img.Source = noTileBitmap;
                img.Stretch = Stretch.Fill;
                img.Width = widthHeight;
                img.Height = widthHeight;
                img.MouseEnter += new MouseEventHandler(m_PaintManager.GridElementMouseOver);
                img.MouseDown += new MouseButtonEventHandler(m_PaintManager.GridElementMouseOver);

                xStep++;

                //Handling of correct spacing of loop indexes
                if (xStep > m_GridWidth - 1) {
                    xStep = 0;
                    yStep++;
                }

                //Setting location of image
                Canvas.SetLeft(img, 0 + (widthHeight * xStep));
                Canvas.SetTop(img, 0 + (widthHeight * yStep));

                m_GridCanvas.Children.Add(img);
            }

            m_LayerIndex = -1;
        }

        /// <summary>
        /// Adds layer to Grid canvas
        /// </summary>
        public void AddLayer() {
            m_LayerIndex++;

            ///Creating layer
            GridLayer layer = new GridLayer("Layer " + m_LayerIndex.ToString());
            AddGridLayer(layer);

            double widthHeight = m_GridCanvas.Width / m_GridWidth;

            //Loop indexes
            int xStep = -1;
            int yStep = 0;

            int size = m_GridWidth * m_GridHeight;

            for (int i = 0; i < size; i++) {
                //Creation of grid elememnt
                GridElement element = new GridElement();

                //Setting of various grid element variables
                element.m_GridID = i;
                element.m_ID = -1;
                element.m_GridLayer = m_LayerIndex;

                element.m_Image = new Image();
                element.m_Image.Stretch = Stretch.Fill;
                element.m_Image.Width = widthHeight;
                element.m_Image.Height = widthHeight;
                element.m_Image.MouseEnter += new MouseEventHandler(m_PaintManager.GridElementMouseOver);
                element.m_Image.MouseDown += new MouseButtonEventHandler(m_PaintManager.GridElementMouseOver);

                xStep++;

                //Handling of correct spacing of loop indexes
                if (xStep > m_GridWidth - 1) {
                    xStep = 0;
                    yStep++;
                }

                //Setting image position in grid canvas
                Canvas.SetLeft(element.m_Image, 0 + (widthHeight * xStep));
                Canvas.SetTop(element.m_Image, 0 + (widthHeight * yStep));

                m_GridCanvas.Children.Add(element.m_Image);

                //Grid element position
                element.m_PositionX = Canvas.GetLeft(element.m_Image);
                element.m_PositionY = Canvas.GetTop(element.m_Image);

                AddGridElement(element);
            }

            //Setting up stack panel
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.MouseDown += new MouseButtonEventHandler(ButtonLayerSelect);
            m_LayerSelectStackPanels.Add(sp);

            //Setting up layer label
            Label layerButton = new Label();
            layerButton.Content = layer.m_Name;
           
            //Setting up layer visibility image
            Image layerVisibleImage = new Image();
            BitmapImage layerVisibleBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/LayerVisible.png", UriKind.Absolute));
            layerVisibleImage.Width = 15;
            layerVisibleImage.Height = 15;
            layerVisibleImage.Source = layerVisibleBitmap;
            layerVisibleImage.Stretch = Stretch.Fill;
            layerVisibleImage.MouseDown += new MouseButtonEventHandler(ButtonLayerVisibilityClick);
            m_LayerVisibilityImages.Add(layerVisibleImage);

            //Adding to stackpanel
            sp.Children.Add(layerButton);
            sp.Children.Add(layerVisibleImage);
            m_GridLayerStackPanel.Children.Add(sp);

            VisuallyActivateGridLayer(m_LayerIndex);
        }

        /// <summary>
        /// UI functions that sets the active layer index to be a specific number
        /// </summary>
        /// <param name="sender">Sender obj</param>
        /// <param name="e">Mouse event arguments</param>
        private void ButtonLayerSelect(object sender, MouseEventArgs e) {
            StackPanel senderButton = (StackPanel)sender;
            int chainedLayerIndex = 0;

            //Getting clicked button in the list of select layer buttons
            for (int i = 0; i < m_LayerSelectStackPanels.Count; i++) {
                if (m_LayerSelectStackPanels[i] == senderButton) {
                    chainedLayerIndex = i;
                }
            }

            m_LayerIndex = chainedLayerIndex;
            VisuallyActivateGridLayer(m_LayerIndex);
        } 

        /// <summary>
        /// UI function that sets the visibility of a specific layer
        /// </summary>
        /// <param name="sender">sender obj</param>
        /// <param name="e">Mouse button event arguments</param>
        private void ButtonLayerVisibilityClick(object sender, MouseButtonEventArgs e) {
            Image senderCheckbox = (Image)sender;
            int chainedLayerIndex = 0;

            //Getting clicked checkbox in the list of visiblity checkboxes
            for (int i = 0; i < m_LayerVisibilityImages.Count; i++) {
                if (m_LayerVisibilityImages[i] == senderCheckbox) {
                    chainedLayerIndex = i;
                }
            }

            SetVisibilityToLayer(chainedLayerIndex, !m_GridLayers[chainedLayerIndex].m_Visible);
        }

        /// <summary>
        /// Sets new active layer stack panel and makes sure to visually represents that a certain layer is the new active layer
        /// </summary>
        /// <param name="layerIndex">index on which layer to set as the new active layer</param>
        private void VisuallyActivateGridLayer(int layerIndex) {
            m_CurrentActiveLayerStackPanel.Background = Brushes.Transparent;
            m_CurrentActiveLayerStackPanel = m_LayerSelectStackPanels[layerIndex];
            m_CurrentActiveLayerStackPanel.Background = Brushes.LightBlue;
        }
    }
}
