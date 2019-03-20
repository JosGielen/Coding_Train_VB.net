Public Class Turtle
    Private my_StartX As Double
    Private my_StartY As Double
    Private my_X As Double
    Private my_Y As Double
    Private my_Angle As Double
    Private my_Color As Brush
    Private my_Size As Double
    Private my_Drawing As Boolean
    Private my_Canvas As Canvas

    Public Sub New(Startlocation As Point, canvas As Canvas)
        my_StartX = Startlocation.X
        my_StartY = Startlocation.Y
        my_X = Startlocation.X
        my_Y = Startlocation.Y
        my_Drawing = True
        my_Canvas = canvas
        my_Color = Brushes.Black
        my_Size = 1.0
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

    Public Property Color As Brush
        Get
            Return my_Color
        End Get
        Set(value As Brush)
            my_Color = value
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

    Public Property Angle As Double
        Get
            Return my_Angle
        End Get
        Set(value As Double)
            my_Angle = value
        End Set
    End Property

    Public Sub Reset()
        my_X = my_StartX
        my_Y = my_StartY
        my_Drawing = True
        my_Color = Brushes.Black
        my_Size = 1.0
        my_Angle = 0.0
    End Sub

    Public Sub ExecuteCmd(cmd As LogoCommand)
        Dim endX As Double
        Dim endY As Double
        Dim l As Line
        Select Case cmd.Command
            Case "fd"
                endX = my_X + cmd.Value * Math.Cos(my_Angle)
                endY = my_Y + cmd.Value * Math.Sin(my_Angle)
                If my_Drawing Then
                    l = New Line With {
                        .Stroke = my_Color,
                        .StrokeThickness = my_Size,
                        .X1 = my_X,
                        .Y1 = my_Y,
                        .X2 = endX,
                        .Y2 = endY
                    }
                    my_Canvas.Children.Add(l)
                End If
                my_X = endX
                my_Y = endY
            Case "bd"
                endX = my_X - cmd.Value * Math.Cos(my_Angle)
                endY = my_Y - cmd.Value * Math.Sin(my_Angle)
                If my_Drawing Then
                    l = New Line With {
                        .Stroke = my_Color,
                        .StrokeThickness = my_Size,
                        .X1 = my_X,
                        .Y1 = my_Y,
                        .X2 = endX,
                        .Y2 = endY
                    }
                    my_Canvas.Children.Add(l)
                End If
                my_X = endX
                my_Y = endY
            Case "rt"
                my_Angle += cmd.Value * Math.PI / 180
            Case "lt"
                my_Angle -= cmd.Value * Math.PI / 180
            Case "pu"
                my_Drawing = False
            Case "pd"
                my_Drawing = True
            Case "col"
                Dim parts As String() = cmd.Parameter.Split(","c)
                Try
                    Dim r As Byte = Byte.Parse(parts(0))
                    Dim g As Byte = Byte.Parse(parts(1))
                    Dim b As Byte = Byte.Parse(parts(2))
                    my_Color = New SolidColorBrush(Media.Color.FromRgb(r, g, b))
                Catch ex As Exception
                    'Do nothing
                End Try
            Case "size"
                my_Size = cmd.Value
            Case "setx"
                Dim newX As Double = 0.0
                Dim newY As Double = 0.0
                Try
                    newX = Double.Parse(cmd.Value)
                Catch ex As Exception
                    Exit Sub
                End Try
                my_X = newX
            Case "sety"
                Dim newX As Double = 0.0
                Dim newY As Double = 0.0
                Try
                    newY = Double.Parse(cmd.Value)
                Catch ex As Exception
                    Exit Sub
                End Try
                my_Y = newY
            Case "home"
                my_X = my_StartX
                my_Y = my_StartY
        End Select
    End Sub
End Class
