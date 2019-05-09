Imports RayTracing

Public Class Camera
    Private my_Pos As Point
    Private my_Angle As Double
    Private my_Dir As Vector
    Private my_FOV As Integer
    Private my_Size As Double
    Private my_Dot As Rectangle
    Private rotT As RotateTransform
    Private my_Rays As List(Of Ray)

    Public Sub New(pos As Point, angle As Double, fov As Integer, raycount As Integer)
        my_Pos = pos
        my_Angle = angle
        my_Size = 10.0
        rotT = New RotateTransform(angle)
        rotT.CenterX = my_Size
        rotT.CenterY = my_Size / 2
        my_Dir = New Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180))
        my_FOV = fov
        my_Rays = New List(Of Ray)
        Dim r As Ray
        Dim rayAngle As Double = my_Angle - my_FOV / 2
        For I As Integer = 0 To raycount - 1
            r = New Ray(my_Pos, rayAngle)
            my_Rays.Add(r)
            rayAngle += my_FOV / (raycount - 1)
        Next
    End Sub

    Public Property Pos As Point
        Get
            Return my_Pos
        End Get
        Set(value As Point)
            my_Pos = value
        End Set
    End Property

    Public ReadOnly Property Dir As Vector
        Get
            Return my_Dir
        End Get
    End Property

    Public Property FOV As Integer
        Get
            Return my_FOV
        End Get
        Set(value As Integer)
            my_FOV = value
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

    Public Property Angle As Double
        Get
            Return my_Angle
        End Get
        Set(value As Double)
            my_Angle = value
            my_Dir = New Vector(Math.Cos(Angle * Math.PI / 180), Math.Sin(Angle * Math.PI / 180))
            rotT.Angle = my_Angle
        End Set
    End Property

    Public ReadOnly Property Rays As List(Of Ray)
        Get
            Return my_Rays
        End Get
    End Property

    Public Sub Show(c As Canvas)
        For I As Integer = 0 To my_Rays.Count - 1
            my_Rays(I).Show(c)
        Next
        my_Dot = New Rectangle() With
        {
            .Width = 2 * my_Size,
            .Height = my_Size,
            .Stroke = Brushes.Black,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Red
        }
        my_Dot.SetValue(Canvas.LeftProperty, my_Pos.X - my_Size)
        my_Dot.SetValue(Canvas.TopProperty, my_Pos.Y - my_Size / 2)
        my_Dot.RenderTransform = rotT
        c.Children.Add(my_Dot)
    End Sub

    Public Sub Update()
        my_Dot.SetValue(Canvas.LeftProperty, my_Pos.X - my_Size)
        my_Dot.SetValue(Canvas.TopProperty, my_Pos.Y - my_Size / 2)
        Dim rayAngle As Double = my_Angle - my_FOV / 2
        For I As Integer = 0 To my_Rays.Count - 1
            my_Rays(I).Update(my_Pos, rayAngle)
            rayAngle += my_FOV / (my_Rays.Count - 1)
        Next
    End Sub

End Class
