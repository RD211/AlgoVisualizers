using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Priority_Queue;

namespace Visualizers
{
    public class GraphVisualizer
    {
        #region Global variables
        public PictureBox pbox;
        public List<Vertex> Vertexes = new List<Vertex>();
        public List<List<Edge>> Edges = new List<List<Edge>>();
        private int VertexRadius = 20;
        private Boolean weighted, directed, residual;
        public enum Algorithms
        {
            DFS = 1,
            BFS = 0,
            DIJKSTRA = 2,
            BF = 3,
            KRUSKAL = 4
        };
        #endregion

        #region Constructor
        public GraphVisualizer(PictureBox pbox)
        {
            this.pbox = pbox;
        }
        #endregion

        #region Add vertex
        public void addVertex(Point vertexLocation)
        {
            Vertexes.Add(new Vertex(vertexLocation,0));
            Edges.Add(new List<Edge>());
            drawGraph();
        }
        #endregion
        
        #region Update Graph type
        public void updateType(Boolean weighted = false, Boolean orientated = false, Boolean residual = false)
        {
            if (this.directed != orientated)
                clearEdges();
            this.weighted = weighted;
            this.directed = orientated;
            this.residual = residual;
            drawGraph();
        }
        #endregion

        #region Force update graph
        public void forceUpdate()
        {
            drawGraph();
        }
        #endregion

        #region Add edge
        public void addEdge(int start, int end, int weight = 1)
        {
            bool startEdge = false, opposedEdge = false;
            foreach (Edge e in Edges[start])
            {
                if (e.destination == end)
                {
                    e.weight += weight;
                    startEdge = true;
                }
            }
            if (!directed)
            {
                foreach (Edge e in Edges[end])
                {
                    if (e.destination == start)
                    {
                        e.weight += weight;
                        opposedEdge = true;
                    }
                }
            }
            if(!startEdge)
                Edges[start].Add(new Edge(end, weight));
            if(!opposedEdge&&!directed)
                Edges[end].Add(new Edge(start, weight,true));
            drawGraph();
        }
        #endregion

        #region Clear Edges
        public void clearEdges()
        {
            foreach(List<Edge> l in Edges)
            {
                l.Clear();
            }
            drawGraph();
        }
        #endregion

        #region Clear vertexes (clear Graph)
        public void clearVertexes()
        {
            Edges.Clear();
            Vertexes.Clear();
            drawGraph();
        }
        #endregion

        #region Find vertex
        public int findVertex(Point m)
        {
            int selectedVertex = -1;
            for (int i = 0; i < Vertexes.Count(); i++)
            {
                Point p = Vertexes[i].position;
                if (Math.Sqrt((p.X - m.X) * (p.X - m.X) + (p.Y - m.Y) * (p.Y - m.Y)) <= VertexRadius)
                {
                    selectedVertex = i;
                }
            }
            return selectedVertex;
        }
        #endregion

        #region Update vertex
        public void updateVertex(int id,Point p)
        {
            Vertexes[id].position = p;
            drawGraph();
        }
        #endregion

