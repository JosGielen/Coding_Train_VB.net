Imports System.Windows.Threading
Imports System.Threading
Imports System.IO

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private parser As LogoParser
    Private turtle As Turtle
    Private App_Started As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        parser = New LogoParser()
        turtle = New Turtle(New Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight / 2), Canvas1)
        App_Started = False
        Using sr As New StreamReader(Environment.CurrentDirectory & "\CodingTrain Logo.txt") '"\Circles.text"
            TxtInput.Text = sr.ReadToEnd()
        End Using
    End Sub

    Private Sub TxtInput_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TxtInput.TextChanged
        parser.Input = TxtInput.Text
        parser.Parse()
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not App_Started Then
            App_Started = True
            BtnStart.Content = "STOP"
            Canvas1.Children.Clear()
            turtle.Reset()
            For Each cmd As LogoCommand In parser.Commands
                turtle.ExecuteCmd(cmd)
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
                If Not App_Started Then Exit For
            Next
        Else
            App_Started = False
            BtnStart.Content = "START"
        End If
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
