﻿using System;
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
            grid.SetHeatedSurfaces(true, true, true, true);
            Jacobian jacobian = new Jacobian();
            Element testElem = new Element();
            testElem.SetSurface(0, true);
            //testElem.SetSurface(1, true);
            Node testNode1 = new Node(0.0, 0.0, 0.0);
            Node testNode2 = new Node(0.025, 0.0, 0.0);
            Node testNode3 = new Node(0.025, 0.025, 0.0);
            Node testNode4 = new Node(0.0, 0.025, 0.0);
            testElem.setNodesArray(ref testNode1, ref testNode2, ref testNode3, ref testNode4);
            MatrixH testMatrixH = new MatrixH();
            MatrixC testMatrixC = new MatrixC(700.0, 7800.0, jacobian.getDetJArray());
            MatrixH_BC testMatrixH_BC = new MatrixH_BC(25.0, testElem);
            VectorP testVectorP = new VectorP(300.0, 1200.0, testElem);
            jacobian.CalculateShapeFunctions();
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
            //testMatrixH.calculateMatrixH(jacobian,30.0);
            //testMatrixH.printDNdx();
            //testMatrixH.printDNdy();
            //testMatrixH.printDNdxT(3);
            //testMatrixH.printSum(3);
            //testMatrixH.printMatrixH();

            //testMatrixC.calculatePc();
            //testMatrixC.calculateMatrixC();
            //testMatrixC.printMatrixC();

            testMatrixC.CalculateMatrixC();
            //testMatrixC.print();
            testMatrixH_BC.CalculateMatrixH_BC();
            //testMatrixH_BC.print();
            //testMatrixC.printMatrixC();
            testElem.LocalVectorP=testVectorP.CalculateVectorP();
            //testVectorP.print();
            //Console.WriteLine(testElem.LocalVectorP.ToString());
        }
    }
}