        #region Draw graph to picture box
        private void drawGraph()
        {
            try
            {
                Bitmap bmpGraphs = new Bitmap(pbox.Width, pbox.Height);
                Graphics g = Graphics.FromImage(bmpGraphs);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                for (int i = 0; i < Edges.Count(); i++)
                {
                    for (int j = 0; j < Edges[i].Count(); j++)
                    {
                        if (!directed||!Edges[i][j].isOpposed)
                        {
                            Pen linePen = new Pen(Color.DarkBlue);
                            SolidBrush brushArrow = new SolidBrush(Color.DarkBlue);
                            SolidBrush brushEllipse = new SolidBrush(Color.White);
                            SolidBrush brushString = new SolidBrush(Color.Black);
                            linePen.Width = 5;
                            if(Edges[i][j].visited)
                            {
                                linePen.Color = Color.LightSkyBlue;
                                brushArrow.Color = Color.LightSkyBlue;
                                brushString.Color = Color.LightSkyBlue;
                            }
                            if (Edges[i][j].selected)
                            {
                                linePen.Color = Color.DarkRed;
                                brushArrow.Color = Color.DarkRed;
                                brushString.Color = Color.DarkRed;
                            }
                            PointF firstPoint = Vertexes[i].position;
                            PointF secondPoint = Vertexes[Edges[i][j].destination].position;
                            PointF middlePoint = ExtraMath.GetMiddlePoint(firstPoint, secondPoint);
                            PointF quarterPoint = ExtraMath.GetMiddlePoint(middlePoint, secondPoint);
                            try
                            {
                                g.DrawLine(linePen, firstPoint, secondPoint);
                            }
                            catch { }
                            if (directed)
                            {
                                double thetaLeft = ExtraMath.ToRadians(150);
                                double thetaRight = ExtraMath.ToRadians(210);
                                PointF rotationPoint = ExtraMath.GetPointAlongLine(quarterPoint, firstPoint, 25);
                                PointF leftPart = ExtraMath.RotateAroundPoint(quarterPoint, rotationPoint, thetaLeft);
                                PointF rightPart = ExtraMath.RotateAroundPoint(quarterPoint, rotationPoint, thetaRight);
                                PointF[] arrow = { quarterPoint, leftPart, rightPart, quarterPoint };
                                try
                                {
                                    g.FillPolygon(brushArrow, arrow, System.Drawing.Drawing2D.FillMode.Alternate);
                                }
                                catch { }
                            }
                            if (weighted)
                            {
                                try
                                {
                                    g.FillEllipse(brushEllipse, middlePoint.X - 15, middlePoint.Y - 15, 30, 30);
                                    g.DrawString(Edges[i][j].weight.ToString(), new Font("Adobe Gothic Std B", 14),
                                       brushString, middlePoint.X - 10, middlePoint.Y - 10);
                                }
                                catch { }
                            }
                        }
                    }
                }
                foreach (Vertex v in Vertexes)
                {
                    SolidBrush brushVertex = new SolidBrush(Color.Blue);
                    SolidBrush brushText = new SolidBrush(Color.White);
                    if (v.visited)
                        brushVertex.Color = Color.LightSkyBlue;
                    if (v.selected)
                        brushVertex.Color = Color.DarkRed;
                    try
                    {
                        g.FillEllipse(brushVertex, v.position.X - VertexRadius, v.position.Y - VertexRadius, VertexRadius * 2, VertexRadius * 2);
                        g.DrawString(v.distance.ToString(), new Font("Adobe Gothic Std B", 15), brushText, v.position.X - VertexRadius + 10, v.position.Y - VertexRadius + 10);
                    }
                    catch { }
                }

                try
                {
                    pbox.Image = bmpGraphs;
                    pbox.Update();
                }
                catch
                {

                }
                GC.Collect();
                System.Threading.Thread.Sleep(10);
            }
            catch
            {
            }
        }
        #endregion

        public void runAlgorithm(Algorithms id,int start=0)
        {
            switch (id)
            {
                case Algorithms.BFS:
                    BFS(start);
                    break;
                case Algorithms.DFS:
                    DFS(start);
                    break;
                case Algorithms.DIJKSTRA:
                    DIJKSTRA(start);
                    break;
                case Algorithms.BF:
                    break;
            }
        }
        #region Algorithms

        #region BFS
        private void BFS(int start)
        {
            Queue<int> coada = new Queue<int>();
            BitArray viz = new BitArray(Vertexes.Count());
            viz[start] = true;
            coada.Enqueue(start);

            foreach (Vertex v in Vertexes)
            {
                v.distance = -1;
                v.Deselect();
                v.visited = false;
            }
            foreach (List<Edge> le in Edges)
            {
                foreach (Edge e in le)
                {
                    e.Deselect();
                    e.visited = false;
                }
            }
            System.Threading.Thread.Sleep(20);
            drawGraph();
            while(coada.Count>0)
            {
                int current = coada.Peek();
                Vertexes[current].Select();
                Vertexes[current].Visit();
                drawGraph();
                System.Threading.Thread.Sleep(500);
                for(int i = 0;i<Edges[current].Count();i++)
                {
                    Edges[current][i].Select();
                    Edges[current][i].Visit();
                    for (int j = 0; j < Edges[Edges[current][i].destination].Count(); j++)
                    {
                        if (Edges[Edges[current][i].destination][j].destination == current)
                        {
                            Edges[Edges[current][i].destination][j].Select();
                            Edges[Edges[current][i].destination][j].Visit();
                            break;
                        }
                    }
                    drawGraph();
                    System.Threading.Thread.Sleep(250);

                    int connection = Edges[current][i].destination;
                    if(!viz[connection])
                    {
                        viz[connection] = true;
                        Vertexes[connection].distance = Vertexes[current].distance + 1;
                        coada.Enqueue(connection);
                        drawGraph();
                    }
                    for (int j = 0; j < Edges[Edges[current][i].destination].Count(); j++)
                    {
                        if (Edges[Edges[current][i].destination][j].destination == current)
                        {
                            Edges[Edges[current][i].destination][j].Deselect();
                            break;
                        }
                    }
                    Edges[current][i].Deselect();
                    drawGraph();
                    System.Threading.Thread.Sleep(250);
                }
                Vertexes[current].Deselect();
                drawGraph();
                System.Threading.Thread.Sleep(500);

                coada.Dequeue();
            }
        }
        #endregion

