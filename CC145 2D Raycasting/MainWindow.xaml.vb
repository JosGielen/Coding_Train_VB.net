Class MainWindow
    Private my_Camera As Camera
    Private CameraSpeed As Double = 2.0
    Private FrontWallDistance As Double
    Private BackWallDistance As Double
    Private my_Walls As List(Of Wall)
    Private my_MouseDown As Boolean = False
    Private RayCount As Integer
    Private WallLines As List(Of Line)
    Private FirstPersonViewScale As Double = 30
    Private Rnd As Random = New Random()
    'Maze Generator
    Private Building As Boolean
    Private AllowRandomRemoval As Boolean = True
    Private Size As Integer = 44
    Private Rows As Integer = 0
    Private Cols As Integer = 0
    Private Grid(,) As Cell
    Private CurrentCell As Cell
    Private NextCell As Cell
    Private CellStack As List(Of Cell)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        RayCount = CInt(canvas2.ActualWidth)
        my_Walls = New List(Of Wall)
        Dim w As Wall
        Dim WallColor As Brush = Brushes.DarkKhaki
        'Initialize the Maze generator used to make the walls
        Building = True
        Rows = CInt(Math.Floor(canvas1.ActualHeight / Size))
        Cols = CInt(Math.Floor(canvas1.ActualWidth / Size))
        ReDim Grid(Rows, Cols)
        CellStack = New List(Of Cell)
        For I As Integer = 0 To Rows - 1
            For J As Integer = 0 To Cols - 1
                Grid(I, J) = New Cell(I, J, Size)
                Grid(I, J).Draw(canvas1)
            Next
        Next
        CurrentCell = Grid(0, 0)
        CurrentCell.IsCurrent = True
        CellStack.Add(CurrentCell)
        'Make the outer walls
        w = New Wall(New Point(1, 1), New Point(Cols * Size - 1, 1), WallColor) 'Top wall
        my_Walls.Add(w)
        w = New Wall(New Point(1, 1), New Point(1, Rows * Size - 1), WallColor) 'Left Wall
        my_Walls.Add(w)
        w = New Wall(New Point(1, Rows * Size - 1), New Point(Cols * Size - 1, Rows * Size - 1), WallColor) 'Bottom wall
        my_Walls.Add(w)
        w = New Wall(New Point(Cols * Size - 1, 1), New Point(Cols * Size - 1, Rows * Size - 1), WallColor) 'Right wall
        my_Walls.Add(w)
        'Set the ground and sky in the First Person View
        Dim ground As Rectangle = New Rectangle() With
        {
            .Width = canvas2.ActualWidth,
            .Height = canvas2.ActualHeight / 2,
            .Fill = Brushes.LightBlue
        }
        ground.SetValue(Canvas.LeftProperty, 0.0)
        ground.SetValue(Canvas.TopProperty, 0.0)
        canvas2.Children.Add(ground)
        Dim Sky As Rectangle = New Rectangle() With
        {
            .Width = canvas2.ActualWidth,
            .Height = canvas2.ActualHeight / 2,
            .Fill = Brushes.Green
        }
        Sky.SetValue(Canvas.LeftProperty, 0.0)
        Sky.SetValue(Canvas.TopProperty, canvas2.ActualHeight / 2)
        canvas2.Children.Add(Sky)
        'Make the WallLines for the First Person View
        WallLines = New List(Of Line)
        Dim wl As Line
        For I As Integer = 0 To RayCount
            wl = New Line() With
            {
            .Stroke = Brushes.DarkGray,
            .StrokeThickness = 2.0,
            .X1 = I,
            .Y1 = 0.0,
            .X2 = I,
            .Y2 = canvas2.ActualHeight
            }
            WallLines.Add(wl)
            canvas2.Children.Add(wl)
        Next
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim w As Wall
        Dim WallColor As Brush = Brushes.White
        'Generate the maze
        If Building Then
            CurrentCell.IsVisited = True
            NextCell = GetUnvisitedNeighbour(CurrentCell)
            If NextCell IsNot Nothing Then
                RemoveWalls(CurrentCell, NextCell)
                CurrentCell.IsCurrent = False
                CurrentCell = NextCell
                CellStack.Add(CurrentCell)
            Else
                If CellStack.Count > 0 Then
                    CurrentCell.IsCurrent = False
                    CurrentCell = CellStack.Last
                    CellStack.RemoveAt(CellStack.Count - 1)
                Else 'THE MAZE IS FINISHED
                    Building = False
                    canvas1.Children.Clear()
                    'Make a camera
                    my_Camera = New Camera(New Point(Size / 2, Size / 2), 0.0, 45, RayCount)
                    my_Camera.Show(canvas1)
                    'Create the maze walls
                    For I As Integer = 0 To Rows - 1
                        For J As Integer = 0 To Cols - 1
                            If Grid(I, J).HasTopWall Then
                                WallColor = New SolidColorBrush(Colors.Brown)
                                w = New Wall(New Point(J * Size, I * Size), New Point((J + 1) * Size, I * Size), WallColor)
                                my_Walls.Add(w)
                            End If
                            If Grid(I, J).HasLeftWall Then
                                WallColor = New SolidColorBrush(Colors.DarkGray)
                                w = New Wall(New Point(J * Size, I * Size), New Point(J * Size, (I + 1) * Size), WallColor)
                                my_Walls.Add(w)
                            End If
                            If Grid(I, J).HasBottomWall Then
                                WallColor = New SolidColorBrush(Colors.Brown)
                                w = New Wall(New Point(J * Size, (I + 1) * Size), New Point((J + 1) * Size, (I + 1) * Size), WallColor)
                                my_Walls.Add(w)
                            End If
                            If Grid(I, J).HasRightWall Then
                                WallColor = New SolidColorBrush(Colors.DarkGray)
                                w = New Wall(New Point((J + 1) * Size, I * Size), New Point((J + 1) * Size, (I + 1) * Size), WallColor)
                                my_Walls.Add(w)
                            End If
                        Next
                    Next
                    'Show the walls
                    For I As Integer = 0 To my_Walls.Count - 1
                        my_Walls(I).Show(canvas1)
                    Next
                End If
            End If
            CurrentCell.IsCurrent = True
        Else
            'For each Ray calculate the closest Wall intersect
            Dim dist As Double
            Dim mindist As Double
            Dim intPt As Point
            Dim closestPt As Point
            Dim WallLineHeight As Double
            For I As Integer = 0 To my_Camera.Rays.Count - 1
                mindist = Double.MaxValue
                closestPt = New Point(-1, -1)
                For J As Integer = 0 To my_Walls.Count - 1
                    intPt = my_Walls(J).Intersect(my_Camera.Rays(I))
                    If intPt.X >= 0 And intPt.Y >= 0 Then
                        dist = Math.Sqrt((my_Camera.Pos.X - intPt.X) ^ 2 + (my_Camera.Pos.Y - intPt.Y) ^ 2)
                        If dist < mindist Then
                            mindist = dist
                            closestPt = intPt
                            wallColor = my_Walls(J).WallColor
                        End If
                    End If
                Next
                'End the ray at the closest intersect point
                my_Camera.Rays(I).X2 = closestPt.X
                my_Camera.Rays(I).Y2 = closestPt.Y
                'Set the wall height in the First Person View
                Dim rayAngleOffset As Double = Vector.AngleBetween(my_Camera.Rays(I).Dir, my_Camera.Dir) * Math.PI / 180
                WallLineHeight = FirstPersonViewScale * canvas2.ActualHeight / (mindist * Math.Abs(Math.Cos(rayAngleOffset)))
                WallLines(I).Y1 = (canvas2.ActualHeight - WallLineHeight) / 2
                WallLines(I).Y2 = (canvas2.ActualHeight + WallLineHeight) / 2
                WallLines(I).Stroke = wallColor
            Next
            'Calculate the camera front to Wall distance
            FrontWallDistance = Double.MaxValue
            Dim front As Point = my_Camera.Pos + my_Camera.Size * my_Camera.Dir
            For I As Integer = 0 To my_Walls.Count - 1
                dist = (my_Walls(I).Intersect(my_Camera.Rays(CInt(RayCount / 2))) - front).Length
                If dist < FrontWallDistance Then FrontWallDistance = dist
            Next
            'Calculate the camera back to Wall distance
            BackWallDistance = Double.MaxValue
            Dim back As Point = my_Camera.Pos - my_Camera.Size * my_Camera.Dir
            Dim backray As Ray = New Ray(my_Camera.Pos, my_Camera.Angle + 180)
            For I As Integer = 0 To my_Walls.Count - 1
                dist = (my_Walls(I).Intersect(backray) - back).Length
                If dist < BackWallDistance Then BackWallDistance = dist
            Next
        End If
    End Sub

    Private Function GetUnvisitedNeighbour(c As Cell) As Cell
        Dim Neighbours As List(Of Cell) = New List(Of Cell)
        Dim index As Integer
        If c.Col > 0 Then
            If Not Grid(c.Row, c.Col - 1).IsVisited Then Neighbours.Add(Grid(c.Row, c.Col - 1))
        End If
        If c.Col < Cols - 1 Then
            If Not Grid(c.Row, c.Col + 1).IsVisited Then Neighbours.Add(Grid(c.Row, c.Col + 1))
        End If
        If c.Row > 0 Then
            If Not Grid(c.Row - 1, c.Col).IsVisited Then Neighbours.Add(Grid(c.Row - 1, c.Col))
        End If
        If c.Row < Rows - 1 Then
            If Not Grid(c.Row + 1, c.Col).IsVisited Then Neighbours.Add(Grid(c.Row + 1, c.Col))
        End If
        If Neighbours.Count = 0 Then
            Return Nothing
        Else
            index = Rnd.Next(Neighbours.Count)
            Return Neighbours(index)
        End If
    End Function

    Private Sub RemoveWalls(Cell1 As Cell, Cell2 As Cell)
        If Cell1.Row > Cell2.Row Then
            Cell1.RemoveTopWall()
            Cell2.RemoveBottomWall()
        ElseIf Cell1.Row < Cell2.Row Then
            Cell1.RemoveBottomWall()
            Cell2.RemoveTopWall()
        ElseIf Cell1.Col < Cell2.Col Then
            Cell1.RemoveRightWall()
            Cell2.RemoveLeftWall()
        ElseIf Cell1.Col > Cell2.Col Then
            Cell1.RemoveLeftWall()
            Cell2.RemoveRightWall()
        End If
        If AllowRandomRemoval Then 'Remove some walls at random to create more open space
            If 100 * Rnd.NextDouble() < 5 Then
                Cell1.RemoveRightWall()
                Cell2.RemoveLeftWall()
            End If
            If 100 * Rnd.NextDouble() < 5 Then
                Cell1.RemoveTopWall()
                Cell2.RemoveBottomWall()
            End If
        End If

    End Sub


    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Up
                If FrontWallDistance > 1 Then my_Camera.Pos = my_Camera.Pos + CameraSpeed * my_Camera.Dir
            Case Key.Down
                If BackWallDistance > 1 Then my_Camera.Pos = my_Camera.Pos - CameraSpeed * my_Camera.Dir
            Case Key.Left
                my_Camera.Angle -= CameraSpeed
            Case Key.Right
                my_Camera.Angle += CameraSpeed
        End Select
        my_Camera.Update()
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
