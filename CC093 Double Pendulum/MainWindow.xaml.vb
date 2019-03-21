Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Private Delegate Sub RenderDelegate()
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 40
    Private My_Mousedown As Boolean = False
    Private X0 As Double = 0.0
    Private Y0 As Double = 0.0
    Private X1 As Double = 0.0
    Private Y1 As Double = 0.0
    Private X2 As Double = 0.0
    Private Y2 As Double = 0.0
    Private A1 As Double = 0.0
    Private A2 As Double = 0.0
    Private V1 As Double = 0.0
    Private V2 As Double = 0.0
    Private Ac1 As Double = 0.0
    Private Ac2 As Double = 0.0
    Private L1 As Double = 0.0
    Private L2 As Double = 0.0
    Private M1 As Double = 0.0
    Private M2 As Double = 0.0
    Private g As Double = 0.0
    Private Ellipse1 As Ellipse
    Private Ellipse2 As Ellipse
    Private line1 As Line
    Private line2 As Line

    Private Sub Init()
        A1 = 0
        A2 = 0
        L1 = Canvas1.ActualWidth / 4
        L2 = Canvas1.ActualWidth / 5
        M1 = 20.0
        M2 = 20.0
        g = 1

        X0 = Canvas1.ActualWidth / 2
        Y0 = Canvas1.ActualHeight / 5
        X1 = X0 + L1 * Math.Sin(A1)
        Y1 = Y0 + L1 * Math.Cos(A1)
        X2 = X1 + L2 * Math.Sin(A2)
        Y2 = Y1 + L2 * Math.Cos(A2)
        line1 = New Line With {
            .X1 = X0,
            .Y1 = Y0,
            .X2 = X1,
            .Y2 = Y1,
            .Stroke = Brushes.Red,
            .StrokeThickness = 2}
        line2 = New Line With {
            .X1 = X1,
            .Y1 = Y1,
            .X2 = X2,
            .Y2 = Y2,
            .Stroke = Brushes.Red,
            .StrokeThickness = 2}
        Ellipse1 = New Ellipse With {
            .Fill = Brushes.Blue,
            .Width = M1,
            .Height = M1}
        Ellipse2 = New Ellipse With {
            .Fill = Brushes.Blue,
            .Width = M2,
            .Height = M2}
        Ellipse1.SetValue(Canvas.LeftProperty, X1 - M1 / 2)
        Ellipse1.SetValue(Canvas.TopProperty, Y1 - M1 / 2)
        Ellipse2.SetValue(Canvas.LeftProperty, X2 - M2 / 2)
        Ellipse2.SetValue(Canvas.TopProperty, Y2 - M2 / 2)
        Canvas1.Children.Add(line1)
        Canvas1.Children.Add(line2)
        Canvas1.Children.Add(Ellipse1)
        Canvas1.Children.Add(Ellipse2)
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub Draw()
        line1.X1 = X0
        line1.Y1 = Y0
        line1.X2 = X1
        line1.Y2 = Y1
        line2.X1 = X1
        line2.Y1 = Y1
        line2.X2 = X2
        line2.Y2 = Y2
        Ellipse1.SetValue(Canvas.LeftProperty, X1 - M1 / 2)
        Ellipse1.SetValue(Canvas.TopProperty, Y1 - M1 / 2)
        Ellipse2.SetValue(Canvas.LeftProperty, X2 - M2 / 2)
        Ellipse2.SetValue(Canvas.TopProperty, Y2 - M2 / 2)
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

    Private Sub Window_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Dim XR1 As Double = 0.0
        Dim XR2 As Double = 0.0
        Dim YR1 As Double = 0.0
        Dim YR2 As Double = 0.0
        My_Mousedown = True
        X2 = e.GetPosition(Canvas1).X
        Y2 = e.GetPosition(Canvas1).Y
        If Y2 = Y0 Then Exit Sub
        Dim A1 = (X0 - X2) / (Y2 - Y0)
        Dim B1 = (L1 * L1 - L2 * L2 + X2 * X2 - X0 * X0 + Y2 * Y2 - Y0 * Y0) / (2 * (Y2 - Y0))
        Dim A2 = A1 * A1 + 1
        Dim B2 = 2 * A1 * B1 - 2 * A1 * Y0 - 2 * X0
        Dim C2 = B1 * B1 - 2 * B1 * Y0 - L1 * L1 + X0 * X0 + Y0 * Y0
        If B2 * B2 - 4 * A2 * C2 > 0 Then
            XR1 = (-B2 + Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2)
            XR2 = (-B2 - Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2)
            YR1 = A1 * XR1 + B1
            YR2 = A1 * XR2 + B1
            'Motion continuity
            If (XR1 - X1) ^ 2 + (YR1 - Y1) ^ 2 < (XR2 - X1) ^ 2 + (YR2 - Y1) ^ 2 Then
                X1 = XR1
                Y1 = YR1
            Else
                X1 = XR2
                Y1 = YR2
            End If
            Draw()
            End If
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        Dim XR1 As Double = 0.0
        Dim XR2 As Double = 0.0
        Dim YR1 As Double = 0.0
        Dim YR2 As Double = 0.0
        If My_Mousedown Then
            X2 = e.GetPosition(Canvas1).X
            Y2 = e.GetPosition(Canvas1).Y
            If Y2 = Y0 Then Exit Sub
            Dim A1 = (X0 - X2) / (Y2 - Y0)
            Dim B1 = (L1 * L1 - L2 * L2 + X2 * X2 - X0 * X0 + Y2 * Y2 - Y0 * Y0) / (2 * (Y2 - Y0))
            Dim A2 = A1 * A1 + 1
            Dim B2 = 2 * A1 * B1 - 2 * A1 * Y0 - 2 * X0
            Dim C2 = B1 * B1 - 2 * B1 * Y0 - L1 * L1 + X0 * X0 + Y0 * Y0
            If B2 * B2 - 4 * A2 * C2 > 0 Then
                XR1 = (-B2 + Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2)
                XR2 = (-B2 - Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2)
                YR1 = A1 * XR1 + B1
                YR2 = A1 * XR2 + B1
                'Motion continuity
                If (XR1 - X1) ^ 2 + (YR1 - Y1) ^ 2 < (XR2 - X1) ^ 2 + (YR2 - Y1) ^ 2 Then
                    X1 = XR1
                    Y1 = YR1
                Else
                    X1 = XR2
                    Y1 = YR2
                End If
                Draw()
                End If
            End If
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        My_Mousedown = False
        If X1 > X0 Then
            A1 = Math.PI / 2 - Math.Atan((Y1 - Y0) / (X1 - X0))
        Else
            A1 = -Math.PI / 2 - Math.Atan((Y1 - Y0) / (X1 - X0))
        End If
        If X2 > X1 Then
            A2 = Math.PI / 2 - Math.Atan((Y2 - Y1) / (X2 - X1))
        Else
            A2 = -Math.PI / 2 - Math.Atan((Y2 - Y1) / (X2 - X1))
        End If
        Do While My_Mousedown = False
            Ac1 = (-g * (2 * M1 + M2) * Math.Sin(A1) - M2 * g * Math.Sin(A1 - 2 * A2) - 2 * Math.Sin(A1 - A2) * M2 * (V2 * V2 * L2 + V1 * V1 * L1 * Math.Cos(A1 - A2))) / (L1 * (2 * M1 + M2 - M2 * Math.Cos(2 * A1 - 2 * A2)))
            Ac2 = (2 * Math.Sin(A1 - A2) * (V1 * V1 * L1 * (M1 + M2) + g * (M1 + M2) * Math.Cos(A1) + V2 * V2 * L2 * M2 * Math.Cos(A1 - A2))) / (L2 * (2 * M1 + M2 - M2 * Math.Cos(2 * A1 - 2 * A2)))
            V1 = V1 + Ac1
            V2 = V2 + Ac2
            A1 = A1 + V1
            A2 = A2 + V2
            A1 = 0.9995 * A1
            A2 = 0.9995 * A2
            X1 = X0 + L1 * Math.Sin(A1)
            Y1 = Y0 + L1 * Math.Cos(A1)
            X2 = X1 + L2 * Math.Sin(A2)
            Y2 = Y1 + L2 * Math.Cos(A2)
            Draw()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        End
    End Sub
End Class
