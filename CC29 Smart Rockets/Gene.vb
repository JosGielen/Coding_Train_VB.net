Public Class Gene
    Private my_Dir As Double
    Private my_Size As Double
    Shared rnd As Random = New Random

    Public Sub New()
        my_Dir = 2 * Math.PI * rnd.NextDouble()
        my_Size = 4 * rnd.NextDouble()
    End Sub

    Public Property Dir As Double
        Get
            Return my_Dir
        End Get
        Set(value As Double)
            my_Dir = value
        End Set
    End Property

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property
End Class
