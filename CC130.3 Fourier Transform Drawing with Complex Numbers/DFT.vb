Public Class DFT

    Public Shared Function Process(signal As List(Of Double)) As List(Of Vector)
        Dim N As Integer = signal.Count
        Dim result As List(Of Vector) = New List(Of Vector)
        Dim Re As Double
        Dim Im As Double
        Dim alfa As Double
        For K As Integer = 0 To signal.Count - 1
            Re = 0.0
            Im = 0.0
            For I As Integer = 0 To N - 1
                alfa = 2 * Math.PI * K * I / N
                Re += signal(I) * Math.Cos(alfa)
                Im += -1 * signal(I) * Math.Sin(alfa)
            Next
            result.Add(New Vector(Re / N, Im / N))
        Next
        Return result
    End Function

    Public Shared Function GetEpicycles(signal As List(Of Double)) As List(Of Epicycle)
        Dim N As Integer = signal.Count
        Dim result As List(Of Epicycle) = New List(Of Epicycle)
        Dim Re As Double
        Dim Im As Double
        Dim alfa As Double
        For K As Integer = 0 To signal.Count - 1
            Re = 0.0
            Im = 0.0
            For I As Integer = 0 To N - 1
                alfa = 2 * Math.PI * K * I / N
                Re += signal(I) * Math.Cos(alfa)
                Im += -1 * signal(I) * Math.Sin(alfa)
            Next
            result.Add(New Epicycle(K, New Vector(Re / N, Im / N)))
        Next
        Return result
    End Function

    Public Shared Function Process(signal As List(Of Point)) As List(Of Vector)
        Dim N As Integer = signal.Count
        Dim result As List(Of Vector) = New List(Of Vector)
        Dim Re As Double
        Dim Im As Double
        Dim a As Double
        Dim b As Double
        Dim c As Double
        Dim d As Double
        Dim alfa As Double
        For K As Integer = 0 To signal.Count - 1
            Re = 0.0
            Im = 0.0
            For I As Integer = 0 To N - 1
                alfa = 2 * Math.PI * K * I / N
                a = signal(I).X
                b = signal(I).Y
                c = Math.Cos(alfa)
                d = -1 * Math.Sin(alfa)
                '(a+bi)*(c+di) = (ac-bd) + (ad+bc)i
                Re = Re + (a * c - b * d)
                Im = Im + (a * d + b * c)
            Next
            result.Add(New Vector(Re / N, Im / N))
        Next
        Return result
    End Function


    Public Shared Function GetEpicycles(signal As List(Of Point)) As List(Of Epicycle)
        Dim N As Integer = signal.Count
        Dim result As List(Of Epicycle) = New List(Of Epicycle)
        Dim Re As Double
        Dim Im As Double
        Dim a As Double
        Dim b As Double
        Dim c As Double
        Dim d As Double
        Dim alfa As Double
        For K As Integer = 0 To signal.Count - 1
            Re = 0.0
            Im = 0.0
            For I As Integer = 0 To N - 1
                alfa = 2 * Math.PI * K * I / N
                a = signal(I).X
                b = signal(I).Y
                c = Math.Cos(alfa)
                d = -1 * Math.Sin(alfa)
                '(a+bi)*(c+di) = (ac-bd) + (ad+bc)i
                Re = Re + (a * c - b * d)
                Im = Im + (a * d + b * c)
            Next
            result.Add(New Epicycle(K, New Vector(Re / N, Im / N)))
        Next
        Return result
    End Function

    Public Shared Function GetSortedEpicycles(signal As List(Of Double), ascendingOrder As Boolean) As List(Of Epicycle)
        Dim result As List(Of Epicycle) = GetEpicycles(signal)
        SortEpicycles(result, ascendingOrder)
        Return result
    End Function

    Public Shared Function GetSortedEpicycles(signal As List(Of Point), ascendingOrder As Boolean) As List(Of Epicycle)
        Dim result As List(Of Epicycle) = GetEpicycles(signal)
        SortEpicycles(result, ascendingOrder)
        Return result
    End Function

    Private Shared Sub SortEpicycles(epi As List(Of Epicycle), ascendingOrder As Boolean)
        Dim dummyFreq As Double
        Dim dummyAmp As Double
        Dim dummyPhase As Double
        For I As Integer = 0 To epi.Count - 1
            For J As Integer = I + 1 To epi.Count - 1
                If ascendingOrder Then
                    If epi(J).Amplitude < epi(I).Amplitude Then
                        dummyFreq = epi(I).Freqency
                        dummyAmp = epi(I).Amplitude
                        dummyPhase = epi(I).Phase
                        epi(I).Freqency = epi(J).Freqency
                        epi(I).Amplitude = epi(J).Amplitude
                        epi(I).Phase = epi(J).Phase
                        epi(J).Freqency = dummyFreq
                        epi(J).Amplitude = dummyAmp
                        epi(J).Phase = dummyPhase
                    End If
                Else
                    If epi(J).Amplitude > epi(I).Amplitude Then
                        dummyFreq = epi(I).Freqency
                        dummyAmp = epi(I).Amplitude
                        dummyPhase = epi(I).Phase
                        epi(I).Freqency = epi(J).Freqency
                        epi(I).Amplitude = epi(J).Amplitude
                        epi(I).Phase = epi(J).Phase
                        epi(J).Freqency = dummyFreq
                        epi(J).Amplitude = dummyAmp
                        epi(J).Phase = dummyPhase
                    End If
                End If
            Next
        Next
    End Sub

End Class

'==================================================================================================================

Public Class Epicycle
    Public Freqency As Double
    Public Amplitude As Double
    Public Phase As Double

    Public Sub New(Freq As Double, V As Vector)
        Freqency = Freq
        Amplitude = V.Length
        Phase = Math.Atan2(V.Y, V.X)
    End Sub
End Class


