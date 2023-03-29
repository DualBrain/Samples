' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #1 - Triangles & Projections" -- @javidx9
' https://youtu.be/ih20l3pJoeU
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #2 - Normals, Kulling, Lighting & Object Files" -- @javidx9
' https://youtu.be/XgMWc6LumG4
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #3 - Cameras & Clipping" -- @javidx9
' https://youtu.be/HXSuNxpCzdM
' Inspired by "Code-It-Yourself! 3D Graphics Engine Part #4 - Texturing & Depth Buffers" -- @javidx9
' https://youtu.be/nBzCS-Y0FcY

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
  Private m_matProj As New Mat4x4 ' Matrix that converts from view space to screen space
  Private m_camera As New Vec3d ' Location of camera in the world space
  Private m_lookDir As New Vec3d ' Direction vector along the direction camera points
  Private m_yaw As Single ' FPS Camera rotation in XZ plane
  Private m_theta As Single ' Spins World transform

  Private m_sprite As Sprite

  Private m_depthBuffer() As Single

  Public Sub New()
    m_sAppName = "3D Demo"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    ReDim m_depthBuffer((ScreenWidth() * ScreenHeight()) - 1)

    ' Load object file
    'meshCube.LoadFromObjectFile("mountains.obj")

    m_meshCube.Tris = New List(Of Triangle) From {New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' SOUTH
                                                  New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}),
                                                                                                                                                                                               _
                                                  New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' EAST
                                                  New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                                _
                                                  New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' NORTH
                                                  New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                                _
                                                  New Triangle({0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' WEST
                                                  New Triangle({0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                                _
                                                  New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' TOP
                                                  New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                                _
                                                  New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' BOTTOM
                                                  New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F})}

    m_sprite = New Sprite("fps_wall1.spr")

    ' Projection Matrix
    m_matProj = Matrix_MakeProjection(90.0F, CSng(ScreenHeight() / ScreenWidth()), 0.1F, 1000.0F)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    If (GetKey(VK_UP).Held) Then m_camera.Y += 8.0F * elapsedTime ' Travel Upwards
    If (GetKey(VK_DOWN).Held) Then m_camera.Y -= 8.0F * elapsedTime ' Travel Downwards

    ' Dont use these two in FPS mode, it is confusing :P
    If (GetKey(VK_LEFT).Held) Then m_camera.X -= 8.0F * elapsedTime ' Travel Along X-Axis
    If (GetKey(VK_RIGHT).Held) Then m_camera.X += 8.0F * elapsedTime ' Travel Along X-Axis
    '///////

    Dim forward = Vector_Mul(m_lookDir, 8.0F * elapsedTime)

    ' Standard FPS Control scheme, but turn instead of strafe
    If (GetKey(AscW("W"c)).Held) Then m_camera = Vector_Add(m_camera, forward)
    If (GetKey(AscW("S"c)).Held) Then m_camera = Vector_Sub(m_camera, forward)
    If (GetKey(AscW("A"c)).Held) Then m_yaw -= 2.0F * elapsedTime
    If (GetKey(AscW("D"c)).Held) Then m_yaw += 2.0F * elapsedTime

    ' Set up "World Tranmsform" though not updating theta 
    ' makes this a bit redundant
    m_theta += 1.0F * elapsedTime ' Uncomment to spin me right round baby right round
    Dim matRotZ = Matrix_MakeRotationZ(m_theta * 0.5F)
    Dim matRotX = Matrix_MakeRotationX(m_theta)

    Dim matTrans = Matrix_MakeTranslation(0.0F, 0.0F, 2.5F)

    Dim matWorld = Matrix_MakeIdentity() ' Form World Matrix
    matWorld = Matrix_MultiplyMatrix(matRotZ, matRotX) ' Transform by rotation
    matWorld = Matrix_MultiplyMatrix(matWorld, matTrans) ' Transform by translation

    ' Create "Point At" Matrix for camera
    Dim up = New Vec3d(0, 1, 0)
    Dim target = New Vec3d(0, 0, 1)
    Dim matCameraRot = Matrix_MakeRotationY(m_yaw)
    m_lookDir = Matrix_MultiplyVector(matCameraRot, target)
    target = Vector_Add(m_camera, m_lookDir)
    Dim matCamera = Matrix_PointAt(m_camera, target, up)

    ' Make view matrix from camera
    Dim matView = Matrix_QuickInverse(matCamera)

    ' Store triagles for rastering later
    Dim trianglesToRaster = New List(Of Triangle)()

    ' Draw Triangles
    For Each tri In m_meshCube.Tris

      Dim triProjected, triTransformed, triViewed As New Triangle

      ' World Matrix Transform
      triTransformed.P(0) = Matrix_MultiplyVector(matWorld, tri.P(0))
      triTransformed.P(1) = Matrix_MultiplyVector(matWorld, tri.P(1))
      triTransformed.P(2) = Matrix_MultiplyVector(matWorld, tri.P(2))
      triTransformed.T(0) = tri.T(0)
      triTransformed.T(1) = tri.T(1)
      triTransformed.T(2) = tri.T(2)

      ' Calculate triangle Normal
      Dim normal, line1, line2 As Vec3d

      ' Get lines either side of triangle
      line1 = Vector_Sub(triTransformed.P(1), triTransformed.P(0))
      line2 = Vector_Sub(triTransformed.P(2), triTransformed.P(0))

      ' Take cross product of lines to get normal to triangle surface
      normal = Vector_CrossProduct(line1, line2)

      ' You normally need to normalise a normal!
      normal = Vector_Normalise(normal)

      ' Get Ray from triangle to camera
      Dim cameraRay = Vector_Sub(triTransformed.P(0), m_camera)

      ' If ray is aligned with normal, then triangle is visible
      If Vector_DotProduct(normal, cameraRay) < 0.0F Then

        ' Illumination
        Dim lightDirection As New Vec3d(0.0F, 1.0F, -1.0F)
        lightDirection = Vector_Normalise(lightDirection)

        ' How "aligned" are light direction and triangle surface normal?
        Dim dp = Math.Max(0.1F, Vector_DotProduct(lightDirection, normal))

        ' Choose console colours as required (much easier with RGB)
        Dim c As CHAR_INFO = GetColour(dp)
        triTransformed.col = c.Attributes
        triTransformed.sym = c.Ch

        ' Convert World Space --> View Space
        triViewed.P(0) = Matrix_MultiplyVector(matView, triTransformed.P(0))
        triViewed.P(1) = Matrix_MultiplyVector(matView, triTransformed.P(1))
        triViewed.P(2) = Matrix_MultiplyVector(matView, triTransformed.P(2))
        triViewed.sym = triTransformed.sym
        triViewed.col = triTransformed.col
        triViewed.T(0) = triTransformed.T(0)
        triViewed.T(1) = triTransformed.T(1)
        triViewed.T(2) = triTransformed.T(2)

        ' Clip Viewed Triangle against near plane, this could form two additional
        ' additional triangles. 
        Dim clipped(1) As Triangle : clipped(0) = New Triangle : clipped(1) = New Triangle
        Dim clippedTriangles = Triangle_ClipAgainstPlane(New Vec3d(0.0F, 0.0F, 0.1F), New Vec3d(0.0F, 0.0F, 1.0F), triViewed, clipped(0), clipped(1))

        ' We may end up with multiple triangles form the clip, so project as required
        For n = 0 To clippedTriangles - 1

          ' Project triangles from 3D --> 2D
          triProjected.P(0) = Matrix_MultiplyVector(m_matProj, clipped(n).P(0))
          triProjected.P(1) = Matrix_MultiplyVector(m_matProj, clipped(n).P(1))
          triProjected.P(2) = Matrix_MultiplyVector(m_matProj, clipped(n).P(2))
          triProjected.col = clipped(n).col
          triProjected.sym = clipped(n).sym
          triProjected.T(0) = clipped(n).T(0)
          triProjected.T(1) = clipped(n).T(1)
          triProjected.T(2) = clipped(n).T(2)

          triProjected.T(0).U = triProjected.T(0).U / triProjected.P(0).W
          triProjected.T(1).U = triProjected.T(1).U / triProjected.P(1).W
          triProjected.T(2).U = triProjected.T(2).U / triProjected.P(2).W

          triProjected.T(0).V = triProjected.T(0).V / triProjected.P(0).W
          triProjected.T(1).V = triProjected.T(1).V / triProjected.P(1).W
          triProjected.T(2).V = triProjected.T(2).V / triProjected.P(2).W

          triProjected.T(0).W = 1.0F / triProjected.P(0).W
          triProjected.T(1).W = 1.0F / triProjected.P(1).W
          triProjected.T(2).W = 1.0F / triProjected.P(2).W

          ' Scale into view, we moved the normalising into cartesian space
          ' out of the matrix.vector function from the previous videos, so
          ' do this manually
          triProjected.P(0) = Vector_Div(triProjected.P(0), triProjected.P(0).W)
          triProjected.P(1) = Vector_Div(triProjected.P(1), triProjected.P(1).W)
          triProjected.P(2) = Vector_Div(triProjected.P(2), triProjected.P(2).W)

          ' X/Y are inverted so put them back
          triProjected.P(0).X *= -1.0F
          triProjected.P(1).X *= -1.0F
          triProjected.P(2).X *= -1.0F
          triProjected.P(0).Y *= -1.0F
          triProjected.P(1).Y *= -1.0F
          triProjected.P(2).Y *= -1.0F

          ' Offset verts into visible normalised space
          Dim offsetView = New Vec3d(1, 1, 0)
          triProjected.P(0) = Vector_Add(triProjected.P(0), offsetView)
          triProjected.P(1) = Vector_Add(triProjected.P(1), offsetView)
          triProjected.P(2) = Vector_Add(triProjected.P(2), offsetView)
          triProjected.P(0).X *= 0.5F * CSng(ScreenWidth())
          triProjected.P(0).Y *= 0.5F * CSng(ScreenHeight())
          triProjected.P(1).X *= 0.5F * CSng(ScreenWidth())
          triProjected.P(1).Y *= 0.5F * CSng(ScreenHeight())
          triProjected.P(2).X *= 0.5F * CSng(ScreenWidth())
          triProjected.P(2).Y *= 0.5F * CSng(ScreenHeight())

          ' Store triangle for sorting
          trianglesToRaster.Add(triProjected)

        Next

      End If

    Next

    ' Sort triangles from back to front
    'vecTrianglesToRaster.Sort(New TriangleComparer)

    ' Clear Screen
    Fill(0, 0, ScreenWidth(), ScreenHeight(), PIXEL_SOLID, FG_BLACK)

    ' Clear Depth Buffer
    For i = 0 To (ScreenWidth() * ScreenHeight()) - 1
      m_depthBuffer(i) = 0.0F
    Next

    ' Loop through all transformed, viewed, projected, and sorted triangles
    For Each triToRaster In trianglesToRaster

      ' Clip triangles against all four screen edges, this could yield
      ' a bunch of triangles, so create a queue that we traverse to 
      ' ensure we only test new triangles generated against planes
      Dim clipped(1) As Triangle : clipped(0) = New Triangle : clipped(1) = New Triangle

      ' Add initial triangle
      Dim listTriangles As New List(Of Triangle) From {triToRaster}
      Dim newTriangles = 1

      For p = 0 To 3

        Dim trisToAdd = 0

        While newTriangles > 0

          ' Take triangle from front of queue
          Dim test = listTriangles(0)
          listTriangles.RemoveAt(0)
          newTriangles -= 1

          ' Clip it against a plane. We only need to test each 
          ' subsequent plane, against subsequent new triangles
          ' as all triangles after a plane clip are guaranteed
          ' to lie on the inside of the plane. I like how this
          ' comment is almost completely and utterly justified
          Select Case p
            Case 0 : trisToAdd = Triangle_ClipAgainstPlane(New Vec3d(0.0F, 0.0F, 0.0F), New Vec3d(0.0F, 1.0F, 0.0F), test, clipped(0), clipped(1))
            Case 1 : trisToAdd = Triangle_ClipAgainstPlane(New Vec3d(0.0F, ScreenHeight() - 1, 0.0F), New Vec3d(0.0F, -1.0F, 0.0F), test, clipped(0), clipped(1))
            Case 2 : trisToAdd = Triangle_ClipAgainstPlane(New Vec3d(0.0F, 0.0F, 0.0F), New Vec3d(1.0F, 0.0F, 0.0F), test, clipped(0), clipped(1))
            Case 3 : trisToAdd = Triangle_ClipAgainstPlane(New Vec3d(ScreenWidth() - 1, 0.0F, 0.0F), New Vec3d(-1.0F, 0.0F, 0.0F), test, clipped(0), clipped(1))
          End Select

          ' Clipping may yield a variable number of triangles, so
          ' add these new ones to the back of the queue for subsequent
          ' clipping against next planes
          For w = 0 To trisToAdd - 1
            listTriangles.Add(clipped(w))
          Next

        End While

        newTriangles = listTriangles.Count

      Next

      ' Draw the transformed, viewed, clipped, projected, sorted, clipped triangles
      For Each t In listTriangles

        TexturedTriangle(CInt(Fix(t.P(0).X)), CInt(Fix(t.P(0).Y)), t.T(0).U, t.T(0).V, t.T(0).W,
                         CInt(Fix(t.P(1).X)), CInt(Fix(t.P(1).Y)), t.T(1).U, t.T(1).V, t.T(1).W,
                         CInt(Fix(t.P(2).X)), CInt(Fix(t.P(2).Y)), t.T(2).U, t.T(2).V, t.T(2).W, m_sprite)

        'FillTriangle(t.P(0).X, t.P(0).Y, t.P(1).X, t.P(1).Y, t.P(2).X, t.P(2).Y, t.sym, t.col)
        'DrawTriangle(t.P(0).X, t.P(0).Y, t.P(1).X, t.P(1).Y, t.P(2).X, t.P(2).Y, PIXEL_SOLID, FG_WHITE)

      Next

    Next

    Return True

  End Function

  Private Shared Sub Swap(ByRef x As Integer, ByRef y As Integer)
    Dim t = x
    x = y
    y = t
  End Sub

  Private Shared Sub Swap(ByRef x As Single, ByRef y As Single)
    Dim t = x
    x = y
    y = t
  End Sub

  Private Sub TexturedTriangle(x1 As Integer, y1 As Integer, u1 As Single, v1 As Single, w1 As Single,
                               x2 As Integer, y2 As Integer, u2 As Single, v2 As Single, w2 As Single,
                               x3 As Integer, y3 As Integer, u3 As Single, v3 As Single, w3 As Single,
                               tex As Sprite)
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

    Dim dax_step = 0.0F, dbx_step = 0.0F,
        du1_step = 0.0F, dv1_step = 0.0F,
        du2_step = 0.0F, dv2_step = 0.0F,
        dw1_step = 0.0F, dw2_step = 0.0F

    If dy1 <> 0 Then dax_step = dx1 / CSng(Math.Abs(dy1))
    If dy2 <> 0 Then dbx_step = dx2 / CSng(Math.Abs(dy2))

    If dy1 <> 0 Then du1_step = du1 / Math.Abs(dy1)
    If dy1 <> 0 Then dv1_step = dv1 / Math.Abs(dy1)
    If dy1 <> 0 Then dw1_step = dw1 / Math.Abs(dy1)

    If dy2 <> 0 Then du2_step = du2 / Math.Abs(dy2)
    If dy2 <> 0 Then dv2_step = dv2 / Math.Abs(dy2)
    If dy2 <> 0 Then dw2_step = dw2 / Math.Abs(dy2)

    If dy1 <> 0 Then

      For i = y1 To y2

        Dim ax = CInt(Fix(x1 + ((i - y1) * dax_step)))
        Dim bx = CInt(Fix(x1 + ((i - y1) * dbx_step)))

        Dim tex_su = u1 + ((i - y1) * du1_step)
        Dim tex_sv = v1 + ((i - y1) * dv1_step)
        Dim tex_sw = w1 + ((i - y1) * dw1_step)

        Dim tex_eu = u1 + ((i - y1) * du2_step)
        Dim tex_ev = v1 + ((i - y1) * dv2_step)
        Dim tex_ew = w1 + ((i - y1) * dw2_step)

        If ax > bx Then
          Swap(ax, bx)
          Swap(tex_su, tex_eu)
          Swap(tex_sv, tex_ev)
          Swap(tex_sw, tex_ew)
        End If

        'Dim tex_u = tex_su
        'Dim tex_v = tex_sv
        'Dim tex_w = tex_sw

        Dim tstep = 1.0F / (bx - ax)
        Dim t = 0.0F

        For j = ax To bx - 1

          Dim tex_u = (1.0F - t) * tex_su + (t * tex_eu)
          Dim tex_v = (1.0F - t) * tex_sv + (t * tex_ev)
          Dim tex_w = (1.0F - t) * tex_sw + (t * tex_ew)

          If (tex_w > m_depthBuffer((i * ScreenWidth()) + j)) Then
            Dim glyph = tex.SampleGlyph(tex_u / tex_w, tex_v / tex_w)
            Dim clr = tex.SampleColour(tex_u / tex_w, tex_v / tex_w)
            Draw(j, i, glyph, clr)
            m_depthBuffer((i * ScreenWidth()) + j) = tex_w
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

    If (dy1 <> 0) Then
      dax_step = CSng(dx1 / Math.Abs(dy1))
    End If

    If (dy2 <> 0) Then
      dbx_step = CSng(dx2 / Math.Abs(dy2))
    End If

    du1_step = 0
    dv1_step = 0

    If (dy1 <> 0) Then
      du1_step = du1 / Math.Abs(dy1)
      dv1_step = dv1 / Math.Abs(dy1)
      dw1_step = dw1 / Math.Abs(dy1)
    End If

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

        'Dim tex_u = tex_su
        'Dim tex_v = tex_sv
        'Dim tex_w = tex_sw

        Dim tstep = 1.0F / (bx - ax)
        Dim t = 0.0F

        For j = ax To bx - 1

          Dim tex_u = (1.0F - t) * tex_su + t * tex_eu
          Dim tex_v = (1.0F - t) * tex_sv + t * tex_ev
          Dim tex_w = (1.0F - t) * tex_sw + t * tex_ew

          If tex_w > m_depthBuffer(i * ScreenWidth() + j) Then
            Draw(j, i, tex.SampleGlyph(tex_u / tex_w, tex_v / tex_w), tex.SampleColour(tex_u / tex_w, tex_v / tex_w))
            m_depthBuffer(i * ScreenWidth() + j) = tex_w
          End If

          t += tstep

        Next

      Next

    End If

  End Sub


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

  Private Shared Function Vector_IntersectPlane(plane_p As Vec3d, plane_n As Vec3d, lineStart As Vec3d, lineEnd As Vec3d, ByRef t As Single) As Vec3d
    plane_n = Vector_Normalise(plane_n)
    Dim plane_d = -Vector_DotProduct(plane_n, plane_p)
    Dim ad = Vector_DotProduct(lineStart, plane_n)
    Dim bd = Vector_DotProduct(lineEnd, plane_n)
    t = (-plane_d - ad) / (bd - ad)
    Dim lineStartToEnd = Vector_Sub(lineEnd, lineStart)
    Dim lineToIntersect = Vector_Mul(lineStartToEnd, t)
    Return Vector_Add(lineStart, lineToIntersect)
  End Function

  Private Shared Function Triangle_ClipAgainstPlane(planeP As Vec3d, planeN As Vec3d, inTri As Triangle, ByRef outTri1 As Triangle, ByRef outTri2 As Triangle) As Integer

    ' Make sure plane normal is indeed normal
    planeN = Vector_Normalise(planeN)

    ' Return signed shortest distance from point to plane, plane normal must be normalised
    Dim dist = Function(p As Vec3d) As Single
                 Dim n = Vector_Normalise(p)
                 Return (planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - Vector_DotProduct(planeN, planeP))
               End Function

    ' Create two temporary storage arrays to classify points either side of plane
    ' If distance sign is positive, point lies on "inside" of plane
    Dim insidePoints(2) As Vec3d : Dim insidePointCount = 0
    Dim outsidePoints(2) As Vec3d : Dim outsidePointCount = 0
    Dim insideTex(2) As Vec2d : Dim insideTexCount = 0
    Dim outsideTex(2) As Vec2d : Dim outsideTexCount = 0

    For index = 0 To 2
      insidePoints(index) = New Vec3d
      outsidePoints(index) = New Vec3d
      insideTex(index) = New Vec2d
      outsideTex(index) = New Vec2d
    Next

    ' Get signed distance of each point in triangle to plane
    Dim d0 = dist(inTri.P(0))
    Dim d1 = dist(inTri.P(1))
    Dim d2 = dist(inTri.P(2))

    If d0 >= 0 Then
      insidePoints(insidePointCount).Assign(inTri.P(0)) : insidePointCount += 1
      insideTex(insideTexCount).Assign(inTri.T(0)) : insideTexCount += 1
    Else
      outsidePoints(outsidePointCount).Assign(inTri.P(0)) : outsidePointCount += 1
      outsideTex(outsideTexCount).Assign(inTri.T(0)) : outsideTexCount += 1
    End If

    If d1 >= 0 Then
      insidePoints(insidePointCount).Assign(inTri.P(1)) : insidePointCount += 1
      insideTex(insideTexCount).Assign(inTri.T(1)) : insideTexCount += 1
    Else
      outsidePoints(outsidePointCount).Assign(inTri.P(1)) : outsidePointCount += 1
      outsideTex(outsideTexCount).Assign(inTri.T(1)) : outsideTexCount += 1
    End If

    If d2 >= 0 Then
      insidePoints(insidePointCount).Assign(inTri.P(2)) : insidePointCount += 1
      insideTex(insideTexCount).Assign(inTri.T(2)) : insideTexCount += 1
    Else
      outsidePoints(outsidePointCount).Assign(inTri.P(2)) : outsidePointCount += 1
      outsideTex(outsideTexCount).Assign(inTri.T(2)) : outsideTexCount += 1
    End If

    ' Now classify triangle points, and break the input triangle into 
    ' smaller output triangles if required. There are four possible
    ' outcomes...
    If insidePointCount = 0 Then
      ' All points lie on the outside of plane, so clip whole triangle
      ' It ceases to exist
      Return 0 ' No returned triangles are valid
    End If

    If insidePointCount = 3 Then
      ' All points lie on the inside of plane, so do nothing
      ' and allow the triangle to simply pass through
      outTri1 = inTri
      Return 1 ' Just the one returned original triangle is valid
    End If

    If insidePointCount = 1 AndAlso outsidePointCount = 2 Then

      ' Triangle should be clipped. As two points lie outside
      ' the plane, the triangle simply becomes a smaller triangle

      ' Copy appearance info to new triangle
      outTri1.col = inTri.col
      outTri1.sym = inTri.sym

      ' The inside point is valid, so keep that...
      outTri1.P(0) = insidePoints(0)
      outTri1.T(0) = insideTex(0)

      ' but the two new points are at the locations where the 
      ' original sides of the triangle (lines) intersect with the plane
      Dim t As Single
      outTri1.P(1) = Vector_IntersectPlane(planeP, planeN, insidePoints(0), outsidePoints(0), t)
      outTri1.T(1).U = t * (outsideTex(0).U - insideTex(0).U) + insideTex(0).U
      outTri1.T(1).V = t * (outsideTex(0).V - insideTex(0).V) + insideTex(0).V
      outTri1.T(1).W = t * (outsideTex(0).W - insideTex(0).W) + insideTex(0).W

      outTri1.P(2) = Vector_IntersectPlane(planeP, planeN, insidePoints(0), outsidePoints(1), t)
      outTri1.T(2).U = t * (outsideTex(1).U - insideTex(0).U) + insideTex(0).U
      outTri1.T(2).V = t * (outsideTex(1).V - insideTex(0).V) + insideTex(0).V
      outTri1.T(2).W = t * (outsideTex(1).W - insideTex(0).W) + insideTex(0).W

      Return 1 ' Return the newly formed single triangle

    End If

    If insidePointCount = 2 AndAlso outsidePointCount = 1 Then

      ' Triangle should be clipped. As two points lie inside the plane,
      ' the clipped triangle becomes a "quad". Fortunately, we can
      ' represent a quad with two new triangles
      ' Copy appearance info to new triangles
      outTri1.col = inTri.col
      outTri1.sym = inTri.sym

      outTri2.col = inTri.col
      outTri2.sym = inTri.sym

      ' The first triangle consists of the two inside points and a new
      ' point determined by the location where one side of the triangle
      ' intersects with the plane
      outTri1.P(0) = insidePoints(0)
      outTri1.P(1) = insidePoints(1)
      outTri1.T(0) = insideTex(0)
      outTri1.T(1) = insideTex(1)

      Dim t As Single ' pulled from Vector_IntersectPlane
      outTri1.P(2) = Vector_IntersectPlane(planeP, planeN, insidePoints(0), outsidePoints(0), t)
      outTri1.T(2).U = t * (outsideTex(0).U - insideTex(0).U) + insideTex(0).U
      outTri1.T(2).V = t * (outsideTex(0).V - insideTex(0).V) + insideTex(0).V
      outTri1.T(2).W = t * (outsideTex(0).W - insideTex(0).W) + insideTex(0).W

      ' The second triangle is composed of one of the inside points, a
      ' new point determined by the intersection of the other side of the
      ' triangle and the plane, and the newly created point above
      outTri2.P(0) = insidePoints(1)
      outTri2.T(0) = insideTex(1)
      outTri2.P(1) = outTri1.P(2)
      outTri2.T(1) = outTri1.T(2)
      outTri2.P(2) = Vector_IntersectPlane(planeP, planeN, insidePoints(1), outsidePoints(0), t)
      outTri2.T(2).U = t * (outsideTex(0).U - insideTex(1).U) + insideTex(1).U
      outTri2.T(2).V = t * (outsideTex(0).V - insideTex(1).V) + insideTex(1).V
      outTri2.T(2).W = t * (outsideTex(0).W - insideTex(1).W) + insideTex(1).W

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

    Dim bgCol, fgCol As Integer
    Dim sym As Integer
    Dim pixelBw = CInt(13.0F * lum)

    Select Case pixelBw
      Case 0 : bgCol = BG_BLACK : fgCol = FG_BLACK : sym = PIXEL_SOLID
      Case 1 : bgCol = BG_BLACK : fgCol = FG_DARK_GREY : sym = PIXEL_QUARTER
      Case 2 : bgCol = BG_BLACK : fgCol = FG_DARK_GREY : sym = PIXEL_HALF
      Case 3 : bgCol = BG_BLACK : fgCol = FG_DARK_GREY : sym = PIXEL_THREEQUARTERS
      Case 4 : bgCol = BG_BLACK : fgCol = FG_DARK_GREY : sym = PIXEL_SOLID
      Case 5 : bgCol = BG_DARK_GREY : fgCol = FG_GREY : sym = PIXEL_QUARTER
      Case 6 : bgCol = BG_DARK_GREY : fgCol = FG_GREY : sym = PIXEL_HALF
      Case 7 : bgCol = BG_DARK_GREY : fgCol = FG_GREY : sym = PIXEL_THREEQUARTERS
      Case 8 : bgCol = BG_DARK_GREY : fgCol = FG_GREY : sym = PIXEL_SOLID
      Case 9 : bgCol = BG_GREY : fgCol = FG_WHITE : sym = PIXEL_QUARTER
      Case 10 : bgCol = BG_GREY : fgCol = FG_WHITE : sym = PIXEL_HALF
      Case 11 : bgCol = BG_GREY : fgCol = FG_WHITE : sym = PIXEL_THREEQUARTERS
      Case 12 : bgCol = BG_GREY : fgCol = FG_WHITE : sym = PIXEL_SOLID
        'Case Else : bg_col = BG_BLACK : fg_col = FG_BLACK : sym = PIXEL_SOLID
      Case Else : bgCol = BG_WHITE : fgCol = FG_WHITE : sym = PIXEL_SOLID
    End Select

    Dim c = New CHAR_INFO With {.Attributes = bgCol Or fgCol, .Ch = sym}

    Return c

  End Function

End Class

Friend Class Vec2d

  Friend U As Single = 0.0F
  Friend V As Single = 0.0F
  Friend W As Single = 1.0F

  Friend Sub New()
  End Sub

  Friend Sub New(u As Single, v As Single, Optional w As Single = 1.0F)
    Me.U = u
    Me.V = v
    Me.W = w
  End Sub

  Friend Sub Assign(value As Vec2d)
    U = value.U
    V = value.V
    W = value.W
  End Sub

End Class

Friend Class Vec3d

  Friend Property X As Single
  Friend Property Y As Single
  Friend Property Z As Single

  Friend Property W As Single = 1.0F

  Friend Sub New()
  End Sub

  Friend Sub New(x As Single, y As Single, z As Single, Optional w As Single = 1.0F)
    Me.X = x
    Me.Y = y
    Me.Z = z
    Me.W = w
  End Sub

  Friend Sub Assign(value As Vec3d)
    X = value.X
    Y = value.Y
    Z = value.Z
    W = value.W
  End Sub

End Class

'Friend Class TriangleComparer
'  Implements IComparer(Of Triangle)

'  Public Function Compare(t1 As Triangle, t2 As Triangle) As Integer Implements IComparer(Of Triangle).Compare
'    Dim z1 = (t1.P(0).Z + t1.P(1).Z + t1.P(2).Z) / 3.0F
'    Dim z2 = (t2.P(0).Z + t2.P(1).Z + t2.P(2).Z) / 3.0F
'    If z1 > z2 Then
'      Return -1
'    ElseIf z1 < z2 Then
'      Return 1
'    Else
'      Return 0
'    End If
'  End Function

'End Class

Friend Class Triangle

  Private ReadOnly m_p(2) As Vec3d
  Private ReadOnly m_t(2) As Vec2d ' Added texture coord per vertex
  Public sym As Integer
  Public col As Integer

  Public Property P(index As Integer) As Vec3d
    Get
      Return m_p(index)
    End Get
    Set(value As Vec3d)
      m_p(index).X = value.X
      m_p(index).Y = value.Y
      m_p(index).Z = value.Z
      m_p(index).W = value.W
    End Set
  End Property

  Public Property T(index As Integer) As Vec2d
    Get
      Return m_t(index)
    End Get
    Set(value As Vec2d)
      m_t(index).V = value.V
      m_t(index).U = value.U
      m_t(index).W = value.W
    End Set
  End Property

  Public Sub New()
    m_p(0) = New Vec3d
    m_p(1) = New Vec3d
    m_p(2) = New Vec3d
    m_t(0) = New Vec2d
    m_t(1) = New Vec2d
    m_t(2) = New Vec2d
  End Sub

  Friend Sub New(vector0 As Vec3d, vector1 As Vec3d, vector2 As Vec3d)
    m_p(0) = vector0
    m_p(1) = vector1
    m_p(2) = vector2
    m_t(0) = New Vec2d
    m_t(1) = New Vec2d
    m_t(2) = New Vec2d
  End Sub

  Friend Sub New(vector0 As Vec3d, vector1 As Vec3d, vector2 As Vec3d, texture0 As Vec2d, texture1 As Vec2d, texture2 As Vec2d)
    m_p(0) = vector0
    m_p(1) = vector1
    m_p(2) = vector2
    m_t(0) = texture0
    m_t(1) = texture1
    m_t(2) = texture2
  End Sub

  Friend Sub New(values As Single())
    Dim index = 0
    For entry = 0 To 2
      m_p(entry) = New Vec3d(values(index), values(index + 1), values(index + 2), values(index + 3)) : index += 4
    Next
    For entry = 0 To 2
      m_t(entry) = New Vec2d(values(index), values(index + 1), values(index + 2)) : index += 3
    Next
  End Sub

End Class

Friend Class Mesh

  Public Property Tris As New List(Of Triangle)

  Public Function LoadFromObjectFile(filename As String, Optional hasTexture As Boolean = False) As Boolean

    Dim f = New IO.StreamReader(filename)

    If f Is Nothing Then Return False

    ' Local cache of verts
    Dim verts = New List(Of Vec3d)()
    Dim texs = New List(Of Vec2d)()

    While Not f.EndOfStream

      Dim line = f.ReadLine()
      If line <> "" Then

        Dim values = line.Split(" "c)

        Dim index = 0
        Dim value = values(index) : index += 1

        If value(0) = "v"c Then
          If value(1) = "t"c Then
            Dim v As New Vec2d
            'index += 1 ' skip junk
            index += 1 ' skip junk
            v.U = Single.Parse(values(index)) : index += 1
            v.V = Single.Parse(values(index))
            ' A little hack for the spyro texture
            'v.U = 1.0F - v.U
            'v.V = 1.0F = v.V
            texs.Add(v)
          Else
            Dim v = New Vec3d()
            'index += 1 ' skip junk
            v.X = Single.Parse(values(index)) : index += 1
            v.Y = Single.Parse(values(index)) : index += 1
            v.Z = Single.Parse(values(index))
            verts.Add(v)
          End If
        ElseIf value(0) = "f"c Then
          If Not hasTexture Then
            Dim vals() = New Integer(2) {}
            'index += 1 ' skip junk
            vals(0) = Integer.Parse(values(index)) : index += 1
            vals(1) = Integer.Parse(values(index)) : index += 1
            vals(2) = Integer.Parse(values(index))
            Tris.Add(New Triangle(verts(vals(0) - 1), verts(vals(1) - 1), verts(vals(2) - 1)))
          Else
            Dim tokens(7) As String
            Dim tokenCount = -1
            While index < values.Length - 1
              Dim c = values(index) : index += 1
              If c = " "c Or c = "/"c Then
                tokenCount += 1
              Else
                tokens(tokenCount) &= c
              End If
            End While
            tokens(tokenCount) = tokens(tokenCount).Remove(tokens(tokenCount).Length - 1)
            Tris.Add(New Triangle(verts(CInt(tokens(0)) - 1),
                                       verts(CInt(tokens(2)) - 1),
                                       verts(CInt(tokens(4)) - 1),
                                       texs(CInt(tokens(1)) - 1),
                                       texs(CInt(tokens(3)) - 1),
                                       texs(CInt(tokens(5)) - 1)))
          End If
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