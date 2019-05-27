Public Class Symbol
    Inherits Shape

    Private my_Origin As Point
    Private my_Text As String
    Private face As Typeface
    Private ftext As FormattedText
    Private my_Color As Color
    Private my_FontFamily As FontFamily
    Private my_FontSize As Integer
    Private my_FontStyle As FontStyle
    Private My_FontWeight As FontWeight
    Private Shared Rnd As Random = New Random

    Public Sub New(text As String, origin As Point, color As Color, fontFamilyName As String, fontSize As Integer)
        my_Text = text
        my_Origin = origin
        my_Color = color
        my_FontFamily = New FontFamily(fontFamilyName)
        my_FontSize = fontSize
        my_FontStyle = FontStyles.Normal
        My_FontWeight = FontWeights.Normal
        SetFText()
    End Sub

    Public Property Text As String
        Get
            Return my_Text
        End Get
        Set(value As String)
            my_Text = value
            SetFText()
        End Set
    End Property

    Public Property Left As Double
        Get
            Return my_Origin.X
        End Get
        Set(value As Double)
            my_Origin.X = value
        End Set
    End Property

    Public Property Top As Double
        Get
            Return my_Origin.Y
        End Get
        Set(value As Double)
            my_Origin.Y = value
        End Set
    End Property

    Public Property TxtColor As Color
        Get
            Return my_Color
        End Get
        Set(value As Color)
            my_Color = value
            SetFText()
        End Set
    End Property

    Public Property FontFamily As FontFamily
        Get
            Return my_FontFamily
        End Get
        Set(value As FontFamily)
            my_FontFamily = value
            SetFText()
        End Set
    End Property

    Public Property FontSize As Integer
        Get
            Return my_FontSize
        End Get
        Set(value As Integer)
            my_FontSize = value
            SetFText()
        End Set
    End Property

    Public Property FontStyle As FontStyle
        Get
            Return my_FontStyle
        End Get
        Set(value As FontStyle)
            my_FontStyle = value
            SetFText()
        End Set
    End Property

    Public Property FontWeight As FontWeight
        Get
            Return My_FontWeight
        End Get
        Set(value As FontWeight)
            My_FontWeight = value
            SetFText()
        End Set
    End Property

    Private Sub SetFText()
        face = New Typeface(my_FontFamily, my_FontStyle, My_FontWeight, FontStretches.Normal)
        ftext = New FormattedText(my_Text, New Globalization.CultureInfo("en-GB"), FlowDirection.LeftToRight, face, my_FontSize * 96.0 / 72.0, New SolidColorBrush(my_Color))
        Height = ftext.Height
        Width = ftext.Width
        InvalidateVisual()
    End Sub

    Protected Overrides ReadOnly Property DefiningGeometry As Geometry
        Get
            Return Geometry.Empty
        End Get
    End Property

    Protected Overrides Sub OnRender(drawingContext As DrawingContext)
        drawingContext.DrawText(ftext, my_Origin)
    End Sub

End Class
