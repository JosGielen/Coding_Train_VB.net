Imports System.IO

Class MainWindow
    Private Rendering As Boolean = False
    Private SignalX As List(Of Double)
    Private SignalY As List(Of Double)
    Private EpicyclesX As List(Of Epicycle)
    Private EpicyclesY As List(Of Epicycle)
    Private CirclesX As List(Of Ellipse)
    Private CirclesY As List(Of Ellipse)
    Private RadiaX As List(Of Line)
    Private RadiaY As List(Of Line)
    Private dotX As Ellipse
    Private dotY As Ellipse
    Private HLine As Line
    Private VLine As Line
    Private PtX As Point
    Private PtY As Point
    Private newPt As Point
    Private p As Polyline
    Private Time As Double
    Private TimeStep As Double

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        SignalX = New List(Of Double)
        SignalY = New List(Of Double)
        CirclesX = New List(Of Ellipse)
        CirclesY = New List(Of Ellipse)
        RadiaX = New List(Of Line)
        RadiaY = New List(Of Line)
        'Read the Logo from the file into the Signal lists.
        Dim fields() As String
        Dim SkipCounter As Integer = 0
        Dim sr As StreamReader = New StreamReader(Environment.CurrentDirectory & "\CodingTrainLogo.txt")
        Do While Not sr.EndOfStream
            fields = sr.ReadLine().Split(";"c)
            SkipCounter += 1
            If SkipCounter = 10 Then   'Skip 9 out of 10 points
                SignalX.Add(Double.Parse(fields(0)))
                SignalY.Add(Double.Parse(fields(1)))
                SkipCounter = 0
            End If
        Loop
        'Calculate the Epicycle data from the Signals with the Discrete Fourier Transform
        EpicyclesX = DFT.GetSortedEpicycles(SignalX, False)
        EpicyclesY = DFT.GetSortedEpicycles(SignalY, False)
        'Create the Epicycle visualisation (Circles with a Radius line)
        Dim Ex As Ellipse
        Dim Lx As Line
        For I As Integer = 0 To EpicyclesX.Count - 1
            Ex = New Ellipse() With
            {
                .Stroke = New SolidColorBrush(Color.FromRgb(80, 80, 80)),
                .StrokeThickness = 1
            }
            CirclesX.Add(Ex)
            canvas1.Children.Add(Ex)
            Lx = New Line With
            {
                .Stroke = Brushes.Yellow,
                .StrokeThickness = 1
            }
            RadiaX.Add(Lx)
            canvas1.Children.Add(Lx)
        Next
        Dim Ey As Ellipse
        Dim Ly As Line
        For I As Integer = 0 To EpicyclesY.Count - 1
            Ey = New Ellipse() With
            {
                .Stroke = New SolidColorBrush(Color.FromRgb(80, 80, 80)),
                .StrokeThickness = 1
            }
            CirclesY.Add(Ey)
            canvas1.Children.Add(Ey)
            Ly = New Line With
            {
                .Stroke = Brushes.Yellow,
                .StrokeThickness = 1
            }
            RadiaY.Add(Ly)
            canvas1.Children.Add(Ly)
        Next
        'Create the endpoint indicator dots
        dotX = New Ellipse With
        {
            .Stroke = Brushes.DarkGray,
            .StrokeThickness = 1,
            .Fill = Brushes.Yellow,
            .Width = 8,
            .Height = 8
        }
        canvas1.Children.Add(dotX)
        dotY = New Ellipse With
        {
            .Stroke = Brushes.DarkGray,
            .StrokeThickness = 1,
            .Fill = Brushes.Yellow,
            .Width = 8,
            .Height = 8
        }
        canvas1.Children.Add(dotY)
        'Create the drawing lines
        HLine = New Line() With
        {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1
        }
        canvas1.Children.Add(HLine)
        VLine = New Line() With
        {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1
        }
        canvas1.Children.Add(VLine)
        'Create the resulting drawing
        p = New Polyline() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1
        }
        canvas1.Children.Add(p)
        TimeStep = 2 * Math.PI / EpicyclesX.Count
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        'Draw the Epicycles
        DrawEpicycles()
        'Draw a horizontal line from the EpicyclesY endpoint to the new polyline point
        HLine.X1 = PtY.X
        HLine.Y1 = PtY.Y
        HLine.X2 = newPt.X
        HLine.Y2 = newPt.Y
        'Draw a Vertical line from the EpicyclesX endpoint to the new polyline point
        VLine.X1 = PtX.X
        VLine.Y1 = PtX.Y
        VLine.X2 = newPt.X
        VLine.Y2 = newPt.Y
        'Add the new polyline point to the polyline
        p.Points.Add(newPt)
        Time += TimeStep
        If Time >= 2 * Math.PI Then 'Reset the drawing
            Time = 0
            p.Points.Clear()
        End If
    End Sub

    Private Sub DrawEpicycles()
        Dim X As Double
        Dim Y As Double
        Dim PreviousX As Double
        Dim PreviousY As Double
        Dim Freq As Double
        Dim Amp As Double
        Dim Phase As Double
        'Draw the EpicycleX
        X = 500
        Y = 75
        PreviousX = X
        PreviousY = Y
        For I As Integer = 0 To EpicyclesX.Count - 1
            Freq = EpicyclesX(I).Freqency
            Amp = EpicyclesX(I).Amplitude
            Phase = EpicyclesX(I).Phase
            X += Amp * Math.Cos(Freq * Time + Phase)
            Y += Amp * Math.Sin(Freq * Time + Phase)
            CirclesX(I).Width = 2 * Amp
            CirclesX(I).Height = 2 * Amp
            CirclesX(I).SetValue(Canvas.LeftProperty, PreviousX - Amp)
            CirclesX(I).SetValue(Canvas.TopProperty, PreviousY - Amp)
            RadiaX(I).X1 = PreviousX
            RadiaX(I).Y1 = PreviousY
            RadiaX(I).X2 = X
            RadiaX(I).Y2 = Y
            PreviousX = X
            PreviousY = Y
        Next
        'Final point of EpicyclesX.
        dotX.SetValue(Canvas.LeftProperty, X - 4)
        dotX.SetValue(Canvas.TopProperty, Y - 4)
        PtX = New Point(X, Y)

        'Draw the EpicyclesY
        X = 125
        Y = canvas1.ActualHeight / 2 + 50
        PreviousX = X
        PreviousY = Y
        For I As Integer = 0 To EpicyclesY.Count - 1
            Freq = EpicyclesY(I).Freqency
            Amp = EpicyclesY(I).Amplitude
            Phase = EpicyclesY(I).Phase
            X += Amp * Math.Cos(Freq * Time + Phase + Math.PI / 2)
            Y += Amp * Math.Sin(Freq * Time + Phase + Math.PI / 2)
            CirclesY(I).Width = 2 * Amp
            CirclesY(I).Height = 2 * Amp
            CirclesY(I).SetValue(Canvas.LeftProperty, PreviousX - Amp)
            CirclesY(I).SetValue(Canvas.TopProperty, PreviousY - Amp)
            RadiaY(I).X1 = PreviousX
            RadiaY(I).Y1 = PreviousY
            RadiaY(I).X2 = X
            RadiaY(I).Y2 = Y
            PreviousX = X
            PreviousY = Y
        Next
        'Final point of EpicyclesY.
        dotY.SetValue(Canvas.LeftProperty, X - 4)
        dotY.SetValue(Canvas.TopProperty, Y - 4)
        PtY = New Point(X, Y)
        'get the new polyline point as the intersection of both X and Y epicycles endpoints
        newPt.X = PtX.X
        newPt.Y = PtY.Y
    End Sub

    Private Sub StartRender()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "STOP"
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "START"
        Rendering = False
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub
End Class


