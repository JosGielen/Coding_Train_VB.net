Imports System.Windows.Media.Media3D

Public Class Box
    Private my_Center As Point3D
    Private my_Size As Double

    Public Sub New(center As Point3D, size As Double)
        my_Center = center
        my_Size = size
    End Sub

    Public Property Center As Point3D
        Get
            Return my_Center
        End Get
        Set(value As Point3D)
            my_Center = value
        End Set
    End Property

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property

    Public Function GetVertices() As List(Of Vector3D)
        Dim result As List(Of Vector3D) = New List(Of Vector3D)
        'Top surface
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z + Size))
        'Bottom surface
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z + Size))
        'Front surface
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z + Size))
        'Back surface 
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z - Size))
        'Left surface 
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y - Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X - Size, Center.Y + Size, Center.Z + Size))
        'Right surface
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z + Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y - Size, Center.Z - Size))
        result.Add(New Vector3D(Center.X + Size, Center.Y + Size, Center.Z - Size))
        Return result
    End Function

    Public Function GetNormals() As List(Of Vector3D)
        Dim result As List(Of Vector3D) = New List(Of Vector3D)
        'Top surface 
        result.Add(New Vector3D(0.0, 1.0, 0.0))
        result.Add(New Vector3D(0.0, 1.0, 0.0))
        result.Add(New Vector3D(0.0, 1.0, 0.0))
        result.Add(New Vector3D(0.0, 1.0, 0.0))
        'Bottom surface
        result.Add(New Vector3D(0.0, -1.0, 0.0))
        result.Add(New Vector3D(0.0, -1.0, 0.0))
        result.Add(New Vector3D(0.0, -1.0, 0.0))
        result.Add(New Vector3D(0.0, -1.0, 0.0))
        'Front surface 
        result.Add(New Vector3D(0.0, 0.0, 1.0))
        result.Add(New Vector3D(0.0, 0.0, 1.0))
        result.Add(New Vector3D(0.0, 0.0, 1.0))
        result.Add(New Vector3D(0.0, 0.0, 1.0))
        'Back surface 
        result.Add(New Vector3D(0.0, 0.0, -1.0))
        result.Add(New Vector3D(0.0, 0.0, -1.0))
        result.Add(New Vector3D(0.0, 0.0, -1.0))
        result.Add(New Vector3D(0.0, 0.0, -1.0))
        'Left surface 
        result.Add(New Vector3D(-1.0, 0.0, 0.0))
        result.Add(New Vector3D(-1.0, 0.0, 0.0))
        result.Add(New Vector3D(-1.0, 0.0, 0.0))
        result.Add(New Vector3D(-1.0, 0.0, 0.0))
        'Right surface
        result.Add(New Vector3D(1.0, 0.0, 0.0))
        result.Add(New Vector3D(1.0, 0.0, 0.0))
        result.Add(New Vector3D(1.0, 0.0, 0.0))
        result.Add(New Vector3D(1.0, 0.0, 0.0))
        Return result
    End Function

    Public Function getIndices(startindex As Integer) As List(Of Integer)
        Dim result As List(Of Integer) = New List(Of Integer)
        'Top surface 
        result.Add(startindex)
        result.Add(startindex + 1)
        result.Add(startindex + 2)
        result.Add(startindex + 2)
        result.Add(startindex + 3)
        result.Add(startindex + 0)
        'Bottom surface
        result.Add(startindex + 4)
        result.Add(startindex + 5)
        result.Add(startindex + 6)
        result.Add(startindex + 6)
        result.Add(startindex + 7)
        result.Add(startindex + 4)
        'Front surface 
        result.Add(startindex + 8)
        result.Add(startindex + 9)
        result.Add(startindex + 10)
        result.Add(startindex + 10)
        result.Add(startindex + 11)
        result.Add(startindex + 8)
        'Back surface
        result.Add(startindex + 12)
        result.Add(startindex + 13)
        result.Add(startindex + 14)
        result.Add(startindex + 14)
        result.Add(startindex + 15)
        result.Add(startindex + 12)
        'Left surface
        result.Add(startindex + 16)
        result.Add(startindex + 17)
        result.Add(startindex + 18)
        result.Add(startindex + 18)
        result.Add(startindex + 19)
        result.Add(startindex + 16)
        'Right surface
        result.Add(startindex + 20)
        result.Add(startindex + 21)
        result.Add(startindex + 22)
        result.Add(startindex + 22)
        result.Add(startindex + 23)
        result.Add(startindex + 20)
        Return result
    End Function


    Public Function Devide() As List(Of Box)
        Dim newBoxes As List(Of Box) = New List(Of Box)
        Dim b As Box
        Dim newSize As Double = my_Size / 3
        Dim centerX As Double = 0.0
        Dim centerY As Double = 0.0
        Dim centerZ As Double = 0.0
        Dim sum As Integer = 0
        For I As Integer = -1 To 1
            For J As Integer = -1 To 1
                For K As Integer = -1 To 1
                    sum = Math.Abs(I) + Math.Abs(J) + Math.Abs(K)
                    If sum > 1 Then
                        centerX = my_Center.X + 2 * I * newSize
                        centerY = my_Center.Y + 2 * J * newSize
                        centerZ = my_Center.Z + 2 * K * newSize
                        b = New Box(New Point3D(centerX, centerY, centerZ), newSize)
                        newBoxes.Add(b)
                    End If
                Next
            Next
        Next
        Return newBoxes
    End Function
End Class
