Imports System.IO
Imports System.Threading

Class MainWindow
    Private Circles As List(Of CircleObstacle)
    Private Squares As List(Of SquareObstacle)
    Private CircleCount As Integer = 8
    Private r As Ray
    Private frameNumber As Integer = 0
    Private recording As Boolean = False
    Private rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Circles = New List(Of CircleObstacle)
        Squares = New List(Of SquareObstacle)
        Dim Circ As CircleObstacle
        Dim Sq As SquareObstacle
        Dim pt As Point
        Dim size As Double
        'Add 4 square obstacles
        Sq = New SquareObstacle(New Point(canvas1.ActualWidth / 5, canvas1.ActualHeight / 2), 50)
        Sq.Show(canvas1)
        Squares.Add(Sq)
        Sq = New SquareObstacle(New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 4), 50)
        Sq.Show(canvas1)
        Squares.Add(Sq)
        Sq = New SquareObstacle(New Point(4 * canvas1.ActualWidth / 5, canvas1.ActualHeight / 2), 50)
        Sq.Show(canvas1)
        Squares.Add(Sq)
        Sq = New SquareObstacle(New Point(canvas1.ActualWidth / 2, 3 * canvas1.ActualHeight / 4), 50)
        Sq.Show(canvas1)
        Squares.Add(Sq)
        'Add the circles (no overlapping)
        Do
            pt = New Point((canvas1.ActualWidth - 2 * size) * rnd.NextDouble() + size, (canvas1.ActualHeight - 2 * size) * rnd.NextDouble() + size)
            size = 20 * rnd.NextDouble() + 20
            Dim ok As Boolean = True
            For I As Integer = 0 To Circles.Count - 1
                If (pt - Circles(I).Pos).Length <= size + Circles(I).Radius Then
                    ok = False
                    Exit For
                End If
            Next
            For I As Integer = 0 To Squares.Count - 1
                If (pt - Squares(I).Pos).Length < size + 1.5 * Squares(I).Size Then
                    ok = False
                    Exit For
                End If
            Next
            If ok Then
                Circ = New CircleObstacle(pt, size, 360 * rnd.NextDouble(), rnd.NextDouble() + 0.6)
                Circ.Show(canvas1)
                Circles.Add(Circ)
            End If
        Loop While Circles.Count < CircleCount

        r = New Ray(New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2), 0.0)
        r.Show(canvas1)
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim currentPos As Point
        Dim minDist As Double
        Dim dist As Double
        Dim closestCircle As CircleObstacle
        Dim closestSquare As SquareObstacle
        Dim circle As Ellipse
        If recording Then
            Dim imageName As String = Environment.CurrentDirectory & "\Output\Image-" & frameNumber.ToString("0000") & ".jpg"
            SaveImage(imageName)
            frameNumber += 1
            Thread.Sleep(50)
        End If
        canvas1.Children.Clear()
        For I As Integer = 0 To Squares.Count - 1
            Squares(I).Show(canvas1)
        Next
        'Move the Circles
        For I As Integer = 0 To Circles.Count - 1
            Circles(I).Update()
            'Check for obstacles collision
            For J As Integer = I + 1 To Circles.Count - 1
                Circles(I).Collide(Circles(J))
            Next
            For J As Integer = 0 To Squares.Count - 1
                Circles(I).Collide(Squares(J))
            Next
            Circles(I).Show(canvas1)
        Next
        'March the Ray
        closestCircle = Nothing
        closestSquare = Nothing
        currentPos = r.Pos
        r.Show(canvas1)
        Do
            minDist = Double.MaxValue
            For I As Integer = 0 To Circles.Count - 1
                dist = Circles(I).Signeddistance(currentPos)
                If dist < minDist Then
                    minDist = dist
                End If
                If Math.Abs(dist) < 1 Then
                    closestCircle = Circles(I)
                    Exit Do
                End If
            Next
            For I As Integer = 0 To Squares.Count - 1
                dist = Squares(I).Signeddistance(currentPos)
                If dist < minDist Then
                    minDist = dist
                End If
                If Math.Abs(dist) < 1 Then
                    closestSquare = Squares(I)
                    Exit Do
                End If
            Next
            If minDist > 0 Then
                r.X2 = (currentPos + minDist * r.Dir).X
                r.Y2 = (currentPos + minDist * r.Dir).Y
                circle = New Ellipse() With
                {
                    .Width = 2 * minDist,
                    .Height = 2 * minDist,
                    .Stroke = Brushes.Blue,
                    .StrokeThickness = 1.0,
                    .Fill = New SolidColorBrush(Color.FromArgb(80, 0, 0, 255))
                }
                circle.SetValue(Canvas.LeftProperty, currentPos.X - minDist)
                circle.SetValue(Canvas.TopProperty, currentPos.Y - minDist)
                canvas1.Children.Add(circle)
            Else
                r.X2 = r.Pos.X
                r.Y2 = r.Pos.Y
            End If
            currentPos = currentPos + Math.Abs(minDist) * r.Dir
            If currentPos.X < 0 Or currentPos.X > canvas1.ActualWidth Or currentPos.Y < 0 Or currentPos.Y > canvas1.ActualHeight Then
                Exit Do
            End If
        Loop
        'Show the Ray end point as a red dot and highlight the target
        If closestCircle IsNot Nothing Or closestSquare IsNot Nothing Then
            If closestCircle IsNot Nothing Then closestCircle.HighLite(True)
            If closestSquare IsNot Nothing Then closestSquare.HighLite(True)
            If minDist > 0 Then
                Dim spot As Ellipse = New Ellipse() With
                {
                    .Width = 4,
                    .Height = 4,
                    .Stroke = Brushes.Red,
                    .StrokeThickness = 1.0,
                    .Fill = Brushes.Red
                }
                spot.SetValue(Canvas.LeftProperty, currentPos.X - 2)
                spot.SetValue(Canvas.TopProperty, currentPos.Y - 2)
                canvas1.Children.Add(spot)
            End If
        End If
        'Show the Ray origin as a white dot 
        Dim origin As Ellipse = New Ellipse() With
        {
            .Width = 6,
            .Height = 6,
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0,
            .Fill = Brushes.White
        }
        origin.SetValue(Canvas.LeftProperty, r.Pos.X - 3)
        origin.SetValue(Canvas.TopProperty, r.Pos.Y - 3)
        canvas1.Children.Add(origin)
        'Rotate the ray
        r.Rotate(0.15)
    End Sub

    Private Sub SaveImage(filename As String)
        Dim MyEncoder As BitmapEncoder = New JpegBitmapEncoder()
        Dim renderbmp As RenderTargetBitmap = New RenderTargetBitmap(CInt(canvas1.ActualWidth), CInt(canvas1.ActualHeight), 96.0, 96.0, PixelFormats.Default)
        renderbmp.Render(canvas1)
        Try
            MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp))
            ' Create a FileStream to write the image to the file.
            Using sw As FileStream = New FileStream(filename, FileMode.Create)
                MyEncoder.Save(sw)
            End Using
        Catch ex As Exception
            MessageBox.Show("The Image could not be saved.", "RayMarching error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
