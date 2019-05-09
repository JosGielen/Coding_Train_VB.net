Public Class Cell
    Private my_Row As Integer
    Private my_Col As Integer
    Private my_Size As Double
    Private my_IsCurrent As Boolean
    Private my_IsVisited As Boolean
    Private my_HasTopWall As Boolean
    Private my_HasLeftWall As Boolean
    Private my_HasBottomWall As Boolean
    Private my_HasRightWall As Boolean

    Private TopLine As Line
    Private LeftLine As Line
    Private BottomLine As Line
    Private RightLine As Line
    Private floor As Rectangle

    Public Sub New(row As Integer, col As Integer, size As Integer)
        my_Row = row
        my_Col = col
        my_Size = size
        my_IsCurrent = False
        my_IsVisited = False
        my_HasTopWall = True
        my_HasLeftWall = True
        my_HasBottomWall = True
        my_HasRightWall = True
    End Sub

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

    Public Property IsCurrent As Boolean
        Get
            Return my_IsCurrent
        End Get
        Set(value As Boolean)
            my_IsCurrent = value
            If my_IsCurrent Then
                floor.Fill = Brushes.Green
            Else
                If my_IsVisited Then
                    floor.Fill = Brushes.Black
                End If
            End If
        End Set
    End Property

    Public Property IsVisited As Boolean
        Get
            Return my_IsVisited
        End Get
        Set(value As Boolean)
            my_IsVisited = value
            If my_IsVisited And Not my_IsCurrent Then floor.Fill = Brushes.Black
        End Set
    End Property

    Public ReadOnly Property HasTopWall As Boolean
        Get
            Return my_HasTopWall
        End Get
    End Property

    Public ReadOnly Property HasLeftWall As Boolean
        Get
            Return my_HasLeftWall
        End Get
    End Property

    Public ReadOnly Property HasBottomWall As Boolean
        Get
            Return my_HasBottomWall
        End Get
    End Property

    Public ReadOnly Property HasRightWall As Boolean
        Get
            Return my_HasRightWall
        End Get
    End Property

    Public Sub RemoveTopWall()
        TopLine.StrokeThickness = 0.0
        my_HasTopWall = False
    End Sub

    Public Sub RemoveLeftWall()
        LeftLine.StrokeThickness = 0.0
        my_HasLeftWall = False
    End Sub

    Public Sub RemoveBottomWall()
        BottomLine.StrokeThickness = 0.0
        my_HasBottomWall = False
    End Sub

    Public Sub RemoveRightWall()
        RightLine.StrokeThickness = 0.0
        my_HasRightWall = False
    End Sub

    Public Sub SetFill(fillColor As Brush)
        floor.Fill = fillColor
    End Sub

    Public Sub Draw(c As Canvas)
        Dim Top As Double = my_Row * my_Size
        Dim Left As Double = my_Col * my_Size
        'Draw the walls as seperate lines
        TopLine = New Line() With {
            .Stroke = Brushes.White,
            .StrokeThickness = 4,
            .X1 = Left,
            .Y1 = Top,
            .X2 = Left + my_Size,
            .Y2 = Top
        }
        c.Children.Add(TopLine)
        LeftLine = New Line() With {
            .Stroke = Brushes.White,
            .StrokeThickness = 4,
            .X1 = Left,
            .Y1 = Top,
            .X2 = Left,
            .Y2 = Top + my_Size
        }
        c.Children.Add(LeftLine)
        BottomLine = New Line() With {
            .Stroke = Brushes.White,
            .StrokeThickness = 4,
            .X1 = Left,
            .Y1 = Top + my_Size,
            .X2 = Left + my_Size,
            .Y2 = Top + my_Size
        }
        c.Children.Add(BottomLine)
        RightLine = New Line() With {
            .Stroke = Brushes.White,
            .StrokeThickness = 4,
            .X1 = Left + my_Size,
            .Y1 = Top,
            .X2 = Left + my_Size,
            .Y2 = Top + my_Size
        }
        c.Children.Add(RightLine)
        'Draw the fill
        floor = New Rectangle() With {
            .Stroke = Brushes.Blue,
            .StrokeThickness = 0.0,
            .Width = my_Size,
            .Height = my_Size,
            .Fill = Brushes.Gray
        }
        floor.SetValue(Canvas.TopProperty, Top)
        floor.SetValue(Canvas.LeftProperty, Left)
        c.Children.Add(floor)
    End Sub

End Class

