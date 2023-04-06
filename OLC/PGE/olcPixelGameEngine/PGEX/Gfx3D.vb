Option Explicit On
Option Strict On
Option Infer On

Imports Olc.PixelGameEngine

' Container class for Advanced 3D Drawing functions
Public Class Gfx3D
  Inherits PgeX

  Private Shared m_DepthBuffer As Single()

  Public Shared Sub ConfigureDisplay()
    m_DepthBuffer = New Single(Pge.ScreenWidth() * Pge.ScreenHeight() - 1) {}
  End Sub

  Public Shared Sub ClearDepth()
    Array.Clear(m_DepthBuffer, 0, m_DepthBuffer.Length)
  End Sub

  Public Shared Sub AddTriangleToScene(ByRef tri As Triangle)
    Throw New NotImplementedException()
  End Sub

  Public Shared Sub RenderScene()
    Throw New NotImplementedException()
  End Sub

  Public Shared Sub DrawTriangleFlat(tri As Triangle)
    Pge.FillTriangle(tri.P(0).X, tri.P(0).Y, tri.P(1).X, tri.P(1).Y, tri.P(2).X, tri.P(2).Y, tri.Col)
  End Sub

  Public Shared Sub DrawTriangleWire(tri As Triangle)
    DrawTriangleWire(tri, Presets.White)
  End Sub

  Public Shared Sub DrawTriangleWire(tri As Triangle, col As Pixel)
    Pge.DrawTriangle(tri.P(0).X, tri.P(0).Y, tri.P(1).X, tri.P(1).Y, tri.P(2).X, tri.P(2).Y, col)
  End Sub

  Public Shared Sub DrawTriangleTex(tri As Triangle, spr As Sprite)

    If tri.P(1).Y < tri.P(0).Y Then
      Swap(tri.P(0).Y, tri.P(1).Y)
      Swap(tri.P(0).X, tri.P(1).X)
      Swap(tri.T(0).X, tri.T(1).X)
      Swap(tri.T(0).Y, tri.T(1).Y)
      Swap(tri.T(0).Z, tri.T(1).Z)
    End If

    If tri.P(2).Y < tri.P(0).Y Then
      Swap(tri.P(0).Y, tri.P(2).Y)
      Swap(tri.P(0).X, tri.P(2).X)
      Swap(tri.T(0).X, tri.T(2).X)
      Swap(tri.T(0).Y, tri.T(2).Y)
      Swap(tri.T(0).Z, tri.T(2).Z)
    End If

    If tri.P(2).Y < tri.P(1).Y Then
      Swap(tri.P(1).Y, tri.P(2).Y)
      Swap(tri.P(1).X, tri.P(2).X)
      Swap(tri.T(1).X, tri.T(2).X)
      Swap(tri.T(1).Y, tri.T(2).Y)
      Swap(tri.T(1).Z, tri.T(2).Z)
    End If

    Dim dy1 = CInt(Fix(tri.P(1).Y - tri.P(0).Y))
    Dim dx1 = CInt(Fix(tri.P(1).X - tri.P(0).X))
    Dim dv1 = tri.T(1).Y - tri.T(0).Y
    Dim du1 = tri.T(1).X - tri.T(0).X
    Dim dz1 = tri.T(1).Z - tri.T(0).Z

    Dim dy2 = CInt(Fix(tri.P(2).Y - tri.P(0).Y))
    Dim dx2 = CInt(Fix(tri.P(2).X - tri.P(0).X))
    Dim dv2 = tri.T(2).Y - tri.T(0).Y
    Dim du2 = tri.T(2).X - tri.T(0).X
    Dim dz2 = tri.T(2).Z - tri.T(0).Z

    Dim tex_x, tex_y, tex_z As Single

    Dim du1_step = 0.0F, dv1_step = 0.0F, du2_step = 0.0F, dv2_step = 0.0F, dz1_step = 0.0F, dz2_step = 0.0F
    Dim dax_step = 0.0F, dbx_step = 0.0F

    If dy1 <> 0 Then dax_step = CInt(Fix(dx1 / System.Math.Abs(dy1)))
    If dy2 <> 0 Then dbx_step = CInt(Fix(dx2 / System.Math.Abs(dy2)))

    If dy1 <> 0 Then du1_step = du1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dv1_step = dv1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dz1_step = dz1 / System.Math.Abs(dy1)

    If dy2 <> 0 Then du2_step = du2 / System.Math.Abs(dy2)
    If dy2 <> 0 Then dv2_step = dv2 / System.Math.Abs(dy2)
    If dy2 <> 0 Then dz2_step = dz2 / System.Math.Abs(dy2)

    If dy1 <> 0 Then

      For i = CInt(Fix(tri.P(0).Y)) To CInt(Fix(tri.P(1).Y))

        Dim ax = CInt(Fix(tri.P(0).X + (i - tri.P(0).Y) * dax_step))
        Dim bx = CInt(Fix(tri.P(0).X + (i - tri.P(0).Y) * dbx_step))

        ' Start and end points in texture space
        Dim tex_su = tri.T(0).X + (i - tri.P(0).Y) * du1_step
        Dim tex_sv = tri.T(0).Y + (i - tri.P(0).Y) * dv1_step
        Dim tex_sz = tri.T(0).Z + (i - tri.P(0).Y) * dz1_step

        Dim tex_eu = tri.T(0).X + (i - tri.P(0).Y) * du2_step
        Dim tex_ev = tri.T(0).Y + (i - tri.P(0).Y) * dv2_step
        Dim tex_ez = tri.T(0).Z + (i - tri.P(0).Y) * dz2_step

        If ax > bx Then
          Swap(ax, bx)
          Swap(tex_su, tex_eu)
          Swap(tex_sv, tex_ev)
          Swap(tex_sz, tex_ez)
        End If

        'tex_x = tex_su
        'tex_y = tex_sv
        'tex_z = tex_sz

        Dim tstep = 1.0F / (bx - ax)
        Dim t = 0.0F

        For j = ax To bx - 1

          tex_x = (1.0F - t) * tex_su + t * tex_eu
          tex_y = (1.0F - t) * tex_sv + t * tex_ev
          tex_z = (1.0F - t) * tex_sz + t * tex_ez

          If tex_z > m_DepthBuffer(i * Pge.ScreenWidth() + j) Then
            Pge.Draw(j, i, spr.Sample(tex_x / tex_z, tex_y / tex_z))
            m_DepthBuffer(i * Pge.ScreenWidth() + j) = tex_z
          End If
          t += tstep

        Next

      Next

    End If

    dy1 = CInt(Fix(tri.P(2).Y - tri.P(1).Y))
    dx1 = CInt(Fix(tri.P(2).X - tri.P(1).X))
    dv1 = tri.T(2).Y - tri.T(1).Y
    du1 = tri.T(2).X - tri.T(1).X
    dz1 = tri.T(2).Z - tri.T(1).Z

    If dy1 <> 0 Then dax_step = CSng(dx1 / System.Math.Abs(dy1))
    If dy2 <> 0 Then dbx_step = CSng(dx2 / System.Math.Abs(dy2))

    du1_step = 0 : dv1_step = 0 ' , dz1_step = 0;// , du2_step = 0, dv2_step = 0;
    If dy1 <> 0 Then
      du1_step = du1 / System.Math.Abs(dy1)
      dv1_step = dv1 / System.Math.Abs(dy1)
      dz1_step = dz1 / System.Math.Abs(dy1)
    End If

    If dy1 <> 0 Then

      For i = CInt(Fix(tri.P(1).Y)) To CInt(Fix(tri.P(2).Y))

        Dim ax = CInt(Fix(tri.P(1).X + (i - tri.P(1).Y) * dax_step))
        Dim bx = CInt(Fix(tri.P(0).X + (i - tri.P(0).Y) * dbx_step))

        ' Start and end points in texture space
        Dim tex_su = tri.T(1).X + (i - tri.P(1).Y) * du1_step
        Dim tex_sv = tri.T(1).Y + (i - tri.P(1).Y) * dv1_step
        Dim tex_sz = tri.T(1).Z + (i - tri.P(1).Y) * dz1_step

        Dim tex_eu = tri.T(0).X + (i - tri.P(0).Y) * du2_step
        Dim tex_ev = tri.T(0).Y + (i - tri.P(0).Y) * dv2_step
        Dim tex_ez = tri.T(0).Z + (i - tri.P(0).Y) * dz2_step

        If ax > bx Then
          Swap(ax, bx)
          Swap(tex_su, tex_eu)
          Swap(tex_sv, tex_ev)
          Swap(tex_sz, tex_ez)
        End If

        'tex_x = tex_su
        'tex_y = tex_sv
        'tex_z = tex_sz

        Dim tstep = 1.0F / ((bx - ax))
        Dim t = 0.0F

        For j = ax To bx - 1

          tex_x = (1.0F - t) * tex_su + t * tex_eu
          tex_y = (1.0F - t) * tex_sv + t * tex_ev
          tex_z = (1.0F - t) * tex_sz + t * tex_ez

          If tex_z > m_DepthBuffer(i * Pge.ScreenWidth + j) Then
            Pge.Draw(j, i, spr.Sample(tex_x / tex_z, tex_y / tex_z))
            m_DepthBuffer(i * Pge.ScreenWidth + j) = tex_z
          End If

          t += tstep

        Next

      Next

    End If

    '*

  End Sub

  Public Shared Sub TexturedTriangle(x1 As Integer, y1 As Integer, u1 As Single, v1 As Single, w1 As Single, x2 As Integer, y2 As Integer, u2 As Single, v2 As Single, w2 As Single, x3 As Integer, y3 As Integer, u3 As Single, v3 As Single, w3 As Single, spr As Sprite)

    If y2 < y1 Then
      Swap(y1, y2)
      Swap(x1, x2)
      Swap(u1, u2)
      Swap(v1, v2)
      Swap(w1, w2)
    End If

    If y3 < y1 Then
      Swap(y1, y3)
      Swap(x1, x3)
      Swap(u1, u3)
      Swap(v1, v3)
      Swap(w1, w3)
    End If

    If y3 < y2 Then
      Swap(y2, y3)
      Swap(x2, x3)
      Swap(u2, u3)
      Swap(v2, v3)
      Swap(w2, w3)
    End If

    Dim dy1 = y2 - y1
    Dim dx1 = x2 - x1
    Dim dv1 = v2 - v1
    Dim du1 = u2 - u1
    Dim dw1 = w2 - w1

    Dim dy2 = y3 - y1
    Dim dx2 = x3 - x1
    Dim dv2 = v3 - v1
    Dim du2 = u3 - u1
    Dim dw2 = w3 - w1

    Dim tex_u, tex_v, tex_w As Single

    Dim dax_step = 0.0F, dbx_step = 0.0F,
        du1_step = 0.0F, dv1_step = 0.0F,
        du2_step = 0.0F, dv2_step = 0.0F,
        dw1_step = 0.0F, dw2_step = 0.0F

    If dy1 <> 0 Then dax_step = CSng(dx1 / System.Math.Abs(dy1))
    If dy2 <> 0 Then dbx_step = CSng(dx2 / System.Math.Abs(dy2))

    If dy1 <> 0 Then du1_step = du1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dv1_step = dv1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dw1_step = dw1 / System.Math.Abs(dy1)

    If dy2 <> 0 Then du2_step = du2 / System.Math.Abs(dy2)
    If dy2 <> 0 Then dv2_step = dv2 / System.Math.Abs(dy2)
    If dy2 <> 0 Then dw2_step = dw2 / System.Math.Abs(dy2)

    If dy1 <> 0 Then

      For i = y1 To y2

        Dim ax = CInt(Fix(x1 + (i - y1) * dax_step))
        Dim bx = CInt(Fix(x1 + (i - y1) * dbx_step))

        Dim tex_su = u1 + (i - y1) * du1_step
        Dim tex_sv = v1 + (i - y1) * dv1_step
        Dim tex_sw = w1 + (i - y1) * dw1_step

        Dim tex_eu = u1 + (i - y1) * du2_step
        Dim tex_ev = v1 + (i - y1) * dv2_step
        Dim tex_ew = w1 + (i - y1) * dw2_step

        If ax > bx Then
          Swap(ax, bx)
          Swap(tex_su, tex_eu)
          Swap(tex_sv, tex_ev)
          Swap(tex_sw, tex_ew)
        End If

        'tex_u = tex_su
        'tex_v = tex_sv
        'tex_w = tex_sw

        Dim tstep = 1.0F / (bx - ax)
        Dim t = 0.0F

        For j = ax To bx - 1
          tex_u = (1.0F - t) * tex_su + t * tex_eu
          tex_v = (1.0F - t) * tex_sv + t * tex_ev
          tex_w = (1.0F - t) * tex_sw + t * tex_ew
          If tex_w > m_DepthBuffer(i * Pge.ScreenWidth() + j) Then
            Pge.Draw(j, i, spr.Sample(tex_u / tex_w, tex_v / tex_w))
            m_DepthBuffer(i * Pge.ScreenWidth() + j) = tex_w
          End If
          t += tstep
        Next

      Next

    End If

    dy1 = y3 - y2
    dx1 = x3 - x2
    dv1 = v3 - v2
    du1 = u3 - u2
    dw1 = w3 - w2

    If dy1 <> 0 Then dax_step = CSng(dx1 / System.Math.Abs(dy1))
    If dy2 <> 0 Then dbx_step = CSng(dx2 / System.Math.Abs(dy2))

    If dy1 <> 0 Then du1_step = du1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dv1_step = dv1 / System.Math.Abs(dy1)
    If dy1 <> 0 Then dw1_step = dw1 / System.Math.Abs(dy1)

    If dy1 <> 0 Then

      For i = y2 To y3

        Dim ax = CInt(Fix(x2 + (i - y2) * dax_step))
        Dim bx = CInt(Fix(x1 + (i - y1) * dbx_step))

        Dim tex_su = u2 + (i - y2) * du1_step
        Dim tex_sv = v2 + (i - y2) * dv1_step
        Dim tex_sw = w2 + (i - y2) * dw1_step

        Dim tex_eu = u1 + (i - y1) * du2_step
        Dim tex_ev = v1 + (i - y1) * dv2_step
        Dim tex_ew = w1 + (i - y1) * dw2_step

        If ax > bx Then
          Swap(ax, bx)
          Swap(tex_su, tex_eu)
          Swap(tex_sv, tex_ev)
          Swap(tex_sw, tex_ew)
        End If

        'tex_u = tex_su
        'tex_v = tex_sv
        'tex_w = tex_sw

        Dim tstep = 1.0F / (bx - ax)
        Dim t = 0.0F

        For j = ax To bx - 1

          tex_u = (1.0F - t) * tex_su + t * tex_eu
          tex_v = (1.0F - t) * tex_sv + t * tex_ev
          tex_w = (1.0F - t) * tex_sw + t * tex_ew

          If tex_w > m_DepthBuffer(i * Pge.ScreenWidth() + j) Then
            Pge.Draw(j, i, spr.Sample(tex_u / tex_w, tex_v / tex_w))
            m_DepthBuffer(i * Pge.ScreenWidth() + j) = tex_w
          End If
          t += tstep

        Next

      Next

    End If

  End Sub

  Public Enum RenderFlags
    RenderWire = &H1
    RenderFlat = &H2
    RenderTextured = &H4
    RenderCullCw = &H8
    RenderCullCcw = &H10
    RenderDepth = &H20
  End Enum

  Public Class Vec2d

    Public X As Single
    Public Y As Single
    Public Z As Single

    Sub New()
      X = 0.0F
      Y = 0.0F
      Z = 0.0F
    End Sub

    Sub New(x As Single, y As Single, z As Single)
      Me.X = x : Me.Y = y : Me.Z = z
    End Sub

  End Class

  Public Structure Vec3d
    Public X As Single
    Public Y As Single
    Public Z As Single
    Public W As Single 'Need a 4th term to perform sensible matrix vector multiplication
    Public Sub New(x As Single, y As Single, z As Single)
      Me.X = x : Me.Y = y : Me.Z = z : Me.W = 1.0F
    End Sub
    Public Sub New(x As Single, y As Single, z As Single, w As Single)
      Me.X = x : Me.Y = y : Me.Z = z : Me.W = w
    End Sub
  End Structure

  Public Class Triangle

    Public P(2) As Vec3d
    Public T(2) As Vec2d
    Public Col As Pixel

    Public Sub New()
      P(0) = New Vec3d
      P(1) = New Vec3d
      P(2) = New Vec3d
      T(0) = New Vec2d
      T(1) = New Vec2d
      T(2) = New Vec2d
    End Sub

    Public Sub New(values As Single())
      Dim index = 0
      For entry = 0 To 2
        P(entry) = New Vec3d(values(index), values(index + 1), values(index + 2), values(index + 3)) : index += 4
      Next
      For entry = 0 To 2
        T(entry) = New Vec2d(values(index), values(index + 1), values(index + 2)) : index += 3
      Next
    End Sub

  End Class

  Public Class Mat4x4
    Public M(3, 3) As Single
  End Class

  Public Class Mesh
    Public Tris As List(Of Triangle)
  End Class

  Public Class Math

    Public Sub New()
    End Sub

    Public Shared Function Mat_MultiplyVector(m As Mat4x4, i As Vec3d) As Vec3d
      Dim v As New Vec3d
      v.X = i.X * m.M(0, 0) + i.Y * m.M(1, 0) + i.Z * m.M(2, 0) + i.W * m.M(3, 0)
      v.Y = i.X * m.M(0, 1) + i.Y * m.M(1, 1) + i.Z * m.M(2, 1) + i.W * m.M(3, 1)
      v.Z = i.X * m.M(0, 2) + i.Y * m.M(1, 2) + i.Z * m.M(2, 2) + i.W * m.M(3, 2)
      v.W = i.X * m.M(0, 3) + i.Y * m.M(1, 3) + i.Z * m.M(2, 3) + i.W * m.M(3, 3)
      Return v
    End Function

    Public Shared Function Mat_MultiplyMatrix(m1 As Mat4x4, m2 As Mat4x4) As Mat4x4
      Dim matrix As New Mat4x4
      For c = 0 To 3
        For r = 0 To 3
          matrix.M(r, c) = m1.M(r, 0) * m2.M(0, c) + m1.M(r, 1) * m2.M(1, c) + m1.M(r, 2) * m2.M(2, c) + m1.M(r, 3) * m2.M(3, c)
        Next
      Next
      Return matrix
    End Function

    Public Shared Function Mat_MakeIdentity() As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = 1.0F
      matrix.M(1, 1) = 1.0F
      matrix.M(2, 2) = 1.0F
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_MakeRotationX(angleRad As Single) As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = 1.0F
      matrix.M(1, 1) = MathF.Cos(angleRad)
      matrix.M(1, 2) = MathF.Sin(angleRad)
      matrix.M(2, 1) = -MathF.Sin(angleRad)
      matrix.M(2, 2) = MathF.Cos(angleRad)
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_MakeRotationY(angleRad As Single) As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = MathF.Cos(angleRad)
      matrix.M(0, 2) = MathF.Sin(angleRad)
      matrix.M(2, 0) = -MathF.Sin(angleRad)
      matrix.M(1, 1) = 1.0F
      matrix.M(2, 2) = MathF.Cos(angleRad)
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_MakeRotationZ(angleRad As Single) As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = MathF.Cos(angleRad)
      matrix.M(0, 1) = MathF.Sin(angleRad)
      matrix.M(1, 0) = -MathF.Sin(angleRad)
      matrix.M(1, 1) = MathF.Cos(angleRad)
      matrix.M(2, 2) = 1.0F
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_MakeScale(x As Single, y As Single, z As Single) As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = x
      matrix.M(1, 1) = y
      matrix.M(2, 2) = z
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_MakeTranslation(x As Single, y As Single, z As Single) As Mat4x4
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = 1.0F
      matrix.M(1, 1) = 1.0F
      matrix.M(2, 2) = 1.0F
      matrix.M(3, 3) = 1.0F
      matrix.M(3, 0) = x
      matrix.M(3, 1) = y
      matrix.M(3, 2) = z
      Return matrix
    End Function

    Public Shared Function Mat_MakeProjection(fovDegrees As Single, aspectRatio As Single, near As Single, far As Single) As Mat4x4
      Dim fFovRad = 1.0F / MathF.Tan(fovDegrees * 0.5F / 180.0F * 3.14159F)
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = aspectRatio * fFovRad
      matrix.M(1, 1) = fFovRad
      matrix.M(2, 2) = far / (far - near)
      matrix.M(3, 2) = (-far * near) / (far - near)
      matrix.M(2, 3) = 1.0F
      matrix.M(3, 3) = 0.0F
      Return matrix
    End Function

    Public Shared Function Mat_PointAt(pos As Vec3d, target As Vec3d, up As Vec3d) As Mat4x4

      ' Calculate new forward direction
      Dim newForward = Vec_Sub(target, pos)
      newForward = Vec_Normalise(newForward)

      ' Calculate new Up direction
      Dim a = Vec_Mul(newForward, Vec_DotProduct(up, newForward))
      Dim newUp = Vec_Sub(up, a)
      newUp = Vec_Normalise(newUp)

      ' New Right direction is easy, its just cross product
      Dim newRight = Vec_CrossProduct(newUp, newForward)

      ' Construct Dimensioning and Translation Matrix
      Dim matrix As New Mat4x4
      matrix.M(0, 0) = newRight.X
      matrix.M(0, 1) = newRight.Y
      matrix.M(0, 2) = newRight.Z
      matrix.M(0, 3) = 0.0F
      matrix.M(1, 0) = newUp.X
      matrix.M(1, 1) = newUp.Y
      matrix.M(1, 2) = newUp.Z
      matrix.M(1, 3) = 0.0F
      matrix.M(2, 0) = newForward.X
      matrix.M(2, 1) = newForward.Y
      matrix.M(2, 2) = newForward.Z
      matrix.M(2, 3) = 0.0F
      matrix.M(3, 0) = pos.X
      matrix.M(3, 1) = pos.Y
      matrix.M(3, 2) = pos.Z
      matrix.M(3, 3) = 1.0F

      Return matrix

    End Function

    Public Shared Function Mat_QuickInverse(m As Mat4x4) As Mat4x4
      Dim matrix As New Mat4x4()
      matrix.M(0, 0) = m.M(0, 0) : matrix.M(0, 1) = m.M(1, 0) : matrix.M(0, 2) = m.M(2, 0) : matrix.M(0, 3) = 0.0F
      matrix.M(1, 0) = m.M(0, 1) : matrix.M(1, 1) = m.M(1, 1) : matrix.M(1, 2) = m.M(2, 1) : matrix.M(1, 3) = 0.0F
      matrix.M(2, 0) = m.M(0, 2) : matrix.M(2, 1) = m.M(1, 2) : matrix.M(2, 2) = m.M(2, 2) : matrix.M(2, 3) = 0.0F
      matrix.M(3, 0) = -(m.M(3, 0) * matrix.M(0, 0) + m.M(3, 1) * matrix.M(1, 0) + m.M(3, 2) * matrix.M(2, 0))
      matrix.M(3, 1) = -(m.M(3, 0) * matrix.M(0, 1) + m.M(3, 1) * matrix.M(1, 1) + m.M(3, 2) * matrix.M(2, 1))
      matrix.M(3, 2) = -(m.M(3, 0) * matrix.M(0, 2) + m.M(3, 1) * matrix.M(1, 2) + m.M(3, 2) * matrix.M(2, 2))
      matrix.M(3, 3) = 1.0F
      Return matrix
    End Function

    Public Shared Function Mat_Inverse(m As Mat4x4) As Mat4x4

      Dim matInv = New Mat4x4

      matInv.M(0, 0) = m.M(1, 1) * m.M(2, 2) * m.M(3, 3) - m.M(1, 1) * m.M(2, 3) * m.M(3, 2) - m.M(2, 1) * m.M(1, 2) * m.M(3, 3) + m.M(2, 1) * m.M(1, 3) * m.M(3, 2) + m.M(3, 1) * m.M(1, 2) * m.M(2, 3) - m.M(3, 1) * m.M(1, 3) * m.M(2, 2)
      matInv.M(1, 0) = -m.M(1, 0) * m.M(2, 2) * m.M(3, 3) + m.M(1, 0) * m.M(2, 3) * m.M(3, 2) + m.M(2, 0) * m.M(1, 2) * m.M(3, 3) - m.M(2, 0) * m.M(1, 3) * m.M(3, 2) - m.M(3, 0) * m.M(1, 2) * m.M(2, 3) + m.M(3, 0) * m.M(1, 3) * m.M(2, 2)
      matInv.M(2, 0) = m.M(1, 0) * m.M(2, 1) * m.M(3, 3) - m.M(1, 0) * m.M(2, 3) * m.M(3, 1) - m.M(2, 0) * m.M(1, 1) * m.M(3, 3) + m.M(2, 0) * m.M(1, 3) * m.M(3, 1) + m.M(3, 0) * m.M(1, 1) * m.M(2, 3) - m.M(3, 0) * m.M(1, 3) * m.M(2, 1)
      matInv.M(3, 0) = -m.M(1, 0) * m.M(2, 1) * m.M(3, 2) + m.M(1, 0) * m.M(2, 2) * m.M(3, 1) + m.M(2, 0) * m.M(1, 1) * m.M(3, 2) - m.M(2, 0) * m.M(1, 2) * m.M(3, 1) - m.M(3, 0) * m.M(1, 1) * m.M(2, 2) + m.M(3, 0) * m.M(1, 2) * m.M(2, 1)
      matInv.M(0, 1) = -m.M(0, 1) * m.M(2, 2) * m.M(3, 3) + m.M(0, 1) * m.M(2, 3) * m.M(3, 2) + m.M(2, 1) * m.M(0, 2) * m.M(3, 3) - m.M(2, 1) * m.M(0, 3) * m.M(3, 2) - m.M(3, 1) * m.M(0, 2) * m.M(2, 3) + m.M(3, 1) * m.M(0, 3) * m.M(2, 2)
      matInv.M(1, 1) = m.M(0, 0) * m.M(2, 2) * m.M(3, 3) - m.M(0, 0) * m.M(2, 3) * m.M(3, 2) - m.M(2, 0) * m.M(0, 2) * m.M(3, 3) + m.M(2, 0) * m.M(0, 3) * m.M(3, 2) + m.M(3, 0) * m.M(0, 2) * m.M(2, 3) - m.M(3, 0) * m.M(0, 3) * m.M(2, 2)
      matInv.M(2, 1) = -m.M(0, 0) * m.M(2, 1) * m.M(3, 3) + m.M(0, 0) * m.M(2, 3) * m.M(3, 1) + m.M(2, 0) * m.M(0, 1) * m.M(3, 3) - m.M(2, 0) * m.M(0, 3) * m.M(3, 1) - m.M(3, 0) * m.M(0, 1) * m.M(2, 3) + m.M(3, 0) * m.M(0, 3) * m.M(2, 1)
      matInv.M(3, 1) = m.M(0, 0) * m.M(2, 1) * m.M(3, 2) - m.M(0, 0) * m.M(2, 2) * m.M(3, 1) - m.M(2, 0) * m.M(0, 1) * m.M(3, 2) + m.M(2, 0) * m.M(0, 2) * m.M(3, 1) + m.M(3, 0) * m.M(0, 1) * m.M(2, 2) - m.M(3, 0) * m.M(0, 2) * m.M(2, 1)
      matInv.M(0, 2) = m.M(0, 1) * m.M(1, 2) * m.M(3, 3) - m.M(0, 1) * m.M(1, 3) * m.M(3, 2) - m.M(1, 1) * m.M(0, 2) * m.M(3, 3) + m.M(1, 1) * m.M(0, 3) * m.M(3, 2) + m.M(3, 1) * m.M(0, 2) * m.M(1, 3) - m.M(3, 1) * m.M(0, 3) * m.M(1, 2)
      matInv.M(1, 2) = -m.M(0, 0) * m.M(1, 2) * m.M(3, 3) + m.M(0, 0) * m.M(1, 3) * m.M(3, 2) + m.M(1, 0) * m.M(0, 2) * m.M(3, 3) - m.M(1, 0) * m.M(0, 3) * m.M(3, 2) - m.M(3, 0) * m.M(0, 2) * m.M(1, 3) + m.M(3, 0) * m.M(0, 3) * m.M(1, 2)
      matInv.M(2, 2) = m.M(0, 0) * m.M(1, 1) * m.M(3, 3) - m.M(0, 0) * m.M(1, 3) * m.M(3, 1) - m.M(1, 0) * m.M(0, 1) * m.M(3, 3) + m.M(1, 0) * m.M(0, 3) * m.M(3, 1) + m.M(3, 0) * m.M(0, 1) * m.M(1, 3) - m.M(3, 0) * m.M(0, 3) * m.M(1, 1)
      matInv.M(3, 2) = -m.M(0, 0) * m.M(1, 1) * m.M(3, 2) + m.M(0, 0) * m.M(1, 2) * m.M(3, 1) + m.M(1, 0) * m.M(0, 1) * m.M(3, 2) - m.M(1, 0) * m.M(0, 2) * m.M(3, 1) - m.M(3, 0) * m.M(0, 1) * m.M(1, 2) + m.M(3, 0) * m.M(0, 2) * m.M(1, 1)
      matInv.M(0, 3) = -m.M(0, 1) * m.M(1, 2) * m.M(2, 3) + m.M(0, 1) * m.M(1, 3) * m.M(2, 2) + m.M(1, 1) * m.M(0, 2) * m.M(2, 3) - m.M(1, 1) * m.M(0, 3) * m.M(2, 2) - m.M(2, 1) * m.M(0, 2) * m.M(1, 3) + m.M(2, 1) * m.M(0, 3) * m.M(1, 2)
      matInv.M(1, 3) = m.M(0, 0) * m.M(1, 2) * m.M(2, 3) - m.M(0, 0) * m.M(1, 3) * m.M(2, 2) - m.M(1, 0) * m.M(0, 2) * m.M(2, 3) + m.M(1, 0) * m.M(0, 3) * m.M(2, 2) + m.M(2, 0) * m.M(0, 2) * m.M(1, 3) - m.M(2, 0) * m.M(0, 3) * m.M(1, 2)
      matInv.M(2, 3) = -m.M(0, 0) * m.M(1, 1) * m.M(2, 3) + m.M(0, 0) * m.M(1, 3) * m.M(2, 1) + m.M(1, 0) * m.M(0, 1) * m.M(2, 3) - m.M(1, 0) * m.M(0, 3) * m.M(2, 1) - m.M(2, 0) * m.M(0, 1) * m.M(1, 3) + m.M(2, 0) * m.M(0, 3) * m.M(1, 1)
      matInv.M(3, 3) = m.M(0, 0) * m.M(1, 1) * m.M(2, 2) - m.M(0, 0) * m.M(1, 2) * m.M(2, 1) - m.M(1, 0) * m.M(0, 1) * m.M(2, 2) + m.M(1, 0) * m.M(0, 2) * m.M(2, 1) + m.M(2, 0) * m.M(0, 1) * m.M(1, 2) - m.M(2, 0) * m.M(0, 2) * m.M(1, 1)

      Dim det As Double = m.M(0, 0) * matInv.M(0, 0) + m.M(0, 1) * matInv.M(1, 0) + m.M(0, 2) * matInv.M(2, 0) + m.M(0, 3) * matInv.M(3, 0)

      'If det = 0 Then Return False

      det = 1.0 / det

      For i = 0 To 3
        For j = 0 To 3
          matInv.M(i, j) *= CSng(det)
        Next j
      Next i

      Return matInv

    End Function

    Public Shared Function Vec_Add(v1 As Vec3d, v2 As Vec3d) As Vec3d
      Return New Vec3d(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z)
    End Function

    Public Shared Function Vec_Sub(v1 As Vec3d, v2 As Vec3d) As Vec3d
      Return New Vec3d(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z)
    End Function

    Public Shared Function Vec_Mul(v1 As Vec3d, k As Single) As Vec3d
      Return New Vec3d(v1.X * k, v1.Y * k, v1.Z * k)
    End Function

    Public Shared Function Vec_Div(v1 As Vec3d, k As Single) As Vec3d
      Return New Vec3d(v1.X / k, v1.Y / k, v1.Z / k)
    End Function

    Public Shared Function Vec_DotProduct(v1 As Vec3d, v2 As Vec3d) As Single
      Return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z
    End Function

    Public Shared Function Vec_Length(v As Vec3d) As Single
      Return MathF.Sqrt(Vec_DotProduct(v, v))
    End Function

    Public Shared Function Vec_Normalise(v As Vec3d) As Vec3d
      Dim l = Vec_Length(v)
      Return New Vec3d(v.X / l, v.Y / l, v.Z / l)
    End Function

    Public Shared Function Vec_CrossProduct(v1 As Vec3d, v2 As Vec3d) As Vec3d
      Dim v As New Vec3d
      v.X = v1.Y * v2.Z - v1.Z * v2.Y
      v.Y = v1.Z * v2.X - v1.X * v2.Z
      v.Z = v1.X * v2.Y - v1.Y * v2.X
      Return v
    End Function

    Public Shared Function Vec_IntersectPlane(plane_p As Vec3d, plane_n As Vec3d, lineStart As Vec3d, lineEnd As Vec3d, t As Single) As Vec3d
      plane_n = Vec_Normalise(plane_n)
      Dim plane_d = -Vec_DotProduct(plane_n, plane_p)
      Dim ad = Vec_DotProduct(lineStart, plane_n)
      Dim bd = Vec_DotProduct(lineEnd, plane_n)
      t = (-plane_d - ad) / (bd - ad)
      Dim lineStartToEnd = Vec_Sub(lineEnd, lineStart)
      Dim lineToIntersect = Vec_Mul(lineStartToEnd, t)
      Return Vec_Add(lineStart, lineToIntersect)
    End Function

    Public Shared Function Triangle_ClipAgainstPlane(plane_p As Vec3d, plane_n As Vec3d, in_tri As Triangle, out_tri1 As Triangle, out_tri2 As Triangle) As Integer

      ' Make sure plane normal is indeed normal
      plane_n = Vec_Normalise(plane_n)

      out_tri1.T(0) = in_tri.T(0)
      out_tri2.T(0) = in_tri.T(0)
      out_tri1.T(1) = in_tri.T(1)
      out_tri2.T(1) = in_tri.T(1)
      out_tri1.T(2) = in_tri.T(2)
      out_tri2.T(2) = in_tri.T(2)

      ' Return signed shortest distance from point to plane, plane normal must be normalised
      Dim dist = Function(p As Vec3d)
                   Dim n = Vec_Normalise(p)
                   Return (plane_n.X * p.X + plane_n.Y * p.Y + plane_n.Z * p.Z - Vec_DotProduct(plane_n, plane_p))
                 End Function

      ' Create two temporary storage arrays to classify points either side of plane
      ' If distance sign is positive, point lies on "inside" of plane
      Dim inside_points(2) As Vec3d
      Dim nInsidePointCount = 0
      Dim outside_points(2) As Vec3d
      Dim nOutsidePointCount = 0
      Dim inside_tex(2) As Vec2d
      Dim nInsideTexCount = 0
      Dim outside_tex(2) As Vec2d
      Dim nOutsideTexCount = 0

      ' Get signed distance of each point in triangle to plane
      Dim d0 = dist(in_tri.P(0))
      Dim d1 = dist(in_tri.P(1))
      Dim d2 = dist(in_tri.P(2))

      If d0 >= 0 Then
        inside_points(nInsidePointCount) = in_tri.P(0)
        inside_tex(nInsideTexCount) = in_tri.T(0)
        nInsidePointCount += 1
        nInsideTexCount += 1
      Else
        outside_points(nOutsidePointCount) = in_tri.P(0)
        outside_tex(nOutsideTexCount) = in_tri.T(0)
        nOutsidePointCount += 1
        nOutsideTexCount += 1
      End If

      If d1 >= 0 Then
        inside_points(nInsidePointCount) = in_tri.P(1)
        inside_tex(nInsideTexCount) = in_tri.T(1)
        nInsidePointCount += 1
        nInsideTexCount += 1
      Else
        outside_points(nOutsidePointCount) = in_tri.P(1)
        outside_tex(nOutsideTexCount) = in_tri.T(1)
        nOutsidePointCount += 1
        nOutsideTexCount += 1
      End If

      If d2 >= 0 Then
        inside_points(nInsidePointCount) = in_tri.P(2)
        inside_tex(nInsideTexCount) = in_tri.T(2)
        nInsidePointCount += 1
        nInsideTexCount += 1
      Else
        outside_points(nOutsidePointCount) = in_tri.P(2)
        outside_tex(nOutsideTexCount) = in_tri.T(2)
        nOutsidePointCount += 1
        nOutsideTexCount += 1
      End If

      ' Now classify triangle points, and break the input triangle into 
      ' smaller output triangles if required. There are four possible
      ' outcomes...

      If nInsidePointCount = 0 Then
        ' All points lie on the outside of plane, so clip whole triangle
        ' It ceases to exist
        Return 0 ' No returned triangles are valid
      End If

      If nInsidePointCount = 3 Then

        ' All points lie on the inside of plane, so do nothing
        ' and allow the triangle to simply pass through

        'out_tri1 = in_tri
        out_tri1.P = in_tri.P
        out_tri1.T = in_tri.T
        out_tri1.Col = in_tri.Col

        Return 1 ' Just the one returned original triangle is valid

      End If

      If nInsidePointCount = 1 AndAlso nOutsidePointCount = 2 Then

        ' Triangle should be clipped. As two points lie outside
        ' the plane, the triangle simply becomes a smaller triangle
        ' Copy appearance info to new triangle
        out_tri1.Col = Presets.Magenta ' in_tri.col

        ' The inside point is valid, so keep that...
        out_tri1.P(0) = inside_points(0)
        out_tri1.T(0) = inside_tex(0)

        ' but the two new points are at the locations where the 
        ' original sides of the triangle (lines) intersect with the plane
        Dim t As Single
        out_tri1.P(1) = Vec_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(0), t)
        out_tri1.T(1).X = t * (outside_tex(0).X - inside_tex(0).X) + inside_tex(0).X
        out_tri1.T(1).Y = t * (outside_tex(0).Y - inside_tex(0).Y) + inside_tex(0).Y
        out_tri1.T(1).Z = t * (outside_tex(0).Z - inside_tex(0).Z) + inside_tex(0).Z

        out_tri1.P(2) = Vec_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(1), t)
        out_tri1.T(2).X = t * (outside_tex(1).X - inside_tex(0).X) + inside_tex(0).X
        out_tri1.T(2).Y = t * (outside_tex(1).Y - inside_tex(0).Y) + inside_tex(0).Y
        out_tri1.T(2).Z = t * (outside_tex(1).Z - inside_tex(0).Z) + inside_tex(0).Z

        Return 1 ' Return the newly formed single triangle

      End If

      If nInsidePointCount = 2 AndAlso nOutsidePointCount = 1 Then

        ' Triangle should be clipped. As two points lie inside the plane,
        ' the clipped triangle becomes a "quad". Fortunately, we can
        ' represent a quad with two new triangles

        ' Copy appearance info to new triangles
        out_tri1.Col = Presets.Green ' in_tri.col
        out_tri2.Col = Presets.Red ' in_tri.col

        ' The first triangle consists of the two inside points and a new
        ' point determined by the location where one side of the triangle
        ' intersects with the plane
        out_tri1.P(0) = inside_points(0)
        out_tri1.T(0) = inside_tex(0)

        out_tri1.P(1) = inside_points(1)
        out_tri1.T(1) = inside_tex(1)

        Dim t As Single
        out_tri1.P(2) = Vec_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(0), t)
        out_tri1.T(2).X = t * (outside_tex(0).X - inside_tex(0).X) + inside_tex(0).X
        out_tri1.T(2).Y = t * (outside_tex(0).Y - inside_tex(0).Y) + inside_tex(0).Y
        out_tri1.T(2).Z = t * (outside_tex(0).Z - inside_tex(0).Z) + inside_tex(0).Z

        ' The second triangle is composed of one of he inside points, a
        ' new point determined by the intersection of the other side of the 
        ' triangle and the plane, and the newly created point above
        out_tri2.P(1) = inside_points(1)
        out_tri2.T(1) = inside_tex(1)
        out_tri2.P(0) = out_tri1.P(2)
        out_tri2.T(0) = out_tri1.T(2)
        out_tri2.P(2) = Vec_IntersectPlane(plane_p, plane_n, inside_points(1), outside_points(0), t)
        out_tri2.T(2).X = t * (outside_tex(0).X - inside_tex(1).X) + inside_tex(1).X
        out_tri2.T(2).Y = t * (outside_tex(0).Y - inside_tex(1).Y) + inside_tex(1).Y
        out_tri2.T(2).Z = t * (outside_tex(0).Z - inside_tex(1).Z) + inside_tex(1).Z

        Return 2 ' Return two newly formed triangles which form a quad

      End If

      Return 0

    End Function

  End Class

  Public Class PipeLine

    Private m_matProj As Mat4x4
    Private m_matView As Mat4x4
    Private m_matWorld As Mat4x4
    Private m_texture As Sprite
    Private m_viewX As Single
    Private m_viewY As Single
    Private m_viewW As Single
    Private m_viewH As Single

    Public Sub New()

    End Sub

    Public Sub SetProjection(fovDegrees As Single, aspectRatio As Single, near As Single, far As Single, left As Single, top As Single, width As Single, height As Single)
      m_matProj = Math.Mat_MakeProjection(fovDegrees, aspectRatio, near, far)
      m_viewX = left
      m_viewY = top
      m_viewW = width
      m_viewH = height
    End Sub

    Public Sub SetCamera(pos As Vec3d, lookat As Vec3d, up As Vec3d)
      m_matView = Math.Mat_PointAt(pos, lookat, up)
      m_matView = Math.Mat_QuickInverse(m_matView)
    End Sub

    Public Sub SetTransform(transform As Mat4x4)
      m_matWorld = transform
    End Sub

    Public Sub SetTexture(texture As Sprite)
      m_texture = texture
    End Sub

    Public Sub SetLightSource(pos As Vec3d, dir As Vec3d, col As Pixel)
    End Sub

    Public Function Render(triangles As List(Of Triangle), Optional flags As RenderFlags = RenderFlags.RenderCullCw Or RenderFlags.RenderTextured Or RenderFlags.RenderDepth) As Integer

      ' Calculate Transformation Matrix
      Dim matWorldView = Math.Mat_MultiplyMatrix(m_matWorld, m_matView)

      ' Store triangles for rastering later
      Dim vecTrianglesToRaster As New List(Of Gfx3D.Triangle)

      Dim nTriangleDrawnCount = 0

      ' Process Triangles
      For Each tri In triangles

        Dim triTransformed As New Gfx3D.Triangle

        ' Just copy through texture coordinates
        triTransformed.T(0) = New Gfx3D.Vec2d(tri.T(0).X, tri.T(0).Y, tri.T(0).Z)
        triTransformed.T(1) = New Gfx3D.Vec2d(tri.T(1).X, tri.T(1).Y, tri.T(1).Z)
        triTransformed.T(2) = New Gfx3D.Vec2d(tri.T(2).X, tri.T(2).Y, tri.T(2).Z)

        ' Transform Triangle from object into projected space
        triTransformed.P(0) = Gfx3D.Math.Mat_MultiplyVector(matWorldView, tri.P(0))
        triTransformed.P(1) = Gfx3D.Math.Mat_MultiplyVector(matWorldView, tri.P(1))
        triTransformed.P(2) = Gfx3D.Math.Mat_MultiplyVector(matWorldView, tri.P(2))

        ' Calculate Triangle Normal in WorldView Space
        Dim normal As Gfx3D.Vec3d, line1 As Gfx3D.Vec3d, line2 As Gfx3D.Vec3d
        line1 = Gfx3D.Math.Vec_Sub(triTransformed.P(1), triTransformed.P(0))
        line2 = Gfx3D.Math.Vec_Sub(triTransformed.P(2), triTransformed.P(0))
        normal = Gfx3D.Math.Vec_CrossProduct(line1, line2)
        normal = Gfx3D.Math.Vec_Normalise(normal)

        ' Cull triangles that face away from viewer
        If (flags And RenderFlags.RenderCullCw) <> 0 AndAlso Gfx3D.Math.Vec_DotProduct(normal, triTransformed.P(0)) > 0.0F Then Continue For
        If (flags And RenderFlags.RenderCullCcw) <> 0 AndAlso Gfx3D.Math.Vec_DotProduct(normal, triTransformed.P(0)) < 0.0F Then Continue For

        ' If Lighting, calculate shading
        triTransformed.Col = Presets.White

        ' Clip triangle against near plane
        Dim nClippedTriangles = 0
        Dim clipped(1) As Gfx3D.Triangle : clipped(0) = New Triangle : clipped(1) = New Triangle
        nClippedTriangles = Gfx3D.Math.Triangle_ClipAgainstPlane(New Gfx3D.Vec3d(0.0F, 0.0F, 0.1F), New Gfx3D.Vec3d(0.0F, 0.0F, 1.0F), triTransformed, clipped(0), clipped(1))

        ' This may yield two new triangles
        For n = 0 To nClippedTriangles - 1

          Dim triProjected As Triangle = clipped(n)

          ' Project new triangle
          triProjected.P(0) = Math.Mat_MultiplyVector(m_matProj, clipped(n).P(0))
          triProjected.P(1) = Math.Mat_MultiplyVector(m_matProj, clipped(n).P(1))
          triProjected.P(2) = Math.Mat_MultiplyVector(m_matProj, clipped(n).P(2))

          ' Apply Projection to Verts
          triProjected.P(0).X = triProjected.P(0).X / triProjected.P(0).W
          triProjected.P(1).X = triProjected.P(1).X / triProjected.P(1).W
          triProjected.P(2).X = triProjected.P(2).X / triProjected.P(2).W

          triProjected.P(0).Y = triProjected.P(0).Y / triProjected.P(0).W
          triProjected.P(1).Y = triProjected.P(1).Y / triProjected.P(1).W
          triProjected.P(2).Y = triProjected.P(2).Y / triProjected.P(2).W

          triProjected.P(0).Z = triProjected.P(0).Z / triProjected.P(0).W
          triProjected.P(1).Z = triProjected.P(1).Z / triProjected.P(1).W
          triProjected.P(2).Z = triProjected.P(2).Z / triProjected.P(2).W

          ' Apply Projection to Tex coords
          triProjected.T(0).X = triProjected.T(0).X / triProjected.P(0).W
          triProjected.T(1).X = triProjected.T(1).X / triProjected.P(1).W
          triProjected.T(2).X = triProjected.T(2).X / triProjected.P(2).W

          triProjected.T(0).Y = triProjected.T(0).Y / triProjected.P(0).W
          triProjected.T(1).Y = triProjected.T(1).Y / triProjected.P(1).W
          triProjected.T(2).Y = triProjected.T(2).Y / triProjected.P(2).W

          triProjected.T(0).Z = 1.0F / triProjected.P(0).W
          triProjected.T(1).Z = 1.0F / triProjected.P(1).W
          triProjected.T(2).Z = 1.0F / triProjected.P(2).W

          ' Clip against viewport in screen space
          ' Clip triangles against all four screen edges, this could yield
          ' a bunch of triangles, so create a queue that we traverse to 
          ' ensure we only test new triangles generated against planes
          Dim sclipped(1) As Triangle : sclipped(0) = New Triangle : sclipped(1) = New Triangle
          Dim listTriangles As New List(Of Triangle)()

          ' Add initial triangle
          listTriangles.Add(triProjected)
          Dim nNewTriangles = 1

          For p = 0 To 3

            Dim nTrisToAdd = 0

            While nNewTriangles > 0

              ' Take triangle from front of queue
              Dim test = listTriangles(0)
              listTriangles.RemoveAt(0)
              nNewTriangles -= 1

              ' Clip it against a plane. We only need to test each 
              ' subsequent plane, against subsequent new triangles
              ' as all triangles after a plane clip are guaranteed
              ' to lie on the inside of the plane. I like how this
              ' comment is almost completely and utterly justified
              Select Case p
                Case 0 : nTrisToAdd = Gfx3D.Math.Triangle_ClipAgainstPlane(New Vec3d(0.0F, -1.0F, 0.0F), New Vec3d(0.0F, 1.0F, 0.0F), test, sclipped(0), sclipped(1))
                Case 1 : nTrisToAdd = Gfx3D.Math.Triangle_ClipAgainstPlane(New Vec3d(0.0F, +1.0F, 0.0F), New Vec3d(0.0F, -1.0F, 0.0F), test, sclipped(0), sclipped(1))
                Case 2 : nTrisToAdd = Gfx3D.Math.Triangle_ClipAgainstPlane(New Vec3d(-1.0F, 0.0F, 0.0F), New Vec3d(1.0F, 0.0F, 0.0F), test, sclipped(0), sclipped(1))
                Case 3 : nTrisToAdd = Gfx3D.Math.Triangle_ClipAgainstPlane(New Vec3d(+1.0F, 0.0F, 0.0F), New Vec3d(-1.0F, 0.0F, 0.0F), test, sclipped(0), sclipped(1))
              End Select

              ' Clipping may yield a variable number of triangles, so
              ' add these new ones to the back of the queue for subsequent
              ' clipping against next planes
              For w = 0 To nTrisToAdd - 1
                listTriangles.Add(sclipped(w))
              Next

            End While

            nNewTriangles = listTriangles.Count
          Next

          For Each triRaster In listTriangles
            ' Scale to viewport
            'triRaster.p(0).x *= -1.0f
            'triRaster.p(1).x *= -1.0f
            'triRaster.p(2).x *= -1.0f
            'triRaster.p(0).y *= -1.0f
            'triRaster.p(1).y *= -1.0f
            'triRaster.p(2).y *= -1.0f
            Dim vOffsetView = New Vec3d(1, 1, 0)
            triRaster.P(0) = Math.Vec_Add(triRaster.P(0), vOffsetView)
            triRaster.P(1) = Math.Vec_Add(triRaster.P(1), vOffsetView)
            triRaster.P(2) = Math.Vec_Add(triRaster.P(2), vOffsetView)
            triRaster.P(0).X *= 0.5F * m_viewW
            triRaster.P(0).Y *= 0.5F * m_viewH
            triRaster.P(1).X *= 0.5F * m_viewW
            triRaster.P(1).Y *= 0.5F * m_viewH
            triRaster.P(2).X *= 0.5F * m_viewW
            triRaster.P(2).Y *= 0.5F * m_viewH
            vOffsetView = New Vec3d(m_viewX, m_viewY, 0)
            triRaster.P(0) = Math.Vec_Add(triRaster.P(0), vOffsetView)
            triRaster.P(1) = Math.Vec_Add(triRaster.P(1), vOffsetView)
            triRaster.P(2) = Math.Vec_Add(triRaster.P(2), vOffsetView)

            ' For now, just draw triangle
            If (flags And RenderFlags.RenderTextured) <> 0 Then
              TexturedTriangle(CInt(Fix(triRaster.P(0).X)), CInt(Fix(triRaster.P(0).Y)), triRaster.T(0).X, triRaster.T(0).Y, triRaster.T(0).Z,
                               CInt(Fix(triRaster.P(1).X)), CInt(Fix(triRaster.P(1).Y)), triRaster.T(1).X, triRaster.T(1).Y, triRaster.T(1).Z,
                               CInt(Fix(triRaster.P(2).X)), CInt(Fix(triRaster.P(2).Y)), triRaster.T(2).X, triRaster.T(2).Y, triRaster.T(2).Z,
                               m_texture)
            End If

            If (flags And RenderFlags.RenderWire) <> 0 Then
              DrawTriangleWire(triRaster, Presets.Red)
            End If

            If (flags And RenderFlags.RenderFlat) <> 0 Then
              DrawTriangleFlat(triRaster)
            End If

            nTriangleDrawnCount += 1

          Next

        Next

      Next

      Return nTriangleDrawnCount

    End Function

  End Class

End Class