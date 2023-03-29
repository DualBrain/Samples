' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #1 - Triangles & Projections" -- @javidx9
' https://youtu.be/ih20l3pJoeU
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #2 - Normals, Kulling, Lighting & Object Files" -- @javidx9
' https://youtu.be/XgMWc6LumG4
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #3 - Cameras & Clipping" -- @javidx9
' https://youtu.be/HXSuNxpCzdM

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine
Imports System.IO

Module Program

  Sub Main() 'args As String())
    Dim game As New Engine3d
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Engine3d
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly meshCube As New Mesh
  Private matProj As New Mat4x4

  Private ReadOnly vCamera As New Vec3d

  Private fTheta As Single

  Public Sub New()
    m_sAppName = "3D Demo"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    ' Load object file
    meshCube.LoadFromObjectFile("teapot.obj")

    ' Projection Matrix
    matProj = Matrix_MakeProjection(90.0F, CSng(ScreenHeight() / ScreenWidth()), 0.1F, 1000.0F)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_BLACK)

    fTheta += 1.0F * elapsedTime ' Uncomment to spin me right round baby right round

    ' Set up rotation matrices

    Dim matRotZ = Matrix_MakeRotationZ(fTheta * 0.5F)
    Dim matRotX = Matrix_MakeRotationX(fTheta)

    Dim matTrans = Matrix_MakeTranslation(0.0F, 0.0F, 8.0F) ' Modify Z for "zoom"...

    Dim matWorld = Matrix_MakeIdentity()
    matWorld = Matrix_MultiplyMatrix(matRotZ, matRotX)
    matWorld = Matrix_MultiplyMatrix(matWorld, matTrans)

    ' Store triangles for rasterizing later
    Dim vecTrianglesToRaster As New List(Of Triangle)

    ' Draw Triangles
    For Each tri In meshCube.Tris

      Dim triProjected, triTransformed As New Triangle

      triTransformed.P(0) = Matrix_MultiplyVector(matWorld, tri.P(0))
      triTransformed.P(1) = Matrix_MultiplyVector(matWorld, tri.P(1))
      triTransformed.P(2) = Matrix_MultiplyVector(matWorld, tri.P(2))

      ' Calculate triangle Normal

      ' Get lines either side of triangle
      Dim line1 = Vector_Sub(triTransformed.P(1), triTransformed.P(0))
      Dim line2 = Vector_Sub(triTransformed.P(2), triTransformed.P(0))

      ' Take cross product of lines to get normal to triangle surface
      Dim normal = Vector_CrossProduct(line1, line2)

      ' It's normally normal to normalise the normal
      Dim l = CSng(Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z))
      normal.X /= l : normal.Y /= l : normal.Z /= l

      ' Get Ray from triangle to camera
      Dim vCameraRay = Vector_Sub(triTransformed.P(0), vCamera)

      ' If ray is aligned with normal, then triangle is visible
      If Vector_DotProduct(normal, vCameraRay) < 0.0F Then

        'Illumination
        Dim light_direction As New Vec3d(0.0F, 0.0F, -1.0F)
        light_direction = Vector_Normalise(light_direction)

        'How "aligned" are light direction and triangle surface normal?
        Dim dp = Single.Max(0.1F, Vector_DotProduct(light_direction, normal))
        'Choose console colours as required (much easier with RGB)
        Dim c = GetColour(dp)
        tritransformed.col = c.Attributes
        triTransformed.sym = c.Ch '.UnicodeChar

        'Project triangles from 3D --> 2D
        triProjected.P(0) = Matrix_MultiplyVector(matProj, triTransformed.P(0))
        triProjected.P(1) = Matrix_MultiplyVector(matProj, triTransformed.P(1))
        triProjected.P(2) = Matrix_MultiplyVector(matProj, triTransformed.P(2))
        triProjected.col = triTransformed.col
        triProjected.sym = triTransformed.sym

        ' Scale into view, we moved the normalizing into cartesian space
        ' out of the matrix.vector function from the previous videos, so
        ' do this manually
        triProjected.P(0) = Vector_Div(triProjected.P(0), triProjected.P(0).W)
        triProjected.P(1) = Vector_Div(triProjected.P(1), triProjected.P(1).W)
        triProjected.P(2) = Vector_Div(triProjected.P(2), triProjected.P(2).W)

        ' Offset verts into visible normalized space
        Dim vOffsetView = New Vec3d(1, 1, 0)
        triProjected.P(0) = Vector_Add(triProjected.P(0), vOffsetView)
        triProjected.P(1) = Vector_Add(triProjected.P(1), vOffsetView)
        triProjected.P(2) = Vector_Add(triProjected.P(2), vOffsetView)
        triProjected.P(0).X *= 0.5F * ScreenWidth()
        triProjected.P(0).Y *= 0.5F * ScreenHeight()
        triProjected.P(1).X *= 0.5F * ScreenWidth()
        triProjected.P(1).Y *= 0.5F * ScreenHeight()
        triProjected.P(2).X *= 0.5F * ScreenWidth()
        triProjected.P(2).Y *= 0.5F * ScreenHeight()

        'Store triangle for sorting
        vecTrianglesToRaster.Add(triProjected)

      End If

    Next

    ' Sort triangles from back to front
    vecTrianglesToRaster.Sort(New TriangleComparer)

    For Each triProjected In vecTrianglesToRaster

      ' Rasterize triangle
      FillTriangle(triProjected.P(0).X, triProjected.P(0).Y,
                   triProjected.P(1).X, triProjected.P(1).Y,
                   triProjected.P(2).X, triProjected.P(2).Y,
                   triProjected.sym, triProjected.col)

      'DrawTriangle(triProjected.P(0).X, triProjected.P(0).Y,
      '             triProjected.P(1).X, triProjected.P(1).Y,
      '             triProjected.P(2).X, triProjected.P(2).Y,
      '             PIXEL_SOLID, FG_BLACK)

    Next

    Return True

  End Function

