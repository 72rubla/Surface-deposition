using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MicroparticleClass;
using System.Threading;
using ZedGraph;

namespace Alfa
{
    public partial class SurfaceGrowth : Form
    {
        public SurfaceGrowth()
        {
            InitializeComponent();
            Place.Class1 = new Microparticle();
            Place.Class1 = Place.Class1;
            //Form2 fm2 = new Form2();
            this.comboBox1.Text = "10";
            label1.Text = "Ballistic \n" + "x  |   y" + "\n" + "0  " + "|" + "  0" + "\n" + " i=0";
            Place.GridChecked = false;
            Invalidate();
        }
        Point placePoint;
        Thread thread;
        Form2 fm2;
        int LawDist;

        private void butt1_Click(object sender, EventArgs e)
        {
            fm2 = new Form2(Place.Width / Convert.ToInt32(comboBox1.Text));
            fm2.ShowDialog();
            if (fm2.DialogResult == DialogResult.OK) { this.LawDist = fm2.LawNumber(); }
        }        

        #region Процес осаждения
        private void DepositionData()
        {
            int square_size = 0;//Размер частицы
            int anim_speed = 0;            
            Invoke((MethodInvoker)delegate () { square_size = Convert.ToInt32(comboBox1.Text); });            
            switch (square_size)//Скорость анимации
            {
                case 5:
                    anim_speed = 0;//10
                    break;
                case 10:
                    anim_speed = 1;//20
                    break;
                case 20:
                    anim_speed = 3;//30
                    break;
                default:
                    anim_speed = 30;
                    break;
            }

            SolidFigure.defaultSize = square_size;
            int place_width = Place.Width;//Ширина
            int place_height = Place.Height;//Высота
            int countX = place_width / square_size;//Количество столбцов
            int countY = place_height / square_size;//Количество рядов            
            var arrX = new int[countX];
            var arrY = new int[countY];
            var arrColumn = new int[arrX.Length];
            for (int i = 1; i < countX; i++)
            {
                arrX[0] = square_size;
                arrX[i] = arrX[i - 1] + square_size;
            }
            for (int i = 1; i < countY; i++)
            {
                arrY[0] = square_size;
                arrY[i] = arrY[i - 1] + square_size;
            }
            int a, b;
            if (fm2 == null)
            {
                a = 0;
                b = countX;
            }
            else { a = fm2.A; b = fm2.B; }
            //else { a = 0; b = countX; }
            var rnd = new Random();
            switch (LawDist)
            {
                case 1: arrColumn = NormalLawDist(countX); break;
                case 2:
                    for (int j = a; j < b; j++)//Количество частиц в столбце
                    {
                        int x = 10;
                        if (EquableLawDist(countX, a, b) < 10) x = EquableLawDist(countX, a, b);
                        arrColumn[j] = x;
                    }
                    break;
                case 3: arrColumn = ExponLawDist(countX); break;
                case 4:
                    for (int j = a; j < b; j++)//Количество частиц в столбце
                    {
                        arrColumn[j] = rnd.Next(1, 6);
                    }
                    break;
                default:
                    for (int j = a; j < b; j++)//Количество частиц в столбце
                    {
                        arrColumn[j] = rnd.Next(1, 6);
                    }
                    break;
            }
            Place.GridSize = square_size;
            Deposition(square_size, arrX, arrY, arrColumn, rnd, countX, countY, anim_speed);//Баллистическое         
            Invoke((MethodInvoker)delegate () { testBtn.Enabled = true; });
        }
        /// <summary>
        /// Процесс осаждение
        /// </summary>
        private void Deposition(int square_size, int[] arrX, int[] arrY, int[] arrColumn, Random rnd, int countX, int countY, int anim_speed)
        {
            int i = 0;
            bool r = true, b = false, u = false;
            PointF p = new PointF();
            int red = rnd.Next(0, 256);
            int green = rnd.Next(0, 256);
            int blue = rnd.Next(0, 256);
            int weight = 0;
            List<int> tempX = new List<int>();
            List<int> tempY = new List<int>();
            int start_y = 0;
            int x = 0;
            int[] tempCol = new int[arrColumn.Length];
            Array.Copy(arrColumn, tempCol, arrColumn.Length);
            int y_max = tempCol.Max();
            int h = 0;
            for (int j = y_max; j >= 0; j--)
            {
                red = rnd.Next(0, 256);
                green = rnd.Next(0, 256);
                blue = rnd.Next(0, 256);
                weight = rnd.Next(10, 51);
                start_y = arrY[j] - square_size;
                for (int e = 0; e < countX; e++)
                {
                    x = arrX[e];
                    if (tempCol[e] != 0)
                    {
                        AddSquare(x, start_y, Color.FromArgb(red, green, blue), weight);
                        tempX.Add(x);
                        tempY.Add(start_y);
                        tempCol[e] -= 1;
                        h++;
                    }
                }
                Thread.Sleep(20);
            }

            while (Dep_chB.Checked == false)
            {
                if (Dep_chB.Checked)
                    break;
                Thread.Sleep(100);
            }
            if (Dep_chB.Checked)
            {
                while (r)
                {
                    r = false;
                    b = false;
                    u = false;
                    x = 0;
                    Figure f = null;
                    int n = rnd.Next(0, countX);
                    x = arrX[n];
                    int t = rnd.Next(tempY.Min(), tempY.Max());
                    //for (int s = square_size; s <= tempY.Max(); s += square_size)
                    for (int s = tempY.Max(); s >= square_size; s -= square_size)
                    {
                        if (Place.FindFigure(x, s) != null)
                        {
                            f = Place.FindFigure(x, s);
                            break;
                        }
                    }
                    if (f != null)
                    {
                        int m = ((tempY.Max() - (int)(f as SolidFigure).location.Y) / square_size) + 1;

                        do
                        {
                            MoveSquare(0, square_size);
                            m--;
                        }
                        while (m > 0);
                        bool top = false;
                        p = Place.GetLocation();
                        if (Place.FindFigureRandom(p.X, p.Y, 0) == true)
                        {
                            Place.SelectedDelete(); top = true; b = true;
                        }
                        //if (Place.TopPosition(x, square_size) == false)
                        if (top == false)
                        {
                            bool l = false;
                            do
                            {
                                if (BallBtn.Checked)//Баллистическое
                                {
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        p = Place.GetLocation();
                                        if (p.Y == 600) { l = false; }
                                        else l = true;
                                        if (l == true && Place.FindFigureBalistic(p.X, p.Y, square_size) == false)
                                        {
                                            label1.Text = "Ballistic \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1);
                                            MoveSquare(0, square_size);
                                            b = true;
                                        }
                                        else
                                        { l = false; }

                                    }); Thread.Sleep(anim_speed);
                                }
                                else if (RandBtn.Checked)//Рандомное
                                {
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        p = Place.GetLocation();
                                        if (p.Y == 600) { l = false; }
                                        else l = true;
                                        if (l == true && Place.FindFigureRandom(p.X, p.Y, square_size) == false)
                                        {
                                            label1.Text = "Random \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1);
                                            MoveSquare(0, square_size);
                                            b = true;
                                        }
                                        else
                                        { l = false; }

                                    }); Thread.Sleep(anim_speed);
                                }
                                else if (RandRelBtn.Checked)//Рандомное со смещением
                                {
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        p = Place.GetLocation();
                                        if (p.Y == 600) { l = false; }
                                        else l = true;
                                        if (l == true && Place.FindFigureBottom(p.X, p.Y, square_size) == false)
                                        {
                                            label1.Text = "RandomRelax \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1);
                                            MoveSquare(0, square_size);
                                            b = true;
                                        }
                                        else if (l == true && Place.FindFigureBottom(p.X, p.Y, square_size) == true)
                                        {
                                            if (Place.FindFigureLeft(p.X, p.Y, square_size) == false && Place.FindFigureBottom(p.X, p.Y, square_size) == true && Place.FindFigureBottomLeft(p.X, p.Y, square_size) == false && p.X > square_size)
                                            {
                                                int xx = 0;
                                                xx -= square_size;
                                                MoveSquare(xx, square_size);
                                                b = true;
                                            }
                                            else if (Place.FindFigureRight(p.X, p.Y, square_size) == false && Place.FindFigureBottom(p.X, p.Y, square_size) == true && Place.FindFigureBottomRight(p.X, p.Y, square_size) == false && p.X < 600)
                                            {
                                                int xx = 0;
                                                xx += square_size;
                                                MoveSquare(xx, square_size);
                                                b = true;
                                            }
                                            else l = false;
                                            label1.Text = "RandomRelax \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1);
                                        }
                                        else
                                        { l = false; }

                                    }); Thread.Sleep(anim_speed);
                                }
                                else if (BallRBtn.Checked)//Баллистическое
                                {
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        p = Place.GetLocation();
                                        if (p.Y == 600) { l = false; }
                                        else l = true;
                                        if (l == true && Place.FindFigureBallisticNext(p.X, p.Y, square_size) == false)
                                        {
                                            label1.Text = "Ballistic \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1);
                                            MoveSquare(0, square_size);
                                            b = true;
                                        }
                                        else
                                        { l = false; }
                                    }); Thread.Sleep(anim_speed);
                                }
                                else if (AllRandBtn.Checked)
                                {
                                    Invoke((MethodInvoker)delegate ()
                                    {
                                        p = Place.GetLocation();
                                        if (p.Y == 600) { l = false; }
                                        else l = true;
                                        if ((f as SolidFigure).FigureWeight < 20 && (f as SolidFigure).FigureWeight > 9)
                                        {
                                            if (l == true && Place.FindFigureBalistic(p.X, p.Y, square_size) == false)
                                            {
                                                label1.Text = "AllRandom \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1) + "\n" + (f as SolidFigure).FigureWeight.ToString();
                                                MoveSquare(0, square_size);
                                                b = true;
                                            }
                                            else
                                            { l = false; }
                                        }
                                        else if ((f as SolidFigure).FigureWeight < 30 && (f as SolidFigure).FigureWeight > 20)
                                        {
                                            if (l == true && Place.FindFigureBallisticNext(p.X, p.Y, square_size) == false)
                                            {
                                                label1.Text = "AllRandom \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1) + "\n" + (f as SolidFigure).FigureWeight.ToString();
                                                MoveSquare(0, square_size);
                                                b = true;
                                            }
                                            else
                                            { l = false; }
                                        }
                                        else if ((f as SolidFigure).FigureWeight < 50 && (f as SolidFigure).FigureWeight > 30)
                                        {
                                            if (l == true && Place.FindFigureRandom(p.X, p.Y, square_size) == false)
                                            {
                                                label1.Text = "AllRandom \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1) + "\n" + (f as SolidFigure).FigureWeight.ToString();
                                                MoveSquare(0, square_size);
                                                b = true;
                                            }
                                            else
                                            { l = false; }
                                        }
                                        else
                                        {
                                            if (l == true && Place.FindFigureRandom(p.X, p.Y, square_size) == false)
                                            {
                                                label1.Text = "AllRandom \n" + "x  |   y" + "\n" + p.X.ToString() + "|" + p.Y.ToString() + "\n" + " i=" + (i + 1) + "\n" + (f as SolidFigure).FigureWeight.ToString();
                                                MoveSquare(0, square_size);
                                                b = true;
                                            }
                                            else
                                            { l = false; }
                                        }
                                    }); Thread.Sleep(anim_speed);
                                }
                            } while (l);
                        }
                    }
                    if (b)
                    {
                        i++;
                        h--;
                    }
                    if (h > 0) r = true;
                }
            }
            Graph(arrX, arrY, arrColumn, square_size, countX, countY);
        }
        /// <summary>
        /// Вычисляет максимальную высоту в столбце
        /// </summary>
        private void Graph(int[] arrX, int[] arrY, int[] arrColumn, int square_size, int countX, int countY)
        {
            int[] maxY = new int[countX];
            int ii = 0;
            for (int i = arrX[0]; i <= arrX[countX - 1]; i += square_size)
            {
                for (int j = arrY[0]; j < arrY[countY - 1]; j += square_size)
                {
                    if (Place.FindFigureRandom(i, j, square_size) == true)
                    {
                        maxY[ii] += (j + square_size);
                        break;
                    }
                }
                ii++;
            }
            Invoke((MethodInvoker)delegate () { label2.Text = RoughnessRa(maxY, square_size, countY).ToString(); });
            Invoke((MethodInvoker)delegate () { label3.Text = RoughnessRz(maxY, square_size, countY).ToString(); });

            DrawGraph(arrX, maxY, arrColumn, square_size);
        }
        private void FindPath(int[] arrX, int[] arrY, int[] arrColumn, int square_size, int countX, int countY)
        {
            int CountX = Place.Width;
            int CountY = Place.Height;

            bool f;
            do
            {
                f = false;
            }
            while (f);

        }
        #endregion
        #region Законы распредиления
        /// <summary>
        /// Нормыльный закон распредиления
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private int[] NormalLawDist(int count)
        {
            //600 - 5(120) 10(60) 20(30)
            int[] Arr = new int[count];
            double Fi = 0.0;
            int[] x = new int[count];
            for (int i = 1; i < count; i++)
            {
                x[0] = -(count / 2);
                x[i] = x[i - 1] + 1;
            }
            for (int i = 0; i < count; i++)
            {
                Fi = (1 / Math.Sqrt(2 * Math.PI)) * Math.Exp(-(Math.Pow(x[i], 2) / 2)) * 25;
                if (Fi < 0.1) Fi = 0;
                Arr[i] = (int)Fi;
            }
            return Arr;
        }
        /// <summary>
        /// Равномерный закон распредиления
        /// </summary>
        /// <param name="count"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int EquableLawDist(int count, int a, int b)
        {
            int x = 0;
            double fx = 0.0;
            fx = 1 / ((double)b - (double)a);
            if (count == 120)
                x = (int)(fx * 1000);
            else if (count == 60)
                x = (int)(fx * 200);
            else if (count == 30)
                x = (int)(fx * 50);
            return x;
        }
        /// <summary>
        /// Экспоненциальный закон распредиления
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private int[] ExponLawDist(int count)
        {
            int[] Arr = new int[count];
            double Lmd = 0.13;
            int[] x = new int[count];
            double fx = 0.0;
            for (int i = 1; i < count; i++)
            {
                x[0] = 1;
                x[i] = x[i - 1] + 1;
            }
            for (int i = 0; i < count; i++)
            {
                fx = Lmd * Math.Exp(-(Lmd * x[i]));
                Arr[i] = (int)(fx * 100);
            }
            return Arr;
        }
        #endregion
        #region Работа с фигурой
        private void AddSquare(int x, int y, Color clr, int fgWeight)
        {
            SolidFigure.color = clr;
            placePoint = new Point { X = x, Y = y };
            Place.AddFigure<Square>(placePoint);
        }
        private void MoveSquare(int x, int y)
        {
            Place.MoveFigure(x, y);
        }
        #endregion
        #region Характеристика поверхности
        /// <summary>
        /// Построение графиков
        /// </summary>        
        private void DrawGraph(int[] arrX, int[] maxY, int[] arrColumn, int size)
        {
            // Получим панель для рисования
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.Title.Text = "Закони розподілу";
            pane.XAxis.Title.Text = "X";
            pane.YAxis.Title.Text = "Y";
            pane.XAxis.Type = ZedGraph.AxisType.Linear;
            pane.XAxis.Scale.MajorStep = 20;
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();

            // Интервал, где есть данные
            int xmin = 0;
            int xmax = maxY.Length;

            int xmin_limit = 0;
            int xmax_limit = 600;

            int ymin_limit = 0;
            int ymax_limit = 700;

            //Заполняем список точек
            for (int x = xmin; x < xmax; x++)
            {
                list.Add(arrX[x], 600 - maxY[x]);
            }
            for (int x = xmin; x < xmax; x++)
            {
                list2.Add(arrX[x], (600 + arrColumn[x] * size));
            }

            LineItem myCurve = pane.AddCurve("Вихідний", list, Color.Blue, SymbolType.None);
            LineItem myCurve2 = pane.AddCurve("Вхідний", list2, Color.Red, SymbolType.None);

            
            // !!!
            // Устанавливаем интересующий нас интервал по оси X
            pane.XAxis.Scale.Min = xmin_limit;
            pane.XAxis.Scale.Max = xmax_limit;

            // !!!
            // Устанавливаем интересующий нас интервал по оси Y
            pane.YAxis.Scale.Min = ymin_limit;
            pane.YAxis.Scale.Max = ymax_limit;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraphControl1.AxisChange();

            // Обновляем график
            zedGraphControl1.Invalidate();
        }
        /// <summary>
        /// Шероховатость Ra
        /// </summary>
        private double RoughnessRa(int[] Arr, int SquareSize, int CountY)
        {
            double Ra = 0.0;            
            //Array.Sort(Arr);
            double Min = ((CountY - (Arr.Max() / SquareSize)) + 1);
            double Max = ((CountY - (Arr.Min() / SquareSize)) + 1);
            int[] temp = new int[Arr.Length];
            Array.Copy(Arr, temp, Arr.Length);
            Array.Sort(temp);
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[0] == 0 && (temp[i] > 0))
                {
                    Max = (CountY - temp[i] / SquareSize) + 1;
                    break;
                }
            }
            //Min = Arr.Min() == 0 ? Max : Min;
            int count = 0;
            foreach (var i in Arr)
            {
                count += i == 0 ? 0 : 1;
            }
            double Avv = (Max + Min) / 2.0;
            for (int i = 0; i < Arr.Length; i++)
            {
                double y = 0.0;
                y = Arr[i] != 0 ? Avv - ((CountY - (Arr[i] / SquareSize)) + 1) : 0;
                Ra += Math.Abs(y);
            }
            Ra = (1.0 / count) * Ra;
            return Ra;
        }
        /// <summary>
        /// Шероховатость Rz
        /// </summary>
        private double RoughnessRz(int[] ArrX, int SquareSize, int CountY)
        {
            double Rz = 0.0;
            int[] Arr = new int[ArrX.Length];
            Array.Copy(ArrX, Arr, ArrX.Length);
            for (int i = 0; i < Arr.Length; i++)
            {
                Arr[i] = Arr[i] != 0 ? ((CountY - (Arr[i] / SquareSize)) + 1) : 0;
            }
            Array.Sort(Arr);
            int[] ArrMin = new int[5];
            int[] ArrMax = new int[5];
            int count = 0;
            for (int i = 0; i < Arr.Length; i++)
            {
                count += Arr[i] != 0 ? 1 : 0;                
            }
            Array.Copy(Arr, Arr.Length - count, ArrMin, 0, 5);
            Array.Reverse(Arr);
            Array.Copy(Arr, ArrMax, 5);            
            for (int i = 0; i < 5; i++)
            {
                Rz += ArrMax[i] - ArrMin[i];
            }
            Rz = (1.0 / 5.0) * Rz;
            return Rz;
        }
        private void DrawGraph2(int[]x, int[]X)
        {
            // Получим панель для рисования
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.Title.Text = "Закони розподілу";
            pane.XAxis.Title.Text = "X";
            pane.YAxis.Title.Text = "Y";
            pane.XAxis.Type = ZedGraph.AxisType.Linear;
            pane.XAxis.Scale.MajorStep = 20;
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list = new PointPairList();

            // Интервал, где есть данные
            int xmin = 0;
           

            int xmin_limit = -20;
            int xmax_limit = 20;

            int ymin_limit = 0;
            int ymax_limit = 100;
          
            
            //Заполняем список точек
            for (int i = 0; i<x.Length; i++)
            {
                list.Add(X[i], x[i]);
            }
            

            LineItem myCurve = pane.AddCurve("", list, Color.Blue, SymbolType.None);


            // !!!
            // Устанавливаем интересующий нас интервал по оси X
            pane.XAxis.Scale.Min = xmin_limit;
            pane.XAxis.Scale.Max = xmax_limit;

            // !!!
            // Устанавливаем интересующий нас интервал по оси Y
            pane.YAxis.Scale.Min = ymin_limit;
            pane.YAxis.Scale.Max = ymax_limit;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraphControl1.AxisChange();

            // Обновляем график
            zedGraphControl1.Invalidate();
        }
        #endregion

        #region Обработка событий

        private void testBtn_Click(object sender, EventArgs e)
        {
            testBtn.Enabled = false;
            label1.Text = "Ballistic \n" + "x  |   y" + "\n" + "0  " + "|" + "  0" + "\n" + " i=0";
            thread = new Thread(new ThreadStart(DepositionData));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();

            //x = 10; верх-лево
            //y = 10;

            //x = 500; низ-право
            //y = 300;

            //x = 500; верх-право
            //y = 10;

            //x = 10; низ-лево
            //y = 300;                        
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (thread != null)
                thread.Abort();
            Place.Class1 = new Microparticle();
            Place.Class1 = Place.Class1;
            fm2 = null;
            testBtn.Enabled = true;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (thread != null)
                thread.Abort();
            Place.Class1 = new Microparticle();
            Place.Class1 = Place.Class1;
            fm2 = null;
            testBtn.Enabled = true;
        }        
        private void Grid_chB_Checked(object sender, EventArgs e)
        {
            if (Grid_chB.Checked)
            {
                Place.GridChecked = true;
                Place.UpD();
            }
            else { Place.GridChecked = false; Place.UpD(); }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread != null)
                thread.Abort();
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpDep helpDep = new HelpDep();
            helpDep.ShowDialog();            
        }
    }
}