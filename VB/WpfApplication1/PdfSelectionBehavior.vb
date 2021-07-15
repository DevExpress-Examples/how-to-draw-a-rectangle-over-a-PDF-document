Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.PdfViewer
Imports DevExpress.Mvvm.UI
Imports DevExpress.Xpf.DocumentViewer
Imports DevExpress.Pdf
Imports System.Windows.Threading

Namespace WpfApplication1
	Public Class PdfSelectionBehavior
		Inherits Behavior(Of PdfViewerControl)

		Private ControlAdornerLayer As AdornerLayer
		Private SelectionAdorner As SelectionAdorner
		Private mouseStartPoint As New Point()
		Private DocumentPanel As DocumentViewerPanel

		Public Event PdfSelectionChanged As PdfSelectionChangedEventHandler
		Protected Sub NotifyPdfSelectionChanged()
			If PdfSelectionChangedEvent IsNot Nothing Then
				Dim startPoint = SelectionAdorner.Location
				Dim endPoint = startPoint
				endPoint.Offset(SelectionAdorner.OffSet.X, SelectionAdorner.OffSet.Y)
				Dim startPosition As PdfDocumentPosition = AssociatedObject.ConvertPixelToDocumentPosition(DocumentPanel.TranslatePoint(startPoint, AssociatedObject))
				Dim endPosition As PdfDocumentPosition = AssociatedObject.ConvertPixelToDocumentPosition(DocumentPanel.TranslatePoint(endPoint, AssociatedObject))
				RaiseEvent PdfSelectionChanged(Me, New PdfSelectionChangedEventArgs(startPosition, endPosition))
			End If
		End Sub


		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()

			If AssociatedObject.IsLoaded Then
				CreateAdorner()
			Else
				AddHandler AssociatedObject.Loaded, AddressOf OnLoaded
			End If
		End Sub
		Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			RemoveHandler AssociatedObject.Loaded, AddressOf OnLoaded
			CreateAdorner()
		End Sub

		Protected Overridable Sub CreateAdorner()

			AddHandler AssociatedObject.PreviewMouseLeftButtonDown, AddressOf OnPreviewMouseLeftButtonDown
			AddHandler AssociatedObject.PreviewMouseLeftButtonUp, AddressOf OnPreviewMouseLeftButtonUp
			AddHandler AssociatedObject.PreviewMouseMove, AddressOf OnPreviewMouseMove


			DocumentPanel = LayoutTreeHelper.GetVisualChildren(AssociatedObject).OfType(Of DocumentViewerPanel)().FirstOrDefault()
			If DocumentPanel Is Nothing Then
				Return
			End If

			ControlAdornerLayer = AdornerLayer.GetAdornerLayer(DocumentPanel)
			SelectionAdorner = New SelectionAdorner(DocumentPanel)

			ControlAdornerLayer.Add(SelectionAdorner)
			AddHandler SelectionAdorner.PreviewMouseLeftButtonDown, AddressOf OnAdornerPreviewMouseLeftButtonDown
			AddHandler SelectionAdorner.PreviewMouseMove, AddressOf OnAdornerPreviewMouseMove
			AddHandler SelectionAdorner.PreviewMouseLeftButtonUp, AddressOf OnAdornerPreviewMouseLeftButtonUp

		End Sub


		Private Sub OnScrollChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.ScrollChangedEventArgs)
			Dim startPoint = SelectionAdorner.Location
			startPoint.Offset(-e.HorizontalChange, -e.VerticalChange)
			SelectionAdorner.Location = startPoint
			UpdateAdorner()
		End Sub

		'Create Adorner 
		Protected Overridable Sub OnPreviewMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If Not (TypeOf e.OriginalSource Is DevExpress.Xpf.PdfViewer.DXScrollViewer) Then
				Return
			End If
			If DocumentPanel Is Nothing Then
				Return
			End If

			SelectionAdorner.Location = e.GetPosition(DocumentPanel)
			If Not DocumentPanel.IsMouseCaptured Then
				DocumentPanel.CaptureMouse()
				e.Handled = True
			End If
		End Sub

		Private Sub OnPreviewMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If DocumentPanel Is Nothing Then
				Return
			End If

			If DocumentPanel.IsMouseCaptured AndAlso e.LeftButton = MouseButtonState.Pressed Then
				Dim mouseOffset = e.GetPosition(SelectionAdorner)
				mouseOffset.Offset(-SelectionAdorner.Location.X, -SelectionAdorner.Location.Y)
				SelectionAdorner.OffSet = mouseOffset
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub

		Protected Overridable Sub OnPreviewMouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If DocumentPanel Is Nothing Then
				Return
			End If

			If DocumentPanel.IsMouseCaptured Then
				DocumentPanel.ReleaseMouseCapture()
				UpdateAdorner()
				NotifyPdfSelectionChanged()
				e.Handled = True
			End If
		End Sub

		' Move Adorner
		Private Sub OnAdornerPreviewMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			mouseStartPoint = e.GetPosition(SelectionAdorner)
			If Not SelectionAdorner.IsMouseCaptured Then
				SelectionAdorner.CaptureMouse()
			End If
			e.Handled = True
		End Sub

		Private Sub OnAdornerPreviewMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If SelectionAdorner.IsMouseCaptured AndAlso e.LeftButton = MouseButtonState.Pressed Then
				Dim mouseOffset = e.GetPosition(SelectionAdorner)
				mouseOffset.Offset(-mouseStartPoint.X, -mouseStartPoint.Y)
				mouseStartPoint = e.GetPosition(SelectionAdorner)
				Dim adornerPosition = SelectionAdorner.Location
				adornerPosition.Offset(mouseOffset.X, mouseOffset.Y)
				SelectionAdorner.Location = adornerPosition
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub

		Private Sub OnAdornerPreviewMouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If SelectionAdorner.IsMouseCaptured Then
				SelectionAdorner.ReleaseMouseCapture()
				NotifyPdfSelectionChanged()
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub
		Protected Overridable Sub DestroyAdorner()
			If SelectionAdorner IsNot Nothing Then
				ControlAdornerLayer.Remove(SelectionAdorner)
			End If
		End Sub

		Protected Overridable Sub UpdateAdorner()
			If SelectionAdorner IsNot Nothing Then
				SelectionAdorner.InvalidateVisual()
			End If
		End Sub

		Protected Overrides Sub OnDetaching()
			RemoveHandler AssociatedObject.Loaded, AddressOf OnLoaded
			DestroyAdorner()
			MyBase.OnDetaching()
		End Sub
	End Class

	Public Class SelectionAdorner
		Inherits Adorner

		Private privatePdfViewer As DocumentViewerPanel
		Protected Property PdfViewer() As DocumentViewerPanel
			Get
				Return privatePdfViewer
			End Get
			Private Set(ByVal value As DocumentViewerPanel)
				privatePdfViewer = value
			End Set
		End Property
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
			PdfViewer = adornedElement
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
