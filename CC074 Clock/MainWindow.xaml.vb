Imports System.Windows.Threading

Class MainWindow
    Private Rnd As Random = New Random()
    Private Hrptsnum As Integer = 100
    Private Minptsnum As Integer = 100
    Private Secptsnum As Integer = 80
    Private PrevHr As String
    Private PrevMin As String
    Private PrevSec As String
    Private Hrsym As Symbol
    Private Minsym As Symbol
    Private Secsym As Symbol
    Private HrAgents As List(Of Agent)
    Private MinAgents As List(Of Agent)
    Private SecAgents As List(Of Agent)
    Private w As Double
    Private h As Double
    Private midPt As Point
    Private HrPath As Path
    Private HrPG As PathGeometry
    Private Hrfigure As PathFigure
    Private HrSeg As ArcSegment
    Private MinPath As Path
    Private MinPG As PathGeometry
    Private Minfigure As PathFigure
    Private MinSeg As ArcSegment
    Private SecPath As Path
    Private SecPG As PathGeometry
    Private Secfigure As PathFigure
    Private SecSeg As ArcSegment


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Set the Canvas background
        Dim bmp As BitmapImage
        Dim imgbrush As ImageBrush
        bmp = New BitmapImage(New Uri(Environment.CurrentDirectory & "\Clockface.png"))
        imgbrush = New ImageBrush With {
            .Stretch = Stretch.Fill,
            .ImageSource = bmp
        }
        Canvas1.Background = imgbrush
        w = Canvas1.ActualWidth
        h = Canvas1.ActualHeight
        midPt = New Point(w / 2, h / 2)
        'Set the Hours Arc
        HrPath = New Path() With {
            .Stroke = Brushes.Green,
            .StrokeThickness = 8
        }
        HrPG = New PathGeometry()
        Hrfigure = New PathFigure() With {
            .StartPoint = New Point(w / 2, 5)
        }
        HrSeg = New ArcSegment() With {
            .SweepDirection = SweepDirection.Clockwise,
            .IsStroked = True
        }
        Hrfigure.Segments.Add(HrSeg)
        HrPG.Figures.Add(Hrfigure)
        HrPath.Data = HrPG
        Canvas1.Children.Add(HrPath)
        'Set the Minutes Arc
        MinPath = New Path() With {
            .Stroke = Brushes.Blue,
            .StrokeThickness = 8
        }
        MinPG = New PathGeometry()
        Minfigure = New PathFigure() With {
            .StartPoint = New Point(w / 2, 13)
        }
        MinSeg = New ArcSegment() With {
            .SweepDirection = SweepDirection.Clockwise,
            .IsStroked = True
        }
        Minfigure.Segments.Add(MinSeg)
        MinPG.Figures.Add(Minfigure)
        MinPath.Data = MinPG
        Canvas1.Children.Add(MinPath)
        'Set the Seconds Arc
        SecPath = New Path() With {
            .Stroke = Brushes.Red,
            .StrokeThickness = 8
        }
        SecPG = New PathGeometry()
        Secfigure = New PathFigure() With {
            .StartPoint = New Point(w / 2, 21)
        }
        SecSeg = New ArcSegment() With {
            .SweepDirection = SweepDirection.Clockwise,
            .IsStroked = True
        }
        Secfigure.Segments.Add(SecSeg)
        SecPG.Figures.Add(Secfigure)
        SecPath.Data = SecPG
        Canvas1.Children.Add(SecPath)
        'Set the digital ParticleClock
        PrevHr = ""
        PrevMin = ""
        PrevSec = ""
        Dim agt As Agent
        Hrsym = New Symbol("") With {
            .Fill = Brushes.LightGray,
            .Stroke = Brushes.LightGray,
            .StrokeThickness = 1.0,
            .FontFamily = New FontFamily("Arial"),
            .FontSize = 40,
            .FontStyle = FontStyles.Normal,
            .FontWeight = FontWeights.Bold,
            .Origin = New Point(90, 150)
        }
        HrAgents = New List(Of Agent)
        For I As Integer = 0 To Hrptsnum
            agt = New Agent(midPt, 1.0, 2.0, 2.0) With {
                .Size = 2,
                .Color = Brushes.Green,
                .Breakingdistance = 50
            }
            agt.Draw(Canvas1)
            HrAgents.Add(agt)
        Next
        Minsym = New Symbol("") With {
            .Fill = Brushes.LightGray,
            .Stroke = Brushes.LightGray,
            .StrokeThickness = 1.0,
            .FontFamily = New FontFamily("Arial"),
            .FontSize = 40,
            .FontStyle = FontStyles.Normal,
            .FontWeight = FontWeights.Bold,
            .Origin = New Point(170, 150)
        }
        MinAgents = New List(Of Agent)
        For I As Integer = 0 To Minptsnum
            agt = New Agent(midPt, 1.0, 3.0, 3.0) With {
                .Size = 2,
                .Color = Brushes.Blue,
                .Breakingdistance = 50
            }
            agt.Draw(Canvas1)
            MinAgents.Add(agt)
        Next
        Secsym = New Symbol("") With {
            .Fill = Brushes.LightGray,
            .Stroke = Brushes.LightGray,
            .StrokeThickness = 1.0,
            .FontFamily = New FontFamily("Arial"),
            .FontSize = 40,
            .FontStyle = FontStyles.Normal,
            .FontWeight = FontWeights.Bold,
            .Origin = New Point(250, 150)
        }
        SecAgents = New List(Of Agent)
        For I As Integer = 0 To Secptsnum
            agt = New Agent(midPt, 1.0, 4.0, 4.0) With {
                .Size = 2,
                .Color = Brushes.Red,
                .Breakingdistance = 20
            }
            agt.Draw(Canvas1)
            SecAgents.Add(agt)
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub SetTargets(Hour As String, Minute As String, Second As String)
        Dim pt As Point
        Dim tangent As Point
        Dim geo As PathGeometry
        Dim arcsize As Size
        Dim endpt As Point
        Dim hrs As Integer
        Dim mins As Integer
        Dim secs As Integer
        Dim angle As Double
        'Update the Clock
        If Hour <> PrevHr Then
            Hrsym.Text = Hour & ":"
            geo = Hrsym.Geometry.GetFlattenedPathGeometry
            For I As Integer = 0 To Hrptsnum
                geo.GetPointAtFractionLength(I / Hrptsnum, pt, tangent)
                HrAgents(I).Location = New Vector(90 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble())
                HrAgents(I).SetTarget(pt)
            Next
            hrs = Integer.Parse(Hour)
            angle = (hrs Mod 12) * Math.PI / 6 - Math.PI / 2
            arcsize = New Size(w / 2 - 5, h / 2 - 5)
            endpt = midPt + New Point(arcsize.Width * Math.Cos(angle), arcsize.Height * Math.Sin(angle))
            HrSeg.Point = endpt
            HrSeg.Size = arcsize
            HrSeg.RotationAngle = angle
            HrSeg.IsLargeArc = hrs > 6
            PrevHr = Hour
        End If
        If Minute <> PrevMin Then
            Minsym.Text = Minute & ":"
            geo = Minsym.Geometry.GetFlattenedPathGeometry
            For I As Integer = 0 To Minptsnum
                geo.GetPointAtFractionLength(I / Minptsnum, pt, tangent)
                MinAgents(I).Location = New Vector(170 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble())
                MinAgents(I).SetTarget(pt)
            Next
            mins = Integer.Parse(Minute)
            angle = mins * Math.PI / 30 - Math.PI / 2
            arcsize = New Size(w / 2 - 13, h / 2 - 13)
            endpt = midPt + New Point(arcsize.Width * Math.Cos(angle), arcsize.Height * Math.Sin(angle))
            MinSeg.Point = endpt
            MinSeg.Size = arcsize
            MinSeg.RotationAngle = angle
            MinSeg.IsLargeArc = mins > 30
            PrevMin = Minute
        End If
        If Second <> PrevSec Then
            Secsym.Text = Second
            geo = Secsym.Geometry.GetFlattenedPathGeometry
            For I As Integer = 0 To Secptsnum
                geo.GetPointAtFractionLength(I / Secptsnum, pt, tangent)
                SecAgents(I).Location = New Vector(250 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble())
                SecAgents(I).SetTarget(pt)
            Next
            secs = Integer.Parse(Second)
            angle = secs * Math.PI / 30 - Math.PI / 2
            arcsize = New Size(w / 2 - 21, h / 2 - 21)
            endpt = midPt + New Point(arcsize.Width * Math.Cos(angle), arcsize.Height * Math.Sin(angle))
            SecSeg.Point = endpt
            SecSeg.Size = arcsize
            SecSeg.RotationAngle = angle
            SecSeg.IsLargeArc = secs > 30
            PrevSec = Second
        End If
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        SetTargets(Now.Hour.ToString("00"), Now.Minute.ToString("00"), Now.Second.ToString("00"))
        For I As Integer = 0 To HrAgents.Count - 1
            HrAgents(I).Update()
        Next
        For I As Integer = 0 To MinAgents.Count - 1
            MinAgents(I).Update()
        Next
        For I As Integer = 0 To SecAgents.Count - 1
            SecAgents(I).Update()
        Next
    End Sub

End Class
