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
            int h = int.Parse(lines[0]);
            int l = int.Parse(lines[1]);
            int nH = int.Parse(lines[2]);
            int nL = int.Parse(lines[3]);
            Grid grid = new Grid(h, l, nH, nL);
            int nodeNumber = grid.CalculateNodeArray();
            Console.WriteLine("Number of Nodes: "+nodeNumber);
            grid.PrintNodes();
            int elementNumber = grid.CalculateElementArray();
            Console.WriteLine("Number of Elements: "+elementNumber);
            grid.PrintElements();
            Jacobian jacobian = new Jacobian();
            Element testElem = new Element();
            Node testNode1 = new Node(0.0, 0.0, 0.0);
            Node testNode2 = new Node(0.025, 0.0, 0.0);
            Node testNode3 = new Node(0.025, 0.025, 0.0);
            Node testNode4 = new Node(0.0, 0.025, 0.0);
            testElem.setNodesArray(ref testNode1, ref testNode2, ref testNode3, ref testNode4);
            MatrixH testMatrixH = new MatrixH();

            jacobian.calculateShapeFunctions();
            //jacobian.printShapeFunctions();
            jacobian.calculateInterpolatedCoordinates(testElem);
            //jacobian.printInterpolatedCoordinates();
            jacobian.calculateShapeFunctionsDerivatives();
            //jacobian.printShapeFunctionsDerivativesKsi();
            //jacobian.printShapeFunctionsDerivativesEta();
            jacobian.calculateJacobian(testElem);
            //jacobian.printJacobian();
            //jacobian.printDetJacobian();
            //jacobian.printInversedJacobian();
            testMatrixH.calculateMatrixH(jacobian,30.0);
            //testMatrixH.printDNdx();
            //testMatrixH.printDNdy();
            //testMatrixH.printDNdxT(3);
            //testMatrixH.printSum(3);
            testMatrixH.printMatrixH();
        }
    }
}
