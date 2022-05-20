Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports DevExpress.Pdf

Namespace WpfApplication1

    ''' <summary>
    ''' Logica di interazione per MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            Me.pdfView.DocumentSource = "Demo.pdf"
        End Sub

        Private Sub PdfSelectionBehavior_PdfSelectionChanged(ByVal sender As Object, ByVal args As PdfSelectionChangedEventArgs)
            Dim p1 As PdfPoint = args.StartPosition.Point
            Dim p2 As PdfPoint = args.EndPosition.Point
            Dim rect As PdfRectangle = New PdfRectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y))
            Dim pdfDocumentArea As PdfDocumentArea = New PdfDocumentArea(args.StartPosition.PageNumber, rect)
            Me.txtText.Text = Me.pdfView.GetText(pdfDocumentArea)
            Me.txtPositions.Text = $"Start position: {args.StartPosition.Point.X}, {args.StartPosition.Point.Y}
End Position: " & $"{args.EndPosition.Point.X}, {args.EndPosition.Point.Y}"
        End Sub
    End Class
End Namespace
