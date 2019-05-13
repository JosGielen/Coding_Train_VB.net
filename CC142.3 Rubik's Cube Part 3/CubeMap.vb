
Public Class CubeMap
    Private my_CubeletFaces As List(Of CubeletFace)

    Public Sub New(c As Canvas)
        'Make the 2D CubeletFaces
        Dim cf As CubeletFace
        Dim side As Double 'The size of a cubeletFace
        Dim borderX As Double
        Dim borderY As Double
        Dim left As Double
        Dim top As Double
        Dim fixed As Boolean
        'Determine the optimal size of a cubeletFace
        my_CubeletFaces = New List(Of CubeletFace)
        If (c.ActualWidth - 50) / 4 > (c.ActualHeight - 50) / 3 Then
            side = (c.ActualHeight - 50) / 9
            borderX = (c.ActualWidth - 12 * side) / 2
            borderY = 25
        Else
            side = (c.ActualWidth - 50) / 12
            borderX = 25
            borderY = (c.ActualHeight - 9 * side) / 2
        End If
        c.Children.Clear()
        'Left Side (Orange)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + I * side
                top = borderY + (J + 3) * side
                cf = New CubeletFace(4, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
        'FRONT Side (Green)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + (I + 3) * side
                top = borderY + (J + 3) * side
                cf = New CubeletFace(2, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
        'RIGHT Side (Red)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + (I + 6) * side
                top = borderY + (J + 3) * side
                cf = New CubeletFace(5, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
        'BACK Side (Blue)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + (I + 9) * side
                top = borderY + (J + 3) * side
                cf = New CubeletFace(3, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
        'UP Side (White)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + (I + 3) * side
                top = borderY + J * side
                cf = New CubeletFace(0, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
        'DOWN Side (Yellow)
        For I As Integer = 0 To 2
            For J As Integer = 0 To 2
                fixed = (I = 1 And J = 1)
                left = borderX + (I + 3) * side
                top = borderY + (J + 6) * side
                cf = New CubeletFace(1, left, top, side, fixed)
                my_CubeletFaces.Add(cf)
                cf.Draw(c)
            Next
        Next
    End Sub

    Public ReadOnly Property CubeletFaces As List(Of CubeletFace)
        Get
            Return my_CubeletFaces
        End Get
    End Property

#Region "Rotations"

    Public Sub RotateUP2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(37).FaceColorNumber
                CubeletFaces(37).FaceColorNumber = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(41).FaceColorNumber
                CubeletFaces(41).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(43).FaceColorNumber
                CubeletFaces(43).FaceColorNumber = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = CubeletFaces(39).FaceColorNumber
                CubeletFaces(39).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = CubeletFaces(3).FaceColorNumber
                CubeletFaces(3).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(12).FaceColorNumber
                CubeletFaces(12).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(21).FaceColorNumber
                CubeletFaces(21).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(30).FaceColorNumber
                CubeletFaces(30).FaceColorNumber = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(39).FaceColorNumber
                CubeletFaces(39).FaceColorNumber = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = CubeletFaces(43).FaceColorNumber
                CubeletFaces(43).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(41).FaceColorNumber
                CubeletFaces(41).FaceColorNumber = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(37).FaceColorNumber
                CubeletFaces(37).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = CubeletFaces(30).FaceColorNumber
                CubeletFaces(30).FaceColorNumber = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = CubeletFaces(21).FaceColorNumber
                CubeletFaces(21).FaceColorNumber = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(12).FaceColorNumber
                CubeletFaces(12).FaceColorNumber = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = CubeletFaces(3).FaceColorNumber
                CubeletFaces(3).FaceColorNumber = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = temp
            Next
        End If
    End Sub

    Public Sub RotateDOWN2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(46).FaceColorNumber
                CubeletFaces(46).FaceColorNumber = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = CubeletFaces(50).FaceColorNumber
                CubeletFaces(50).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = CubeletFaces(52).FaceColorNumber
                CubeletFaces(52).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(48).FaceColorNumber
                CubeletFaces(48).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = CubeletFaces(32).FaceColorNumber
                CubeletFaces(32).FaceColorNumber = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(23).FaceColorNumber
                CubeletFaces(23).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(14).FaceColorNumber
                CubeletFaces(14).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(5).FaceColorNumber
                CubeletFaces(5).FaceColorNumber = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(48).FaceColorNumber
                CubeletFaces(48).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(52).FaceColorNumber
                CubeletFaces(52).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = CubeletFaces(50).FaceColorNumber
                CubeletFaces(50).FaceColorNumber = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = CubeletFaces(46).FaceColorNumber
                CubeletFaces(46).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = CubeletFaces(5).FaceColorNumber
                CubeletFaces(5).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(14).FaceColorNumber
                CubeletFaces(14).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(23).FaceColorNumber
                CubeletFaces(23).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(32).FaceColorNumber
                CubeletFaces(32).FaceColorNumber = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = temp
            Next
        End If
    End Sub

    Public Sub RotateLEFT2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = CubeletFaces(1).FaceColorNumber
                CubeletFaces(1).FaceColorNumber = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = CubeletFaces(5).FaceColorNumber
                CubeletFaces(5).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(7).FaceColorNumber
                CubeletFaces(7).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = CubeletFaces(3).FaceColorNumber
                CubeletFaces(3).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = CubeletFaces(46).FaceColorNumber
                CubeletFaces(46).FaceColorNumber = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(10).FaceColorNumber
                CubeletFaces(10).FaceColorNumber = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(37).FaceColorNumber
                CubeletFaces(37).FaceColorNumber = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = CubeletFaces(34).FaceColorNumber
                CubeletFaces(34).FaceColorNumber = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = CubeletFaces(3).FaceColorNumber
                CubeletFaces(3).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = CubeletFaces(7).FaceColorNumber
                CubeletFaces(7).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(5).FaceColorNumber
                CubeletFaces(5).FaceColorNumber = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = CubeletFaces(1).FaceColorNumber
                CubeletFaces(1).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = CubeletFaces(34).FaceColorNumber
                CubeletFaces(34).FaceColorNumber = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(37).FaceColorNumber
                CubeletFaces(37).FaceColorNumber = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(10).FaceColorNumber
                CubeletFaces(10).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(46).FaceColorNumber
                CubeletFaces(46).FaceColorNumber = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = temp
            Next
        End If
    End Sub

    Public Sub RotateRIGHT2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(19).FaceColorNumber
                CubeletFaces(19).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(23).FaceColorNumber
                CubeletFaces(23).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(25).FaceColorNumber
                CubeletFaces(25).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = CubeletFaces(21).FaceColorNumber
                CubeletFaces(21).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(28).FaceColorNumber
                CubeletFaces(28).FaceColorNumber = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = CubeletFaces(43).FaceColorNumber
                CubeletFaces(43).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(16).FaceColorNumber
                CubeletFaces(16).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(52).FaceColorNumber
                CubeletFaces(52).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(21).FaceColorNumber
                CubeletFaces(21).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = CubeletFaces(25).FaceColorNumber
                CubeletFaces(25).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(23).FaceColorNumber
                CubeletFaces(23).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(19).FaceColorNumber
                CubeletFaces(19).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(28).FaceColorNumber
                CubeletFaces(28).FaceColorNumber = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = CubeletFaces(52).FaceColorNumber
                CubeletFaces(52).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(16).FaceColorNumber
                CubeletFaces(16).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(43).FaceColorNumber
                CubeletFaces(43).FaceColorNumber = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = temp
            Next
        End If
    End Sub

    Public Sub RotateFRONT2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(10).FaceColorNumber
                CubeletFaces(10).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(14).FaceColorNumber
                CubeletFaces(14).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(16).FaceColorNumber
                CubeletFaces(16).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(12).FaceColorNumber
                CubeletFaces(12).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = CubeletFaces(7).FaceColorNumber
                CubeletFaces(7).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(48).FaceColorNumber
                CubeletFaces(48).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(19).FaceColorNumber
                CubeletFaces(19).FaceColorNumber = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(41).FaceColorNumber
                CubeletFaces(41).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(9).FaceColorNumber
                CubeletFaces(9).FaceColorNumber = CubeletFaces(12).FaceColorNumber
                CubeletFaces(12).FaceColorNumber = CubeletFaces(15).FaceColorNumber
                CubeletFaces(15).FaceColorNumber = CubeletFaces(16).FaceColorNumber
                CubeletFaces(16).FaceColorNumber = CubeletFaces(17).FaceColorNumber
                CubeletFaces(17).FaceColorNumber = CubeletFaces(14).FaceColorNumber
                CubeletFaces(14).FaceColorNumber = CubeletFaces(11).FaceColorNumber
                CubeletFaces(11).FaceColorNumber = CubeletFaces(10).FaceColorNumber
                CubeletFaces(10).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(38).FaceColorNumber
                CubeletFaces(38).FaceColorNumber = CubeletFaces(41).FaceColorNumber
                CubeletFaces(41).FaceColorNumber = CubeletFaces(44).FaceColorNumber
                CubeletFaces(44).FaceColorNumber = CubeletFaces(18).FaceColorNumber
                CubeletFaces(18).FaceColorNumber = CubeletFaces(19).FaceColorNumber
                CubeletFaces(19).FaceColorNumber = CubeletFaces(20).FaceColorNumber
                CubeletFaces(20).FaceColorNumber = CubeletFaces(51).FaceColorNumber
                CubeletFaces(51).FaceColorNumber = CubeletFaces(48).FaceColorNumber
                CubeletFaces(48).FaceColorNumber = CubeletFaces(45).FaceColorNumber
                CubeletFaces(45).FaceColorNumber = CubeletFaces(8).FaceColorNumber
                CubeletFaces(8).FaceColorNumber = CubeletFaces(7).FaceColorNumber
                CubeletFaces(7).FaceColorNumber = CubeletFaces(6).FaceColorNumber
                CubeletFaces(6).FaceColorNumber = temp
            Next
        End If
    End Sub

    Public Sub RotateBACK2D(ClockWise)
        Dim temp As Integer
        If ClockWise Then
            For I As Integer = 0 To 1
                temp = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(28).FaceColorNumber
                CubeletFaces(28).FaceColorNumber = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(32).FaceColorNumber
                CubeletFaces(32).FaceColorNumber = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = CubeletFaces(34).FaceColorNumber
                CubeletFaces(34).FaceColorNumber = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = CubeletFaces(30).FaceColorNumber
                CubeletFaces(30).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = CubeletFaces(25).FaceColorNumber
                CubeletFaces(25).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = CubeletFaces(50).FaceColorNumber
                CubeletFaces(50).FaceColorNumber = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = CubeletFaces(1).FaceColorNumber
                CubeletFaces(1).FaceColorNumber = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(39).FaceColorNumber
                CubeletFaces(39).FaceColorNumber = temp
            Next
        Else
            For I As Integer = 0 To 1
                temp = CubeletFaces(27).FaceColorNumber
                CubeletFaces(27).FaceColorNumber = CubeletFaces(30).FaceColorNumber
                CubeletFaces(30).FaceColorNumber = CubeletFaces(33).FaceColorNumber
                CubeletFaces(33).FaceColorNumber = CubeletFaces(34).FaceColorNumber
                CubeletFaces(34).FaceColorNumber = CubeletFaces(35).FaceColorNumber
                CubeletFaces(35).FaceColorNumber = CubeletFaces(32).FaceColorNumber
                CubeletFaces(32).FaceColorNumber = CubeletFaces(29).FaceColorNumber
                CubeletFaces(29).FaceColorNumber = CubeletFaces(28).FaceColorNumber
                CubeletFaces(28).FaceColorNumber = temp
            Next
            For I As Integer = 0 To 2
                temp = CubeletFaces(42).FaceColorNumber
                CubeletFaces(42).FaceColorNumber = CubeletFaces(39).FaceColorNumber
                CubeletFaces(39).FaceColorNumber = CubeletFaces(36).FaceColorNumber
                CubeletFaces(36).FaceColorNumber = CubeletFaces(0).FaceColorNumber
                CubeletFaces(0).FaceColorNumber = CubeletFaces(1).FaceColorNumber
                CubeletFaces(1).FaceColorNumber = CubeletFaces(2).FaceColorNumber
                CubeletFaces(2).FaceColorNumber = CubeletFaces(47).FaceColorNumber
                CubeletFaces(47).FaceColorNumber = CubeletFaces(50).FaceColorNumber
                CubeletFaces(50).FaceColorNumber = CubeletFaces(53).FaceColorNumber
                CubeletFaces(53).FaceColorNumber = CubeletFaces(26).FaceColorNumber
                CubeletFaces(26).FaceColorNumber = CubeletFaces(25).FaceColorNumber
                CubeletFaces(25).FaceColorNumber = CubeletFaces(24).FaceColorNumber
                CubeletFaces(24).FaceColorNumber = temp
            Next
        End If
    End Sub

#End Region

    Public Sub ToggleColor(Pt As Point)
        For I As Integer = 0 To CubeletFaces.Count - 1
            If CubeletFaces(I).Contains(Pt) Then
                CubeletFaces(I).ToggleColor()
            End If
        Next
    End Sub

End Class
