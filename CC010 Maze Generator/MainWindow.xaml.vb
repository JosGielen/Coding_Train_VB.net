Imports System.Threading

Class MainWindow
    Private WaitTime As Integer = 0
    Private Size As Double = 15.0
    Private Rows As Integer = 0
    Private Cols As Integer = 0
    Private Grid(,) As Cell
    Private CurrentCell As Cell
    Private NextCell As Cell
    Private CellStack As List(Of Cell)
    Private Rnd As Random = New Random()
    Private AllowRandomRemoval As Boolean = False
    Private Rendering As Boolean

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Rows = Math.Floor(canvas1.ActualHeight / Size)
        Cols = Math.Floor(canvas1.ActualWidth / Size)
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
        StartRender()
    End Sub

    Private Sub StartRender()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = True
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        'Generate the maze
        If Not Rendering Then Exit Sub
        Wait()
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
            Else
                Exit Sub
            End If
        End If
        CurrentCell.IsCurrent = True
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
        ElseIf cell1.Col > Cell2.Col Then
            Cell1.RemoveLeftWall()
            Cell2.RemoveRightWall()
        End If
        If AllowRandomRemoval Then
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

    Private Sub Wait()
        Thread.Sleep(WaitTime)
    End Sub

End Class
