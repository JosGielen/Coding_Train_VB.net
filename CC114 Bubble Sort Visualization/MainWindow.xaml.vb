Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 1
    Private App_Started As Boolean = False
    Private values() As Double
    Private lines As List(Of Line)
    Private myColors As List(Of Brush)
    Private Rnd As Random = New Random()

    Private Sub Init()
        Dim l As Line
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        ReDim values(CInt(canvas1.ActualWidth / 2))
        lines = New List(Of Line)
        myColors = pal.GetColorBrushes(canvas1.ActualHeight - 10)
        canvas1.Children.Clear()
        WaitTime = SldWaitTime.Value
        For I As Integer = 0 To values.Length - 1
            values(I) = (canvas1.ActualHeight - 20) * Rnd.NextDouble()
            l = New Line() With
            {
                .X1 = 2 * I,
                .Y1 = canvas1.ActualHeight,
                .X2 = 2 * I,
                .Y2 = canvas1.ActualHeight - values(I),
                .Stroke = myColors(CInt(values(I))),
                .StrokeThickness = 2.0
            }
            lines.Add(l)
            canvas1.Children.Add(l)
        Next
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not App_Started Then
            BtnStart.Content = "STOP"
            App_Started = True
            Init()
            Render()
        Else
            BtnStart.Content = "START"
            App_Started = False
        End If
    End Sub

    Private Sub Render()
        Dim last As Integer = values.Length - 1
        Dim temp As Double = 0.0
        Do While App_Started
            For I As Integer = 0 To last - 1
                If values(I) > values(I + 1) Then
                    temp = values(I)
                    values(I) = values(I + 1)
                    values(I + 1) = temp
                    lines(I).Y2 = canvas1.ActualHeight - values(I)
                    lines(I).Stroke = myColors(CInt(values(I)))
                    lines(I + 1).Y2 = canvas1.ActualHeight - values(I + 1)
                    lines(I + 1).Stroke = myColors(CInt(values(I + 1)))
                    Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New WaitDelegate(AddressOf Wait), WaitTime)
                End If
            Next
            last -= 1
            If last < 2 Then
                BtnStart.Content = "START"
                App_Started = False
                Exit Do
            End If
        Loop
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub SldWaitTime_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Started Then Exit Sub
        WaitTime = SldWaitTime.Value
    End Sub
End Class
