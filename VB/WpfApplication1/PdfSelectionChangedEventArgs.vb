Imports DevExpress.Pdf
Imports System

Namespace WpfApplication1

    Public Delegate Sub PdfSelectionChangedEventHandler(ByVal sender As Object, ByVal args As PdfSelectionChangedEventArgs)

    Public Class PdfSelectionChangedEventArgs
        Inherits EventArgs

        Private _StartPosition As PdfDocumentPosition, _EndPosition As PdfDocumentPosition

        Public Property StartPosition As PdfDocumentPosition
            Get
                Return _StartPosition
            End Get

            Private Set(ByVal value As PdfDocumentPosition)
                _StartPosition = value
            End Set
        End Property

        Public Property EndPosition As PdfDocumentPosition
            Get
                Return _EndPosition
            End Get

            Private Set(ByVal value As PdfDocumentPosition)
                _EndPosition = value
            End Set
        End Property

        Public Sub New(ByVal startPosition As PdfDocumentPosition, ByVal endPosition As PdfDocumentPosition)
            Me.StartPosition = startPosition
            Me.EndPosition = endPosition
        End Sub
    End Class
End Namespace
