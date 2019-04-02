Public Class SevenSegmentDisplay

    Private Back As Rectangle
    Private Segments(6) As Polygon
    Private Dot As Ellipse = New Ellipse
    Private my_Value As Integer
    Private HexCodes(9) As String
    Private my_Width As Double
    Private my_Height As Double
    Private my_Left As Double
    Private my_Top As Double
    Private my_SegmentColor As Brush
    Private my_Backcolor As Brush
    Private my_Border As Double             '% of segment Height
    Private my_SegmentThickness As Double   '% of segment Height
    Private my_VertSegmentLength As Double
    Private my_HorSegmentLength As Double
    Private my_Campher As Double
    Private my_HasCampher As Boolean
    Private my_SegmentSpace As Double       '% of segment Height
    Private my_IsTilted As Boolean
    Private my_ShowDot As Boolean

    Public Sub New(width As Double, height As Double, left As Double, top As Double)
        my_Width = width
        my_Height = height
        my_Left = left
        my_Top = top
        my_SegmentColor = Brushes.Red
        my_Backcolor = Brushes.Black
        my_Border = CInt(3 * my_Height / 100)
        my_SegmentThickness = CInt(10 * my_Height / 100)
        my_HasCampher = True
        my_Campher = CInt(my_SegmentThickness / 2)
        my_SegmentSpace = CInt(my_Height / 100)
        my_IsTilted = True
        HexCodes = {"1111110", "0110000", "1101101", "1111001", "0110011", "1011011", "1011111", "1110000", "1111111", "1111011"}
        Back = New Rectangle()
        For I As Integer = 0 To 6
            Segments(I) = New Polygon() With
            {
                .Visibility = Visibility.Hidden
            }
        Next
        Dot = New Ellipse() With
        {
            .Visibility = Visibility.Hidden
        }
        MakeSegments()
    End Sub

#Region "Properties"

    Public Property Width As Double
        Get
            Return my_Width
        End Get
        Set(value As Double)
            my_Width = value
            MakeSegments()
        End Set
    End Property

    Public Property Height As Double
        Get
            Return my_Height
        End Get
        Set(value As Double)
            my_Height = value
            MakeSegments()
        End Set
    End Property

    Public Property Left As Double
        Get
            Return my_Left
        End Get
        Set(value As Double)
            my_Left = value
            MakeSegments()
        End Set
    End Property

    Public Property Top As Double
        Get
            Return my_Top
        End Get
        Set(value As Double)
            my_Top = value
            MakeSegments()
        End Set
    End Property

    Public Property SegmentColor As Brush
        Get
            Return my_SegmentColor
        End Get
        Set(value As Brush)
            my_SegmentColor = value
            MakeSegments()
        End Set
    End Property

    Public Property Border As Double
        Get
            Return my_Border
        End Get
        Set(value As Double)
            my_Border = value
            MakeSegments()
        End Set
    End Property

    Public Property SegmentThickness As Double
        Get
            Return my_SegmentThickness
        End Get
        Set(value As Double)
            my_SegmentThickness = value
            MakeSegments()
        End Set
    End Property

    Public Property Campher As Double
        Get
            Return my_Campher
        End Get
        Set(value As Double)
            my_Campher = value
            MakeSegments()
        End Set
    End Property

    Public Property HasCampher As Boolean
        Get
            Return my_HasCampher
        End Get
        Set(value As Boolean)
            my_HasCampher = value
            MakeSegments()
        End Set
    End Property

    Public Property SegmentSpace As Double
        Get
            Return my_SegmentSpace
        End Get
        Set(value As Double)
            my_SegmentSpace = value
            MakeSegments()
        End Set
    End Property

    Public Property IsTilted As Boolean
        Get
            Return my_IsTilted
        End Get
        Set(value As Boolean)
            my_IsTilted = value
            MakeSegments()
        End Set
    End Property

    Public Property ShowDot As Boolean
        Get
            Return my_ShowDot
        End Get
        Set(value As Boolean)
            my_ShowDot = value
            MakeSegments()
        End Set
    End Property

    Public Property Backcolor As Brush
        Get
            Return my_Backcolor
        End Get
        Set(value As Brush)
            my_Backcolor = value
            MakeSegments()
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return my_Value
        End Get
        Set(value As Integer)
            my_Value = value
            If my_Value > 9 Then my_Value = 9
            If my_Value < 0 Then my_Value = 0
            'Set segment visibility to display the value
            For I As Integer = 0 To 6
                If HexCodes(my_Value)(I) = "1"c Then
                    Segments(I).Visibility = Visibility.Visible
                Else
                    Segments(I).Visibility = Visibility.Hidden
                End If
            Next
            If my_ShowDot Then
                Dot.Visibility = Visibility.Visible
            Else
                Dot.Visibility = Visibility.Hidden
            End If
        End Set
    End Property

