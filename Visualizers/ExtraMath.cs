using System;
using System.Drawing;

namespace Visualizers
{
    public class ExtraMath
    {
        public static PointF GetMiddlePoint(PointF p1,PointF p2)
        {

            return new PointF(p1.X / 2 + p2.X / 2,
                              p1.Y / 2 + p2.Y / 2);
        }
        public static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
        public static PointF GetPointAlongLine(PointF p2,PointF p1,double distance)
        {
            double dist = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) +
                                    (p2.Y - p1.Y) * (p2.Y - p1.Y));

            double t = distance / dist;
            return new PointF((float)((1 - t) * p2.X + t * p1.X), (float)((1 - t) * p2.Y + t * p1.Y));
        }
        public static PointF RotateAroundPoint(PointF p1,PointF p2,double angle)
        {
            return new PointF((float)(Math.Cos(angle) * (p1.X - p2.X) - Math.Sin(angle) * (p1.Y - p2.Y) + p2.X),
                               (float)(Math.Sin(angle) * (p1.X - p2.X) + Math.Cos(angle) * (p1.Y - p2.Y) + p2.Y));
        }
        public static PointF GetPointAlongTrajectory(PointF init,double angle,float distance)
        {
            return new PointF((float)(distance * Math.Cos(angle))+init.X, (float)(distance * Math.Sin(angle))+init.Y);
        }
        public static T MapNumberToRange<T>(T variable, T variableMin, T variableMax, T rangeMin, T rangeMax)
        {
            dynamic X = variable,
                    A = variableMin,
                    B = variableMax, 
                    C = rangeMin, 
                    D = rangeMax;

            return (X - A) / (B - A) * (D - C) + C;
        }
    }
}
