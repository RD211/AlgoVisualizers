using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizers
{
    public class StackVisualizer
    {
        public String postfix;
        public String ouput;
        private Dashboard dashboard;
        private Stack<Char> stackPostFix;
        public StackVisualizer(Dashboard dash)
        {
            dashboard = dash;
        }
        public void DrawStack()
        {
            Bitmap bmp = new Bitmap(dashboard.pbox_stack.Width, dashboard.pbox_stack.Height);
            Graphics g = Graphics.FromImage(bmp);
            dashboard.lbl_postFixStack.Text = postfix;
            SolidBrush brsh = new SolidBrush(Color.MediumBlue);
            PointF stackCharPoint = new PointF(30, 20);
            foreach(Char c in stackPostFix)
            {
                g.DrawString(c.ToString(), new Font("Adobe Gothic Std B", 14), brsh, stackCharPoint);
                stackCharPoint.Y += 20;
            }
            dashboard.pbox_stack.Image = bmp;
            dashboard.pbox_stack.Update();
        }
        public void ToPostFix(String input)
        {
            stackPostFix = new Stack<Char>();

        }
    }
}
