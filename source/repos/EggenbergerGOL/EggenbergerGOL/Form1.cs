using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EggenbergerGOL
{
    

    public partial class Form1 : Form
    {
        int uWidth = 30; //variable to manipulte universe width
        int uHeight; //varialble to manipulate universe height
        int timerInt = 100; //variable to control timer intervals
        int seedRand = 52;

        // The universe array
        bool[,] universe; //array szeundelcared to allow manipulation with uWidth and uHeight

        // scratch pad to store next generation array
        bool[,] scratchPad; //array size not declared to allow manipulation with uWidth and uHeight

        bool seeNeighbors = true; //bool to allow for toggle of visibilty neighbor count
        bool seeGrid = true;  //bool to allow for visibility of grid
        bool seeHUD = true;
        bool toroidal = true;

        // Drawing colors
        Color gridColor = Color.Black; //standard grid
        Color cellColor = Color.Gray; //living cell color
        Color gridXColor = Color.Black; //Grid X10 color
        Color background; //= Color.White; //establishing color for backbground rect

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        //constructor for the class, initiates all member fields
        public Form1()
        {
            //reading in the settings for color options
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;
            //reading settings for geid dimensions and time interval
            uWidth = Properties.Settings.Default.GridWidth;
            uHeight = Properties.Settings.Default.GridHeight;
            timerInt = Properties.Settings.Default.Interval;
            
            universe = new bool[uWidth, uHeight]; //declare array size in constructor
            scratchPad = new bool[uWidth, uHeight]; 
            InitializeComponent();

            // Setup the timer
            timer.Interval = timerInt; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

        }

        //counts the neighboring living cells, treating the boundaries as though the continue on opposite side
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

        //counts living nighbors in the universe, treating the boundaries as a hard edge
        private int CountNeighborsFinite(int x, int y)
        {
            int nCount = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            

            for(int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for(int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xCheck < 0) continue;
                    if (yCheck < 0) continue;
                    if (xCheck >= xLen) continue;
                    if (yCheck >= yLen) continue;
                    if ((xOffset == 0) && (yOffset == 0)) continue;

                    if (universe[xCheck, yCheck] == true) nCount++;

                }
            }


            return nCount;
            
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            for (int y = 0; y < universe.GetLength(1); y++)  // nested forloop to check every cell in array
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors;
                    if (toroidal) neighbors = CountNeighborsToroidal(x, y); //toggle for choice of toroidal or finite universe
                    else neighbors = CountNeighborsFinite(x, y);

                    if (universe[x, y] == true)
                    {
                        if (neighbors > 3) scratchPad[x, y] = false;
                        else if (neighbors < 2) scratchPad[x, y] = false;
                        else scratchPad[x, y] = true;
                    }
                    else                         //implementation of the 4 rules. store next gen into scratch pad
                    {
                        if (neighbors == 3) scratchPad[x, y] = true;
                        else scratchPad[x, y] = false;
                    }
                }
            }

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;           
            graphicsPanel1.Invalidate();
        }

        //updates the bottom strip of the application window with current information about the universe
        private void StatusStripUpdate()
        {
            int alive = CellCount(); //to display living cells in bottom status strip
            
            // Update status strip generations, interval, living cells, and current seed
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString() + "    Interval: " + timerInt.ToString() + "ms    Living Cells: " + alive.ToString() + "    Seed: " + seedRand.ToString();
        }

        //returns an int of the current number of living cells within the univers
        private int CellCount()
        {
            int c = 0; //int to be returned with living cell count

            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++) //nested forloop to check each cell
                {
                    if (universe[x, y] == true) c++; //if cell is alive, adds 1 to c
                }
            }

            return c; //returns total living cells
        }

        //allows for the array to be randomly re-assigned. can be used with custom seed, current seed, or seed determined by current time
        private void Randomize()
        {           
            Random rUniverse = new Random(seedRand); //allows variable to seed random object

            for(int y = 0; y< universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rUniverse.Next() % 2 == 0) universe[x, y] = true; //checks random number to see if even or odd, even creates living cell, odd created dead cell
                    else universe[x, y] = false;
                }
            }
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {                       
            NextGeneration(); //calls next gen every interval           
        }

        //renders the realtime graphics into the client window
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {

            StatusStripUpdate();
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
                        if ((x * 10) < universe.GetLength(0) && (y * 10) < universe.GetLength(1)) //makes sure the 10x grid is within the bounds of the standard grid
                        {
                            e.Graphics.DrawRectangle(gridPenX, cellXRect.X, cellXRect.Y, cellXRect.Width, cellXRect.Height);
                        }
                    }
                    
                }
            }

           if (seeNeighbors) //bool that can be toggled by user to see neighbor count
            {
                Font font = new Font("Arial", 0.3f *(ClientSize.Width/universe.GetLength(1))); //font and size used to paint neighbor count in cells

                StringFormat stringN = new StringFormat();
                stringN.Alignment = StringAlignment.Center; //allows neighbor count to be centered within it's cell
                stringN.LineAlignment = StringAlignment.Center;

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)  //nested forloop to paint neighbor count in each cell
                    {
                        int neighbors;
                        if (toroidal) neighbors = CountNeighborsToroidal(x, y);
                        else neighbors = CountNeighborsFinite(x, y);

                        Rectangle nRect = Rectangle.Empty;
                        nRect.X = x * cellWidth;
                        nRect.Y = y * cellHeight;  //creates the boundary of the cell to align with the grid
                        nRect.Width = cellWidth;
                        nRect.Height = cellHeight;

                        if (neighbors > 0)  //condition to not paint neighbor count if there are no living neighbors
                        {
                            if (neighbors == 3) //green for 3 nighbor cells, indiciating they will come to life, or remain livnig
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, nRect, stringN);
                            }
                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, nRect, stringN); //red for cells that will remain dead, or could die unless they are have 2 neighbors
                            }
                        }
                    }
                }
            }

            if (seeHUD) //bool user can toggle to turn heads up display on and off
            {
                int hcellWidth = graphicsPanel1.ClientSize.Width; //establishing cell size for neighbor text
                int hcellHeight = graphicsPanel1.ClientSize.Height;
                Font font = new Font("Arial", 16f); //declare font and point size for neighbor count

                string boundary;
                if (toroidal) boundary = " Toroidal";
                else boundary = " Finite";

                StringFormat stringN = new StringFormat();
                stringN.Alignment = StringAlignment.Near;  //code that will allow for centering neighbor count within cell
                stringN.LineAlignment = StringAlignment.Far;
                int cells = CellCount();
                string hudG = "Generations: " + generations + "\nCell Count: " + cells + "\nBoundary Type:" + boundary + "\nUniverse Dimensions: "  + uWidth + "x" + uHeight ; //the text contained within the HUD


                
                Rectangle nRect = Rectangle.Empty;
                nRect.X = 0;
                nRect.Y = 0;
                nRect.Width = hcellWidth;  //create a rectangle the size of the client window
                nRect.Height = hcellHeight;



                e.Graphics.DrawString(hudG, font, Brushes.Red, nRect, stringN); //paints the HUD
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            gridPenX.Dispose();
        }

        //allows user to manually turn on/off celles by clicking inside the client window
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

        //exit option in menu. Closes the application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); //closes the main window
        }

        //tool strip button to play GOL
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true; //play button toggles the timer on
        }

        //toolstrip button to pause GOL
        private void toolStrip1_Click(object sender, EventArgs e)
        {
            timer.Enabled = false; //pause button toggles timer off
        }

        //tool strip button that advances universe one step into the future
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration(); //next button calls Nextgeneraton once, to see the next step in GOL
        }

        //menu item to clear the universe and set all cells to dead
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++) //sets all indexes in the universe array yo false(dead)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0; //resets geneeration count to 0
            graphicsPanel1.Invalidate();
        }

        //menu itme to toggle on/off neoghbor count visibilty
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(seeNeighbors == true) //checks to see if neighborcount is currently 'on'
            {
                seeNeighbors = false;
                veiwNeighbors.Checked = false; //toggles 'off' neighbor count. updates check boxes in menu and context menu
                cNeighbors.Checked = false;
            }
            else
            {
                seeNeighbors = true;
                veiwNeighbors.Checked = true; //toggles bool to turn 'on' neighbors, updates check boxes in both menus
                cNeighbors.Checked = true;
            }
            
            graphicsPanel1.Invalidate();
        }

        //context menu item to open color dialog to edit color of living cells
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog(); //new dialog for color options
            dlg.Color = cellColor; //sets focus to current color of cells
            if(DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color; //assigns selected color to cells if user clicks 'ok'
            }
            graphicsPanel1.Invalidate();

        }

        //context menu itme to open color dialog to edit color of standard grid
        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog(); //new dialog for color options
            dlg.Color = gridColor; //sets focus to current color of grid
            if(DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color; //assigns selected color to grid if user clicks 'ok'
            }
            graphicsPanel1.Invalidate();
        }

        //context menu to open color dialog to edit color of grid x10
        private void gridX10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog(); //new dialog for color options
            dlg.Color = gridXColor;//sets focus on dialog to current grid color
            if(DialogResult.OK == dlg.ShowDialog())
            {
                gridXColor = dlg.Color; //assigns selected color to grid x10 if user clicks 'ok'
            }
            graphicsPanel1.Invalidate();
        }
      
        //context menu item to open color dialog box to edit background color
        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog(); //new dialog for color options
            dlg.Color = background; //sets focus to current background colo in dialog
            if(DialogResult.OK == dlg.ShowDialog())
            {
                background = dlg.Color;  //assigns selected color in dialog to background if user clicks 'ok'
                
            }
            graphicsPanel1.Invalidate();
        }

        //menu item to open modal window for universe options (universe size, and timing intervals)
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
                
                timerInt = dlg.Time; //update timer variable, then assign it to timer.
                timer.Interval = timerInt;

                graphicsPanel1.Invalidate();         
            }

        }

        //menu item that allows visibilty of grid to be toggled on/off
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seeGrid == true)
            {
                seeGrid = false;         //toggles bool based on current state, updates check boxes for menu and context menu
                cGrid.Checked = false;
                showGrid.Checked = false;
            }
            else
            {
                seeGrid = true;
                cGrid.Checked = true;      //toggles bool based on current state, updates check boxes accoridingly
                showGrid.Checked = true;
            }
            
            graphicsPanel1.Invalidate();
        }

        //saves settings when application is closing (color options, universe size, interval time)
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackgroundColor = background;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Grid10Color = gridXColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridWidth = uWidth;
            Properties.Settings.Default.GridHeight = uHeight;
            Properties.Settings.Default.Interval = timerInt;
            Properties.Settings.Default.Save();
        }

        //menu item to reset all settings to default values (color options, universe size, interval time)
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            //reading in the settings for color options and universe settings from default
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;
            uWidth = Properties.Settings.Default.GridWidth;
            uHeight = Properties.Settings.Default.GridHeight;
            timerInt = Properties.Settings.Default.Interval;

            universe = new bool[uWidth, uHeight]; //rebuilds universe to original size
            scratchPad = new bool[uWidth, uHeight];
            graphicsPanel1.Invalidate();
        }

        //menu item to reload last saved Settings (color options, universe size, interval time)
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            //reading in the settings for color options and universe settings from last saved
            background = Properties.Settings.Default.BackgroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridXColor = Properties.Settings.Default.Grid10Color;
            cellColor = Properties.Settings.Default.CellColor;
            uWidth = Properties.Settings.Default.GridWidth;
            uHeight = Properties.Settings.Default.GridHeight;
            timerInt = Properties.Settings.Default.Interval;

            universe = new bool[uWidth, uHeight]; //rebuilds universe to last saved size
            scratchPad = new bool[uWidth, uHeight];

            graphicsPanel1.Invalidate();
        }

        //context or main menu item to toggle on/off seeNeighbors
        private void headsUpDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(seeHUD == true)
            {
                seeHUD = false;
                headsUpDisplay.Checked = false; //toggles bool from current state, and updates both check-boxes accordingly
                cHUD.Checked = false;
            }
            else
            {
                seeHUD = true;
                headsUpDisplay.Checked = true; //toggles bool from current state, and updates both check-boxes accordingly
                cHUD.Checked = true;
            }

            graphicsPanel1.Invalidate(); //updates client window

        }

        //menu item to randomize from current seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize(); //random new array with current seed
            graphicsPanel1.Invalidate(); //update client window
        }

        //opens modal window for setting custom random seed
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedModal dlg = new SeedModal(); //displays menu for Randomizing 
            dlg.Seed = seedRand; //sets value to current seed

            if (DialogResult.OK == dlg.ShowDialog())
            {
                seedRand = dlg.Seed; //changes seed to value entered by user
                Randomize(); //randomizes array with new seed
                graphicsPanel1.Invalidate(); //refreshes screen in realtim
            }
        }

        //menu item for turning on/off finite universe mode
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidal = false; //toggles off toroidal, sets ti 'finite' mode
            if (finiteM.Checked) toroidalM.Checked = false; //ensures both 'toroidal' and 'finite' check boxes are not both checked.
            graphicsPanel1.Invalidate(); //update client window
        }

        //menu item for turning on/off turoidal mode
        private void toroidalM_Click(object sender, EventArgs e)
        {
            if (toroidalM.Checked) //verifies if box for toroidal is checked
            {
                toroidal = true; //sets toroidal bool to true if box is checked
                finiteM.Checked = false; //unchecks finite menu check box
            }
            graphicsPanel1.Invalidate(); //update client window

        }

        //menu item to set current time as random seed
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //generates a number from current time ex.(3:45:15pm would become 154515) and assigns it to seedRand
            seedRand = ((int)DateTime.Now.Hour * 10000) + ((int)DateTime.Now.Minute * 100) + (int)DateTime.Now.Second; 
            Randomize(); //calls randomize with new seed
            graphicsPanel1.Invalidate();
        }

        //tool strip 'save' button
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    string currentLine = string.Empty;
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            currentLine = currentLine + "O";
                        }
                        else
                        {
                            currentLine = currentLine + ".";
                        }
                    }
                    writer.WriteLine(currentLine);
                }
                writer.Close();
            }
        }

        //tool strip 'open' button
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;


            if(DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader read = new StreamReader(dlg.FileName);
                int xLen = 0;
                int yLen = 0;

                while (!read.EndOfStream)
                {
                    string row = read.ReadLine();
                    if (row[0] == 'O'||row[0] == '.')
                    {
                        yLen++;
                        xLen = row.Length;
                    }
                    
                }

                

                uWidth = xLen;
                uHeight = yLen;
                universe = new bool[uWidth, uHeight]; //creates new universe array with updated value
                scratchPad = new bool[uWidth, uHeight]; //creatsnew scratch arraty with updated value

                read.BaseStream.Seek(0, SeekOrigin.Begin);
                int y = 0;
                while (!read.EndOfStream)
                {
                    
                    string row = read.ReadLine();
                    if (row[0] == 'O'||row[0] == '.')
                    {
                        for(int x = 0; x < row.Length; x++)
                        {
                            if (row[x] == 'O') universe[x, y] = true;
                            else universe[x, y] = false;
                        }
                        y++;
                    }
                    
                }
                read.Close();
            }
            
            graphicsPanel1.Invalidate();
        }
    }
}
