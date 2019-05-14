
Public Class OpenSimplexNoise

    ' Visual Basic .Net version of OpenSimplexNoise
    ' based off of Java code by Kurt Spencer
    ' Ported and modified by Jos Gielen 02/03/2019
    '
    Private Const STRETCH_CONSTANT_2D As Double = -0.211324865405187 '(1/Math.sqrt(2+1)-1)/2
    Private Const SQUISH_CONSTANT_2D As Double = 0.366025403784439 '(Math.sqrt(2+1)-1)/2
    Private Const STRETCH_CONSTANT_3D As Double = -1.0 / 6 '(1/Math.sqrt(3+1)-1)/3
    Private Const SQUISH_CONSTANT_3D As Double = 1.0 / 3 '(Math.sqrt(3+1)-1)/3
    Private Const STRETCH_CONSTANT_4D As Double = -0.138196601125011 '(1/Math.sqrt(4+1)-1)/4
    Private Const SQUISH_CONSTANT_4D As Double = 0.309016994374947 '(Math.sqrt(4+1)-1)/4
    Private Const NORM_CONSTANT_2D As Double = 47
    Private Const NORM_CONSTANT_3D As Double = 103
    Private Const NORM_CONSTANT_4D As Double = 30
    Private Const DEFAULT_SEED As Long = 0
    Private Shared ReadOnly perm As Short()
    Private Shared ReadOnly permGradIndex3D As Short()

    Shared Sub New()
        Dim seed As Long
        Dim Rnd As Random = New Random()
        ReDim perm(256)
        ReDim permGradIndex3D(256)
        Dim source(256) As Short
        For i As Short = 0 To 255
            source(i) = i
        Next
        For i As Integer = 255 To 0 Step -1
            seed = CLng(Rnd.NextDouble() * Long.MaxValue)
            Dim r As Integer = CInt((seed + 31) Mod (i + 1))
            If (r < 0) Then r += (i + 1)
            perm(i) = source(r)
            permGradIndex3D(i) = CShort((perm(i) Mod (Gradients3D.Length / 3)) * 3)
            source(r) = source(i)
        Next
    End Sub

    ''' <summary>
    ''' Calculates a widened single octave 2D OpenSimplex Noise value between 0 and 1 for input values x,y.
    ''' </summary>
    ''' <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex2D(x As Double, y As Double, factor As Integer) As Double
        Return SmoothStep(Simplex2D(x, y), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a single octave 2D OpenSimplex Noise value between 0 and 1 for input values x,y.
    ''' </summary>
    ''' <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
    Public Shared Function Simplex2D(x As Double, y As Double) As Double
        'Place input coordinates onto grid.
        Dim stretchOffset As Double = (x + y) * STRETCH_CONSTANT_2D
        Dim xs As Double = x + stretchOffset
        Dim ys As Double = y + stretchOffset
        'Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
        Dim xsb As Integer = FastFloor(xs)
        Dim ysb As Integer = FastFloor(ys)
        'Skew out to get actual coordinates of rhombus origin. We'll need these later.
        Dim squishOffset As Double = (xsb + ysb) * SQUISH_CONSTANT_2D
        Dim xb As Double = xsb + squishOffset
        Dim yb As Double = ysb + squishOffset
        'Compute grid coordinates relative to rhombus origin.
        Dim xins As Double = xs - xsb
        Dim yins As Double = ys - ysb
        'Sum those together to get a value that determines which region we're in.
        Dim inSum As Double = xins + yins
        'Positions relative to origin point.
        Dim dx0 As Double = x - xb
        Dim dy0 As Double = y - yb
        'We'll be defining these inside the next block and using them afterwards.
        Dim dx_ext As Double
        Dim dy_ext As Double
        Dim xsv_ext As Integer
        Dim ysv_ext As Integer
        Dim value As Double = 0
        'Contribution (1,0)
        Dim dx1 As Double = dx0 - 1 - SQUISH_CONSTANT_2D
        Dim dy1 As Double = dy0 - 0 - SQUISH_CONSTANT_2D
        Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1
        If (attn1 > 0) Then
            attn1 *= attn1
            value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, dx1, dy1)
        End If
        'Contribution (0,1)
        Dim dx2 As Double = dx0 - 0 - SQUISH_CONSTANT_2D
        Dim dy2 As Double = dy0 - 1 - SQUISH_CONSTANT_2D
        Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2
        If (attn2 > 0) Then
            attn2 *= attn2
            value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, dx2, dy2)
        End If
        If (inSum <= 1) Then 'We're inside the triangle (2-Simplex) at (0,0)
            Dim zins As Double = 1 - inSum
            If (zins > xins Or zins > yins) Then '(0,0) Is one of the closest two triangular vertices
                If (xins > yins) Then
                    xsv_ext = xsb + 1
                    ysv_ext = ysb - 1
                    dx_ext = dx0 - 1
                    dy_ext = dy0 + 1
                Else
                    xsv_ext = xsb - 1
                    ysv_ext = ysb + 1
                    dx_ext = dx0 + 1
                    dy_ext = dy0 - 1
                End If
            Else '(1,0) And (0,1) are the closest two vertices.
                xsv_ext = xsb + 1
                ysv_ext = ysb + 1
                dx_ext = dx0 - 1 - 2 * SQUISH_CONSTANT_2D
                dy_ext = dy0 - 1 - 2 * SQUISH_CONSTANT_2D
            End If
        Else 'We're inside the triangle (2-Simplex) at (1,1)
            Dim zins As Double = 2 - inSum
            If (zins < xins Or zins < yins) Then '(0,0) is one of the closest two triangular vertices
                If (xins > yins) Then
                    xsv_ext = xsb + 2
                    ysv_ext = ysb + 0
                    dx_ext = dx0 - 2 - 2 * SQUISH_CONSTANT_2D
                    dy_ext = dy0 + 0 - 2 * SQUISH_CONSTANT_2D
                Else
                    xsv_ext = xsb + 0
                    ysv_ext = ysb + 2
                    dx_ext = dx0 + 0 - 2 * SQUISH_CONSTANT_2D
                    dy_ext = dy0 - 2 - 2 * SQUISH_CONSTANT_2D
                End If
            Else '(1,0) And (0,1) are the closest two vertices.
                dx_ext = dx0
                dy_ext = dy0
                xsv_ext = xsb
                ysv_ext = ysb
            End If
            xsb += 1
            ysb += 1
            dx0 = dx0 - 1 - 2 * SQUISH_CONSTANT_2D
            dy0 = dy0 - 1 - 2 * SQUISH_CONSTANT_2D
        End If
        'Contribution (0,0) Or (1,1)
        Dim attn0 As Double = 2 - dx0 * dx0 - dy0 * dy0
        If (attn0 > 0) Then
            attn0 *= attn0
            value += attn0 * attn0 * Extrapolate(xsb, ysb, dx0, dy0)
        End If
        'Extra Vertex
        Dim attn_ext As Double = 2 - dx_ext * dx_ext - dy_ext * dy_ext
        If (attn_ext > 0) Then
            attn_ext *= attn_ext
            value += attn_ext * attn_ext * Extrapolate(xsv_ext, ysv_ext, dx_ext, dy_ext)
        End If
        Return (value / NORM_CONSTANT_2D + 1) / 2
    End Function

    ''' <summary>
    ''' Calculates a widened 2D OpenSimplex Noise value between 0 and 1 for input values x,y with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 2D OpenSimplex Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 2D OpenSimplex Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex2D(x As Double, y As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Simplex2D(x, y, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 2D OpenSimplex Noise value between 0 and 1 for input values x,y with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 2D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 2D OpenSimplex Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 2D OpenSimplex Noise function.</param>
    Public Shared Function Simplex2D(x As Double, y As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Simplex2D(x * frequency, y * frequency) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

    ''' <summary>
    ''' Calculates a widened single octave 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z.
    ''' </summary>
    ''' <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex3D(x As Double, y As Double, z As Double, factor As Integer) As Double
        Return SmoothStep(Simplex3D(x, y, z), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a single octave 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z.
    ''' </summary>
    ''' <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
    Public Shared Function Simplex3D(x As Double, y As Double, z As Double) As Double
        'Place input coordinates on simplectic honeycomb.
        Dim stretchOffset As Double = (x + y + z) * STRETCH_CONSTANT_3D
        Dim xs As Double = x + stretchOffset
        Dim ys As Double = y + stretchOffset
        Dim zs As Double = z + stretchOffset
        'Floor to get simplectic honeycomb coordinates of rhombohedron (stretched cube) super-cell origin.
        Dim xsb As Integer = FastFloor(xs)
        Dim ysb As Integer = FastFloor(ys)
        Dim zsb As Integer = FastFloor(zs)
        'Skew out to get actual coordinates of rhombohedron origin. We'll need these later.
        Dim squishOffset As Double = (xsb + ysb + zsb) * SQUISH_CONSTANT_3D
        Dim xb As Double = xsb + squishOffset
        Dim yb As Double = ysb + squishOffset
        Dim zb As Double = zsb + squishOffset
        'Compute simplectic honeycomb coordinates relative to rhombohedral origin.
        Dim xins As Double = xs - xsb
        Dim yins As Double = ys - ysb
        Dim zins As Double = zs - zsb
        'Sum those together to get a value that determines which region we're in.
        Dim inSum As Double = xins + yins + zins
        'Positions relative to origin point.
        Dim dx0 As Double = x - xb
        Dim dy0 As Double = y - yb
        Dim dz0 As Double = z - zb
        'We'll be defining these inside the next block and using them afterwards.
        Dim dx_ext0, dy_ext0, dz_ext0 As Double
        Dim dx_ext1, dy_ext1, dz_ext1 As Double
        Dim xsv_ext0, ysv_ext0, zsv_ext0 As Integer
        Dim xsv_ext1, ysv_ext1, zsv_ext1 As Integer

        Dim value As Double = 0
        If (inSum <= 1) Then  'We're inside the tetrahedron (3-Simplex) at (0,0,0)
            'Determine which two of (0,0,1), (0,1,0), (1,0,0) are closest.
            Dim aPoint As Byte = &H1
            Dim aScore As Double = xins
            Dim bPoint As Byte = &H2
            Dim bScore As Double = yins
            If (aScore >= bScore And zins > bScore) Then
                bScore = zins
                bPoint = &H4
            ElseIf (aScore < bScore And zins > aScore) Then
                aScore = zins
                aPoint = &H4
            End If
            'Now we determine the two lattice points Not part of the tetrahedron that may contribute.
            'This depends on the closest two tetrahedral vertices, including (0,0,0)
            Dim wins As Double = 1 - inSum
            If (wins > aScore Or wins > bScore) Then '(0,0,0) Is one of the closest two tetrahedral vertices.
                Dim c As Byte 'Our other closest vertex Is the closest out of a And b.
                If bScore > aScore Then
                    c = bPoint
                Else
                    c = aPoint
                End If
                If ((c And &H1) = 0) Then
                    xsv_ext0 = xsb - 1
                    xsv_ext1 = xsb
                    dx_ext0 = dx0 + 1
                    dx_ext1 = dx0
                Else
                    xsv_ext0 = xsb + 1
                    dx_ext0 = dx0 - 1
                    xsv_ext1 = xsb + 1
                    dx_ext1 = dx0 - 1
                End If
                If ((c And &H2) = 0) Then
                    ysv_ext0 = ysb
                    dy_ext0 = dy0
                    ysv_ext1 = ysb
                    dy_ext1 = dy0
                    If ((c And &H1) = 0) Then
                        ysv_ext1 -= 1
                        dy_ext1 += 1
                    Else
                        ysv_ext0 -= 1
                        dy_ext0 += 1
                    End If
                Else
                    ysv_ext0 = ysb + 1
                    dy_ext0 = dy0 - 1
                    ysv_ext1 = ysb + 1
                    dy_ext1 = dy0 - 1
                End If
                If ((c And &H4) = 0) Then
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb - 1
                    dz_ext0 = dz0
                    dz_ext1 = dz0 + 1
                Else
                    zsv_ext0 = zsb + 1
                    dz_ext0 = dz0 - 1
                    zsv_ext1 = zsb + 1
                    dz_ext1 = dz0 - 1
                End If
            Else '(0,0,0) Is Not one of the closest two tetrahedral vertices.
                Dim c As Byte = CByte(aPoint Or bPoint) 'Our two extra vertices are determined by the closest two.
                If ((c And &H1) = 0) Then
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb - 1
                    dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_3D
                    dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D
                Else
                    xsv_ext0 = xsb + 1
                    xsv_ext1 = xsb + 1
                    dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_3D
                    dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D
                End If
                If ((c And &H2) = 0) Then
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb - 1
                    dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_3D
                    dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D
                Else
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_3D
                    dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D
                End If
                If ((c And &H4) = 0) Then
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb - 1
                    dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_3D
                    dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D
                Else
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_3D
                    dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D
                End If
            End If
            'Contribution (0,0,0)
            Dim attn0 As Double = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0
            If (attn0 > 0) Then
                attn0 *= attn0
                value += attn0 * attn0 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, dx0, dy0, dz0)
            End If
            'Contribution (1,0,0)
            Dim dx1 As Double = dx0 - 1 - SQUISH_CONSTANT_3D
            Dim dy1 As Double = dy0 - 0 - SQUISH_CONSTANT_3D
            Dim dz1 As Double = dz0 - 0 - SQUISH_CONSTANT_3D
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1)
            End If
            'Contribution (0,1,0)
            Dim dx2 As Double = dx0 - 0 - SQUISH_CONSTANT_3D
            Dim dy2 As Double = dy0 - 1 - SQUISH_CONSTANT_3D
            Dim dz2 As Double = dz1
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2)
            End If
            'Contribution (0,0,1)
            Dim dx3 As Double = dx2
            Dim dy3 As Double = dy1
            Dim dz3 As Double = dz0 - 1 - SQUISH_CONSTANT_3D
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3)
            End If
        ElseIf (inSum >= 2) Then 'We're inside the tetrahedron (3-Simplex) at (1,1,1)
            'Determine which two tetrahedral vertices are the closest, out of (1,1,0), (1,0,1), (0,1,1) but not (1,1,1).
            Dim aPoint As Byte = &H6
            Dim aScore As Double = xins
            Dim bPoint As Byte = &H5
            Dim bScore As Double = yins
            If (aScore <= bScore And zins < bScore) Then
                bScore = zins
                bPoint = &H3
            ElseIf (aScore > bScore And zins < aScore) Then
                aScore = zins
                aPoint = &H3
            End If
            'Now we determine the two lattice points not part of the tetrahedron that may contribute.
            'This depends on the closest two tetrahedral vertices, including (1,1,1)
            Dim wins As Double = 3 - inSum
            If (wins < aScore Or wins < bScore) Then '(1,1,1) is one of the closest two tetrahedral vertices.
                Dim c As Byte
                If bScore < aScore Then 'Our other closest vertex is the closest out of a and b.
                    c = bPoint
                Else
                    c = aPoint
                End If
                If ((c And &H1) <> 0) Then
                    xsv_ext0 = xsb + 2
                    xsv_ext1 = xsb + 1
                    dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_3D
                    dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D
                Else
                    xsv_ext0 = xsb
                    dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_3D
                    xsv_ext1 = xsb
                    dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_3D
                End If
                If ((c And &H2) <> 0) Then
                    ysv_ext0 = ysb + 1
                    dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D
                    ysv_ext1 = ysb + 1
                    dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D
                    If ((c And &H1) <> 0) Then
                        ysv_ext1 += 1
                        dy_ext1 -= 1
                    Else
                        ysv_ext0 += 1
                        dy_ext0 -= 1
                    End If
                Else
                    ysv_ext0 = ysb
                    dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_3D
                    ysv_ext1 = ysb
                    dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_3D
                End If
                If ((c And &H4) <> 0) Then
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 2
                    dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D
                    dz_ext1 = dz0 - 2 - 3 * SQUISH_CONSTANT_3D
                Else
                    zsv_ext0 = zsb
                    dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_3D
                    zsv_ext1 = zsb
                    dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_3D
                End If
            Else '(1,1,1) is not one of the closest two tetrahedral vertices.
                Dim c As Byte = CByte(aPoint And bPoint) 'Our two extra vertices are determined by the closest two.
                If ((c And &H1) <> 0) Then
                    xsv_ext0 = xsb + 1
                    xsv_ext1 = xsb + 2
                    dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D
                    dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D
                Else
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb
                    dx_ext0 = dx0 - SQUISH_CONSTANT_3D
                    dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D
                End If
                If ((c And &H2) <> 0) Then
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 2
                    dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D
                    dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D
                Else
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    dy_ext0 = dy0 - SQUISH_CONSTANT_3D
                    dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D
                End If
                If ((c And &H4) <> 0) Then
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 2
                    dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D
                    dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D
                Else
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    dz_ext0 = dz0 - SQUISH_CONSTANT_3D
                    dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D
                End If
            End If
            'Contribution (1,1,0)
            Dim dx3 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim dy3 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim dz3 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx3, dy3, dz3)
            End If
            'Contribution (1,0,1)
            Dim dx2 As Double = dx3
            Dim dy2 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim dz2 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx2, dy2, dz2)
            End If
            'Contribution (0,1,1)
            Dim dx1 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim dy1 As Double = dy3
            Dim dz1 As Double = dz2
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx1, dy1, dz1)
            End If
            'Contribution (1,1,1)
            dx0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D
            dy0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D
            dz0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D
            Dim attn0 As Double = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0
            If (attn0 > 0) Then
                attn0 *= attn0
                value += attn0 * attn0 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, dx0, dy0, dz0)
            End If
        Else 'We're inside the octahedron (Rectified 3-Simplex) in between.
            Dim aScore As Double
            Dim aPoint As Byte
            Dim aIsFurtherSide As Boolean
            Dim bScore As Double
            Dim bPoint As Byte
            Dim bIsFurtherSide As Boolean
            'Decide between point (0,0,1) and (1,1,0) as closest
            Dim p1 As Double = xins + yins
            If (p1 > 1) Then
                aScore = p1 - 1
                aPoint = &H3
                aIsFurtherSide = True
            Else
                aScore = 1 - p1
                aPoint = &H4
                aIsFurtherSide = False
            End If
            'Decide between point (0,1,0) and (1,0,1) as closest
            Dim p2 As Double = xins + zins
            If (p2 > 1) Then
                bScore = p2 - 1
                bPoint = &H5
                bIsFurtherSide = True
            Else
                bScore = 1 - p2
                bPoint = &H2
                bIsFurtherSide = False
            End If
            'The closest out of the two (1,0,0) and (0,1,1) will replace the furthest out of the two decided above, if closer.
            Dim p3 As Double = yins + zins
            If (p3 > 1) Then
                Dim score As Double = p3 - 1
                If (aScore <= bScore And aScore < score) Then
                    aScore = score
                    aPoint = &H6
                    aIsFurtherSide = True
                ElseIf (aScore > bScore And bScore < score) Then
                    bScore = score
                    bPoint = &H6
                    bIsFurtherSide = True
                End If
            Else
                Dim score As Double = 1 - p3
                If (aScore <= bScore And aScore < score) Then
                    aScore = score
                    aPoint = &H1
                    aIsFurtherSide = False
                ElseIf (aScore > bScore And bScore < score) Then
                    bScore = score
                    bPoint = &H1
                    bIsFurtherSide = False
                End If
            End If
            'Where each of the two closest points are, determines how the extra two vertices are calculated.
            If (aIsFurtherSide = bIsFurtherSide) Then
                If (aIsFurtherSide) Then
                    'Both closest points on (1,1,1) side
                    'One of the two extra points is (1,1,1)
                    dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_3D
                    dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_3D
                    dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_3D
                    xsv_ext0 = xsb + 1
                    ysv_ext0 = ysb + 1
                    zsv_ext0 = zsb + 1
                    'Other extra point is based on the shared axis.
                    Dim c As Byte = CByte(aPoint And bPoint)
                    If ((c And &H1) <> 0) Then
                        dx_ext1 = dx0 - 2 - 2 * SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb + 2
                        ysv_ext1 = ysb
                        zsv_ext1 = zsb
                    ElseIf ((c And &H2) <> 0) Then
                        dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 - 2 - 2 * SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb
                        ysv_ext1 = ysb + 2
                        zsv_ext1 = zsb
                    Else
                        dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 - 2 - 2 * SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb
                        ysv_ext1 = ysb
                        zsv_ext1 = zsb + 2
                    End If
                Else
                    'Both closest points on (0,0,0) side
                    'One of the two extra points is (0,0,0)
                    dx_ext0 = dx0
                    dy_ext0 = dy0
                    dz_ext0 = dz0
                    xsv_ext0 = xsb
                    ysv_ext0 = ysb
                    zsv_ext0 = zsb
                    'Other extra point is based on the omitted axis.
                    Dim c As Byte = CByte(aPoint Or bPoint)
                    If ((c And &H1) = 0) Then
                        dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb - 1
                        ysv_ext1 = ysb + 1
                        zsv_ext1 = zsb + 1
                    ElseIf ((c And &H2) = 0) Then
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 + 1 - SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb + 1
                        ysv_ext1 = ysb - 1
                        zsv_ext1 = zsb + 1
                    Else
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_3D
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_3D
                        dz_ext1 = dz0 + 1 - SQUISH_CONSTANT_3D
                        xsv_ext1 = xsb + 1
                        ysv_ext1 = ysb + 1
                        zsv_ext1 = zsb - 1
                    End If
                End If
            Else 'One point on (0,0,0) side, one point on (1,1,1) side
                Dim c1, c2 As Byte
                If (aIsFurtherSide) Then
                    c1 = aPoint
                    c2 = bPoint
                Else
                    c1 = bPoint
                    c2 = aPoint
                End If
                'One contribution is a permutation of (1,1,-1)
                If ((c1 And &H1) = 0) Then
                    dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_3D
                    dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D
                    dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D
                    xsv_ext0 = xsb - 1
                    ysv_ext0 = ysb + 1
                    zsv_ext0 = zsb + 1
                ElseIf ((c1 And &H2) = 0) Then
                    dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D
                    dy_ext0 = dy0 + 1 - SQUISH_CONSTANT_3D
                    dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_3D
                    xsv_ext0 = xsb + 1
                    ysv_ext0 = ysb - 1
                    zsv_ext0 = zsb + 1
                Else
                    dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_3D
                    dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_3D
                    dz_ext0 = dz0 + 1 - SQUISH_CONSTANT_3D
                    xsv_ext0 = xsb + 1
                    ysv_ext0 = ysb + 1
                    zsv_ext0 = zsb - 1
                End If
                'One contribution is a permutation of (0,0,2)
                dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_3D
                dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_3D
                dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_3D
                xsv_ext1 = xsb
                ysv_ext1 = ysb
                zsv_ext1 = zsb
                If ((c2 And &H1) <> 0) Then
                    dx_ext1 -= 2
                    xsv_ext1 += 2
                ElseIf ((c2 And &H2) <> 0) Then
                    dy_ext1 -= 2
                    ysv_ext1 += 2
                Else
                    dz_ext1 -= 2
                    zsv_ext1 += 2
                End If
            End If
            'Contribution (1,0,0)
            Dim dx1 As Double = dx0 - 1 - SQUISH_CONSTANT_3D
            Dim dy1 As Double = dy0 - 0 - SQUISH_CONSTANT_3D
            Dim dz1 As Double = dz0 - 0 - SQUISH_CONSTANT_3D
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1)
            End If
            'Contribution (0,1,0)
            Dim dx2 As Double = dx0 - 0 - SQUISH_CONSTANT_3D
            Dim dy2 As Double = dy0 - 1 - SQUISH_CONSTANT_3D
            Dim dz2 As Double = dz1
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2)
            End If
            'Contribution (0,0,1)
            Dim dx3 As Double = dx2
            Dim dy3 As Double = dy1
            Dim dz3 As Double = dz0 - 1 - SQUISH_CONSTANT_3D
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3)
            End If
            'Contribution (1,1,0)
            Dim dx4 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim dy4 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim dz4 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim attn4 As Double = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4
            If (attn4 > 0) Then
                attn4 *= attn4
                value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, dx4, dy4, dz4)
            End If
            'Contribution (1,0,1)
            Dim dx5 As Double = dx4
            Dim dy5 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim dz5 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_3D
            Dim attn5 As Double = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5
            If (attn5 > 0) Then
                attn5 *= attn5
                value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, dx5, dy5, dz5)
            End If
            'Contribution (0,1,1)
            Dim dx6 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_3D
            Dim dy6 As Double = dy4
            Dim dz6 As Double = dz5
            Dim attn6 As Double = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6
            If (attn6 > 0) Then
                attn6 *= attn6
                value += attn6 * attn6 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, dx6, dy6, dz6)
            End If
        End If
        'First extra vertex
        Dim attn_ext0 As Double = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0
        If (attn_ext0 > 0) Then
            attn_ext0 *= attn_ext0
            value += attn_ext0 * attn_ext0 * Extrapolate(xsv_ext0, ysv_ext0, zsv_ext0, dx_ext0, dy_ext0, dz_ext0)
        End If
        'Second extra vertex
        Dim attn_ext1 As Double = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1
        If (attn_ext1 > 0) Then
            attn_ext1 *= attn_ext1
            value += attn_ext1 * attn_ext1 * Extrapolate(xsv_ext1, ysv_ext1, zsv_ext1, dx_ext1, dy_ext1, dz_ext1)
        End If
        Return (value / NORM_CONSTANT_3D + 1) / 2
    End Function

    ''' <summary>
    ''' Calculates a widened 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 3D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 3D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex3D(x As Double, y As Double, z As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Simplex3D(x, y, z, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 3D OpenSimplex Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 3D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 3D OpenSimplex Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 3D OpenSimplex Noise function.</param>
    Public Shared Function Simplex3D(x As Double, y As Double, z As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Simplex3D(x * frequency, y * frequency, z * frequency) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

    ''' <summary>
    ''' Calculates a single octave widened 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w.
    ''' </summary>
    ''' <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex4D(x As Double, y As Double, z As Double, w As Double, factor As Integer) As Double
        Return SmoothStep(Simplex4D(x, y, z, w), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a single octave 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w.
    ''' </summary>
    ''' <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
    Public Shared Function Simplex4D(x As Double, y As Double, z As Double, w As Double) As Double
        'Place input coordinates on simplectic honeycomb.
        Dim stretchOffset As Double = (x + y + z + w) * STRETCH_CONSTANT_4D
        Dim xs As Double = x + stretchOffset
        Dim ys As Double = y + stretchOffset
        Dim zs As Double = z + stretchOffset
        Dim ws As Double = w + stretchOffset

        'Floor to get simplectic honeycomb coordinates of rhombo-hypercube super-cell origin.
        Dim xsb As Integer = FastFloor(xs)
        Dim ysb As Integer = FastFloor(ys)
        Dim zsb As Integer = FastFloor(zs)
        Dim wsb As Integer = FastFloor(ws)

        'Skew out to get actual coordinates of stretched rhombo-hypercube origin. We'll need these later.
        Dim squishOffset As Double = (xsb + ysb + zsb + wsb) * SQUISH_CONSTANT_4D
        Dim xb As Double = xsb + squishOffset
        Dim yb As Double = ysb + squishOffset
        Dim zb As Double = zsb + squishOffset
        Dim wb As Double = wsb + squishOffset

        'Compute simplectic honeycomb coordinates relative to rhombo-hypercube origin.
        Dim xins As Double = xs - xsb
        Dim yins As Double = ys - ysb
        Dim zins As Double = zs - zsb
        Dim wins As Double = ws - wsb

        'Sum those together to get a value that determines which region we're in.
        Dim inSum As Double = xins + yins + zins + wins

        'Positions relative to origin point.
        Dim dx0 As Double = x - xb
        Dim dy0 As Double = y - yb
        Dim dz0 As Double = z - zb
        Dim dw0 As Double = w - wb

        'We'll be defining these inside the next block and using them afterwards.
        Dim dx_ext0, dy_ext0, dz_ext0, dw_ext0 As Double
        Dim dx_ext1, dy_ext1, dz_ext1, dw_ext1 As Double
        Dim dx_ext2, dy_ext2, dz_ext2, dw_ext2 As Double
        Dim xsv_ext0, ysv_ext0, zsv_ext0, wsv_ext0 As Integer
        Dim xsv_ext1, ysv_ext1, zsv_ext1, wsv_ext1 As Integer
        Dim xsv_ext2, ysv_ext2, zsv_ext2, wsv_ext2 As Integer

        Dim value As Double = 0
        If (inSum <= 1) Then 'We're inside the pentachoron (4-Simplex) at (0,0,0,0)
            'Determine which two of (0,0,0,1), (0,0,1,0), (0,1,0,0), (1,0,0,0) are closest.
            Dim aPoint As Byte = &H1
            Dim aScore As Double = xins
            Dim bPoint As Byte = &H2
            Dim bScore As Double = yins
            If (aScore >= bScore And zins > bScore) Then
                bScore = zins
                bPoint = &H4
            ElseIf (aScore < bScore And zins > aScore) Then
                aScore = zins
                aPoint = &H4
            End If
            If (aScore >= bScore And wins > bScore) Then
                bScore = wins
                bPoint = &H8
            ElseIf (aScore < bScore And wins > aScore) Then
                aScore = wins
                aPoint = &H8
            End If

            'Now we determine the three lattice points not part of the pentachoron that may contribute.
            'This depends on the closest two pentachoron vertices, including (0,0,0,0)
            Dim uins As Double = 1 - inSum
            If (uins > aScore Or uins > bScore) Then '(0,0,0,0) is one of the closest two pentachoron vertices.
                Dim c As Byte
                If bScore > aScore Then 'Our other closest vertex is the closest out of a and b.
                    c = bPoint
                Else
                    c = aPoint
                End If
                If ((c And &H1) = 0) Then
                    xsv_ext0 = xsb - 1
                    xsv_ext1 = xsb
                    xsv_ext2 = xsb
                    dx_ext0 = dx0 + 1
                    dx_ext1 = dx0
                    dx_ext2 = dx0
                Else
                    xsv_ext0 = xsb + 1
                    dx_ext0 = dx0 - 1
                    xsv_ext1 = xsb + 1
                    dx_ext1 = dx0 - 1
                    xsv_ext2 = xsb + 1
                    dx_ext2 = dx0 - 1
                End If
                If ((c And &H2) = 0) Then
                    ysv_ext0 = ysb
                    dy_ext0 = dy0
                    ysv_ext1 = ysb
                    dy_ext1 = dy0
                    ysv_ext2 = ysb
                    dy_ext2 = dy0
                    If ((c And &H1) = &H1) Then
                        ysv_ext0 -= 1
                        dy_ext0 += 1
                    Else
                        ysv_ext1 -= 1
                        dy_ext1 += 1
                    End If
                Else
                    ysv_ext0 = ysb + 1
                    dy_ext0 = dy0 - 1
                    ysv_ext1 = ysb + 1
                    dy_ext1 = dy0 - 1
                    ysv_ext2 = ysb + 1
                    dy_ext2 = dy0 - 1
                End If

                If ((c And &H4) = 0) Then
                    zsv_ext0 = zsb
                    dz_ext0 = dz0
                    zsv_ext1 = zsb
                    dz_ext1 = dz0
                    zsv_ext2 = zsb
                    dz_ext2 = dz0

                    If ((c And &H3) <> 0) Then
                        If ((c And &H3) = &H3) Then
                            zsv_ext0 -= 1
                            dz_ext0 += 1
                        Else
                            zsv_ext1 -= 1
                            dz_ext1 += 1
                        End If
                    Else
                        zsv_ext2 -= 1
                        dz_ext2 += 1
                    End If
                Else
                    zsv_ext0 = zsb + 1
                    dz_ext0 = dz0 - 1
                    zsv_ext1 = zsb + 1
                    dz_ext1 = dz0 - 1
                    zsv_ext2 = zsb + 1
                    dz_ext2 = dz0 - 1
                End If

                If ((c And &H8) = 0) Then
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    wsv_ext2 = wsb - 1
                    dw_ext0 = dw0
                    dw_ext1 = dw0
                    dw_ext2 = dw0 + 1
                Else
                    wsv_ext0 = wsb + 1
                    dw_ext0 = dw0 - 1
                    wsv_ext1 = wsb + 1
                    dw_ext1 = dw0 - 1
                    wsv_ext2 = wsb + 1
                    dw_ext2 = dw0 - 1
                End If
            Else  '(0,0,0,0) is not one of the closest two pentachoron vertices.
                Dim c As Byte = CByte(aPoint Or bPoint) 'Our three extra vertices are determined by the closest two.

                If ((c And &H1) = 0) Then
                    xsv_ext0 = xsb
                    xsv_ext2 = xsb
                    xsv_ext1 = xsb - 1
                    dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 + 1 - SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - SQUISH_CONSTANT_4D
                Else
                    xsv_ext0 = xsb + 1
                    xsv_ext1 = xsb + 1
                    xsv_ext2 = xsb + 1
                    dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c And &H2) = 0) Then
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    ysv_ext2 = ysb
                    dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - SQUISH_CONSTANT_4D
                    If ((c And &H1) = &H1) Then
                        ysv_ext1 -= 1
                        dy_ext1 += 1
                    Else
                        ysv_ext2 -= 1
                        dy_ext2 += 1
                    End If
                Else
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    ysv_ext2 = ysb + 1
                    dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c And &H4) = 0) Then
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    zsv_ext2 = zsb
                    dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - SQUISH_CONSTANT_4D
                    If ((c And &H3) = &H3) Then
                        zsv_ext1 -= 1
                        dz_ext1 += 1
                    Else
                        zsv_ext2 -= 1
                        dz_ext2 += 1
                    End If
                Else
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    zsv_ext2 = zsb + 1
                    dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c And &H8) = 0) Then
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    wsv_ext2 = wsb - 1
                    dw_ext0 = dw0 - 2 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 + 1 - SQUISH_CONSTANT_4D
                Else
                    wsv_ext0 = wsb + 1
                    wsv_ext1 = wsb + 1
                    wsv_ext2 = wsb + 1
                    dw_ext0 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 1 - SQUISH_CONSTANT_4D
                End If
            End If

            'Contribution (0,0,0,0)
            Dim attn0 As Double = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0 - dw0 * dw0
            If (attn0 > 0) Then
                attn0 *= attn0
                value += attn0 * attn0 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 0, dx0, dy0, dz0, dw0)
            End If

            'Contribution (1,0,0,0)
            Dim dx1 As Double = dx0 - 1 - SQUISH_CONSTANT_4D
            Dim dy1 As Double = dy0 - 0 - SQUISH_CONSTANT_4D
            Dim dz1 As Double = dz0 - 0 - SQUISH_CONSTANT_4D
            Dim dw1 As Double = dw0 - 0 - SQUISH_CONSTANT_4D
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 0, dx1, dy1, dz1, dw1)
            End If

            'Contribution (0,1,0,0)
            Dim dx2 As Double = dx0 - 0 - SQUISH_CONSTANT_4D
            Dim dy2 As Double = dy0 - 1 - SQUISH_CONSTANT_4D
            Dim dz2 As Double = dz1
            Dim dw2 As Double = dw1
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 0, dx2, dy2, dz2, dw2)
            End If

            'Contribution (0,0,1,0)
            Dim dx3 As Double = dx2
            Dim dy3 As Double = dy1
            Dim dz3 As Double = dz0 - 1 - SQUISH_CONSTANT_4D
            Dim dw3 As Double = dw1
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 0, dx3, dy3, dz3, dw3)
            End If

            'Contribution (0,0,0,1)
            Dim dx4 As Double = dx2
            Dim dy4 As Double = dy1
            Dim dz4 As Double = dz1
            Dim dw4 As Double = dw0 - 1 - SQUISH_CONSTANT_4D
            Dim attn4 As Double = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4
            If (attn4 > 0) Then
                attn4 *= attn4
                value += attn4 * attn4 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 1, dx4, dy4, dz4, dw4)
            End If
        ElseIf (inSum >= 3) Then 'We're inside the pentachoron (4-Simplex) at (1,1,1,1)
            'Determine which two of (1,1,1,0), (1,1,0,1), (1,0,1,1), (0,1,1,1) are closest.
            Dim aPoint As Byte = &HE
            Dim aScore As Double = xins
            Dim bPoint As Byte = &HD
            Dim bScore As Double = yins
            If (aScore <= bScore And zins < bScore) Then
                bScore = zins
                bPoint = &HB
            ElseIf (aScore > bScore And zins < aScore) Then
                aScore = zins
                aPoint = &HB
            End If
            If (aScore <= bScore And wins < bScore) Then
                bScore = wins
                bPoint = &H7
            ElseIf (aScore > bScore And wins < aScore) Then
                aScore = wins
                aPoint = &H7
            End If

            'Now we determine the three lattice points not part of the pentachoron that may contribute.
            'This depends on the closest two pentachoron vertices, including (0,0,0,0)
            Dim uins As Double = 4 - inSum
            If (uins < aScore Or uins < bScore) Then '(1,1,1,1) is one of the closest two pentachoron vertices.
                Dim c As Byte
                If bScore < aScore Then 'Our other closest vertex is the closest out of a and b.
                    c = bPoint
                Else
                    c = aPoint
                End If
                If ((c And &H1) <> 0) Then
                    xsv_ext0 = xsb + 2
                    xsv_ext1 = xsb + 1
                    xsv_ext2 = xsb + 1
                    dx_ext0 = dx0 - 2 - 4 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D
                Else
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb
                    xsv_ext2 = xsb
                    dx_ext0 = dx0 - 4 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 4 * SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - 4 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H2) <> 0) Then
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    ysv_ext2 = ysb + 1
                    dy_ext0 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D
                    If ((c And &H1) <> 0) Then
                        ysv_ext1 += 1
                        dy_ext1 -= 1
                    Else
                        ysv_ext0 += 1
                        dy_ext0 -= 1
                    End If
                Else
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    ysv_ext2 = ysb
                    dy_ext0 = dy0 - 4 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 4 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 4 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H4) <> 0) Then
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    zsv_ext2 = zsb + 1
                    dz_ext0 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D
                    If ((c And &H3) <> &H3) Then
                        If ((c And &H3) = 0) Then
                            zsv_ext0 += 1
                            dz_ext0 -= 1
                        Else
                            zsv_ext1 += 1
                            dz_ext1 -= 1
                        End If
                    Else
                        zsv_ext2 += 1
                        dz_ext2 -= 1
                    End If
                Else
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    zsv_ext2 = zsb
                    dz_ext0 = dz0 - 4 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 4 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 4 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H8) <> 0) Then
                    wsv_ext0 = wsb + 1
                    wsv_ext1 = wsb + 1
                    wsv_ext2 = wsb + 2
                    dw_ext0 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 2 - 4 * SQUISH_CONSTANT_4D
                Else
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    wsv_ext2 = wsb
                    dw_ext0 = dw0 - 4 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 4 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 4 * SQUISH_CONSTANT_4D
                End If
            Else  '(1,1,1,1) is not one of the closest two pentachoron vertices.
                Dim c As Byte = CByte(aPoint And bPoint) 'Our three extra vertices are determined by the closest two.

                If ((c And &H1) <> 0) Then
                    xsv_ext0 = xsb + 1
                    xsv_ext2 = xsb + 1
                    xsv_ext1 = xsb + 2
                    dx_ext0 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
                Else
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb
                    xsv_ext2 = xsb
                    dx_ext0 = dx0 - 2 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D
                    dx_ext2 = dx0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H2) <> 0) Then
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    ysv_ext2 = ysb + 1
                    dy_ext0 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                    If ((c And &H1) <> 0) Then
                        ysv_ext2 += 1
                        dy_ext2 -= 1
                    Else
                        ysv_ext1 += 1
                        dy_ext1 -= 1
                    End If
                Else
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    ysv_ext2 = ysb
                    dy_ext0 = dy0 - 2 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H4) <> 0) Then
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    zsv_ext2 = zsb + 1
                    dz_ext0 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                    If ((c And &H3) <> 0) Then
                        zsv_ext2 += 1
                        dz_ext2 -= 1
                    Else
                        zsv_ext1 += 1
                        dz_ext1 -= 1
                    End If
                Else
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    zsv_ext2 = zsb
                    dz_ext0 = dz0 - 2 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c And &H8) <> 0) Then
                    wsv_ext0 = wsb + 1
                    wsv_ext1 = wsb + 1
                    wsv_ext2 = wsb + 2
                    dw_ext0 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D
                Else
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    wsv_ext2 = wsb
                    dw_ext0 = dw0 - 2 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 3 * SQUISH_CONSTANT_4D
                End If
            End If

            'Contribution (1,1,1,0)
            Dim dx4 As Double = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dy4 As Double = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dz4 As Double = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dw4 As Double = dw0 - 3 * SQUISH_CONSTANT_4D
            Dim attn4 As Double = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4
            If (attn4 > 0) Then
                attn4 *= attn4
                value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 0, dx4, dy4, dz4, dw4)
            End If

            'Contribution (1,1,0,1)
            Dim dx3 As Double = dx4
            Dim dy3 As Double = dy4
            Dim dz3 As Double = dz0 - 3 * SQUISH_CONSTANT_4D
            Dim dw3 As Double = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 1, dx3, dy3, dz3, dw3)
            End If

            'Contribution (1,0,1,1)
            Dim dx2 As Double = dx4
            Dim dy2 As Double = dy0 - 3 * SQUISH_CONSTANT_4D
            Dim dz2 As Double = dz4
            Dim dw2 As Double = dw3
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 1, dx2, dy2, dz2, dw2)
            End If

            'Contribution (0,1,1,1)
            Dim dx1 As Double = dx0 - 3 * SQUISH_CONSTANT_4D
            Dim dz1 As Double = dz4
            Dim dy1 As Double = dy4
            Dim dw1 As Double = dw3
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 1, dx1, dy1, dz1, dw1)
            End If

            'Contribution (1,1,1,1)
            dx0 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D
            dy0 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D
            dz0 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D
            dw0 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D
            Dim attn0 As Double = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0 - dw0 * dw0
            If (attn0 > 0) Then
                attn0 *= attn0
                value += attn0 * attn0 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 1, dx0, dy0, dz0, dw0)
            End If
        ElseIf (inSum <= 2) Then 'We're inside the first dispentachoron (Rectified 4-Simplex)
            Dim aScore As Double
            Dim aPoint As Byte
            Dim aIsBiggerSide As Boolean = True
            Dim bScore As Double
            Dim bPoint As Byte
            Dim bIsBiggerSide As Boolean = True

            'Decide between (1,1,0,0) and (0,0,1,1)
            If (xins + yins > zins + wins) Then
                aScore = xins + yins
                aPoint = &H3
            Else
                aScore = zins + wins
                aPoint = &HC
            End If

            'Decide between (1,0,1,0) and (0,1,0,1)
            If (xins + zins > yins + wins) Then
                bScore = xins + zins
                bPoint = &H5
            Else
                bScore = yins + wins
                bPoint = &HA
            End If

            'Closer between (1,0,0,1) and (0,1,1,0) will replace the further of a and b, if closer.
            If (xins + wins > yins + zins) Then
                Dim score As Double = xins + wins
                If (aScore >= bScore And score > bScore) Then
                    bScore = score
                    bPoint = &H9
                ElseIf (aScore < bScore And score > aScore) Then
                    aScore = score
                    aPoint = &H9
                End If
            Else
                Dim score As Double = yins + zins
                If (aScore >= bScore And score > bScore) Then
                    bScore = score
                    bPoint = &H6
                ElseIf (aScore < bScore And score > aScore) Then
                    aScore = score
                    aPoint = &H6
                End If
            End If

            'Decide if (1,0,0,0) is closer.
            Dim p1 As Double = 2 - inSum + xins
            If (aScore >= bScore And p1 > bScore) Then
                bScore = p1
                bPoint = &H1
                bIsBiggerSide = False
            ElseIf (aScore < bScore And p1 > aScore) Then
                aScore = p1
                aPoint = &H1
                aIsBiggerSide = False
            End If

            'Decide if (0,1,0,0) is closer.
            Dim p2 As Double = 2 - inSum + yins
            If (aScore >= bScore And p2 > bScore) Then
                bScore = p2
                bPoint = &H2
                bIsBiggerSide = False
            ElseIf (aScore < bScore And p2 > aScore) Then
                aScore = p2
                aPoint = &H2
                aIsBiggerSide = False
            End If

            'Decide if (0,0,1,0) is closer.
            Dim p3 As Double = 2 - inSum + zins
            If (aScore >= bScore And p3 > bScore) Then
                bScore = p3
                bPoint = &H4
                bIsBiggerSide = False
            ElseIf (aScore < bScore And p3 > aScore) Then
                aScore = p3
                aPoint = &H4
                aIsBiggerSide = False
            End If

            'Decide if (0,0,0,1) is closer.
            Dim p4 As Double = 2 - inSum + wins
            If (aScore >= bScore And p4 > bScore) Then
                bScore = p4
                bPoint = &H8
                bIsBiggerSide = False
            ElseIf (aScore < bScore And p4 > aScore) Then
                aScore = p4
                aPoint = &H8
                aIsBiggerSide = False
            End If

            'Where each of the two closest points are, determines how the extra three vertices are calculated.
            If (aIsBiggerSide = bIsBiggerSide) Then
                If (aIsBiggerSide) Then
                    'Both closest points on the bigger side
                    Dim c1 As Byte = CByte(aPoint Or bPoint)
                    Dim c2 As Byte = CByte(aPoint And bPoint)
                    If ((c1 And &H1) = 0) Then
                        xsv_ext0 = xsb
                        xsv_ext1 = xsb - 1
                        dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 + 1 - 2 * SQUISH_CONSTANT_4D
                    Else
                        xsv_ext0 = xsb + 1
                        xsv_ext1 = xsb + 1
                        dx_ext0 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
                    End If

                    If ((c1 And &H2) = 0) Then
                        ysv_ext0 = ysb
                        ysv_ext1 = ysb - 1
                        dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 + 1 - 2 * SQUISH_CONSTANT_4D
                    Else
                        ysv_ext0 = ysb + 1
                        ysv_ext1 = ysb + 1
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
                    End If

                    If ((c1 And &H4) = 0) Then
                        zsv_ext0 = zsb
                        zsv_ext1 = zsb - 1
                        dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 + 1 - 2 * SQUISH_CONSTANT_4D
                    Else
                        zsv_ext0 = zsb + 1
                        zsv_ext1 = zsb + 1
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
                    End If

                    If ((c1 And &H8) = 0) Then
                        wsv_ext0 = wsb
                        wsv_ext1 = wsb - 1
                        dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 + 1 - 2 * SQUISH_CONSTANT_4D
                    Else
                        wsv_ext0 = wsb + 1
                        wsv_ext1 = wsb + 1
                        dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
                    End If

                    'One combination is a permutation of (0,0,0,2) based on c2
                    xsv_ext2 = xsb
                    ysv_ext2 = ysb
                    zsv_ext2 = zsb
                    wsv_ext2 = wsb
                    dx_ext2 = dx0 - 2 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 2 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 2 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 2 * SQUISH_CONSTANT_4D
                    If ((c2 And &H1) <> 0) Then
                        xsv_ext2 += 2
                        dx_ext2 -= 2
                    ElseIf ((c2 And &H2) <> 0) Then
                        ysv_ext2 += 2
                        dy_ext2 -= 2
                    ElseIf ((c2 And &H4) <> 0) Then
                        zsv_ext2 += 2
                        dz_ext2 -= 2
                    Else
                        wsv_ext2 += 2
                        dw_ext2 -= 2
                    End If
                Else
                    'Both closest points on the smaller side
                    'One of the two extra points is (0,0,0,0)
                    xsv_ext2 = xsb
                    ysv_ext2 = ysb
                    zsv_ext2 = zsb
                    wsv_ext2 = wsb
                    dx_ext2 = dx0
                    dy_ext2 = dy0
                    dz_ext2 = dz0
                    dw_ext2 = dw0

                    'Other two points are based on the omitted axes.
                    Dim c As Byte = CByte(aPoint Or bPoint)

                    If ((c And &H1) = 0) Then
                        xsv_ext0 = xsb - 1
                        xsv_ext1 = xsb
                        dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 - SQUISH_CONSTANT_4D
                    Else
                        xsv_ext0 = xsb + 1
                        xsv_ext1 = xsb + 1
                        dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H2) = 0) Then
                        ysv_ext0 = ysb
                        ysv_ext1 = ysb
                        dy_ext0 = dy0 - SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 - SQUISH_CONSTANT_4D
                        If ((c And &H1) = &H1) Then
                            ysv_ext0 -= 1
                            dy_ext0 += 1
                        Else
                            ysv_ext1 -= 1
                            dy_ext1 += 1
                        End If
                    Else
                        ysv_ext0 = ysb + 1
                        ysv_ext1 = ysb + 1
                        dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H4) = 0) Then
                        zsv_ext0 = zsb
                        zsv_ext1 = zsb
                        dz_ext0 = dz0 - SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 - SQUISH_CONSTANT_4D
                        If ((c And &H3) = &H3) Then
                            zsv_ext0 -= 1
                            dz_ext0 += 1
                        Else
                            zsv_ext1 -= 1
                            dz_ext1 += 1
                        End If
                    Else
                        zsv_ext0 = zsb + 1
                        zsv_ext1 = zsb + 1
                        dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H8) = 0) Then
                        wsv_ext0 = wsb
                        wsv_ext1 = wsb - 1
                        dw_ext0 = dw0 - SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 + 1 - SQUISH_CONSTANT_4D
                    Else
                        wsv_ext0 = wsb + 1
                        wsv_ext1 = wsb + 1
                        dw_ext0 = dw0 - 1 - SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D
                    End If
                End If
            Else  'One point on each "side"
                Dim c1, c2 As Byte
                If (aIsBiggerSide) Then
                    c1 = aPoint
                    c2 = bPoint
                Else
                    c1 = bPoint
                    c2 = aPoint
                End If

                'Two contributions are the bigger-sided point with each 0 replaced with -1.
                If ((c1 And &H1) = 0) Then
                    xsv_ext0 = xsb - 1
                    xsv_ext1 = xsb
                    dx_ext0 = dx0 + 1 - SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - SQUISH_CONSTANT_4D
                Else
                    xsv_ext0 = xsb + 1
                    xsv_ext1 = xsb + 1
                    dx_ext0 = dx0 - 1 - SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H2) = 0) Then
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    dy_ext0 = dy0 - SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - SQUISH_CONSTANT_4D
                    If ((c1 And &H1) = &H1) Then
                        ysv_ext0 -= 1
                        dy_ext0 += 1
                    Else
                        ysv_ext1 -= 1
                        dy_ext1 += 1
                    End If
                Else
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    dy_ext0 = dy0 - 1 - SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H4) = 0) Then
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    dz_ext0 = dz0 - SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - SQUISH_CONSTANT_4D
                    If ((c1 And &H3) = &H3) Then
                        zsv_ext0 -= 1
                        dz_ext0 += 1
                    Else
                        zsv_ext1 -= 1
                        dz_ext1 += 1
                    End If
                Else
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    dz_ext0 = dz0 - 1 - SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 1 - SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H8) = 0) Then
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb - 1
                    dw_ext0 = dw0 - SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 + 1 - SQUISH_CONSTANT_4D
                Else
                    wsv_ext0 = wsb + 1
                    wsv_ext1 = wsb + 1
                    dw_ext0 = dw0 - 1 - SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 1 - SQUISH_CONSTANT_4D
                End If

                'One contribution is a permutation of (0,0,0,2) based on the smaller-sided point
                xsv_ext2 = xsb
                ysv_ext2 = ysb
                zsv_ext2 = zsb
                wsv_ext2 = wsb
                dx_ext2 = dx0 - 2 * SQUISH_CONSTANT_4D
                dy_ext2 = dy0 - 2 * SQUISH_CONSTANT_4D
                dz_ext2 = dz0 - 2 * SQUISH_CONSTANT_4D
                dw_ext2 = dw0 - 2 * SQUISH_CONSTANT_4D
                If ((c2 And &H1) <> 0) Then
                    xsv_ext2 += 2
                    dx_ext2 -= 2
                ElseIf ((c2 And &H2) <> 0) Then
                    ysv_ext2 += 2
                    dy_ext2 -= 2
                ElseIf ((c2 And &H4) <> 0) Then
                    zsv_ext2 += 2
                    dz_ext2 -= 2

                Else
                    wsv_ext2 += 2
                    dw_ext2 -= 2
                End If
            End If

            'Contribution (1,0,0,0)
            Dim dx1 As Double = dx0 - 1 - SQUISH_CONSTANT_4D
            Dim dy1 As Double = dy0 - 0 - SQUISH_CONSTANT_4D
            Dim dz1 As Double = dz0 - 0 - SQUISH_CONSTANT_4D
            Dim dw1 As Double = dw0 - 0 - SQUISH_CONSTANT_4D
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 0, dx1, dy1, dz1, dw1)
            End If

            'Contribution (0,1,0,0)
            Dim dx2 As Double = dx0 - 0 - SQUISH_CONSTANT_4D
            Dim dy2 As Double = dy0 - 1 - SQUISH_CONSTANT_4D
            Dim dz2 As Double = dz1
            Dim dw2 As Double = dw1
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 0, dx2, dy2, dz2, dw2)
            End If

            'Contribution (0,0,1,0)
            Dim dx3 As Double = dx2
            Dim dy3 As Double = dy1
            Dim dz3 As Double = dz0 - 1 - SQUISH_CONSTANT_4D
            Dim dw3 As Double = dw1
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 0, dx3, dy3, dz3, dw3)
            End If

            'Contribution (0,0,0,1)
            Dim dx4 As Double = dx2
            Dim dy4 As Double = dy1
            Dim dz4 As Double = dz1
            Dim dw4 As Double = dw0 - 1 - SQUISH_CONSTANT_4D
            Dim attn4 As Double = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4
            If (attn4 > 0) Then
                attn4 *= attn4
                value += attn4 * attn4 * Extrapolate(xsb + 0, ysb + 0, zsb + 0, wsb + 1, dx4, dy4, dz4, dw4)
            End If

            'Contribution (1,1,0,0)
            Dim dx5 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy5 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz5 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw5 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn5 As Double = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5 - dw5 * dw5
            If (attn5 > 0) Then
                attn5 *= attn5
                value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 0, dx5, dy5, dz5, dw5)
            End If

            'Contribution (1,0,1,0)
            Dim dx6 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy6 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz6 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw6 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn6 As Double = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6 - dw6 * dw6
            If (attn6 > 0) Then
                attn6 *= attn6
                value += attn6 * attn6 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 0, dx6, dy6, dz6, dw6)
            End If

            'Contribution (1,0,0,1)
            Dim dx7 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy7 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz7 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw7 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn7 As Double = 2 - dx7 * dx7 - dy7 * dy7 - dz7 * dz7 - dw7 * dw7
            If (attn7 > 0) Then
                attn7 *= attn7
                value += attn7 * attn7 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 1, dx7, dy7, dz7, dw7)
            End If

            'Contribution (0,1,1,0)
            Dim dx8 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy8 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz8 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw8 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn8 As Double = 2 - dx8 * dx8 - dy8 * dy8 - dz8 * dz8 - dw8 * dw8
            If (attn8 > 0) Then
                attn8 *= attn8
                value += attn8 * attn8 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 0, dx8, dy8, dz8, dw8)
            End If

            'Contribution (0,1,0,1)
            Dim dx9 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy9 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz9 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw9 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn9 As Double = 2 - dx9 * dx9 - dy9 * dy9 - dz9 * dz9 - dw9 * dw9
            If (attn9 > 0) Then
                attn9 *= attn9
                value += attn9 * attn9 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 1, dx9, dy9, dz9, dw9)
            End If

            'Contribution (0,0,1,1)
            Dim dx10 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy10 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz10 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw10 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn10 As Double = 2 - dx10 * dx10 - dy10 * dy10 - dz10 * dz10 - dw10 * dw10
            If (attn10 > 0) Then
                attn10 *= attn10
                value += attn10 * attn10 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 1, dx10, dy10, dz10, dw10)
            End If
        Else 'We're inside the second dispentachoron (Rectified 4-Simplex)
            Dim aScore As Double
            Dim aPoint As Byte
            Dim aIsBiggerSide As Boolean = True
            Dim bScore As Double
            Dim bPoint As Byte
            Dim bIsBiggerSide As Boolean = True

            'Decide between (0,0,1,1) and (1,1,0,0)
            If (xins + yins < zins + wins) Then
                aScore = xins + yins
                aPoint = &HC
            Else
                aScore = zins + wins
                aPoint = &H3
            End If

            'Decide between (0,1,0,1) and (1,0,1,0)
            If (xins + zins < yins + wins) Then
                bScore = xins + zins
                bPoint = &HA
            Else
                bScore = yins + wins
                bPoint = &H5
            End If

            'Closer between (0,1,1,0) and (1,0,0,1) will replace the further of a and b, if closer.
            If (xins + wins < yins + zins) Then
                Dim score As Double = xins + wins
                If (aScore <= bScore And score < bScore) Then
                    bScore = score
                    bPoint = &H6
                ElseIf (aScore > bScore And score < aScore) Then
                    aScore = score
                    aPoint = &H6
                End If
            Else
                Dim score As Double = yins + zins
                If (aScore <= bScore And score < bScore) Then
                    bScore = score
                    bPoint = &H9
                ElseIf (aScore > bScore And score < aScore) Then
                    aScore = score
                    aPoint = &H9
                End If
            End If

            'Decide if (0,1,1,1) is closer.
            Dim p1 As Double = 3 - inSum + xins
            If (aScore <= bScore And p1 < bScore) Then
                bScore = p1
                bPoint = &HE
                bIsBiggerSide = False
            ElseIf (aScore > bScore And p1 < aScore) Then
                aScore = p1
                aPoint = &HE
                aIsBiggerSide = False
            End If

            'Decide if (1,0,1,1) is closer.
            Dim p2 As Double = 3 - inSum + yins
            If (aScore <= bScore And p2 < bScore) Then
                bScore = p2
                bPoint = &HD
                bIsBiggerSide = False
            ElseIf (aScore > bScore And p2 < aScore) Then
                aScore = p2
                aPoint = &HD
                aIsBiggerSide = False
            End If

            'Decide if (1,1,0,1) is closer.
            Dim p3 As Double = 3 - inSum + zins
            If (aScore <= bScore And p3 < bScore) Then
                bScore = p3
                bPoint = &HB
                bIsBiggerSide = False
            ElseIf (aScore > bScore And p3 < aScore) Then
                aScore = p3
                aPoint = &HB
                aIsBiggerSide = False
            End If

            'Decide if (1,1,1,0) is closer.
            Dim p4 As Double = 3 - inSum + wins
            If (aScore <= bScore And p4 < bScore) Then
                bScore = p4
                bPoint = &H7
                bIsBiggerSide = False
            ElseIf (aScore > bScore And p4 < aScore) Then
                aScore = p4
                aPoint = &H7
                aIsBiggerSide = False
            End If

            'Where each of the two closest points are determines how the extra three vertices are calculated.
            If (aIsBiggerSide = bIsBiggerSide) Then
                If (aIsBiggerSide) Then
                    'Both closest points on the bigger side
                    Dim c1 As Byte = CByte(aPoint And bPoint)
                    Dim c2 As Byte = CByte(aPoint Or bPoint)

                    'Two contributions are permutations of (0,0,0,1) and (0,0,0,2) based on c1
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    dx_ext0 = dx0 - SQUISH_CONSTANT_4D
                    dy_ext0 = dy0 - SQUISH_CONSTANT_4D
                    dz_ext0 = dz0 - SQUISH_CONSTANT_4D
                    dw_ext0 = dw0 - SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 2 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 2 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 2 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 2 * SQUISH_CONSTANT_4D
                    If ((c1 And &H1) <> 0) Then
                        xsv_ext0 += 1
                        dx_ext0 -= 1
                        xsv_ext1 += 2
                        dx_ext1 -= 2
                    ElseIf ((c1 And &H2) <> 0) Then
                        ysv_ext0 += 1
                        dy_ext0 -= 1
                        ysv_ext1 += 2
                        dy_ext1 -= 2
                    ElseIf ((c1 And &H4) <> 0) Then
                        zsv_ext0 += 1
                        dz_ext0 -= 1
                        zsv_ext1 += 2
                        dz_ext1 -= 2
                    Else
                        wsv_ext0 += 1
                        dw_ext0 -= 1
                        wsv_ext1 += 2
                        dw_ext1 -= 2
                    End If

                    'One contribution is a permutation of (1,1,1,-1) based on c2
                    xsv_ext2 = xsb + 1
                    ysv_ext2 = ysb + 1
                    zsv_ext2 = zsb + 1
                    wsv_ext2 = wsb + 1
                    dx_ext2 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
                    If ((c2 And &H1) = 0) Then
                        xsv_ext2 -= 2
                        dx_ext2 += 2
                    ElseIf ((c2 And &H2) = 0) Then
                        ysv_ext2 -= 2
                        dy_ext2 += 2
                    ElseIf ((c2 And &H4) = 0) Then
                        zsv_ext2 -= 2
                        dz_ext2 += 2
                    Else
                        wsv_ext2 -= 2
                        dw_ext2 += 2
                    End If
                Else 'Both closest points on the smaller side
                    'One of the two extra points is (1,1,1,1)
                    xsv_ext2 = xsb + 1
                    ysv_ext2 = ysb + 1
                    zsv_ext2 = zsb + 1
                    wsv_ext2 = wsb + 1
                    dx_ext2 = dx0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dy_ext2 = dy0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dz_ext2 = dz0 - 1 - 4 * SQUISH_CONSTANT_4D
                    dw_ext2 = dw0 - 1 - 4 * SQUISH_CONSTANT_4D

                    'Other two points are based on the shared axes.
                    Dim c As Byte = CByte(aPoint And bPoint)

                    If ((c And &H1) <> 0) Then
                        xsv_ext0 = xsb + 2
                        xsv_ext1 = xsb + 1
                        dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
                    Else
                        xsv_ext0 = xsb
                        xsv_ext1 = xsb
                        dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D
                        dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H2) <> 0) Then
                        ysv_ext0 = ysb + 1
                        ysv_ext1 = ysb + 1
                        dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                        If ((c And &H1) = 0) Then
                            ysv_ext0 += 1
                            dy_ext0 -= 1
                        Else
                            ysv_ext1 += 1
                            dy_ext1 -= 1
                        End If
                    Else
                        ysv_ext0 = ysb
                        ysv_ext1 = ysb
                        dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D
                        dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H4) <> 0) Then
                        zsv_ext0 = zsb + 1
                        zsv_ext1 = zsb + 1
                        dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                        If ((c And &H3) = 0) Then
                            zsv_ext0 += 1
                            dz_ext0 -= 1
                        Else
                            zsv_ext1 += 1
                            dz_ext1 -= 1
                        End If
                    Else
                        zsv_ext0 = zsb
                        zsv_ext1 = zsb
                        dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D
                        dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D
                    End If

                    If ((c And &H8) <> 0) Then
                        wsv_ext0 = wsb + 1
                        wsv_ext1 = wsb + 2
                        dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D
                    Else
                        wsv_ext0 = wsb
                        wsv_ext1 = wsb
                        dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D
                        dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D
                    End If
                End If

            Else  'One point on each "side"
                Dim c1, c2 As Byte
                If (aIsBiggerSide) Then
                    c1 = aPoint
                    c2 = bPoint
                Else
                    c1 = bPoint
                    c2 = aPoint
                End If

                'Two contributions are the bigger-sided point with each 1 replaced with 2.
                If ((c1 And &H1) <> 0) Then
                    xsv_ext0 = xsb + 2
                    xsv_ext1 = xsb + 1
                    dx_ext0 = dx0 - 2 - 3 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
                Else
                    xsv_ext0 = xsb
                    xsv_ext1 = xsb
                    dx_ext0 = dx0 - 3 * SQUISH_CONSTANT_4D
                    dx_ext1 = dx0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H2) <> 0) Then
                    ysv_ext0 = ysb + 1
                    ysv_ext1 = ysb + 1
                    dy_ext0 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
                    If ((c1 And &H1) = 0) Then
                        ysv_ext0 += 1
                        dy_ext0 -= 1
                    Else
                        ysv_ext1 += 1
                        dy_ext1 -= 1
                    End If
                Else
                    ysv_ext0 = ysb
                    ysv_ext1 = ysb
                    dy_ext0 = dy0 - 3 * SQUISH_CONSTANT_4D
                    dy_ext1 = dy0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H4) <> 0) Then
                    zsv_ext0 = zsb + 1
                    zsv_ext1 = zsb + 1
                    dz_ext0 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
                    If ((c1 And &H3) = 0) Then
                        zsv_ext0 += 1
                        dz_ext0 -= 1
                    Else
                        zsv_ext1 += 1
                        dz_ext1 -= 1
                    End If
                Else
                    zsv_ext0 = zsb
                    zsv_ext1 = zsb
                    dz_ext0 = dz0 - 3 * SQUISH_CONSTANT_4D
                    dz_ext1 = dz0 - 3 * SQUISH_CONSTANT_4D
                End If

                If ((c1 And &H8) <> 0) Then
                    wsv_ext0 = wsb + 1
                    wsv_ext1 = wsb + 2
                    dw_ext0 = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 2 - 3 * SQUISH_CONSTANT_4D
                Else
                    wsv_ext0 = wsb
                    wsv_ext1 = wsb
                    dw_ext0 = dw0 - 3 * SQUISH_CONSTANT_4D
                    dw_ext1 = dw0 - 3 * SQUISH_CONSTANT_4D
                End If

                'One contribution is a permutation of (1,1,1,-1) based on the smaller-sided point
                xsv_ext2 = xsb + 1
                ysv_ext2 = ysb + 1
                zsv_ext2 = zsb + 1
                wsv_ext2 = wsb + 1
                dx_ext2 = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
                dy_ext2 = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
                dz_ext2 = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
                dw_ext2 = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
                If ((c2 And &H1) = 0) Then
                    xsv_ext2 -= 2
                    dx_ext2 += 2
                ElseIf ((c2 And &H2) = 0) Then
                    ysv_ext2 -= 2
                    dy_ext2 += 2
                ElseIf ((c2 And &H4) = 0) Then
                    zsv_ext2 -= 2
                    dz_ext2 += 2
                Else
                    wsv_ext2 -= 2
                    dw_ext2 += 2
                End If
            End If

            'Contribution (1,1,1,0)
            Dim dx4 As Double = dx0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dy4 As Double = dy0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dz4 As Double = dz0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim dw4 As Double = dw0 - 3 * SQUISH_CONSTANT_4D
            Dim attn4 As Double = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4 - dw4 * dw4
            If (attn4 > 0) Then
                attn4 *= attn4
                value += attn4 * attn4 * Extrapolate(xsb + 1, ysb + 1, zsb + 1, wsb + 0, dx4, dy4, dz4, dw4)
            End If

            'Contribution (1,1,0,1)
            Dim dx3 As Double = dx4
            Dim dy3 As Double = dy4
            Dim dz3 As Double = dz0 - 3 * SQUISH_CONSTANT_4D
            Dim dw3 As Double = dw0 - 1 - 3 * SQUISH_CONSTANT_4D
            Dim attn3 As Double = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3 - dw3 * dw3
            If (attn3 > 0) Then
                attn3 *= attn3
                value += attn3 * attn3 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 1, dx3, dy3, dz3, dw3)
            End If

            'Contribution (1,0,1,1)
            Dim dx2 As Double = dx4
            Dim dy2 As Double = dy0 - 3 * SQUISH_CONSTANT_4D
            Dim dz2 As Double = dz4
            Dim dw2 As Double = dw3
            Dim attn2 As Double = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2 - dw2 * dw2
            If (attn2 > 0) Then
                attn2 *= attn2
                value += attn2 * attn2 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 1, dx2, dy2, dz2, dw2)
            End If

            'Contribution (0,1,1,1)
            Dim dx1 As Double = dx0 - 3 * SQUISH_CONSTANT_4D
            Dim dz1 As Double = dz4
            Dim dy1 As Double = dy4
            Dim dw1 As Double = dw3
            Dim attn1 As Double = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1 - dw1 * dw1
            If (attn1 > 0) Then
                attn1 *= attn1
                value += attn1 * attn1 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 1, dx1, dy1, dz1, dw1)
            End If

            'Contribution (1,1,0,0)
            Dim dx5 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy5 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz5 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw5 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn5 As Double = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5 - dw5 * dw5
            If (attn5 > 0) Then
                attn5 *= attn5
                value += attn5 * attn5 * Extrapolate(xsb + 1, ysb + 1, zsb + 0, wsb + 0, dx5, dy5, dz5, dw5)
            End If

            'Contribution (1,0,1,0)
            Dim dx6 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy6 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz6 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw6 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn6 As Double = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6 - dw6 * dw6
            If (attn6 > 0) Then
                attn6 *= attn6
                value += attn6 * attn6 * Extrapolate(xsb + 1, ysb + 0, zsb + 1, wsb + 0, dx6, dy6, dz6, dw6)
            End If

            'Contribution (1,0,0,1)
            Dim dx7 As Double = dx0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dy7 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz7 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw7 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn7 As Double = 2 - dx7 * dx7 - dy7 * dy7 - dz7 * dz7 - dw7 * dw7
            If (attn7 > 0) Then
                attn7 *= attn7
                value += attn7 * attn7 * Extrapolate(xsb + 1, ysb + 0, zsb + 0, wsb + 1, dx7, dy7, dz7, dw7)
            End If

            'Contribution (0,1,1,0)
            Dim dx8 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy8 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz8 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw8 As Double = dw0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim attn8 As Double = 2 - dx8 * dx8 - dy8 * dy8 - dz8 * dz8 - dw8 * dw8
            If (attn8 > 0) Then
                attn8 *= attn8
                value += attn8 * attn8 * Extrapolate(xsb + 0, ysb + 1, zsb + 1, wsb + 0, dx8, dy8, dz8, dw8)
            End If

            'Contribution (0,1,0,1)
            Dim dx9 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy9 As Double = dy0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dz9 As Double = dz0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dw9 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn9 As Double = 2 - dx9 * dx9 - dy9 * dy9 - dz9 * dz9 - dw9 * dw9
            If (attn9 > 0) Then
                attn9 *= attn9
                value += attn9 * attn9 * Extrapolate(xsb + 0, ysb + 1, zsb + 0, wsb + 1, dx9, dy9, dz9, dw9)
            End If

            'Contribution (0,0,1,1)
            Dim dx10 As Double = dx0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dy10 As Double = dy0 - 0 - 2 * SQUISH_CONSTANT_4D
            Dim dz10 As Double = dz0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim dw10 As Double = dw0 - 1 - 2 * SQUISH_CONSTANT_4D
            Dim attn10 As Double = 2 - dx10 * dx10 - dy10 * dy10 - dz10 * dz10 - dw10 * dw10
            If (attn10 > 0) Then
                attn10 *= attn10
                value += attn10 * attn10 * Extrapolate(xsb + 0, ysb + 0, zsb + 1, wsb + 1, dx10, dy10, dz10, dw10)
            End If
        End If

        'First extra vertex
        Dim attn_ext0 As Double = 2 - dx_ext0 * dx_ext0 - dy_ext0 * dy_ext0 - dz_ext0 * dz_ext0 - dw_ext0 * dw_ext0
        If (attn_ext0 > 0) Then
            attn_ext0 *= attn_ext0
            value += attn_ext0 * attn_ext0 *
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            Extrapolate(xsv_ext0, ysv_ext0, zsv_ext0, wsv_ext0, dx_ext0, dy_ext0, dz_ext0, dw_ext0)
        End If

        'Second extra vertex
        Dim attn_ext1 As Double = 2 - dx_ext1 * dx_ext1 - dy_ext1 * dy_ext1 - dz_ext1 * dz_ext1 - dw_ext1 * dw_ext1
        If (attn_ext1 > 0) Then
            attn_ext1 *= attn_ext1
            value += attn_ext1 * attn_ext1 *
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                Extrapolate(xsv_ext1, ysv_ext1, zsv_ext1, wsv_ext1, dx_ext1, dy_ext1, dz_ext1, dw_ext1)
        End If

        'Third extra vertex
        Dim attn_ext2 As Double = 2 - dx_ext2 * dx_ext2 - dy_ext2 * dy_ext2 - dz_ext2 * dz_ext2 - dw_ext2 * dw_ext2
        If (attn_ext2 > 0) Then
            attn_ext2 *= attn_ext2
            value += attn_ext2 * attn_ext2 *
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    Extrapolate(xsv_ext2, ysv_ext2, zsv_ext2, wsv_ext2, dx_ext2, dy_ext2, dz_ext2, dw_ext2)
        End If

        Return (value / NORM_CONSTANT_4D + 1) / 2
    End Function

    ''' <summary>
    ''' Calculates a widened 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 4D OpenSimplex Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 4D OpenSimplex Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideSimplex4D(x As Double, y As Double, z As Double, w As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Simplex4D(x, y, z, w, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 4D OpenSimplex Noise value between 0 and 1 for input values x,y,z,w with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="y">The y input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="z">The z input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="w">The w input of the 4D OpenSimplex Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 4D OpenSimplex Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 4D OpenSimplex Noise function.</param>
    Public Shared Function Simplex4D(x As Double, y As Double, z As Double, w As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Simplex4D(x * frequency, y * frequency, z * frequency, w * frequency) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

#Region "Private Functions"

    Private Shared Function Extrapolate(xsb As Integer, ysb As Integer, dx As Double, dy As Double) As Double
        Dim index As Integer = perm((perm(xsb And &HFF) + ysb) And &HFF) And &HE
        Return Gradients2D(index) * dx + Gradients2D(index + 1) * dy
    End Function

    Private Shared Function Extrapolate(xsb As Integer, ysb As Integer, zsb As Integer, dx As Double, dy As Double, dz As Double) As Double
        Dim index As Integer = permGradIndex3D((perm((perm(xsb And &HFF) + ysb) And &HFF) + zsb) And &HFF)
        Return Gradients3D(index) * dx + Gradients3D(index + 1) * dy + Gradients3D(index + 2) * dz
    End Function

    Private Shared Function Extrapolate(xsb As Integer, ysb As Integer, zsb As Integer, wsb As Integer, dx As Double, dy As Double, dz As Double, dw As Double) As Double
        Dim index As Integer = perm((perm((perm((perm(xsb And &HFF) + ysb) And &HFF) + zsb) And &HFF) + wsb) And &HFF) And &HFC
        Return Gradients4D(index) * dx + Gradients4D(index + 1) * dy + Gradients4D(index + 2) * dz + Gradients4D(index + 3) * dw
    End Function

    Private Shared Function FastFloor(x As Double) As Integer
        Dim xi As Integer = CInt(x)
        If x < xi Then
            Return xi - 1
        Else
            Return xi
        End If
    End Function

    'Gradients for 2D. They approximate the directions to the
    'vertices of an octagon from the center.
    Private Shared ReadOnly Gradients2D As SByte() =
        {
            5, 2, 2, 5,
            -5, 2, -2, 5,
            5, -2, 2, -5,
            -5, -2, -2, -5
        }

    'Gradients for 3D. They approximate the directions to the
    'vertices of a rhombicuboctahedron from the center, skewed so
    'that the triangular and square facets can be inscribed inside
    'circles of the same radius.
    Private Shared ReadOnly Gradients3D As SByte() =
    {
        -11, 4, 4, -4, 11, 4, -4, 4, 11,
        11, 4, 4, 4, 11, 4, 4, 4, 11,
        -11, -4, 4, -4, -11, 4, -4, -4, 11,
        11, -4, 4, 4, -11, 4, 4, -4, 11,
        -11, 4, -4, -4, 11, -4, -4, 4, -11,
        11, 4, -4, 4, 11, -4, 4, 4, -11,
        -11, -4, -4, -4, -11, -4, -4, -4, -11,
        11, -4, -4, 4, -11, -4, 4, -4, -11
    }

    'Gradients for 4D. They approximate the directions to the
    'vertices of a disprismatotesseractihexadecachoron from the center,
    'skewed so that the tetrahedral and cubic facets can be inscribed inside
    'spheres of the same radius.
    Private Shared ReadOnly Gradients4D As SByte() =
    {
        3, 1, 1, 1, 1, 3, 1, 1, 1, 1, 3, 1, 1, 1, 1, 3,
        -3, 1, 1, 1, -1, 3, 1, 1, -1, 1, 3, 1, -1, 1, 1, 3,
        3, -1, 1, 1, 1, -3, 1, 1, 1, -1, 3, 1, 1, -1, 1, 3,
        -3, -1, 1, 1, -1, -3, 1, 1, -1, -1, 3, 1, -1, -1, 1, 3,
        3, 1, -1, 1, 1, 3, -1, 1, 1, 1, -3, 1, 1, 1, -1, 3,
        -3, 1, -1, 1, -1, 3, -1, 1, -1, 1, -3, 1, -1, 1, -1, 3,
        3, -1, -1, 1, 1, -3, -1, 1, 1, -1, -3, 1, 1, -1, -1, 3,
        -3, -1, -1, 1, -1, -3, -1, 1, -1, -1, -3, 1, -1, -1, -1, 3,
        3, 1, 1, -1, 1, 3, 1, -1, 1, 1, 3, -1, 1, 1, 1, -3,
        -3, 1, 1, -1, -1, 3, 1, -1, -1, 1, 3, -1, -1, 1, 1, -3,
        3, -1, 1, -1, 1, -3, 1, -1, 1, -1, 3, -1, 1, -1, 1, -3,
        -3, -1, 1, -1, -1, -3, 1, -1, -1, -1, 3, -1, -1, -1, 1, -3,
        3, 1, -1, -1, 1, 3, -1, -1, 1, 1, -3, -1, 1, 1, -1, -3,
        -3, 1, -1, -1, -1, 3, -1, -1, -1, 1, -3, -1, -1, 1, -1, -3,
        3, -1, -1, -1, 1, -3, -1, -1, 1, -1, -3, -1, 1, -1, -1, -3,
        -3, -1, -1, -1, -1, -3, -1, -1, -1, -1, -3, -1, -1, -1, -1, -3
    }

    ''' <summary>
    ''' Stretches a value toward the ends of an interval (uses the Fade function defined by K. Perlin).
    ''' </summary>
    ''' <param name="x">value that will be stretched</param>
    ''' <param name="lower">Lower end of the interval</param>
    ''' <param name="upper">Upper end of the interval</param>
    ''' <param name="Strength">Number of times the Fade function is applied.</param>
    ''' <returns></returns>
    Private Shared Function SmoothStep(x As Double, lower As Double, upper As Double, Strength As Integer) As Double
        'Make sure lower <= upper
        If lower > upper Then
            Dim dummy As Double
            dummy = lower
            lower = upper
            upper = dummy
        End If
        'Lock x between lower and upper
        If x < lower Then x = lower
        If x > upper Then x = upper
        'Scale x between 0 and 1
        x = (x - lower) / (upper - lower)
        'Widen x
        For I As Integer = 0 To Strength - 1
            x = x * x * x * (x * (x * 6 - 15) + 10)
        Next
        'Scale x back between lower and upper
        Return (upper - lower) * x + lower
    End Function

#End Region

End Class

