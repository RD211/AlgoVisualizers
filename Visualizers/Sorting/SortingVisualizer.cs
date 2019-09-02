using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visualizers
{
    public class SortingVisualizer
    {
        #region Global Variables
        public Vector<Int16> numbers = new Vector<Int16>();
        public Int16 size,minValue,maxValue;
        private Dashboard mainForm;
        private Bitmap bmp = new Bitmap(1100, 1100);
        private Boolean Colored = false;
        private Boolean Reversed = false;
        private VisualTypes Type;
        public enum Algorithms
        {
            BubbleSort = 0,
            SelectionSort = 1,
            CountSort = 2,
            MergeSort = 3,
            QuickSort = 4,
            RadixSort = 5,
            HeapSort = 6
        };
        public enum VisualTypes
        {
            Bars = 0,
            Spiral = 1,
            Circle = 2,
            Pyramid = 3
        };
        #endregion

        #region Shuffle list
        public void shuffle()
        {
            numbers.Shuffle();
            DisplaySort();
        }
        #endregion

        #region Display to picturebox
        public void DisplaySort()
        {
            if (Reversed)
                numbers.Reverse();

            try
            {
                if (Type != VisualTypes.Bars)
                    bmp = new Bitmap(mainForm.pbox_sorts.Width*2, mainForm.pbox_sorts.Height*2);
                else
                    bmp = new Bitmap(1100, 1100);
                Graphics g = Graphics.FromImage(bmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                g.Clear(Color.White);
                float space=1;
                Int16 drawn = 0;
                switch(Type)
                {
                    case VisualTypes.Bars:
                        space = (float)bmp.Size.Width / size;
                        break;
                    case VisualTypes.Spiral:
                        space = (float)360/size;
                        break;
                    case VisualTypes.Pyramid:
                        space = (float)bmp.Size.Height / size;
                        break;
                }
                SolidBrush brsh = new SolidBrush(Color.Blue);
                for(int j = 0;j<numbers.Count;j++)
                {
                    Int16 i = numbers[j];
                    if (numbers.isSelected(j))
                    {
                        brsh.Color = Color.Red;
                    }
                    else if (Colored)
                    {
                        int value = (int)ExtraMath.MapNumberToRange<float>((float)i, (float)0, (float)maxValue, (float)0, (float)255);
                        brsh.Color = Color.FromArgb(0, 0, 255 - value);
                    }
                    else
                    {
                        brsh.Color = Color.Blue;
                    }
                    float mapped;
                    RectangleF rect;
                    switch (Type)
                    {
                        case VisualTypes.Bars:
                            mapped = ExtraMath.MapNumberToRange<float>((float)i, (float)0, (float)maxValue, (float)0, (float)bmp.Height);
                            rect = new RectangleF((float)space * drawn, bmp.Height - mapped, (float)space - 0, mapped);
                            g.FillRectangle(brsh, rect);
                            break;
                        case VisualTypes.Spiral:
                            mapped = ExtraMath.MapNumberToRange<float>((float)i, (float)minValue, (float)maxValue, (float)0, (float)Math.Min(bmp.Width/2, bmp.Height/2));
                            PointF Point1 = ExtraMath.GetPointAlongTrajectory(new PointF(bmp.Width / 2, bmp.Height / 2), ExtraMath.ToRadians(drawn * space), 10);
                            PointF Point2 = ExtraMath.GetPointAlongTrajectory(new PointF(bmp.Width / 2, bmp.Height / 2), ExtraMath.ToRadians(drawn * (space)), mapped + 10);
                            PointF Point3 = ExtraMath.GetPointAlongTrajectory(new PointF(bmp.Width / 2, bmp.Height / 2), ExtraMath.ToRadians((drawn+1) * (space)-0.1), mapped + 10);
                            PointF[] Points = { Point1, Point2, Point3 };
                            g.FillPolygon(brsh,Points);
                            break;
                        case VisualTypes.Pyramid:
                            mapped = ExtraMath.MapNumberToRange<float>((float)i, (float)0, (float)maxValue, (float)0, (float)bmp.Width/2);
                            rect = new RectangleF(mapped, (drawn + 1) * space, bmp.Width - mapped - mapped, space);
                            g.FillRectangle(brsh, rect);
                            break;
                    }
                
                    drawn++;
                }
                try
                {
                    mainForm.pbox_sorts.Image = bmp;
                    mainForm.pbox_sorts.Update();
                }
                catch
                { }
                System.Threading.Thread.Sleep(20);
            }
            catch { }
            if (Reversed)
                numbers.Reverse();
            GC.Collect();
        }
        #endregion

        #region Build list
        public void buildArray(Int16 min,Int16 max,Dashboard frm)
        {

            numbers.Clear();
            this.size = (Int16)(max-min+1);
            this.minValue = min;
            this.maxValue = max;
            for (Int16 i = minValue; i <= maxValue; i++)
                numbers.Add(i);
            mainForm = frm;
            shuffle();

        }
        #endregion

        #region Constructor
        public SortingVisualizer(Int16 min,Int16 max,Dashboard frm,VisualTypes vs)
        {
            buildArray(min,max,frm);
            Type = vs;
        }
        #endregion

        public void ToggleColor()
        {
            Colored = !Colored;
            DisplaySort();
        }
        public void ToggleOrder()
        {
            Reversed = !Reversed;
            DisplaySort();
        }
        public void UpdateType(VisualTypes vs)
        {
            Type = vs;
            DisplaySort();
        }

        #region Swap function overwrite
        private void swap(Int16 a, Int16 b)
        {
            Int16 temp = numbers[a];
            numbers[a] = numbers[b];
            DisplaySort();

            numbers[b] = temp;
            DisplaySort();

        }
        #endregion

        #region Sort method
        public void Sort(Algorithms algo)
        {
            switch (algo)
            {
                case Algorithms.BubbleSort:
                    BubbleSort();
                    break;
                case Algorithms.SelectionSort:
                    SelectionSort();
                    break;
                case Algorithms.CountSort:
                    CountSort();
                    break;
                case Algorithms.MergeSort:
                    MergeSort(0,(Int16)(size-1));
                    break;
                case Algorithms.QuickSort:
                    QuickSort(0, (Int16)(size -1));
                    break;
                case Algorithms.RadixSort:
                    RadixSort();
                    break;
                case Algorithms.HeapSort:
                    HeapSort(size);
                    break;
            }
        }
        #endregion

        #region Algorithms
        private void BubbleSort()
        {
            Int32 lastSwap = size - 1;
            for (Int16 i = 1; i < size; i++)
            {
                Boolean is_sorted = true;
                int currentSwap = -1;

                for (Int16 j = 0; j < lastSwap; j++)
                {
                    if (numbers[j] > numbers[j + 1])
                    {
                        swap(j, (Int16)(j+1));
                        is_sorted = false;
                        currentSwap = j;
                    }
                }

                if (is_sorted) return;
                lastSwap = currentSwap;
            }
        }
        void CountSort()
        {
            int max = size;
            int min = 1;
            int range = max - min + 1;

            List<int> count=new List<int>(range), output=new List<int>(size);
            count = count.Select(c => { c = 0; return c; }).ToList();
            output = output.Select(c => { c = 0; return c; }).ToList();
            
            for (int i = 0; i < size; i++)
                count[numbers[i] - min]++;

            for (int i = 1; i < count.Count(); i++)
                count[i] += count[i - 1];

            for (int i = size - 1; i >= 0; i--)
            {
                output[count[numbers[i] - min] - 1] = numbers[i];
                count[numbers[i] - min]--;
            }

            for (int i = 0; i < size; i++) {
                numbers[i] = (Int16)output[i];
                swap(1, 1);
            }
        }
        private void SelectionSort()
        {
            Int16 i, j, min_idx;

            for (i = 0; i < size - 1; i++)
            {
                min_idx = i;
                for (j = (Int16)(i + 1); j < size; j++)
                    if (numbers[j] < numbers[min_idx])
                        min_idx = j;

                swap(min_idx, i);
            }
        }
        Int16 partition(Int16 l, Int16 r)
        {
            Int16 x = numbers[r];
            Int16 i = (Int16)(l - 1);

            for (Int16 j = l; j <= r - 1; j++)
            {
                if (numbers[j] <= x)
                {
                    i++;
                    swap(i, j);
                }
            }
            swap((Int16)(i + 1), r);
            return ((Int16)(i + 1));
        }
        private void QuickSort(Int16 l, Int16 r)
        {
            if (l < r)
            {
                Int16 p = partition(l, r);
                QuickSort(l, (Int16)(p - 1));
                QuickSort((Int16)(p + 1), r);
            }
        }
        void Merge(Int16 l, Int16 m, Int16 r)
        {
            Int16 i, j, k;
            Int16 n1 = (Int16)(m - l + 1);
            Int16 n2 = (Int16)(r - m);

            Int16[] L= new Int16[n1], R= new Int16[n2];

            for (i = 0; i < n1; i++)
                L[i] = numbers[l + i];
            for (j = 0; j < n2; j++)
                R[j] = numbers[m + 1 + j];

            i = 0;  
            j = 0;  
            k = l; 
            while (i < n1 && j < n2)
            {
                if (L[i] <= R[j])
                {
                    numbers[k] = L[i];
                    swap(1, 1);
                    i++;
                }
                else
                {
                    numbers[k] = R[j];
                    swap(1, 1);
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                numbers[k] = L[i];
                swap(1, 1);
                i++;
                k++;
            }
            while (j < n2)
            {
                numbers[k] = R[j];
                j++;
                k++;
            }
        }

        private void MergeSort(Int16 l,Int16 r)
        { 
            if (l < r)
            {
                Int16 m = (Int16)(l + (r - l) / 2);
                MergeSort(l, m);
                MergeSort((Int16)(m + 1), r);
                Merge( l, m, r);
            }
        }
        private void RadixSort()
        {

        }
        void heapify(Int16 n, Int16 i)
        {
            Int16 largest = i;
            Int16 l = (Int16)(2 * i + 1);
            Int16 r = (Int16)(2 * i + 2);

            if (l < n && numbers[l] > numbers[largest])
                largest = l;

            if (r < n && numbers[r] > numbers[largest])
                largest = r;

            if (largest != i)
            {
                swap(i, largest);
                heapify(n, largest);
            }
        }

        void HeapSort(Int16 n)
        {
            for (int i = n / 2 - 1; i >= 0; i--)
                heapify( n, (Int16)i);

            for (int i = n - 1; i >= 0; i--)
            {
                swap(0, (Int16)i);

                heapify( (Int16)i, 0);
            }
        }
        #endregion
    }
}
