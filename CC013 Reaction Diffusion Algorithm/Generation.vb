Public Class Generation
    Private CellsA(,) As Double
    Private CellsB(,) As Double
    Private NextA(,) As Double
    Private NextB(,) As Double
    Private m_Volgnummer As Long
    Private m_Breedte As Integer
    Private m_Hoogte As Integer
    Private m_DiffA As Double
    Private m_DiffB As Double
    Private m_Feed As Double
    Private m_Kill As Double

    Public Sub New(ByVal Breedte As Integer, ByVal Hoogte As Integer)
        ReDim CellsA(Breedte - 1, Hoogte - 1)
        ReDim CellsB(Breedte - 1, Hoogte - 1)
        ReDim NextA(Breedte - 1, Hoogte - 1)
        ReDim NextB(Breedte - 1, Hoogte - 1)
        For X As Integer = 0 To Breedte - 1
            For Y As Integer = 0 To Hoogte - 1
                CellsA(X, Y) = 1.0
                CellsB(X, Y) = 0.0
            Next
        Next
        m_Volgnummer = 1
        m_Breedte = Breedte
        m_Hoogte = Hoogte
        'Appy a seed area
        For X As Integer = Breedte / 2 - 5 To Breedte / 2 + 5
            For Y As Integer = Hoogte / 2 - 5 To Hoogte / 2 + 5
                CellsB(X, Y) = 1.0
            Next
        Next
    End Sub

    Public ReadOnly Property Volgnummer() As Long
        Get
            Return m_Volgnummer
        End Get
    End Property

    Public ReadOnly Property Breedte() As Integer
        Get
            Return m_Breedte
        End Get
    End Property
    Public ReadOnly Property Hoogte() As Integer
        Get
            Return m_Hoogte
        End Get
    End Property

    Public Property CellA(ByVal X As Integer, ByVal Y As Integer) As Double
        Get
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                Return CellsA(X, Y)
            Else
                Return 1.0
            End If
        End Get
        Set(ByVal value As Double)
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                CellsA(X, Y) = value
            Else
                CellsA(X, Y) = 1.0
            End If
        End Set
    End Property

    Public Property CellB(ByVal X As Integer, ByVal Y As Integer) As Double
        Get
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                Return CellsB(X, Y)
            Else
                Return 0.0
            End If
        End Get
        Set(ByVal value As Double)
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                CellsB(X, Y) = value
            Else
                CellsB(X, Y) = 0.0
            End If
        End Set
    End Property

    Public Property DiffA As Double
        Get
            Return m_DiffA
        End Get
        Set(value As Double)
            m_DiffA = value
        End Set
    End Property

    Public Property DiffB As Double
        Get
            Return m_DiffB
        End Get
        Set(value As Double)
            m_DiffB = value
        End Set
    End Property

    Public Property Feed As Double
        Get
            Return m_Feed
        End Get
        Set(value As Double)
            m_Feed = value
        End Set
    End Property

    Public Property Kill As Double
        Get
            Return m_Kill
        End Get
        Set(value As Double)
            m_Kill = value
        End Set
    End Property

    Public Sub update()
        'Use the Diffusion-Reaction algorithm to create a new generation in NextA and NextB
        Dim LaplaceA As Double
        Dim LaplaceB As Double
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                'Calculate the Laplacian 3x3 convolution for each cell A and B
                LaplaceA = -1.0 * CellsA(X, Y)
                LaplaceB = -1.0 * CellsB(X, Y)
                If X > 0 And Y > 0 Then
                    LaplaceA += 0.05 * CellsA(X - 1, Y - 1)
                    LaplaceB += 0.05 * CellsB(X - 1, Y - 1)
                End If
                If X > 0 Then
                    LaplaceA += 0.2 * CellsA(X - 1, Y)
                    LaplaceB += 0.2 * CellsB(X - 1, Y)
                End If
                If X > 0 And Y < m_Hoogte - 1 Then
                    LaplaceA += 0.05 * CellsA(X - 1, Y + 1)
                    LaplaceB += 0.05 * CellsB(X - 1, Y + 1)
                End If
                If Y > 0 Then
                    LaplaceA += 0.2 * CellsA(X, Y - 1)
                    LaplaceB += 0.2 * CellsB(X, Y - 1)
                End If
                If Y < m_Hoogte - 1 Then
                    LaplaceA += 0.2 * CellsA(X, Y + 1)
                    LaplaceB += 0.2 * CellsB(X, Y + 1)
                End If
                If X < m_Breedte - 1 And Y > 0 Then
                    LaplaceA += 0.05 * CellsA(X + 1, Y - 1)
                    LaplaceB += 0.05 * CellsB(X + 1, Y - 1)
                End If
                If X < m_Breedte - 1 Then
                    LaplaceA += 0.2 * CellsA(X + 1, Y)
                    LaplaceB += 0.2 * CellsB(X + 1, Y)
                End If
                If X < m_Breedte - 1 And Y < m_Hoogte - 1 Then
                    LaplaceA += 0.05 * CellsA(X + 1, Y + 1)
                    LaplaceB += 0.05 * CellsB(X + 1, Y + 1)
                End If
                'Calculate NextA and NextB
                NextA(X, Y) = CellsA(X, Y) + m_DiffA * LaplaceA - CellsA(X, Y) * CellsB(X, Y) * CellsB(X, Y) + Feed * (1 - CellsA(X, Y))
                NextB(X, Y) = CellsB(X, Y) + m_DiffB * LaplaceB + CellsA(X, Y) * CellsB(X, Y) * CellsB(X, Y) - (Kill + Feed) * CellsB(X, Y)
            Next
        Next
        'Copy Next back to Cells
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                CellsA(X, Y) = NextA(X, Y)
                CellsB(X, Y) = NextB(X, Y)
            Next
        Next
        m_Volgnummer = m_Volgnummer + 1
    End Sub

    Public Sub Clear()
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                CellsA(X, Y) = 1.0
                CellsB(X, Y) = 0.0
            Next
        Next
    End Sub
End Class
