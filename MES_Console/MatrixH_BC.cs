using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{
    struct PointKsiEta
    {
        public double ksi, eta;
        public void SetCoordinates(double ksi, double eta)
        {
            this.ksi = ksi;
            this.eta = eta;
        }
    };

    struct Surface
    {
        public Matrix<double> shapeFuncPc1, shapeFuncPc2, pc1, pc2, sum;
    };

    class MatrixH_BC
    {

        double alpha;
        Element element;
        PointKsiEta[] integralPoints;
        double[] detJ;
        double[] length;
        Matrix<double> matrixH_BC;

        public MatrixH_BC(double alpha, Element element)
        {
            this.alpha = alpha;
            this.element = element;

            integralPoints = new PointKsiEta[8];
            length = new double[4];
            detJ = new double[4];
            matrixH_BC = Matrix<double>.Build.Dense(4, 4, 0.0);
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
            result.pc1 = result.shapeFuncPc1.Transpose().Multiply(result.shapeFuncPc1);
            result.pc1 = result.pc1.Multiply(alpha);
            result.pc2 = result.shapeFuncPc2.Transpose().Multiply(result.shapeFuncPc2);
            result.pc2 = result.pc2.Multiply(alpha);
            result.sum = result.pc1.Add(result.pc2);
            result.sum = result.sum.Multiply(detJ);
            return result;
        }

        public Matrix<double> CalculateMatrixH_BC()
        {
            Surface bottom, right, top, left;

            bottom = CalculateSurface(integralPoints[0], integralPoints[1], detJ[0]);
            right = CalculateSurface(integralPoints[2], integralPoints[3], detJ[1]);
            top = CalculateSurface(integralPoints[4], integralPoints[5], detJ[2]);
            left = CalculateSurface(integralPoints[6], integralPoints[7], detJ[3]);

            if (element.GetSurface(0)) matrixH_BC = matrixH_BC.Add(bottom.sum);
            if (element.GetSurface(1)) matrixH_BC = matrixH_BC.Add(right.sum);
            if (element.GetSurface(2)) matrixH_BC = matrixH_BC.Add(top.sum);
            if (element.GetSurface(3)) matrixH_BC = matrixH_BC.Add(left.sum);

            return matrixH_BC;
        }

        public void print()
        {
            Console.Write(matrixH_BC.ToString());
        }

    }
}

