using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{

    class VectorP
    {
        double alpha;
        double ambientTemperature;
        Element element;
        Vector<double> vectorP;
        PointKsiEta[] integralPoints;
        double[] detJ;
        double[] length;

        public VectorP(double alpha, double ambientTemperature, Element element)
        {
            this.alpha = alpha;
            this.ambientTemperature = ambientTemperature;
            this.element = element;
            vectorP = Vector<double>.Build.Dense(4);
            integralPoints = new PointKsiEta[8];
            length = new double[4];
            detJ = new double[4];

            Init();
        }

        public void Init()
        {

            integralPoints[0].SetCoordinates(-1.0 / Math.Sqrt(3.0), -1.0);
            integralPoints[1].SetCoordinates(1.0 / Math.Sqrt(3.0), -1.0);
            integralPoints[2].SetCoordinates(1.0, -1.0 / Math.Sqrt(3.0));
            integralPoints[3].SetCoordinates(1.0, 1.0 / Math.Sqrt(3.0));
            integralPoints[4].SetCoordinates(1.0 / Math.Sqrt(3.0), 1.0);
            integralPoints[5].SetCoordinates(-1.0 / Math.Sqrt(3.0), 1.0);
            integralPoints[6].SetCoordinates(-1.0, 1.0 / Math.Sqrt(3.0));
            integralPoints[7].SetCoordinates(-1.0, -1.0 / Math.Sqrt(3.0));

            length[0] = Math.Sqrt(Math.Pow(element.getNode(1).X1 - element.getNode(0).X1, 2) + Math.Pow(element.getNode(1).Y1 - element.getNode(0).Y1, 2));
            length[1] = Math.Sqrt(Math.Pow(element.getNode(1).X1 - element.getNode(2).X1, 2) + Math.Pow(element.getNode(1).Y1 - element.getNode(2).Y1, 2));
            length[2] = Math.Sqrt(Math.Pow(element.getNode(2).X1 - element.getNode(3).X1, 2) + Math.Pow(element.getNode(2).Y1 - element.getNode(3).Y1, 2));
            length[3] = Math.Sqrt(Math.Pow(element.getNode(0).X1 - element.getNode(3).X1, 2) + Math.Pow(element.getNode(0).Y1 - element.getNode(3).Y1, 2));

            detJ[0] = length[0] / 2.0;  //side is 1D, detJ=length/2
            detJ[1] = length[1] / 2.0;
            detJ[2] = length[2] / 2.0;
            detJ[3] = length[3] / 2.0;
        }

        private Matrix<double> CalculateShapeFunctions(PointKsiEta integralPoint)
        {
            return Matrix<double>.Build.DenseOfRowArrays(new double[] { 0.25 * (1 - integralPoint.ksi) * (1 - integralPoint.eta), 0.25 * (1 + integralPoint.ksi) * (1 - integralPoint.eta), 0.25 * (1 + integralPoint.ksi) * (1 + integralPoint.eta), 0.25 * (1 - integralPoint.ksi) * (1 + integralPoint.eta) });
        }

        private Surface CalculateSurface(PointKsiEta point1, PointKsiEta point2, double detJ)
        {
            Surface result = new Surface();
            result.shapeFuncPc1 = CalculateShapeFunctions(point1);
            result.shapeFuncPc2 = CalculateShapeFunctions(point2);
            result.pc1 = result.shapeFuncPc1.Multiply((-alpha) * ambientTemperature);
            result.pc2 = result.shapeFuncPc2.Multiply((-alpha) * ambientTemperature);
            result.sum = result.pc1.Add(result.pc2);
            result.sum = result.sum.Multiply(detJ);
            return result;
        }

        public Vector<double> CalculateVectorP()
        {
            Surface bottom, right, top, left;

            bottom = CalculateSurface(integralPoints[0], integralPoints[1], detJ[0]);
            right = CalculateSurface(integralPoints[2], integralPoints[3], detJ[1]);
            top = CalculateSurface(integralPoints[4], integralPoints[5], detJ[2]);
            left = CalculateSurface(integralPoints[6], integralPoints[7], detJ[3]);



            if (element.GetSurface(0)) vectorP = vectorP.Add(bottom.sum.Row(0));
            if (element.GetSurface(1)) vectorP = vectorP.Add(right.sum.Row(0));
            if (element.GetSurface(2)) vectorP = vectorP.Add(top.sum.Row(0));
            if (element.GetSurface(3)) vectorP = vectorP.Add(left.sum.Row(0));

            return vectorP;
        }

        public void Print()
        {
            Console.Write(vectorP.ToString());
        }

    }
}
