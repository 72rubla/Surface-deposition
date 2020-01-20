using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Threading;



namespace MicroparticleClass
{
    public class Microparticle
    {
        public readonly List<Figure> figures = new List<Figure>();
    }

    public abstract class Figure
    {
        protected GraphicsPath Path { get; } = new GraphicsPath();
        Color _penColor = Color.Black;//цвет линии
        public float _penWidth = 1; //толщина линии
        protected Pen _pen;
        public virtual Pen pen
        {
            get
            {
                if (_pen == null)
                    _pen = new Pen(_penColor, _penWidth);
                return _pen;
            }
        }
        public Color penColor
        {
            get { return _penColor; }
            set { _penColor = value; _pen = null; }
        }
        public float penWidth
        {
            get { return _penWidth; }
            set { _penWidth = value; _pen = null; }
        }
        public abstract bool IsInsidePoint(PointF p);
        public abstract void Draw(Graphics gr);        
    }

    public abstract class SolidFigure : Figure
    {
        //размер новой фигуры, по умолчанию
        public static int defaultSize { get; set; }
        //заливка фигуры
        //Color _color = Color.Green;
        public static Color color { get; set; }
        //вес фигуры
        public int FigureWeight { get; set; }
        //местоположение центра фигуры
        public PointF location;
        public Brush _brush;
        //public Color color
        //{
        //    get { return _color; }
        //    set { _color = value; _brush = null; }
        //}
        public virtual Brush brush
        {
            get
            {
                if (_brush == null)
                    _brush = new SolidBrush(color);
                return _brush;
            }
        }
        public virtual void ZOrderChange(Microparticle dep, int d)
        {
            int i = dep.figures.IndexOf(this);

            if (d < 0 && i > 0)
            {
                dep.figures.RemoveAt(i);
                dep.figures.Insert(i - 1, this);
            }
            if (d > 0 && i < dep.figures.Count - 1)
            {
                dep.figures.RemoveAt(i);
                dep.figures.Insert(i + 1, this);
            }
        }

        public override bool IsInsidePoint(PointF p)
        {
            //return Path.IsVisible(p.X-location.X, p.Y-location.Y);
            lock (this)
            {
                return Path.IsVisible(p.X - location.X, p.Y - location.Y);
            }

        }
        //сдвиг местоположения фигуры
        public virtual void Offset(int dx, int dy)
        {
            location = location.Offset(dx, dy);
            if (location.X < 0)
                location.X = 0;
            if (location.Y < 0)
                location.Y = 0;
        }

        //отрисовка фигуры
        public override void Draw(Graphics gr)
        {
            lock (this)
            {
                GraphicsState transState = gr.Save();
                gr.TranslateTransform(location.X, location.Y);
                gr.FillPath(brush, Path);
                gr.DrawPath(pen, Path);
                gr.Restore(transState);
            }
        }
    }

    public class Square : SolidFigure
    {
        public Square()
        {
            Path.AddRectangle(new RectangleF(-defaultSize, -defaultSize, defaultSize, defaultSize));            
        }
    }
          

    public static class PointHelper
    {
        public static PointF Offset(this PointF p, float x, float y)
        {
            p.X = p.X + x;
            p.Y = p.Y + y;
            return p;
        }
    }
}
