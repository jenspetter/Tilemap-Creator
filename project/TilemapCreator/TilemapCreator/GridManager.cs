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
        public List<GridLayer> m_GridLayers = new List<GridLayer>();

        public int m_LayerIndex;

        public int m_GridWidth;
        public int m_GridHeight;

        public GridManager(Canvas gridCanvas, int gridWidth, int gridHeight) {
            m_GridCanvas = gridCanvas;
            m_LayerIndex = -1;
            m_GridWidth = gridWidth;
            m_GridHeight = gridHeight;

            m_GridLayers.Add(new GridLayer("Layer 1"));
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

        public void CreateGrid(int width, int height) {
            if (m_GridCanvas.Children.Count > 0) {
                m_GridCanvas.Children.Clear();
            }

            double widthHeight = m_GridCanvas.Width / width;

            int xStep = -1;
            int yStep = 0;

            int size = width * height;

            Image[] carImg = new Image[size];

            for (int i = 0; i < carImg.Length; i++) {
                carImg[i] = new Image();
                carImg[i].Stretch = Stretch.Fill;
                carImg[i].Width = widthHeight;
                carImg[i].Height = widthHeight;

                xStep++;

                if (xStep > (width - 1)) {
                    xStep = 0;
                    yStep++;
                }

                Canvas.SetLeft(carImg[i], 0 + (widthHeight * xStep));
                Canvas.SetTop(carImg[i], 0 + (widthHeight * yStep));
                m_GridCanvas.Children.Add(carImg[i]);
            }
        }
    }
}
