Imports DevExpress.Xpf.DocumentViewer
Imports System.Windows
Imports System.Windows.Documents
Imports System.Windows.Media

Namespace WpfApplication1

    Public Class SelectionAdorner
        Inherits Adorner

        Private locationField As Point = New Point()

        Private offsetField As Point = New Point()

        Public Property Location As Point
            Get
                Return locationField
            End Get

            Set(ByVal value As Point)
                locationField = value
            End Set
        End Property

        Public Property OffSet As Point
            Get
                Return offsetField
            End Get

            Set(ByVal value As Point)
                offsetField = value
            End Set
        End Property

        Public Sub New(ByVal adornedElement As DocumentViewerPanel)
            MyBase.New(adornedElement)
        End Sub

        Protected Overrides Sub OnRender(ByVal drawingContext As DrawingContext)
            Dim renderBrush As SolidColorBrush = New SolidColorBrush(Colors.Green)
            renderBrush.Opacity = 0.2
            Dim renderPen As Pen = New Pen(New SolidColorBrush(Colors.Navy), 1.5)
            Dim selectionLocation = Location
            If OffSet.X <> 0 OrElse OffSet.Y <> 0 Then drawingContext.DrawRectangle(renderBrush, renderPen, New Rect(selectionLocation, Point.Subtract(OffSet, New Point())))
        End Sub
    End Class
End Namespace
