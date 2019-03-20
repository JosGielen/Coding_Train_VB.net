Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Private Delegate Sub RenderDelegate()
    Private MyBrushes As List(Of Brush)
    Private Total As Integer
    Private MyCircleRadius As Double
    Private MyEllipse As Ellipse = New Ellipse()
    Private startPts As List(Of Point) = New List(Of Point)
    Private MyLines As List(Of Line) = New List(Of Line)
    Private Multiplier As Double = 2
    Private ColorIndex As Integer = 0
    Private MyLoaded As Boolean = False

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Title = "CLICK TO START"
        Total = 400
        'Define the colors
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow continuous.cpl")
        MyBrushes = pal.GetColorBrushes(256)
        MyLoaded = True
        Init()
    End Sub

    Private Sub Init()
        MyCanvas.Children.Clear()
        'Get the Graph Diameter
        If MyCanvas.ActualWidth > MyCanvas.ActualHeight Then
            MyCircleRadius = MyCanvas.ActualHeight / 2 - 10
        Else
            MyCircleRadius = MyCanvas.ActualWidth / 2 - 10
        End If
        'Set the circle
        MyEllipse.Width = 2 * MyCircleRadius
        MyEllipse.Height = 2 * MyCircleRadius
        MyEllipse.Stroke = MyBrushes(ColorIndex)
        MyEllipse.StrokeThickness = 1
        MyEllipse.SetValue(Canvas.TopProperty, MyCanvas.ActualHeight / 2 - MyCircleRadius)
        MyEllipse.SetValue(Canvas.LeftProperty, MyCanvas.ActualWidth / 2 - MyCircleRadius)
        MyCanvas.Children.Add(MyEllipse)
        'Calculate the Points on the circle
        startPts.Clear()
        For I As Integer = 0 To Total - 1
            startPts.Add(New Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(I * Math.PI / 200), MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(I * Math.PI / 200)))
        Next
        'Make the lines
        Dim Endvalue As Double
        MyLines.Clear()
        For I As Integer = 0 To Total - 1
            MyLines.Add(New Line())
            MyLines(I).StrokeThickness = 1
            Endvalue = Multiplier * I Mod Total
            MyLines(I).X1 = startPts(I).X
            MyLines(I).Y1 = startPts(I).Y
            MyLines(I).X2 = MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(Endvalue * Math.PI / 200)
            MyLines(I).Y2 = MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(Endvalue * Math.PI / 200)
            MyLines(I).Stroke = MyBrushes(ColorIndex)
            MyCanvas.Children.Add(MyLines(I))
        Next
    End Sub

    Private Sub Start()
        Do
            Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
            ColorIndex += 1
            If ColorIndex >= 256 Then ColorIndex = 0
            Multiplier += 0.01
            Title = Multiplier.ToString()
            Thread.Sleep(30)
        Loop
    End Sub

    Private Sub Render()
        'Generate the TimesGraphs
        Dim Endvalue As Double
        MyEllipse.Stroke = MyBrushes(ColorIndex)
        For I As Integer = 0 To Total - 1
            Endvalue = Multiplier * I Mod Total
            MyLines(I).X1 = startPts(I).X
            MyLines(I).Y1 = startPts(I).Y
            MyLines(I).X2 = MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(Endvalue * Math.PI / 200)
            MyLines(I).Y2 = MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(Endvalue * Math.PI / 200)
            MyLines(I).Stroke = MyBrushes(ColorIndex)
        Next
    End Sub

    Private Sub MainWindow_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If MyLoaded Then
            Init()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Start()
    End Sub
End Class
