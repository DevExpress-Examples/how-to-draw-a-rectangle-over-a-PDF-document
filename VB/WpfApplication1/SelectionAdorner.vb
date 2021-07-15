Imports DevExpress.Xpf.DocumentViewer
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Documents
Imports System.Windows.Media

Namespace WpfApplication1
	Public Class SelectionAdorner
		Inherits Adorner

'INSTANT VB NOTE: The field location was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private location_Conflict As New Point()
'INSTANT VB NOTE: The field offset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private offset_Conflict As New Point()

		Public Property Location() As Point
			Get
				Return location_Conflict
			End Get
			Set(ByVal value As Point)
				location_Conflict = value
			End Set
		End Property

		Public Property OffSet() As Point
			Get
				Return offset_Conflict
			End Get
			Set(ByVal value As Point)
				offset_Conflict = value
			End Set
		End Property

		Public Sub New(ByVal adornedElement As DocumentViewerPanel)
			MyBase.New(adornedElement)
		End Sub

		Protected Overrides Sub OnRender(ByVal drawingContext As DrawingContext)
			Dim renderBrush As New SolidColorBrush(Colors.Green)
			renderBrush.Opacity = 0.2
			Dim renderPen As New Pen(New SolidColorBrush(Colors.Navy), 1.5)
			Dim selectionLocation = Location

			If OffSet.X <> 0 OrElse OffSet.Y <> 0 Then
				drawingContext.DrawRectangle(renderBrush, renderPen, New Rect(selectionLocation, Point.Subtract(OffSet, New Point())))
			End If
		End Sub
	End Class
End Namespace
