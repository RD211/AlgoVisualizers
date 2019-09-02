using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizers
{
    public class Edge
    {
        public int weight;
        public int destination;
        public bool isOpposed;
        public bool selected;
        public bool visited;
        public Edge(int destination,int weight,bool isOpposed=false)
        {
            this.isOpposed = isOpposed;
            this.weight = weight;
            this.destination = destination;
            this.selected = false;
            this.visited = false;
        }
        public void Select()
        {
            this.selected = true;
        }
        public void Deselect()
        {
            this.selected = false;
        }
        public void Visit()
        {
            visited = true;
        }
    }
}
