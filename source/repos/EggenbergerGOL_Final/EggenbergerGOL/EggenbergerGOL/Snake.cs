using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggenbergerGOL
{
    public class Snake
    {

        int cellAge = 0;
        bool alive = false;
        bool food = false;

        public Snake()
        {
            cellAge = 0;
            alive = false;
            food = false;
        }

     

        public bool living
        {
            get
            {
                return alive;
            }

            set
            {
                alive = value;
            }
        }

        public int age
        {
            get
            {
                return cellAge;
            }
            set
            {
                cellAge = value;
            }
        }

        public bool edible
        {
            get
            {
                return food;
            }
            set
            {
                food = value;
            }
        }



    }
}
