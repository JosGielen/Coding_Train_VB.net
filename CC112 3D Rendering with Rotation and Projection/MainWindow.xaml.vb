Class MainWindow
    Private points(7) As Matrix
    Private dots(7) As Ellipse
    Private dotSize As Double = 8
    Private lines(11) As Line
    Private ProjectionMatrix As Matrix
    Private RotationX As Matrix
    Private RotationY As Matrix
    Private RotationZ As Matrix
    Private Projected(7) As Matrix
    Private w As Double = 0.0
    Private h As Double = 0.0
    Private AngleX As Double = 0.0
    Private AngleY As Double = 0.0
    Private AngleZ As Double = 0.0
    Private DeltaAngleX As Double = 0.01
    Private DeltaAngleY As Double = 0.015
    Private DeltaAngleZ As Double = 0.01
    Private Perspective As Boolean = False
    Private Rendering As Boolean
    Private LastTimeRendered As Double
    Private RenderPeriod As Double

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim el As Ellipse
        Dim l As Line
        w = canvas1.ActualWidth / 4
        h = canvas1.ActualHeight / 4
        RenderPeriod = 40   '50 frames per second
        'Create the corner points
        Dim index As Integer = 0
        For X As Integer = -1 To 1 Step 2
            For Y As Integer = -1 To 1 Step 2
                For Z As Integer = -1 To 1 Step 2
                    points(index) = Matrix.FromArray({X * w, Y * w, Z * w})
                    index += 1
                Next
            Next
        Next
        'Create the corner dots of the cube
        For I As Integer = 0 To 7
            el = New Ellipse() With {
                .Width = dotSize,
                .Height = dotSize,
                .Fill = Brushes.Black
            }
            dots(I) = el
            canvas1.Children.Add(el)
        Next
        'Create the edges of the cube
        For I As Integer = 0 To 11
            l = New Line() With {
                .Stroke = Brushes.Black,
                .StrokeThickness = 2}
            lines(I) = l
            canvas1.Children.Add(l)
        Next
        'Create default Rotation Matrices
        RotationX = New Matrix(3)
        RotationY = New Matrix(3)
        RotationZ = New Matrix(3)
        'Create default Projection Matrix
        ProjectionMatrix = Matrix.FromArray({{1, 0, 0}, {0, 1, 0}})
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If (Not Rendering) Then
            AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            Rendering = True
        Else
            RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            Rendering = False
        End If
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim rargs As RenderingEventArgs = CType(e, RenderingEventArgs)
        Dim Rotated(7) As Matrix
        Dim Zfactor As Double = 1.0
        If rargs.RenderingTime.TotalMilliseconds - LastTimeRendered > RenderPeriod Then
            'Create the Rotation Matrices
            RotationX = Matrix.FromArray({{1, 0, 0}, {0, Math.Cos(AngleX), -Math.Sin(AngleX)}, {0, Math.Sin(AngleX), Math.Cos(AngleX)}})
            RotationY = Matrix.FromArray({{Math.Cos(AngleY), 0, Math.Sin(AngleY)}, {0, 1, 0}, {-Math.Sin(AngleY), 0, Math.Cos(AngleY)}})
            RotationZ = Matrix.FromArray({{Math.Cos(AngleZ), -Math.Sin(AngleZ), 0}, {Math.Sin(AngleZ), Math.Cos(AngleZ), 0}, {0, 0, 1}})
            'Rotate and project the points on the canvas
            For I As Integer = 0 To points.Length - 1
                Rotated(I) = RotationX * points(I)
                Rotated(I) = RotationY * Rotated(I)
                Rotated(I) = RotationZ * Rotated(I)
                If Perspective Then
                    Zfactor = 27 / (30 - Rotated(I).Value(2, 0) / 30)
                    ProjectionMatrix = Matrix.FromArray({{Zfactor, 0, 0}, {0, Zfactor, 0}})
                Else
                    ProjectionMatrix = Matrix.FromArray({{1, 0, 0}, {0, 1, 0}})
                End If
                Projected(I) = ProjectionMatrix * Rotated(I)
            Next
            'Draw the cube
            Draw()
            'Update the angles
            AngleX += DeltaAngleX
            AngleY += DeltaAngleY
            AngleZ += DeltaAngleZ
            LastTimeRendered = rargs.RenderingTime.TotalMilliseconds
        End If
    End Sub

    Private Sub Draw()
        'Draw the corners on the Canvas
        For I As Integer = 0 To dots.Length - 1
            dots(I).SetValue(Canvas.LeftProperty, Projected(I).Value(0, 0) + 2 * w - dotSize / 2)
            dots(I).SetValue(Canvas.TopProperty, Projected(I).Value(1, 0) + 2 * h - dotSize / 2)
        Next
        'Draw the edges on the Canvas
        lines(0).X1 = Projected(0).Value(0, 0) + 2 * w
        lines(0).Y1 = Projected(0).Value(1, 0) + 2 * h
        lines(0).X2 = Projected(1).Value(0, 0) + 2 * w
        lines(0).Y2 = Projected(1).Value(1, 0) + 2 * h

        lines(1).X1 = Projected(0).Value(0, 0) + 2 * w
        lines(1).Y1 = Projected(0).Value(1, 0) + 2 * h
        lines(1).X2 = Projected(2).Value(0, 0) + 2 * w
        lines(1).Y2 = Projected(2).Value(1, 0) + 2 * h

        lines(2).X1 = Projected(3).Value(0, 0) + 2 * w
        lines(2).Y1 = Projected(3).Value(1, 0) + 2 * h
        lines(2).X2 = Projected(1).Value(0, 0) + 2 * w
        lines(2).Y2 = Projected(1).Value(1, 0) + 2 * h

        lines(3).X1 = Projected(3).Value(0, 0) + 2 * w
        lines(3).Y1 = Projected(3).Value(1, 0) + 2 * h
        lines(3).X2 = Projected(2).Value(0, 0) + 2 * w
        lines(3).Y2 = Projected(2).Value(1, 0) + 2 * h

        lines(4).X1 = Projected(4).Value(0, 0) + 2 * w
        lines(4).Y1 = Projected(4).Value(1, 0) + 2 * h
        lines(4).X2 = Projected(5).Value(0, 0) + 2 * w
        lines(4).Y2 = Projected(5).Value(1, 0) + 2 * h

        lines(5).X1 = Projected(4).Value(0, 0) + 2 * w
        lines(5).Y1 = Projected(4).Value(1, 0) + 2 * h
        lines(5).X2 = Projected(6).Value(0, 0) + 2 * w
        lines(5).Y2 = Projected(6).Value(1, 0) + 2 * h

        lines(6).X1 = Projected(7).Value(0, 0) + 2 * w
        lines(6).Y1 = Projected(7).Value(1, 0) + 2 * h
        lines(6).X2 = Projected(5).Value(0, 0) + 2 * w
        lines(6).Y2 = Projected(5).Value(1, 0) + 2 * h

        lines(7).X1 = Projected(7).Value(0, 0) + 2 * w
        lines(7).Y1 = Projected(7).Value(1, 0) + 2 * h
        lines(7).X2 = Projected(6).Value(0, 0) + 2 * w
        lines(7).Y2 = Projected(6).Value(1, 0) + 2 * h

        lines(8).X1 = Projected(0).Value(0, 0) + 2 * w
        lines(8).Y1 = Projected(0).Value(1, 0) + 2 * h
        lines(8).X2 = Projected(4).Value(0, 0) + 2 * w
        lines(8).Y2 = Projected(4).Value(1, 0) + 2 * h

        lines(9).X1 = Projected(1).Value(0, 0) + 2 * w
        lines(9).Y1 = Projected(1).Value(1, 0) + 2 * h
        lines(9).X2 = Projected(5).Value(0, 0) + 2 * w
        lines(9).Y2 = Projected(5).Value(1, 0) + 2 * h

        lines(10).X1 = Projected(2).Value(0, 0) + 2 * w
        lines(10).Y1 = Projected(2).Value(1, 0) + 2 * h
        lines(10).X2 = Projected(6).Value(0, 0) + 2 * w
        lines(10).Y2 = Projected(6).Value(1, 0) + 2 * h

        lines(11).X1 = Projected(3).Value(0, 0) + 2 * w
        lines(11).Y1 = Projected(3).Value(1, 0) + 2 * h
        lines(11).X2 = Projected(7).Value(0, 0) + 2 * w
        lines(11).Y2 = Projected(7).Value(1, 0) + 2 * h

    End Sub

    Private Sub Window_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Perspective = Not Perspective
    End Sub
End Class
