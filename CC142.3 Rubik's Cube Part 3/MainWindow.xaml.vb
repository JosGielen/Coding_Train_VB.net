
Class MainWindow
    Private Delegate Sub WaitDelegate(t As Integer)
    Private App_Initialized As Boolean = False
    Private Cube As Cube3D   'A Cube3D contains 27 Cubelets
    Private Map As CubeMap   'A CubeMap contains 54 CubeletFaces
    Private myRotation As QuarterRotation
    Private ReverseMoves As Boolean
    Private RotationSpeed As Double = 2

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
        App_Initialized = True
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub Init()
        canvas1.Children.Clear()
        Scene1.Geometries.Clear()
        Map = New CubeMap(canvas1)
        Cube = New Cube3D(Scene1, Map) With
        {
            .RotationSpeed = RotationSpeed
        }
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not App_Initialized Then Exit Sub
        If ReverseMoves Then
            If Cube.Rotation = QuarterRotation.NONE Then
                myRotation = Reverse(Cube.GetLastRotation())
                If myRotation = QuarterRotation.NONE Then
                    ReverseMoves = False
                    Exit Sub
                End If
            End If
        End If
        'Apply the rotation
        Cube.Rotate(myRotation)
        If Cube.Rotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.NONE
        End If
        'Render the scene.
        Scene1.Render()
    End Sub

    Private Function Reverse(rot As QuarterRotation) As QuarterRotation
        Select Case rot
            Case QuarterRotation.UPCW
                Return QuarterRotation.UPCCW
            Case QuarterRotation.UPCCW
                Return QuarterRotation.UPCW
            Case QuarterRotation.DWNCW
                Return QuarterRotation.DWNCCW
            Case QuarterRotation.DWNCCW
                Return QuarterRotation.DWNCW
            Case QuarterRotation.LFTCW
                Return QuarterRotation.LFTCCW
            Case QuarterRotation.LFTCCW
                Return QuarterRotation.LFTCW
            Case QuarterRotation.RGTCW
                Return QuarterRotation.RGTCCW
            Case QuarterRotation.RGTCCW
                Return QuarterRotation.RGTCW
            Case QuarterRotation.FRTCW
                Return QuarterRotation.FRTCCW
            Case QuarterRotation.FRTCCW
                Return QuarterRotation.FRTCW
            Case QuarterRotation.BCKCW
                Return QuarterRotation.BCKCCW
            Case QuarterRotation.BCKCCW
                Return QuarterRotation.BCKCW
        End Select
        Return QuarterRotation.NONE
    End Function

#Region "EventHandlers"

    Private Sub Canvas1_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim Pt As Point = e.GetPosition(canvas1)
        Map.ToggleColor(Pt)
        Cube.UpdateColors()
    End Sub

    Private Sub Window_KeyUp(sender As Object, e As KeyEventArgs)
        If Not myRotation = QuarterRotation.NONE Then Exit Sub
        Select Case e.Key
            Case Key.U
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.UPCCW
                Else
                    myRotation = QuarterRotation.UPCW
                End If
            Case Key.D
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.DWNCCW
                Else
                    myRotation = QuarterRotation.DWNCW
                End If
            Case Key.L
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.LFTCCW
                Else
                    myRotation = QuarterRotation.LFTCW
                End If
            Case Key.R
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.RGTCCW
                Else
                    myRotation = QuarterRotation.RGTCW
                End If
            Case Key.F
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.FRTCCW
                Else
                    myRotation = QuarterRotation.FRTCW
                End If
            Case Key.B
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = QuarterRotation.BCKCCW
                Else
                    myRotation = QuarterRotation.BCKCW
                End If
            Case Key.Escape
                Init()
            Case Key.F1
                ReverseMoves = True
        End Select
        If Not myRotation = QuarterRotation.NONE Then Cube.Scrable.Add(myRotation)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub


    Private Sub Normalize(list As List(Of Double))
        Dim sum As Double = 0.0
        For I As Integer = 0 To list.Count - 1
            sum += list(I)
        Next
        For I As Integer = 0 To list.Count - 1
            list(I) = list(I) / sum
        Next
    End Sub

    Private Sub BtnBackCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.BCKCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnUpCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.UPCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnUpCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.UPCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnBackCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.BCKCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnLeftCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.LFTCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnLeftCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.LFTCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnRightCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.RGTCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnRightCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.RGTCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnFrontCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.FRTCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnDownCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.DWNCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnDownCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.DWNCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

    Private Sub BtnFrontCCW_Click(sender As Object, e As RoutedEventArgs)
        If myRotation = QuarterRotation.NONE Then
            myRotation = QuarterRotation.FRTCCW
            Cube.Scrable.Add(myRotation)
        End If
    End Sub

#End Region

    Private Sub Wait(waitTime As Integer)
        Threading.Thread.Sleep(waitTime)
    End Sub
End Class

Public Enum QuarterRotation As Integer
    NONE = 0
    UPCW = 1
    UPCCW = 2
    DWNCW = 3
    DWNCCW = 4
    LFTCW = 5
    LFTCCW = 6
    RGTCW = 7
    RGTCCW = 8
    FRTCCW = 9
    FRTCW = 10
    BCKCW = 11
    BCKCCW = 12
End Enum

