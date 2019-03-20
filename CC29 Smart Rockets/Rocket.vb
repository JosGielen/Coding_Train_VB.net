Imports Smart_Rockets

Public Class Rocket
    Private my_ID As Integer
    Private my_DNA As DNA
    Private my_Life As Boolean = True
    Private my_MinDistance As Double = Double.MaxValue
    Private my_HitTarget As Boolean = False
    Private my_HitWall As Boolean = False
    Private my_Crashed As Boolean = False
    Private my_MinTime As Integer = 0
    Private my_Fitness As Double = 0
    Private my_Position As Point
    Private my_Speed As Double = 0.0
    Private my_StartDir As Double = 0.0
    Private my_SpeedX As Double = 0.0
    Private my_SpeedY As Double = 0.0
    Private My_ell As Ellipse
    Private my_Target As Point
    Private my_obstacles As List(Of Rect)

    Public Sub New(target As Point, obstacles As List(Of Rect), pos As Point)
        my_Target = target
        my_obstacles = New List(Of Rect)
        my_Position = pos
        For I As Integer = 0 To obstacles.Count - 1
            my_obstacles.Add(obstacles(I))
        Next
        My_ell = New Ellipse() With {
            .Width = 6,
            .Height = 6,
            .Fill = Brushes.White
        }
        My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3)
        My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3)
    End Sub

    Public Property DNA As DNA
        Get
            Return my_DNA
        End Get
        Set(value As DNA)
            my_DNA = value
        End Set
    End Property

    Public ReadOnly Property HitTarget As Boolean
        Get
            Return my_HitTarget
        End Get
    End Property

    Public ReadOnly Property Crashed As Boolean
        Get
            Return my_Crashed
        End Get
    End Property

    Public ReadOnly Property MinTime As Integer
        Get
            Return my_MinTime
        End Get
    End Property

    Public Property Fitness As Double
        Get
            Return my_Fitness
        End Get
        Set(value As Double)
            my_Fitness = value
        End Set
    End Property

    Public Property Position As Point
        Get
            Return my_Position
        End Get
        Set(value As Point)
            my_Position = value
            My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3)
            My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3)
        End Set
    End Property

    Public Property Speed As Double
        Get
            Return my_Speed
        End Get
        Set(value As Double)
            my_Speed = value
            my_SpeedX = my_Speed * Math.Cos(my_StartDir)
            my_SpeedY = my_Speed * Math.Sin(my_StartDir)
        End Set
    End Property

    Public Property StartDir As Double
        Get
            Return my_StartDir
        End Get
        Set(value As Double)
            my_StartDir = value
            my_SpeedX = my_Speed * Math.Cos(my_StartDir)
            my_SpeedY = my_Speed * Math.Sin(my_StartDir)
        End Set
    End Property

    Public Property drawing As Ellipse
        Get
            Return My_ell
        End Get
        Set(value As Ellipse)
            My_ell = value
        End Set
    End Property

    Public ReadOnly Property MinDistance As Double
        Get
            Return my_MinDistance
        End Get
    End Property

    Public Property HitWall As Boolean
        Get
            Return my_HitWall
        End Get
        Set(value As Boolean)
            my_HitWall = value
        End Set
    End Property

    Public Property ID As Integer
        Get
            Return my_ID
        End Get
        Set(value As Integer)
            my_ID = value
        End Set
    End Property

    Public ReadOnly Property alive As Boolean
        Get
            Return my_Life
        End Get
    End Property

    Public Sub Update(counter As Integer)
        Dim dist As Double
        If my_Crashed Or my_HitWall Or my_HitTarget Then
            my_Life = False
            Exit Sub
        End If
        If counter < my_DNA.Genes.Count Then
            my_SpeedX += my_DNA.Genes(counter).Size * Math.Cos(my_DNA.Genes(counter).Dir)
            my_SpeedY += my_DNA.Genes(counter).Size * Math.Sin(my_DNA.Genes(counter).Dir)
            my_Position.X += my_SpeedX
            my_Position.Y -= my_SpeedY
            dist = Math.Sqrt((my_Target.X - my_Position.X) ^ 2 + (my_Target.Y - my_Position.Y) ^ 2)
            If dist < my_MinDistance Then
                my_MinDistance = dist
                my_MinTime = counter
            End If
            If dist < 10 Then my_HitTarget = True
            For I As Integer = 0 To my_obstacles.Count - 1
                If my_Position.X > my_obstacles(I).X And my_Position.X < my_obstacles(I).X + my_obstacles(I).Width And my_Position.Y > my_obstacles(I).Y And my_Position.Y < my_obstacles(I).Y + my_obstacles(I).Height Then
                    my_Crashed = True
                End If
            Next
            My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3)
            My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3)
        End If
    End Sub

    Public Sub Reset()
        my_Life = True
        my_MinDistance = Double.MaxValue
        my_HitTarget = False
        my_HitWall = False
        my_Crashed = False
        my_MinTime = 0
        my_Fitness = 0
    End Sub

End Class
