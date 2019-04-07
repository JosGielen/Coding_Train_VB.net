Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub SwapDelegate(a As Integer, b As Integer)
    Public Delegate Sub FinishDelegate() 'Used to detect when the multiThreaded version has finished
    Private WaitTime As Integer = 1
    Private App_Started As Boolean = False
    Private values() As Double
    Private lines As List(Of Line)
    Private myColors As List(Of Brush)
    Private Rnd As Random = New Random()
    Private LineWidth As Integer = 1
    Private Multithreaded As Boolean
    Private StartTime As Date
    Private Pivotcount As Integer  'Used to detect when the multiThreaded version has finished

    Private Sub Init()
        Dim l As Line
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        Pivotcount = 0
        ReDim values(CInt(canvas1.ActualWidth / LineWidth))
        lines = New List(Of Line)
        myColors = pal.GetColorBrushes(canvas1.ActualHeight - 10)
        canvas1.Children.Clear()
        WaitTime = SldWaitTime.Value
        Multithreaded = CbMulti.IsChecked.Value
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
            If Multithreaded Then
                Dim T = New Thread(Sub() QuickSort(0, values.Length - 1))
                T.Start()
            Else
                QuickSort(0, values.Length - 1)
                BtnStart.Content = "START"
                App_Started = False
            End If
        Else
            BtnStart.Content = "START"
            App_Started = False
        End If
    End Sub

    Private Sub Finish()
        TxtTime.Text = ((Now - StartTime).TotalMilliseconds / 1000).ToString("F2")
        BtnStart.Content = "START"
        App_Started = False
    End Sub

    Private Function QuickSort(first As Integer, last As Integer)
        If Not App_Started Or first >= last Then Return True
        Dim index As Integer = Partition(first, last)
        Pivotcount += 1
        If Multithreaded Then
            Dim T1 = New Thread(Sub() QuickSort(first, index - 1))
            Dim T2 = New Thread(Sub() QuickSort(index + 1, last))
            T1.Start()
            T2.Start()
            T1.Join()
            T2.Join()
        Else
            QuickSort(first, index - 1)
            QuickSort(index + 1, last)
        End If

        Pivotcount -= 1
        If Pivotcount = 0 Then 'Finished
            Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New FinishDelegate(AddressOf Finish))
        End If
        Return True
    End Function

    Private Function Partition(first, last) As Integer
        Dim pivotValue As Double = values(last)
        Dim pivotIndex As Integer = first
        For I As Integer = first To last - 1
            If values(I) < pivotValue Then
                Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New SwapDelegate(AddressOf SwapItems), I, pivotIndex)
                pivotIndex += 1
            End If
        Next
        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New SwapDelegate(AddressOf SwapItems), pivotIndex, last)
        Return pivotIndex
    End Function

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

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub SldWaitTime_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Started Then Exit Sub
        WaitTime = SldWaitTime.Value
    End Sub
End Class


