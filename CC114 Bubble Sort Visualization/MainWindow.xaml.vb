Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub SwapDelegate(a As Integer, b As Integer)
    Private WaitTime As Integer = 1
    Private App_Started As Boolean = False
    Private values() As Double
    Private lines As List(Of Line)
    Private myColors As List(Of Brush)
    Private Rnd As Random = New Random()
    Private LineWidth As Integer = 1
    Private StartTime As Date

    Private Sub Init()
        Dim l As Line
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        ReDim values(CInt(canvas1.ActualWidth / LineWidth))
        lines = New List(Of Line)
        myColors = pal.GetColorBrushes(canvas1.ActualHeight - 10)
        canvas1.Children.Clear()
        WaitTime = SldWaitTime.Value
        For I As Integer = 0 To values.Length - 1
            values(I) = (canvas1.ActualHeight - 20) * Rnd.NextDouble()
            l = New Line() With
            {
                .X1 = LineWidth * I,
                .Y1 = canvas1.ActualHeight,
                .X2 = LineWidth * I,
                .Y2 = canvas1.ActualHeight - values(I),
                .Stroke = myColors(CInt(values(I))),
                .StrokeThickness = LineWidth
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
            StartTime = Now
            BubbleSort()
        Else
            BtnStart.Content = "START"
            App_Started = False
        End If
    End Sub

    Private Sub BubbleSort()
        Dim last As Integer = values.Length - 1
        Dim temp As Double = 0.0
        Do While App_Started
            For I As Integer = 0 To last - 1
                If values(I) > values(I + 1) Then
                    Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New SwapDelegate(AddressOf SwapItems), I, I + 1)
                End If
            Next
            last -= 1
            If last < 2 Then
                TxtTime.Text = ((Now - StartTime).TotalMilliseconds / 1000).ToString("F2")
                BtnStart.Content = "START"
                App_Started = False
                Exit Do
            End If
        Loop
    End Sub

    Private Sub SwapItems(a As Integer, b As Integer)
        Dim temp As Double = values(a)
        values(a) = values(b)
        values(b) = temp
        lines(a).Y2 = canvas1.ActualHeight - values(a)
        lines(a).Stroke = myColors(CInt(values(a)))
        lines(b).Y2 = canvas1.ActualHeight - values(b)
        lines(b).Stroke = myColors(CInt(values(b)))
        Thread.Sleep(WaitTime)
    End Sub

    Private Sub SldWaitTime_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        WaitTime = SldWaitTime.Value
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
