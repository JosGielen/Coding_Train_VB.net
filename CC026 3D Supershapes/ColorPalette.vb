Imports System.IO
Imports System.Windows
Imports System.Windows.Media

Public Class ColorPalette
    Private Shared my_Colors As List(Of Color)

    ''' <summary>
    ''' Creates a new empty ColorPalette.
    ''' Use LoadPalette to read the colors from a .CPL file.
    ''' </summary>
    Public Sub New()
        my_Colors = New List(Of Color)
    End Sub

    ''' <summary>
    ''' Creates a ColorPalette from a .CPL file.
    ''' Each line in the file must have R;G;B format (R, G, B = 0-255)
    ''' </summary>
    Public Sub New(file As String)
        LoadPalette(file)
    End Sub

    ''' <summary>
    ''' Read the color data from a .CPL file.
    ''' Each line in the file must have R;G;B format (R, G, B = 0-255).
    ''' </summary>
    Public Sub LoadPalette(file As String)
        Dim Line As String = ""
        Dim txtparts(2) As String
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0
        my_Colors = New List(Of Color)
        Dim sr As StreamReader = Nothing
        Try
            sr = New StreamReader(file)
            Do While Not sr.EndOfStream
                Line = sr.ReadLine()
                txtparts = Line.Split(";"c)
                r = Byte.Parse(txtparts(0))
                g = Byte.Parse(txtparts(1))
                b = Byte.Parse(txtparts(2))
                my_Colors.Add(Color.FromRgb(r, g, b))
            Loop
        Catch ex As Exception
            MessageBox.Show("Cannot load the palette. Original error: " & ex.Message, "Lorenz Attractor error", MessageBoxButton.OK, MessageBoxImage.Error)
        Finally
            If sr IsNot Nothing Then
                sr.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Creates a List(of Color) filled with the rainbow colors (Purple to Red).
    ''' </summary>
    Public Function Rainbow(size As Integer) As List(Of Color)
        MakeRainbow()
        Return GetColors(size)
    End Function


    Public Function GetColors(size As Integer) As List(Of Color)
        Dim result As List(Of Color) = New List(Of Color)
        Dim my_Size As Integer = my_Colors.Count
        Dim newindex As Integer = 0
        Dim mappedIndex As Double = 0.0

        For oldIndex As Integer = 0 To my_Size - 1
            mappedIndex = oldIndex * size / my_Size
            While mappedIndex >= newindex
                result.Add(my_Colors(oldIndex))
                newindex += 1
            End While
        Next
        Return result
    End Function

    Public Function GetColorBrushes(size As Integer) As List(Of Brush)
        Dim tempColors As List(Of Color) = GetColors(size)
        Dim result As List(Of Brush) = New List(Of Brush)
        For I As Integer = 0 To tempColors.Count - 1
            result.Add(New SolidColorBrush(tempColors(I)))
        Next
        Return result
    End Function

    Private Sub MakeRainbow()
        Dim Sharpness As Double = 3.0
        Dim colorCount As Integer = 1024
        Dim r As Byte
        Dim g As Byte
        Dim b As Byte
        my_Colors = New List(Of Color)
        'Fill the Rainbow list
        my_Colors.Clear()
        If Sharpness = 0 Then Sharpness = 1 / colorCount
        For I As Integer = 0 To colorCount
            If I < colorCount / 5 Then         'Purple To Blue
                r = CByte(155 * (1 - Smooth(0, colorCount / 5, I, Sharpness)))
                g = 0
                b = 255
            ElseIf I < 2 * colorCount / 5 Then 'Blue to Cyan
                r = 0
                g = CByte(255 * (Smooth(colorCount / 5, 2 * colorCount / 5, I, Sharpness)))
                b = 255
            ElseIf I < 3 * colorCount / 5 Then 'Cyan to Green
                r = 0
                g = 255
                b = CByte(255 * (1 - Smooth(2 * colorCount / 5, 3 * colorCount / 5, I, Sharpness)))
            ElseIf I < 4 * colorCount / 5 Then 'Green to Yellow
                r = CByte(255 * (Smooth(3 * colorCount / 5, 4 * colorCount / 5, I, Sharpness)))
                g = 255
                b = 0
            Else                               'Yellow to Red
                r = 255
                g = CByte(255 * (1 - Smooth(4 * colorCount / 5, colorCount, I, Sharpness)))
                b = 0
            End If
            my_Colors.Add(Color.FromRgb(r, g, b))
        Next
    End Sub

    Private Function Normalize(min As Double, max As Double, X As Double) As Double
        Return (X - min) / (max - min)
    End Function

    Private Function Sigmoid(X As Double) As Double
        Return 1 / (1 + Math.Exp(-1 * X))
    End Function

    Private Function Smooth(min As Double, max As Double, X As Double, Sharpness As Double) As Double
        Dim Xn As Double = (2 * Normalize(min, max, X) - 1) * Sharpness
        Dim Xmin As Double = Sigmoid(-1 * Sharpness)
        Dim Xmax As Double = Sigmoid(Sharpness)
        Return Normalize(Xmin, Xmax, Sigmoid(Xn))
    End Function

End Class
