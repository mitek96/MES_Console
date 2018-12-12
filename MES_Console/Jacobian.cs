using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Console
{
    class Jacobian
    {
        double[,] N;
        double[] ksi;
        double[] eta;
        double[,] interpolatedCoordinates;
        double[,] dNdKsi;
        double[,] dNdEta;
        double[,] jacobian;
        double[] detJacobian;
        double[,] inversedJacobian;

        public Jacobian()
        {
            ksi = new double[4];
            eta = new double[4];
            N = new double[4, 4];
            interpolatedCoordinates = new double[4, 2];
            dNdKsi = new double[4, 4];
            dNdEta = new double[4, 4];
            jacobian = new double[4, 4];
            detJacobian = new double[4];
            inversedJacobian = new double[4, 4];

            ksi[0] = -1.0 / Math.Sqrt(3);
            ksi[1] = -ksi[0];
            ksi[2] = ksi[1];
            ksi[3] = -ksi[2];

            eta[0] = ksi[0];
            eta[1] = eta[0];
            eta[2] = -eta[1];
            eta[3] = eta[2];

        }

        public double getDNdKsi(int x, int y)
        {
            return dNdKsi[x, y];
        }

        public double getDNdEta(int x,int y)
        {
            return dNdEta[x, y];
        }

        public double getJacobian(int x, int y)
        {
            return jacobian[x, y];
        }

        public double getInversedJacobian(int x, int y)
        {
            return inversedJacobian[x, y];
        }

        public double getDetJ(int x)
        {
            return detJacobian[x];
        }


        public void calculateShapeFunctions()
        {
            
            for(int i =0;i<4;++i)
            {
                N[i, 0] = 0.25 * (1 - ksi[i]) * (1 - eta[i]);
                N[i, 1] = 0.25 * (1 + ksi[i]) * (1 - eta[i]);
                N[i, 2] = 0.25 * (1 + ksi[i]) * (1 + eta[i]);
                N[i, 3] = 0.25 * (1 - ksi[i]) * (1 + eta[i]);
            }
        }

        public void printShapeFunctions()
        {
            for(int i =0;i<4; ++i)
            {
                Console.WriteLine("N(" + i + ",0)=" + N[i, 0]);
                Console.WriteLine("N(" + i + ",1)=" + N[i, 1]);
                Console.WriteLine("N(" + i + ",2)=" + N[i, 2]);
                Console.WriteLine("N(" + i + ",3)=" + N[i, 3]);
            }
        }

        public void calculateInterpolatedCoordinates(Element element)
        {
            for(int i=0;i<4;++i)
            {
                interpolatedCoordinates[i, 0] = N[i, 0] * element.getNode(0).X1 + N[i, 1] * element.getNode(1).X1 + N[i, 2] * element.getNode(2).X1 + N[i, 3] * element.getNode(3).X1;
                interpolatedCoordinates[i,1]= N[i, 0] * element.getNode(0).Y1 + N[i, 1] * element.getNode(1).Y1 + N[i, 2] * element.getNode(2).Y1 + N[i, 3] * element.getNode(3).Y1;
            }
        }

        public void printInterpolatedCoordinates()
        {
            for(int i=0;i<4;++i)
            {
                Console.WriteLine("Xp(" + i + ")=" + interpolatedCoordinates[i, 0]+"  , Yp("+i+")="+interpolatedCoordinates[i,1]);
            }
        }

        public void calculateShapeFunctionsDerivatives()
        {
            for (int i=0;i<4;++i)
            {
                dNdKsi[0, i] = -0.25 * (1.0-eta[i]);
                dNdKsi[1, i] = 0.25 * (1.0 - eta[i]);
                dNdKsi[2, i] = 0.25 * (1.0 + eta[i]);
                dNdKsi[3, i] = -0.25 * (1.0 + eta[i]);

                dNdEta[0, i] = -0.25 * (1.0 - ksi[i]);
                dNdEta[1, i] = -0.25 * (1.0 + ksi[i]);
                dNdEta[2, i] = 0.25 * (1.0 + ksi[i]);
                dNdEta[3, i] = 0.25 * (1.0 - ksi[i]);
            }
        }

        public void printShapeFunctionsDerivativesKsi()
        {
            for(int i=0;i<4;++i)
            {
                Console.WriteLine("dN/dKsi[0," + i + "]=" + dNdKsi[0, i]);
                Console.WriteLine("dN/dKsi[1," + i + "]=" + dNdKsi[1, i]);
                Console.WriteLine("dN/dKsi[2," + i + "]=" + dNdKsi[2, i]);
                Console.WriteLine("dN/dKsi[3," + i + "]=" + dNdKsi[3, i]);
            }
        }

        public void printShapeFunctionsDerivativesEta()
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("dN/dEta[0," + i + "]=" + dNdEta[0, i]);
                Console.WriteLine("dN/dEta[1," + i + "]=" + dNdEta[1, i]);
                Console.WriteLine("dN/dEta[2," + i + "]=" + dNdEta[2, i]);
                Console.WriteLine("dN/dEta[3," + i + "]=" + dNdEta[3, i]);
            }
        }

        public void calculateJacobian(Element element)
        {
            for(int i=0;i<4;++i)
            {
                jacobian[0, i] = element.getNode(0).X1 * dNdKsi[0, i] + element.getNode(1).X1 * dNdKsi[1, i] + element.getNode(2).X1 * dNdKsi[2, i] + element.getNode(3).X1 * dNdKsi[3, i];
                jacobian[1, i] = element.getNode(0).Y1 * dNdKsi[0, i] + element.getNode(1).Y1 * dNdKsi[1, i] + element.getNode(2).Y1 * dNdKsi[2, i] + element.getNode(3).Y1 * dNdKsi[3, i];
                jacobian[2, i] = element.getNode(0).X1 * dNdEta[0, i] + element.getNode(1).X1 * dNdEta[1, i] + element.getNode(2).X1 * dNdEta[2, i] + element.getNode(3).X1 * dNdEta[3, i];
                jacobian[3, i] = element.getNode(0).Y1 * dNdEta[0, i] + element.getNode(1).Y1 * dNdEta[1, i] + element.getNode(2).Y1 * dNdEta[2, i] + element.getNode(3).Y1 * dNdEta[3, i];

                detJacobian[i] = jacobian[0, i] * jacobian[3, i] - jacobian[1, i] * jacobian[2, i];

                inversedJacobian[0, i] = jacobian[3, i] / detJacobian[i];
                inversedJacobian[1, i] = -(jacobian[1, i] / detJacobian[i]);
                inversedJacobian[2, i] = -(jacobian[2, i] / detJacobian[i]);
                inversedJacobian[3, i] = jacobian[0, i] / detJacobian[i];
            }

            
        }

        public void printJacobian()
        {
            for(int i =0;i<4;++i)
            {
                Console.WriteLine("j[" + i + ",0]=" + jacobian[i, 0]);
                Console.WriteLine("j[" + i + ",1]=" + jacobian[i, 1]);
                Console.WriteLine("j[" + i + ",2]=" + jacobian[i, 2]);
                Console.WriteLine("j[" + i + ",3]=" + jacobian[i, 3]);
            }
            
        }

        public void printDetJacobian()
        {
            for(int i=0;i<4;++i)
            {
                Console.WriteLine("detJ[" + i + "]=" + detJacobian[i]);
            }
        }

        public void printInversedJacobian()
        {
            for(int i=0;i<4;++i)
            {
                Console.WriteLine("invJ[" + i + ",0]=" + inversedJacobian[i, 0]);
                Console.WriteLine("invJ[" + i + ",1]=" + inversedJacobian[i, 1]);
                Console.WriteLine("invJ[" + i + ",2]=" + inversedJacobian[i, 2]);
                Console.WriteLine("invJ[" + i + ",3]=" + inversedJacobian[i, 3]);
            }
        }
            
    }
}
