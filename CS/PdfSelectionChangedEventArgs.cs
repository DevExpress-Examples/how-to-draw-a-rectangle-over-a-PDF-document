using DevExpress.Pdf;
using System;
using System.Linq;

namespace WpfApplication1
{
    public delegate void PdfSelectionChangedEventHandler(object sender, PdfSelectionChangedEventArgs args);
    public class PdfSelectionChangedEventArgs : EventArgs
    {
        public PdfDocumentPosition StartPosition { get; private set; }
        public PdfDocumentPosition EndPosition { get; private set; }

        public PdfSelectionChangedEventArgs(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}
