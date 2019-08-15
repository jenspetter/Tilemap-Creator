using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TilemapCreator {

    /// <summary>
    /// TileSetElement class
    /// </summary>
    public class TilesetElement {
        public int m_ID;
        public Image m_Image;
        public CroppedBitmap m_CroppedImage;

        /// <summary>
        /// TilesetElement constructor
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="img">Image</param>
        /// <param name="croppedImg">CroppedImage</param>
        public TilesetElement(int id, Image img, CroppedBitmap croppedImg) {
            m_ID = id;
            m_Image = img;
            m_CroppedImage = croppedImg;
        }
    }

    /// <summary>
    /// Main Tileset manager class which handles all the Tileset functionality
    /// </summary>
    public class TileSetManager {
        //Tileset canvas reference
        private Canvas m_TileSetCanvas;

        //List of all Tileset elements
        private List<TilesetElement> m_TileSetElements = new List<TilesetElement>();

        //Current selected TilesetElement
        public TilesetElement m_CurrentSelectedTileSetElement;
        //Path to the loaded TilesetElement
        public string m_LoadedTilesetPath;

        /// <summary>
        /// Init function of TileSetManager class
        /// </summary>
        /// <param name="tilesetCanvas">Reference to tileset canvas</param>
        public void Init(Canvas tilesetCanvas) {
            //Setting up base variables
            m_TileSetCanvas = tilesetCanvas;
        }

        /// <summary>
        /// Gets a TilesetElement based on an image
        /// </summary>
        /// <param name="image">Image to search TilesetElement with</param>
        /// <returns></returns>
        public TilesetElement GetTileSetElementFromImage(Image image) {
            for (int i = 0; i < m_TileSetElements.Count; i++) {
                if (m_TileSetElements[i].m_Image == image) {
                    return m_TileSetElements[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a TilesetElement based on an ID
        /// </summary>
        /// <param name="id">ID to search TilesetElement with</param>
        /// <returns></returns>
        public TilesetElement GetTileSetElementFromID(int id) {
            for (int i = 0; i < m_TileSetElements.Count; i++) {
                if (m_TileSetElements[i].m_ID == id) {
                    return m_TileSetElements[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Loads Tileset
        /// </summary>
        /// <param name="loadedPath">Path to Tileset</param>
        /// <param name="tileWidth">Width per tile</param>
        /// <param name="tileHeight">Height per tile</param>
        public void LoadTileSet(string loadedPath, int tileWidth, int tileHeight) {
            //Image from loaded path
            BitmapImage image = new BitmapImage(new Uri(loadedPath, UriKind.Absolute));
            m_LoadedTilesetPath = loadedPath;

            ///Amount of tiles to fill in x and y of grid based on width of tile
            double xAmountOfTile = image.PixelWidth / tileWidth;
            double yAmountOfTile = image.PixelHeight / tileHeight;

            ///Width and height of image based on amount of tiles need in an axes
            double widthImageInCanvas = m_TileSetCanvas.Width / xAmountOfTile;
            double heightImageInCanvas = m_TileSetCanvas.Height / yAmountOfTile;

            ///X and y image place counter
            int xPositionIndex = -1;
            int yPositionIndex = 0;

            //When Tileset was already loaded, make sure to reset that
            if (m_TileSetCanvas.Children.Count > 0) {
                m_TileSetCanvas.Children.Clear();
                m_TileSetElements.Clear();
            }

            for (int i = 0; i < xAmountOfTile * yAmountOfTile; i++) {
                ///Creating the actual Tileset image
                Image img = new Image();
                img.Width = widthImageInCanvas;
                img.Height = widthImageInCanvas;
                img.MouseDown += new MouseButtonEventHandler(TilesetElementMouseDown);

                xPositionIndex++;

                ///When x amount in image in met, reset indexes
                if (xPositionIndex > xAmountOfTile - 1) {
                    xPositionIndex = 0;
                    yPositionIndex++;
                }

                //Creating a cropped zoomed in image based on image size and position indexes
                CroppedBitmap cb = new CroppedBitmap(image, 
                    new Int32Rect(0 + (tileWidth * xPositionIndex), 0 + (tileHeight * yPositionIndex), tileWidth, tileHeight));
                img.Source = cb;

                ///Setting location of image
                Canvas.SetLeft(img, 0 + (widthImageInCanvas * xPositionIndex));
                Canvas.SetTop(img, 0 + (widthImageInCanvas * yPositionIndex));

                m_TileSetCanvas.Children.Add(img);

                ///Creating the tilesetElement to add to the array of Tileset elememnts
                TilesetElement tileElement = new TilesetElement(i, img, cb);
                m_TileSetElements.Add(tileElement);
            }

            ///Creating and defining a border to visually display a tileset element being selected
            Border b = new Border();
            b.BorderThickness = new Thickness(1);
            b.BorderBrush = Brushes.Black;
            b.Width = widthImageInCanvas;
            b.Height = widthImageInCanvas;
            m_TileSetCanvas.Children.Add(b);
            SetTilesetBorderLocation(0, 0);
        }

        /// <summary>
        /// UI Tileset element mouse down function
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Mouse button event arguments</param>
        private void TilesetElementMouseDown(object sender, MouseButtonEventArgs e) {
            Image img = (Image)sender;
            double imgLeft = Canvas.GetLeft(img);
            double imgTop = Canvas.GetTop(img);

            ///Setting border to new location
            SetTilesetBorderLocation(imgLeft, imgTop);

            ///Getting and setting new TilesetElement
            TilesetElement element = GetTileSetElementFromImage(img);

            if (img != null) {
                m_CurrentSelectedTileSetElement = element;
            }
        }

        /// <summary>
        /// Used to set a new location for the Tileset element selection border
        /// </summary>
        /// <param name="x">X location to set border to</param>
        /// <param name="y">Y location to set border to</param>
        private void SetTilesetBorderLocation(double x, double y) {
            Border b = (Border)m_TileSetCanvas.Children[m_TileSetCanvas.Children.Count - 1];

            Canvas.SetLeft(b, x);
            Canvas.SetTop(b, y);
        }

        public void ResetTileSet() {
            m_TileSetElements.Clear();
            m_TileSetCanvas.Children.Clear();
        }
    }
}
