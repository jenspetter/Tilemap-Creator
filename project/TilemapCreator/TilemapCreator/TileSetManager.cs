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
    public enum TileSetElementType {
        None,
        Wall,
        Ground,
        DoorwayUp,
        DoorwayRight,
        DoorwayDown,
        DoorwayLeft
    }

    public class TileSetElement {
        public int m_ID;
        public TileSetElementType m_Type;
        public int m_ElementTypeID;
        public Image m_Image;

        public TileSetElement() {
            m_Type = TileSetElementType.None;

            int typeIndex = (int)m_Type;
            m_ElementTypeID = typeIndex;
        }
    }

    public class TileSetManager {
        private Canvas m_TileSetCanvas;
        private MainWindow m_MainWindow;
        private List<TileSetElement> m_TileSetElements = new List<TileSetElement>();

        private Image m_CurrentSelectedTileSetElement;

        public TileSetManager(Canvas tilesetCanvas, MainWindow mainwindow) {
            //Setting up base variables
            m_TileSetCanvas = tilesetCanvas;
            m_MainWindow = mainwindow;
        }

        public TileSetElement GetTileSetElementFromImage(Image image) {
            for (int i = 0; i < m_TileSetElements.Count; i++) {
                if (m_TileSetElements[i].m_Image == image) {
                    return m_TileSetElements[i];
                }
            }
            return null;
        }

        public void LoadTileSet(BitmapImage image, int tileWidth, int tileHeight) {
            double xAmountOfTile = image.PixelWidth / tileWidth;
            double yAmountOfTile = image.PixelHeight / tileHeight;

            double widthImageInCanvas = m_TileSetCanvas.Width / xAmountOfTile;
            double heightImageInCanvas = m_TileSetCanvas.Height / yAmountOfTile;

            int xPositionIndex = -1;
            int yPositionIndex = 0;

            for (int i = 0; i < xAmountOfTile * yAmountOfTile; i++) {
                Image img = new Image();
                img.Width = widthImageInCanvas;
                img.Height = widthImageInCanvas;
                img.MouseDown += new MouseButtonEventHandler(m_MainWindow.Image_MouseDown);
                img.MouseDown += new MouseButtonEventHandler(Image_MouseDown);

                xPositionIndex++;

                if (xPositionIndex > xAmountOfTile - 1) {
                    xPositionIndex = 0;
                    yPositionIndex++;
                }

                CroppedBitmap cb = new CroppedBitmap(image, 
                    new Int32Rect(0 + (tileWidth * xPositionIndex), 0 + (tileHeight * yPositionIndex), tileWidth, tileHeight));
                img.Source = cb;

                Canvas.SetLeft(img, 0 + (widthImageInCanvas * xPositionIndex));
                Canvas.SetTop(img, 0 + (widthImageInCanvas * yPositionIndex));

                m_TileSetCanvas.Children.Add(img);

                TileSetElement tileElement = new TileSetElement();
                tileElement.m_ID = i;
                tileElement.m_Image = img;
                m_TileSetElements.Add(tileElement);
            }

            Border b = new Border();
            b.BorderThickness = new Thickness(1);
            b.BorderBrush = Brushes.Black;
            b.Width = widthImageInCanvas;
            b.Height = widthImageInCanvas;
            Canvas.SetLeft(b, 0);
            Canvas.SetTop(b, 0);
            m_TileSetCanvas.Children.Add(b);
        }

        public void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            Image img = (Image)sender;
            Border b = (Border)m_TileSetCanvas.Children[m_TileSetCanvas.Children.Count - 1];

            double imgLeft = Canvas.GetLeft(img);
            double imgTop = Canvas.GetTop(img);

            Canvas.SetLeft(b, imgLeft);
            Canvas.SetTop(b, imgTop);

            m_CurrentSelectedTileSetElement = img;
        }
    }
}
