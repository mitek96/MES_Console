using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace MES_Console
{
    class Grid
    {
        Node[] nodes;
        Element[] elements;
        private double H, L;
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
        double[,] data;

        string workingDirectory, pathToSavePlot;



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
            data = new double[nH, nL];

            workingDirectory = Environment.CurrentDirectory;
            pathToSavePlot = Directory.GetParent(workingDirectory).Parent.FullName + "\\..\\plot\\" + DateTime.UtcNow.ToString().Replace(' ', '_').Replace(':', '_');
            Directory.CreateDirectory(pathToSavePlot);
        }

        public Element GetElement(int index)
        {
            return elements[index];
        }

        public int CalculateNodeArray()
        {
            numberOfNodes = nH * nL;
            dx = L / (nL - 1);
            dy = H / (nH - 1);
            nodes = new Node[numberOfNodes];
            for (int i = 0; i < numberOfNodes; ++i)
            {
                nodes[i] = new Node
                {
                    T = initTemperature
                };
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

        public void CalculateLocalMatricesAndVector()
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
                //elements[i].PrintLocalMatrices();
            }
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

        public void AgregateaMatricesAndVector()
        {
            for (int i = 0; i < numberOfElements; ++i)
            {
                for (int iLocal = 0; iLocal < 4; ++iLocal)
                {
                    int iGlobal = elements[i].getNode(iLocal).Id - 1;

                    double valuePLocal = elements[i].LocalVectorP[iLocal];
                    globalVectorP[iGlobal] += valuePLocal;

                    for (int jLocal = 0; jLocal < 4; ++jLocal)
                    {
                        
                        int jGlobal = elements[i].getNode(jLocal).Id - 1;

                        double valueHLocal = elements[i].LocalMatrixH[iLocal, jLocal];
                        globalMatrixH[iGlobal, jGlobal] += valueHLocal;

                        double valueCLocal = elements[i].LocalMatrixC[iLocal, jLocal];
                        globalMatrixC[iGlobal, jGlobal] += valueCLocal;
                    }
                }
            }
            PrintGlobalMatrices();
            PrintGlobalVector();
        }

        public void Heat()
        {
            var linearModel = new PlotModel { Title = "MIN/MAX" };
            var lineSeries = new LineSeries();
            var lineSeries2 = new LineSeries();
            lineSeries.Points.Add(new DataPoint(0, initTemperature));
            lineSeries.Title = "Min";
            lineSeries2.Points.Add(new DataPoint(0, initTemperature));
            lineSeries2.Title = "Max";
            linearModel.Axes.Add(new LinearAxis {Position=AxisPosition.Bottom, Title="Time[s]" });
            linearModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Temperature[C]" });


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
                lineSeries.Points.Add(new DataPoint(time, t1.Min()));
                lineSeries2.Points.Add(new DataPoint(time, t1.Max()));
                PutTemperaturesToArray();
                //Plot form1 = new Plot(data, nH, nL, ambientTemperature);
                //form1.ShowDialog();
                SavePlot(time);

            }

            linearModel.Series.Add(lineSeries);
            linearModel.Series.Add(lineSeries2);
            linearModel.LegendPosition = 0;
            linearModel.IsLegendVisible = true;
            var pngExporter = new PngExporter { Width = 800, Height = 800, Background = OxyColors.White };


            pngExporter.ExportToFile(linearModel, pathToSavePlot + "\\" + "minMax" + ".png");

        }

        public void SavePlot(double t)
        {
            var model = new PlotModel { Title = "GRID" };


            // Color axis


            model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(1200), Maximum = 1200, Minimum = 0 });


            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = nL,
                Y0 = 0,
                Y1 = nH,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);

            var pngExporter = new PngExporter { Width = 1500, Height = 1500, Background = OxyColors.White };


            pngExporter.ExportToFile(model, pathToSavePlot + "\\" + t.ToString() + ".png");
        }

        public void PutTemperaturesToArray()
        {
            int s = 0;
            for (int i = 0; i < nH; ++i)
            {
                for (int j = 0; j < nL; ++j)
                {
                    data[i, j] = nodes[s].T;
                    s++;
                }
            }
        }

        public void PrintGlobalMatrices()
        {
            Console.WriteLine("MatrixH+H_BC\n" + globalMatrixH.ToString());
            Console.WriteLine("MatrixC\n" + globalMatrixC.ToString());
        }

        public void PrintGlobalVector()
        {
            Console.WriteLine("VectorP\n" + globalVectorP.ToString());
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
    }
}
