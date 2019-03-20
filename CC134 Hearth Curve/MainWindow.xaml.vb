Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Private Delegate Sub DrawDelegate()
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private Rendering As Boolean
    Private WaitTime As Integer
    Private Rnd As Random = New Random()
    Private points As PointCollection
    Private polys As List(Of Polygon)
    Private total As Integer
    Private ST As ScaleTransform

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Title = "CLICK TO START"
        WaitTime = 300
        total = 20
        Rendering = False
        'WindowState = WindowState.Maximized
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If Not Rendering Then
            Init()
            Rendering = True
            Render()
        Else
            Rendering = False
        End If
    End Sub

    Private Sub Init()
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim t As Double = 0.0
        Dim p As Polygon
        canvas1.Children.Clear()
        points = New PointCollection()
        polys = New List(Of Polygon)
        Do
            t = t + 0.05
            X = 10 * (16 * (Math.Sin(t) ^ 3))
            Y = 10 * (13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t))
            points.Add(New Point(X, 10 - Y))
            If t > 2 * Math.PI Then Exit Do
        Loop
        For I As Integer = 0 To total
            p = New Polygon() With
            {
                .Stroke = Brushes.Black,
                .StrokeThickness = 2,
                .Fill = Brushes.Black,
                .Points = points
            }
            polys.Add(p)
            canvas1.Children.Add(p)
        Next
    End Sub

    Private Sub Render()
        If Not Rendering Then Exit Sub
        Dim scale As Double
        Dim Xpos As Double
        Dim Ypos As Double
        Do
            For Each p As Polygon In polys
                scale = 0.4 * Rnd.NextDouble() + 0.1
                Xpos = canvas1.ActualWidth * (0.8 * Rnd.NextDouble() + 0.1)
                Ypos = canvas1.ActualHeight * (0.8 * Rnd.NextDouble() + 0.1)
                p.SetValue(Canvas.LeftProperty, Xpos)
                p.SetValue(Canvas.TopProperty, Ypos)
                p.Fill = Brushes.Red
                ST = New ScaleTransform(scale, scale)
                p.RenderTransform = ST
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            Next
            If Not Rendering Then Exit Sub
        Loop
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class

