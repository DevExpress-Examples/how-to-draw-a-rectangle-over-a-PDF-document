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

		Private controlAdornerLayer As AdornerLayer
		Private selectionAdorner As SelectionAdorner
		Private mouseStartPoint As New Point()
		Private documentPanel As DocumentViewerPanel

		Public Event PdfSelectionChanged As PdfSelectionChangedEventHandler
		Protected Sub NotifyPdfSelectionChanged()
			If PdfSelectionChangedEvent IsNot Nothing Then
				Dim startPoint = selectionAdorner.Location
				Dim endPoint = startPoint
				endPoint.Offset(selectionAdorner.OffSet.X, selectionAdorner.OffSet.Y)
				Dim startPosition As PdfDocumentPosition = AssociatedObject.ConvertPixelToDocumentPosition(documentPanel.TranslatePoint(startPoint, AssociatedObject))
				Dim endPosition As PdfDocumentPosition = AssociatedObject.ConvertPixelToDocumentPosition(documentPanel.TranslatePoint(endPoint, AssociatedObject))
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

			documentPanel = LayoutTreeHelper.GetVisualChildren(AssociatedObject).OfType(Of DocumentViewerPanel)().FirstOrDefault()
			If documentPanel Is Nothing Then
				Return
			End If

			controlAdornerLayer = AdornerLayer.GetAdornerLayer(documentPanel)
			selectionAdorner = New SelectionAdorner(documentPanel)

			controlAdornerLayer.Add(selectionAdorner)
			AddHandler selectionAdorner.PreviewMouseLeftButtonDown, AddressOf OnAdornerPreviewMouseLeftButtonDown
			AddHandler selectionAdorner.PreviewMouseMove, AddressOf OnAdornerPreviewMouseMove
			AddHandler selectionAdorner.PreviewMouseLeftButtonUp, AddressOf OnAdornerPreviewMouseLeftButtonUp

		End Sub
		Private Sub OnScrollChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.ScrollChangedEventArgs)
			Dim startPoint = selectionAdorner.Location
			startPoint.Offset(-e.HorizontalChange, -e.VerticalChange)
			selectionAdorner.Location = startPoint
			UpdateAdorner()
		End Sub

		'Create Adorner 
		Protected Overridable Sub OnPreviewMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If Not (TypeOf e.OriginalSource Is DevExpress.Xpf.PdfViewer.DXScrollViewer) Then
				Return
			End If
			If documentPanel Is Nothing Then
				Return
			End If

			selectionAdorner.Location = e.GetPosition(documentPanel)
			If Not documentPanel.IsMouseCaptured Then
				documentPanel.CaptureMouse()
				e.Handled = True
			End If
		End Sub

		Private Sub OnPreviewMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If documentPanel Is Nothing Then
				Return
			End If

			If documentPanel.IsMouseCaptured AndAlso e.LeftButton = MouseButtonState.Pressed Then
				Dim mouseOffset = e.GetPosition(selectionAdorner)
				mouseOffset.Offset(-selectionAdorner.Location.X, -selectionAdorner.Location.Y)
				selectionAdorner.OffSet = mouseOffset
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub

		Protected Overridable Sub OnPreviewMouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If documentPanel Is Nothing Then
				Return
			End If

			If documentPanel.IsMouseCaptured Then
				documentPanel.ReleaseMouseCapture()
				UpdateAdorner()
				NotifyPdfSelectionChanged()
				e.Handled = True
			End If
		End Sub

		' Move Adorner
		Private Sub OnAdornerPreviewMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			mouseStartPoint = e.GetPosition(selectionAdorner)
			If Not selectionAdorner.IsMouseCaptured Then
				selectionAdorner.CaptureMouse()
			End If
			e.Handled = True
		End Sub

		Private Sub OnAdornerPreviewMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If selectionAdorner.IsMouseCaptured AndAlso e.LeftButton = MouseButtonState.Pressed Then
				Dim mouseOffset = e.GetPosition(selectionAdorner)
				mouseOffset.Offset(-mouseStartPoint.X, -mouseStartPoint.Y)
				mouseStartPoint = e.GetPosition(selectionAdorner)
				Dim adornerPosition = selectionAdorner.Location
				adornerPosition.Offset(mouseOffset.X, mouseOffset.Y)
				selectionAdorner.Location = adornerPosition
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub

		Private Sub OnAdornerPreviewMouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If selectionAdorner.IsMouseCaptured Then
				selectionAdorner.ReleaseMouseCapture()
				NotifyPdfSelectionChanged()
				UpdateAdorner()
				e.Handled = True
			End If
		End Sub
		Protected Overridable Sub DestroyAdorner()
			If selectionAdorner IsNot Nothing Then
				controlAdornerLayer.Remove(selectionAdorner)
			End If
		End Sub

		Protected Overridable Sub UpdateAdorner()
			If selectionAdorner IsNot Nothing Then
				selectionAdorner.InvalidateVisual()
			End If
		End Sub

		Protected Overrides Sub OnDetaching()
			RemoveHandler AssociatedObject.Loaded, AddressOf OnLoaded
			DestroyAdorner()
			MyBase.OnDetaching()
		End Sub
	End Class

End Namespace
