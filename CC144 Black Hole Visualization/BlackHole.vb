Public Class BlackHole
    Private pos As Vector
    Private mass As Double
    Private sr As Double
    Private Hole As Ellipse
    Private Orbit As Ellipse
    Private Diskouter As Ellipse
    Private Diskinner As Ellipse

    Public Sub New(x As Double, y As Double, m As Double)
        pos = New Vector(x, y)
        mass = m
        sr = (2 * G * mass) / (c * c)
    End Sub

    Public Sub Pull(p As Photon)
        Dim force As Vector = pos - p.Pos
        Dim theta As Double = Vector.AngleBetween(force, New Vector(1, 0)) * Math.PI / 180
        Dim r As Double = force.Length()
        Dim fg = G * mass / (r * r)
        Dim deltaTheta As Double = -fg * (dt / c) * Math.Sin(p.Theta - theta)
        deltaTheta = deltaTheta / (Math.Abs(1.0 - 2.0 * G * mass / (r * c * c)))
        p.Theta += deltaTheta
        p.Velocity = c * New Vector(Math.Cos(p.Theta), -Math.Sin(p.Theta))
        If r < sr + 0.2 Then p.Dead()
    End Sub

    Public Sub Show(c As Canvas)
        'Draw the Accretion Disk
        Diskouter = New Ellipse() With
        {
            .Width = 8 * sr,
            .Height = 8 * sr,
            .Stroke = Brushes.Yellow,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Yellow
        }
        Diskouter.SetValue(Canvas.LeftProperty, pos.X - 4 * sr)
        Diskouter.SetValue(Canvas.TopProperty, pos.Y - 4 * sr)
        c.Children.Add(Diskouter)
        Diskinner = New Ellipse() With
        {
            .Width = 6 * sr,
            .Height = 6 * sr,
            .Stroke = Brushes.Black,
            .StrokeThickness = 0.0,
            .Fill = Brushes.Black
        }
        Diskinner.SetValue(Canvas.LeftProperty, pos.X - 3 * sr)
        Diskinner.SetValue(Canvas.TopProperty, pos.Y - 3 * sr)
        c.Children.Add(Diskinner)
        'Draw the Photon Orbit Radius
        Orbit = New Ellipse() With
        {
            .Width = 3 * sr,
            .Height = 3 * sr,
            .Stroke = Brushes.Orange,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Black
        }
        Orbit.SetValue(Canvas.LeftProperty, pos.X - 1.5 * sr)
        Orbit.SetValue(Canvas.TopProperty, pos.Y - 1.5 * sr)
        c.Children.Add(Orbit)
        'Draw the SchwartzShield Radius
        Hole = New Ellipse() With
        {
            .Width = 2 * sr,
            .Height = 2 * sr,
            .Stroke = Brushes.Purple,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Black
        }
        Hole.SetValue(Canvas.LeftProperty, pos.X - sr)
        Hole.SetValue(Canvas.TopProperty, pos.Y - sr)
        c.Children.Add(Hole)
        'Draw the Shadow outline at 2.6 * SchwartzShield Radius
        Dim sl As Line = New Line() With
        {
            .Stroke = Brushes.Blue,
            .StrokeThickness = 3.0,
            .X1 = 0.0,
            .Y1 = c.ActualHeight / 2 - 2.6 * sr,
            .X2 = c.ActualWidth,
            .Y2 = c.ActualHeight / 2 - 2.6 * sr
        }
        c.Children.Add(sl)
        sl = New Line() With
        {
            .Stroke = Brushes.Blue,
            .StrokeThickness = 3.0,
            .X1 = 0.0,
            .Y1 = c.ActualHeight / 2 + 2.6 * sr,
            .X2 = c.ActualWidth,
            .Y2 = c.ActualHeight / 2 + 2.6 * sr
        }
        c.Children.Add(sl)
    End Sub

End Class
