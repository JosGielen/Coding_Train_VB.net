Class MainWindow
    Private Rain As List(Of Drop)
    Private DropCount As Integer = 500
    Private rnd As Random = New Random()
    Private Rendering As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim d As Drop
        Rain = New List(Of Drop)
        For I As Integer = 0 To DropCount - 1
            d = New Drop(canvas1.ActualWidth * rnd.NextDouble(), canvas1.ActualHeight * rnd.NextDouble())
            d.Show(canvas1)
            Rain.Add(d)
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To Rain.Count - 1
            Rain(I).Update()
        Next
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
