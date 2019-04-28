Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 1
    Private Snowflake As List(Of Particle)
    Private deviation As Integer
    Private particleSize As Double
    Private App_Loaded As Boolean = False
    Private App_Started As Boolean = False
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Snowflake = New List(Of Particle)
        deviation = 2
        particleSize = 3
        sldDeviation.Value = deviation
        TxtDeviation.Text = deviation.ToString()
        sldSize.Value = particleSize
        TxtSize.Text = particleSize.ToString()
        App_Loaded = True
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not App_Started Then
            App_Started = True
            BtnStart.Content = "STOP"
            Snowflake.Clear()
            canvas1.Children.Clear()
            Render()
        Else
            App_Started = False
            BtnStart.Content = "START"
        End If
    End Sub

    Private Sub Render()
        Dim p As Particle
        Dim finished As Boolean
        Do While App_Started
            p = New Particle(particleSize, canvas1.ActualWidth / 2, deviation)
            Do
                finished = p.Update(Snowflake)
            Loop While Not finished
            If p.GetX() >= canvas1.ActualHeight / 2 - 20 Or p.GetX() >= canvas1.ActualWidth / 2 - 20 Then Exit Do 'Prevent growth outside the window
            p.Show(canvas1)
            Snowflake.Add(p)
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
        App_Started = False
        BtnStart.Content = "START"
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub SldDeviation_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If App_Loaded Then
            deviation = CInt(sldDeviation.Value)
            TxtDeviation.Text = sldDeviation.Value.ToString()
        End If
    End Sub

    Private Sub SldSize_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If App_Loaded Then
            particleSize = sldSize.Value
            TxtSize.Text = sldSize.Value.ToString()
        End If
    End Sub
End Class
