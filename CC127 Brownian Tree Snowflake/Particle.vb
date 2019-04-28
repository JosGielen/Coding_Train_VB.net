Public Class Particle

    Private Pos As Vector
    Private Size As Double
    Private my_dot As Ellipse
    Private my_MaxX As Double
    Private my_YDev As Integer
    Private Shared Rnd As Random = New Random()

    Public Sub New(dotsize As Double, maxX As Double, Ydeviation As Integer)
        Pos = New Vector(maxX, Rnd.Next(Ydeviation))
        Size = dotsize
        my_MaxX = maxX
        my_YDev = Ydeviation
    End Sub

    Public Function GetX() As Double
        Return Pos.X
    End Function

    Public Function Update(particleList As List(Of Particle)) As Boolean
        Dim result As Boolean = False
        Pos.X -= 1
        Pos.Y += Rnd.Next(-1 * my_YDev, my_YDev + 1)
        'Constrain in ±30° cone
        If Pos.Y > 0.57 * Pos.X Then
            Pos.Y = 0.57 * Pos.X
        ElseIf Pos.Y < -0.57 * Pos.X Then
            Pos.Y = -0.57 * Pos.X
        End If
        'Check is Finished
        If Pos.X < 1 Then result = True
        For Each p As Particle In particleList
            If (p.Pos.X - Pos.X) ^ 2 + (p.Pos.Y - Pos.Y) ^ 2 <= Size * Size Then
                result = True
                Exit For
            End If
        Next
        Return result
    End Function

    Public Sub Show(c As Canvas)
        Dim Pt1 As Point = New Point(c.ActualWidth / 2 + Pos.X, c.ActualHeight / 2 + Pos.Y)
        Dim Pt2 As Point = New Point(c.ActualWidth / 2 + Pos.X, c.ActualHeight / 2 - Pos.Y)
        Dim rotT As RotateTransform = New RotateTransform(30, c.ActualWidth / 2, c.ActualHeight / 2)
        'Rotate 30° to make 1 direction vertical
        Pt1 = rotT.Transform(Pt1)
        Pt2 = rotT.Transform(Pt2)
        rotT.Angle = 60
        For I As Integer = 0 To 5
            'Set the calculated Dot
            Pt1 = rotT.Transform(Pt1)
            my_dot = New Ellipse With
            {
                .Height = Size,
                .Width = Size,
                .Stroke = Brushes.White,
                .StrokeThickness = 1.0,
                .Fill = Brushes.White
            }
            my_dot.SetValue(Canvas.LeftProperty, Pt1.X)
            my_dot.SetValue(Canvas.TopProperty, Pt1.Y)
            c.Children.Add(my_dot)
            'Set the mirror image dot
            Pt2 = rotT.Transform(Pt2)
            my_dot = New Ellipse With
            {
                .Height = Size,
                .Width = Size,
                .Stroke = Brushes.White,
                .StrokeThickness = 1.0,
                .Fill = Brushes.White
            }
            my_dot.SetValue(Canvas.LeftProperty, Pt2.X)
            my_dot.SetValue(Canvas.TopProperty, Pt2.Y)
            c.Children.Add(my_dot)
        Next
    End Sub

End Class
