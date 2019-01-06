using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Milosz Ryba\documents\visual studio 2015\Projects\MES_Console\input.txt");
            double initTemperature = double.Parse(lines[0]);
            double simulationTime = double.Parse(lines[1]);
            double timeStep = double.Parse(lines[2]);
            double ambientTemperature = double.Parse(lines[3]);
            double alpha = double.Parse(lines[4]);
            double h = double.Parse(lines[5]);
            double l = double.Parse(lines[6]);
            int nH = int.Parse(lines[7]);
            int nL = int.Parse(lines[8]);
            double specificHeat = double.Parse(lines[9]);
            double conductivity = double.Parse(lines[10]);
            double density = double.Parse(lines[11]);
            Grid grid = new Grid(initTemperature, simulationTime, timeStep, ambientTemperature, alpha, h, l, nH, nL, specificHeat, conductivity, density);
            int nodeNumber = grid.CalculateNodeArray();
            Console.WriteLine("Number of Nodes: "+nodeNumber);
            grid.PrintNodes();
            int elementNumber = grid.CalculateElementArray();
            Console.WriteLine("Number of Elements: "+elementNumber);
            grid.PrintElements();
            grid.SetHeatedSurfaces(true, true, true, true);
          

            grid.CalculateElements();
            
            grid.AgregateaMatrices();
            grid.AgregateVectorP();
            grid.Heat();
            grid.PrintNodes();
            

        }
    }
}
