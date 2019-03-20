Public Class LogoCommand
    Private my_command As String
    Private my_value As Integer
    Private my_Parameter As String

    Public Property Command As String
        Get
            Return my_command
        End Get
        Set(value As String)
            my_command = value
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return my_value
        End Get
        Set(value As Integer)
            my_value = value
        End Set
    End Property

    Public Property Parameter As String
        Get
            Return my_Parameter
        End Get
        Set(value As String)
            my_Parameter = value
        End Set
    End Property

End Class
