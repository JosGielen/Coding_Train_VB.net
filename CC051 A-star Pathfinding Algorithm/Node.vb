Imports A_star_Pathfinder

Public Class Node
    Private my_Row As Integer
    Private my_Col As Integer
    Private my_Location As Point
    Private my_F As Double
    Private my_G As Double
    Private my_Previous As Node

    Public Sub New(col As Integer, row As Integer, width As Double, height As Double)
        my_Row = row
        my_Col = col
        my_Location = New Point(col * width, row * height)
        my_F = Double.MaxValue
        my_G = Double.MaxValue
    End Sub

    Public ReadOnly Property Location As Point
        Get
            Return my_Location
        End Get
    End Property

    Public Property G As Double
        Get
            Return my_G
        End Get
        Set(value As Double)
            my_G = value
        End Set
    End Property

    Public Property F As Double
        Get
            Return my_F
        End Get
        Set(value As Double)
            my_F = value
        End Set
    End Property

    Public Property Previous As Node
        Get
            Return my_Previous
        End Get
        Set(value As Node)
            my_Previous = value
        End Set
    End Property

    Public ReadOnly Property Row As Integer
        Get
            Return my_Row
        End Get
    End Property

    Public ReadOnly Property Col As Integer
        Get
            Return my_Col
        End Get
    End Property

    Public Function Distance(otherNode As Node) As Double
        Return Math.Sqrt((my_Location.X - otherNode.Location.X) ^ 2 + (my_Location.Y - otherNode.Location.Y) ^ 2)
    End Function

End Class
