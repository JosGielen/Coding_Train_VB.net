Public Class Flagellum
    Private Segments As List(Of Segment)
    Private my_Base As Vector
    Private my_SegmentCount As Integer
    Private my_Segmentlength As Double

    Public Sub New(base As Vector, segmentcount As Integer, totallength As Double, color_ As Brush)
        Segments = New List(Of Segment)
        Dim s As Segment
        my_Base = base
        my_SegmentCount = segmentcount
        my_Segmentlength = totallength / segmentcount
        s = New Segment(base.X, base.Y, my_Segmentlength)
        s.LineColor = color_
        s.LineThickness = 7
        Segments.Add(s)
        For I As Integer = 0 To segmentcount - 1
            s = New Segment(base.X, base.Y, my_Segmentlength)
            s.LineColor = color_
            s.LineThickness = 5 * (segmentcount - I - 1) / (segmentcount - 1) + 2
            Segments.Add(s)
        Next
    End Sub

    Public Sub Show(c As Canvas)
        For I As Integer = 0 To Segments.Count - 1
            Segments(I).Show(c)
        Next
    End Sub

    Public Sub Follow(target As Vector)
        For I As Integer = Segments.Count - 1 To 0 Step -1
            Segments(I).Follow(target)
            target = Segments(I).StartPt
        Next
        Dim fixedPt As Vector = my_Base
        For I As Integer = 0 To Segments.Count - 1
            Segments(I).SetStart(fixedPt)
            fixedPt = Segments(I).EndPt
        Next
    End Sub

End Class
