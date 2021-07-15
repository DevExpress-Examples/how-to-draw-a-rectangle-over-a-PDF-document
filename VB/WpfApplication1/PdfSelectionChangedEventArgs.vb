Imports DevExpress.Pdf
Imports System
Imports System.Linq

Namespace WpfApplication1
	Public Delegate Sub PdfSelectionChangedEventHandler(ByVal sender As Object, ByVal args As PdfSelectionChangedEventArgs)
	Public Class PdfSelectionChangedEventArgs
		Inherits EventArgs

		Private privateStartPosition As PdfDocumentPosition
		Public Property StartPosition() As PdfDocumentPosition
			Get
				Return privateStartPosition
			End Get
			Private Set(ByVal value As PdfDocumentPosition)
				privateStartPosition = value
			End Set
		End Property
		Private privateEndPosition As PdfDocumentPosition
		Public Property EndPosition() As PdfDocumentPosition
			Get
				Return privateEndPosition
			End Get
			Private Set(ByVal value As PdfDocumentPosition)
				privateEndPosition = value
			End Set
		End Property

		Public Sub New(ByVal startPosition As PdfDocumentPosition, ByVal endPosition As PdfDocumentPosition)
			Me.StartPosition = startPosition
			Me.EndPosition = endPosition
		End Sub
	End Class
End Namespace
