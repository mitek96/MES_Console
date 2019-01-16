using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MES_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string[] lines = System.IO.File.ReadAllLines(Directory.GetParent(workingDirectory).Parent.FullName + "\\..\\input.txt");    //read input file
            //parse values from file:
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


            Grid grid = new Grid(initTemperature, simulationTime, timeStep, ambientTemperature, alpha, h, l, nH, nL, specificHeat, conductivity, density);  //create new grid
            int nodeNumber = grid.CalculateNodeArray(); //create nodes
            //Console.WriteLine("Number of Nodes: " + nodeNumber);
            //grid.PrintNodes();    //print nodes
            int elementNumber = grid.CalculateElementArray();   //create elements
            //Console.WriteLine("Number of Elements: " + elementNumber);
            //grid.PrintElements(); //print elements
            grid.SetHeatedSurfaces(true, true, true, true); //set boundary conditions
            grid.CalculateLocalMatricesAndVector();   //calculate local matrices and vector for each element
            grid.AgregateaMatricesAndVector();   //agregate matrices and vector
            grid.Heat();    //start grid heating

        }
    }
}
