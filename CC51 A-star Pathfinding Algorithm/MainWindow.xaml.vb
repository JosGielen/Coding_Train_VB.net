Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 50
    Private App_Started As Boolean = False

    Private Rows As Integer = 40
    Private Cols As Integer = 40
    Private ConnChance As Integer = 25
    Private Nodes(Cols, Rows) As Node
    Private goal As Node
    Private current As Node
    Private Connections As List(Of Connection)

    Private CellWidth As Double
    Private CellHeight As Double
    Private NodeSize As Double = 6
    Private Rnd As Random = New Random()

    Private OpenSet As List(Of Node)
    Private ClosedSet As List(Of Node)
    Private my_Path As List(Of Node)
    Private DrawnPath As List(Of Line)
    Private DrawNodes As List(Of Ellipse)

#Region "Window Events"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If Not App_Started Then
            Init()
            'Start the search
            App_Started = True
            Start()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        App_Started = False
        End
    End Sub

#End Region

    Private Sub Init()
        CellWidth = Canvas1.ActualWidth / (Cols + 1)   'Leave half a cell width at the edges
        CellHeight = Canvas1.ActualHeight / (Rows + 1) 'so devide by Cols + 1 and Rows + 1
        'Initialize the Lists
        OpenSet = New List(Of Node)
        ClosedSet = New List(Of Node)
        DrawnPath = New List(Of Line)
        DrawNodes = New List(Of Ellipse)
        Connections = New List(Of Connection)
        'Set the goal
        goal = New Node(Cols, Rows, CellWidth, CellHeight)
        'Make the Nodes
        For I As Integer = 0 To Cols
            For J As Integer = 0 To Rows
                Nodes(I, J) = New Node(I, J, CellWidth, CellHeight)
            Next
        Next
        'Make random connections between Nodes
        For I As Integer = 0 To Cols
            For J As Integer = 0 To Rows
                If I > 0 And J > 0 Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I - 1, J - 1)))
                    End If
                End If
                If I > 0 Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I - 1, J)))
                    End If
                End If
                If I > 0 And J < Rows Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I - 1, J + 1)))
                    End If
                End If
                If J > 0 Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I, J - 1)))
                    End If
                End If
                If J < Rows Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I, J + 1)))
                    End If
                End If
                If I < Cols And J > 0 Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I + 1, J - 1)))
                    End If
                End If
                If I < Cols Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I + 1, J)))
                    End If
                End If
                If I < Cols And J < Rows Then
                    If Rnd.Next(100) < ConnChance Then
                        Connections.Add(New Connection(Nodes(I, J), Nodes(I + 1, J + 1)))
                    End If
                End If
            Next
        Next
        'Connect the Start and End Nodes to their neighbors
        If Not ConnectionExist(Nodes(0, 0), Nodes(0, 1)) Then Connections.Add(New Connection(Nodes(0, 0), Nodes(0, 1)))
        If Not ConnectionExist(Nodes(0, 0), Nodes(1, 1)) Then Connections.Add(New Connection(Nodes(0, 0), Nodes(1, 1)))
        If Not ConnectionExist(Nodes(0, 0), Nodes(1, 0)) Then Connections.Add(New Connection(Nodes(0, 0), Nodes(1, 0)))
        If Not ConnectionExist(Nodes(Cols, Rows), Nodes(Cols - 1, Rows)) Then Connections.Add(New Connection(Nodes(Cols, Rows), Nodes(Cols - 1, Rows)))
        If Not ConnectionExist(Nodes(Cols, Rows), Nodes(Cols - 1, Rows - 1)) Then Connections.Add(New Connection(Nodes(Cols, Rows), Nodes(Cols - 1, Rows - 1)))
        If Not ConnectionExist(Nodes(Cols, Rows), Nodes(Cols, Rows - 1)) Then Connections.Add(New Connection(Nodes(Cols, Rows), Nodes(Cols, Rows - 1)))

        'Draw the Nodes as small black circles
        Canvas1.Children.Clear()
        Dim ell As Ellipse
        For I As Integer = 0 To Cols
            For J As Integer = 0 To Rows
                ell = New Ellipse() With {
                    .Width = NodeSize,
                    .Height = NodeSize,
                    .Stroke = Brushes.Black,
                    .Fill = Brushes.Black,
                    .StrokeThickness = 1}
                Canvas.SetLeft(ell, Nodes(I, J).Location.X + CellWidth / 2 - NodeSize / 2)
                Canvas.SetTop(ell, Nodes(I, J).Location.Y + CellHeight / 2 - NodeSize / 2)
                DrawNodes.Add(ell)
                Canvas1.Children.Add(ell)
            Next
        Next

        'Draw the connections as black lines
        Dim l As Line
        For Each conn As Connection In Connections
            l = New Line() With {
                .X1 = conn.Node1.Location.X + CellWidth / 2,
                .Y1 = conn.Node1.Location.Y + CellHeight / 2,
                .X2 = conn.Node2.Location.X + CellWidth / 2,
                .Y2 = conn.Node2.Location.Y + CellHeight / 2,
                .Stroke = Brushes.Black,
                .StrokeThickness = 1}
            Canvas1.Children.Add(l)
        Next

        'Set the Start Node
        Nodes(0, 0).G = 0
        Nodes(0, 0).F = Nodes(0, 0).Distance(goal)
        OpenSet.Add(Nodes(0, 0))
    End Sub

    Private Sub Start()
        Title = "Searching."
        my_Path = New List(Of Node)
        Dim index As Integer = 0
        Dim MinIndex As Integer
        Dim tempG As Double
        While App_Started
            If OpenSet.Count > 0 Then
                MinIndex = 0
                For I As Integer = 0 To OpenSet.Count - 1
                    If OpenSet(I).F < OpenSet(MinIndex).F Then
                        MinIndex = I
                    End If
                Next
                current = OpenSet(MinIndex)
                If current.Location = goal.Location Then
                    'We found the goal!!
                    Title = "Goal was reached!!!"
                    'End the search
                    App_Started = False
                End If
                OpenSet.RemoveAt(MinIndex)
                ClosedSet.Add(current)
                'Color the current node Red
                index = current.Col * (Rows + 1) + current.Row
                DrawNodes(index).Fill = Brushes.Red
                DrawNodes(index).Stroke = Brushes.Red
                For Each n As Node In NodeConnections(current)
                    If Not ClosedSet.Contains(n) Then
                        tempG = current.G + current.Distance(n)
                        If OpenSet.Contains(n) Then
                            If tempG < n.G Then
                                n.G = tempG
                                n.F = n.G + n.Distance(goal)
                                n.Previous = current
                            End If
                        Else
                            n.G = tempG
                            n.F = n.G + n.Distance(goal)
                            n.Previous = current
                            OpenSet.Add(n)
                            'Color this Neighbor green
                            index = n.Col * (Rows + 1) + n.Row
                            DrawNodes(index).Fill = Brushes.LightGreen
                            DrawNodes(index).Stroke = Brushes.LightGreen
                        End If
                    End If
                Next
            Else
                'Unable to find the goal!
                Title = "The Goal can not be reached!!!"
                'End the search
                App_Started = False
            End If
            Render()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        End While
    End Sub

    Private Sub Render()
        Dim temp As Node
        Dim l As Line
        'Remove the previous path
        For Each ln As Line In DrawnPath
            Canvas1.Children.Remove(ln)
        Next
        'Get the current Path
        my_Path.Clear()
        temp = current
        my_Path.Add(temp)
        While temp.Previous IsNot Nothing
            my_Path.Add(temp.Previous)
            temp = temp.Previous
        End While
        'Draw the current Path
        DrawnPath.Clear()
        If my_Path.Count > 0 Then
            For I As Integer = 0 To my_Path.Count - 2
                l = New Line() With {
                    .X1 = my_Path(I).Location.X + CellWidth / 2,
                    .Y1 = my_Path(I).Location.Y + CellHeight / 2,
                    .X2 = my_Path(I + 1).Location.X + CellWidth / 2,
                    .Y2 = my_Path(I + 1).Location.Y + CellHeight / 2,
                    .Stroke = Brushes.Blue,
                    .StrokeThickness = 3}
                Canvas1.Children.Add(l)
                DrawnPath.Add(l)
            Next
        End If
    End Sub

#Region "Help Functions"

    Private Function ConnectionExist(node1 As Node, node2 As Node) As Boolean
        For Each conn As Connection In Connections
            If conn.Node1.Location = node1.Location And conn.Node2.Location = node2.Location Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function NodeConnections(n As Node) As List(Of Node)
        Dim connNodes As List(Of Node) = New List(Of Node)
        For Each conn As Connection In Connections
            If conn.Node1.Location = n.Location Then
                If Not connNodes.Contains(conn.Node2) Then connNodes.Add(conn.Node2)
            End If
            If conn.Node2.Location = n.Location Then
                If Not connNodes.Contains(conn.Node1) Then connNodes.Add(conn.Node1)
            End If
        Next
        Return connNodes
    End Function

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

#End Region

End Class
