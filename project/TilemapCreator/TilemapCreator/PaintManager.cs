using System.Windows.Controls;
using System.Windows.Input;

namespace TilemapCreator {
    /// <summary>
    /// Paint mode enum
    /// </summary>
    public enum PaintMode {
        Paint,
        Erase,
        Fill,
        DeleteFill
    }

    /// <summary>
    /// Handles all paint functionality in the program
    /// </summary>
    public class PaintManager {
        //Local paintmode enum
        private PaintMode m_PaintMode = PaintMode.Paint;

        ///References to other manager or upper classes
        private GridManager m_GridManager;
        private TileSetManager m_TilesetManager;

        /// <summary>
        /// Init function for this PaintManager class
        /// </summary>
        /// <param name="gridManager">grid manager to set</param>
        /// <param name="tilesetManager">tileset manager to set</param>
        public void Init(GridManager gridManager, TileSetManager tilesetManager) {
            m_GridManager = gridManager;
            m_TilesetManager = tilesetManager;
        }

        /// <summary>
        /// Getter for the active paint mode
        /// </summary>
        /// <returns>The active paint mode</returns>
        public PaintMode GetActivePaintMode() { return m_PaintMode; }

        /// <summary>
        /// Sets a new paint mode
        /// </summary>
        /// <param name="paintMode">New paint mode to set</param>
        /// <returns></returns>
        public PaintMode SetNewPaintMode(PaintMode paintMode) {
            m_PaintMode = paintMode;
            return m_PaintMode;
        }

        /// <summary>
        /// Grid element mouse over function
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Mouse event arguements</param>
        public void GridElementMouseOver(object sender, MouseEventArgs e) {
            ///If mouse is down
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;

            if (mouseIsDown) {
                ///Get Grid Element in correct layer from location
                Image img = (Image)sender;
                double x = Canvas.GetLeft(img);
                double y = Canvas.GetTop(img);

                GridElement element = m_GridManager.GetGridElementOnPositionFromLayer(m_GridManager.m_LayerIndex, x, y);

                //Based on the active paint mode,
                //paint grid elements differently
                if (img != null) {
                    if (m_PaintMode == PaintMode.Erase) {
                        DeColorGridElement(element);
                    }
                    else if (m_PaintMode == PaintMode.Paint) {
                        ColorGridElement(element);
                    }
                    else if (m_PaintMode == PaintMode.Fill) {
                        RecursiveColorGridElement(element);
                    }
                    else if (m_PaintMode == PaintMode.DeleteFill) {
                        RecursiveDeColorGridElement(element);
                    }
                }
            }
        }

        /// <summary>
        /// Color a grid element based on an id
        /// </summary>
        /// <param name="element">Grid element to paint</param>
        /// <param name="id">ID to paint from</param>
        public void ColorGridElementBasedOnID(GridElement element, int id) {
            if (id == -1) {
                DeColorGridElement(element);
            }
            else {
                element.m_Image.Opacity = 1;
                element.m_Visible = true;
                element.m_ID = id;
                element.m_Image.Source = m_TilesetManager.GetTileSetElementFromID(id).m_CroppedImage;
            }
        }

        /// <summary>
        /// Colors a grid element
        /// </summary>
        /// <param name="element">Grid element to color</param>
        private void ColorGridElement(GridElement element) {
            ///Setting grid element values
            element.m_Image.Opacity = 1;
            element.m_Visible = true;
            element.m_ID = m_TilesetManager.m_CurrentSelectedTileSetElement.m_ID;
            element.m_Image.Source = m_TilesetManager.m_CurrentSelectedTileSetElement.m_CroppedImage;
        }

        /// <summary>
        /// De-Colors a grid element
        /// </summary>
        /// <param name="element">Grid element to de-color</param>
        private void DeColorGridElement(GridElement element) {
            ///Setting grid element values
            element.m_Image.Opacity = 0;
            element.m_ID = -1;
            element.m_Visible = false;
        }

        /// <summary>
        /// Recursive Coloring of grid element
        /// </summary>
        /// <param name="element">Grid element to recursively color</param>
        private void RecursiveColorGridElement(GridElement element) {
            ColorGridElement(element);
            RecursiveColorSideGridElements(element);
        }

        /// <summary>
        /// Tries to color 4 grid element sides of a grid element
        /// </summary>
        /// <param name="element">Grid element to look from</param>
        private void RecursiveColorSideGridElements(GridElement element) {
            //Up grid element
            GridElement upElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Up);

            if (upElement != null) {
                if (upElement.m_ID == -1) {
                    RecursiveColorGridElement(upElement);
                }
            }

            //Right grid element
            GridElement rightElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Right);

            if (rightElement != null) {
                if (rightElement.m_ID == -1) {
                    RecursiveColorGridElement(rightElement);
                }
            }

            //Down grid element
            GridElement downElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Down);

            if (downElement != null) {
                if (downElement.m_ID == -1) {
                    RecursiveColorGridElement(downElement);
                }
            }

            //Left grid element
            GridElement leftElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Left);

            if (leftElement != null) {
                if (leftElement.m_ID == -1) {
                    RecursiveColorGridElement(leftElement);
                }
            }
        }

        /// <summary>
        /// Recursive de-coloring of grid element
        /// </summary>
        /// <param name="element">Grid element to recursively de-color</param>
        private void RecursiveDeColorGridElement(GridElement element) {
            DeColorGridElement(element);
            RecursiveDeColorSideGridElements(element);
        }

        /// <summary>
        /// Tries to color 4 grid element sides of a grid element
        /// </summary>
        /// <param name="element">Grid element to look from</param>
        private void RecursiveDeColorSideGridElements(GridElement element) {
            //Up grid element
            GridElement upElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Up);

            if (upElement != null) {
                if (upElement.m_ID != -1) {
                    RecursiveDeColorGridElement(upElement);
                }
            }

            //Right grid element
            GridElement rightElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Right);

            if (rightElement != null) {
                if (rightElement.m_ID != -1) {
                    RecursiveDeColorGridElement(rightElement);
                }
            }

            //Down grid element
            GridElement downElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Down);

            if (downElement != null) {
                if (downElement.m_ID != -1) {
                    RecursiveDeColorGridElement(downElement);
                }
            }

            //Left grid element
            GridElement leftElement = m_GridManager.GetGridElementInDirectionFromGridElement(element, Direction.Left);

            if (leftElement != null) {
                if (leftElement.m_ID != -1) {
                    RecursiveDeColorGridElement(leftElement);
                }
            }
        }
    }
}
