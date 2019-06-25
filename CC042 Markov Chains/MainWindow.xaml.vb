Imports System.IO

Class MainWindow
    Private order As Integer = 4
    Private nGrams As List(Of String)
    Private possibilities As List(Of String)
    Private MaxLength As Integer = 350
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sr As StreamReader = New StreamReader(Environment.CurrentDirectory & "\Rainbow.txt")
        Dim Txt As String = sr.ReadToEnd()
        Dim gram As String = ""
        Dim index As Integer = 0
        nGrams = New List(Of String)
        possibilities = New List(Of String)
        For I As Integer = 0 To Txt.Length - order - 1
            gram = Txt.Substring(I, order)
            If Not nGrams.Contains(gram) Then
                nGrams.Add(gram)
                possibilities.Add(Txt.Substring(I + order, 1))
            Else
                index = nGrams.IndexOf(gram)
                possibilities(index) &= Txt.Substring(I + order, 1)
            End If
        Next
    End Sub

    Private Sub BtnGenerate_Click(sender As Object, e As RoutedEventArgs)
        Dim index As Integer = 0
        Dim nextIndex As Integer = 0
        Dim result As String = nGrams(0)
        Dim current As String = nGrams(0)
        For I As Integer = 0 To MaxLength - 1
            index = nGrams.IndexOf(current)
            nextIndex = Rnd.Next(possibilities(index).Length)
            result &= possibilities(index).Substring(nextIndex, 1)
            current = result.Substring(result.Length - order, order)
        Next
        Text1.Text = result
    End Sub
End Class
