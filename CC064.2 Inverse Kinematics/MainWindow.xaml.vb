Class MainWindow
    Private fl As Flagellum
    Private SegmentCount As Integer = 30
    Private FlagellumLength As Double = 400
    Private MousePos As Vector

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        fl = New Flagellum(SegmentCount, FlagellumLength)
        fl.Show(canvas1)
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        MousePos = CType(e.GetPosition(canvas1), Vector)
        fl.Follow(MousePos)
    End Sub

End Class