#Region "Matrix"

  Private Shared Function Matrix_MultiplyVector(m As Mat4x4, i As Vec3d) As Vec3d
    Dim v As New Vec3d
    v.X = i.X * m.M(0, 0) + i.Y * m.M(1, 0) + i.Z * m.M(2, 0) + i.W * m.M(3, 0)
    v.Y = i.X * m.M(0, 1) + i.Y * m.M(1, 1) + i.Z * m.M(2, 1) + i.W * m.M(3, 1)
    v.Z = i.X * m.M(0, 2) + i.Y * m.M(1, 2) + i.Z * m.M(2, 2) + i.W * m.M(3, 2)
    v.W = i.X * m.M(0, 3) + i.Y * m.M(1, 3) + i.Z * m.M(2, 3) + i.W * m.M(3, 3)
    Return v
  End Function

  Private Shared Function Matrix_MakeIdentity() As Mat4x4
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = 1.0F
    matrix.M(1, 1) = 1.0F
    matrix.M(2, 2) = 1.0F
    matrix.M(3, 3) = 1.0F
    Return matrix
  End Function

  Private Shared Function Matrix_MakeRotationX(angleRad As Single) As Mat4x4
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = 1.0F
    matrix.M(1, 1) = CSng(Math.Cos(angleRad))
    matrix.M(1, 2) = CSng(Math.Sin(angleRad))
    matrix.M(2, 1) = -CSng(Math.Sin(angleRad))
    matrix.M(2, 2) = CSng(Math.Cos(angleRad))
    matrix.M(3, 3) = 1.0F
    Return matrix
  End Function

  Private Shared Function Matrix_MakeRotationY(angleRad As Single) As Mat4x4
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = CSng(Math.Cos(angleRad))
    matrix.M(0, 2) = CSng(Math.Sin(angleRad))
    matrix.M(2, 0) = -CSng(Math.Sin(angleRad))
    matrix.M(1, 1) = 1.0F
    matrix.M(2, 2) = CSng(Math.Cos(angleRad))
    matrix.M(3, 3) = 1.0F
    Return matrix
  End Function

  Private Shared Function Matrix_MakeRotationZ(angleRad As Single) As Mat4x4
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = CSng(Math.Cos(angleRad))
    matrix.M(0, 1) = CSng(Math.Sin(angleRad))
    matrix.M(1, 0) = -CSng(Math.Sin(angleRad))
    matrix.M(1, 1) = CSng(Math.Cos(angleRad))
    matrix.M(2, 2) = 1.0F
    matrix.M(3, 3) = 1.0F
    Return matrix
  End Function

  Private Shared Function Matrix_MakeTranslation(x As Single, y As Single, z As Single) As Mat4x4
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

  Private Shared Function Matrix_MakeProjection(fovDegrees As Single, aspectRatio As Single, near As Single, far As Single) As Mat4x4
    Dim fFovRad = 1.0F / CSng(Math.Tan(fovDegrees * 0.5F / 180.0F * 3.14159F))
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = aspectRatio * fFovRad
    matrix.M(1, 1) = fFovRad
    matrix.M(2, 2) = far / (far - near)
    matrix.M(3, 2) = (-far * near) / (far - near)
    matrix.M(2, 3) = 1.0F
    matrix.M(3, 3) = 0.0F
    Return matrix
  End Function

  Private Shared Function Matrix_MultiplyMatrix(m1 As Mat4x4, m2 As Mat4x4) As Mat4x4
    Dim matrix As New Mat4x4
    For c = 0 To 3
      For r = 0 To 3
        matrix.M(r, c) = m1.M(r, 0) * m2.M(0, c) + m1.M(r, 1) * m2.M(1, c) + m1.M(r, 2) * m2.M(2, c) + m1.M(r, 3) * m2.M(3, c)
      Next
    Next
    Return matrix
  End Function

  Private Shared Function Matrix_PointAt(pos As Vec3d, target As Vec3d, up As Vec3d) As Mat4x4

    ' Calculate new forward direction
    Dim newForward = Vector_Sub(target, pos)
    newForward = Vector_Normalise(newForward)

    ' Calculate new Up direction
    Dim a = Vector_Mul(newForward, Vector_DotProduct(up, newForward))
    Dim newUp = Vector_Sub(up, a)
    newUp = Vector_Normalise(newUp)

    ' New Right direction is easy, its just cross product
    Dim newRight = Vector_CrossProduct(newUp, newForward)

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

  Private Shared Function Matrix_QuickInverse(m As Mat4x4) As Mat4x4 'Only for Rotation/Translation Matrices
    Dim matrix As New Mat4x4
    matrix.M(0, 0) = m.M(0, 0) : matrix.M(0, 1) = m.M(1, 0) : matrix.M(0, 2) = m.M(2, 0) : matrix.M(0, 3) = 0.0F
    matrix.M(1, 0) = m.M(0, 1) : matrix.M(1, 1) = m.M(1, 1) : matrix.M(1, 2) = m.M(2, 1) : matrix.M(1, 3) = 0.0F
    matrix.M(2, 0) = m.M(0, 2) : matrix.M(2, 1) = m.M(1, 2) : matrix.M(2, 2) = m.M(2, 2) : matrix.M(2, 3) = 0.0F
    matrix.M(3, 0) = -(m.M(3, 0) * matrix.M(0, 0) + m.M(3, 1) * matrix.M(1, 0) + m.M(3, 2) * matrix.M(2, 0))
    matrix.M(3, 1) = -(m.M(3, 0) * matrix.M(0, 1) + m.M(3, 1) * matrix.M(1, 1) + m.M(3, 2) * matrix.M(2, 1))
    matrix.M(3, 2) = -(m.M(3, 0) * matrix.M(0, 2) + m.M(3, 1) * matrix.M(1, 2) + m.M(3, 2) * matrix.M(2, 2))
    matrix.M(3, 3) = 1.0F
    Return matrix
  End Function

