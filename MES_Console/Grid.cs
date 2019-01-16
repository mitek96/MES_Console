using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{
    class Grid
    {
        Node[] nodes;
        Element[] elements;

        double H, L;
        double dx, dy;
        int nH, nL, numberOfNodes, numberOfElements;
        static double startX = 0.0;
        static double startY = 0.0;
        private double initTemperature;
        private double simulationTime;
        private double timeStep;
        private double ambientTemperature;
        private double alpha;
        private double specificHeat;
        private double conductivity;
        private double density;
        Matrix<double> globalMatrixH, globalMatrixC;
        Vector<double> globalVectorP;


        public Grid(double initTemperature, double simulationTime, double timeStep, double ambientTemperature, double alpha, double h, double l, int nH, int nL, double specificHeat, double conductivity, double density)
        {
            this.initTemperature = initTemperature;
            this.simulationTime = simulationTime;
            this.timeStep = timeStep;
            this.ambientTemperature = ambientTemperature;
            this.alpha = alpha;
            H = h;
            L = l;
            this.nH = nH;
            this.nL = nL;
            this.specificHeat = specificHeat;
            this.conductivity = conductivity;
            this.density = density;
            globalMatrixH = Matrix<double>.Build.Dense(nH * nL, nH * nL);
            globalMatrixC = Matrix<double>.Build.Dense(nH * nL, nH * nL);
            globalVectorP = Vector<double>.Build.Dense(nH * nL);
        }

        public int CalculateNodeArray()
        {
            numberOfNodes = nH * nL;
            dx = L / (nL - 1);
            dy = H / (nH - 1);
            nodes = new Node[numberOfNodes];
            for (int i = 0; i < numberOfNodes; ++i)
            {
                nodes[i] = new Node();
                nodes[i].T = initTemperature;
            }
            for (int nX = 0; nX < nL; ++nX)
            {
                for (int nY = 0; nY < nH; ++nY)
                {
                    if (nY == nH - 1) nodes[nY + (nX * nH)].Y1 = startY + H;
                    else nodes[nY + (nX * nH)].Y1 = startY + (nY * dy);

                    if (nX == nL - 1) nodes[nY + (nX * nH)].X1 = startX + L;
                    else nodes[nY + (nX * nH)].X1 = startX + (nX * dx);
                }
            }
            return numberOfNodes;
        }

        public int CalculateElementArray()
        {

            numberOfElements = (nH - 1) * (nL - 1);
            if (numberOfElements < 1) return 0;
            elements = new Element[numberOfElements];
            int dNodeX = nH;
            int offset = 0;
            for (int i = 0; i < numberOfElements; ++i)
            {
                if (i != 0 && (i % (nH - 1)) == 0) offset++;
                elements[i] = new Element(i + offset + 1, i + dNodeX + offset + 1, i + dNodeX + offset + 2, i + offset + 2);
                elements[i].setNodesArray(ref nodes[i + offset], ref nodes[i + dNodeX + offset], ref nodes[i + dNodeX + offset + 1], ref nodes[i + offset + 1]);
            }

            return numberOfElements;
        }

        public void CalculateElements()
        {
            for (int i = 0; i < numberOfElements; ++i)
            {
                Jacobian jacobian = new Jacobian();
                jacobian.calculateJacobian(elements[i]);
                MatrixH tempMatrixH = new MatrixH();
                elements[i].LocalMatrixH = tempMatrixH.calculateMatrixH(jacobian, conductivity);
                MatrixC tempMatrixC = new MatrixC(specificHeat, density, jacobian.getDetJArray());
                MatrixH_BC tempMatrixH_BC = new MatrixH_BC(alpha, elements[i]);
                VectorP tempVectorP = new VectorP(alpha, ambientTemperature, elements[i]);
                elements[i].LocalMatrixC = tempMatrixC.CalculateMatrixC();
                elements[i].LocalMatrixH = elements[i].LocalMatrixH.Add(tempMatrixH_BC.CalculateMatrixH_BC());
                elements[i].LocalVectorP = tempVectorP.CalculateVectorP();
                elements[i].PrintLocalMatrices();
            }
        }


        public void PrintNodes()
        {
            for (int i = 0; i < numberOfNodes; ++i)
            {
                Console.WriteLine("NodeId:" + nodes[i].Id + "   NodeCoordinates: (" + nodes[i].X1 + "|" + nodes[i].Y1 + ") T=" + nodes[i].T);
            }
        }

        public void PrintNode(int node)
        {
            Console.WriteLine("NodeId:" + nodes[node].Id + "   NodeCoordinates: (" + nodes[node].X1 + "|" + nodes[node].Y1 + ")");
        }

        public void PrintNodeById(int id)
        {
            Console.WriteLine("NodeId:" + nodes[id - 1].Id + "   NodeCoordinates: (" + nodes[id - 1].X1 + "|" + nodes[id - 1].Y1 + ")");
        }

        public void PrintElements()
        {
            for (int i = 0; i < numberOfElements; ++i)
            {
                int[] tmp = elements[i].getNodesId();
                Console.WriteLine("ElementId:" + i + "   ElementNodes: " + tmp[0] + "," + tmp[1] + "," + tmp[2] + "," + tmp[3]);
            }
        }

        public Element GetElement(int index)
        {
            return elements[index];
        }

        public void SetHeatedSurfaces(Boolean bottom, Boolean right, Boolean top, Boolean left)
        {
            if (bottom)
            {
                for (int i = 0; i < nL - 1; ++i)
                    elements[i * (nH - 1)].SetSurface(0, true);
            }

            if (right)
            {
                for (int i = ((nH - 1) * (nL - 1)) - 1; i > ((nH - 1) * (nL - 1)) - 1 - (nH - 1); i--)
                    elements[i].SetSurface(1, true);
            }

            if (top)
            {
                for (int i = 0; i < nL - 1; i++)
                    elements[i * (nH - 1) + (nH - 2)].SetSurface(2, true);
            }

            if (left)
            {
                for (int i = 0; i < nH - 1; i++)
                    elements[i].SetSurface(3, true);
            }
        }

        public void AgregateaMatrices()
        {
            for (int i = 0; i < numberOfElements; ++i)
            {
                for (int iLocal = 0; iLocal < 4; ++iLocal)
                {
                    for (int jLocal = 0; jLocal < 4; ++jLocal)
                    {
                        int iGlobal = elements[i].getNode(iLocal).Id - 1;
                        int jGlobal = elements[i].getNode(jLocal).Id - 1;

                        double valueHLocal = elements[i].LocalMatrixH[iLocal, jLocal];
                        globalMatrixH[iGlobal, jGlobal] += valueHLocal;

                        double valueCLocal = elements[i].LocalMatrixC[iLocal, jLocal];
                        globalMatrixC[iGlobal, jGlobal] += valueCLocal;
                    }
                }
            }
            PrintGlobalMatrices();
        }

        public void AgregateVectorP()
        {
            for (int i = 0; i < numberOfElements; ++i)
            {
                for (int iLocal = 0; iLocal < 4; ++iLocal)
                {
                    int iGlobal = elements[i].getNode(iLocal).Id - 1;

                    double valuePLocal = elements[i].LocalVectorP[iLocal];
                    globalVectorP[iGlobal] += valuePLocal;
                }
            }

            PrintGlobalVector();
        }

        public void Heat()
        {
            Matrix<double> leftEquation = globalMatrixH.Add(globalMatrixC.Divide(timeStep));
            Console.WriteLine("H+C/dt" + leftEquation.ToString());
            for (double time = timeStep; time <= simulationTime; time += timeStep)
            {
                Vector<double> t0 = Vector<double>.Build.Dense(numberOfNodes);
                for (int i = 0; i < numberOfNodes; ++i)
                {
                    t0[i] = nodes[i].T;
                }

                Vector<double> rightEquation = -globalVectorP + globalMatrixC.Divide(timeStep).Multiply(t0);
                Vector<double> t1 = leftEquation.Solve(rightEquation);

                for (int i = 0; i < numberOfNodes; ++i)
                {
                    nodes[i].T = t1[i];
                }

                Console.WriteLine("Interval=" + time);
                Console.WriteLine("======================================");
                Console.WriteLine("Min=" + t1.Min());
                Console.WriteLine("Max=" + t1.Max());
                Console.WriteLine("P" + rightEquation.ToString());

            }

        }

        public void PrintGlobalMatrices()
        {
            Console.WriteLine("MatrixH\n" + globalMatrixH.ToString());
            Console.WriteLine("MatrixC\n" + globalMatrixC.ToString());
        }

        public void PrintGlobalVector()
        {
            Console.WriteLine("VectorP\n" + globalVectorP.ToString());
        }
    }
}
