Public Class Tile
    Private my_Poly As Polygon
    Private my_LineColor As Brush
    Private my_LineThickness As Double
    Private my_FillColor As Brush
    Private my_StarColor As Brush
    Private HankinLines As List(Of Hankin)
    Private my_StarPoints As PointCollection
    Private my_Star As Polygon

    Public Sub New()
        my_Poly = New Polygon()
        my_LineColor = Brushes.Black
        my_FillColor = Brushes.White
        my_StarColor = Brushes.Transparent
        my_LineThickness = 1.0
    End Sub

    Public Property Points As PointCollection
        Get
            Return my_Poly.Points
        End Get
        Set(value As PointCollection)
            my_Poly.Points = value
        End Set
    End Property

    Public Property LineColor As Brush
        Get
            Return my_LineColor
        End Get
        Set(value As Brush)
            my_LineColor = value
        End Set
    End Property

    Public Property LineThickness As Double
        Get
            Return my_LineThickness
        End Get
        Set(value As Double)
            my_LineThickness = value
        End Set
    End Property

    Public Property FillColor As Brush
        Get
            Return my_FillColor
        End Get
        Set(value As Brush)
            my_FillColor = value
        End Set
    End Property

    Public Property StarColor As Brush
        Get
            Return my_StarColor
        End Get
        Set(value As Brush)
            my_StarColor = value
        End Set
    End Property

    Public Sub AddPoint(X As Double, Y As Double)
        my_Poly.Points.Add(New Point(X, Y))
    End Sub

    Public Sub MakeStar(delta As Double, angle As Double)
        my_Poly.Points.Add(my_Poly.Points(0))
        Dim mrot As Matrix
        Dim bestPt As Point
        HankinLines = New List(Of Hankin)
        my_StarPoints = New PointCollection
        'Make the Hankins
        Dim h1 As Hankin
        Dim h2 As Hankin
        Dim V As Vector
        For I As Integer = 0 To my_Poly.Points.Count - 2
            h1 = New Hankin()
            h2 = New Hankin()
            V = my_Poly.Points(I + 1) - my_Poly.Points(I)
            h1.pt = my_Poly.Points(I) + (0.5 - delta / 100) * V
            mrot = New Matrix()
            mrot.Rotate(angle)
            h1.dir = mrot.Transform(V)
            HankinLines.Add(h1)
            h2.pt = my_Poly.Points(I) + (0.5 + delta / 100) * V
            mrot = New Matrix()
            mrot.Rotate(180 - angle)
            h2.dir = mrot.Transform(V)
            HankinLines.Add(h2)
        Next

        'Find the closest intersects of the Hankins
        Dim intPt As Point
        Dim dist As Double
        Dim mindist As Double
        Dim closestindex As Integer
        For I As Integer = 0 To HankinLines.Count - 1
            mindist = Double.MaxValue
            closestindex = -1
            For J As Integer = 0 To HankinLines.Count - 1
                If I <> J Then
                    intPt = Intersect(HankinLines(I), HankinLines(J))
                    If intPt.X > -10000 And intPt.Y > -10000 Then
                        dist = Distsquared(HankinLines(I).pt, intPt) + Distsquared(intPt, HankinLines(J).pt)
                        If dist < mindist Then
                            mindist = dist
                            closestindex = J
                            bestPt = intPt
                        End If
                    End If
                End If
            Next
            If closestindex >= 0 Then
                my_StarPoints.Add(HankinLines(I).pt)
                my_StarPoints.Add(bestPt)
                my_StarPoints.Add(HankinLines(closestindex).pt)
            End If
        Next
        MakeStarPolygon()
    End Sub

    Public Function Distsquared(pt1 As Point, pt2 As Point) As Double
        Return (pt1.X - pt2.X) ^ 2 + (pt1.Y - pt2.Y) ^ 2
    End Function

    Public Function Intersect(H1 As Hankin, H2 As Hankin) As Point
        Dim result As Point = New Point()
        Dim nom As Double = H2.dir.Y * H1.dir.X - H2.dir.X * H1.dir.Y
        Dim UA As Double = (H2.dir.X * (H1.pt.Y - H2.pt.Y) - H2.dir.Y * (H1.pt.X - H2.pt.X)) / nom
        Dim UB As Double = (H1.dir.X * (H1.pt.Y - H2.pt.Y) - H1.dir.Y * (H1.pt.X - H2.pt.X)) / nom
        If UA > 0 And UB > 0 Then
            result.X = H1.pt.X + UA * H1.dir.X
            result.Y = H1.pt.Y + UA * H1.dir.Y
        Else
            result.X = -10000
            result.Y = -10000
        End If
        Return result
    End Function

    Private Sub MakeStarPolygon()
        'Eliminate double points
        Dim pts As PointCollection = New PointCollection
        Dim isOK As Boolean
        For I As Integer = 0 To my_StarPoints.Count - 1
            isOK = True
            For J As Integer = 0 To pts.Count - 1
                If Distsquared(my_StarPoints(I), pts(J)) < 1 Then
                    isOK = False
                    Exit For
                End If
            Next
            If isOK Then pts.Add(my_StarPoints(I))
        Next
        my_StarPoints = pts
        'Find the center of the Star
        Dim centerX As Double = 0.0
        Dim centerY As Double = 0.0
        Dim center As Point
        Dim angles As List(Of Double) = New List(Of Double)
        For I As Integer = 0 To my_StarPoints.Count - 1
            centerX += my_StarPoints(I).X
            centerY += my_StarPoints(I).Y
        Next
        center = New Point(centerX / (my_StarPoints.Count), centerY / (my_StarPoints.Count))
        'Get the angle from each point towards the center
        Dim V As Vector
        For I As Integer = 0 To my_StarPoints.Count - 1
            V = my_StarPoints(I) - center
            angles.Add(Vector.AngleBetween(V, New Vector(1, 0)))
        Next
        'Sort the angles and the corresponding points in increasing order
        Dim dummyAngle As Double
        Dim dummyPoint As Point
        For I As Integer = 0 To my_StarPoints.Count - 1
            For J As Integer = I + 1 To my_StarPoints.Count - 1
                If angles(J) < angles(I) Then
                    dummyAngle = angles(I)
                    angles(I) = angles(J)
                    angles(J) = dummyAngle
                    dummyPoint = my_StarPoints(I)
                    my_StarPoints(I) = my_StarPoints(J)
                    my_StarPoints(J) = dummyPoint
                End If
            Next
        Next
        my_Star = New Polygon With
        {
            .Points = my_StarPoints
        }
    End Sub

    Public Sub Draw(c As Canvas)
        'Draw the polygon 
        my_Poly.Stroke = my_LineColor
        my_Poly.StrokeThickness = my_LineThickness
        my_Poly.Fill = my_FillColor
        c.Children.Add(my_Poly)
    End Sub

    Public Sub DrawStar(c As Canvas)
        'Draw the star
        If my_Star IsNot Nothing Then
            my_Star.Stroke = my_LineColor
            my_Star.StrokeThickness = my_LineThickness
            my_Star.Fill = my_StarColor
            c.Children.Add(my_Star)
        End If
    End Sub

End Class

Public Class Hankin
    Public pt As Point
    Public dir As Vector
End Class
