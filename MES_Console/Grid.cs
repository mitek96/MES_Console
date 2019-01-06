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
        //Matrix<double>


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
        }

        public int CalculateNodeArray()
        {
            numberOfNodes = nH * nL;
            dx = L / (nL-1);
            dy = H / (nH-1);
            nodes = new Node[numberOfNodes];
            for(int i=0; i<numberOfNodes;++i)
            {
                nodes[i] = new Node();
            }
            for(int nX = 0; nX < nL;++nX)
            {
                for(int nY=0;nY<nH;++nY)
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
            if (numberOfElements<1) return 0;
            elements = new Element[numberOfElements];
            int dNodeX = nH;
            int offset=0;
            for(int i=0;i<numberOfElements;++i)
            {
                if (i != 0 && (i % (nH - 1)) == 0) offset++;
                elements[i] = new Element(i + offset + 1, i + dNodeX + offset + 1, i + dNodeX + offset + 2, i + offset + 2);
                elements[i].setNodesArray(ref nodes[i + offset], ref nodes[i + dNodeX + offset], ref nodes[i + dNodeX + offset + 1], ref nodes[i + offset + 1]);
            }

            return numberOfElements;
        }

        public void CalculateElements()
        {
            for(int i=0;i<numberOfElements;++i)
            {
                Jacobian jacobian = new Jacobian();
                jacobian.calculateJacobian(elements[i]);
                MatrixH tempMatrixH = new MatrixH();
                elements[i].LocalMatrixH = tempMatrixH.calculateMatrixH(jacobian, conductivity);
                MatrixC tempMatrixC = new MatrixC(specificHeat, density, jacobian.getDetJArray());
                MatrixH_BC tempMatrixH_BC = new MatrixH_BC(alpha, elements[i]);
                VectorP tempVectorP = new VectorP(alpha, ambientTemperature, elements[i]);
                elements[i].LocalMatrixC=tempMatrixC.CalculateMatrixC();
                elements[i].LocalMatrixH.Add(tempMatrixH_BC.CalculateMatrixH_BC());
                elements[i].LocalVectorP=tempVectorP.CalculateVectorP();
                elements[i].PrintLocalMatrices();
            }
        }

        public void CalculateFEM(double time, int iterations)
        {
            
        }

        public void PrintNodes()
        {
            for(int i=0;i<numberOfNodes;++i)
            {
                Console.WriteLine("NodeId:"+nodes[i].Id+"   NodeCoordinates: ("+nodes[i].X1+"|"+nodes[i].Y1+")");
            }
        }

        public void PrintNode(int node)
        {
            Console.WriteLine("NodeId:" + nodes[node].Id + "   NodeCoordinates: (" + nodes[node].X1 + "|" + nodes[node].Y1 + ")");
        }

        public void PrintNodeById(int id)
        {
            Console.WriteLine("NodeId:" + nodes[id-1].Id + "   NodeCoordinates: (" + nodes[id-1].X1 + "|" + nodes[id-1].Y1 + ")");
        }

        public void PrintElements()
        {
            for(int i=0;i<numberOfElements;++i)
            {
                int[] tmp = elements[i].getNodesId();
                Console.WriteLine("ElementId:"+i+"   ElementNodes: "+tmp[0]+","+tmp[1]+","+tmp[2] + "," + tmp[3]);
            }
        }

        public Element GetElement(int index)
        {
            return elements[index];
        }

        public void SetHeatedSurfaces(Boolean bottom, Boolean right,Boolean top,Boolean left)
        {
            if(bottom)
            {
                for(int i=0; i < nL - 1; ++i)
                elements[i * (nH - 1)].SetSurface(0,true);
            }

            if (right)
            {
                for (int i = ((nH - 1) * (nL - 1)) - 1; i > ((nH - 1) * (nL - 1)) - 1 - (nH - 1); i--)
                    elements[i].SetSurface(1, true);
            }

            if(top)
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
    }
}
