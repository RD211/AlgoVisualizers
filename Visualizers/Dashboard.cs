using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using MetroFramework.Forms;

namespace Visualizers
{
    public partial class Dashboard : Form
    {
        #region Global variables
        SortingVisualizer sortingVisual;
        GraphVisualizer graphVisual;
        StackVisualizer stackVisual;
        Thread sortThread;
        Thread graphThread;
        Thread stackThread;
        Thread moveVertex;
        bool VertexMove = false;
        #endregion

        #region Constructor
        public Dashboard()
        {
            InitializeComponent();
            graphVisual = new GraphVisualizer(pbox_graphs);
            sortingVisual = new SortingVisualizer(1, 100, this,SortingVisualizer.VisualTypes.Bars);
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            MinimumSize = new Size(900,375);
        }
        #endregion

        private void pbox_graphs_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs m = (MouseEventArgs)e;
            graphVisual.addVertex(m.Location);
        }
        int selectedVertex = -1;
        Point VertexNewPosition;
        private void updateVertexPos()
        {
            while(VertexMove)
            {
                graphVisual.updateVertex(selectedVertex, VertexNewPosition);
            }
        }
        private void pbox_graphs_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                graphVisual.Vertexes[selectedVertex].Deselect();
                graphVisual.forceUpdate();
            }
            catch { }
            MouseEventArgs m = (MouseEventArgs)e;
            selectedVertex = graphVisual.findVertex(m.Location);
            try
            {
                graphVisual.Vertexes[selectedVertex].Select();
                graphVisual.forceUpdate();
            }
            catch { }
            if (VertexMove)
            {
                VertexNewPosition = m.Location;
                moveVertex = new Thread(updateVertexPos);
                moveVertex.Start();
            }
        }

        private void pbox_graphs_MouseUp(object sender, MouseEventArgs e)
        {
            if (moveVertex.IsAlive)
            {
                moveVertex.Abort();
                return;
            }
            if (selectedVertex == -1)
                return;
            MouseEventArgs m = (MouseEventArgs)e;
            int selectedVertex2 = graphVisual.findVertex(m.Location);

            if (selectedVertex2 == -1)
                return;
            graphVisual.addEdge(selectedVertex, selectedVertex2);
            graphVisual.Vertexes[selectedVertex].Deselect();
            graphVisual.forceUpdate();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                VertexMove = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (VertexMove)
                VertexMove = false;
        }

        private void pbox_graphs_MouseMove(object sender, MouseEventArgs e)
        {
            if(VertexMove)
            {
                VertexNewPosition = e.Location;
            }
        }

        private void switch_weighted_OnValueChange(object sender, EventArgs e)
        {
            graphVisual.updateType(switch_weighted.Value, switch_directed.Value, switch_residual.Value);
        }

        private void switch_orientated_OnValueChange(object sender, EventArgs e)
        {
            graphVisual.updateType(switch_weighted.Value, switch_directed.Value, switch_residual.Value);
        }

        private void runGraph()
        {
            btn_stop.Enabled = true;
            btn_random.Enabled = false;
            btn_run.Enabled = false;
            btn_clear.Enabled = false;
            graphVisual.runAlgorithm((GraphVisualizer.Algorithms)dropDown_graphAlgo.selectedIndex,selectedVertex==-1?0:selectedVertex);
            btn_stop.Enabled = false;
            btn_random.Enabled = true;
            btn_run.Enabled = true;
            btn_clear.Enabled = true;
        }


        #region Sort thread
        private void startSort()
        {
            btn_stop.Enabled = true;
            btn_random.Enabled = false;
            btn_run.Enabled = false;
            btn_clear.Enabled = false;
            if(sortingVisual.minValue!=range_sortNumbers.RangeMin||sortingVisual.maxValue!=range_sortNumbers.RangeMax)
                sortingVisual.buildArray((Int16)range_sortNumbers.RangeMin, (Int16)range_sortNumbers.RangeMax,this);
            sortingVisual.Sort((SortingVisualizer.Algorithms)dropDown_sortAlgo.selectedIndex);
            btn_random.Enabled = true;
            btn_run.Enabled = true;
            btn_stop.Enabled = false;
            btn_clear.Enabled = true;
        }
        #endregion

        #region On load
        private void Form1_Load(object sender, EventArgs e)
        {

            pbox_graphs.AllowDrop = true;
            tab_Category.AllowDrop = true;
            tab_graphs.AllowDrop = true;
            btn_stop.Enabled = false;
            pbox_sorts.Image = new Bitmap(1100, 1100);
            Btn_random_Click(sender,e);
            tab_Category.Appearance = TabAppearance.FlatButtons;
            tab_Category.ItemSize = new Size(0, 1);
            tab_Category.SizeMode = TabSizeMode.Fixed;
            moveVertex = new Thread(updateVertexPos);
            this.KeyPreview = true;
            this.MinimumSize = this.Size;
        }

        private void Dashboard_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                switch (tab_Category.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        graphVisual = (GraphVisualizer)JsonConvert.DeserializeObject<GraphVisualizer>(File.ReadAllText(files[0]));
                        graphVisual.pbox = pbox_graphs;
                        graphVisual.forceUpdate();

                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }

            }
            catch { }
        }

        private void Dashboard_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        #endregion

        #region Category buttons
        private void btn_sortCategory_Click(object sender, EventArgs e)
        {
            tab_Category.SelectedTab = tab_sorts;
        }
        private void btn_miscCategory_Click(object sender, EventArgs e)
        {
            tab_Category.SelectedTab = tab_misc;
            label1.Text = "Height: " + this.Size.Height + " ,Width: " + this.Size.Width;
        }
        private void btn_graphCategory_Click(object sender, EventArgs e)
        {
            tab_Category.SelectedTab = tab_graphs;
        }

        private void btn_treeCategory_Click(object sender, EventArgs e)
        {
            tab_Category.SelectedTab = tab_trees;

        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        private void Dashboard_SizeChanged(object sender, EventArgs e)
        {
            if (tab_Category.SelectedIndex == 1)
                graphVisual.forceUpdate();
            else if (tab_Category.SelectedIndex == 0)
                sortingVisual.DisplaySort();
        }

        private void Btn_burgerMenu_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
            graphVisual.forceUpdate();
        }

        private void Btn_run_Click(object sender, EventArgs e)
        {
            switch(tab_Category.SelectedIndex)
            {
                case 0:
                    sortThread = new Thread(startSort);
                    sortThread.Start();
                    break;
                case 1:
                    graphThread = new Thread(runGraph);
                    graphThread.Start();
                    break;
                case 2:
                    break;
                case 3:

                    break;
            }
        }

        private void Btn_stop_Click(object sender, EventArgs e)
        {
            switch(tab_Category.SelectedIndex)
            {
                case 0:
                    sortThread.Abort();
                    break;
                case 1:
                    graphThread.Abort();
                    foreach (Vertex v in graphVisual.Vertexes)
                        v.Deselect();
                    foreach(List<Edge> el in graphVisual.Edges)
                        foreach (Edge ee in el)
                            ee.Deselect();
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            btn_run.Enabled = true;
            btn_random.Enabled = true;
            btn_stop.Enabled = false;
            btn_clear.Enabled = true;
        }

        private void Btn_random_Click(object sender, EventArgs e)
        {
            switch(tab_Category.SelectedIndex)
            {
                case 0:
                    sortingVisual.buildArray((Int16)range_sortNumbers.RangeMin, (Int16)range_sortNumbers.RangeMax, this);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        private void Btn_clear_Click(object sender, EventArgs e)
        {
            switch(tab_Category.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    graphVisual.clearVertexes();
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        private void Pbox_graphs_Click(object sender, EventArgs e)
        {

        }

        private void Btn_image_stack_Click(object sender, EventArgs e)
        {
            tab_Category.SelectedTab = tab_stacks;
        }

        private void Btn_Stack_Click(object sender, EventArgs e)
        {
            try
            {
                string s = (((Bunifu.Framework.UI.BunifuThinButton2)sender).ButtonText.ToString());
                if (s == "<")
                    lbl_inputStack.Text = lbl_inputStack.Text.Remove(lbl_inputStack.Text.Count() - 1, 1);
                else
                    lbl_inputStack.Text += s;
                lbl_inputStack.Update();
            }
            catch { }
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            SaveFileDialog dialog = new SaveFileDialog();
            switch (tab_Category.SelectedIndex)
            {
                case 0:
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(dialog.FileName.Split('.')[0] + ".array"))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, sortingVisual);
                        }
                    }
                    break;
                case 1:
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        graphVisual.pbox = null;
                        using (StreamWriter sw = new StreamWriter(dialog.FileName.Split('.')[0]+".graph"))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, graphVisual);
                        }
                        graphVisual.pbox = pbox_graphs;
                    }
                    break;
                case 2:

                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        private void Switch_sortGradient_OnValueChange(object sender, EventArgs e) => sortingVisual.ToggleColor();

        private void Btn_spiral_Click(object sender, EventArgs e) => sortingVisual.UpdateType(SortingVisualizer.VisualTypes.Spiral);

        private void Btn_pyramid_Click(object sender, EventArgs e) => sortingVisual.UpdateType(SortingVisualizer.VisualTypes.Pyramid);

        private void Btn_bars_Click(object sender, EventArgs e) => sortingVisual.UpdateType(SortingVisualizer.VisualTypes.Bars);

        private void Switch_sortReverse_OnValueChange(object sender, EventArgs e)
        {
            sortingVisual.ToggleOrder();
        }
    }
}
