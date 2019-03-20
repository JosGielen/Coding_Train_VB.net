Class MainWindow
    Private Rendering As Boolean
    Private sym As Symbol
    Private Agents As List(Of Agent)
    Private Rnd As Random = New Random()
    Private num As Integer = 500


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim agt As Agent
        sym = New Symbol("") With {
            .Fill = Brushes.LightGray,
            .Stroke = Brushes.LightGray,
            .StrokeThickness = 1.0,
            .FontFamily = New FontFamily("Arial"),
            .FontSize = 100,
            .FontStyle = FontStyles.Normal,
            .FontWeight = FontWeights.Bold,
            .Origin = New Point(100, 200)
        }

        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        Dim my_brushes As List(Of Brush) = pal.GetColorBrushes(256)

        Agents = New List(Of Agent)
        For I As Integer = 0 To num
            agt = New Agent(New Point(Canvas1.ActualWidth * Rnd.NextDouble(), Canvas1.ActualHeight * Rnd.NextDouble()), 1.0, 3.0, 4.0, my_brushes(I Mod 256))
            agt.Size = 4
            agt.Breakingdistance = 100
            agt.Draw(Canvas1)
            Agents.Add(agt)
        Next
        SetTargets()
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As RoutedEventArgs)
        SetTargets()
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub

    Private Sub StartRender()
        'Some initial code
        For I As Integer = 0 To Agents.Count - 1
            Agents(I).Location = New Point(Canvas1.ActualWidth * Rnd.NextDouble(), Canvas1.ActualHeight * Rnd.NextDouble())
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "Stop"
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "Start"
        Rendering = False
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        FleeFromMouse()
        For I As Integer = 0 To Agents.Count - 1
            Agents(I).Update()
        Next
    End Sub

    Private Sub FleeFromMouse()
        Dim mouseloc As Vector
        If Canvas1.IsMouseOver Then
            mouseloc = Mouse.GetPosition(Canvas1)
            For I As Integer = 0 To Agents.Count - 1
                Agents(I).ApplyForce(Agents(I).Location - mouseloc)
            Next
        End If
    End Sub

    Private Sub SetTargets()
        Dim pt As Point
        Dim tangent As Point
        Dim geo As PathGeometry
        sym.Text = TxtInput.Text
        geo = sym.Geometry.GetFlattenedPathGeometry
        For I As Integer = 0 To num
            geo.GetPointAtFractionLength(I / num, pt, tangent)
            Agents(I).SetTarget(pt)
        Next
    End Sub

End Class
