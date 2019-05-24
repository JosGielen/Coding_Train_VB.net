Public Class Flagellum
    Private Segments As List(Of Segment)
    Private My_Colors As List(Of Brush)

    Public Sub New(segmentcount As Integer, totallength As Double)
        Segments = New List(Of Segment)
        My_Colors = New List(Of Brush)
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        My_Colors = pal.GetColorBrushes(segmentcount)
        Dim Segmentlength = totallength / segmentcount
        Dim s As Segment
        For I As Integer = 0 To segmentcount - 1
            s = New Segment(Segmentlength) With
            {
                .LineColor = My_Colors(segmentcount - I - 1),
                .LineThickness = 6 * I / (segmentcount - 1) + 1
            }
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
    End Sub

End Class