#End Region

#Region "Vector"

  Private Shared Function Vector_Add(v1 As Vec3d, v2 As Vec3d) As Vec3d
    Return New Vec3d With {.X = v1.X + v2.X,
                           .Y = v1.Y + v2.Y,
                           .Z = v1.Z + v2.Z}
  End Function

  Private Shared Function Vector_Sub(v1 As Vec3d, v2 As Vec3d) As Vec3d
    Return New Vec3d With {.X = v1.X - v2.X,
                           .Y = v1.Y - v2.Y,
                           .Z = v1.Z - v2.Z}
  End Function

  Private Shared Function Vector_Mul(v1 As Vec3d, k As Single) As Vec3d
    Return New Vec3d With {.X = v1.X * k,
                           .Y = v1.Y * k,
                           .Z = v1.Z * k}
  End Function

  Private Shared Function Vector_Div(v1 As Vec3d, k As Single) As Vec3d
    Return New Vec3d With {.X = v1.X / k,
                           .Y = v1.Y / k,
                           .Z = v1.Z / k}
  End Function

  Private Shared Function Vector_DotProduct(v1 As Vec3d, v2 As Vec3d) As Single
    Return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z
  End Function

  Private Shared Function Vector_Length(v As Vec3d) As Single
    Return CSng(Math.Sqrt(Vector_DotProduct(v, v)))
  End Function

  Private Shared Function Vector_Normalise(v As Vec3d) As Vec3d
    Dim l = Vector_Length(v)
    Return New Vec3d(v.X / l, v.Y / l, v.Z / l)
  End Function

  Private Shared Function Vector_CrossProduct(v1 As Vec3d, v2 As Vec3d) As Vec3d
    Dim v As New Vec3d
    v.X = v1.Y * v2.Z - v1.Z * v2.Y
    v.Y = v1.Z * v2.X - v1.X * v2.Z
    v.Z = v1.X * v2.Y - v1.Y * v2.X
    Return v
  End Function

  Private Shared Function Vector_IntersectPlane(plane_p As Vec3d, plane_n As Vec3d, lineStart As Vec3d, lineEnd As Vec3d) As Vec3d
    plane_n = Vector_Normalise(plane_n)
    Dim plane_d = -Vector_DotProduct(plane_n, plane_p)
    Dim ad = Vector_DotProduct(lineStart, plane_n)
    Dim bd = Vector_DotProduct(lineEnd, plane_n)
    Dim t = (-plane_d - ad) / (bd - ad)
    Dim lineStartToEnd = Vector_Sub(lineEnd, lineStart)
    Dim lineToIntersect = Vector_Mul(lineStartToEnd, t)
    Return Vector_Add(lineStart, lineToIntersect)
  End Function

  Private Shared Function Triangle_ClipAgainstPlane(plane_p As Vec3d, plane_n As Vec3d, in_tri As Triangle, ByRef out_tri1 As Triangle, ByRef out_tri2 As Triangle) As Integer

    ' Make sure plane normal is indeed normal
    plane_n = Vector_Normalise(plane_n)

    ' Return signed shortest distance from point to plane, plane normal must be normalised
    Dim dist = Function(p As Vec3d) As Single
                 Dim n = Vector_Normalise(p)
                 Return (plane_n.X * p.X + plane_n.Y * p.Y + plane_n.Z * p.Z - Vector_DotProduct(plane_n, plane_p))
               End Function

    ' Create two temporary storage arrays to classify points either side of plane
    ' If distance sign is positive, point lies on "inside" of plane
    Dim inside_points(2) As Vec3d, outside_points(2) As Vec3d
    Dim nInsidePointCount, nOutsidePointCount As Integer

    ' Get signed distance of each point in triangle to plane
    Dim d0 = dist(in_tri.P(0))
    Dim d1 = dist(in_tri.P(1))
    Dim d2 = dist(in_tri.P(2))

    If d0 >= 0 Then
      inside_points(nInsidePointCount) = in_tri.P(0)
      nInsidePointCount += 1
    Else
      outside_points(nOutsidePointCount) = in_tri.P(0)
      nOutsidePointCount += 1
    End If

    If d1 >= 0 Then
      inside_points(nInsidePointCount) = in_tri.P(1)
      nInsidePointCount += 1
    Else
      outside_points(nOutsidePointCount) = in_tri.P(1)
      nOutsidePointCount += 1
    End If

    If d2 >= 0 Then
      inside_points(nInsidePointCount) = in_tri.P(2)
      nInsidePointCount += 1
    Else
      outside_points(nOutsidePointCount) = in_tri.P(2)
      nOutsidePointCount += 1
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
      out_tri1 = in_tri
      Return 1 ' Just the one returned original triangle is valid
    End If

    If nInsidePointCount = 1 AndAlso nOutsidePointCount = 2 Then

      ' Triangle should be clipped. As two points lie outside
      ' the plane, the triangle simply becomes a smaller triangle

      ' Copy appearance info to new triangle
      out_tri1.col = in_tri.col
      out_tri1.sym = in_tri.sym

      ' The inside point is valid, so keep that...
      out_tri1.P(0) = inside_points(0)

      ' but the two new points are at the locations where the 
      ' original sides of the triangle (lines) intersect with the plane
      out_tri1.P(1) = Vector_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(0))
      out_tri1.P(2) = Vector_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(1))

      Return 1 ' Return the newly formed single triangle

    End If

    If nInsidePointCount = 2 AndAlso nOutsidePointCount = 1 Then

      ' Triangle should be clipped. As two points lie inside the plane,
      ' the clipped triangle becomes a "quad". Fortunately, we can
      ' represent a quad with two new triangles
      ' Copy appearance info to new triangles
      out_tri1.col = in_tri.col
      out_tri1.sym = in_tri.sym

      out_tri2.col = in_tri.col
      out_tri2.sym = in_tri.sym

      ' The first triangle consists of the two inside points and a new
      ' point determined by the location where one side of the triangle
      ' intersects with the plane
      out_tri1.P(0) = inside_points(0)
      out_tri1.P(1) = inside_points(1)
      out_tri1.P(2) = Vector_IntersectPlane(plane_p, plane_n, inside_points(0), outside_points(0))

      ' The second triangle is composed of one of the inside points, a
      ' new point determined by the intersection of the other side of the 
      ' triangle and the plane, and the newly created point above
      out_tri2.P(0) = inside_points(1)
      out_tri2.P(1) = out_tri1.P(2)
      out_tri2.P(2) = Vector_IntersectPlane(plane_p, plane_n, inside_points(1), outside_points(0))

      Return 2 ' Return two newly formed triangles which form a quad

    End If

    Return 0

  End Function

