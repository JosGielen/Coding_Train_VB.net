Public Class Ball
    Private my_X As Double
    Private my_Y As Double
    Private my_DirX As Double
    Private my_DirY As Double
    Private my_Size As Double

    Public Sub New(size As Double, Location As Point, dirX As Double, dirY As Double)
        my_Size = size
        my_X = Location.X
        my_Y = Location.Y
        my_DirX = dirX
        my_DirY = dirY
    End Sub

    Public Property X As Double
        Get
            Return my_X
        End Get
        Set(value As Double)
            my_X = value
        End Set
    End Property

    Public Property Y As Double
        Get
            Return my_Y
        End Get
        Set(value As Double)
            my_Y = value
        End Set
    End Property

    Public Property DirX As Double
        Get
            Return my_DirX
        End Get
        Set(value As Double)
            my_DirX = value
        End Set
    End Property

    Public Property DirY As Double
        Get
            Return my_DirY
        End Get
        Set(value As Double)
            my_DirY = value
        End Set
    End Property

    Public Sub Update(width As Integer, height As Integer)
        X += DirX
        Y += DirY
        If X < 1.5 * my_Size Or X > width - 1.5 * my_Size Then DirX = -DirX
        If Y < 1.5 * my_Size Or Y > height - 1.5 * my_Size Then DirY = -DirY
    End Sub


End Class
