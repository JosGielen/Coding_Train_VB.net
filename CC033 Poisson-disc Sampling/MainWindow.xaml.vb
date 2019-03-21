Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private Delegate Sub RenderDelegate()
    Private WaitTime As Integer = 5
    Private AppLoaded As Boolean = False
    Private AppRunning As Boolean = False
    Private FieldWidth As Integer = 0
    Private FieldHeight As Integer = 0
    Private Field(,) As Point
    Private Filled(,) As Boolean
    Private rows As Integer = 0
    Private cols As Integer = 0
    Private Active As List(Of Point)
    Private r As Double
    Private k As Integer
    Private w As Double
    Private rnd As Random

    Private Sub Init()
        Dim col As Integer = 0
        Dim row As Integer = 0
        Dim el As Ellipse

        FieldWidth = Canvas1.ActualWidth
        FieldHeight = Canvas1.ActualHeight
        r = 8.0
        k = 30
        w = r / Math.Sqrt(2)
        rows = CInt(FieldWidth / w)
        cols = CInt(FieldHeight / w)
        ReDim Field(rows, cols)
        ReDim Filled(rows, cols)
        For I As Integer = 0 To rows
            For J As Integer = 0 To cols
                Filled(I, J) = False
            Next
        Next
        Active = New List(Of Point)
        rnd = New Random()
        'Pick the first point
        Canvas1.Children.Clear()
        Dim p As Point = New Point(FieldWidth / 2, FieldHeight / 2)
        col = CInt(p.X / w)
        row = CInt(p.Y / w)
        Active.Add(p)
        Field(row, col) = p
        Filled(row, col) = True
        el = New Ellipse() With {
            .Width = 4,
            .Height = 4,
            .Fill = Brushes.Red}
        el.SetValue(Canvas.LeftProperty, p.X - 2)
        el.SetValue(Canvas.TopProperty, p.Y - 2)
        Canvas1.Children.Add(el)
    End Sub

#Region "Window Events"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
        AppLoaded = True
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Start()
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If AppLoaded Then
            AppRunning = False
            Init()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        AppRunning = False
        End
    End Sub

#End Region

    Private Sub Start()
        Dim col As Integer = 0
        Dim row As Integer = 0
        Dim index As Integer = 0
        Dim angle As Double
        Dim dist As Double
        Dim sample As Point
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim neighbor As Point
        Dim NewOK As Boolean = False
        Dim ActiveOK As Boolean = False
        Dim el As Ellipse

        AppRunning = True
        While AppRunning
            If Active.Count > 0 Then
                'Take a random point from the Active List
                index = rnd.Next(Active.Count)
                ActiveOK = False
                For N As Integer = 0 To k
                    'Make a new point at random location but between r and 2r from Active(Index) point
                    angle = 2 * Math.PI * rnd.NextDouble()
                    dist = r + r * rnd.NextDouble()
                    X = Active(index).X + dist * Math.Cos(angle)
                    Y = Active(index).Y + dist * Math.Sin(angle)
                    col = CInt(X / w)
                    row = CInt(Y / w)
                    If col < 1 Or col > cols - 1 Or row < 1 Or row > rows - 1 Then Continue For 'The new point must be inside the Canvas
                    sample = New Point(X, Y)
                    NewOK = True
                    'Check the cells around the cell where the new point is
                    For I As Integer = -1 To 1
                        For J As Integer = -1 To 1
                            If Filled(row + I, col + J) Then
                                neighbor = Field(row + I, col + J)
                                If Distance(sample, neighbor) <= r Then
                                    NewOK = False
                                End If
                            End If
                        Next
                    Next
                    If NewOK Then
                        Active.Add(sample)
                        Field(row, col) = sample
                        Filled(row, col) = True
                        ActiveOK = True
                        el = New Ellipse() With {
                            .Width = 4,
                            .Height = 4,
                            .Fill = Brushes.Red}
                        el.SetValue(Canvas.LeftProperty, sample.X - 2)
                        el.SetValue(Canvas.TopProperty, sample.Y - 2)
                        Canvas1.Children.Add(el)
                        'Exit For
                    End If
                Next
                If Not ActiveOK Then
                    el = New Ellipse() With {
                            .Width = 4,
                            .Height = 4,
                            .Fill = Brushes.Black}
                    el.SetValue(Canvas.LeftProperty, Active(index).X - 2)
                    el.SetValue(Canvas.TopProperty, Active(index).Y - 2)
                    Canvas1.Children.Add(el)
                    Active.RemoveAt(index)
                End If
            Else
                AppRunning = False
            End If
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        End While
    End Sub

    Private Sub Render()

    End Sub

    Private Function Distance(p1 As Point, p2 As Point) As Double
        Return Math.Sqrt((p1.X - p2.X) ^ 2 + (p1.Y - p2.Y) ^ 2)
    End Function

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

End Class
