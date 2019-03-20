Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private AppStarted As Boolean = False
    Private WaitTime As Integer = 100
    Private Volley As Integer = 100
    Private LifeSpan As Integer = 100
    Private Counter As Integer = 0
    Private rnd As Random = New Random()
    Private Speed As Double = 2.0
    Private AllowMutation As Boolean = True
    Private RandomGenePick As Boolean = True
    Private Rockets As List(Of Rocket)
    Private MatingPool As List(Of Rocket)
    Private Target As Point
    Private Obstacles As List(Of Rect)


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Obstacles = New List(Of Rect)
        Target = New Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight / 7)
        Dim r As Rocket
        Dim obst As Rectangle
        'Make a Target
        Dim TargetEllipse As Ellipse = New Ellipse() With {
            .Width = 30,
            .Height = 30,
            .Stroke = Brushes.Yellow,
            .StrokeThickness = 5,
            .Fill = Brushes.Red}
        TargetEllipse.SetValue(Canvas.LeftProperty, Target.X - 15)
        TargetEllipse.SetValue(Canvas.TopProperty, Target.Y - 15)
        'Make just 1 obstacle
        Dim rect As Rect = New Rect() With {
            .X = Canvas1.ActualWidth / 3,
            .Y = Canvas1.ActualHeight / 2.5,
            .Width = Canvas1.ActualWidth / 3,
            .Height = 30}
        Obstacles.Add(rect)
        'Make the rockets
        Rockets = New List(Of Rocket)
        For I As Integer = 0 To Volley
            r = New Rocket(Target, Obstacles, New Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight - 100))
            r.DNA = New DNA(LifeSpan, rnd)
            r.ID = I
            r.Speed = Speed
            r.StartDir = Math.PI * rnd.NextDouble()
            Rockets.Add(r)
        Next
        'Draw the target and obstacles
        Canvas1.Children.Add(TargetEllipse)
        For I As Integer = 0 To Obstacles.Count - 1
            obst = New Rectangle() With {
                .Width = Obstacles(I).Width,
                .Height = Obstacles(I).Height,
                .Stroke = Brushes.Yellow,
                .StrokeThickness = 1,
                .Fill = Brushes.Yellow}
            obst.SetValue(Canvas.LeftProperty, Obstacles(I).X)
            obst.SetValue(Canvas.TopProperty, Obstacles(I).Y)
            Canvas1.Children.Add(obst)
        Next
        'Draw the rockets
        For I As Integer = 0 To Volley
            Canvas1.Children.Add(Rockets(I).drawing)
        Next
        AppStarted = False
    End Sub

    Private Sub NextGeneration()
        Dim mateIndex1 As Integer = 0
        Dim mateIndex2 As Integer = 0
        Dim Mate1 As Rocket
        Dim Mate2 As Rocket
        Dim MaxFitness As Double = 0.0
        'Determine the fitness of the Rockets 
        For I As Integer = 0 To Volley
            If Rockets(I).MinDistance > 0 Then Rockets(I).Fitness = (10 / Rockets(I).MinDistance) ^ 2
        Next
        'Determine the maxfitness of all the Rockets 
        For I As Integer = 0 To Volley
            If Rockets(I).Fitness > MaxFitness Then MaxFitness = Rockets(I).Fitness
        Next
        'Normalise the fitness of all Rockets with '0.05 for crash, 10 for hit target
        For I As Integer = 0 To Volley
            Rockets(I).Fitness = Rockets(I).Fitness / MaxFitness
            If Rockets(I).Crashed Then Rockets(I).Fitness = 0.05
            If Rockets(I).HitTarget Then Rockets(I).Fitness = 70 * (Rockets(I).Fitness + 5 * (LifeSpan - Rockets(I).MinTime))
        Next
        'Copy Rocket DNA into the matingpool number of copies = normalized fitness score of the Rocket
        'Rockets with higher fitness have more chance to become a Mate
        MatingPool = New List(Of Rocket)
        For I As Integer = 0 To Volley
            For j As Integer = 1 To CInt(100 * Rockets(I).Fitness)
                MatingPool.Add(Rockets(I))
            Next
        Next
        'Reset the Rockets and assign them new DNA made from DNA of 2 rockets random taken out the matingpool
        For I As Integer = 0 To Volley
            mateIndex1 = rnd.Next(MatingPool.Count)
            mateIndex2 = rnd.Next(MatingPool.Count)
            Mate1 = MatingPool(mateIndex1)
            Mate2 = MatingPool(mateIndex2)
            Rockets(I).Reset()
            Rockets(I).Position = New Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight - 100)
            Rockets(I).StartDir = Mate1.StartDir
            Rockets(I).DNA = New DNA(Mate1.DNA, Mate2.DNA, RandomGenePick, AllowMutation, LifeSpan, rnd)
        Next
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        'START
        AppStarted = True
        Dim lifecounter As Integer = 0
        Do While AppStarted
            lifecounter = 0
            For I As Integer = 0 To Volley
                Rockets(I).Update(Counter)
                If Rockets(I).Position.X < 2 Or Rockets(I).Position.X > Canvas1.ActualWidth - 2 Or Rockets(I).Position.Y < 2 Or Rockets(I).Position.Y > Canvas1.ActualHeight - 2 Then Rockets(I).HitWall = True
                If Rockets(I).alive Then lifecounter += 1
            Next
            If lifecounter = 0 Then Counter = LifeSpan
            Counter += 1
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            If Counter > LifeSpan Then
                'Count the number of hits
                Dim hitcounter As Integer = 0
                For I As Integer = 0 To Volley
                    If Rockets(I).HitTarget Then hitcounter += 1
                Next
                Me.Title = hitcounter.ToString() & " Hits!"
                'Create the next generation
                NextGeneration()
                Counter = 0
            End If
        Loop
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        AppStarted = False
        End
    End Sub
End Class