#End Region

  Private Structure CHAR_INFO
    Public Attributes As Integer
    Public Ch As Integer
  End Structure

  Private Shared Function GetColour(lum As Single) As CHAR_INFO

    Dim bg_col, fg_col As Integer
    Dim sym As Integer
    Dim pixel_bw = CInt(13.0F * lum)

    Select Case pixel_bw
      Case 0 : bg_col = BG_BLACK : fg_col = FG_BLACK : sym = PIXEL_SOLID
      Case 1 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_QUARTER
      Case 2 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_HALF
      Case 3 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_THREEQUARTERS
      Case 4 : bg_col = BG_BLACK : fg_col = FG_DARK_GREY : sym = PIXEL_SOLID
      Case 5 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_QUARTER
      Case 6 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_HALF
      Case 7 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_THREEQUARTERS
      Case 8 : bg_col = BG_DARK_GREY : fg_col = FG_GREY : sym = PIXEL_SOLID
      Case 9 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_QUARTER
      Case 10 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_HALF
      Case 11 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_THREEQUARTERS
      Case 12 : bg_col = BG_GREY : fg_col = FG_WHITE : sym = PIXEL_SOLID
        'Case Else : bg_col = BG_BLACK : fg_col = FG_BLACK : sym = PIXEL_SOLID
      Case Else : bg_col = BG_WHITE : fg_col = FG_WHITE : sym = PIXEL_SOLID
    End Select

    Dim c = New CHAR_INFO With {.Attributes = bg_col Or fg_col, .Ch = sym}

    Return c

  End Function

