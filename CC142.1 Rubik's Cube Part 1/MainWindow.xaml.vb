Imports System.Windows.Media.Media3D

Class MainWindow
    Private App_Loaded As Boolean = False
    Private Cube As List(Of Cubelet)
    Private spacing As Double
    Private size As Double
    Private myRotation As CubeletRotation
    Private Scrable As List(Of CubeletRotation)
    Private Solve As Boolean
    Private RotationSpeed As Double = 2
    Private TotalAngle As Double

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Scrable = New List(Of CubeletRotation)
        Solve = False
        Init()
        App_Loaded = True
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub Init()
        spacing = 1.5
        size = Scene1.ActualWidth / 25
        Dim b As Cubelet
        Dim cp As CubeletPosition = New CubeletPosition
        Cube = New List(Of Cubelet)
        Scene1.Geometries.Clear()
        For X As Integer = -1 To 1
            For Y As Integer = -1 To 1
                For Z As Integer = -1 To 1
                    If X = -1 Then
                        cp.LEFT = True
                        cp.RIGHT = False
                    ElseIf X = 1 Then
                        cp.LEFT = False
                        cp.RIGHT = True
                    Else
                        cp.LEFT = False
                        cp.RIGHT = False
                    End If
                    If Y = -1 Then
                        cp.DOWN = True
                        cp.UP = False
                    ElseIf Y = 1 Then
                        cp.DOWN = False
                        cp.UP = True
                    Else
                        cp.DOWN = False
                        cp.UP = False
                    End If
                    If Z = -1 Then
                        cp.BACK = True
                        cp.FRONT = False
                    ElseIf Z = 1 Then
                        cp.BACK = False
                        cp.FRONT = True
                    Else
                        cp.BACK = False
                        cp.FRONT = False
                    End If
                    b = New Cubelet(cp, size, spacing)
                    Scene1.AddGeometry(b)
                    Cube.Add(b)
                Next
            Next
        Next
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not App_Loaded Then Exit Sub
        Dim solveRotation As CubeletRotation
        If Solve Then
            If myRotation = CubeletRotation.NONE Then
                If Scrable.Count > 1 Then
                    Scrable.RemoveAt(Scrable.Count - 1)
                    solveRotation = Scrable.Last
                    TotalAngle = 0.0
                Else
                    Solve = False
                    myRotation = CubeletRotation.NONE
                    Init()
                End If
            End If
            Select Case solveRotation
                Case CubeletRotation.UPCW
                    myRotation = CubeletRotation.UPCCW
                Case CubeletRotation.UPCCW
                    myRotation = CubeletRotation.UPCW
                Case CubeletRotation.DWNCW
                    myRotation = CubeletRotation.DWNCCW
                Case CubeletRotation.DWNCCW
                    myRotation = CubeletRotation.DWNCW
                Case CubeletRotation.LFTCW
                    myRotation = CubeletRotation.LFTCCW
                Case CubeletRotation.LFTCCW
                    myRotation = CubeletRotation.LFTCW
                Case CubeletRotation.RGTCW
                    myRotation = CubeletRotation.RGTCCW
                Case CubeletRotation.RGTCCW
                    myRotation = CubeletRotation.RGTCW
                Case CubeletRotation.FRTCW
                    myRotation = CubeletRotation.FRTCCW
                Case CubeletRotation.FRTCCW
                    myRotation = CubeletRotation.FRTCW
            End Select
        End If
        'Apply the rotation
        Select Case myRotation
            Case CubeletRotation.UPCW
                RotateUp(True)
            Case CubeletRotation.UPCCW
                RotateUp(False)
            Case CubeletRotation.DWNCW
                RotateDown(True)
            Case CubeletRotation.DWNCCW
                RotateDown(False)
            Case CubeletRotation.LFTCW
                RotateLeft(True)
            Case CubeletRotation.LFTCCW
                RotateLeft(False)
            Case CubeletRotation.RGTCW
                RotateRight(True)
            Case CubeletRotation.RGTCCW
                RotateRight(False)
            Case CubeletRotation.FRTCW
                RotateFront(True)
            Case CubeletRotation.FRTCCW
                RotateFront(False)
        End Select
        'Render the scene.
        Scene1.Render()
    End Sub

