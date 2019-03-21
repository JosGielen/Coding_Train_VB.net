Class MainWindow
    'General data
    Private Rnd As Random = New Random()
    Private locCount As Integer
    Private locations() As Point
    Private StartTime As Date
    Private App_Loaded As Boolean = False
    Private Rendering As Boolean = False
    Private initialised As Boolean = False
    'Genetic data
    Private popCount As Integer
    Private population As List(Of Integer())
    Private MutationRate As Integer = 20
    Private bestGeneration As Integer
    Private GenerationCounter As Integer
    Private fitness As List(Of Double)
    Private indices1() As Integer
    Private bestOrder() As Integer
    Private path1 As Polyline
    Private bestPath1 As Polyline
    Private bestDistance1 As Double
    'Lexicographic data
    Private Counter2 As Integer
    Private indices2() As Integer
    Private path2 As Polyline
    Private bestPath2 As Polyline
    Private bestDistance2 As Double
    Private TotalPermutations As Long
    Private findTime As String = ""
    Private finished As Boolean
    'Random solver data
    Private Counter3 As Integer
    Private path3 As Polyline
    Private bestPath3 As Polyline
    Private bestDistance3 As Double
    Private indices3() As Integer

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Create the locations
        locCount = 10
        popCount = 500
        TxtLocationCount.Text = locCount.ToString()
        Init()
        App_Loaded = True
    End Sub

    Private Sub Init()
        locCount = Integer.Parse(TxtLocationCount.Text)
        ReDim locations(locCount - 1)
        ReDim indices1(locCount - 1)
        ReDim indices2(locCount - 1)
        ReDim indices3(locCount - 1)
        For I As Integer = 0 To locCount - 1
            indices1(I) = I
        Next
        For I As Integer = 0 To locCount - 1
            locations(I) = New Point(5 + (canvas1.ActualWidth - 10) * Rnd.NextDouble(), 5 + (canvas1.ActualHeight - 10) * Rnd.NextDouble())
        Next
        canvas1.Children.Clear()
        canvas2.Children.Clear()
        canvas3.Children.Clear()
        '1 : GENETIC START
        'Initialize Genetic data
        Dim E1 As Ellipse
        For I As Integer = 0 To locCount - 1
            E1 = New Ellipse() With
            {
                .Width = 6,
                .Height = 6,
                .Stroke = Brushes.Black,
                .StrokeThickness = 1.0
            }
            E1.SetValue(Canvas.TopProperty, locations(I).Y - 3)
            E1.SetValue(Canvas.LeftProperty, locations(I).X - 3)
            canvas1.Children.Add(E1)
        Next
        population = New List(Of Integer())
        fitness = New List(Of Double)
        bestDistance1 = Double.MaxValue
        path1 = New Polyline() With
        {
        .Stroke = Brushes.Black,
        .StrokeThickness = 1
        }
        bestPath1 = New Polyline() With
        {
        .Stroke = Brushes.Red,
        .StrokeThickness = 2
        }
        'Make the first generation as random sequences
        For I As Integer = 0 To popCount - 1
            For J As Integer = 0 To indices1.Length - 1
                indices1(J) = J
            Next
            Dim a As Integer
            Dim b As Integer
            For J As Integer = 0 To 100
                a = Rnd.Next(locCount)
                b = Rnd.Next(locCount)
                SwapIndices(indices1, a, b)
            Next
            population.Add(indices1.Clone())
            fitness.Add(0.0)
        Next
        'Draw the first generation
        For I As Integer = 0 To locations.Count - 1
            bestPath1.Points.Add(locations(I))
            path1.Points.Add(locations(I))
        Next
        canvas1.Children.Add(bestPath1)
        canvas1.Children.Add(path1)
        GenerationCounter = 1
        bestGeneration = 1

        '2 : LEXICOGRAPHIC START
        'Initialize Lexicographic data
        For J As Integer = 0 To indices2.Length - 1
            indices2(J) = J
        Next
        For I As Integer = 0 To locCount - 1
            E1 = New Ellipse() With
            {
                .Width = 6,
                .Height = 6,
                .Stroke = Brushes.Black,
                .StrokeThickness = 1.0
            }
            E1.SetValue(Canvas.TopProperty, locations(I).Y - 3)
            E1.SetValue(Canvas.LeftProperty, locations(I).X - 3)
            canvas2.Children.Add(E1)
        Next
        TotalPermutations = fact(locCount)
        bestDistance2 = Double.MaxValue
        finished = False
        Counter2 = 0
        path2 = New Polyline() With
        {
        .Stroke = Brushes.Black,
        .StrokeThickness = 1
        }
        bestPath2 = New Polyline() With
        {
        .Stroke = Brushes.Red,
        .StrokeThickness = 2
        }
        'Draw the first generation
        For I As Integer = 0 To locations.Count - 1
            path2.Points.Add(locations(I))
            bestPath2.Points.Add(locations(I))
        Next
        canvas2.Children.Add(bestPath2)
        canvas2.Children.Add(path2)

        '3 : RANDOM START
        'Initialize random data
        For J As Integer = 0 To indices3.Length - 1
            indices3(J) = J
        Next
        For I As Integer = 0 To locCount - 1
            E1 = New Ellipse() With
            {
                .Width = 6,
                .Height = 6,
                .Stroke = Brushes.Black,
                .StrokeThickness = 1.0
            }
            E1.SetValue(Canvas.TopProperty, locations(I).Y - 3)
            E1.SetValue(Canvas.LeftProperty, locations(I).X - 3)
            canvas3.Children.Add(E1)
        Next
        bestDistance3 = Double.MaxValue
        Counter3 = 0
        path3 = New Polyline() With
        {
        .Stroke = Brushes.Black,
        .StrokeThickness = 1
        }
        bestPath3 = New Polyline() With
        {
        .Stroke = Brushes.Red,
        .StrokeThickness = 2
        }
        'Draw the first generation
        For I As Integer = 0 To locations.Count - 1
            path3.Points.Add(locations(I))
            bestPath3.Points.Add(locations(I))
        Next
        canvas3.Children.Add(bestPath3)
        canvas3.Children.Add(path3)
        initialised = True
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not Rendering Then Exit Sub
        '1 : GENETIC ALGORITHM
        'Evaluate every member of the current population and give it a Fitness value.
        Dim currentBestOrder1 As Integer() = population(0)
        Dim currentbestdist1 As Double = Double.MaxValue
        For I As Integer = 0 To popCount - 1
            Dim dist1 As Double = TotalDistance(population(I))
            fitness(I) = dist1
            'Find the best of the current population
            If dist1 < currentbestdist1 Then
                currentbestdist1 = dist1
                currentBestOrder1 = population(I)
            End If
            'Find the All-time best
            If dist1 < bestDistance1 Then
                bestDistance1 = dist1
                bestOrder = population(I)
                bestGeneration = GenerationCounter
                TxtResult1.Text = Math.Round(Math.Sqrt(bestDistance1), 2).ToString() & " found in generation " & GenerationCounter.ToString() & " after " & Math.Round((Now - StartTime).TotalSeconds, 2).ToString() & " seconds."
                'Show the all-time best path
                bestPath1.Points.Clear()
                For J As Integer = 0 To locations.Count - 1
                    bestPath1.Points.Add(locations(bestOrder(J)))
                Next
            End If
        Next
        Normalize(fitness)
        'Show the best of the current population
        path1.Points.Clear()
        For J As Integer = 0 To locations.Count - 1
            path1.Points.Add(locations(currentBestOrder1(J)))
        Next
        'Make the next generation
        Dim newPopulation As List(Of Integer()) = New List(Of Integer())
        If GenerationCounter - bestGeneration > 100 Then 'Stuck at local minimum
            newPopulation = Swapsequence(bestOrder)
            bestGeneration = GenerationCounter
        Else
            Dim newOrder As Integer()
            Dim orderA As Integer()
            Dim orderB As Integer()
            For I As Integer = 0 To population.Count - 1
                orderA = bestOrder
                orderB = PickOne()
                newOrder = CrossOver(orderA, orderB)
                Mutate(newOrder, MutationRate)
                newPopulation.Add(newOrder)
            Next
        End If
        population = newPopulation
        GenerationCounter += 1

        '2: LEXICOGRAPHIC ALGORITHM
        If Not finished Then
            Dim currentBestOrder2 As Integer() = indices2
            Dim currentbestdist2 As Double = Double.MaxValue
            For N As Integer = 0 To popCount - 1
                If FindNextPermutation(indices2) Then
                    Counter2 += 1
                    path2.Points.Clear()
                    For I As Integer = 0 To locations.Count - 1
                        path2.Points.Add(locations(indices2(I)))
                    Next
                    Dim dist2 As Double = TotalDistance(indices2)
                    'Find the best of the current loop
                    If dist2 < currentbestdist2 Then
                        currentbestdist2 = dist2
                        currentBestOrder2 = indices2
                    End If
                    'Find the All-time best
                    If dist2 < bestDistance2 Then
                        bestDistance2 = dist2
                        findTime = Math.Round(Math.Sqrt(bestDistance2), 2).ToString() & " found after " & Math.Round((Now - StartTime).TotalSeconds, 2).ToString() & " seconds.  ("
                        bestPath2.Points.Clear()
                        For I As Integer = 0 To locations.Count - 1
                            bestPath2.Points.Add(locations(indices2(I)))
                        Next
                    End If
                Else
                    finished = True
                End If
            Next
            TxtResult2.Text = findTime & Math.Round(100 * Counter2 / TotalPermutations, 5).ToString() & "% completed. )"
            'Show the best of the current loop
            path2.Points.Clear()
            For J As Integer = 0 To locations.Count - 1
                path2.Points.Add(locations(currentBestOrder2(J)))
            Next
        Else
            TxtResult2.Text = findTime & "Finished. )"
            StopRender()
        End If

        '3 : RANDOM ALGORITHM
        Dim currentBestOrder3 As Integer() = indices3
        Dim currentbestdist3 As Double = Double.MaxValue
        Dim a As Integer
        Dim b As Integer
        For N As Integer = 0 To popCount - 1
            Counter3 += 1
            a = Rnd.Next(locations.Count)
            b = Rnd.Next(locations.Count)
            SwapIndices(indices3, a, b)
            Dim dist3 As Double = TotalDistance(indices3)
            'Find the best of the current loop
            If dist3 < currentbestdist3 Then
                currentbestdist3 = dist3
                currentBestOrder3 = indices3
            End If
            'Find the All-time best
            If dist3 < bestDistance3 Then
                bestDistance3 = dist3
                bestPath3.Points.Clear()
                For I As Integer = 0 To locations.Count - 1
                    bestPath3.Points.Add(locations(indices3(I)))
                Next
                TxtResult3.Text = Math.Round(Math.Sqrt(bestDistance3), 2).ToString() & " found after " & Math.Round((Now - StartTime).TotalSeconds, 2).ToString() & " seconds."
            End If
        Next
        'Show the best of the current loop
        path3.Points.Clear()
        For I As Integer = 0 To locations.Count - 1
            path3.Points.Add(locations(currentBestOrder3(I)))
        Next
        Me.Title = "Iteration " & GenerationCounter.ToString()
        initialised = False
    End Sub

    Private Sub Normalize(list As List(Of Double))
        'scale the values reversed
        Dim min As Double = list.Min
        Dim max As Double = list.Max
        For I As Integer = 0 To list.Count - 1
            list(I) = 1 - ((list(I) - min + 1) / (max - min))
        Next
        'Normalize the values
        Dim sum As Double = 0.0
        For I As Integer = 0 To list.Count - 1
            sum += list(I)
        Next
        For I As Integer = 0 To list.Count - 1
            list(I) = list(I) / sum
        Next
    End Sub

    Private Function PickOne() As Integer()
        Dim index As Integer = 0
        Dim r As Double = Rnd.NextDouble()
        While r > 0
            r = r - fitness(index)
            index += 1
        End While
        index -= 1
        Return population(index)
    End Function

    Private Function CrossOver(A As Integer(), B As Integer()) As Integer()
        Dim startindex As Integer = Rnd.Next(A.Length)
        Dim endindex As Integer = Rnd.Next(startindex + 1, A.Length)
        Dim num As Integer = endindex - startindex
        Dim neworder(A.Length - 1) As Integer
        For I As Integer = 0 To neworder.Length - 1
            neworder(I) = -1
        Next
        Array.Copy(A, startindex, neworder, 0, num)
        For I As Integer = 0 To B.Length - 1
            If Not neworder.Contains(B(I)) Then
                neworder(num) = B(I)
                num += 1
            End If
        Next
        Return neworder
    End Function

    Private Function Mutate(order As Integer(), mutationRate As Double) As Integer()
        For I As Integer = 0 To order.Length - 1
            If 100 * Rnd.NextDouble() < mutationRate Then
                Dim a As Integer = Rnd.Next(order.Count)
                Dim b As Integer = Rnd.Next(order.Count)
                Dim temp As Integer = order(a)
                order(a) = order(b)
                order(b) = temp
            End If
        Next
        Return order
    End Function

    Private Function SwapsequenceX(best As Integer()) As List(Of Integer())
        Dim result As List(Of Integer()) = New List(Of Integer())
        Dim neworder As Integer()
        'Swap elements at distance = 1 then 2 then 3 .... till swap first and last.
        For N As Integer = 1 To locations.Count - 1
            For I As Integer = 0 To locations.Count - (N + 1)
                neworder = best.Clone()
                Dim temp As Integer = neworder(I)
                neworder(I) = neworder(I + N)
                neworder(I + N) = temp
                If result.Count < population.Count Then
                    result.Add(neworder)
                Else
                    Return result
                End If
            Next
        Next
        'Keep the rest of the population intact
        If result.Count < population.Count Then
            For I As Integer = result.Count To population.Count - 1
                result.Add(population(I))
            Next
        End If
        Return result
    End Function

    Private Function Swapsequence(best As Integer()) As List(Of Integer())
        Dim result As List(Of Integer()) = New List(Of Integer())
        Dim neworder As Integer()
        'Keep 1 copy of the best
        result.Add(best.Clone())
        'Reverse parts of the best array with length = 1 then 2 then 3 ....
        For N As Integer = 2 To locations.Count - 1
            For I As Integer = 0 To locations.Count - (N + 1)
                neworder = best.Clone()
                Array.Reverse(neworder, I, N)
                If result.Count < population.Count Then
                    result.Add(neworder)
                Else
                    Return result
                End If
            Next
        Next
        'Swap elements of the best array at distance = 1 then 2 then 3 .... till swap first and last.
        For N As Integer = 1 To locations.Count - 1
            For I As Integer = 0 To locations.Count - (N + 1)
                neworder = best.Clone()
                Dim temp As Integer = neworder(I)
                neworder(I) = neworder(I + N)
                neworder(I + N) = temp
                If result.Count < population.Count Then
                    result.Add(neworder)
                Else
                    Return result
                End If
            Next
        Next
        'Keep the rest of the population intact
        If result.Count < population.Count Then
            For I As Integer = result.Count To population.Count - 1
                result.Add(population(I))
            Next
        End If
        Return result
    End Function

    Private Sub SwapIndices(ind As Integer(), a As Integer, b As Integer)
        Dim temp As Integer = ind(a)
        ind(a) = ind(b)
        ind(b) = temp
    End Sub

    Private Function TotalDistance(ind As Integer())
        Dim result As Double = 0.0
        For I As Integer = 0 To locations.Count - 2
            result += (locations(ind(I)).X - locations(ind(I + 1)).X) ^ 2 + (locations(ind(I)).Y - locations(ind(I + 1)).Y) ^ 2
        Next
        Return result
    End Function

    Private Function FindNextPermutation(A As Integer()) As Boolean
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim dummy As Integer = 0
        X = -1
        'Find largest k with A(k) < A(k+1)
        For J As Integer = A.GetUpperBound(0) - 1 To 0 Step -1
            If A(J) < A(J + 1) Then
                X = J
                Exit For
            End If
        Next
        If X = -1 Then 'No more permutations available
            Return False
        End If
        'Find largest l with A(l) > A(k)
        For J As Integer = A.GetUpperBound(0) To X + 1 Step -1
            If A(J) > A(X) Then
                Y = J
                Exit For
            End If
        Next
        'Swap A(k) and A(l)
        dummy = A(X)
        A(X) = A(Y)
        A(Y) = dummy
        'Reverse array A(k+1) to A(N)
        Array.Reverse(A, X + 1, A.GetUpperBound(0) - X)
        Return True
    End Function

    Private Function fact(ByVal n As Integer) As Long
        Dim result As Long = 1
        For I As Integer = 1 To n
            result *= I
        Next
        Return result
    End Function

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub

    Private Sub StartRender()
        If Not initialised Then Init()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "STOP"
        StartTime = Now()
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "START"
        Rendering = False
    End Sub

    Private Sub TxtLocationCount_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TxtLocationCount.TextChanged
        If Not App_Loaded Then Exit Sub
        locCount = Integer.Parse(TxtLocationCount.Text)
        initialised = False
    End Sub
End Class