#End Region

    Private Sub MakeSegments()
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim DX As Double = 0.0
        Dim DY1 As Double = 0.0 'Y of furthest tilted point
        Dim DX1 As Double = 0.0 'X shift of furthest tilted point
        Dim points As PointCollection
        my_VertSegmentLength = (my_Height - 2 * my_Border - 3 * my_SegmentThickness - 4 * my_SegmentSpace) / 2
        my_HorSegmentLength = my_Width - 2 * my_Border - 3 * my_SegmentThickness - 3 * my_SegmentSpace
        DY1 = my_Height - my_Border - my_SegmentThickness - my_SegmentSpace 'Y of furthest tilted point
        DX1 = my_Width - 2 * my_Border - 2 * my_SegmentThickness - 2 * my_SegmentSpace - my_HorSegmentLength 'X shift of furthest tilted point
        'Display Background
        Back.Width = my_Width
        Back.Height = my_Height
        'segment A
        points = New PointCollection()
        X = my_Border + my_SegmentThickness + my_SegmentSpace
        Y = my_Border
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(0).Points = points
        'segment B
        points = New PointCollection()
        X = my_Border + my_SegmentThickness + my_HorSegmentLength + 2 * my_SegmentSpace
        Y = my_Border + my_SegmentThickness + my_SegmentSpace
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y - my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(1).Points = points
        'segment C
        points = New PointCollection()
        X = my_Border + my_SegmentThickness + my_HorSegmentLength + 2 * my_SegmentSpace
        Y = my_Border + 2 * my_SegmentThickness + 3 * my_SegmentSpace + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y - my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(2).Points = points
        'segment D
        points = New PointCollection()
        X = my_Border + my_SegmentThickness + my_SegmentSpace
        Y = my_Border + 2 * my_SegmentThickness + 2 * my_VertSegmentLength + 4 * my_SegmentSpace
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(3).Points = points
        'segment E
        points = New PointCollection()
        X = my_Border
        Y = my_Border + 2 * my_SegmentThickness + 3 * my_SegmentSpace + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y - my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(4).Points = points
        'segment F
        points = New PointCollection()
        X = my_Border
        Y = my_Border + my_SegmentThickness + my_SegmentSpace
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y + my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y + my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_SegmentThickness / 2
        Y = Y - my_Campher
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Y = Y - my_VertSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(5).Points = points
        'segment G
        points = New PointCollection()
        X = my_Border + my_SegmentThickness + my_SegmentSpace
        Y = my_Border + my_SegmentThickness + my_VertSegmentLength + 2 * my_SegmentSpace
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y + my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_HorSegmentLength
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X - my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        X = X + my_Campher
        Y = Y - my_SegmentThickness / 2
        If my_IsTilted Then DX = DX1 * (my_Height - Y) / DY1
        points.Add(New Point(X + DX, Y))
        Segments(6).Points = points
        'Decimal Point
        X = my_Width - my_Border - my_SegmentThickness
        Y = my_Height - my_Border - my_SegmentThickness
        Dot.Width = my_SegmentThickness
        Dot.Height = my_SegmentThickness
        Dot.SetValue(Canvas.LeftProperty, X + my_Left)
        Dot.SetValue(Canvas.TopProperty, Y + my_Top)
        'Set the colors and the position of the display in the canvas.
        Back.Fill = Backcolor
        Back.SetValue(Canvas.LeftProperty, my_Left)
        Back.SetValue(Canvas.TopProperty, my_Top)
        For I As Integer = 0 To 6
            Segments(I).SetValue(Canvas.LeftProperty, my_Left)
            Segments(I).SetValue(Canvas.TopProperty, my_Top)
            Segments(I).Fill = my_SegmentColor
        Next
        Dot.Fill = my_SegmentColor
    End Sub

    Public Sub Draw(c As Canvas)
        Back.SetValue(Canvas.LeftProperty, my_Left)
        Back.SetValue(Canvas.TopProperty, my_Top)
        c.Children.Add(Back)
        For I As Integer = 0 To 6
            Segments(I).SetValue(Canvas.LeftProperty, my_Left)
            Segments(I).SetValue(Canvas.TopProperty, my_Top)
            Segments(I).Fill = my_SegmentColor
            c.Children.Add(Segments(I))
        Next
        Dot.Fill = my_SegmentColor
        c.Children.Add(Dot)
    End Sub

    Public Sub SetDefault()
        my_SegmentColor = Brushes.Red
        my_Backcolor = Brushes.Black
        my_Border = CInt(3 * my_Height / 100)
        my_SegmentThickness = CInt(10 * my_Height / 100)
        my_HasCampher = True
        my_Campher = CInt(my_SegmentThickness / 2)
        my_SegmentSpace = CInt(my_Height / 100)
        my_IsTilted = True
        MakeSegments()
    End Sub
End Class
