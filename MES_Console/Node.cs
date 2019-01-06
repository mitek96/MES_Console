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

        double X, Y;
        double T;

        public Node(double x, double y, double t)
        {
            X = x;
            Y = y;
            T = t;
            Id = ++idReference;
        }

        public Node()
        {
            Id = ++idReference;
        }

        public int Id { get; set; }

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
