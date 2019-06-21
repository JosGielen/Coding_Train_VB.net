Class MainWindow
    Private Rendering As Boolean = False
    Private particles As List(Of Particle)
    Private Rnd As Random = New Random()

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            Init()
            AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            BtnStart.Content = "Stop"
            Rendering = True
        Else
            RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            BtnStart.Content = "Start"
            Rendering = False
        End If
    End Sub

    Private Sub Init()
        Dim p As Particle
        particles = New List(Of Particle)
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim p As Particle
        For I As Integer = 0 To 1
            p = New Particle(canvas1.ActualWidth / 2 + 10 * Rnd.NextDouble() - 5, canvas1.ActualHeight - 10, 0.2 * Rnd.NextDouble() - 0.1, 1.0, 6.0)
            particles.Add(p)
            p.Draw(canvas1)
        Next
        For I As Integer = particles.Count - 1 To 0 Step -1
            particles(I).Update(0.02 * Rnd.NextDouble() - 0.01)
            If particles(I).Alpha = 0 Then
                particles.RemoveAt(I)
                canvas1.Children.RemoveAt(I)
            End If
        Next
        Title = particles.Count.ToString() & " particles."
    End Sub

End Class
