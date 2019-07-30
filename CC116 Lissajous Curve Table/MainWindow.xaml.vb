
Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private Rendering As Boolean
    Private Rows As Integer
    Private Cols As Integer
    Private Angle As Double
    Private CircleDia As Double
    Private ColCenters() As Double
    Private RowCenters() As Double
    Private X() As Double
    Private Y() As Double
    Private ColDots() As Ellipse
    Private RowDots() As Ellipse
    Private ColLines() As Line
    Private RowLines() As Line
    Private Figures(,) As Polyline

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim El As Ellipse
        Dim Spacing As Double = 20.0
        Cols = 8
        Rows = 8
        ReDim ColCenters(Cols)
        ReDim X(Cols)
        ReDim ColDots(Cols)
        ReDim ColLines(Cols)
        ReDim RowCenters(Rows)
        ReDim Y(Rows)
        ReDim RowDots(Rows)
        ReDim RowLines(Rows)
        ReDim Figures(Cols, Rows)
        'Determine the Circle diameter
        If (canvas1.ActualWidth - (Cols + 2) * Spacing) / (Cols + 1) < (canvas1.ActualHeight - (Rows + 2) * Spacing) / (Rows + 1) Then
            CircleDia = (canvas1.ActualWidth - (Cols + 2) * Spacing) / (Cols + 1)
        Else
            CircleDia = (canvas1.ActualHeight - (Rows + 2) * Spacing) / (Rows + 1)
        End If
        'Calculate the starting positions
        Angle = -1 * Math.PI / 2
        For I As Integer = 0 To Cols
            ColCenters(I) = Spacing + I * (Spacing + CircleDia) + CircleDia / 2
            X(I) = ColCenters(I) + (CircleDia / 2) * Math.Cos(Angle)
        Next
        For I As Integer = 0 To Rows
            RowCenters(I) = Spacing + I * (Spacing + CircleDia) + CircleDia / 2
            Y(I) = RowCenters(I) + (CircleDia / 2) * Math.Sin(Angle)
        Next
        'Draw the Column Circles, Dots and Lines
        For I As Integer = 1 To Cols
            El = New Ellipse() With
            {
                .Width = CircleDia,
                .Height = CircleDia,
                .Stroke = Brushes.White,
                .StrokeThickness = 1.0
            }
            El.SetValue(Canvas.LeftProperty, ColCenters(I) - CircleDia / 2)
            El.SetValue(Canvas.TopProperty, Spacing)
            canvas1.Children.Add(El)
            ColDots(I) = New Ellipse() With
            {
                .Width = 6,
                .Height = 6,
                .Fill = Brushes.Red
            }
            ColDots(I).SetValue(Canvas.LeftProperty, X(I) - 3)
            ColDots(I).SetValue(Canvas.TopProperty, Y(0) - 3)
            canvas1.Children.Add(ColDots(I))
            ColLines(I) = New Line() With
            {
                .Stroke = New SolidColorBrush(Color.FromArgb(150, 150, 150, 150)),
                .StrokeThickness = 1.0,
                .X1 = X(I),
                .Y1 = Y(0),
                .X2 = X(I),
                .Y2 = canvas1.ActualHeight
            }
            canvas1.Children.Add(ColLines(I))
        Next
        'Draw the Row Circles, Dots and Lines
        For I As Integer = 1 To Rows
            El = New Ellipse() With
            {
                .Width = CircleDia,
                .Height = CircleDia,
                .Stroke = Brushes.White,
                .StrokeThickness = 1.0
            }
            El.SetValue(Canvas.LeftProperty, Spacing)
            El.SetValue(Canvas.TopProperty, RowCenters(I) - CircleDia / 2)
            canvas1.Children.Add(El)
            RowDots(I) = New Ellipse() With
            {
                .Width = 6,
                .Height = 6,
                .Fill = Brushes.Red
            }
            RowDots(I).SetValue(Canvas.LeftProperty, X(0) - 3)
            RowDots(I).SetValue(Canvas.TopProperty, Y(I) - 3)
            canvas1.Children.Add(RowDots(I))
            RowLines(I) = New Line() With
            {
                .Stroke = New SolidColorBrush(Color.FromArgb(150, 150, 150, 150)),
                .StrokeThickness = 1.0,
                .X1 = X(0),
                .Y1 = Y(I),
                .X2 = canvas1.ActualWidth,
                .Y2 = Y(I)
            }
            canvas1.Children.Add(RowLines(I))
        Next
        For I As Integer = 1 To Cols
            For J As Integer = 1 To Rows
                Figures(I, J) = New Polyline() With
                {
                    .Stroke = Brushes.White,
                    .StrokeThickness = 1.0
                }
                canvas1.Children.Add(Figures(I, J))
            Next
        Next
        Rendering = True
        AddHandler CompositionTarget.Rendering, AddressOf Render
    End Sub

    Public Sub Render(sender As Object, e As EventArgs)
        If Not Rendering Then Exit Sub
        For I As Integer = 0 To Cols
            X(I) = ColCenters(I) + (CircleDia / 2) * Math.Cos(Angle * I)
        Next
        For I As Integer = 0 To Rows
            Y(I) = RowCenters(I) + (CircleDia / 2) * Math.Sin(Angle * I)
        Next
        'Update the Column Dots and Lines
        For I As Integer = 1 To Cols
            ColDots(I).SetValue(Canvas.LeftProperty, X(I) - 3)
            ColDots(I).SetValue(Canvas.TopProperty, RowCenters(0) + (CircleDia / 2) * Math.Sin(Angle * I) - 3)
            ColLines(I).X1 = X(I)
            ColLines(I).Y1 = RowCenters(0) + (CircleDia / 2) * Math.Sin(Angle * I)
            ColLines(I).X2 = X(I)
            ColLines(I).Y2 = canvas1.ActualHeight
        Next
        'Update the Row Dots and Lines
        For I As Integer = 1 To Rows
            RowDots(I).SetValue(Canvas.LeftProperty, ColCenters(0) + (CircleDia / 2) * Math.Cos(Angle * I) - 3)
            RowDots(I).SetValue(Canvas.TopProperty, Y(I) - 3)
            RowLines(I).X1 = ColCenters(0) + (CircleDia / 2) * Math.Cos(Angle * I)
            RowLines(I).Y1 = Y(I)
            RowLines(I).X2 = canvas1.ActualWidth
            RowLines(I).Y2 = Y(I)
        Next
        'Draw the Figures
        For I As Integer = 1 To Cols
            For J As Integer = 1 To Rows
                Figures(I, J).Points.Add(New Point(X(I), Y(J)))
            Next
        Next
        Angle -= 0.01
        If Angle < -5 * Math.PI / 2 Then
            Threading.Thread.Sleep(3000)
            'Reset the angle and curves
            Angle = -1 * Math.PI / 2
            For I As Integer = 1 To Cols
                For J As Integer = 1 To Rows
                    Figures(I, J).Points.Clear()
                Next
            Next
        End If
    End Sub

End Class
