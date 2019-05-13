Imports System.Windows.Media.Media3D
Imports RubiksCube

Public Class Cube3D

    Private my_Scene As GLScene
    Private my_Map As CubeMap
    Private my_Cubelets As List(Of Cubelet)
    Private ReadOnly CubeletSize As Double
    Private ReadOnly CubeletSpacing As Double = 1.5
    Private my_Rotation As QuarterRotation
    Private my_RotationSpeed As Double
    Private my_Scrable As List(Of QuarterRotation)
    Private SelectedCubelets As List(Of Cubelet)
    Private my_TotalAngle As Double

    Public Sub New(scene As GLScene, map As CubeMap)
        Dim cubie As Cubelet
        Dim pos As Vector3D
        my_Scene = scene
        my_Map = map
        my_Cubelets = New List(Of Cubelet)
        my_Scrable = New List(Of QuarterRotation)
        my_RotationSpeed = 1
        my_TotalAngle = 0.0
        CubeletSize = scene.ActualWidth / 25
        Dim CubeletFaceNumbers(2) As Integer
        'The FaceNumbers link the CubeletFaces to the X (or -X), Y (or -Y) and Z (or -Z) face of each Cubelet
        'Each Cubelet has 1, 2 or 3 CubeletFaces (-1 = hidden face)
        Dim FaceNumbers() As Integer = {2, 47, 35, 5, 46, -1, 8, 45, 11, 1, -1, 34, 4, -1, -1, 7, -1,
                                        10, 0, 36, 33, 3, 37, -1, 6, 38, 9, -1, 50, 32, -1, 49, -1,
                                        -1, 48, 14, -1, -1, 31, -1, -1, -1, -1, -1, 13, -1, 39, 30, -1, 40,
                                        -1, -1, 41, 12, 26, 53, 29, 23, 52, -1, 20, 51, 17, 25, -1,
                                        28, 22, -1, -1, 19, -1, 16, 24, 42, 27, 21, 43, -1, 18, 44, 15}
        Dim counter As Integer = 0
        'Make the Cubelets
        For X As Integer = -1 To 1
            For Y As Integer = -1 To 1
                For Z As Integer = -1 To 1
                    pos = New Vector3D(X, Y, Z)
                    CubeletFaceNumbers(0) = FaceNumbers(3 * counter)
                    CubeletFaceNumbers(1) = FaceNumbers(3 * counter + 1)
                    CubeletFaceNumbers(2) = FaceNumbers(3 * counter + 2)
                    cubie = New Cubelet(pos, CubeletSize, CubeletSpacing, CubeletFaceNumbers)
                    cubie.SetColors(map.CubeletFaces)
                    my_Scene.AddGeometry(cubie)
                    my_Cubelets.Add(cubie)
                    counter += 1
                Next
            Next
        Next
    End Sub

#Region "Parameters"

    Public Property RotationSpeed As Double
        Get
            Return my_RotationSpeed
        End Get
        Set(value As Double)
            my_RotationSpeed = value
        End Set
    End Property

    Public Property Rotation As QuarterRotation
        Get
            Return my_Rotation
        End Get
        Set(value As QuarterRotation)
            my_Rotation = value
        End Set
    End Property

    Public ReadOnly Property Cubelets As List(Of Cubelet)
        Get
            Return my_Cubelets
        End Get
    End Property

    Public ReadOnly Property Scrable As List(Of QuarterRotation)
        Get
            Return my_Scrable
        End Get
    End Property

#End Region

