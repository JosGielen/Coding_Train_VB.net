Public Class Cell
    Private My_Visible As Boolean
    Private my_Value As Integer
    Private my_Row As Integer
    Private my_Col As Integer
    Private my_color As Brush
    Private my_TopLeft As Point
    Private my_Rect As Rectangle
    Private my_Text As Label

    Public Sub New(row As Integer, col As Integer, width As Double, Height As Double)
        My_Visible = False
        my_Row = row
        my_Col = col
        my_Rect = New Rectangle()
        my_Rect.Width = width
        my_Rect.Height = Height
        my_Rect.RadiusX = 40
        my_Rect.RadiusY = 40
        my_Rect.Stroke = Brushes.Black
        my_Rect.StrokeThickness = 1
        my_Text = New Label()
        my_Text.Width = 0.9 * width
        my_Text.Height = 0.8 * Height
        my_Text.Background = Brushes.Transparent
        my_Text.FontSize = 52
        my_Text.FontWeight = FontWeights.Bold
        my_Text.HorizontalContentAlignment = HorizontalAlignment.Center
        my_Text.VerticalContentAlignment = VerticalAlignment.Center
        my_Text.Padding = New Thickness(0.0)
        my_Text.Content = my_Value.ToString()
    End Sub

    Public Property CellValue As Integer
        Get
            Return my_Value
        End Get
        Set(value As Integer)
            my_Value = value
            my_Text.Content = value.ToString()
        End Set
    End Property

    Public Property BackColor As Brush
        Get
            Return my_color
        End Get
        Set(value As Brush)
            my_color = value
            my_Rect.Fill = value
        End Set
    End Property

    Public Property TextSize As Double
        Get
            Return my_Text.FontSize
        End Get
        Set(value As Double)
            my_Text.FontSize = value
        End Set
    End Property

    Public Property Top As Double
        Get
            Return my_TopLeft.Y
        End Get
        Set(value As Double)
            my_TopLeft.Y = value
            my_Rect.SetValue(Canvas.TopProperty, value)
            my_Text.SetValue(Canvas.TopProperty, value + 0.1 * my_Rect.Height)
        End Set
    End Property

    Public Property Left As Double
        Get
            Return my_TopLeft.X
        End Get
        Set(value As Double)
            my_TopLeft.X = value
            my_Rect.SetValue(Canvas.LeftProperty, value)
            my_Text.SetValue(Canvas.LeftProperty, value + 0.05 * my_Rect.Width)
        End Set
    End Property

    Public Property Visible As Boolean
        Get
            Return My_Visible
        End Get
        Set(value As Boolean)
            My_Visible = value
        End Set
    End Property

    Public Sub SetFocus(focus As Boolean)
        If focus = True Then
            my_Rect.Stroke = Brushes.Red
            my_Rect.StrokeThickness = 3
        Else
            my_Rect.Stroke = Brushes.Black
            my_Rect.StrokeThickness = 1
        End If
    End Sub

    Public Sub Draw(can As Canvas)
        If My_Visible Then
            If Not can.Children.Contains(my_Rect) Then can.Children.Add(my_Rect)
            If Not can.Children.Contains(my_Text) Then can.Children.Add(my_Text)
        Else
            If can.Children.Contains(my_Rect) Then can.Children.Remove(my_Rect)
            If can.Children.Contains(my_Text) Then can.Children.Remove(my_Text)
        End If
    End Sub

End Class
