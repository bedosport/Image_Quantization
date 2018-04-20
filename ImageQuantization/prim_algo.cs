using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace ImageQuantization
{

    public class prim_algo
    {
        public static List<edges> Sort(List<edges> input)
        {
            List<edges> Result = new List<edges>();
            List<edges> Left = new List<edges>();
            List<edges> Right = new List<edges>();

            if (input.Count <= 1)
                return input;

            int midpoint = input.Count / 2;
            for (int i = 0; i < midpoint; i++)
                Left.Add(input[i]);
            for (int i = midpoint; i < input.Count; i++)
                Right.Add(input[i]);

            Left = Sort(Left); //Recursion! :o
            Right = Sort(Right);
            Result = Merge(Left, Right);

            return Result;
        }
        private static List<edges> Merge(List<edges> Left, List<edges> Right)
        {
            List<edges> Result = new List<edges>();
            while (Left.Count > 0 && Right.Count > 0)
            {
                if (Left[0].we > Right[0].we)
                {
                    Result.Add(Left[0]);
                    Left.RemoveAt(0);
                }
                else
                {
                    Result.Add(Right[0]);
                    Right.RemoveAt(0);
                }
            }

            while (Left.Count > 0)
            {
                Result.Add(Left[0]);
                Left.RemoveAt(0);
            }

            while (Right.Count > 0)
            {
                Result.Add(Right[0]);
                Right.RemoveAt(0);
            }

            return Result;
        }



        public static Tuple<double, List<edges>> prim_edit(List<int> x)
        {
            int n = x.Count();
            List<bool> vis = new List<bool>(new bool[n]);  // intalize it with zeros
            double mst_cost = 0;
            List<edges> e = new List<edges>(n-1);
            PriorityQueue<edges> q = new PriorityQueue<edges>();

            float[] arr = new float[n];

            for(int i=0;i<n;i++)
                arr[i]=float.MaxValue;

            q.Enqueue(new edges(-1, 0, 0)); // intialiy
            while (q.Count() > 0)
            {
                edges ee = q.Dequeue();             //O(1)
                if (vis[ee.to] == true)             // O(1)
                    continue;                       // O(1)
                vis[ee.to] = true;                  // O(1)
                mst_cost += (float)ee.we;           // O(1)
                if (ee.to != 0)   // to not push new edges(-1, 0, 0) the initial edge" // O(1)
                    e.Add(ee);                      // O(1)
                
                // loop throgh all list consider it fully connected

                for (int j = 0; j < n; j++)                         // O(V)   (V number of vertices in graph)
                {
                    if (j == ee.to || vis[j] == true)
                        continue;
                    
                    int r1 = (x[ee.to] >> 16) & 0xFF;             //O(1)
                    int g1 = (x[ee.to] >> 8) & 0xFF;              //O(1)
                    int b1 = x[ee.to] & 0xFF;                     //O(1)
                    int r2 = (x[j] >> 16) & 0xFF;                 //O(1)
                    int g2 = (x[j] >> 8) & 0xFF;                  //O(1)
                    int b2 = x[j] & 0xFF;                         //O(1)
                    double w = Math.Sqrt((r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2));  // O(1)
                    if (arr[j] > w)
                        arr[j] = (float)w;
                    else                                  // (j=ee.to) if i have smaller weight to that edge don't push it in queue 
                        continue;
                    q.Enqueue(new edges(ee.to, j, w));   //O(log(V)) (V number of vertices in graph)

                }
            }
            

            return new Tuple<double, List<edges>>(mst_cost, e);


        }






    }


}