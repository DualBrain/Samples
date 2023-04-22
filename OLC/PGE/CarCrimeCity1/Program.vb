' Inspired by "BIG PROJECT! Top Down City Based Car Crime Game #1" -- @javidx9
' https://youtu.be/mD6b_hP17WI

' current at 49:25 in the video...

Option Explicit On
Option Strict On
Option Infer On

Imports System.Net.Http
Imports Olc
Imports Olc.Gfx3D
Imports Olc.Gfx3D.Math

Friend Module Program

  Sub Main()
    Dim game As New CarCrime
    If game.Construct(768, 480, 2, 2) Then
      game.Start()
    End If
  End Sub

End Module

Friend Class CarCrime
  Inherits PixelGameEngine

  Private ReadOnly meshCube As New Mesh
  Private meshFlat As New Mesh
  Private meshWallsOut As New Mesh

  Private vUp As New Vec3d(0, 1, 0)
  Private vEye As New Vec3d(0, 0, -10) '4)
  Private vLookDir As New Vec3d(0, 0, 1)

  Private ReadOnly pipeRender As New PipeLine

  Private sprAll As Sprite
  Private sprGround As Sprite
  Private sprRoof As Sprite
  Private sprFrontage As Sprite
  Private sprWindows As Sprite
  Private sprRoad(11) As Sprite
  Private sprCar As Sprite

  Private fTheta As Single

  ' Define the cell
  Private Class sCell
    Public nHeight As Integer = 0
    Public nWorldX As Integer = 0
    Public nWorldY As Integer = 0
    Public bRoad As Boolean = False
    Public bBuilding As Boolean = True
  End Class

  ' Map variables
  Private nMapWidth As Integer
  Private nMapHeight As Integer
  Private pMap() As sCell

  Private fCameraX As Single = 0.0F
  Private fCameraY As Single = 0.0F
  Private fCameraZ As Single = -10.0F

  Private fCarAngle As Single = 0.0F
  Private fCarSpeed As Single = 2.0F
  Private vecCarVel As Vec3d = New Vec3d(0, 0, 0)
  Private vecCarPos As Vec3d = New Vec3d(0, 0, 0)

  Private nMouseWorldX As Integer = 0
  Private nMouseWorldY As Integer = 0
  Private matProj As Mat4x4

  Private setSelectedCells As New HashSet(Of sCell)

  Private viewWorldTopLeft As Vec3d
  Private viewWorldBottomRight As Vec3d

  Friend Sub New()
    AppName = "Car Crime City"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    ' Load Sprite Sheet
    sprAll = New Sprite("City_Roads1_mip0.png")

    ' Here we break up the sprite sheet into individual textures. This is more
    ' out of convenience than anything else, as it keeps the texture coordinates
    ' easy to manipulate

    ' Building Lowest Floor
    sprFrontage = New Sprite(32, 96)
    SetDrawTarget(sprFrontage)
    DrawPartialSprite(0, 0, sprAll, 288, 64, 32, 96)

    ' Building Windows
    sprWindows = New Sprite(32, 96)
    SetDrawTarget(sprWindows)
    DrawPartialSprite(0, 0, sprAll, 320, 64, 32, 96)

    ' Plain Grass Field
    sprGround = New Sprite(96, 96)
    SetDrawTarget(sprGround)
    DrawPartialSprite(0, 0, sprAll, 192, 0, 96, 96)

    ' Building Roof
    sprRoof = New Sprite(96, 96)
    SetDrawTarget(sprRoof)
    DrawPartialSprite(0, 0, sprAll, 352, 64, 96, 96)

    ' There are 12 Road Textures, arranged in a 3x4 grid
    For r = 0 To 11
      sprRoad(r) = New Sprite(96, 96)
      SetDrawTarget(sprRoad(r))
      DrawPartialSprite(0, 0, sprAll, (r Mod 3) * 96, (r \ 3) * 96, 96, 96)
    Next

    ' Don't forget to set the draw target back to being the main screen (been there... wasted 1.5 hours :| )
    SetDrawTarget(Nothing)

    ' The Yellow Car
    sprCar = New Sprite("car_top.png")

    ' A Full cube - Always useful for debugging
    meshCube.Tris = New List(Of Triangle) From {New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' SOUTH
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

    ' A Flat quad
    meshFlat.Tris = New List(Of Triangle) From {New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}),
                                                New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F})}

    ' The four outer walls of a cell
    meshWallsOut.Tris = New List(Of Triangle) From {New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F}), ' EAST
                                                    New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 0.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F}),
                                                                                                                                                                                                 _
                                                    New Triangle({0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F}), ' WEST
                                                    New Triangle({0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F}), _
                                                                                                                                                                                                  _
                                                    New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F}), ' TOP
                                                    New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}), _
                                                                                                                                                                                                  _
                                                    New Triangle({1.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F}), ' BOTTOM
                                                    New Triangle({1.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F})}


    ' Initialize the 3D Graphics PGE Extension. This is required
    ' to setup internal buffers to the same size as the main output
    ConfigureDisplay()

    ' Configure the rendering pipeline with projection and viewport properties
    pipeRender.SetProjection(90.0F, CSng(ScreenHeight / ScreenWidth), 0.1F, 1000.0F, 0.0F, 0.0F, ScreenWidth, ScreenHeight)

    matProj = Mat_MakeProjection(90.0F, CSng(ScreenHeight / ScreenWidth), 0.1F, 1000.0F)

    ' Define the city map, a 64x32 array of Cells. Initialize cells to be just grass fields.
    nMapWidth = 64
    nMapHeight = 32
    ReDim pMap(nMapWidth * nMapHeight)
    For x = 0 To nMapWidth - 1
      For y = 0 To nMapHeight - 1
        pMap(y * nMapWidth + x) = New sCell With {.nHeight = 0, .bRoad = False, .nWorldX = x, .nWorldY = y}
      Next
    Next

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    'fTheta += elapsedTime

    'If GetKey(Key.W).Held Then fCameraY -= 2.0F * elapsedTime
    'If GetKey(Key.S).Held Then fCameraY += 2.0F * elapsedTime
    'If GetKey(Key.A).Held Then fCameraX -= 2.0F * elapsedTime
    'If GetKey(Key.D).Held Then fCameraX += 2.0F * elapsedTime
    If GetKey(Key.X).Held Then fCameraZ -= 5.0F * elapsedTime
    If GetKey(Key.Z).Held Then fCameraZ += 5.0F * elapsedTime

    If GetKey(Key.LEFT).Held Then fCarAngle -= 4.0F * elapsedTime
    If GetKey(Key.RIGHT).Held Then fCarAngle += 4.0F * elapsedTime

    Dim a = New Vec3d(1, 0, 0)
    Dim m = Mat_MakeRotationZ(fCarAngle)
    vecCarVel = Mat_MultiplyVector(m, a)

    If GetKey(Key.UP).Held Then
      vecCarPos.X += vecCarVel.X * fCarSpeed * elapsedTime
      vecCarPos.Y += vecCarVel.Y * fCarSpeed * elapsedTime
    End If

    If nMouseWorldX >= 0 AndAlso nMouseWorldX < nMapWidth AndAlso nMouseWorldY >= 0 AndAlso nMouseWorldY < nMapHeight Then

      ' Press "R" to toggle a Road flag for selected cell(s)
      If GetKey(Key.R).Pressed Then
        If setSelectedCells.Any Then
          For Each cell In setSelectedCells
            cell.bRoad = Not cell.bRoad
          Next
        Else
          pMap(nMouseWorldY * nMapWidth + nMouseWorldX).bRoad = Not pMap(nMouseWorldY * nMapWidth + nMouseWorldX).bRoad
        End If
      End If

      ' Press "T" to raise a building...
      If GetKey(Key.T).Pressed Then
        If setSelectedCells.Any Then
          For Each cell In setSelectedCells
            cell.nHeight += 1
          Next
        Else
          pMap(nMouseWorldY * nMapWidth + nMouseWorldX).nHeight += 1
        End If
      End If

      ' Press "E" to lower a building...
      If GetKey(Key.E).Pressed Then
        If setSelectedCells.Any Then
          For Each cell In setSelectedCells
            cell.nHeight -= 1
          Next
        Else
          pMap(nMouseWorldY * nMapWidth + nMouseWorldX).nHeight -= 1
        End If
      End If

    End If

    Clear(Presets.Blue)
    ClearDepth()

    Dim vLookTarget = Vec_Add(vEye, vLookDir)

    ' Setup the camera properties for the pipeline - aka "view" transform
    fCameraX = vecCarPos.X
    fCameraY = vecCarPos.Y
    vEye = New Vec3d(fCameraX, fCameraY, fCameraZ)
    pipeRender.SetCamera(vEye, vLookTarget, vUp)

    ' Create a point at matrix, if you recall, this is the inverse of the look at matrix
    ' used by the camera
    Dim matView = Mat_PointAt(vEye, vLookTarget, vUp)

    ' Assume the origin of the mouse ray is the middle of the screen...
    Dim vecMouseOrigin = New Vec3d(0.0F, 0.0F, 0.0F)

    ' ... and that a ray is cast to the mouse location from the origin. Here we translate
    ' the mouse coordinates into viewport coordinates
    Dim vecMouseDir = New Vec3d(2.0F * (CSng(GetMouseX() / ScreenWidth) - 0.5F) / matProj.M(0, 0),
                                2.0F * (CSng(GetMouseY() / ScreenHeight) - 0.5F) / matProj.M(1, 1),
                                1.0F,
                                0.0F)

    ' Now transform the origin point and ray direction by the inverse of the camera
    vecMouseOrigin = Mat_MultiplyVector(matView, vecMouseOrigin)
    vecMouseDir = Mat_MultiplyVector(matView, vecMouseDir)

    ' Extend the mouse ray to a large length
    vecMouseDir = Vec_Mul(vecMouseDir, 1000.0F)

    ' Offset the mouse ray by the mouse origin
    vecMouseDir = Vec_Add(vecMouseOrigin, vecMouseDir)

    ' All of our intersections for mouse checks occur in the ground plane (z=0), so
    ' define a plane at that location
    Dim plane_p = New Vec3d(0.0F, 0.0F, 0.0F)
    Dim plane_n = New Vec3d(0.0F, 0.0F, 1.0F)

    ' Calculate Mouse Location in plane, by doing a line/plane intersection test
    Dim t = 0.0F
    Dim mouse3d = Vec_IntersectPlane(plane_p, plane_n, vecMouseOrigin, vecMouseDir, t)

    nMouseWorldX = CInt(Fix(mouse3d.X))
    nMouseWorldY = CInt(Fix(mouse3d.Y))

    If GetMouse(0).Held Then
      setSelectedCells.Add(pMap(nMouseWorldY * nMapWidth + nMouseWorldX))
    End If
    If GetMouse(1).Released Then
      setSelectedCells.Clear()
    End If

    ' Work out the Top Left Ground Cell
    vecMouseDir = New Vec3d(-1.0F / matProj.M(0, 0), -1.0F / matProj.M(1, 1), 1.0F, 0.0F)
    vecMouseDir = Mat_MultiplyVector(matView, vecMouseDir)
    vecMouseDir = Vec_Mul(vecMouseDir, 1000.0F)
    vecMouseDir = Vec_Add(vecMouseOrigin, vecMouseDir)
    viewWorldTopLeft = Vec_IntersectPlane(plane_p, plane_n, vecMouseOrigin, vecMouseDir, t)

    ' Work out the Bottom Right Ground Cell
    vecMouseDir = New Vec3d(1.0F / matProj.M(0, 0), 1.0F / matProj.M(1, 1), 1.0F, 0.0F)
    vecMouseDir = Mat_MultiplyVector(matView, vecMouseDir)
    vecMouseDir = Vec_Mul(vecMouseDir, 1000.0F)
    vecMouseDir = Vec_Add(vecMouseOrigin, vecMouseDir)
    viewWorldBottomRight = Vec_IntersectPlane(plane_p, plane_n, vecMouseOrigin, vecMouseDir, t)

    Dim nStartX = Integer.Max(0, CInt(Fix(viewWorldTopLeft.X - 1)))
    Dim nEndX = Integer.Min(nMapWidth, CInt(Fix(viewWorldBottomRight.X + 1)))
    Dim nStartY = Integer.Max(0, CInt(Fix(viewWorldTopLeft.Y - 1)))
    Dim nEndY = Integer.Min(nMapHeight, CInt(Fix(viewWorldBottomRight.Y + 1)))

    For x = nStartX To nEndX - 1
      For y = nStartY To nEndY - 1

        If pMap(y * nMapWidth + x).bRoad Then

          Dim road = 0

          Dim r = Function(i As Integer, j As Integer) As Boolean
                    Return pMap((y + j) * nMapWidth + (x + i)).bRoad
                  End Function

          If r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso Not r(+1, 0) Then road = 0
          If Not r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then road = 1

          If Not r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then road = 3
          If Not r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then road = 4

          If Not r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then road = 5

          If r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then road = 6
          If r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then road = 7
          If r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then road = 8

          If r(0, -1) AndAlso Not r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then road = 9
          If r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then road = 10
          If r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then road = 11

          ' Set the appropriate texture to use
          pipeRender.SetTexture(sprRoad(road))
          Dim matWorld = Mat_MakeTranslation(x, y, 0.0F)
          pipeRender.SetTransform(matWorld)
          pipeRender.Render(meshFlat.Tris)

        Else

          If pMap(y * nMapWidth + x).nHeight < 0 Then
            ' Water?
          End If

          If pMap(y * nMapWidth + x).nHeight = 0 Then
            ' Cell is ground, draw a flat grass quad at height 0
            Dim matWorld = Mat_MakeTranslation(x, y, 0.0F)
            pipeRender.SetTransform(matWorld)
            pipeRender.SetTexture(sprGround)
            pipeRender.Render(meshFlat.Tris)
          End If

          If pMap(y * nMapWidth + x).nHeight > 0 Then

            ' Cell is a Building, for now, we'll draw each story as a seperate mesh
            For h = 0 To pMap(y * nMapWidth + x).nHeight - 1

              ' Create a transform that positions the story according to its height
              Dim matWorld = Mat_MakeTranslation(x, y, -(h + 1) * 0.2F)
              pipeRender.SetTransform(matWorld)

              ' Choose a texture, if its ground level, use the "street level front", otherwise use windows
              pipeRender.SetTexture(If(h = 0, sprFrontage, sprWindows))
              pipeRender.Render(meshWallsOut.Tris)

            Next

            If True Then
              ' Top the building off with a roof
              Dim h = pMap(y * nMapWidth + x).nHeight
              Dim matworld = Mat_MakeTranslation(x, y, -(h) * 0.2F)
              pipeRender.SetTransform(matworld)
              pipeRender.SetTexture(sprRoof)
              pipeRender.Render(meshFlat.Tris)
            End If

          End If

        End If
      Next
    Next

    ' Draw Selected Cells, iterate through the set of cells, and draw a wireframe quad at ground level
    ' to indicate it is in the selection set
    For Each cell In setSelectedCells
      Dim matWorld = Mat_MakeTranslation(cell.nWorldX, cell.nWorldY, 0.0F)
      pipeRender.SetTransform(matWorld)
      pipeRender.Render(meshFlat.Tris, RenderFlags.RenderWire)
    Next

    ' Draw Car, a few transforms required for this

    ' 1) Offset the car to the middle of the quad
    Dim matCarOffset = Mat_MakeTranslation(-0.5F, -0.5F, -0.0F)
    ' 2) The quad is currently unit square, scale it to be more rectangular and smaller than the cells
    Dim matCarScale = Mat_MakeScale(0.4F, 0.2F, 1.0F)
    ' 3) Combine into matrix
    Dim matCar = Mat_MultiplyMatrix(matCarOffset, matCarScale)
    ' 4) Rotate the car around its offset origin, according to its angle
    Dim matCarRot = Mat_MakeRotationZ(fCarAngle)
    matCar = Mat_MultiplyMatrix(matCar, matCarRot)
    ' 5) Translate the car into its position in the world. Give it a little elevation so its above the ground
    Dim matCarTrans = Mat_MakeTranslation(vecCarPos.X, vecCarPos.Y, -0.01F)
    matCar = Mat_MultiplyMatrix(matCar, matCarTrans)

    ' Apply "world" transform to pipeline
    pipeRender.SetTransform(matCar)
    ' Set the car texture to the pipeline
    pipeRender.SetTexture(sprCar)

    ' The car has transparency, so enable it
    SetPixelMode(Pixel.Mode.Alpha)
    ' Render the quad
    pipeRender.Render(meshFlat.Tris)
    ' Set transparency back to none to optimise drawing other pixels
    SetPixelMode(Pixel.Mode.Normal)

    Return True

  End Function

End Class