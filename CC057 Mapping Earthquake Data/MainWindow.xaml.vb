Imports System.IO
Imports System.Net

Class MainWindow
    Private Zoom As Double = 1.0
    Private El As Ellipse

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim bitmap As BitmapImage = New BitmapImage(New System.Uri(Environment.CurrentDirectory & "\Earth.jpg"))
        canvas1.Background = New ImageBrush(bitmap)
        Dim address As String = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_week.csv"
        Dim client As WebClient = New WebClient()
        Dim reader As StreamReader = New StreamReader(client.OpenRead(address))
        Dim EarthQuakes As List(Of EQData) = New List(Of EQData)
        While Not reader.EndOfStream
            EarthQuakes.Add(New EQData(reader.ReadLine))
        End While
        For I As Integer = 1 To EarthQuakes.Count - 1
            MarkEQ(EarthQuakes(I).Longitude, EarthQuakes(I).Latitude, EarthQuakes(I).Magnitude)
        Next
        UpdateLayout()
    End Sub

    Private Sub MarkEQ(Longitude As Double, Latitude As Double, Magnitude As Double)
        Dim size As Double = 2 * (Magnitude + 1)
        If Magnitude < 0 Then Magnitude = 0
        Dim G As Double = 255 - 25.5 * Magnitude
        El = New Ellipse() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 1.0,
            .Width = size,
            .Height = size,
            .Fill = New SolidColorBrush(Color.FromRgb(255, CByte(G), 0))
        }
        El.SetValue(Canvas.LeftProperty, MercX(Longitude) - size / 2)
        El.SetValue(Canvas.TopProperty, MercY(Latitude) - size / 2)
        canvas1.Children.Add(El)
    End Sub

    Private Function MercX(longit As Double) As Double
        Return canvas1.ActualWidth / 720 * Math.Pow(2, Zoom) * (longit + 180) - 3
    End Function

    Private Function MercY(latit As Double) As Double
        Return canvas1.ActualHeight / (4 * Math.PI) * Math.Pow(2, Zoom) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + latit * Math.PI / 360)))
    End Function

    Private Sub LongitudeLine(longitude As Double)
        Dim l As Line = New Line() With
            {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1.0,
            .X1 = MercX(longitude),
            .Y1 = 0,
            .X2 = MercX(longitude),
            .Y2 = canvas1.ActualHeight
            }
        canvas1.Children.Add(l)
    End Sub

    Private Sub LatitudeLine(latitude As Double)
        Dim l As Line = New Line() With
            {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1.0,
            .X1 = 0,
            .Y1 = MercY(latitude),
            .X2 = canvas1.ActualWidth,
            .Y2 = MercY(latitude)
            }
        canvas1.Children.Add(l)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class

Public Structure EQData
    Public Longitude As Double
    Public Latitude As Double
    Public Magnitude As Double

    Public Sub New(source As String)
        Dim data() As String = source.Split(","c)
        If data.Length > 4 Then
            Try
                Latitude = Double.Parse(data(1))
                Longitude = Double.Parse(data(2))
                Magnitude = Double.Parse(data(4))
            Catch ex As Exception

            End Try
        End If
    End Sub
End Structure

