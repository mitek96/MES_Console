using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Console
{
    class Node
    {
        static int idReference=0;

        public Node(double x, double y, double t)
        {
            X = x;
            Y = y;
            T = t;
            id = ++idReference;
        }

        public Node()
        {
            id = ++idReference;
        }

        double X,Y;
        double T;
        int id;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public double X1
        {
            get
            {
                return X;
            }

            set
            {
                X = value;
            }
        }

        public double Y1
        {
            get
            {
                return Y;
            }

            set
            {
                Y = value;
            }
        }
    }
}
