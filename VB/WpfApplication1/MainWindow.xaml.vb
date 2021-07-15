Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports DevExpress.Pdf

Namespace WpfApplication1
	''' <summary>
	''' Logica di interazione per MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window

		Public Sub New()
			InitializeComponent()
			pdfView.DocumentSource = "Demo.pdf"
		End Sub


		Private Sub PdfSelectionBehavior_PdfSelectionChanged(ByVal sender As Object, ByVal args As PdfSelectionChangedEventArgs)
			Dim p1 As PdfPoint = args.StartPosition.Point
			Dim p2 As PdfPoint = args.EndPosition.Point
			Dim rect As New PdfRectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y))
			Dim pdfDocumentArea As New PdfDocumentArea(args.StartPosition.PageNumber, rect)

			txtText.Text = pdfView.GetText(pdfDocumentArea)
			txtPositions.Text = $"Start position: {args.StartPosition.Point.X}, {args.StartPosition.Point.Y}" & vbLf & "End Position: " & $"{args.EndPosition.Point.X}, {args.EndPosition.Point.Y}"
		End Sub
	End Class
End Namespace