        private void DIJKSTRA(int start)
        {
            SimplePriorityQueue <int,int> heap = new SimplePriorityQueue<int, int>();
            foreach (Vertex v in Vertexes)
            {
                v.distance = -1;
                v.Deselect();
                v.visited = false;
            }
            foreach (List<Edge> le in Edges)
            {
                foreach (Edge e in le)
                {
                    e.Deselect();
                    e.visited = false;
                }
            }
            Vertexes[start].distance = 0;
            heap.Enqueue(start, Vertexes[start].distance);
            while(heap.Count>0)
            {
                int heapFirst = heap.First;
                heap.Dequeue();
                Vertexes[heapFirst].Select();
                drawGraph();
                System.Threading.Thread.Sleep(250);
                for (int i = 0;i<Edges[heapFirst].Count();i++)
                {
                    Vertex to = Vertexes[Edges[heapFirst][i].destination];
                    Edges[heapFirst][i].Select();
                    drawGraph();
                    System.Threading.Thread.Sleep(250);
                    if (to.distance>Vertexes[heapFirst].distance+Edges[heapFirst][i].weight||to.distance==-1)
                    {
                        Vertexes[Edges[heapFirst][i].destination].distance = Vertexes[heapFirst].distance + Edges[heapFirst][i].weight;
                        heap.Enqueue(Edges[heapFirst][i].destination, Vertexes[Edges[heapFirst][i].destination].distance);
                        drawGraph();
                        System.Threading.Thread.Sleep(250);
                    }
                    Edges[heapFirst][i].Deselect();
                    drawGraph();
                    System.Threading.Thread.Sleep(250);
                }
                Vertexes[heapFirst].Deselect();
                drawGraph();
                System.Threading.Thread.Sleep(250);
            }
        }
        private void DFS(int start)
        {
            Stack<int> stac = new Stack<int>();
            BitArray viz = new BitArray(Vertexes.Count());
            viz[start] = true;
            stac.Push(start);
            for(int i =0;i<Vertexes.Count();i++)
            {
                Vertex v = Vertexes[i];
                v.distance = i;
                v.Deselect();
                v.visited = false;
            }
            foreach(List<Edge> le in Edges)
            {
                foreach(Edge e in le)
                {
                    e.Deselect();
                    e.visited = false;
                }
            }
            drawGraph();
            while (stac.Count > 0)
            {
                int current = stac.Peek();
                stac.Pop();
                Vertexes[current].Visit();
                Vertexes[current].Select();
                drawGraph();
                for (int i = 0; i < Edges[current].Count(); i++)
                {
                    Edges[current][i].Select();
                    Edges[current][i].Visit();
                    for(int j = 0;j<Edges[Edges[current][i].destination].Count();j++)
                    {
                        if(Edges[Edges[current][i].destination][j].destination==current)
                        {
                            Edges[Edges[current][i].destination][j].Select();
                            Edges[Edges[current][i].destination][j].Visit();
                            break;
                        }
                    }
                    drawGraph();
                    System.Threading.Thread.Sleep(250);

                    int connection = Edges[current][i].destination;
                    if (!viz[connection])
                    {
                        viz[connection] = true;
                        stac.Push(connection);
                        drawGraph();
                    }
                    for (int j = 0; j < Edges[Edges[current][i].destination].Count(); j++)
                    {
                        if (Edges[Edges[current][i].destination][j].destination == current)
                        {
                            Edges[Edges[current][i].destination][j].Deselect();
                            break;
                        }
                    }
                    Edges[current][i].Deselect();
                    drawGraph();
                    System.Threading.Thread.Sleep(250);
                }
                Vertexes[current].Deselect();

                drawGraph();
                System.Threading.Thread.Sleep(500);

            }
        }
        #endregion
    }
}
