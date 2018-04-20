using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImageQuantization
{

    /// <summary>
    /// ///////////////////////////////////////////////////////////
    /// </summary>
   

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        HashSet<int> distinct = new HashSet<int>();
        List<int> my_distinct = new List<int>();
        Tuple<double, List<edges>> mst_edges;
        Stopwatch stopwatch = new Stopwatch();
        int []  agian = new int [100000000];
        bool[] MY_visited = new bool[100000000];
	
       
        private void btnOpen_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                
                string OpenedFilePath = openFileDialog1.FileName;
                
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            get_distict_colors();
        }

        public int get_avg (List<int> l)
        {
            int s = l.Count;
            int r, g, b;
            r=g=b=0;
            
            for(int i=0;i<s;i++)
            {
                r += (my_distinct[l[i]] >> 16) & 0xFF;
                g += (my_distinct[l[i]] >> 8) & 0xFF;
                b += my_distinct[l[i]] & 0xFF;
            }
            r/=s;
            g/=s;
            b/=s;
            int avg_rgb =r;
            avg_rgb = (avg_rgb << 8) + g;
            avg_rgb = (avg_rgb << 8) + b;
            for (int i = 0; i < s; i++)
            {
                agian[(my_distinct[l[i]])] = avg_rgb;
                
            }
            return avg_rgb;
            
        }
       

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            // Begin timing.
            stopwatch.Start();

           int k = int.Parse(k_txt.Text);

           mst_edges = prim_algo.prim_edit(my_distinct);
           textBox1.Text =(Math.Round(mst_edges.Item1,1)).ToString();

         //   for (int i = 0; i < mst_edges.Item2.Count; i++)
         //       MessageBox.Show(mst_edges.Item2[i].ToString());

            List<edges> sortedMST = prim_algo.Sort(mst_edges.Item2);                           //;;  SORT

            if (k-1 > sortedMST.Count||k==1||k==0)
            {
                MessageBox.Show("K is not allawed!");
                return;
            }
            if (my_distinct.Count == 0)
            {
                MessageBox.Show("No Image entered !");
                return;
            }
           //MessageBox.Show(mst_edges.Item2.Count.ToString() + "edges ");


            int[] my_clusters = new int[sortedMST.Count+1];

            
            Graph g = new Graph(my_distinct.Count, false);                  //  vertices    ->(undirected)

            for (int i = k - 1; i < sortedMST.Count; i++)                    // skip longest k-1 edges then buit graph from the rest
                g.AddEdge(sortedMST[i].from, sortedMST[i].to);

            for (int index = 0; index < my_clusters.Length; index++)
                 my_clusters[index] = get_avg(g.BreadthFirstSearch(index));
            
                 // all its values is zero
            
            
            for (int i = 0; i < ImageOperations.GetHeight(ImageMatrix); i++)
            {
                for (int j = 0; j < ImageOperations.GetWidth(ImageMatrix); j++)
                {
                    
                    int rgb = ImageMatrix[i, j].red;
                    rgb = (rgb << 8) + ImageMatrix[i, j].green;
                    rgb = (rgb << 8) + ImageMatrix[i, j].blue;
                   
                    int rgb1 = agian[rgb];

                    int r11 = (rgb1 >> 16) & 0xFF;
                    int g11 = (rgb1 >> 8) & 0xFF;
                    int b11 = rgb1 & 0xFF;
                    //mst_edges.Item2.Clear();
                    ImageMatrix[i, j].red = (byte)r11;
                    ImageMatrix[i, j].green = (byte)g11;
                    ImageMatrix[i, j].blue = (byte)b11;

                }
            }

                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);

                stopwatch.Stop();

                // Write result.
                textBox2.Text = stopwatch.Elapsed.ToString();
                
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (my_distinct.Count == 0)
            {
                MessageBox.Show("No Image entered !");
                return;
            }
            mst_edges = prim_algo.prim_edit(my_distinct);
            textBox1.Text = mst_edges.Item1.ToString();
            
            //MessageBox.Show("mst cost " + mst_edges.Item1.ToString());
            

           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        public void get_distict_colors()     // O(n*m)      overall
        {

            distinct.Clear();       // hashset
            my_distinct.Clear(); ;  // list with values of the hashset

            for (int i = 0; i < ImageOperations.GetHeight(ImageMatrix); i++)             //  O(n)  n-> height of the image
            {
                for (int j = 0; j < ImageOperations.GetWidth(ImageMatrix); j++)          //  O(m)   m-> width of the image
                {
                    //  all O(1)
                    int rgb = ImageMatrix[i, j].red;
                    rgb = (rgb << 8) + ImageMatrix[i, j].green;
                    rgb = (rgb << 8) + ImageMatrix[i, j].blue;
                    distinct.Add(rgb);                                                   //  O(1)   hashset uses hash fn

                }


            }
            Dis_txt.Text = distinct.Count.ToString() + " distinct colors";

            foreach (int x in distinct)                                                  //  O(n)   " n number of distinct color"
                my_distinct.Add(x);
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            int width = ImageOperations.GetWidth(ImageMatrix); // read from file
            int height = ImageOperations.GetHeight(ImageMatrix); // read from file
            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int red = ImageMatrix[y, x].red; // read from array
                    int green = ImageMatrix[y, x].green; // read from array
                    int blue = ImageMatrix[y, x].blue; // read from array
                    bitmap.SetPixel(x, y, Color.FromArgb(0, red, green, blue));
                }
            SaveFileDialog save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Png Image|*.png|Bitmap Image|*.bmp";

            if (save_dialog.ShowDialog() == DialogResult.OK)
            {
                switch (save_dialog.FilterIndex)
                {
                    case 0:
                        {
                            bitmap.Save(save_dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        break;
                    case 1:
                        {
                            bitmap.Save(save_dialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        break;
                }

            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
       
    ///
        public class Graph
        {
            private int _V;
            private bool _directed;
            LinkedList<int>[] _adj;

            public Graph(int V, bool directed)
            {
                _adj = new LinkedList<int>[V];

                for (int i = 0; i < _adj.Length; i++)
                {
                    _adj[i] = new LinkedList<int>();
                }

                _V = V;
                _directed = directed;
            }

            public void AddEdge(int v, int w)
            {
                _adj[v].AddLast(w);

                if (!_directed)
                {
                    _adj[w].AddLast(v);
                }
            }

            public List<int> BreadthFirstSearch(int s)
            {

                bool[] visited = new bool[_V];


                List<int> mycluster = new List<int>();
                // Create a queue for BFS
                LinkedList<int> queue = new LinkedList<int>();

                visited[s] = true;
                queue.AddLast(s);

                while (queue.Any())
                {
                    // Dequeue a vertex from queue and print it
                    s = queue.First();
                    mycluster.Add(s);
                    queue.RemoveFirst();

                    LinkedList<int> list = _adj[s];

                    foreach (var val in list)
                    {
                        if (!visited[val])
                        {
                            visited[val] = true;
                            queue.AddLast(val);
                        }
                    }
                }

                return mycluster;
            }
    }

   

    }
    public struct Node
    {
        public double weight;
        public int number;
    }
}