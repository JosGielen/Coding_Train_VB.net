Imports System.Threading
Imports System.Windows.Threading

Class MainWindow
    Private Delegate Sub WaitDelegate(time As Integer)
    Private l As Line
    Private spacing As Double = 15
    Private X As Double = 0.0
    Private Y As Double = 0.0
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Render()
    End Sub

    Public Sub Render()
        Do
            If Rnd.NextDouble() < 0.5 Then
                l = New Line() With
                {
                    .X1 = X,
                    .Y1 = Y,
                    .X2 = X + spacing,
                    .Y2 = Y + spacing,
                    .Stroke = Brushes.Black,
                    .StrokeThickness = 2
                }
            Else
                l = New Line() With
            {
                .X1 = X,
                .Y1 = Y + spacing,
                .X2 = X + spacing,
                .Y2 = Y,
                .Stroke = Brushes.Black,
                .StrokeThickness = 2
            }
            End If
            canvas1.Children.Add(l)
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 10)
            X = X + spacing
            If X > canvas1.ActualWidth Then
                X = 0
                Y = Y + spacing
                If Y > canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    Public Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Rnd.NextDouble() < 0.5 Then
            l = New Line() With
            {
                .X1 = X,
                .Y1 = Y,
                .X2 = X + spacing,
                .Y2 = Y + spacing,
                .Stroke = Brushes.Black,
                .StrokeThickness = 2
            }
        Else
            l = New Line() With
            {
                .X1 = X,
                .Y1 = Y + spacing,
                .X2 = X + spacing,
                .Y2 = Y,
                .Stroke = Brushes.Black,
                .StrokeThickness = 2
            }
        End If
        canvas1.Children.Add(l)
        X = X + spacing
        If X > canvas1.ActualWidth Then
            X = 0
            Y = Y + spacing
            If Y > canvas1.ActualHeight Then
                RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            End If
        End If
    End Sub

    Private Sub Wait(t As Integer)
        Thread.Sleep(t)
    End Sub
End Class
