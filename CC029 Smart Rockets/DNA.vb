Public Class DNA
    Private my_Genes As List(Of Gene)

    Public Sub New(lifespan As Integer, rnd As Random)
        Dim g As Gene
        my_Genes = New List(Of Gene)
        For I As Integer = 0 To lifespan - 1
            g = New Gene()
            my_Genes.Add(g)
        Next
    End Sub

    Public Sub New(mate1 As DNA, mate2 As DNA, randomchoice As Boolean, allowMutation As Boolean, lifespan As Integer, rnd As Random)
        my_Genes = New List(Of Gene)
        If randomchoice Then
            'Method1 : Random choice
            Dim dummy As Double
            For I As Integer = 0 To lifespan - 1
                dummy = rnd.NextDouble()
                If dummy < 0.5 Then
                    my_Genes.Add(mate1.Genes(I))
                Else
                    my_Genes.Add(mate2.Genes(I))
                End If
            Next
        Else
            'Method2 : Use a midpoint to devide the genes
            Dim midPt As Integer = rnd.Next(lifespan)
            For I As Integer = 0 To lifespan - 1
                If I < midPt Then
                    my_Genes.Add(mate1.Genes(I))
                Else
                    my_Genes.Add(mate2.Genes(I))
                End If
            Next
        End If
        'Mutation
        If allowMutation Then
            For I As Integer = 0 To lifespan - 1
                If rnd.NextDouble() < 0.01 Then
                    my_Genes(I) = New Gene()
                End If
            Next
        End If
    End Sub

    Public ReadOnly Property Genes As List(Of Gene)
        Get
            Return my_Genes
        End Get
    End Property
End Class
