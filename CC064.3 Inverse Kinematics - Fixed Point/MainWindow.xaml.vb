Class MainWindow
    Private flags As List(Of Flagellum)
    Private SegmentCount As Integer = 30
    Private FlagellumLength As Double = 400
    Private FlagellaCount As Integer = 1
    Private MousePos As Vector

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        flags = New List(Of Flagellum)
        Dim fl As Flagellum
        Dim loc As Vector
        For I As Integer = 0 To FlagellaCount - 1
            loc = New Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight - 20)
            fl = New Flagellum(loc, SegmentCount, FlagellumLength, Brushes.Lime)
            fl.Show(canvas1)
            flags.Add(fl)
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To flags.Count - 1
            flags(I).Follow(MousePos)
        Next
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        MousePos = CType(e.GetPosition(canvas1), Vector)
    End Sub

End Class
