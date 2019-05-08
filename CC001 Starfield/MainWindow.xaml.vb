Class MainWindow
    Private Stars As List(Of Star)
    Private StarCount As Integer = 800
    Private speed As Double
    Private rnd As Random = New Random()
    Private Rendering As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim s As Star
        Stars = New List(Of Star)
        speed = SldSpeed.Value
        TxtSpeed.Text = speed.ToString()
        For I As Integer = 0 To StarCount - 1
            s = New Star(canvas1.ActualWidth * rnd.NextDouble(), canvas1.ActualHeight * rnd.NextDouble(), 3 * rnd.NextDouble())
            s.Show(canvas1)
            Stars.Add(s)
        Next
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub

        Private Sub StartRender()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "FULL STOP"
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "ENGAGE"
        Rendering = False
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To Stars.Count - 1
            Stars(I).Update(speed)
        Next
    End Sub

    Private Sub SldSpeed_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not IsLoaded Then Exit Sub
        speed = SldSpeed.Value
        TxtSpeed.Text = speed.ToString()
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
