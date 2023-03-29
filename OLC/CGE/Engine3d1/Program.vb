' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #1 - Triangles & Projections" -- @javidx9
' https://youtu.be/ih20l3pJoeU

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program

  Sub Main() 'args As String())
    Dim game As New Engine3d
    game.ConstructConsole(256, 240, 4, 4)
    game.Start()
  End Sub

End Module

Class Engine3d
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private ReadOnly m_meshCube As New Mesh
  Private m_matProj As New Mat4x4

  Private m_theta As Single

  Public Sub New()
    m_sAppName = "3D Demo"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    m_meshCube.Tris = { _ ' SOUTH
    New Triangle({0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}),
    New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F}), _
                                                                          _ ' EAST
    New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F}),
    New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F}), _
                                                                          _ ' NORTH
    New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F}),
    New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F}), _
                                                                          _ ' WEST
    New Triangle({0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F}),
    New Triangle({0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F}), _
                                                                          _ ' TOP
    New Triangle({0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F}),
    New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F}), _
                                                                          _ ' BOTTOM
    New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F}),
    New Triangle({1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F})}.ToList

    ' Projection Matrix
    Dim near = 0.1F
    Dim far = 1000.0F
    Dim fov = 90.0
    Dim aspectRatio = CSng(ScreenHeight() / ScreenWidth())
    Dim fovRad = 1.0F / CSng(Math.Tan(fov * 0.5F / 180.0F * 3.14159F))

    m_matProj.M(0, 0) = aspectRatio * fovRad
    m_matProj.M(1, 1) = fovRad
    m_matProj.M(2, 2) = far / (far - near)
    m_matProj.M(3, 2) = (-far * near) / (far - near)
    m_matProj.M(2, 3) = 1.0F
    m_matProj.M(3, 3) = 0.0F

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_BLACK)

    ' Set up rotation matrices
    Dim matRotZ As New Mat4x4
    Dim matRotX As New Mat4x4

    m_theta += 1.0F * elapsedTime

    ' Rotation Z
    matRotZ.M(0, 0) = CSng(Math.Cos(m_theta))
    matRotZ.M(0, 1) = CSng(Math.Sin(m_theta))
    matRotZ.M(1, 0) = -CSng(Math.Sin(m_theta))
    matRotZ.M(1, 1) = CSng(Math.Cos(m_theta))
    matRotZ.M(2, 2) = 1
    matRotZ.M(3, 3) = 1

    ' Rotation X
    matRotX.M(0, 0) = 1
    matRotX.M(1, 1) = CSng(Math.Cos(m_theta * 0.5F))
    matRotX.M(1, 2) = CSng(Math.Sin(m_theta * 0.5F))
    matRotX.M(2, 1) = -CSng(Math.Sin(m_theta * 0.5F))
    matRotX.M(2, 2) = CSng(Math.Cos(m_theta * 0.5F))
    matRotX.M(3, 3) = 1

    ' Draw Triangles
    For Each tri In m_meshCube.Tris

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
      triTranslated.P(0).Z = triRotatedZX.P(0).Z + 3.0F
      triTranslated.P(1).Z = triRotatedZX.P(1).Z + 3.0F
      triTranslated.P(2).Z = triRotatedZX.P(2).Z + 3.0F

      ' Project triangles from 3D --> 2D
      MultiplyMatrixVector(triTranslated.P(0), triProjected.P(0), m_matProj)
      MultiplyMatrixVector(triTranslated.P(1), triProjected.P(1), m_matProj)
      MultiplyMatrixVector(triTranslated.P(2), triProjected.P(2), m_matProj)

      ' Scale into view
      triProjected.P(0).X += 1.0F : triProjected.P(0).Y += 1.0F
      triProjected.P(1).X += 1.0F : triProjected.P(1).Y += 1.0F
      triProjected.P(2).X += 1.0F : triProjected.P(2).Y += 1.0F
      triProjected.P(0).X *= 0.5F * ScreenWidth()
      triProjected.P(0).Y *= 0.5F * ScreenHeight()
      triProjected.P(1).X *= 0.5F * ScreenWidth()
      triProjected.P(1).Y *= 0.5F * ScreenHeight()
      triProjected.P(2).X *= 0.5F * ScreenWidth()
      triProjected.P(2).Y *= 0.5F * ScreenHeight()

      ' Rasterize triangle
      DrawTriangle(triProjected.P(0).X, triProjected.P(0).Y,
                   triProjected.P(1).X, triProjected.P(1).Y,
                   triProjected.P(2).X, triProjected.P(2).Y,
                   PIXEL_SOLID, FG_WHITE)

    Next

    Return True

  End Function

  Private Shared Sub MultiplyMatrixVector(ByRef i As Vec3d, ByRef o As Vec3d, ByRef m As Mat4x4)
    o.x = i.x * m.m(0, 0) + i.y * m.m(1, 0) + i.z * m.m(2, 0) + m.m(3, 0)
    o.y = i.x * m.m(0, 1) + i.y * m.m(1, 1) + i.z * m.m(2, 1) + m.m(3, 1)
    o.z = i.x * m.m(0, 2) + i.y * m.m(1, 2) + i.z * m.m(2, 2) + m.m(3, 2)
    Dim w = i.X * m.M(0, 3) + i.Y * m.M(1, 3) + i.Z * m.M(2, 3) + m.M(3, 3)
    If (w <> 0.0F) Then
      o.x /= w
      o.y /= w
      o.z /= w
    End If
  End Sub

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

Friend Class Triangle

  Private ReadOnly m_p(2) As Vec3d

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

  Friend Sub New(values As Single())
    Dim index = 0
    For tri = 0 To 2
      m_p(tri) = New Vec3d(values(index), values(index + 1), values(index + 2)) : index += 3
    Next
  End Sub

End Class

Friend Class Mesh
  Public Tris As New List(Of Triangle)
End Class

Friend Class Mat4x4
  Public M(3, 3) As Single
End Class