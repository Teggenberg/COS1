using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EggenbergerGOL
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];
        // scratch pad to store next generation array
        bool[,] scratchPad = new bool[30, 30];//new bool[universe.GetLength(0), universe.GetLength(1)];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color gridXColor = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int neighbors = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for(int yOffset = -1; yOffset <= 1 ; yOffset++)
            {
                for(int xOffset = -1; xOffset <= 1; xOffset++) 
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) continue;
                    if (xCheck < 0) xCheck = xLen - 1;
                    if (yCheck < 0) yCheck = yLen - 1;
                    if (xCheck > xLen -1) xCheck = 0;
                    if (yCheck > yLen -1) yCheck = 0;

                    if (universe[xCheck, yCheck] == true) neighbors++;

                }
            }


            return neighbors;
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x,y] = universe[x,y];
                }
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neigbors = CountNeighborsToroidal(x, y);
                    if (universe[x,y] == true)
                    {
                        if(neigbors > 3) scratchPad[x, y] = false;  
                        if(neigbors < 2) scratchPad[x, y] = false;
                    }
                    if (universe[x,y] == false)
                    {
                        if (neigbors == 3) scratchPad[x, y] = true;
                    }
                }
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = scratchPad[x, y];
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void DisplayNeighbors()
        {
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            Font font = new Font("Arial", 20f);

            StringFormat stringN = new StringFormat();
            stringN.Alignment = StringAlignment.Center;
            stringN.LineAlignment = StringAlignment.Center;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = CountNeighborsToroidal(x, y);
                    Rectangle nRect = Rectangle.Empty;
                    nRect.X = x * cellWidth;
                    nRect.Y = y * cellHeight;
                    nRect.Width = cellWidth;
                    nRect.Height = cellHeight;

                    if (neighbors > 0)
                    {
                        //e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, nRect, stringN);
                    }
                }
            }
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            
           
            NextGeneration();
            
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            // Cells for the 10 grid
            int cellXWidth = cellWidth * 10;
            int cellXHeight = cellHeight * 10;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            Pen gridPenX = new Pen(gridXColor, 3);
            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    Rectangle cellXRect = Rectangle.Empty;
                    cellXRect.X = x * cellXWidth;
                    cellXRect.Y = y * cellXHeight;
                    cellXRect.Width = cellXWidth;
                    cellXRect.Height = cellXHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    
                    
                    
                    

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    if ((x * 10) < universe.GetLength(0) && (y * 10) < universe.GetLength(1))
                    {
                        e.Graphics.DrawRectangle(gridPenX, cellXRect.X, cellXRect.Y, cellXRect.Width, cellXRect.Height);
                    }
                }
            }

            //int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            //int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            Font font = new Font("Arial", 10f);

            StringFormat stringN = new StringFormat();
            stringN.Alignment = StringAlignment.Center;
            stringN.LineAlignment = StringAlignment.Center;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = CountNeighborsToroidal(x, y);
                    Rectangle nRect = Rectangle.Empty;
                    nRect.X = x * cellWidth;
                    nRect.Y = y * cellHeight;
                    nRect.Width = cellWidth;
                    nRect.Height = cellHeight;

                    if (neighbors > 0)
                    {
                        if (neighbors == 3)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, nRect, stringN);
                        }
                        else
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, nRect, stringN);
                        }
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            gridPenX.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); //closes the main window
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStrip1_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            graphicsPanel1.Invalidate();
        }
    }
}
