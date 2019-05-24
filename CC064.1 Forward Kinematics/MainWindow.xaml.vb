Class MainWindow
    Private flags As List(Of Flagellum)
    Private SegmentCount As Integer = 20
    Private FlagellumLength As Double = 400
    Private FlagellaCount As Integer = 1
    Private YOff As Double = 0.0

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        flags = New List(Of Flagellum)
        Dim fl As Flagellum
        Dim loc As Vector
        For I As Integer = 0 To FlagellaCount - 1
            loc = New Vector((I + 1) / (FlagellaCount + 1) * canvas1.ActualWidth, canvas1.ActualHeight - 20)
            fl = New Flagellum(loc, -90, SegmentCount, FlagellumLength, Brushes.Lime)
            fl.Show(canvas1)
            flags.Add(fl)
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To flags.Count - 1
            flags(I).Update(YOff)
        Next
        YOff += 0.01
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
