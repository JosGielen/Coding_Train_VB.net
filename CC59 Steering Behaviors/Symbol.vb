Public Class Symbol
    Inherits Shape

    Private my_Origin As Point
    Private my_Text As String
    Private face As Typeface
    Private ftext As FormattedText
    Private my_FontFamily As FontFamily
    Private my_FontSize As Double
    Private my_FontStyle As FontStyle
    Private My_FontWeight As FontWeight
    Private my_Geometry As Geometry

    Public Sub New(text As String)
        my_Text = text
        my_Origin = New Point(0, 0)
        my_FontFamily = New FontFamily("Arial")
        my_FontSize = 12
        my_FontStyle = FontStyles.Normal
        My_FontWeight = FontWeights.Bold
        Update()
    End Sub

    Public Property Text As String
        Get
            Return my_Text
        End Get
        Set(value As String)
            my_Text = value
            Update()
        End Set
    End Property

    Public Property Origin As Point
        Get
            Return my_Origin
        End Get
        Set(value As Point)
            my_Origin = value
            Update()
        End Set
    End Property

    Public Property FontFamily As FontFamily
        Get
            Return my_FontFamily
        End Get
        Set(value As FontFamily)
            my_FontFamily = value
            Update()
        End Set
    End Property

    Public Property FontSize As Double
        Get
            Return my_FontSize
        End Get
        Set(value As Double)
            my_FontSize = value
            Update()
        End Set
    End Property

    Public Property FontStyle As FontStyle
        Get
            Return my_FontStyle
        End Get
        Set(value As FontStyle)
            my_FontStyle = value
            Update()
        End Set
    End Property

    Public Property FontWeight As FontWeight
        Get
            Return My_FontWeight
        End Get
        Set(value As FontWeight)
            My_FontWeight = value
            Update()
        End Set
    End Property

    Public ReadOnly Property Geometry As Geometry
        Get
            Return ftext.BuildGeometry(my_Origin)
        End Get
    End Property

    Private Sub Update()
        face = New Typeface(my_FontFamily, my_FontStyle, My_FontWeight, FontStretches.Normal)
        ftext = New FormattedText(my_Text, New Globalization.CultureInfo("en-GB"), FlowDirection.LeftToRight, face, my_FontSize * 96.0 / 72.0, Fill)
        my_Geometry = ftext.BuildGeometry(my_Origin)
    End Sub

    Protected Overrides ReadOnly Property DefiningGeometry As Geometry
        Get
            Return my_Geometry
        End Get
    End Property

    Protected Overrides Function MeasureOverride(constraint As Size) As Size
        If my_Geometry.Bounds = Rect.Empty Then
            Return New Size(0, 0)
        Else
            Return New Size(Math.Min(constraint.Width, my_Geometry.Bounds.Width), Math.Min(constraint.Height, my_Geometry.Bounds.Height))
        End If
    End Function

    Protected Overrides Sub OnRender(drawingContext As DrawingContext)
        drawingContext.DrawGeometry(Me.Fill, New Pen(Me.Stroke, Me.StrokeThickness), my_Geometry)
    End Sub
End Class
