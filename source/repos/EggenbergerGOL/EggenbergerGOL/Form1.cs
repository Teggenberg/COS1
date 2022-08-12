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
        int uWidth = 30; //variable to manipulte universe width
        int uHeight = 30; //varialble to manipulate universe height
        int timerInt = 100; //variable to control timer intervals
        int seedRand = 52;

        // The universe array
        bool[,] universe; //array szeundelcared to allow manipulation with uWidth and uHeight

        // scratch pad to store next generation array
        bool[,] scratchPad; //array size not declared to allow manipulation with uWidth and uHeight

        bool seeNeighbors = true; //bool to allow for toggle of visibilty neighbor count
        bool seeGrid = true;  //bool to allow for visibility of grid
        bool seeHUD = true;

        // Drawing colors
        Color gridColor = Color.Black; //standard grid
        Color cellColor = Color.Gray; //living cell color
        Color gridXColor = Color.Black; //Grid X10 color
        Color background; //= Color.White; //establishing color for backbground rect

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            universe = new bool[uWidth, uHeight]; //declare array size in constructor
            scratchPad = new bool[uWidth, uHeight]; 
            InitializeComponent();

            // Setup the timer
            timer.Interval = timerInt; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            //reading in the settings for color options
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int neighbors = 0; //variable that is returned once method is complete
            int xLen = universe.GetLength(0); //variable to determine width oif grid
            int yLen = universe.GetLength(1); //variable to determine height of grid

            for(int yOffset = -1; yOffset <= 1 ; yOffset++) //checking cell above at, and below
            {
                for(int xOffset = -1; xOffset <= 1; xOffset++) //checking cell to the left, at, and to the right
                {
                    int xCheck = x + xOffset; //using offset against x to check relative cells to x
                    int yCheck = y + yOffset; //using offest against y to check retlative cells to y

                    if (xOffset == 0 && yOffset == 0) continue; //skips over the center cell (not a neighbor)
                    if (xCheck < 0) xCheck = xLen - 1; //moves to opposite side of univers if home cell is on border
                    if (yCheck < 0) yCheck = yLen - 1;
                    if (xCheck > xLen -1) xCheck = 0; //checks if cell is at the last index of the array
                    if (yCheck > yLen -1) yCheck = 0;

                    if (universe[xCheck, yCheck] == true) neighbors++; //increments neighborcount if neighbor is 'true'
                }
            }
            return neighbors;
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {            
            for (int y = 0; y < universe.GetLength(1); y++)  //cloning universe array into scratchpad
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x,y] = universe[x,y];
                }
            }

            for (int y = 0; y < universe.GetLength(1); y++)  // nested forloop to check every cell in array
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neigbors = CountNeighborsToroidal(x, y); //stores cells neighborcount into variable
                    if (universe[x,y] == true)
                    {
                        if(neigbors > 3) scratchPad[x, y] = false;  
                        if(neigbors < 2) scratchPad[x, y] = false;
                    }
                    if (universe[x,y] == false)                         //implementation of the 4 rules. store next gen into scratch pad
                    {
                        if (neigbors == 3) scratchPad[x, y] = true;
                    }
                }
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)  //copying next generation into universe
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

        private int CellCount()
        {
            int c = 0;

            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true) c++;
                }
            }

            return c;
        }

        private void Randomize()
        {
            Random rUniverse = new Random(seedRand);

            for(int y = 0; y< universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rUniverse.Next() % 2 == 0) universe[x, y] = true;
                    else universe[x, y] = false;
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
            // brush to paint background
            Brush backBrush = new SolidBrush(background);

            // Rectangle for background
            Rectangle backRect = Rectangle.Empty;
            backRect.X = 0;
            backRect.Y = 0;
            backRect.Width = graphicsPanel1.ClientSize.Width;
            backRect.Height = graphicsPanel1.ClientSize.Height;

            e.Graphics.FillRectangle(backBrush, backRect);

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

                    // Rectangle for grid X10
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

                    if (seeGrid) //bool to allow menu toggle for grid visibility
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                        //bold outline for grid x 10 lines, separate grid to allow for separate color options from standard grid
                        if ((x * 10) < universe.GetLength(0) && (y * 10) < universe.GetLength(1))
                        {
                            e.Graphics.DrawRectangle(gridPenX, cellXRect.X, cellXRect.Y, cellXRect.Width, cellXRect.Height);
                        }
                    }
                    
                }
            }

           if (seeNeighbors)
            {
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
            }

            if (seeHUD)
            {
                int hcellWidth = graphicsPanel1.ClientSize.Width; //establishing cell size for neighbor text
                int hcellHeight = graphicsPanel1.ClientSize.Height;
                Font font = new Font("Arial", 16f); //declare font and point size for neighbor count

                StringFormat stringN = new StringFormat();
                stringN.Alignment = StringAlignment.Near;  //code that will allow for centering neighbor count within cell
                stringN.LineAlignment = StringAlignment.Far;
                int cells = CellCount();
                string hudG = "Generations: " + generations + "\nCell Count: " + cells + "\nBoundary Type:" + "\nUniverse Dimensions: "  + uWidth + "x" + uHeight ;


                
                Rectangle nRect = Rectangle.Empty;
                nRect.X = 0;
                nRect.Y = 0;
                nRect.Width = hcellWidth;
                nRect.Height = hcellHeight;



                e.Graphics.DrawString(hudG, font, Brushes.Red, nRect, stringN);
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
            generations = 0;
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seeNeighbors = false;
            if(veiwNeighbors.Checked)  seeNeighbors = true;
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            seeNeighbors = true;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();

        }

        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void gridX10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridXColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                gridXColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void colorToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = background;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                background = dlg.Color;
                
            }
            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UniverseModal dlg = new UniverseModal();
            dlg.uniWidth = uWidth; //modal displays current width
            dlg.uniHeight = uHeight; // modal displays current height
            dlg.Time = timerInt; // modal displays current timer interval

            
            
            if (DialogResult.OK == dlg.ShowDialog()) //updates universe width to value entered in dialog
            {
                if (dlg.uniWidth != uWidth || dlg.uniHeight != dlg.uniHeight) //condition to avoid clearing array unless values are updated.
                {
                    uWidth = dlg.uniWidth; //updates uWidth to new value
                    uHeight = dlg.uniHeight; //updates uHeight to new value
                    universe = new bool[uWidth, uHeight]; //creates new universe array with updated value
                    scratchPad = new bool[uWidth, uHeight]; //creatsnew scratch arraty with updated value
                }
                
                timerInt = dlg.Time;
                timer.Interval = timerInt;

                graphicsPanel1.Invalidate();         
            }

        }

        private void modalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seeGrid = false;
            if (showGrid.Checked) seeGrid = true;
            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackgroundColor = background;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Grid10Color = gridXColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            //reading in the settings for color options
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;

            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            //reading in the settings for color options
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;

            graphicsPanel1.Invalidate();
        }

        private void headsUpDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seeHUD = false; //turns off HUD
            if (headsUpDisplay.Checked) seeHUD = true; //if box is checked, HUD on.
            graphicsPanel1.Invalidate();
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize();
            graphicsPanel1.Invalidate();
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedModal dlg = new SeedModal();
            dlg.Seed = seedRand;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                seedRand = dlg.Seed;
                Randomize();
                graphicsPanel1.Invalidate();
            }
        }
    }
}