#Region "Rotations"

    Private Sub RotateUp(dir As Boolean)
        Dim angle As Double
        Dim sel As List(Of Cubelet) = New List(Of Cubelet)
        'Select the cubelets at the top
        For I As Integer = 0 To Cube.Count - 1
            If Cube(I).actualPosition.UP Then
                sel.Add(Cube(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If dir Then
            TotalAngle += RotationSpeed
            angle = -1 * RotationSpeed
        Else
            TotalAngle -= RotationSpeed
            angle = RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 1, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, size + spacing, 0))
        For I As Integer = 0 To sel.Count - 1
            sel(I).Position = rotT.Transform(sel(I).Position)
            sel(I).Rotation = New Vector3D(0, TotalAngle, 0)
        Next
        'Update the cubelets Vertices and ActualPosition when the rotation is done
        If Math.Abs(Math.Abs(TotalAngle) - 90) < 0.001 Then
            myRotation = CubeletRotation.NONE
            For I As Integer = 0 To sel.Count - 1
                If dir Then
                    sel(I).actualPosition.YRotation = (sel(I).actualPosition.YRotation + 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.UPCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.LEFT Then 'Front Left
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Front Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.LEFT Then 'Back Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Right
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    Else
                        If sel(I).actualPosition.LEFT Then 'Middle Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Middle Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                Else
                    sel(I).actualPosition.YRotation = (sel(I).actualPosition.YRotation - 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.UPCCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.LEFT Then 'Front Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Front Right
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.LEFT Then 'Back Left
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    Else
                        If sel(I).actualPosition.LEFT Then 'Middle Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Middle Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                End If
                sel(I).SetPosition()
                sel(I).SetColors()
                sel(I).UpdateColors()
                sel(I).GenerateGeometry(Scene1)
            Next
        End If
    End Sub

    Private Sub RotateDown(dir As Boolean)
        Dim angle As Double
        Dim sel As List(Of Cubelet) = New List(Of Cubelet)
        'Select the cubelets at the bottom
        For I As Integer = 0 To Cube.Count - 1
            If Cube(I).actualPosition.DOWN Then
                sel.Add(Cube(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If dir Then
            TotalAngle -= RotationSpeed
            angle = RotationSpeed
        Else
            TotalAngle += RotationSpeed
            angle = -1 * RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 1, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, -1 * (size + spacing), 0))
        For I As Integer = 0 To sel.Count - 1
            sel(I).Position = rotT.Transform(sel(I).Position)
            sel(I).Rotation = New Vector3D(0, TotalAngle, 0)
        Next
        'Update the cubelets ActualPosition when the rotation is done
        If Math.Abs(Math.Abs(TotalAngle) - 90) < 0.001 Then
            myRotation = CubeletRotation.NONE
            For I As Integer = 0 To sel.Count - 1
                If dir Then
                    sel(I).actualPosition.YRotation = (sel(I).actualPosition.YRotation - 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.DWNCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.LEFT Then 'Front Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Front Right
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.LEFT Then 'Back Left
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    Else
                        If sel(I).actualPosition.LEFT Then 'Middle Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Middle Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                Else
                    sel(I).actualPosition.YRotation = (sel(I).actualPosition.YRotation + 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.DWNCCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.LEFT Then 'Front Left
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Front Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.LEFT Then 'Back Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Right
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    Else
                        If sel(I).actualPosition.LEFT Then 'Middle Left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Middle Right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                End If
                sel(I).SetPosition()
                sel(I).SetColors()
                sel(I).UpdateColors()
                sel(I).GenerateGeometry(Scene1)
            Next
        End If
    End Sub

    Private Sub RotateLeft(dir As Boolean)
        Dim angle As Double
        Dim sel As List(Of Cubelet) = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To Cube.Count - 1
            If Cube(I).actualPosition.LEFT Then
                sel.Add(Cube(I))
            End If
        Next
        'Rotate the selected cubelets around the cube X-axis and around their own X-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If dir Then
            TotalAngle -= RotationSpeed
            angle = RotationSpeed
        Else
            TotalAngle += RotationSpeed
            angle = -1 * RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(1, 0, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(-1 * (size + spacing), 0, 0))
        For I As Integer = 0 To sel.Count - 1
            sel(I).Position = rotT.Transform(sel(I).Position)
            sel(I).Rotation = New Vector3D(TotalAngle, 0, 0)
        Next
        'Update the cubelets ActualPosition when the rotation is done
        If Math.Abs(Math.Abs(TotalAngle) - 90) < 0.001 Then
            myRotation = CubeletRotation.NONE
            For I As Integer = 0 To sel.Count - 1
                If dir Then
                    sel(I).actualPosition.XRotation = (sel(I).actualPosition.XRotation - 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.LFTCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.UP Then ' Front UP
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Front Down
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.DOWN = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.UP Then 'Back UP
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Back Down
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.UP = True
                        End If
                    Else
                        If sel(I).actualPosition.UP Then 'Middle Up
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Middle Right
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.BACK = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                Else
                    sel(I).actualPosition.XRotation = (sel(I).actualPosition.XRotation + 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.LFTCCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.UP Then ' Front UP
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Front Down
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.UP = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.UP Then 'Back UP
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Back Down
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.DOWN = True
                        End If
                    Else
                        If sel(I).actualPosition.UP Then 'Middle Up
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Middle Right
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                End If
                sel(I).SetPosition()
                sel(I).SetColors()
                sel(I).UpdateColors()
                sel(I).GenerateGeometry(Scene1)
            Next
        End If
    End Sub

    Private Sub RotateRight(dir As Boolean)
        Dim angle As Double
        Dim sel As List(Of Cubelet) = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To Cube.Count - 1
            If Cube(I).actualPosition.RIGHT Then
                sel.Add(Cube(I))
            End If
        Next
        'Rotate the selected cubelets around the cube X-axis and around their own X-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If dir Then
            TotalAngle += RotationSpeed
            angle = -1 * RotationSpeed
        Else
            TotalAngle -= RotationSpeed
            angle = RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(1, 0, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(size + spacing, 0, 0))
        For I As Integer = 0 To sel.Count - 1
            sel(I).Position = rotT.Transform(sel(I).Position)
            sel(I).Rotation = New Vector3D(TotalAngle, 0, 0)
        Next
        'Update the cubelets ActualPosition when the rotation is done
        If Math.Abs(Math.Abs(TotalAngle) - 90) < 0.001 Then
            myRotation = CubeletRotation.NONE
            For I As Integer = 0 To sel.Count - 1
                If dir Then
                    sel(I).actualPosition.XRotation = (sel(I).actualPosition.XRotation + 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.RGTCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.UP Then ' Front UP
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Front Down
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.UP = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.UP Then 'Back UP
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Back Down
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.DOWN = True
                        End If
                    Else
                        If sel(I).actualPosition.UP Then 'Middle Up
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.BACK = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Middle Right
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.FRONT = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                Else
                    sel(I).actualPosition.XRotation = (sel(I).actualPosition.XRotation - 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.RGTCCW)
                    If sel(I).actualPosition.FRONT Then
                        If sel(I).actualPosition.UP Then ' Front UP
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Front Down
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.BACK = True
                        Else  'Front Middle
                            sel(I).actualPosition.FRONT = False
                            sel(I).actualPosition.DOWN = True
                        End If
                    ElseIf sel(I).actualPosition.BACK Then
                        If sel(I).actualPosition.UP Then 'Back UP
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Back Down
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        Else  'Back Middle
                            sel(I).actualPosition.BACK = False
                            sel(I).actualPosition.UP = True
                        End If
                    Else
                        If sel(I).actualPosition.UP Then 'Middle Up
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.FRONT = True
                        ElseIf sel(I).actualPosition.DOWN Then 'Middle Right
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.BACK = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                End If
                sel(I).SetPosition()
                sel(I).SetColors()
                sel(I).UpdateColors()
                sel(I).GenerateGeometry(Scene1)
            Next
        End If
    End Sub

    Private Sub RotateFront(dir As Boolean)
        Dim angle As Double
        Dim sel As List(Of Cubelet) = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To Cube.Count - 1
            If Cube(I).actualPosition.FRONT Then
                sel.Add(Cube(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Z-axis and around their own Z-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If dir Then
            TotalAngle += RotationSpeed
            angle = -1 * RotationSpeed
        Else
            TotalAngle -= RotationSpeed
            angle = RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 0, 1), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, 0, size + spacing))
        For I As Integer = 0 To sel.Count - 1
            sel(I).Position = rotT.Transform(sel(I).Position)
            sel(I).Rotation = New Vector3D(0, 0, TotalAngle)
        Next
        'Update the cubelets ActualPosition when the rotation is done
        If Math.Abs(Math.Abs(TotalAngle) - 90) < 0.001 Then
            myRotation = CubeletRotation.NONE
            For I As Integer = 0 To sel.Count - 1
                If dir Then
                    sel(I).actualPosition.ZRotation = (sel(I).actualPosition.ZRotation + 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.FRTCW)
                    If sel(I).actualPosition.UP Then
                        If sel(I).actualPosition.LEFT Then ' UP left
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Up right
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        Else  'Front Middle
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    ElseIf sel(I).actualPosition.DOWN Then
                        If sel(I).actualPosition.LEFT Then 'Back UP
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Down
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Back Middle
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    Else
                        If sel(I).actualPosition.RIGHT Then 'Middle Up
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.LEFT Then 'Middle Right
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.UP = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                Else
                    sel(I).actualPosition.ZRotation = (sel(I).actualPosition.ZRotation - 90) Mod 360
                    sel(I).Rotations.Add(CubeletRotation.FRTCCW)
                    If sel(I).actualPosition.UP Then
                        If sel(I).actualPosition.LEFT Then ' UP left
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.DOWN = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Up right
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.LEFT = True
                        Else  'Front Middle
                            sel(I).actualPosition.UP = False
                            sel(I).actualPosition.LEFT = True
                        End If
                    ElseIf sel(I).actualPosition.DOWN Then
                        If sel(I).actualPosition.LEFT Then 'Back UP
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.RIGHT = True
                        ElseIf sel(I).actualPosition.RIGHT Then 'Back Down
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.UP = True
                        Else  'Back Middle
                            sel(I).actualPosition.DOWN = False
                            sel(I).actualPosition.RIGHT = True
                        End If
                    Else
                        If sel(I).actualPosition.RIGHT Then 'Middle Up
                            sel(I).actualPosition.RIGHT = False
                            sel(I).actualPosition.UP = True
                        ElseIf sel(I).actualPosition.LEFT Then 'Middle Right
                            sel(I).actualPosition.LEFT = False
                            sel(I).actualPosition.DOWN = True
                        Else  'Middle Middle
                            'No change
                        End If
                    End If
                End If
                sel(I).SetPosition()
                sel(I).SetColors()
                sel(I).UpdateColors()
                sel(I).GenerateGeometry(Scene1)
            Next
            Exit Sub
        End If
    End Sub

#End Region

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)

    End Sub

    Private Sub Window_KeyUp(sender As Object, e As KeyEventArgs)
        If Not myRotation = CubeletRotation.NONE Then Exit Sub
        Select Case e.Key
            Case Key.U
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = CubeletRotation.UPCCW
                Else
                    myRotation = CubeletRotation.UPCW
                End If
            Case Key.D
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = CubeletRotation.DWNCCW
                Else
                    myRotation = CubeletRotation.DWNCW
                End If
            Case Key.L
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = CubeletRotation.LFTCCW
                Else
                    myRotation = CubeletRotation.LFTCW
                End If
            Case Key.R
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = CubeletRotation.RGTCCW
                Else
                    myRotation = CubeletRotation.RGTCW
                End If
            Case Key.F
                If Keyboard.IsKeyDown(Key.RightShift) Then
                    myRotation = CubeletRotation.FRTCCW
                Else
                    myRotation = CubeletRotation.FRTCW
                End If
            Case Key.Escape
                Init()
            Case Key.F1
                Solve = True
        End Select
        TotalAngle = 0.0
        Scrable.Add(myRotation)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class

Public Enum CubeletRotation
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
End Enum

Public Structure CubeletPosition
    Public UP As Boolean
    Public DOWN As Boolean
    Public LEFT As Boolean
    Public RIGHT As Boolean
    Public FRONT As Boolean
    Public BACK As Boolean
    Public XRotation As Double
    Public YRotation As Double
    Public ZRotation As Double

    Public Overrides Function ToString() As String
        Dim result As String = ""
        If UP Then result &= "UP ; "
        If DOWN Then result &= "DOWN ; "
        If LEFT Then result &= "LEFT ; "
        If RIGHT Then result &= "RIGHT ; "
        If FRONT Then result &= "FRONT ; "
        If BACK Then result &= "BACK ; "
        Return result & "  X rot=" & XRotation.ToString() & " ; Y rot=" & YRotation.ToString() & " ; Z rot=" & ZRotation.ToString()
    End Function
End Structure
