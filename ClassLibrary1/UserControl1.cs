using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroparticleClass
{
    public partial class GrowthSimulation : UserControl
    {
        Microparticle micrp;
        PointF p;
        Figure draggedFigure = null;        
        
        public GrowthSimulation()
        {
            InitializeComponent();
            AutoScroll = true;
            DoubleBuffered = true;
            ResizeRedraw = true;
            
        }
        /// <summary>
        /// Сдвигает местоположение фигуры
        /// </summary>
        public void MoveFigure(int x, int y)
        {
            if (draggedFigure != null)
                (draggedFigure as SolidFigure).Offset(x, y);
            Invalidate();
        }
        /// <summary>
        /// Возвращает местоположение фигуры
        /// </summary>
        public PointF GetLocation()
        {
            //if (draggedFigure == null) { draggedFigure = class1.figures.Last(); }
            PointF p = (draggedFigure as SolidFigure).location;
            return p;
        }
        /// <summary>
        /// Проверяет наличие объекта снизу
        /// </summary>
        /// <param name="dx">Координата по X</param>
        /// <param name="size">Размер частицы</param>
        public bool TopPosition(float dx, int size)
        {
            p = new PointF
            {
                X = dx, //dx
                Y = size+2 //size+2
            };
            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта снизу
        /// </summary>
        public bool FindFigureBottom(float dx, float dy, int size)
        {
            p = new PointF //снизу
            {
                X = dx - size, //-size
                Y = dy //0
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта снизу слева
        /// </summary>
        public bool FindFigureBottomLeft(float dx, float dy, int size)
        {
            p = new PointF
            {
                X = dx - size * 2, //dx - size * 2
                Y = dy //0
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта снизу справа
        /// </summary>
        public bool FindFigureBottomRight(float dx, float dy, int size)
        {
            p = new PointF
            {
                X = dx + size - 2, //sx+size-2
                Y = dy //0
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                { return true; }
            }
            return false;
        }
        public bool FindFigureRight(float dx, float dy, int size)
        {            
            p = new PointF //справа
            {
                X = dx + size -2,//dx +size-2
                Y = dy - size //-size
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                { return true; }
            }            
            return false;
        }
        public bool FindFigureLeft(float dx, float dy, int size)
        {
            p = new PointF //сбоку
            {
                X = dx - size * 2, //dx-size*2
                Y = dy - size //dy-size
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта для баллистического осаждения со смещением
        /// </summary>
        public bool FindFigureBallisticNext(float dx, float dy, int size)
        {
            if (FindFigureLeft(dx, dy, size) == true) return true;
            if (FindFigureRight(dx, dy, size) == true) return true;
            if (FindFigureBottom(dx, dy, size) == true) return true;
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта для рандомного осаждения
        /// </summary>
        public bool FindFigureRandom(float dx, float dy, int size)
        {
            if (FindFigureBottom(dx, dy, size) == true) return true;
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта для баллистического осаждения
        /// </summary>
        public bool FindFigureBalistic(float dx, float dy, int size)
        {
            if (FindFigureBottom(dx, dy, size) == true) return true;
            if (FindFigureBottomLeft(dx, dy, size) == true) return true;
            if (FindFigureBottomRight(dx, dy, size) == true) return true;
            return false;
        }
        /// <summary>
        /// Проверяет наличие объекта снизу и возращает объект
        /// </summary>            
        public Figure FindFigure(float dx, float dy)
        {
            p = new PointF //снизу
            {
                X = dx - 2,
                Y = dy - 2
            };

            for (int i = micrp.figures.Count - 1; i >= 0; i--)
            {
                if (micrp.figures[i] is SolidFigure && micrp.figures[i].IsInsidePoint(p))
                {
                    draggedFigure = micrp.figures[i];
                    return micrp.figures[i];
                }
            }
            return null;
        }

        public Microparticle Class1
        {
            get { return micrp; }
            set
            {
                micrp = value;
                draggedFigure = null;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }       
        
        public int GridSize { get; set; }
        public void UpD()
        {
            Invalidate();
        }
        public bool GridChecked { get; set; } = false;
        private void Draw(Graphics gr)
        {
            lock (this)
            {
                if (GridChecked==true)
                {
                    int numOfCells = 120;
                    int cellSize = GridSize;
                    Pen p = new Pen(Color.WhiteSmoke);

                    for (int y = 0; y < numOfCells; ++y)
                    {
                        gr.DrawLine(p, 0, y * cellSize, numOfCells * cellSize, y * cellSize);
                    }

                    for (int x = 0; x < numOfCells; ++x)
                    {
                        gr.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCells * cellSize);
                    }                   
                }
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
                if (micrp != null)
                {
                    foreach (Figure f in micrp.figures.ToArray())
                        if (f is SolidFigure)
                            f.Draw(gr);
                }
            }
        }

        //Point startpoint;
        //void Locc()
        //{
        //    Point location = new Point
        //    {
        //        X = 50,
        //        Y = 50
        //    };
        //    startpoint = location;
        //}

        /// <summary>
        /// Создает новую фигуру
        /// </summary>
        public void AddFigure<FigureType>(Point location) where FigureType : SolidFigure, new()
        {
            Random rnd = new Random();
            FigureType figure = new FigureType();
            figure.location = location;
            figure.FigureWeight = rnd.Next(10, 51);
            if (micrp != null)
                micrp.figures.Add(figure);
            Invalidate();
        }
        /// <summary>
        /// Удаляет фигуру, что падает
        /// </summary>
        public void SelectedDelete()
        {
            if (draggedFigure != null)
            {
                micrp.figures.Remove(draggedFigure);
            }
            draggedFigure = null;
            Invalidate();
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
    }
}