End Class

Friend Class Vec3d

  Friend Property X As Single
  Friend Property Y As Single
  Public Property Z As Single

  Public Property W As Single = 1.0F

  Friend Sub New()
  End Sub

  Friend Sub New(x As Single, y As Single, z As Single)
    Me.X = x
    Me.Y = y
    Me.Z = z
    W = 1.0F
  End Sub

End Class

Friend Class TriangleComparer
  Implements IComparer(Of Triangle)

  Public Function Compare(t1 As Triangle, t2 As Triangle) As Integer Implements IComparer(Of Triangle).Compare
    Dim z1 = (t1.P(0).Z + t1.P(1).Z + t1.P(2).Z) / 3.0F
    Dim z2 = (t2.P(0).Z + t2.P(1).Z + t2.P(2).Z) / 3.0F
    If z1 > z2 Then
      Return -1
    ElseIf z1 < z2 Then
      Return 1
    Else
      Return 0
    End If
  End Function

End Class

Friend Class Triangle

  Private ReadOnly m_p(2) As Vec3d
  Public sym As Integer
  Public col As Integer

  Public Property P(index As Integer) As Vec3d
    Get
      Return m_p(index)
    End Get
    Set(value As Vec3d)
      m_p(index) = value
    End Set
  End Property

  Public Sub New()
    m_p(0) = New Vec3d
    m_p(1) = New Vec3d
    m_p(2) = New Vec3d
  End Sub

  Friend Sub New(vector0 As Vec3d, vector1 As Vec3d, vector2 As Vec3d)
    m_p(0) = vector0
    m_p(1) = vector1
    m_p(2) = vector2
  End Sub

  Friend Sub New(values As Single())
    Dim index = 0
    For tri = 0 To 2
      m_p(tri) = New Vec3d(values(index), values(index + 1), values(index + 2)) : index += 3
    Next
  End Sub

