Class MainWindow
    Private Tiles As List(Of Tile)
    Private TileLength As Double = 50.0
    Private TileHeight As Double = 50.0
    Private delta As Double = 0
    Private angle As Double = 60
    Private StarColor As Brush = Brushes.Yellow
    Private App_Loaded As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        SldHorSize.Value = TileLength
        SldVertSize.Value = TileHeight
        SldDelta.Value = delta
        SldAngle.Value = angle
        LstTiling.SelectedIndex = 0
        App_Loaded = True
        DrawTiles()
    End Sub

    Private Sub DrawTiles()
        Tiles = New List(Of Tile)
        canvas1.Children.Clear()
        Select Case LstTiling.SelectedIndex
            Case 0
                TriangleTiling1(TileLength, TileHeight)
            Case 1
                TriangleTiling2(TileLength, TileHeight)
            Case 2
                TriangleTiling3(TileLength, TileHeight)
            Case 3
                RectangleTiling1(TileLength, TileHeight)
            Case 4
                RectangleTiling2(TileLength, TileHeight)
            Case 5
                PentagonTiling(TileLength)
            Case 6
                HexagonTiling1(TileLength)
            Case 7
                HexagonTiling2(TileLength)
            Case 8
                OctagonTiling1(TileLength)
            Case 9
                OctagonTiling2(TileLength)
            Case 10
                DecagonTiling(TileLength)
            Case 11
                DodecagonTiling1(TileLength)
            Case 12
                DodecagonTiling2(TileLength)
        End Select
        'Draw the tiles
        For I As Integer = 0 To Tiles.Count - 1
            Tiles(I).MakeStar(delta, angle)
            Tiles(I).Draw(canvas1)
            If CbShowStar.IsChecked Then Tiles(I).DrawStar(canvas1)
        Next
    End Sub

