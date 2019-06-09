Imports _3DTools
Imports System.Windows.Media.Media3D

Class MainWindow
    Private CamPos As Point3D
    Private CamTarget As Point3D
    Private CamUpDir As Vector3D
    Private CamSpeed As Double
    Private CamDistance As Double
    Private CamPitch As Double = 0.0
    Private CamYaw As Double = 0.0
    Private FOV As Double
    Private my_Mousedown As Boolean
    Private my_MouseStartPos As Point

    Public Sub New()
        InitializeComponent()
        CamDistance = 8
        CamPos = New Point3D(0, 0, -CamDistance)
        CamTarget = New Point3D(0, 0, 0)
        CamUpDir = New Vector3D(0, 1, 0)
        CamSpeed = 1
        FOV = 45
        'Create a Sphere:
        CreateSphere(New Point3D(0, 0, 0), 2.0, 20, 20)
        'Set the camera
        myViewport.Camera = New PerspectiveCamera(CamPos, CamTarget - CamPos, CamUpDir, FOV)
    End Sub

    Private Sub CreateSphere(center As Point3D, radius As Double, slices As Integer, stacks As Integer)
        If slices < 2 Or stacks < 2 Then
            Return
        End If
        Dim Longitude As Double = 0.0
        Dim Latitude As Double = 0.0
        Dim pts(slices, stacks) As Point3D
        'Calculate the 3D Points
        For I As Integer = 0 To slices - 1
            Longitude = 2 * I / (slices - 1) * Math.PI
            For J As Integer = 0 To stacks - 1
                Latitude = J / (stacks - 1) * Math.PI - 0.5 * Math.PI
                pts(I, J) = New Point3D() With
                {
                    .X = radius * Math.Sin(Longitude) * Math.Cos(Latitude),
                    .Y = radius * Math.Sin(Longitude) * Math.Sin(Latitude),
                    .Z = -radius * Math.Cos(Longitude)
                }
                pts(I, J) += CType(center, Vector3D)
            Next
        Next
        'Draw Triangles between the points
        Dim ssl As ScreenSpaceLines3D
        For I As Integer = 0 To slices - 2
            For J As Integer = 0 To stacks - 2
                ssl = New ScreenSpaceLines3D()
                ssl.Points.Add(pts(I, J))
                ssl.Points.Add(pts(I + 1, J))
                ssl.Points.Add(pts(I + 1, J))
                ssl.Points.Add(pts(I + 1, J + 1))
                ssl.Points.Add(pts(I + 1, J + 1))
                ssl.Points.Add(pts(I, J))
                ssl.Color = Colors.Black
                ssl.Thickness = 1.0
                myViewport.Children.Add(ssl)
            Next
        Next
    End Sub

#Region "Camera Control"

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        my_Mousedown = True
        my_MouseStartPos = e.GetPosition(myViewport)
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        'Move the Camera
        If my_Mousedown Then
            Dim pos As Point = e.GetPosition(myViewport)
            Dim dx As Double = pos.X - my_MouseStartPos.X
            Dim dy As Double = my_MouseStartPos.Y - pos.Y
            my_MouseStartPos = pos
            'update the Camera Yaw and Pitch angle
            CamYaw += dx * CamSpeed * Math.PI / 180
            CamPitch -= dy * CamSpeed * Math.PI / 180
            If CamPitch < -0.49 * Math.PI Then CamPitch = -0.49 * Math.PI
            If CamPitch > 0.49 * Math.PI Then CamPitch = 0.49 * Math.PI
            UpdatePosition()
        End If
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        my_Mousedown = False
    End Sub

    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Up Then
            CamDistance += CamSpeed
        ElseIf e.Key = Key.Down Then
            CamDistance -= CamSpeed
        End If
        UpdatePosition()
    End Sub

    Public Sub UpdatePosition()
        CamPos = New Point3D() With
        {
            .X = -CamDistance * Math.Sin(CamYaw) * Math.Cos(CamPitch) + CamTarget.X,
            .Y = CamDistance * Math.Sin(CamPitch) + CamTarget.Y,
            .Z = CamDistance * Math.Cos(CamYaw) * Math.Cos(CamPitch) + CamTarget.Z
        }
        Dim ViewDirection = CamTarget - CamPos
        myViewport.Camera = New PerspectiveCamera(CamPos, CamTarget - CamPos, CamUpDir, FOV)
    End Sub

#End Region

End Class
