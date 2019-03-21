Public Class QuadTree

    Private my_Boundary As Rect
    Private my_Capacity As Integer
    Private my_Points As List(Of Point)
    Private divided As Boolean
    Private parts(3) As QuadTree

    Public Sub New(boundary As Rect, capacity As Integer)
        my_Boundary = boundary
        my_Capacity = capacity
        my_Points = New List(Of Point)
        divided = False
        parts(0) = Nothing
        parts(1) = Nothing
        parts(2) = Nothing
        parts(3) = Nothing
    End Sub

    Public Function Insert(pt As Point) As Boolean
        Dim result As Boolean = False
        If pt.X < my_Boundary.Left Or pt.X > my_Boundary.Right Or pt.Y < my_Boundary.Top Or pt.Y > my_Boundary.Bottom Then
            Return False
        End If
        If my_Points.Count < my_Capacity Then
            my_Points.Add(pt)
            result = True
        Else
            If Not divided Then
                parts(0) = CreateSubTree(0)
                parts(1) = CreateSubTree(1)
                parts(2) = CreateSubTree(2)
                parts(3) = CreateSubTree(3)
                divided = True
            End If
            result = parts(0).Insert(pt)
            If Not result Then result = parts(1).Insert(pt)
            If Not result Then result = parts(2).Insert(pt)
            If Not result Then result = parts(3).Insert(pt)
        End If
        Return result
    End Function

    Public Function Query(area As Rect) As List(Of Point)
        Dim result As List(Of Point) = New List(Of Point)
        If Not my_Boundary.IntersectsWith(area) Then
            Return result
        Else
            For Each pt As Point In my_Points
                If area.Contains(pt) Then
                    result.Add(pt)
                End If
            Next
            If divided Then
                result.AddRange(parts(0).Query(area))
                result.AddRange(parts(1).Query(area))
                result.AddRange(parts(2).Query(area))
                result.AddRange(parts(3).Query(area))
            End If
        End If
        Return result
    End Function

    Public Function Query(center As Point, distance As Double) As List(Of Point)
        Dim result As List(Of Point) = New List(Of Point)
        If Not my_Boundary.IntersectsWith(New Rect(center.X - distance, center.Y - distance, 2 * distance, 2 * distance)) Then
            Return result
        Else
            For Each pt As Point In my_Points
                If (pt.X - center.X) ^ 2 + (pt.Y - center.Y) ^ 2 < distance * distance Then
                    result.Add(pt)
                End If
            Next
            If divided Then
                result.AddRange(parts(0).Query(center, distance))
                result.AddRange(parts(1).Query(center, distance))
                result.AddRange(parts(2).Query(center, distance))
                result.AddRange(parts(3).Query(center, distance))
            End If
        End If
        Return result
    End Function

    Public Sub Clear()
        my_Points.Clear()
        divided = False
        parts(0) = Nothing
        parts(1) = Nothing
        parts(2) = Nothing
        parts(3) = Nothing
    End Sub

    Public Sub Draw(c As Canvas)
        Dim r As Rectangle = New Rectangle() With
        {
        .Width = my_Boundary.Width,
        .Height = my_Boundary.Height,
        .Stroke = Brushes.Black,
        .StrokeThickness = 1
        }
        r.SetValue(Canvas.TopProperty, my_Boundary.Top)
        r.SetValue(Canvas.LeftProperty, my_Boundary.Left)
        c.Children.Add(r)
        Dim El As Ellipse
        For Each pt As Point In my_Points
            El = New Ellipse() With
            {
                .Width = 4.0,
                .Height = 4.0,
                .Stroke = Brushes.Black,
                .StrokeThickness = 1
            }
            El.SetValue(Canvas.TopProperty, pt.Y - 2.0)
            El.SetValue(Canvas.LeftProperty, pt.X - 2.0)
            c.Children.Add(El)
        Next
        If divided Then
            parts(0).Draw(c)
            parts(1).Draw(c)
            parts(2).Draw(c)
            parts(3).Draw(c)
        End If
    End Sub

    Private Function CreateSubTree(nr As Integer) As QuadTree
        Dim result As QuadTree
        Dim top As Double
        Dim left As Double
        Dim width As Double = my_Boundary.Width / 2
        Dim height As Double = my_Boundary.Height / 2
        Select Case nr
            Case 0 'Top Left
                top = my_Boundary.Top
                left = my_Boundary.Left
            Case 1 'Top Right
                top = my_Boundary.Top
                left = my_Boundary.Left + width
            Case 2 'Bottom Left
                top = my_Boundary.Top + height
                left = my_Boundary.Left
            Case 3 'Bottom Right
                top = my_Boundary.Top + height
                left = my_Boundary.Left + width
        End Select
        result = New QuadTree(New Rect(left, top, width, height), my_Capacity)
        Return result
    End Function

End Class
