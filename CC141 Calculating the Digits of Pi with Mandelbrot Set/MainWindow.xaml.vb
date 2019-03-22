Imports System.Windows.Threading
Imports System.Threading
Imports System.Numerics

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 1
    Private digits As Integer = 7
    Private c As BigDecimal
    Private eps As BigDecimal
    Private z As BigDecimal
    Private Iterations As BigInteger

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        BigDecimal.Precision = digits * digits + 1
        Image1.Source = New BitmapImage(New System.Uri(Environment.CurrentDirectory & "\Mandelbrot.jpg"))
        Dim Hundred As BigDecimal = New BigDecimal(1, 2)
        Dim exp As BigDecimal = digits - 1
        c = 0.25
        eps = 1.0 / BigDecimal.Pow(Hundred, exp)
        c = c + eps
        z = 0.0
        Iterations = 0
        Render()
    End Sub

    Private Sub Render()
        Do
            For I As Integer = 0 To 2567
                If z < 2 Then
                    z = z * z + c
                    Iterations += 1
                Else
                    Exit Do
                End If
            Next
            TxtIters.Text = Iterations.ToString()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
        TxtIters.Text = Iterations.ToString()
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub
End Class
