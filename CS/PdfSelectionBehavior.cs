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
        AdornerLayer controlAdornerLayer;
        SelectionAdorner selectionAdorner;
        Point mouseStartPoint = new Point();
        DocumentViewerPanel documentPanel;

        public event PdfSelectionChangedEventHandler PdfSelectionChanged;
        protected void NotifyPdfSelectionChanged() {
            if (PdfSelectionChanged != null) {
                var startPoint = selectionAdorner.Location;
                var endPoint = startPoint;
                endPoint.Offset(selectionAdorner.OffSet.X, selectionAdorner.OffSet.Y);
                PdfDocumentPosition startPosition = AssociatedObject.ConvertPixelToDocumentPosition(documentPanel.TranslatePoint(startPoint, AssociatedObject));
                PdfDocumentPosition endPosition = AssociatedObject.ConvertPixelToDocumentPosition(documentPanel.TranslatePoint(endPoint, AssociatedObject));
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

            documentPanel = LayoutTreeHelper.GetVisualChildren(AssociatedObject).OfType<DocumentViewerPanel>().FirstOrDefault();
            if (documentPanel == null) return;
            controlAdornerLayer = AdornerLayer.GetAdornerLayer(documentPanel);
            selectionAdorner = new SelectionAdorner(documentPanel);

            controlAdornerLayer.Add(selectionAdorner);
            selectionAdorner.PreviewMouseLeftButtonDown += OnAdornerPreviewMouseLeftButtonDown;
            selectionAdorner.PreviewMouseMove += OnAdornerPreviewMouseMove;
            selectionAdorner.PreviewMouseLeftButtonUp += OnAdornerPreviewMouseLeftButtonUp;

            DevExpress.Xpf.PdfViewer.DXScrollViewer scrollViewer = LayoutTreeHelper.GetVisualChildren(AssociatedObject).OfType<DevExpress.Xpf.PdfViewer.DXScrollViewer>().FirstOrDefault(x => x.Name == "PART_ScrollViewer");
            scrollViewer.ScrollChanged += OnScrollChanged;
        }
        private void OnScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
            var startPoint = selectionAdorner.Location;
            startPoint.Offset(-e.HorizontalChange, -e.VerticalChange);
            selectionAdorner.Location = startPoint;
            UpdateAdorner();
        }

        //Create Adorner 
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(e.OriginalSource is DevExpress.Xpf.PdfViewer.DXScrollViewer)) return;
            if (documentPanel == null) return;

            selectionAdorner.Location = e.GetPosition(documentPanel);
            if (!documentPanel.IsMouseCaptured) {
                documentPanel.CaptureMouse();
                e.Handled = true;
            }
        }

        void OnPreviewMouseMove(object sender, MouseEventArgs e) {
            if (documentPanel == null) return;

            if (documentPanel.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed) {
                var mouseOffset = e.GetPosition(selectionAdorner);
                mouseOffset.Offset(-selectionAdorner.Location.X, -selectionAdorner.Location.Y);
                selectionAdorner.OffSet = mouseOffset;
                UpdateAdorner();
                e.Handled = true;
            }
        }

        protected virtual void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (documentPanel == null) return;

            if (documentPanel.IsMouseCaptured) {
                documentPanel.ReleaseMouseCapture();
                UpdateAdorner();
                NotifyPdfSelectionChanged();
                e.Handled = true;
            }
        }

        // Move Adorner
        void OnAdornerPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            mouseStartPoint = e.GetPosition(selectionAdorner);
            if (!selectionAdorner.IsMouseCaptured) {
                selectionAdorner.CaptureMouse();
            }
            e.Handled = true;
        }

        void OnAdornerPreviewMouseMove(object sender, MouseEventArgs e) {
            if (selectionAdorner.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed) {
                var mouseOffset = e.GetPosition(selectionAdorner);
                mouseOffset.Offset(-mouseStartPoint.X, -mouseStartPoint.Y);
                mouseStartPoint = e.GetPosition(selectionAdorner);
                var adornerPosition = selectionAdorner.Location;
                adornerPosition.Offset(mouseOffset.X, mouseOffset.Y);
                selectionAdorner.Location = adornerPosition;
                UpdateAdorner();
                e.Handled = true;
            }
        }

        void OnAdornerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (selectionAdorner.IsMouseCaptured) {
                selectionAdorner.ReleaseMouseCapture();
                NotifyPdfSelectionChanged();
                UpdateAdorner();
                e.Handled = true;
            }
        }
        protected virtual void DestroyAdorner() {
            if (selectionAdorner != null)
                controlAdornerLayer.Remove(selectionAdorner);
        }

        protected virtual void UpdateAdorner() {
            if (selectionAdorner != null)
                selectionAdorner.InvalidateVisual();
        }

        protected override void OnDetaching() {
            AssociatedObject.Loaded -= OnLoaded;
            DestroyAdorner();
            base.OnDetaching();
        }
    }
   
}
