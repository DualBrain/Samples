' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #1 - Triangles & Projections" -- @javidx9
' https://youtu.be/ih20l3pJoeU
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #2 - Normals, Kulling, Lighting & Object Files" -- @javidx9
' https://youtu.be/XgMWc6LumG4

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

  Private vCamera As New Vec3d

  Private fTheta As Single

  Public Sub New()
    m_sAppName = "3D Demo"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    'meshCube.Tris = { _ ' SOUTH
    'New Triangle({0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}),
    'New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F}), _
    '                                                                      _ ' EAST
    'New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F}),
    'New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F}), _
    '                                                                      _ ' NORTH
    'New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F}),
    'New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F}), _
    '                                                                      _ ' WEST
    'New Triangle({0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F}),
    'New Triangle({0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F}), _
    '                                                                      _ ' TOP
    'New Triangle({0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F}),
    'New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F}), _
    '                                                                      _ ' BOTTOM
    'New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F}),
    'New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F})}.ToList

    meshCube.LoadFromObjectFile("VideoShip.obj")
    'meshCube.LoadFromObjectFile("teapot.obj")
    'meshCube.LoadFromObjectFile("axis.obj")

    ' Projection Matrix
    Dim near = 0.1F
    Dim far = 1000.0F
    Dim fov = 90.0
    Dim aspectRatio = CSng(ScreenHeight() / ScreenWidth())
    Dim fovRad = 1.0F / CSng(Math.Tan(fov * 0.5F / 180.0F * 3.14159F))

    matProj.M(0, 0) = aspectRatio * fovRad
    matProj.M(1, 1) = fovRad
    matProj.M(2, 2) = far / (far - near)
    matProj.M(3, 2) = (-far * near) / (far - near)
    matProj.M(2, 3) = 1.0F
    matProj.M(3, 3) = 0.0F

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_BLACK)

    ' Set up rotation matrices
    Dim matRotZ As New Mat4x4
    Dim matRotX As New Mat4x4
    fTheta += 1.0F * elapsedTime

    ' Rotation Z
    matRotZ.M(0, 0) = CSng(Math.Cos(fTheta))
    matRotZ.M(0, 1) = CSng(Math.Sin(fTheta))
    matRotZ.M(1, 0) = -CSng(Math.Sin(fTheta))
    matRotZ.M(1, 1) = CSng(Math.Cos(fTheta))
    matRotZ.M(2, 2) = 1
    matRotZ.M(3, 3) = 1

    ' Rotation X
    matRotX.M(0, 0) = 1
    matRotX.M(1, 1) = CSng(Math.Cos(fTheta * 0.5F))
    matRotX.M(1, 2) = CSng(Math.Sin(fTheta * 0.5F))
    matRotX.M(2, 1) = -CSng(Math.Sin(fTheta * 0.5F))
    matRotX.M(2, 2) = CSng(Math.Cos(fTheta * 0.5F))
    matRotX.M(3, 3) = 1

    ' Store triangles for rasterizing later
    Dim vecTrianglesToRaster As New List(Of Triangle)

    ' Draw Triangles
    For Each tri In meshCube.Tris

      Dim triProjected, triTranslated, triRotatedZ, triRotatedZX As New Triangle

      ' Rotate in Z-Axis
      MultiplyMatrixVector(tri.P(0), triRotatedZ.P(0), matRotZ)
      MultiplyMatrixVector(tri.P(1), triRotatedZ.P(1), matRotZ)
      MultiplyMatrixVector(tri.P(2), triRotatedZ.P(2), matRotZ)

      ' Rotate in X-Axis
      MultiplyMatrixVector(triRotatedZ.P(0), triRotatedZX.P(0), matRotX)
      MultiplyMatrixVector(triRotatedZ.P(1), triRotatedZX.P(1), matRotX)
      MultiplyMatrixVector(triRotatedZ.P(2), triRotatedZX.P(2), matRotX)

      ' Offset into the screen
      triTranslated = triRotatedZX
      triTranslated.P(0).Z = triRotatedZX.P(0).Z + 8.0F
      triTranslated.P(1).Z = triRotatedZX.P(1).Z + 8.0F
      triTranslated.P(2).Z = triRotatedZX.P(2).Z + 8.0F

      ' Use Cross-Product to get surface normal
      Dim normal, line1, line2 As New Vec3d
      line1.X = triTranslated.P(1).X - triTranslated.P(0).X
      line1.Y = triTranslated.P(1).Y - triTranslated.P(0).Y
      line1.Z = triTranslated.P(1).Z - triTranslated.P(0).Z

      line2.X = triTranslated.P(2).X - triTranslated.P(0).X
      line2.Y = triTranslated.P(2).Y - triTranslated.P(0).Y
      line2.Z = triTranslated.P(2).Z - triTranslated.P(0).Z

      normal.X = line1.Y * line2.Z - line1.Z * line2.Y
      normal.Y = line1.Z * line2.X - line1.X * line2.Z
      normal.Z = line1.X * line2.Y - line1.Y * line2.X

      ' It's normally normal to normalise the normal
      Dim l = CSng(Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z))
      normal.X /= l : normal.Y /= l : normal.Z /= l

      'if (normal.z < 0)
      If normal.X * (triTranslated.P(0).X - vCamera.X) + normal.Y * (triTranslated.P(0).Y - vCamera.Y) + normal.Z * (triTranslated.P(0).Z - vCamera.Z) < 0.0F Then

        'Illumination
        Dim light_direction As New Vec3d(0.0F, 0.0F, -1.0F)
        l = CSng(Math.Sqrt(light_direction.X * light_direction.X + light_direction.Y * light_direction.Y + light_direction.Z * light_direction.Z))
        light_direction.X /= l : light_direction.Y /= l : light_direction.Z /= l
        'How similar is normal to light direction
        Dim dp = normal.X * light_direction.X + normal.Y * light_direction.Y + normal.Z * light_direction.Z

        'Choose console colours as required (much easier with RGB)
        Dim c = GetColour(dp)
        triTranslated.col = c.Attributes
        triTranslated.sym = c.Ch '.UnicodeChar

        'Project triangles from 3D --> 2D
        MultiplyMatrixVector(triTranslated.P(0), triProjected.P(0), matProj)
        MultiplyMatrixVector(triTranslated.P(1), triProjected.P(1), matProj)
        MultiplyMatrixVector(triTranslated.P(2), triProjected.P(2), matProj)
        triProjected.col = triTranslated.col
        triProjected.sym = triTranslated.sym

        'Scale into view
        triProjected.P(0).X += 1.0F : triProjected.P(0).Y += 1.0F
        triProjected.P(1).X += 1.0F : triProjected.P(1).Y += 1.0F
        triProjected.P(2).X += 1.0F : triProjected.P(2).Y += 1.0F
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

  Private Shared Sub MultiplyMatrixVector(ByRef i As Vec3d, ByRef o As Vec3d, ByRef m As Mat4x4)
    o.X = i.X * m.M(0, 0) + i.Y * m.M(1, 0) + i.Z * m.M(2, 0) + m.M(3, 0)
    o.Y = i.X * m.M(0, 1) + i.Y * m.M(1, 1) + i.Z * m.M(2, 1) + m.M(3, 1)
    o.Z = i.X * m.M(0, 2) + i.Y * m.M(1, 2) + i.Z * m.M(2, 2) + m.M(3, 2)
    Dim w = i.X * m.M(0, 3) + i.Y * m.M(1, 3) + i.Z * m.M(2, 3) + m.M(3, 3)
    If (w <> 0.0F) Then
      o.X /= w : o.Y /= w : o.Z /= w
    End If
  End Sub

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

  Friend Sub New()
  End Sub

  Friend Sub New(x As Single, y As Single, z As Single)
    Me.X = x
    Me.Y = y
    Me.Z = z
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

  Public ReadOnly Property P(index As Integer) As Vec3d
    Get
      Return m_p(index)
    End Get
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