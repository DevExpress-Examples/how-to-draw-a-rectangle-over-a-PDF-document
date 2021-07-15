using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Pdf;

namespace WpfApplication1
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pdfView.DocumentSource = "Demo.pdf";
        }


        private void PdfSelectionBehavior_PdfSelectionChanged(object sender, PdfSelectionChangedEventArgs args) {
            PdfPoint p1 = args.StartPosition.Point;
            PdfPoint p2 = args.EndPosition.Point;
            PdfRectangle rect = new PdfRectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
            PdfDocumentArea pdfDocumentArea = new PdfDocumentArea(args.StartPosition.PageNumber, rect);

            txtText.Text = pdfView.GetText(pdfDocumentArea);
            txtPositions.Text = $"Start position: {args.StartPosition.Point.X}, {args.StartPosition.Point.Y}\nEnd Position: " +
                $"{args.EndPosition.Point.X}, {args.EndPosition.Point.Y}";
        }
    }
}
