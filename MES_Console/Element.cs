using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MES_Console
{
    class Element
    {
        static int idReference = 0;
        int[] idArray;
        //ArrayList nodeList;
        List<Node> nodeList;
        bool[] isSurfaceHeated;

        public int Id { get; set; }

        public double K1 { get; set; }
        public Matrix<double> LocalMatrixH { get; set; }
        public Matrix<double> LocalMatrixC { get; set; }
        public Vector<double> LocalVectorP { get; set; }

        public Element()
        {
            idArray = new int[4];
            isSurfaceHeated = new bool[4];
            Id = ++idReference;
        }

        public Element(int id1,int id2,int id3,int id4)
        {
            idArray = new int[4];
            isSurfaceHeated = new bool[4];
            idArray[0] = id1;
            idArray[1] = id2;
            idArray[2] = id3;
            idArray[3] = id4;
            Id = ++idReference;
        }

        public int [] getNodesId()
        {
            int [] returnId = new int[4];
            for(int i=0;i<4;++i)
            {
                returnId[i] = idArray[i];
            }
            return returnId;
        }

        public void setNodesId(int id1, int id2, int id3, int id4)
        {
            idArray[0] = id1;
            idArray[1] = id2;
            idArray[2] = id3;
            idArray[3] = id4;
        }

        public void setNodesArray(ref Node node1, ref Node node2, ref Node node3, ref Node node4)
        {
            nodeList = new List<Node>();
            nodeList.Add(node1);
            nodeList.Add(node2);
            nodeList.Add(node3);
            nodeList.Add(node4);
        }

        public Node getNode(int index)
        {
            return nodeList[index];
        }

        public void SetSurface(int index, bool value)
        {
            isSurfaceHeated[index] = value;
        }

        public bool GetSurface(int index)
        {
            return isSurfaceHeated[index]; 
        }

        public void PrintLocalMatrices()
        {
            Console.WriteLine("elID=" + Id);
            Console.WriteLine("MatrixH\n"+LocalMatrixH.ToString());
            Console.WriteLine("MatrixC\n" + LocalMatrixC.ToString());
            Console.WriteLine("VectorP\n" + LocalVectorP.ToString());
        }
    }
}
