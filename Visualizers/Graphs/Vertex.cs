using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizers
{
    public class Vertex
    {
        public Point position;
        public int distance;
        public bool selected;
        public bool visited;
        public Vertex(Point position,int distance)
        {
            this.position = position;
            this.distance = distance;
            this.selected = false;
            this.visited = false;
        }
        public void Select()
        {
            selected = true;
        }
        public void Deselect()
        {
            selected = false;
        }
        public void Visit()
        {
            visited = true;
        }

    }
}
