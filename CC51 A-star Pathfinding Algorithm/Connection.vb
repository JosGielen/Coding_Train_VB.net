Imports A_star_Pathfinder

Public Class Connection
    Private my_Node1 As Node
    Private my_Node2 As Node
    Private my_Distance As Double

    Public Sub New(n1 As Node, n2 As Node)
        Node1 = n1
        Node2 = n2
        my_Distance = n1.Distance(n2)
    End Sub

    Public Sub New(n1 As Node, n2 As Node, distance As Double)
        Node1 = n1
        Node2 = n2
        my_Distance = distance
    End Sub

    Public Property Node1 As Node
        Get
            Return my_Node1
        End Get
        Set(value As Node)
            my_Node1 = value
        End Set
    End Property

    Public Property Node2 As Node
        Get
            Return my_Node2
        End Get
        Set(value As Node)
            my_Node2 = value
        End Set
    End Property

    Public Property Distance As Double
        Get
            Return my_Distance
        End Get
        Set(value As Double)
            my_Distance = value
        End Set
    End Property
End Class
