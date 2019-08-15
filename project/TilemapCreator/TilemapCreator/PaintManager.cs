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

namespace TilemapCreator {
    public enum PaintMode {
        Paint,
        Fill,
        Erase
    }

    public class PaintManager {
        private PaintMode m_PaintMode = PaintMode.Paint;
        private GridManager m_GridManager;
        private TileSetManager m_TilesetManager;

        public void Init(GridManager gridManager, TileSetManager tilesetManager) {
            m_GridManager = gridManager;
            m_TilesetManager = tilesetManager;
        }

        public PaintMode GetActivePaintMode() { return m_PaintMode; }
        public PaintMode SetNewPaintMode(PaintMode paintMode) {
            m_PaintMode = paintMode;
            return m_PaintMode;
        }

        public void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;

            if (mouseIsDown)
            {
                Image img = (Image)sender;

                double x = Canvas.GetLeft(img);
                double y = Canvas.GetTop(img);

                GridElement element = m_GridManager.GetGridElementOnPositionFromLayer(m_GridManager.m_LayerIndex, x, y);

                if (img != null)
                {
                    if (m_PaintMode == PaintMode.Erase)
                    {
                        if (element != null)
                        {
                            element.m_Image.Opacity = 0;
                            element.m_ID = -1;
                            element.m_Visible = false;
                            return;
                        }
                    }
                    else if (m_PaintMode == PaintMode.Paint)
                    {
                        element.m_Image.Opacity = 1;
                        element.m_Visible = true;
                        element.m_ID = m_TilesetManager.m_CurrentSelectedTileSetElement.m_ID;
                        element.m_Image.Source = m_TilesetManager.m_CurrentSelectedTileSetElement.m_CroppedImage;
                    }
                    else if (m_PaintMode == PaintMode.Fill)
                    {
                        ColorGridElement(element);
                    }
                }
            }
        }

        private void ColorGridElement(GridElement element)
        {
            element.m_Image.Opacity = 1;
            element.m_Visible = true;
            element.m_ID = m_TilesetManager.m_CurrentSelectedTileSetElement.m_ID;
            element.m_Image.Source = m_TilesetManager.m_CurrentSelectedTileSetElement.m_CroppedImage;

            SetFillElements(element);
        }

        private void SetFillElements(GridElement element)
        {
            GridElement upElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Up);

            if (upElement != null)
            {
                if (upElement.m_ID == -1)
                {
                    ColorGridElement(upElement);
                }
            }

            GridElement rightElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Right);

            if (rightElement != null)
            {
                if (rightElement.m_ID == -1)
                {
                    ColorGridElement(rightElement);
                }
            }

            GridElement downElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Down);

            if (downElement != null)
            {
                if (downElement.m_ID == -1)
                {
                    ColorGridElement(downElement);
                }
            }

            GridElement leftElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Left);

            if (leftElement != null)
            {
                if (leftElement.m_ID == -1)
                {
                    ColorGridElement(leftElement);
                }
            }
        }
    }
}
