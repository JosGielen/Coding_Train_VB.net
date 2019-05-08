Class MainWindow
    Private Rnd As Random = New Random
    Private inputNr As Integer = 2
    Private hiddenNr As Integer = 3
    Private outputNr As Integer = 1
    Private lr As Double = 0.1
    Private Normdist As Boolean = False
    Private res As Integer = 100
    Private nn As NeuralNet
    Private trainData(3) As TrainingData


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        trainData(0) = New TrainingData(0.01, 0.01, 0.01)
        trainData(1) = New TrainingData(0.99, 0.01, 0.99)
        trainData(2) = New TrainingData(0.01, 0.99, 0.99)
        trainData(3) = New TrainingData(0.99, 0.99, 0.01)
        Reset()
        Train()
        Draw()
    End Sub

    Private Sub Reset()
        'Make a new NeuralNet
        nn = New NeuralNet(inputNr, hiddenNr, outputNr, lr, Normdist)
    End Sub

    Private Sub Train()
        Dim index As Integer = 0
        'Train the Neural net with random TrainingData
        For I As Integer = 0 To 200000
            index = Rnd.Next(4)
            nn.Train(trainData(index).data, trainData(index).targets)
        Next
    End Sub

    Private Sub Draw()
        Dim index As Integer = 0
        Dim rect As Rectangle
        Dim grey As Byte = 0
        Dim inp(inputNr - 1) As Double
        Dim out(outputNr - 1) As Double
        Canvas1.Children.Clear()
        For I As Integer = 0 To res - 1
            For J As Integer = 0 To res - 1
                inp(0) = I / (res - 1)
                inp(1) = J / (res - 1)
                out = nn.Query(inp)
                grey = CByte(255 * out(0))
                rect = New Rectangle() With {
                    .Width = Canvas1.ActualWidth / res + 1,
                    .Height = Canvas1.ActualHeight / res + 1,
                    .Fill = New SolidColorBrush(Color.FromRgb(grey, grey, grey))
                }
                rect.SetValue(Canvas.LeftProperty, I * Canvas1.ActualWidth / res)
                rect.SetValue(Canvas.TopProperty, J * Canvas1.ActualHeight / res)
                Canvas1.Children.Add(rect)
            Next
        Next
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Train()
        Draw()
    End Sub

    Private Sub Window_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Reset()
        Train()
        Draw()
    End Sub
End Class
