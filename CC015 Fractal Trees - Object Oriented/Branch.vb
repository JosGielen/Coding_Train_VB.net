Public Class Branch
    Private my_StartPt As Point
    Private my_EndPt As Point
    Private my_Line As Line
    Private my_Leaf As Ellipse
    Private gotBranches As Boolean

    Public Sub New(start_ As Point, end_ As Point)
        my_StartPt = start_
        my_EndPt = end_
        gotBranches = False
    End Sub

    Public Sub Show(c As Canvas)
        my_Line = New Line() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 2.0,
            .X1 = my_StartPt.X,
            .Y1 = my_StartPt.Y,
            .X2 = my_EndPt.X,
            .Y2 = my_EndPt.Y
        }
        c.Children.Add(my_Line)
    End Sub

    Public ReadOnly Property Length As Double
        Get
            Return (my_EndPt - my_StartPt).Length
        End Get
    End Property

    Public ReadOnly Property EndPt As Point
        Get
            Return my_EndPt
        End Get
    End Property

    Public Function Branch(angle As Double) As List(Of Branch)
        Dim result As List(Of Branch) = New List(Of Branch)
        If gotBranches = False And (my_EndPt - my_StartPt).Length > 3 Then
            Dim BranchEndPt As Point
            Dim RT As RotateTransform = New RotateTransform() With
            {
                .CenterX = my_EndPt.X,
                .CenterY = my_EndPt.Y,
                .Angle = angle
            }
            BranchEndPt = my_EndPt + 0.66 * (my_EndPt - my_StartPt)
            result.Add(New Branch(my_EndPt, RT.Transform(BranchEndPt)))
            RT.Angle = -angle
            result.Add(New Branch(my_EndPt, RT.Transform(BranchEndPt)))
            gotBranches = True
        End If
        Return result
    End Function

End Class
