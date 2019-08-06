Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private Rnd As Random = New Random()
    Private points As List(Of Point)
    Private shell As List(Of Point)
    Private PointCount As Integer = 500
    'Lines to show how the algorithm works
    Private refLine As Line
    Private testLine As Line

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim p As Point
        Dim El As Ellipse
        points = New List(Of Point)
        shell = New List(Of Point)
        'Draw the points in the canvas
        For I As Integer = 0 To PointCount - 1
            p = New Point(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble())
            points.Add(p)
            El = New Ellipse() With
            {
                .Width = 4,
                .Height = 4,
                .Fill = Brushes.Black
            }
            El.SetValue(Canvas.LeftProperty, p.X - 2)
            El.SetValue(Canvas.TopProperty, p.Y - 2)
            canvas1.Children.Add(El)
        Next
        'Add the show lines to the canvas
        refLine = New Line With
        {
            .Stroke = Brushes.Blue,
            .StrokeThickness = 1.0,
            .X1 = 0.0,
            .Y1 = 0.0,
            .X2 = 0.0,
            .Y2 = 0.0
        }
        canvas1.Children.Add(refLine)
        testLine = New Line With
        {
            .Stroke = Brushes.Green,
            .StrokeThickness = 1.0,
            .X1 = 0.0,
            .Y1 = 0.0,
            .X2 = 0.0,
            .Y2 = 0.0
        }
        canvas1.Children.Add(testLine)
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        'Sort the points to get the left most
        Dim left As Point
        Dim referencePt As Point
        Dim testPt As Point
        Dim RefV As Vector
        Dim TestV As Vector
        left = points(0)
        For I As Integer = 0 To points.Count - 1
            If left.X > points(I).X Then left = points(I)
        Next
        'Calculate the Shell starting from the left most point
        shell.Add(left)
        Do
            referencePt = points(shell.Count + 1)
            DrawRefLine(shell.Last, referencePt)
            For I As Integer = 0 To points.Count - 1
                testPt = points(I)
                DrawTestLine(shell.Last, testPt)
                RefV = referencePt - shell.Last
                TestV = testPt - shell.Last
                If Vector.CrossProduct(RefV, TestV) < 0 Then
                    referencePt = testPt
                    DrawRefLine(shell.Last, referencePt)
                End If
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            Next
            If referencePt = shell(0) Then
                DrawShell(shell.Last, shell(0))
                DrawTestLine(shell.Last, shell.Last)
                Exit Do
            Else
                shell.Add(referencePt)
                DrawShell(shell(shell.Count - 2), shell(shell.Count - 1))
            End If
        Loop
        Title = "FINISHED"
    End Sub

    Private Sub DrawRefLine(begin As Point, finish As Point)
        refLine.X1 = begin.X
        refLine.Y1 = begin.Y
        refLine.X2 = finish.X
        refLine.Y2 = finish.Y
    End Sub

    Private Sub DrawTestLine(begin As Point, finish As Point)
        testLine.X1 = begin.X
        testLine.Y1 = begin.Y
        testLine.X2 = finish.X
        testLine.Y2 = finish.Y
    End Sub

    Private Sub DrawShell(previous As Point, current As Point)
        Dim EL As Ellipse = New Ellipse() With
        {
            .Width = 8,
            .Height = 8,
            .Fill = Brushes.Red
        }
        EL.SetValue(Canvas.LeftProperty, current.X - 4)
        EL.SetValue(Canvas.TopProperty, current.Y - 4)
        canvas1.Children.Add(EL)
        Dim l As Line = New Line() With
        {
            .Stroke = Brushes.Red,
            .StrokeThickness = 2.0,
            .X1 = previous.X,
            .Y1 = previous.Y,
            .X2 = current.X,
            .Y2 = current.Y
        }
        canvas1.Children.Add(l)
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
