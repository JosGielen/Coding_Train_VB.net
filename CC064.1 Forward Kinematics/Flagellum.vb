Public Class Flagellum
    Private Segments As List(Of Segment)
    Private my_Base As Vector
    Private my_StartAngle As Double
    Private my_SegmentCount As Integer
    Private my_Segmentlength As Double
    Private Shared Rnd As Random = New Random()
    Private Xoff As Double = Rnd.Next(1000)

    Public Sub New(base As Vector, angle As Double, segmentcount As Integer, totallength As Double, color_ As Brush)
        Segments = New List(Of Segment)
        Dim s As Segment
        my_Base = base
        my_StartAngle = angle
        my_SegmentCount = segmentcount
        my_Segmentlength = totallength / segmentcount
        s = New Segment(base.X, base.Y, my_Segmentlength, my_StartAngle)
        s.LineColor = color_
        s.LineThickness = 7
        Segments.Add(s)
        For I As Integer = 1 To segmentcount - 1
            s = New Segment(base.X, base.Y, my_Segmentlength, 0)
            s.LineColor = color_
            s.LineThickness = 6 * (segmentcount - I) / (segmentcount - 1) + 1
            Segments.Add(s)
        Next
    End Sub

    Public Property Base As Vector
        Get
            Return my_Base
        End Get
        Set(value As Vector)
            my_Base = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        For I As Integer = 0 To Segments.Count - 1
            Segments(I).Show(c)
        Next
    End Sub

    Public Sub Update(Yoff As Double)
        Segments(0).Angle = my_StartAngle
        Segments(0).StartPt = my_Base

        For I As Integer = 1 To Segments.Count - 1
            Segments(I).Angle = 60 * PerlinNoise.Noise((Segments.Count - 1 - I) / 20 + Xoff) - 30
        Next
        Segments(0).Update()
        For I As Integer = 1 To Segments.Count - 1
            Segments(I).Update(Segments(I - 1))
        Next
        Xoff += 0.02
    End Sub

End Class