End Class

Friend Class Mesh

  Public Property Tris As New List(Of Triangle)

  Public Function LoadFromObjectFile(sFilename As String) As Boolean

    Dim f = New StreamReader(sFilename)

    If f Is Nothing Then Return False

    ' Local cache of verts
    Dim verts = New List(Of Vec3d)()

    While Not f.EndOfStream

      Dim line = f.ReadLine()
      If line <> "" Then

        Dim values = line.Split(" "c)

        Dim index = 0
        Dim value = values(index) : index += 1

        If value(0) = "v"c Then
          Dim v = New Vec3d()
          'index += 1 ' skip junk
          v.X = Single.Parse(values(index)) : index += 1
          v.Y = Single.Parse(values(index)) : index += 1
          v.Z = Single.Parse(values(index))
          verts.Add(v)
        ElseIf value(0) = "f"c Then
          Dim fVals() = New Integer(2) {}
          'index += 1 ' skip junk
          fVals(0) = Integer.Parse(values(index)) : index += 1
          fVals(1) = Integer.Parse(values(index)) : index += 1
          fVals(2) = Integer.Parse(values(index))
          Tris.Add(New Triangle(verts(fVals(0) - 1), verts(fVals(1) - 1), verts(fVals(2) - 1)))
        End If

      End If

    End While

    f.Close()

    Return True

  End Function

End Class

Friend Class Mat4x4
  Public M(3, 3) As Single
End Class