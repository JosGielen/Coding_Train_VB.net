Public Class Fluid

    Public N As Integer
    Public dt As Double
    Public diff As Double
    Public visc As Double
    Public s(,) As Double
    Public density(,) As Double
    Public Vx(,) As Double
    Public Vy(,) As Double
    Public Vx0(,) As Double
    Public Vy0(,) As Double

    Public Sub New(size As Integer, timestep As Double, diffusion As Double, viscosity As Double)
        N = size
        dt = timestep
        diff = diffusion
        visc = viscosity
        ReDim s(N, N)
        ReDim density(N, N)
        ReDim Vx(N, N)
        ReDim Vy(N, N)
        ReDim Vx0(N, N)
        ReDim Vy0(N, N)
    End Sub

    Public Sub TimeStep(iter As Integer)
        Diffuse(1, Vx0, Vx, visc, dt, iter)
        Diffuse(2, Vy0, Vy, visc, dt, iter)

        Project(Vx0, Vy0, Vx, Vy, iter)

        AdVect(1, Vx, Vx0, Vx0, Vy0, dt)
        AdVect(2, Vy, Vy0, Vx0, Vy0, dt)

        Project(Vx, Vy, Vx0, Vy0, iter)

        Diffuse(0, s, density, diff, dt, iter)
        AdVect(0, density, s, Vx, Vy, dt)
    End Sub


    Public Sub AddDensity(x As Integer, y As Integer, amount As Double)
        density(x, y) += amount
    End Sub

    Public Sub FadeDensity(amount As Double)
        For J As Integer = 0 To N - 1
            For I As Integer = 0 To N - 1
                If density(I, J) > amount Then density(I, J) -= amount
            Next
        Next
    End Sub

    Public Sub AddVelocity(x As Integer, y As Integer, amountX As Double, amountY As Double)
        Vx(x, y) += amountX
        Vy(x, y) += amountY
    End Sub

    Private Sub Diffuse(b As Integer, Vn As Double(,), Vn0 As Double(,), visc As Double, dt As Double, iter As Integer)
        Dim a As Double = dt * visc ' * (N - 2) * (N - 2)
        Lin_Solve(b, Vn, Vn0, a, 1 + 6 * a, iter)
    End Sub

    Private Sub Lin_Solve(b As Integer, x As Double(,), x0 As Double(,), a As Double, c As Double, iter As Integer)
        Dim cRecip = 1.0 / c
        For K As Integer = 0 To iter - 1
            For J As Integer = 1 To N - 2
                For I As Integer = 1 To N - 2
                    x(I, J) = (x0(I, J) + a * (x(I + 1, J) + x(I - 1, J) + x(I, J + 1) + x(I, J - 1))) * cRecip
                Next
            Next
            Set_bnd(b, x)
        Next
    End Sub

    Private Sub Project(velocX As Double(,), velocY As Double(,), p As Double(,), div As Double(,), iter As Integer)
        For J As Integer = 1 To N - 2
            For I As Integer = 1 To N - 2
                div(I, J) = -0.5 * (velocX(I + 1, J) - velocX(I - 1, J) + velocY(I, J + 1) - velocY(I, J - 1)) / N
                p(I, J) = 0
            Next
        Next
        Set_bnd(0, div)
        Set_bnd(0, p)
        Lin_Solve(0, p, div, 1, 6, iter)
        For J As Integer = 1 To N - 2
            For I As Integer = 1 To N - 2
                velocX(I, J) -= 0.5 * (p(I + 1, J) - p(I - 1, J)) * N
                velocY(I, J) -= 0.5 * (p(I, J + 1) - p(I, J - 1)) * N
            Next
        Next
        Set_bnd(1, velocX)
        Set_bnd(2, velocY)
    End Sub

    Private Sub AdVect(b As Integer, d As Double(,), d0 As Double(,), velocX As Double(,), velocY As Double(,), dt As Double)
        Dim i0 As Double
        Dim i1 As Double
        Dim j0 As Double
        Dim j1 As Double
        Dim s0 As Double
        Dim s1 As Double
        Dim t0 As Double
        Dim t1 As Double
        Dim x As Double
        Dim y As Double
        Dim Nfloat As Double = N

        For J As Integer = 1 To N - 2
            For I As Integer = 1 To N - 2
                x = I - dt * (N - 2) * velocX(I, J)
                y = J - dt * (N - 2) * velocY(I, J)
                If x < 0.5 Then x = 0.5
                If x > N + 0.5 Then x = N + 0.5
                i0 = Math.Floor(x)
                i1 = i0 + 1.0
                If y < 0.5 Then y = 0.5
                If y > N + 0.5 Then y = N + 0.5
                j0 = Math.Floor(y)
                j1 = j0 + 1.0
                s1 = x - i0
                s0 = 1.0 - s1
                t1 = y - j0
                t0 = 1.0 - t1
                If i1 < N And j1 < N Then
                    d(I, J) = s0 * (t0 * d0(i0, j0) + t1 * d0(i0, j1)) + s1 * (t0 * d0(i1, j0) + t1 * d0(i1, j1))
                End If
            Next
        Next
        Set_bnd(b, d)
    End Sub

    Private Sub Set_bnd(b As Integer, x As Double(,))
        'The outer cells mirror the velocity of the cell just inside it to create an impenetrable wall
        For I As Integer = 1 To N - 2 'Top and bottom outer row
            If b = 2 Then
                x(I, 0) = -x(I, 1)
                x(I, N - 1) = -x(I, N - 2)
            Else
                x(I, 0) = x(I, 1)
                x(I, N - 1) = x(I, N - 2)
            End If
        Next
        For J As Integer = 1 To N - 2 'Left and right outer row
            If b = 1 Then
                x(0, J) = -x(1, J)
                x(N - 1, J) = -x(N - 2, J)
            Else
                x(0, J) = x(1, J)
                x(N - 1, J) = x(N - 2, J)
            End If
        Next
        x(0, 0) = 0.5 * (x(1, 0) + x(0, 1))
        x(0, N - 1) = 0.5 * (x(1, N - 1) + x(0, N - 2))
        x(N - 1, 0) = 0.5 * (x(N - 2, 0) + x(N - 1, 1))
        x(N - 1, N - 1) = 0.5 * (x(N - 1, N - 2) + x(N - 2, N - 1))
    End Sub

End Class

