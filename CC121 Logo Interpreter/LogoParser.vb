Public Class LogoParser
    Private my_Input As String = ""
    Private my_Commands As List(Of LogoCommand) = New List(Of LogoCommand)

    Public Property Input As String
        Get
            Return my_Input
        End Get
        Set(value As String)
            my_Input = value
        End Set
    End Property

    Public Sub Parse()
        my_Commands = New List(Of LogoCommand)
        Parse(my_Input)
    End Sub

    Public ReadOnly Property Commands As List(Of LogoCommand)
        Get
            Return my_Commands
        End Get
    End Property

    Private Sub Parse(txt As String)
        Dim cmd As LogoCommand
        Dim index As Integer = 0
        Dim substring As String = ""
        Dim my_Txt = RemoveSpaces(txt)
        While index < my_Txt.Length
            If index < my_Txt.Length - 6 Then
                If my_Txt.Substring(index, 6) = "repeat" Then
                    Dim value As Double = GetValue(my_Txt, index + 6)
                    If value >= 0 Then
                        Dim parameter As String = GetParameter(my_Txt, index + 6 + value.ToString().Length)
                        If parameter <> "" Then
                            index += value.ToString.Length + parameter.Length + 5
                            For I As Integer = 0 To value - 1
                                Parse(parameter)
                            Next
                        End If
                    End If
                End If
            End If
            If index < my_Txt.Length - 4 Then
                substring = my_Txt.Substring(index, 4)
                If substring = "setX" Then
                    Dim value As Integer = GetValue(my_Txt, index + 4)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                                    .Command = "setX",
                                    .Value = value
                                }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 3
                    End If
                ElseIf substring = "setY" Then
                    Dim value As Integer = GetValue(my_Txt, index + 4)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                                    .Command = "setY",
                                    .Value = value
                                }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 3
                    End If
                ElseIf substring = "size" Then
                    Dim value As Integer = GetValue(my_Txt, index + 4)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                                    .Command = "size",
                                    .Value = value
                                }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 3
                    End If
                ElseIf substring = "home" Then
                    cmd = New LogoCommand() With {
                            .Command = "home",
                            .Value = 0
                        }
                    my_Commands.Add(cmd)
                    index += 3
                End If
            End If
            If index < my_Txt.Length - 3 Then
                If my_Txt.Substring(index, 3) = "col" Then
                    Dim parameter As String = GetParameter(my_Txt, index + 3)
                    If parameter <> "" Then
                        cmd = New LogoCommand() With {
                            .Command = "col",
                            .Value = 0,
                            .Parameter = parameter
                        }
                        my_Commands.Add(cmd)
                        index += parameter.Length + 3
                    End If
                End If
            End If
            If index < my_Txt.Length - 2 Then
                substring = my_Txt.Substring(index, 2)
                If substring = "fd" Then
                    Dim value As Integer = GetValue(my_Txt, index + 2)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                            .Command = "fd",
                            .Value = value
                        }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 1
                    End If
                ElseIf substring = "bd" Then
                    Dim value As Integer = GetValue(my_Txt, index + 2)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                            .Command = "bd",
                            .Value = value
                        }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 1
                    End If
                ElseIf substring = "rt" Then
                    Dim value As Integer = GetValue(my_Txt, index + 2)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                            .Command = "rt",
                            .Value = value
                        }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 1
                    End If
                ElseIf substring = "lt" Then
                    Dim value As Integer = GetValue(my_Txt, index + 2)
                    If value >= 0 Then
                        cmd = New LogoCommand() With {
                            .Command = "lt",
                            .Value = value
                        }
                        my_Commands.Add(cmd)
                        index += value.ToString.Length + 1
                    End If
                ElseIf substring = "pu" Then
                    cmd = New LogoCommand() With {
                        .Command = "pu",
                        .Value = 0
                    }
                    my_Commands.Add(cmd)
                    index += 1
                ElseIf substring = "pd" Then
                    cmd = New LogoCommand() With {
                        .Command = "pd",
                        .Value = 0
                    }
                    my_Commands.Add(cmd)
                    index += 1
                End If
            End If
            index += 1
        End While
    End Sub

    Private Function RemoveSpaces(txt As String) As String
        Dim result As String = ""
        Dim index As Integer = 0
        While index < txt.Length
            If txt(index) <> " "c Then result &= txt(index) 'Skip spaces
            index += 1
        End While
        Return result
    End Function

    Private Function GetValue(txt As String, index As Integer) As Integer
        Dim result As Integer = -1
        Dim test As Integer = 0
        Dim I As Integer = 1
        While index + I <= txt.Length
            If Integer.TryParse(txt.Substring(index, I), test) Then
                result = test
                I = I + 1
            Else
                Return result
            End If
        End While
        Return result
    End Function

    Private Function GetParameter(txt As String, index As Integer) As String
        Dim bracketcounter As Integer = 0
        Dim result As String = ""
        Dim I As Integer = index
        If index < txt.Length Then
            If txt(I) <> "[" Then Return ""
            I += 1
            bracketcounter = 1
            While I < txt.Length
                If txt(I) = "[" Then
                    bracketcounter += 1
                ElseIf txt(I) = "]" Then
                    bracketcounter -= 1
                    If bracketcounter = 0 Then Return result
                End If
                result &= txt(I)
                I += 1
            End While
        End If
        Return ""
    End Function

End Class
