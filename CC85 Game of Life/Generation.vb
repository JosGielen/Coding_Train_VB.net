Public Class Generation
    Private Cells(,) As Boolean
    Private Dummy(,) As Boolean
    Private m_Volgnummer As Long
    Private m_Breedte As Integer
    Private m_Hoogte As Integer

    Public Sub New()
        ReDim Cells(199, 199)
        ReDim Dummy(199, 199)
        For X As Integer = 0 To 199
            For Y As Integer = 0 To 199
                Cells(X, Y) = False
            Next
        Next X
        m_Volgnummer = 1
        m_Breedte = 200
        m_Hoogte = 200
    End Sub

    Public Sub New(ByVal Breedte As Integer, ByVal Hoogte As Integer)
        ReDim Cells(Breedte - 1, Hoogte - 1)
        ReDim Dummy(Breedte - 1, Hoogte - 1)
        For X As Integer = 0 To Breedte - 1
            For Y As Integer = 0 To Hoogte - 1
                Cells(X, Y) = False
            Next
        Next X
        m_Volgnummer = 1
        m_Breedte = Breedte
        m_Hoogte = Hoogte
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

    Public Property Cell(ByVal X As Integer, ByVal Y As Integer) As Boolean
        Get
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                Return Cells(X, Y)
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            If X >= 0 And X < m_Breedte And Y >= 0 And Y < m_Hoogte Then
                Cells(X, Y) = value
            Else
                Cells(X, Y) = False
            End If
        End Set
    End Property

    Public Sub update()
        'Use Game of Life rules to create a new generation in Dummy
        Dim buren As Integer
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                buren = 0
                If X > 0 And Y > 0 Then
                    If Cells(X - 1, Y - 1) Then buren = buren + 1
                End If
                If X > 0 Then
                    If Cells(X - 1, Y) Then buren = buren + 1
                End If
                If X > 0 And Y < m_Hoogte - 1 Then
                    If Cells(X - 1, Y + 1) Then buren = buren + 1
                End If
                If Y > 0 Then
                    If Cells(X, Y - 1) Then buren = buren + 1
                End If
                If Y < m_Hoogte - 1 Then
                    If Cells(X, Y + 1) Then buren = buren + 1
                End If
                If X < m_Breedte - 1 And Y > 0 Then
                    If Cells(X + 1, Y - 1) Then buren = buren + 1
                End If
                If X < m_Breedte - 1 Then
                    If Cells(X + 1, Y) Then buren = buren + 1
                End If
                If X < m_Breedte - 1 And Y < m_Hoogte - 1 Then
                    If Cells(X + 1, Y + 1) Then buren = buren + 1
                End If
                Dummy(X, Y) = Cells(X, Y)
                If Dummy(X, Y) = True And buren < 2 Then Dummy(X, Y) = False
                If Dummy(X, Y) = True And buren > 3 Then Dummy(X, Y) = False
                If Dummy(X, Y) = False And buren = 3 Then Dummy(X, Y) = True
            Next
        Next X

        'Copy Dummy back to Cells
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                Cells(X, Y) = Dummy(X, Y)
            Next Y
        Next X
        m_Volgnummer = m_Volgnummer + 1
    End Sub

    Public Sub Clear()
        For X As Integer = 0 To m_Breedte - 1
            For Y As Integer = 0 To m_Hoogte - 1
                Cells(X, Y) = False
            Next Y
        Next X
    End Sub
End Class
