using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{
    class MatrixC
    {
        double[] ksi;
        double[] eta;
        double[] detJ;

        double C, ro;
        Matrix<double> matrixC;
        Matrix<double>[] pc; //integral points matrices

        public MatrixC(double C, double ro, double[] detJ)
        {
            matrixC = Matrix<double>.Build.Dense(4, 4);
            ksi = new double[4];
            eta = new double[4];

            pc = new Matrix<double>[4];
            for (int i = 0; i < 4; ++i)
            {
                pc[i] = Matrix<double>.Build.Dense(4, 4);
            }

            this.C = C;
            this.ro = ro;
            this.detJ = detJ;
            ksi[0] = -1.0 / Math.Sqrt(3);
            ksi[1] = -ksi[0];
            ksi[2] = ksi[1];
            ksi[3] = -ksi[2];

            eta[0] = ksi[0];
            eta[1] = eta[0];
            eta[2] = -eta[1];
            eta[3] = eta[2];
        }

        public Matrix<double> CalculateMatrixC()
        {
            for (int i = 0; i < 4; ++i)
            {
                matrixC[i, 0] = 0.25 * (1 - ksi[i]) * (1 - eta[i]);
                matrixC[i, 1] = 0.25 * (1 + ksi[i]) * (1 - eta[i]);
                matrixC[i, 2] = 0.25 * (1 + ksi[i]) * (1 + eta[i]);
                matrixC[i, 3] = 0.25 * (1 - ksi[i]) * (1 + eta[i]);
            }

            for (int i = 0; i < 4; ++i)
            {
                pc[i] = matrixC.Row(i).ToColumnMatrix() * matrixC.Row(i).ToRowMatrix();
                double toMul = C * ro * detJ[i];
                pc[i] = pc[i].Multiply(toMul);
            }

            matrixC.Clear();

            for (int i = 0; i < 4; ++i)
            {
                matrixC = matrixC.Add(pc[i]);
            }

            return matrixC;
        }

        public void Print()
        {
            Console.Write(matrixC.ToString());
        }

    }
}
