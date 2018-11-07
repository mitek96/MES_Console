using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Console
{
    class MatrixH
    {

        double[,] matrixH;
        double[,] dNdx;
        double[,] dNdy;
        double[,,] dNdxT;
        double[,,] dNdyT;
        double[,,] dNdxTdetJ;
        double[,,] dNdyTdetJ;
        double[,,] sum;
        double[,] H;

        public MatrixH()
        {
            matrixH = new double[4, 4];
            dNdx = new double[4, 4];
            dNdy = new double[4, 4];
            dNdxT = new double[4, 4, 4];
            dNdyT = new double[4, 4, 4];
            dNdxTdetJ = new double[4, 4, 4];
            dNdyTdetJ = new double[4, 4, 4];
            sum = new double[4, 4, 4];
            H = new double[4, 4];
        }


        public void calculateMatrixH(Jacobian jacobian,double k)
        {
            for(int i=0;i<4;++i)
            {
                dNdx[0, i] = jacobian.getInversedJacobian(0, 0) * jacobian.getDNdKsi(i, 0) + jacobian.getInversedJacobian(1, 0) * jacobian.getDNdEta(i, 0);
                dNdx[1, i] = jacobian.getInversedJacobian(0, 1) * jacobian.getDNdKsi(i, 1) + jacobian.getInversedJacobian(1, 1) * jacobian.getDNdEta(i, 1);
                dNdx[2, i] = jacobian.getInversedJacobian(0, 2) * jacobian.getDNdKsi(i, 2) + jacobian.getInversedJacobian(1, 2) * jacobian.getDNdEta(i, 2);
                dNdx[3, i] = jacobian.getInversedJacobian(0, 3) * jacobian.getDNdKsi(i, 3) + jacobian.getInversedJacobian(1, 3) * jacobian.getDNdEta(i, 3);

                dNdy[0, i] = jacobian.getInversedJacobian(2, 0) * jacobian.getDNdKsi(i, 0) + jacobian.getInversedJacobian(3, 0) * jacobian.getDNdEta(i, 0);
                dNdy[1, i] = jacobian.getInversedJacobian(2, 1) * jacobian.getDNdKsi(i, 1) + jacobian.getInversedJacobian(3, 1) * jacobian.getDNdEta(i, 1);
                dNdy[2, i] = jacobian.getInversedJacobian(2, 2) * jacobian.getDNdKsi(i, 2) + jacobian.getInversedJacobian(3, 2) * jacobian.getDNdEta(i, 2);
                dNdy[3, i] = jacobian.getInversedJacobian(2, 3) * jacobian.getDNdKsi(i, 3) + jacobian.getInversedJacobian(3, 3) * jacobian.getDNdEta(i, 3);

            }


            for(int i=0;i<4;++i)
            {
                for(int j=0;j<4;++j)
                {
                    for(int z=0;z<4;++z)
                    {
                        dNdxT[i, j, z] = dNdx[i, j] * dNdx[i, z];
                        dNdyT[i, j, z] = dNdy[i, j] * dNdy[i, z];

                        dNdxTdetJ[i, j, z] = dNdxT[i, j, z] * jacobian.getDetJ(i);
                        dNdyTdetJ[i, j, z] = dNdyT[i, j, z] * jacobian.getDetJ(i);

                        sum[i, j, z] = (dNdxTdetJ[i, j, z] + dNdyTdetJ[i, j, z]) * k;
                    }
                }
            }

            for(int i=0;i<4;++i)
            {
                for(int j=0;j<4;++j)
                {
                    H[i, j] = sum[0, i, j] + sum[1, i, j] + sum[2, i, j] + sum[3, i, j];
                }
            }
        }

        public void printDNdxT(int pcIndex)
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("dNdxT[" + pcIndex + ","+ i + ",0]=" + dNdxT[pcIndex, i, 0]);
                Console.WriteLine("dNdxT[" + pcIndex + ","+ i + ",1]=" + dNdxT[pcIndex, i, 1]);
                Console.WriteLine("dNdxT[" + pcIndex + ","+ i + ",2]=" + dNdxT[pcIndex, i, 2]);
                Console.WriteLine("dNdxT[" + pcIndex + ","+ i + ",3]=" + dNdxT[pcIndex, i, 3]);
            }
            
        }

        public void printDNdxTdetJ(int pcIndex)
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("dNdxTdetJ[" + pcIndex + "," + i + ",0]=" + dNdxTdetJ[pcIndex, i, 0]);
                Console.WriteLine("dNdxTdetJ[" + pcIndex + "," + i + ",1]=" + dNdxTdetJ[pcIndex, i, 1]);
                Console.WriteLine("dNdxTdetJ[" + pcIndex + "," + i + ",2]=" + dNdxTdetJ[pcIndex, i, 2]);
                Console.WriteLine("dNdxTdetJ[" + pcIndex + "," + i + ",3]=" + dNdxTdetJ[pcIndex, i, 3]);
            }
        }

        public void printSum(int pcIndex)
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("sum[" + pcIndex + "," + i + ",0]=" + sum[pcIndex, i, 0]);
                Console.WriteLine("sum[" + pcIndex + "," + i + ",1]=" + sum[pcIndex, i, 1]);
                Console.WriteLine("sum[" + pcIndex + "," + i + ",2]=" + sum[pcIndex, i, 2]);
                Console.WriteLine("sum[" + pcIndex + "," + i + ",3]=" + sum[pcIndex, i, 3]);
            }
        }

        public void printMatrixH()
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("H[" + i + ",0]=" + H[i, 0]);
                Console.WriteLine("H[" + i + ",1]=" + H[i, 1]);
                Console.WriteLine("H[" + i + ",2]=" + H[i, 2]);
                Console.WriteLine("H[" + i + ",3]=" + H[i, 3]);
            }
        }

        public void printDNdx()
        {
            for(int i=0;i<4;++i)
            {
                Console.WriteLine("dNdx[" + i + ",0]=" + dNdx[i, 0]);
                Console.WriteLine("dNdx[" + i + ",1]=" + dNdx[i, 1]);
                Console.WriteLine("dNdx[" + i + ",2]=" + dNdx[i, 2]);
                Console.WriteLine("dNdx[" + i + ",3]=" + dNdx[i, 3]);
            }
        }

        public void printDNdy()
        {
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("dNdy[" + i + ",0]=" + dNdy[i, 0]);
                Console.WriteLine("dNdy[" + i + ",1]=" + dNdy[i, 1]);
                Console.WriteLine("dNdy[" + i + ",2]=" + dNdy[i, 2]);
                Console.WriteLine("dNdy[" + i + ",3]=" + dNdy[i, 3]);
            }
        }


    }
}
