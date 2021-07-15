using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.PdfViewer;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Pdf;
using System.Windows.Threading;

namespace WpfApplication1 {
    public class PdfSelectionBehavior : Behavior<PdfViewerControl> {
        AdornerLayer ControlAdornerLayer;
        SelectionAdorner SelectionAdorner;
        Point mouseStartPoint = new Point();
        DocumentViewerPanel DocumentPanel;

        public event PdfSelectionChangedEventHandler PdfSelectionChanged;
        protected void NotifyPdfSelectionChanged() {
            if (PdfSelectionChanged != null) {
                var startPoint = SelectionAdorner.Location;
                var endPoint = startPoint;
                endPoint.Offset(SelectionAdorner.OffSet.X, SelectionAdorner.OffSet.Y);
                PdfDocumentPosition startPosition = AssociatedObject.ConvertPixelToDocumentPosition(DocumentPanel.TranslatePoint(startPoint, AssociatedObject));
                PdfDocumentPosition endPosition = AssociatedObject.ConvertPixelToDocumentPosition(DocumentPanel.TranslatePoint(endPoint, AssociatedObject));
                PdfSelectionChanged(this, new PdfSelectionChangedEventArgs(startPosition, endPosition));
            }
        }


        protected override void OnAttached() {
            base.OnAttached();

            if (AssociatedObject.IsLoaded)
                CreateAdorner();
            else
                AssociatedObject.Loaded += OnLoaded;
        }
        void OnLoaded(object sender, RoutedEventArgs e) {
            AssociatedObject.Loaded -= OnLoaded;
            CreateAdorner();
        }

        protected virtual void CreateAdorner() {
            
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;


            DocumentPanel = LayoutTreeHelper.GetVisualChildren(AssociatedObject).OfType<DocumentViewerPanel>().FirstOrDefault();
            if (DocumentPanel == null) return;

            ControlAdornerLayer = AdornerLayer.GetAdornerLayer(DocumentPanel);
            SelectionAdorner = new SelectionAdorner(DocumentPanel);

            ControlAdornerLayer.Add(SelectionAdorner);
            SelectionAdorner.PreviewMouseLeftButtonDown += OnAdornerPreviewMouseLeftButtonDown;
            SelectionAdorner.PreviewMouseMove += OnAdornerPreviewMouseMove;
            SelectionAdorner.PreviewMouseLeftButtonUp += OnAdornerPreviewMouseLeftButtonUp;

        }


        private void OnScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
            var startPoint = SelectionAdorner.Location;
            startPoint.Offset(-e.HorizontalChange, -e.VerticalChange);
            SelectionAdorner.Location = startPoint;
            UpdateAdorner();
        }

        //Create Adorner 
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(e.OriginalSource is DevExpress.Xpf.PdfViewer.DXScrollViewer)) return;
            if (DocumentPanel == null) return;

            SelectionAdorner.Location = e.GetPosition(DocumentPanel);
            if (!DocumentPanel.IsMouseCaptured) {
                DocumentPanel.CaptureMouse();
                e.Handled = true;
            }
        }

        void OnPreviewMouseMove(object sender, MouseEventArgs e) {
            if (DocumentPanel == null) return;

            if (DocumentPanel.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed) {
                var mouseOffset = e.GetPosition(SelectionAdorner);
                mouseOffset.Offset(-SelectionAdorner.Location.X, -SelectionAdorner.Location.Y);
                SelectionAdorner.OffSet = mouseOffset;
                UpdateAdorner();
                e.Handled = true;
            }
        }

        protected virtual void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (DocumentPanel == null) return;

            if (DocumentPanel.IsMouseCaptured) {
                DocumentPanel.ReleaseMouseCapture();
                UpdateAdorner();
                NotifyPdfSelectionChanged();
                e.Handled = true;
            }
        }

        // Move Adorner
        void OnAdornerPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            mouseStartPoint = e.GetPosition(SelectionAdorner);
            if (!SelectionAdorner.IsMouseCaptured) {
                SelectionAdorner.CaptureMouse();
            }
            e.Handled = true;
        }

        void OnAdornerPreviewMouseMove(object sender, MouseEventArgs e) {
            if (SelectionAdorner.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed) {
                var mouseOffset = e.GetPosition(SelectionAdorner);
                mouseOffset.Offset(-mouseStartPoint.X, -mouseStartPoint.Y);
                mouseStartPoint = e.GetPosition(SelectionAdorner);
                var adornerPosition = SelectionAdorner.Location;
                adornerPosition.Offset(mouseOffset.X, mouseOffset.Y);
                SelectionAdorner.Location = adornerPosition;
                UpdateAdorner();
                e.Handled = true;
            }
        }

        void OnAdornerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (SelectionAdorner.IsMouseCaptured) {
                SelectionAdorner.ReleaseMouseCapture();
                NotifyPdfSelectionChanged();
                UpdateAdorner();
                e.Handled = true;
            }
        }
        protected virtual void DestroyAdorner() {
            if (SelectionAdorner != null)
                ControlAdornerLayer.Remove(SelectionAdorner);
        }

        protected virtual void UpdateAdorner() {
            if (SelectionAdorner != null)
                SelectionAdorner.InvalidateVisual();
        }

        protected override void OnDetaching() {
            AssociatedObject.Loaded -= OnLoaded;
            DestroyAdorner();
            base.OnDetaching();
        }
    }

    public class SelectionAdorner : Adorner {
        protected DocumentViewerPanel PdfViewer { get; private set; }
        Point location = new Point();
        Point offset = new Point();

        public Point Location { get { return location; } set { location = value; } }

        public Point OffSet {
            get { return offset; }
            set { offset = value; }
        }

        public SelectionAdorner(DocumentViewerPanel adornedElement)
            : base(adornedElement) {
            PdfViewer = adornedElement;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            var selectionLocation = Location;

            if (OffSet.X != 0 || OffSet.Y != 0)
                drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(selectionLocation, Point.Subtract(OffSet, new Point())));
        }
    }

    public delegate void PdfSelectionChangedEventHandler(object sender, PdfSelectionChangedEventArgs args);
    public class PdfSelectionChangedEventArgs : EventArgs {
        public PdfDocumentPosition StartPosition { get; private set; }
        public PdfDocumentPosition EndPosition { get; private set; }

        public PdfSelectionChangedEventArgs(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }

   
}
