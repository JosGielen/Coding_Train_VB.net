Imports System.IO

Public Class Log

    Private my_FileName As String
    Private my_LogItems As List(Of String)
    Private my_Size As Long
    Private my_SizeLimit As Long
    Private my_LogStarted As Boolean
    Private my_UseTimeStamp As Boolean

    Public Sub New()
        my_FileName = Environment.CurrentDirectory & "\Log-" & Now.Year.ToString() & Now.Month.ToString("00") & Now.Day.ToString("00") &
                                                         "-" & Now.Hour.ToString("00") & Now.Minute.ToString("00") & ".log"
        my_LogItems = New List(Of String)
        my_Size = 0
        my_SizeLimit = 1000
        my_LogStarted = True
        my_UseTimeStamp = False
    End Sub

    Public Sub New(started As Boolean, useTimestamp As Boolean)
        my_FileName = Environment.CurrentDirectory & "\Log-" & Now.Year.ToString() & Now.Month.ToString("00") & Now.Day.ToString("00") &
                                                         "-" & Now.Hour.ToString("00") & Now.Minute.ToString("00") & ".log"
        my_LogItems = New List(Of String)
        my_Size = 0
        my_SizeLimit = 1000
        my_LogStarted = started
        my_UseTimeStamp = useTimestamp
    End Sub

    Public Sub New(file As String, memmoryLimit As Integer, started As Boolean, useTimestamp As Boolean)
        my_FileName = file
        my_LogItems = New List(Of String)
        my_Size = 0
        my_SizeLimit = memmoryLimit
        my_LogStarted = started
        my_UseTimeStamp = useTimestamp
    End Sub

    Public Property UseTimeStamp As Boolean
        Get
            Return my_UseTimeStamp
        End Get
        Set(value As Boolean)
            my_UseTimeStamp = value
        End Set
    End Property

    Public Sub AddItem(item As String)
        If my_LogStarted Then
            my_LogItems.Add(item)
            my_Size += 2 * item.Length()
            If my_Size >= my_SizeLimit Then
                If SaveToFile() Then
                    my_LogItems.Clear()
                    my_Size = 0
                End If
            End If
        Else
            Debug.Print(item)
        End If
    End Sub

    Private Function SaveToFile() As Boolean
        'Write the log data to the file
        Dim myStream As StreamWriter = Nothing
        Try
            myStream = New StreamWriter(my_FileName, True)
            If (myStream IsNot Nothing) Then
                For I As Integer = 0 To my_LogItems.Count - 1
                    myStream.WriteLine(my_LogItems(I))
                Next
            End If
        Catch Ex As Exception
            MessageBox.Show("Cannot save the Log data. Original error: " & Ex.Message, "Log Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return False
        Finally
            If (myStream IsNot Nothing) Then
                myStream.Close()
            End If
        End Try
        Return True
    End Function

    Public Sub StartLog()
        my_LogStarted = True
    End Sub

    Public Sub StopLog()
        my_LogStarted = False
        SaveToFile()
    End Sub

    Public Sub ClearLog(deleteFile As Boolean)
        If deleteFile Then
            If File.Exists(my_FileName) Then File.Delete(my_FileName)
        End If
        my_LogItems.Clear()
        my_Size = 0
    End Sub

    Public Sub Close()
        If my_Size > 0 Then SaveToFile()
    End Sub

End Class
