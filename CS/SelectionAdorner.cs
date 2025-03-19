using DevExpress.Xpf.DocumentViewer;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfApplication1
{
    public class SelectionAdorner : Adorner
    {
        Point location = new Point();
        Point offset = new Point();

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public Point OffSet
        {
            get { return offset; }
            set { offset = value; }
        }

        public SelectionAdorner(DocumentViewerPanel adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            var selectionLocation = Location;

            if (OffSet.X != 0 || OffSet.Y != 0)
                drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(selectionLocation, Point.Subtract(OffSet, new Point())));
        }
    }
}
