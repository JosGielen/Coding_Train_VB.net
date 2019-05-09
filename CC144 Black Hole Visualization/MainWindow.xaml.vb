Imports System.Threading
Imports System.Windows.Threading

Class MainWindow
    Private Delegate Sub WaitDelegate(time As Integer)
    Private Rendering As Boolean = False
    Private M87 As BlackHole
    Private Photons As List(Of Photon)
    Private PhotonCount As Integer = 400


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim p As Photon
        Photons = New List(Of Photon)
        M87 = New BlackHole(canvas1.ActualWidth / 2 - 100, canvas1.ActualHeight / 2, 5000)
        M87.Show(canvas1)
        For I As Integer = 0 To PhotonCount - 1
            p = New Photon(canvas1.ActualWidth - 10, canvas1.ActualHeight / 2 + (I - 200), New Vector(-c, 0))
            Photons.Add(p)
            p.Show(canvas1)
        Next
        Render()
    End Sub


    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            BtnStart.Content = "STOP"
            Rendering = True
            Render()
        Else
            BtnStart.Content = "START"
            Rendering = False
        End If
    End Sub

    Private Sub Render()
        Dim liveCount As Integer = 0
        Do
            liveCount = 0
            If Not Rendering Then Exit Sub
            For I As Integer = 0 To Photons.Count - 1
                If Photons(I).Alive Then
                    liveCount += 1
                    M87.Pull(Photons(I))
                    Photons(I).Update(canvas1)
                End If
            Next
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 10)
        Loop While liveCount > 0
        'Show the shadow
        For I As Integer = 0 To Photons.Count - 1
            If Not Photons(I).Escaped Then
                For Each l As Line In Photons(I).Trajectory
                    l.Stroke = New SolidColorBrush(Color.FromArgb(150, 255, 0, 0))
                Next
                If Not Rendering Then Exit Sub
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 10)
            End If
        Next
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