#Region "Rotation"

    Public Sub Rotate(rot As QuarterRotation)
        my_Rotation = rot
        Select Case my_Rotation
            Case QuarterRotation.UPCW
                RotateUp(True)
            Case QuarterRotation.UPCCW
                RotateUp(False)
            Case QuarterRotation.DWNCW
                RotateDown(True)
            Case QuarterRotation.DWNCCW
                RotateDown(False)
            Case QuarterRotation.LFTCW
                RotateLeft(True)
            Case QuarterRotation.LFTCCW
                RotateLeft(False)
            Case QuarterRotation.RGTCW
                RotateRight(True)
            Case QuarterRotation.RGTCCW
                RotateRight(False)
            Case QuarterRotation.FRTCW
                RotateFront(True)
            Case QuarterRotation.FRTCCW
                RotateFront(False)
            Case QuarterRotation.BCKCW
                RotateBack(True)
            Case QuarterRotation.BCKCCW
                RotateBack(False)
        End Select
    End Sub

    Private Sub RotateUp(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the top
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.Y = 1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        Else
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 1, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, CubeletSize + CubeletSpacing, 0))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(0, my_TotalAngle, 0)
        Next
        'When the rotation is finished
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateUP2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
        End If
    End Sub

    Private Sub RotateDown(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the bottom
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.Y = -1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        Else
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 1, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, -1 * (CubeletSize + CubeletSpacing), 0))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(0, my_TotalAngle, 0)
        Next
        'When the rotation is done
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateDOWN2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
        End If
    End Sub

    Private Sub RotateLeft(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.X = -1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube X-axis and around their own X-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        Else
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(1, 0, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(-1 * (CubeletSize + CubeletSpacing), 0, 0))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(my_TotalAngle, 0, 0)
        Next
        'When the rotation is done
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateLEFT2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
        End If
    End Sub

    Private Sub RotateRight(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.X = 1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube X-axis and around their own X-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        Else
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(1, 0, 0), angle)
        rotT = New RotateTransform3D(rot, New Point3D(CubeletSize + CubeletSpacing, 0, 0))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(my_TotalAngle, 0, 0)
        Next
        'When the rotation is done
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateRIGHT2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
        End If
    End Sub

    Private Sub RotateFront(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.Z = 1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Z-axis and around their own Z-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        Else
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 0, 1), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, 0, CubeletSize + CubeletSpacing))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(0, 0, my_TotalAngle)
        Next
        'When the rotation is done
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateFRONT2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
            Exit Sub
        End If
    End Sub

    Private Sub RotateBack(ClockWise As Boolean)
        Dim angle As Double
        SelectedCubelets = New List(Of Cubelet)
        'Select the cubelets at the left side
        For I As Integer = 0 To my_Cubelets.Count - 1
            If my_Cubelets(I).CubeletPosition.Z = -1 Then
                SelectedCubelets.Add(my_Cubelets(I))
            End If
        Next
        'Rotate the selected cubelets around the cube Z-axis and around their own Z-axis
        Dim rot As AxisAngleRotation3D
        Dim rotT As RotateTransform3D
        If ClockWise Then
            my_TotalAngle -= my_RotationSpeed
            angle = my_RotationSpeed
        Else
            my_TotalAngle += my_RotationSpeed
            angle = -1 * my_RotationSpeed
        End If
        rot = New AxisAngleRotation3D(New Vector3D(0, 0, 1), angle)
        rotT = New RotateTransform3D(rot, New Point3D(0, 0, -1 * (CubeletSize + CubeletSpacing)))
        For I As Integer = 0 To SelectedCubelets.Count - 1
            SelectedCubelets(I).Position = rotT.Transform(SelectedCubelets(I).Position)
            SelectedCubelets(I).Rotation = New Vector3D(0, 0, my_TotalAngle)
        Next
        'When the rotation is done
        If Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001 Then
            my_Rotation = QuarterRotation.NONE
            my_TotalAngle = 0.0
            'Update the 2D CubeletFaces colors
            my_Map.RotateBACK2D(ClockWise)
            'Reset the selected cubelets position and update the colors
            For I As Integer = 0 To SelectedCubelets.Count - 1
                SelectedCubelets(I).ResetPosition()
                SelectedCubelets(I).SetColors(my_Map.CubeletFaces)
                SelectedCubelets(I).GenerateGeometry(my_Scene)
            Next
            Exit Sub
        End If
    End Sub

#End Region

    Public Sub UpdateColors()
        For I As Integer = 0 To my_Cubelets.Count - 1
            my_Cubelets(I).SetColors(my_Map.CubeletFaces)
            my_Cubelets(I).GenerateGeometry(my_Scene)
        Next
    End Sub

    Public Function GetLastRotation() As QuarterRotation
        Dim result As QuarterRotation = QuarterRotation.NONE
        If my_Scrable.Count > 0 Then
            result = Scrable.Last
            my_Scrable.RemoveAt(my_Scrable.Count - 1)
        End If
        Return result
    End Function

End Class