#Region "Tiling Patterns"

    ''' <summary>
    ''' Create a Horizontal grid with 2 Triangles Tiling Pattern
    ''' </summary>
    Private Sub TriangleTiling1(L As Double, H As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L, Y0)
            T.AddPoint(X0 + L, Y0 + H)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0, Y0 + H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L
            If X0 >= canvas1.ActualWidth + L / 4 Then
                X0 = 0
                Y0 = Y0 + H
                If Y0 >= canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Horizontal Grid with 4 Triangles Tiling Pattern
    ''' </summary>
    Private Sub TriangleTiling2(L As Double, H As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L, Y0)
            T.AddPoint(X0 + L / 2, Y0 + H / 2)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L, Y0)
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0 + L / 2, Y0 + H / 2)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0, Y0 + H)
            T.AddPoint(X0 + L / 2, Y0 + H / 2)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 + H)
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L / 2, Y0 + H / 2)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L
            If X0 >= canvas1.ActualWidth + L / 2 Then
                X0 = 0
                Y0 = Y0 + H
                If Y0 >= canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a 45° Rotated Grid with 2 Triangles Tiling Pattern
    ''' </summary>
    Private Sub TriangleTiling3(L As Double, H As Double)
        Dim T As Tile
        Dim ToLeft As Boolean = True
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0 - H)
            T.AddPoint(X0 + L / 2, Y0)
            T.AddPoint(X0 - L / 2, Y0)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L / 2, Y0)
            T.AddPoint(X0, Y0 + H)
            T.AddPoint(X0 - L / 2, Y0)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L
            If X0 >= canvas1.ActualWidth + L / 2 Then
                If ToLeft Then
                    X0 = L / 2
                    ToLeft = False
                Else
                    X0 = 0
                    ToLeft = True
                End If
                Y0 = Y0 + H
                If Y0 >= canvas1.ActualHeight + H Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Rectangular grid Tiling Pattern
    ''' </summary>
    Private Sub RectangleTiling1(L As Double, H As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L, Y0)
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0, Y0 + H)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L
            If X0 >= canvas1.ActualWidth + L / 2 Then
                X0 = 0
                Y0 = Y0 + H
                If Y0 >= canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a 4 Rotated Rectangles and square Tiling Pattern
    ''' </summary>
    Private Sub RectangleTiling2(L As Double, H As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0 + H, Y0 + H)
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0 + L, Y0 + L)
            T.AddPoint(X0 + H, Y0 + L)
            T.FillColor = Brushes.Yellow
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + H, Y0)
            T.AddPoint(X0 + H, Y0 + L)
            T.AddPoint(X0, Y0 + L)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + H, Y0)
            T.AddPoint(X0 + H + L, Y0)
            T.AddPoint(X0 + H + L, Y0 + H)
            T.AddPoint(X0 + H, Y0 + H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 + L)
            T.AddPoint(X0 + L, Y0 + L)
            T.AddPoint(X0 + L, Y0 + H + L)
            T.AddPoint(X0, Y0 + H + L)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0 + H + L, Y0 + H)
            T.AddPoint(X0 + H + L, Y0 + H + L)
            T.AddPoint(X0 + L, Y0 + H + L)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + H + L
            If X0 >= canvas1.ActualWidth + H / 2 Then
                X0 = 0
                Y0 = Y0 + H + L
                If Y0 >= canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Pentagonal Tiling Pattern
    ''' </summary>
    Private Sub PentagonTiling(L As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Dim H1 As Double = L * Math.Sin(36 * Math.PI / 180)
        Dim H2 As Double = L * Math.Sin(72 * Math.PI / 180)
        Dim W As Double = L * Math.Cos(36 * Math.PI / 180)
        Do
            T = New Tile()
            T.AddPoint(X0 - L / 2, Y0 + H2)
            T.AddPoint(X0 + L / 2, Y0 + H2)
            T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2)
            T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 - H1)
            T.AddPoint(X0 + W, Y0)
            T.AddPoint(X0 + L / 2, Y0 + H2)
            T.AddPoint(X0 - L / 2, Y0 + H2)
            T.AddPoint(X0 - W, Y0)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + W, Y0)
            T.AddPoint(X0 + W + L, Y0)
            T.AddPoint(X0 + 2 * W + L / 2, Y0 + H2)
            T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2)
            T.AddPoint(X0 + L / 2, Y0 + H2)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - L / 2, Y0 + H2)
            T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2)
            T.AddPoint(X0, Y0 + H1 + 2 * H2)
            T.AddPoint(X0 - L, Y0 + H1 + 2 * H2)
            T.AddPoint(X0 - W - L / 2, Y0 + H1 + H2)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2)
            T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2)
            T.AddPoint(X0 + 2 * W, Y0 + H1 + 2 * H2)
            T.AddPoint(X0 + W, Y0 + 2 * (H1 + H2))
            T.AddPoint(X0, Y0 + H1 + 2 * H2)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + 2 * W, Y0 + H1 + 2 * H2)
            T.AddPoint(X0 + 2 * W + L, Y0 + H1 + 2 * H2)
            T.AddPoint(X0 + W + L, Y0 + 2 * (H1 + H2))
            T.AddPoint(X0 + W, Y0 + 2 * (H1 + H2))
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + 2 * W + L
            If X0 >= canvas1.ActualWidth + L + W Then
                X0 = 0
                Y0 = Y0 + 2 * (H1 + H2)
                If Y0 >= canvas1.ActualHeight Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Hexagonal Tiling Pattern
    ''' </summary>
    Private Sub HexagonTiling1(L As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Dim H As Double = L * Math.Sin(Math.PI / 3)
        Do
            T = New Tile()
            T.AddPoint(X0, Y0 - H)
            T.AddPoint(X0 + L, Y0 - H)
            T.AddPoint(X0 + 3 * L / 2, Y0)
            T.AddPoint(X0 + L, Y0 + H)
            T.AddPoint(X0, Y0 + H)
            T.AddPoint(X0 - L / 2, Y0)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + 3 * L / 2, Y0)
            T.AddPoint(X0 + 5 * L / 2, Y0)
            T.AddPoint(X0 + 3 * L, Y0 + H)
            T.AddPoint(X0 + 5 * L / 2, Y0 + 2 * H)
            T.AddPoint(X0 + 3 * L / 2, Y0 + 2 * H)
            T.AddPoint(X0 + L, Y0 + H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + 3 * L
            If X0 >= canvas1.ActualWidth + L / 2 Then
                X0 = 0
                Y0 = Y0 + 2 * H
                If Y0 >= canvas1.ActualHeight + 2 * H Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a spaced Hexagonal Tiling Pattern
    ''' </summary>
    Private Sub HexagonTiling2(L As Double)
        Dim T As Tile
        Dim up As Boolean = True
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Dim H As Double = L * Math.Sin(Math.PI / 3)
        Dim W As Double = L * Math.Cos(Math.PI / 3)
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + L, Y0)
            T.AddPoint(X0 + L + W, Y0 + H)
            T.AddPoint(X0 + L, Y0 + 2 * H)
            T.AddPoint(X0, Y0 + 2 * H)
            T.AddPoint(X0 - W, Y0 + H)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - W, Y0 + H)
            T.AddPoint(X0, Y0 + 2 * H)
            T.AddPoint(X0 - H, Y0 + 2 * H + W)
            T.AddPoint(X0 - H - W, Y0 + H + W)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 + 2 * H)
            T.AddPoint(X0 + L, Y0 + 2 * H)
            T.AddPoint(X0 + L, Y0 + L + 2 * H)
            T.AddPoint(X0, Y0 + L + 2 * H)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L + W, Y0 + H)
            T.AddPoint(X0 + L + H + W, Y0 + H + W)
            T.AddPoint(X0 + L + H, Y0 + 2 * H + W)
            T.AddPoint(X0 + L, Y0 + 2 * H)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 + 2 * H)
            T.AddPoint(X0, Y0 + L + 2 * H)
            T.AddPoint(X0 - H, Y0 + 2 * H + W)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L, Y0 + 2 * H)
            T.AddPoint(X0 + L + H, Y0 + 2 * H + W)
            T.AddPoint(X0 + L, Y0 + L + 2 * H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L + H + W
            If up Then
                Y0 = Y0 - H - W
                up = False
            Else
                Y0 = Y0 + H + W
                up = True
            End If
            If X0 >= canvas1.ActualWidth + L + W Then
                X0 = 0
                If up Then
                    Y0 = Y0 + L + 2 * H
                Else
                    Y0 = Y0 + L + 3 * H + W

                End If
                up = True
                If Y0 >= canvas1.ActualHeight + 2 * H Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Octagon Grid Tiling Pattern
    ''' </summary>
    Private Sub OctagonTiling1(L As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Dim H As Double = L * Math.Sin(Math.PI / 4)
        Do
            T = New Tile()
            T.AddPoint(X0, Y0 - H)
            T.AddPoint(X0 + L, Y0 - H)
            T.AddPoint(X0 + L + H, Y0)
            T.AddPoint(X0 + L + H, Y0 + L)
            T.AddPoint(X0 + L, Y0 + L + H)
            T.AddPoint(X0, Y0 + L + H)
            T.AddPoint(X0 - H, Y0 + L)
            T.AddPoint(X0 - H, Y0)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L + H, Y0 + L)
            T.AddPoint(X0 + L + 2 * H, Y0 + L + H)
            T.AddPoint(X0 + L + H, Y0 + L + 2 * H)
            T.AddPoint(X0 + L, Y0 + L + H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L + 2 * H
            If X0 >= canvas1.ActualWidth + L Then
                X0 = 0
                Y0 = Y0 + L + 2 * H
                If Y0 >= canvas1.ActualHeight + L Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a 45° rotated Octagon Tiling Pattern
    ''' </summary>
    Private Sub OctagonTiling2(L As Double)
        Dim T As Tile
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Dim H As Double = L * Math.Sin(Math.PI / 4)
        Do
            T = New Tile()
            T.AddPoint(X0 + H, Y0 - H)
            T.AddPoint(X0 + H + L, Y0 - H)
            T.AddPoint(X0 + 2 * H + L, Y0)
            T.AddPoint(X0 + 2 * H + L, Y0 + L)
            T.AddPoint(X0 + H + L, Y0 + H + L)
            T.AddPoint(X0 + H, Y0 + H + L)
            T.AddPoint(X0, Y0 + L)
            T.AddPoint(X0, Y0)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + 2 * H + L, Y0)
            T.AddPoint(X0 + 2 * (H + L), Y0)
            T.AddPoint(X0 + 2 * (H + L), Y0 + L)
            T.AddPoint(X0 + 2 * H + L, Y0 + L)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - L, Y0 + L)
            T.AddPoint(X0, Y0 + L)
            T.AddPoint(X0 + H, Y0 + L + H)
            T.AddPoint(X0 + H, Y0 + 2 * L + H)
            T.AddPoint(X0, Y0 + 2 * (L + H))
            T.AddPoint(X0 - L, Y0 + 2 * (L + H))
            T.AddPoint(X0 - L - H, Y0 + 2 * L + H)
            T.AddPoint(X0 - L - H, Y0 + L + H)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + H, Y0 + H + L)
            T.AddPoint(X0 + L + H, Y0 + H + L)
            T.AddPoint(X0 + L + H, Y0 + H + 2 * L)
            T.AddPoint(X0 + H, Y0 + H + 2 * L)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + 2 * (L + H)
            If X0 >= canvas1.ActualWidth + 3 * L Then
                X0 = 0
                Y0 = Y0 + 2 * (L + H)
                If Y0 >= canvas1.ActualHeight + L Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Decagon Tiling Pattern
    ''' </summary>
    Private Sub DecagonTiling(L As Double)
        Dim T As Tile
        Dim H1 As Double = L * Math.Sin(54 * Math.PI / 180)
        Dim W1 As Double = L * Math.Cos(54 * Math.PI / 180)
        Dim H2 As Double = L * Math.Sin(18 * Math.PI / 180)
        Dim W2 As Double = L * Math.Cos(18 * Math.PI / 180)
        Dim X0 As Double = -W1
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + W1, Y0 - H1)
            T.AddPoint(X0 + W1 + W2, Y0 - H1 - H2)
            T.AddPoint(X0 + W1 + 2 * W2, Y0 - H1)
            T.AddPoint(X0 + 2 * (W1 + W2), Y0)
            T.AddPoint(X0 + 2 * (W1 + W2), Y0 + L)
            T.AddPoint(X0 + W1 + 2 * W2, Y0 + L + H1)
            T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2)
            T.AddPoint(X0 + W1, Y0 + L + H1)
            T.AddPoint(X0, Y0 + L)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + 2 * (W1 + W2), Y0 + L)
            T.AddPoint(X0 + 3 * W1 + 2 * W2, Y0 + L + H1)
            T.AddPoint(X0 + 3 * W1 + W2, Y0 + L + H1 + H2)
            T.AddPoint(X0 + 2 * W1 + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2)
            T.AddPoint(X0 + W1 + 2 * W2, Y0 + L + H1)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + W1, Y0 + L + H1)
            T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2)
            T.AddPoint(X0 + 2 * W1 + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + 2 * W1 + W2, Y0 + 2 * (L + H1) + H2)
            T.AddPoint(X0 + W1 + W2, Y0 + 2 * L + 3 * H1 + H2)
            T.AddPoint(X0 + W1, Y0 + 2 * L + 3 * H1 + 2 * H2)
            T.AddPoint(X0 + W1 - W2, Y0 + 2 * L + 3 * H1 + H2)
            T.AddPoint(X0 - W2, Y0 + 2 * (L + H1) + H2)
            T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + W1 - W2, Y0 + L + H1 + H2)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + 2 * W1 + W2, Y0 + 2 * (L + H1) + H2)
            T.AddPoint(X0 + 3 * W1 + W2, Y0 + 2 * L + 3 * H1 + H2)
            T.AddPoint(X0 + 3 * W1 + 2 * W2, Y0 + 2 * L + 3 * H1 + 2 * H2)
            T.AddPoint(X0 + 2 * (W1 + W2), Y0 + 2 * L + 4 * H1 + 2 * H2)
            T.AddPoint(X0 + W1 + 2 * W2, Y0 + 2 * L + 3 * H1 + 2 * H2)
            T.AddPoint(X0 + W1 + W2, Y0 + 2 * L + 3 * H1 + H2)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + 2 * (W1 + W2)
            If X0 >= canvas1.ActualWidth + L Then
                X0 = -W1
                Y0 = Y0 + 2 * L + 4 * H1 + 2 * H2
                If Y0 >= canvas1.ActualHeight + L Then Exit Do
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a Spaced Dodecagon Tiling Pattern
    ''' </summary>
    Private Sub DodecagonTiling1(L As Double)
        Dim T As Tile
        Dim up As Boolean = True
        Dim H1 As Double = L * Math.Sin(60 * Math.PI / 180)
        Dim W1 As Double = L * Math.Cos(60 * Math.PI / 180)
        Dim H2 As Double = L * Math.Sin(30 * Math.PI / 180)
        Dim W2 As Double = L * Math.Cos(30 * Math.PI / 180)
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0, Y0)
            T.AddPoint(X0 + W2, Y0 - H2)
            T.AddPoint(X0 + L + W2, Y0 - H2)
            T.AddPoint(X0 + L + 2 * W2, Y0)
            T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + H1)
            T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + L + H1)
            T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1)
            T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0, Y0 + L + 2 * H1)
            T.AddPoint(X0 - W1, Y0 + L + H1)
            T.AddPoint(X0 - W1, Y0 + H1)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - W1, Y0 + L + H1)
            T.AddPoint(X0, Y0 + L + 2 * H1)
            T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 - W2 - W1, Y0 + L + H1 + H2)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + L + W2, Y0 + 2 * L + 2 * H1 + H2)
            T.AddPoint(X0 + W2, Y0 + 2 * L + 2 * H1 + H2)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + L + H1)
            T.AddPoint(X0 + L + 3 * W2 + W1, Y0 + L + H1 + H2)
            T.AddPoint(X0 + L + 3 * W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1)
            T.FillColor = Brushes.Yellow
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0, Y0 + L + 2 * H1)
            T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + W2, Y0 + 2 * L + 2 * H1 + H2)
            T.AddPoint(X0, Y0 + 2 * (L + H1 + H2))
            T.AddPoint(X0 - W2, Y0 + 2 * L + 2 * H1 + H2)
            T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1)
            T.AddPoint(X0 + L + 3 * W2, Y0 + L + 2 * H1 + H2)
            T.AddPoint(X0 + L + 3 * W2, Y0 + 2 * L + 2 * H1 + H2)
            T.AddPoint(X0 + L + 2 * W2, Y0 + 2 * (L + H1 + H2))
            T.AddPoint(X0 + L + W2, Y0 + 2 * L + 2 * H1 + H2)
            T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2)
            T.FillColor = Brushes.LightCyan
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + L + 3 * W2 + W1
            If up Then
                Y0 = Y0 - (L + H1 + H2)
                up = False
            Else
                Y0 = Y0 + (L + H1 + H2)
                up = True
            End If
            If X0 >= canvas1.ActualWidth + 2 * W2 Then
                X0 = 0
                If up Then
                    Y0 = Y0 + 2 * L + 2 * H1 + 2 * H2
                    If Y0 >= canvas1.ActualHeight + L + 2 * H1 + H2 Then Exit Do
                Else
                    Y0 = Y0 + 3 * L + 3 * H1 + 3 * H2
                    If Y0 >= canvas1.ActualHeight + 2 * H1 Then Exit Do
                End If
                up = True
            End If
        Loop
    End Sub

    ''' <summary>
    ''' Create a stacked Dodecagon and Triangle Tiling Pattern
    ''' </summary>
    Private Sub DodecagonTiling2(L As Double)
        Dim T As Tile
        Dim up As Boolean = True
        Dim H1 As Double = L * Math.Sin(Math.PI / 4)
        Dim H2 As Double = L * Math.Sin(15 * Math.PI / 180)
        Dim X0 As Double = 0.0
        Dim Y0 As Double = 0.0
        Do
            T = New Tile()
            T.AddPoint(X0 - H1, Y0 - 2 * (H1 + H2))
            T.AddPoint(X0 + H2, Y0 - 2 * H1 - H2)
            T.AddPoint(X0 + H1 + H2, Y0 - H1 - H2)
            T.AddPoint(X0 + H1 + 2 * H2, Y0)
            T.AddPoint(X0 + H1 + H2, Y0 + H1 + H2)
            T.AddPoint(X0 + H2, Y0 + 2 * H1 + H2)
            T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2))
            T.AddPoint(X0 - 2 * H1 - H2, Y0 + 2 * H1 + H2)
            T.AddPoint(X0 - 3 * H1 - H2, Y0 + H1 + H2)
            T.AddPoint(X0 - 3 * H1 - 2 * H2, Y0)
            T.AddPoint(X0 - 3 * H1 - H2, Y0 - H1 - H2)
            T.AddPoint(X0 - 2 * H1 - H2, Y0 - 2 * H1 - H2)
            T.FillColor = Brushes.LightGreen
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + H1 + 2 * H2, Y0)
            T.AddPoint(X0 + 2 * (H1 + H2), Y0 - H1)
            T.AddPoint(X0 + 3 * H1 + 2 * H2, Y0)
            T.AddPoint(X0 + 2 * (H1 + H2), Y0 + H1)
            T.FillColor = Brushes.Orange
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - 3 * H1 - 2 * H2, Y0)
            T.AddPoint(X0 - 3 * H1 - H2, Y0 + H1 + H2)
            T.AddPoint(X0 - 4 * H1 - 2 * H2, Y0 + H1)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - 2 * H1 - H2, Y0 + 2 * H1 + H2)
            T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2))
            T.AddPoint(X0 - 2 * H1, Y0 + 3 * H1 + 2 * H2)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2))
            T.AddPoint(X0 + H2, Y0 + +2 * H1 + H2)
            T.AddPoint(X0, Y0 + 3 * H1 + 2 * H2)
            T.FillColor = Brushes.Pink
            T.StarColor = StarColor
            Tiles.Add(T)
            T = New Tile()
            T.AddPoint(X0 + H1 + 2 * H2, Y0)
            T.AddPoint(X0 + 2 * (H1 + H2), Y0 + H1)
            T.AddPoint(X0 + H1 + H2, Y0 + H1 + H2)
            T.FillColor = Brushes.LightBlue
            T.StarColor = StarColor
            Tiles.Add(T)
            X0 = X0 + 3 * H1 + 2 * H2
            If up Then
                Y0 = Y0 - (3 * H1 + 2 * H2)
                up = False
            Else
                Y0 = Y0 + (3 * H1 + 2 * H2)
                up = True
            End If
            If X0 >= canvas1.ActualWidth + 4 * H1 Then
                X0 = 0

                If up Then
                    Y0 = Y0 + 6 * H1 + 4 * H2
                    If Y0 >= canvas1.ActualHeight + 3 * H1 Then Exit Do
                Else
                    Y0 = Y0 + 9 * H1 + 6 * H2
                    If Y0 >= canvas1.ActualHeight + 5 * H1 + 2 * H2 Then Exit Do
                End If
                up = True
            End If
        Loop
    End Sub

#End Region

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not App_Loaded Then Exit Sub
        DrawTiles()
    End Sub

    Private Sub Sld_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Loaded Then Exit Sub
        TileLength = SldHorSize.Value
        TileHeight = SldVertSize.Value
        DrawTiles()
    End Sub

    Private Sub SldDelta_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Loaded Then Exit Sub
        delta = SldDelta.Value
        DrawTiles()
    End Sub

    Private Sub SldAngle_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Loaded Then Exit Sub
        angle = SldAngle.Value
        DrawTiles()
    End Sub

    Private Sub CbShowStar_Click(sender As Object, e As RoutedEventArgs)
        DrawTiles()
    End Sub
End Class
