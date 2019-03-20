Imports System.IO

Class MainWindow
    Private Rendering As Boolean
    Private a As Double
    Private n As Integer
    Private c As Integer
    Private el As Ellipse
    Private my_Colors As List(Of Color)
    Private colorSize As Integer = 250

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        LoadPalette(Environment.CurrentDirectory & "\Rainbow.cpl", colorSize)
        StartRender()
    End Sub

    Private Sub StartRender()
        'Some initial code
        a = 137.5 'Angle of the pattern
        n = 0
        c = 5
        canvas1.Children.Clear()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = False
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim angle As Double = (n * a) * Math.PI / 180
        Dim radius As Double = c * Math.Sqrt(n)
        Dim X As Double = radius * Math.Cos(angle)
        Dim Y As Double = radius * Math.Sin(angle)
        Dim dist As Integer = Math.Floor(Math.Sqrt(X ^ 2 + Y ^ 2))
        Dim index As Integer = (colorSize - dist) Mod colorSize
        If dist > colorSize Then
            StopRender()
            Exit Sub
        End If
        el = New Ellipse() With {
            .Stroke = New SolidColorBrush(my_Colors(index)),
            .StrokeThickness = 1,
            .Fill = New SolidColorBrush(my_Colors(index)),
            .Width = 8,
            .Height = 8
        }
        el.SetValue(Canvas.TopProperty, Y + canvas1.ActualHeight / 2)
        el.SetValue(Canvas.LeftProperty, X + canvas1.ActualWidth / 2)
        canvas1.Children.Add(el)
        n += 1
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If Rendering Then
            StopRender()
        Else
            StartRender()
        End If
    End Sub

    Private Sub LoadPalette(file As String, size As Integer)
        Dim Line As String = ""
        Dim txtparts(2) As String
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0
        Dim TempColors As New List(Of Color)
        Dim sr As StreamReader = Nothing
        Try
            sr = New StreamReader(file)
            sr.ReadLine() 'Skip the number of color data in the palette file.
            Do While Not sr.EndOfStream
                Line = sr.ReadLine()
                txtparts = Line.Split(";"c)
                r = Byte.Parse(txtparts(0))
                g = Byte.Parse(txtparts(1))
                b = Byte.Parse(txtparts(2))
                TempColors.Add(Color.FromRgb(r, g, b))
            Loop
            my_Colors = New List(Of Color)
            If TempColors.Count <> size Then
                Dim diff As Integer = TempColors.Count - size
                Dim fraction As Double = TempColors.Count / diff
                For I As Integer = 0 To TempColors.Count - 1
                    If I Mod fraction < 1 Then
                        If TempColors.Count > size Then 'Skip some colors
                            Continue For
                        Else 'Duplicate some colors
                            my_Colors.Add(TempColors(I))
                        End If
                    End If
                    my_Colors.Add(TempColors(I))
                Next
            Else
                my_Colors = TempColors
            End If
        Catch ex As Exception
            MessageBox.Show("Cannot load the palette. Original error: " & ex.Message, "Phyllotaxis error", MessageBoxButton.OK, MessageBoxImage.Error)
        Finally
            If sr IsNot Nothing Then
                sr.Close()
            End If
        End Try
    End Sub
End Class
