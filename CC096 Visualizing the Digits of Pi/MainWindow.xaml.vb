Imports System.IO

Class MainWindow
    Private MyBrushes As List(Of Brush)
    Private Pi As String
    Private MyCircleRadius As Double
    Private Arcs As List(Of ArcSegment) = New List(Of ArcSegment)
    Private startAngles(9) As Double
    Private endAngles(9) As Double
    Private digitCounter(9) As Integer
    Private LineColors(9) As Brush
    Private SegmentColors(9) As Brush
    Private counter As Integer = 0
    Private digit As Integer
    Private angleOffset As Double
    Private l As Line
    Private Rnd As Random = New Random()
    Private MyLoaded As Boolean = False

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim sr As StreamReader = New StreamReader(Environment.CurrentDirectory & "\Pi.txt")
        Pi = sr.ReadToEnd()
        sr.Close()
        'Define the colors
        Dim alpha As Byte = 100
        SegmentColors(0) = New SolidColorBrush(Color.FromRgb(255, 121, 117))
        SegmentColors(1) = New SolidColorBrush(Color.FromRgb(135, 250, 135))
        SegmentColors(2) = New SolidColorBrush(Color.FromRgb(70, 140, 240))
        SegmentColors(3) = New SolidColorBrush(Color.FromRgb(216, 216, 110))
        SegmentColors(4) = New SolidColorBrush(Color.FromRgb(70, 185, 75))
        SegmentColors(5) = New SolidColorBrush(Color.FromRgb(195, 86, 125))
        SegmentColors(6) = New SolidColorBrush(Color.FromRgb(60, 70, 247))
        SegmentColors(7) = New SolidColorBrush(Color.FromRgb(228, 168, 97))
        SegmentColors(8) = New SolidColorBrush(Color.FromRgb(110, 210, 210))
        SegmentColors(9) = New SolidColorBrush(Color.FromRgb(140, 140, 240))
        LineColors(0) = New SolidColorBrush(Color.FromArgb(alpha, 255, 121, 117))
        LineColors(1) = New SolidColorBrush(Color.FromArgb(alpha, 135, 250, 135))
        LineColors(2) = New SolidColorBrush(Color.FromArgb(alpha, 70, 140, 240))
        LineColors(3) = New SolidColorBrush(Color.FromArgb(alpha, 216, 216, 110))
        LineColors(4) = New SolidColorBrush(Color.FromArgb(alpha, 70, 185, 75))
        LineColors(5) = New SolidColorBrush(Color.FromArgb(alpha, 195, 86, 125))
        LineColors(6) = New SolidColorBrush(Color.FromArgb(alpha, 60, 70, 247))
        LineColors(7) = New SolidColorBrush(Color.FromArgb(alpha, 228, 168, 97))
        LineColors(8) = New SolidColorBrush(Color.FromArgb(alpha, 110, 210, 210))
        LineColors(9) = New SolidColorBrush(Color.FromArgb(alpha, 140, 140, 240))
        MyLoaded = True
        Init()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub Init()
        MyCanvas.Children.Clear()
        Dim my_Path As Shapes.Path
        Dim my_PG As PathGeometry
        Dim my_figure As PathFigure
        For I As Integer = 0 To 9
            digitCounter(I) = 0
        Next
        'Get the Graph Diameter
        If MyCanvas.ActualWidth > MyCanvas.ActualHeight Then
            MyCircleRadius = MyCanvas.ActualHeight / 2 - 50
        Else
            MyCircleRadius = MyCanvas.ActualWidth / 2 - 50
        End If
        my_Path = New Shapes.Path
        my_PG = New PathGeometry()
        my_figure = New PathFigure
        my_PG.Figures.Add(my_figure)
        my_Path.Data = my_PG
        'Set the arcSegments
        For I As Integer = 0 To 9
            startAngles(I) = (36 * I + 2) * Math.PI / 180
            endAngles(I) = (36 * (I + 1) - 2) * Math.PI / 180
            Dim arcpt1 As Point = New Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Cos(startAngles(I)), MyCanvas.ActualHeight / 2 + MyCircleRadius * Math.Sin(startAngles(I)))
            Dim arcpt2 As Point = New Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Cos(endAngles(I)), MyCanvas.ActualHeight / 2 + MyCircleRadius * Math.Sin(endAngles(I)))
            'Step2: Make an ArcSegment and set it in my_figure
            my_Path = New Shapes.Path
            my_PG = New PathGeometry()
            my_figure = New PathFigure
            my_PG.Figures.Add(my_figure)
            my_Path.Data = my_PG
            my_Path.Stroke = SegmentColors(I)
            my_Path.StrokeThickness = 15.0
            my_figure.StartPoint = arcpt1
            my_figure.Segments.Add(New ArcSegment(arcpt2, New Size(MyCircleRadius, MyCircleRadius), 30, False, SweepDirection.Clockwise, True))
            MyCanvas.Children.Add(my_Path)
        Next
        'Set the Digit labels
        Dim lbl As Symbol
        Dim lblAngle As Double
        For I As Integer = 0 To 9
            lblAngle = (36 * I + 18) * Math.PI / 180
            Dim pt As Point = New Point(0, 0)
            lbl = New Symbol(I.ToString(), pt, Colors.White, "Arial", 18)
            lbl.Left = MyCanvas.ActualWidth / 2 + 1.06 * MyCircleRadius * Math.Cos(lblAngle) - lbl.Width / 2
            lbl.Top = MyCanvas.ActualHeight / 2 + 1.06 * MyCircleRadius * Math.Sin(lblAngle) - lbl.Height / 2
            MyCanvas.Children.Add(lbl)
        Next
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To 20
            l = New Line()
            'Get line start point
            digit = Integer.Parse(Pi(counter))
            counter += 1
            angleOffset = (digitCounter(digit) + 50) * 0.0005
            digitCounter(digit) += 1
            If digitCounter(digit) > 1000 Then digitCounter(digit) = 0
            l.Stroke = LineColors(digit)
            l.StrokeThickness = 1.0
            l.X1 = MyCanvas.ActualWidth / 2 + (MyCircleRadius - 25) * Math.Cos(startAngles(digit) + angleOffset)
            l.Y1 = MyCanvas.ActualHeight / 2 + (MyCircleRadius - 25) * Math.Sin(startAngles(digit) + angleOffset)
            'Get line end point
            digit = Integer.Parse(Pi(counter))
            counter += 1
            angleOffset = (digitCounter(digit) + 50) * 0.0005
            digitCounter(digit) += 1
            If digitCounter(digit) > 1000 Then digitCounter(digit) = 0
            l.X2 = MyCanvas.ActualWidth / 2 + (MyCircleRadius - 25) * Math.Cos(startAngles(digit) + angleOffset)
            l.Y2 = MyCanvas.ActualHeight / 2 + (MyCircleRadius - 25) * Math.Sin(startAngles(digit) + angleOffset)
            'Show the line
            MyCanvas.Children.Add(l)
        Next
        If counter > 10000 Then
            counter = 0
            Init()
        End If
    End Sub

    Private Sub MainWindow_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If MyLoaded Then
            Init()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
