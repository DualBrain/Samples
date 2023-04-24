Option Explicit On
Option Strict On
Option Infer On

Imports System.Drawing
Imports System.Formats
Imports Olc
Imports Olc.Gfx3D
Imports Olc.Gfx3D.Math

Public Class cCarCrimeCity
  Inherits PixelGameEngine

  Dim mapAssetTextures As New Dictionary(Of String, Olc.Sprite)
  Dim mapAssetMeshes As New Dictionary(Of String, Olc.Gfx3D.Mesh)
  Dim mapAssetTransform As New Dictionary(Of String, Olc.Gfx3D.Mat4x4)

  ' Camera variables
  Dim vCamera As Olc.Gfx3D.Vec3d = New Olc.Gfx3D.Vec3d(0.0F, 0.0F, -3.0F)
  Dim vUp As Olc.Gfx3D.Vec3d = New Olc.Gfx3D.Vec3d(0.0F, 1.0F, 0.0F)
  Dim vEye As Olc.Gfx3D.Vec3d = New Olc.Gfx3D.Vec3d(0.0F, 0.0F, -3.0F)
  Dim vLookDir As Olc.Gfx3D.Vec3d = New Olc.Gfx3D.Vec3d(0.0F, 0.0F, 1.0F)

  ' Ray Casting Parameters
  Dim viewWorldTopLeft As Olc.Vf2d
  Dim viewWorldBottomRight As Olc.Vf2d

  ' Cloud movement variables
  Dim fCloudOffsetX As Single = 0.0F
  Dim fCloudOffsetY As Single = 0.0F

  ' Mouse Control
  Dim vOffset As Olc.Vf2d = New Olc.Vf2d(0.0F, 0.0F)
  Dim vStartPan As Olc.Vf2d = New Olc.Vf2d(0.0F, 0.0F)
  Dim vMouseOnGround As Olc.Vf2d = New Olc.Vf2d(0.0F, 0.0F)
  Dim fScale As Single = 1.0F

  Dim listAutomata As New List(Of cAuto_Body)()

  Dim pCity As cCityMap = Nothing

  Dim fGlobalTime As Single = 0.0F

  ' Editing Utilities
  Dim bEditMode As Boolean = True
  Dim nMouseX As Integer = 0
  Dim nMouseY As Integer = 0

  Structure sCellLoc
    Dim x As Integer
    Dim y As Integer
  End Structure

  Dim setSelectedCells As New HashSet(Of Integer)

  Dim carvel As Olc.Vf2d
  Dim carpos As Olc.Vf2d
  Dim fSpeed As Single = 0.0F
  Dim fAngle As Single = 0.0F

  'Dim goCar As cGameObjectQuad = Nothing
  'Dim goObstacle As cGameObjectQuad = Nothing

  'Dim vecObstacles As New List(Of cGameObjectQuad)

  'Dim nTrafficState As Integer = 0

  Friend Sub New()
    AppName = "Car Crime City"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    ' Initialise PGEX 3D
    ConfigureDisplay()

    ' Load fixed system assets, i.e. those need to simply do anything
    If Not LoadAssets() Then
      Return False
    End If

    ' Create Default city
    pCity = New cCityMap(cGameSettings.nDefaultMapWidth, cGameSettings.nDefaultMapHeight, mapAssetTextures, mapAssetMeshes, mapAssetTransform)

    ' If a city map file has been specified, then load it
    If Not String.IsNullOrEmpty(cGameSettings.sDefaultCityFile) Then
      If Not pCity.LoadCity(cGameSettings.sDefaultCityFile) Then
        Console.WriteLine("Failed to load '{0}'", cGameSettings.sDefaultCityFile)
        Return False
      End If
    End If

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    fGlobalTime += elapsedTime

    If (GetKey(Key.TAB).Released) Then bEditMode = Not bEditMode

    If (bEditMode) Then ' Use mouse to pan and zoom, and place objects

      vEye = vCamera
      Dim vMouseScreen = New Vf2d(GetMouseX(), GetMouseY())
      Dim vMouseOnGroundBeforeZoom = GetMouseOnGround(vMouseScreen)

      vOffset = New Vf2d(0, 0)

      If (IsFocused()) Then

        If (GetMouse(2).Pressed) Then vStartPan = vMouseOnGroundBeforeZoom
        If (GetMouse(2).Held) Then vOffset = (vStartPan - vMouseOnGroundBeforeZoom)

        If (GetMouseWheel() > 0) Then
          vCamera.Z *= 0.5F
        End If

        If (GetMouseWheel() < 0) Then
          vCamera.Z *= 1.5F
        End If
      End If

      vEye = vCamera
      Dim vMouseOnGroundAfterZoom = GetMouseOnGround(vMouseScreen)
      vOffset += (vMouseOnGroundBeforeZoom - vMouseOnGroundAfterZoom)
      vCamera.X += vOffset.x
      vCamera.Y += vOffset.y
      vEye = vCamera

      ' Get Integer versions of mouse coords in world space
      nMouseX = CInt(Fix(vMouseOnGroundAfterZoom.x))
      nMouseY = CInt(Fix(vMouseOnGroundAfterZoom.y))

      DoEditMode(elapsedTime)

    Else

      ' Not in edit mode, so camera follows player
      If (GetKey(Key.LEFT).Held) Then fAngle += -2.5F * elapsedTime
      If (GetKey(Key.RIGHT).Held) Then fAngle += 2.5F * elapsedTime
      If (GetKey(Key.UP).Held) Then
        carvel = New Vf2d(MathF.Cos(fAngle), MathF.Sin(fAngle))
        carpos += carvel * 2.0F * elapsedTime
      End If

      vCamera.X = carpos.x
      vCamera.Y = carpos.y
      vEye = vCamera

    End If

    ' Calculate Visible ground plane dimensions
    viewWorldTopLeft = GetMouseOnGround(New Vf2d(0.0F, 0.0F))
    viewWorldBottomRight = GetMouseOnGround(New Vf2d(CSng(ScreenWidth()), CSng(ScreenHeight())))

    ' Calculate visible world extents
    Dim nStartX As Integer = Integer.Max(0, CInt(viewWorldTopLeft.x) - 1)
    Dim nEndX As Integer = Integer.Min(pCity.GetWidth(), CInt(viewWorldBottomRight.x) + 1)
    Dim nStartY As Integer = Integer.Max(0, CInt(viewWorldTopLeft.y) - 1)
    Dim nEndY As Integer = Integer.Min(pCity.GetHeight(), CInt(viewWorldBottomRight.y) + 1)

    ' Only update automata for cells near player
    Dim nAutomStartX As Integer = Integer.Max(0, CInt(viewWorldTopLeft.x) - 3)
    Dim nAutomEndX As Integer = Integer.Min(pCity.GetWidth(), CInt(viewWorldBottomRight.x) + 3)
    Dim nAutomStartY As Integer = Integer.Max(0, CInt(viewWorldTopLeft.y) - 3)
    Dim nAutomEndY As Integer = Integer.Min(pCity.GetHeight(), CInt(viewWorldBottomRight.y) + 3)

    Dim nLocalStartX As Integer = Integer.Max(0, CInt(vCamera.X) - 3)
    Dim nLocalEndX As Integer = Integer.Min(pCity.GetWidth(), CInt(vCamera.X) + 3)
    Dim nLocalStartY As Integer = Integer.Max(0, CInt(vCamera.Y) - 3)
    Dim nLocalEndY As Integer = Integer.Min(pCity.GetHeight(), CInt(vCamera.Y) + 3)

    ' Update Cells
    For x = nStartX To nEndX - 1
      For y = nStartY To nEndY - 1
        pCity.Cell(x, y).Update(elapsedTime)
      Next
    Next

    '' Update Automata
    'For Each a In listAutomata

    '  a.UpdateAuto(elapsedTime)

    '  ' If automata is too far from camera, remove it
    '  If (a.vAutoPos - New Olc.Vf2d(vCamera.X, vCamera.Y)).Mag() > 5.0F Then
    '    ' Despawn automata

    '    ' 1) Disconnect it from track
    '    a.pCurrentTrack.listAutos.Remove(a)

    '    ' 2) Erase it from memory
    '    a = Nothing
    '  End If

    'Next

    '' Remove dead automata, their pointer has been set to Nothing in the list
    'listAutomata.RemoveAll(Function(a) a Is Nothing)

    '' Maintain a certain level of automata in vicinity of player
    'If listAutomata.Count < 20 Then

    '  Dim bSpawnOK = False
    '  Dim nSpawnAttempt = 20

    '  While Not bSpawnOK AndAlso nSpawnAttempt > 0

    '    ' Find random cell on edge of vicinity, which is out of view of the player
    '    Dim fRandomAngle As Single = (CSng(Rnd()) * 2.0F * 3.14159F)
    '    Dim nRandomCellX As Integer = CInt(vCamera.X + MathF.Cos(fRandomAngle) * 3.0F)
    '    Dim nRandomCellY As Integer = CInt(vCamera.Y + MathF.Sin(fRandomAngle) * 3.0F)

    '    nSpawnAttempt -= 1

    '    If pCity.Cell(nRandomCellX, nRandomCellY) IsNot Nothing AndAlso pCity.Cell(nRandomCellX, nRandomCellY).nCellType = CellType.CELL_ROAD Then

    '      bSpawnOK = True

    '      ' Add random automata
    '      If CInt(Rnd() * 100) < 50 Then
    '        ' Spawn Pedestrian
    '        SpawnPedestrian(nRandomCellX, nRandomCellY)
    '      Else
    '        ' Spawn Vehicle
    '        SpawnVehicle(nRandomCellX, nRandomCellY)
    '        ' TODO: Get % chance of vehicle spawn from lua script
    '      End If

    '    End If

    '  End While

    'End If

    ' Render Scene
    Clear(Presets.Blue)
    Gfx3D.ClearDepth()

    ' Create rendering pipeline
    Dim pipe As New PipeLine()
    pipe.SetProjection(90.0F, CSng(ScreenHeight() / ScreenWidth()), 0.1F, 1000.0F, 0.0F, 0.0F, ScreenWidth(), ScreenHeight())
    Dim vLookTarget = Vec_Add(vEye, vLookDir)
    pipe.SetCamera(vEye, vLookTarget, vUp)

    ' Add global illumination vector (sunlight)
    Dim lightdir As New Gfx3D.Vec3d(1.0F, 1.0F, -1.0F)
    pipe.SetLightSource(0, Gfx3D.Light.Ambient, New Pixel(100, 100, 100), New Vec3d(0, 0, 0), lightdir)
    pipe.SetLightSource(1, Gfx3D.Light.Directional, Presets.White, New Vec3d(0, 0, 0), lightdir)

    ' RENDER CELL CONTENTS

    ' Render Base Objects (those without alpha components)
    For x = nStartX To nEndX - 1
      For y = nStartY To nEndY - 1
        pCity.Cell(x, y).DrawBase(Me, pipe)
      Next
    Next

    ' Render Upper Objects (those with alpha components)
    For x = nStartX To nEndX - 1
      For y = nStartY To nEndY - 1
        pCity.Cell(x, y).DrawAlpha(Me, pipe)
      Next
    Next

    If bEditMode Then
      ' Render additional per cell debug information
      For x = nStartX To nEndX - 1
        For y = nStartY To nEndY - 1
          pCity.Cell(x, y).DrawDebug(Me, pipe)
        Next
      Next
    End If

    If bEditMode Then
      ' Draw Selections
      For Each c In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim matWorld1 = Mat_MakeTranslation(CSng(x), CSng(y), 0.01F)
        pipe.SetTransform(matWorld1)
        pipe.Render(mapAssetMeshes("UnitQuad").Tris, Gfx3D.RenderFlags.RenderWire)
      Next
    End If

    ' RENDER AUTOMATA

    Dim test() = {"Sedan", "SUV", "TruckCab", "TruckTrailer", "UTE", "Wagon"}
    Dim i = 0
    For Each a In listAutomata

      Dim v = New Vec3d(a.vAutoPos.x, a.vAutoPos.y, 0.0F)

      'Dim matWorld As olc.GFX3D.mat4x4 = olc.GFX3D.Math.Mat_MakeTranslation(a.vAutoPos.x, a.vAutoPos.y, 0.01F)
      'matWorld = olc.GFX3D.Math.Mat_MultiplyMatrix(mapAssetTransform(test(i)), matWorld)
      'pipe.SetTransform(matWorld)
      'pipe.SetTexture(mapAssetTextures(test(i)))
      'pipe.Render(mapAssetMeshes(test(i)).tris, olc.GFX3D.RENDER_CULL_CW Or olc.GFX3D.RENDER_DEPTH Or olc.GFX3D.RENDER_TEXTURED Or olc.GFX3D.RENDER_LIGHTS)
      'i += 1
      'i = i Mod 6

      pipe.RenderCircleXZ(v, If(a.fAutoLength < 0.1F, 0.05F, 0.07F), If(a.fAutoLength < 0.1F, Presets.Magenta, Presets.Yellow))

    Next

    ' Draw Player Vehicle
    Dim matRotateZ = Mat_MakeRotationZ(fAngle)
    Dim matTranslate = Mat_MakeTranslation(carpos.x, carpos.y, 0.01F)
    Dim matWorld = Mat_MultiplyMatrix(mapAssetTransform("Sedan"), matRotateZ)
    matWorld = Mat_MultiplyMatrix(matWorld, matTranslate)
    pipe.SetTransform(matWorld)
    pipe.SetTexture(mapAssetTextures("Sedan"))
    pipe.Render(mapAssetMeshes("Sedan").Tris, RenderFlags.RenderCullCw Or RenderFlags.RenderDepth Or RenderFlags.RenderTextured Or RenderFlags.RenderLights)

    DrawString(10, 10, "Automata: " & listAutomata.Count.ToString(), Presets.White)
    DrawString(10, 20, $"Car: {carpos.x},{carpos.y}", Presets.White)

    If GetKey(Key.ESCAPE).Pressed Then
      Return False
    End If

    Return True

  End Function

  Sub SpawnPedestrian(x As Integer, y As Integer)
    Dim cell = pCity.Cell(x, y)
    Dim t = TryCast(cell, cCell_Road)?.pSafePedestrianTrack
    If t Is Nothing Then Return
    Dim a As New cAuto_Body With {.fAutoLength = 0.05F, .pCurrentTrack = t}
    t.listAutos.Add(a)
    a.pTrackOriginNode = t.node(0)
    a.UpdateAuto(0.0F)
    listAutomata.Add(a)
  End Sub

  Sub SpawnVehicle(x As Integer, y As Integer)
    Dim cell = pCity.Cell(x, y)
    Dim t = TryCast(cell, cCell_Road)?.pSafeCarTrack
    If t Is Nothing Then Return
    Dim a As New cAuto_Body With {.fAutoLength = 0.2F, .pCurrentTrack = t}
    t.listAutos.Add(a)
    a.pTrackOriginNode = t.node(0)
    a.UpdateAuto(0.0F)
    listAutomata.Add(a)
  End Sub

  Sub DoEditMode(fElapsedTime As Single)

    ' Get cell under mouse cursor
    Dim mcell As cCell = pCity.Cell(nMouseX, nMouseY)
    Dim bTempCellAdded As Boolean = False

    ' Left click and drag adds cells
    If mcell IsNot Nothing AndAlso GetMouse(0).Held Then
      setSelectedCells.Add(nMouseY * pCity.GetWidth() + nMouseX)
    End If

    ' Right click clears selection
    If GetMouse(1).Released Then
      setSelectedCells.Clear()
    End If

    If setSelectedCells.Count = 0 Then
      ' If nothing can be edited validly then just exit
      If mcell Is Nothing Then
        Return
      End If
      ' else set is empty, so temporarily add current cell to it
      setSelectedCells.Add(nMouseY * pCity.GetWidth() + nMouseX)
      bTempCellAdded = True
    End If

    ' If the map changes, we will need to update
    ' the automata, and adjacency
    Dim bMapChanged As Boolean = False

    ' Press "G" to apply grass
    If GetKey(Key.G).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim cell As cCell = pCity.Replace(x, y, New cCell_Plane(pCity, x, y, CELL_PLANE.PLANE_GRASS))
        cell.LinkAssets(mapAssetTextures, mapAssetMeshes, mapAssetTransform)
      Next

      bMapChanged = True
    End If

    ' Press "P" to apply Pavement
    If GetKey(Key.P).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim cell As cCell = pCity.Replace(x, y, New cCell_Plane(pCity, x, y, CELL_PLANE.PLANE_ASPHALT))
        cell.LinkAssets(mapAssetTextures, mapAssetMeshes, mapAssetTransform)
      Next

      bMapChanged = True
    End If

    ' Press "W" to apply Water
    If GetKey(Key.W).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim cell As cCell = pCity.Replace(x, y, New cCell_Water(pCity, x, y))
        cell.LinkAssets(mapAssetTextures, mapAssetMeshes, mapAssetTransform)
      Next

      bMapChanged = True
    End If

    ' Press "Q" to apply Buildings
    If GetKey(Key.Q).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim cell As cCell = pCity.Replace(x, y, New cCell_Building("Apartments_1", pCity, x, y))
        cell.LinkAssets(mapAssetTextures, mapAssetMeshes, mapAssetTransform)
      Next

      bMapChanged = True
    End If

    ' Press "R" to apply Roads
    If GetKey(Key.R).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        Dim cell As cCell = pCity.Replace(x, y, New cCell_Road(pCity, x, y))
        cell.LinkAssets(mapAssetTextures, mapAssetMeshes, mapAssetTransform)
      Next

      bMapChanged = True
    End If

    If GetKey(Key.C).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        SpawnVehicle(x, y)
      Next
    End If

    If GetKey(Key.V).Pressed Then
      For Each c As Integer In setSelectedCells
        Dim x As Integer = c Mod pCity.GetWidth()
        Dim y As Integer = c \ pCity.GetWidth()
        SpawnPedestrian(x, y)
      Next
    End If

    If bMapChanged Then
      ' The navigation nodes may have tracks attached to them, so get rid of them
      ' all. Below we will reconstruct all tracks because city has changed
      pCity.RemoveAllTracks()

      For Each a In listAutomata
        a = Nothing '.Dispose()
      Next
      listAutomata.Clear()

      For x As Integer = 0 To pCity.GetWidth() - 1
        For y As Integer = 0 To pCity.GetHeight() - 1
          Dim c As cCell = pCity.Cell(x, y)

          ' Update adjacency information, i.e. those cells whose
          ' state changes based on neighbouring cells
          c.CalculateAdjacency()
        Next
      Next
    End If

    ' To facilitate "edit under cursor" we added a temporary cell
    ' which needs to be removed now
    If bTempCellAdded Then
      setSelectedCells.Clear()
    End If

  End Sub

  Function GetMouseOnGround(vMouseScreen As Vf2d) As Vf2d
    Dim vLookTarget = Vec_Add(vEye, vLookDir)
    Dim matProj = Mat_MakeProjection(90.0F, CSng(ScreenHeight()) / CSng(ScreenWidth()), 0.1F, 1000.0F)
    Dim matView = Mat_PointAt(vEye, vLookTarget, vUp)
    Dim vecMouseDir As New Vec3d(2.0F * ((vMouseScreen.x / CSng(ScreenWidth())) - 0.5F) / matProj.M(0, 0),
                                 2.0F * ((vMouseScreen.y / CSng(ScreenHeight())) - 0.5F) / matProj.M(1, 1),
                                 1.0F, 0.0F)
    Dim vecMouseOrigin As New Vec3d(0.0F, 0.0F, 0.0F)
    vecMouseOrigin = Mat_MultiplyVector(matView, vecMouseOrigin)
    vecMouseDir = Mat_MultiplyVector(matView, vecMouseDir)
    vecMouseDir = Vec_Mul(vecMouseDir, 1000.0F)
    vecMouseDir = Vec_Add(vecMouseOrigin, vecMouseDir)
    Dim plane_p As New Vec3d(0.0F, 0.0F, 0.0F)
    Dim plane_n As New Vec3d(0.0F, 0.0F, 1.0F)
    Dim t = 0.0F
    Dim mouse3d = Vec_IntersectPlane(plane_p, plane_n, vecMouseOrigin, vecMouseDir, t)
    Return New Vf2d(mouse3d.X, mouse3d.Y)
  End Function

  Function LoadAssets() As Boolean
    ' Game Settings should have loaded all the relevant file information
    ' to start loading asset information. Game assets will be stored in
    ' a map structure. Maps can have slightly longer access times, so each
    ' in game object will have facility to extract required resources once
    ' when it is created, meaning no map search during normal use  End Function

    ' System Meshes
    ' A simple flat unit quad
    Dim meshQuad As New Mesh()
    meshQuad.Tris = New List(Of Triangle) From {New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White),
                                                New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F}, Presets.White, Presets.White, Presets.White)}
    mapAssetMeshes("UnitQuad") = meshQuad

    ' The four outer walls of a cell
    Dim meshWallsOut As New Mesh()
    meshWallsOut.Tris = New List(Of Triangle) From {New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F}, Presets.White, Presets.White, Presets.White), ' EAST
                                                    New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 0.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White),
                                                                                                                                                                                                                                              _
                                                    New Triangle({0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F}, Presets.White, Presets.White, Presets.White), ' WEST
                                                    New Triangle({0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White), _
                                                                                                                                                                                                                                               _
                                                    New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White), ' TOP
                                                    New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.2F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White), _
                                                                                                                                                                                                                                               _
                                                    New Triangle({1.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F}, Presets.White, Presets.White, Presets.White), ' BOTTOM
                                                    New Triangle({1.0F, 0.0F, 0.2F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F}, Presets.White, Presets.White, Presets.White)}
    mapAssetMeshes("WallsOut") = meshWallsOut

    ' System Textures
    For Each asset In cGameSettings.vecAssetTextures
      Dim sprAsset As New Olc.Sprite()
      If sprAsset.LoadFromFile(asset.sFile) = RCode.OK Then
        mapAssetTextures(asset.sName) = sprAsset
      Else
        Console.WriteLine("Failed to load " & asset.sName)
        Return False
      End If
    Next

    ' Break up roads sprite into individual sprites. Why? Its easier to maintain
    ' the roads sprite as a single image, but easier to use if they are all individual.
    ' Breaking it up manually in the image editing software is time consuming so just
    ' do it here
    Dim nRoadTexSize = 256 ' In pixels in base texture
    Dim nRoadTexOffset = 64 ' There exists a 64 pixel offset from top left of source image
    For r = 0 To 11
      Dim road As New Sprite(nRoadTexSize, nRoadTexSize)
      SetDrawTarget(road)
      DrawPartialSprite(0, 0, mapAssetTextures("AllRoads"), ((r Mod 3) * nRoadTexSize) + nRoadTexOffset, ((r \ 3) * nRoadTexSize) + nRoadTexOffset, nRoadTexSize, nRoadTexSize)
      Select Case r
        Case 0 : mapAssetTextures("Road_V") = road
        Case 1 : mapAssetTextures("Road_H") = road
        Case 2 : mapAssetTextures("Pavement") = road
        Case 3 : mapAssetTextures("Road_C1") = road
        Case 4 : mapAssetTextures("Road_T1") = road
        Case 5 : mapAssetTextures("Road_C2") = road
        Case 6 : mapAssetTextures("Road_T2") = road
        Case 7 : mapAssetTextures("Road_X") = road
        Case 8 : mapAssetTextures("Road_T3") = road
        Case 9 : mapAssetTextures("Road_C3") = road
        Case 10 : mapAssetTextures("Road_T4") = road
        Case 11 : mapAssetTextures("Road_C4") = road
      End Select
    Next
    SetDrawTarget(Nothing)

    ' Load Buildings
    For Each asset In cGameSettings.vecAssetBuildings
      mapAssetMeshes(asset.sDescription) = New Mesh()
      mapAssetMeshes(asset.sDescription).LoadOBJFile(asset.sModelOBJ)
      mapAssetTextures(asset.sDescription) = New Sprite(asset.sModelPNG)
      Dim matScale = Mat_MakeScale(asset.fScale(0), asset.fScale(1), asset.fScale(2))
      Dim matTranslate = Mat_MakeTranslation(asset.fTranslate(0), asset.fTranslate(1), asset.fTranslate(2))
      Dim matRotateX = Mat_MakeRotationX(asset.fRotate(0))
      Dim matRotateY = Mat_MakeRotationY(asset.fRotate(1))
      Dim matRotateZ = Mat_MakeRotationZ(asset.fRotate(2))
      Dim matTransform = Mat_MultiplyMatrix(matTranslate, matScale)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateX)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateY)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateZ)
      mapAssetTransform(asset.sDescription) = matTransform
    Next

    ' Load Vehicles
    For Each asset In cGameSettings.vecAssetVehicles
      mapAssetMeshes(asset.sDescription) = New Mesh()
      mapAssetMeshes(asset.sDescription).LoadOBJFile(asset.sModelOBJ)
      mapAssetTextures(asset.sDescription) = New Sprite(asset.sModelPNG)
      Dim matScale = Mat_MakeScale(asset.fScale(0), asset.fScale(1), asset.fScale(2))
      Dim matTranslate = Mat_MakeTranslation(asset.fTranslate(0), asset.fTranslate(1), asset.fTranslate(2))
      Dim matRotateX = Mat_MakeRotationX(asset.fRotate(0))
      Dim matRotateY = Mat_MakeRotationY(asset.fRotate(1))
      Dim matRotateZ = Mat_MakeRotationZ(asset.fRotate(2))
      Dim matTransform = Mat_MultiplyMatrix(matTranslate, matScale)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateX)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateY)
      matTransform = Mat_MultiplyMatrix(matTransform, matRotateZ)
      mapAssetTransform(asset.sDescription) = matTransform
    Next

    Return True

  End Function

  Private Class cGameObjectQuad

    Dim meshTris As List(Of Gfx3D.Triangle)
    Dim vecPointsModel As List(Of Gfx3D.Vec3d)
    Dim vecPointsWorld As List(Of Gfx3D.Vec3d)
    Dim pos As Gfx3D.Vec3d

    Dim fWidth As Single
    Dim fHeight As Single
    Dim fOriginX As Single
    Dim fOriginY As Single
    Dim fAngle As Single

    Structure vec2d
      Public x As Single
      Public y As Single
      Public Sub New(x As Single, y As Single)
        Me.x = x
        Me.y = y
      End Sub
    End Structure

    Public Sub New(w As Single, h As Single)

      fWidth = w
      fHeight = h
      fAngle = 0.0F

      ' Construct Model Quad Geometry
      vecPointsModel = New List(Of Vec3d) From {
          New Vec3d(-fWidth / 2.0F, -fHeight / 2.0F, -0.01F, 1.0F),
          New Vec3d(-fWidth / 2.0F, +fHeight / 2.0F, -0.01F, 1.0F),
          New Vec3d(+fWidth / 2.0F, +fHeight / 2.0F, -0.01F, 1.0F),
          New Vec3d(+fWidth / 2.0F, -fHeight / 2.0F, -0.01F, 1.0F)}

      vecPointsWorld = New List(Of Vec3d)(vecPointsModel.Count - 1) ' {}
      TransformModelToWorld()
    End Sub

    Public Sub TransformModelToWorld()
      For i = 0 To vecPointsModel.Count - 1
        vecPointsWorld(i) = New Vec3d(
            vecPointsModel(i).X * MathF.Cos(fAngle) - vecPointsModel(i).Y * MathF.Sin(fAngle) + pos.X,
            vecPointsModel(i).X * MathF.Sin(fAngle) + vecPointsModel(i).Y * MathF.Cos(fAngle) + pos.Y,
            vecPointsModel(i).Z,
            vecPointsModel(i).W)
      Next
    End Sub

    Public Function GetTriangles() As List(Of Olc.Gfx3D.Triangle)
      ' Return triangles based upon this quad
      Return New List(Of Triangle) From {
          New Triangle(vecPointsWorld(0), vecPointsWorld(1), vecPointsWorld(2), 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, Presets.Red, Presets.Red, Presets.Red),
          New Triangle(vecPointsWorld(0), vecPointsWorld(2), vecPointsWorld(3), 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, Presets.Red, Presets.Red, Presets.Red)}
    End Function

    ' Use rectangle edge intersections.
    Public Function StaticCollisionWith(ByRef r2 As cGameObjectQuad, Optional bResolveStatic As Boolean = False) As Boolean

      Dim bCollision As Boolean = False

      ' Check diagonals of R1 against edges of R2
      For p As Integer = 0 To vecPointsWorld.Count - 1
        Dim line_r1s As vec2d = New vec2d With {.x = pos.X, .y = pos.Y}
        Dim line_r1e As vec2d = New vec2d With {.x = vecPointsWorld(p).X, .y = vecPointsWorld(p).Y}

        Dim displacement As vec2d = New vec2d With {.x = 0, .y = 0}

        For q As Integer = 0 To r2.vecPointsWorld.Count - 1
          Dim line_r2s As vec2d = New vec2d With {.x = r2.vecPointsWorld(q).X, .y = r2.vecPointsWorld(q).Y}
          Dim line_r2e As vec2d = New vec2d With {.x = r2.vecPointsWorld((q + 1) Mod r2.vecPointsWorld.Count).X, .y = r2.vecPointsWorld((q + 1) Mod r2.vecPointsWorld.Count).Y}

          ' Standard "off the shelf" line segment intersection
          Dim h As Single = (line_r2e.x - line_r2s.x) * (line_r1s.y - line_r1e.y) - (line_r1s.x - line_r1e.x) * (line_r2e.y - line_r2s.y)
          Dim t1 As Single = ((line_r2s.y - line_r2e.y) * (line_r1s.x - line_r2s.x) + (line_r2e.x - line_r2s.x) * (line_r1s.y - line_r2s.y)) / h
          Dim t2 As Single = ((line_r1s.y - line_r1e.y) * (line_r1s.x - line_r2s.x) + (line_r1e.x - line_r1s.x) * (line_r1s.y - line_r2s.y)) / h

          If t1 >= 0.0F AndAlso t1 <= 1.0F AndAlso t2 >= 0.0F AndAlso t2 <= 1.0F Then
            If bResolveStatic Then
              displacement.x += (1.0F - t1) * (line_r1e.x - line_r1s.x)
              displacement.y += (1.0F - t1) * (line_r1e.y - line_r1s.y)
              bCollision = True
            Else
              Return True
            End If
          End If
        Next

        pos.X -= displacement.x
        pos.Y -= displacement.y
      Next

      ' Check diagonals of R2 against edges of R1
      For p = 0 To r2.vecPointsWorld.Count - 1
        Dim line_r1s As vec2d = New vec2d(r2.pos.X, r2.pos.Y)
        Dim line_r1e As vec2d = New vec2d(r2.vecPointsWorld(p).X, r2.vecPointsWorld(p).Y)

        Dim displacement As vec2d = New vec2d(0, 0)

        For q As Integer = 0 To vecPointsWorld.Count - 1
          Dim line_r2s As vec2d = New vec2d(vecPointsWorld(q).X, vecPointsWorld(q).Y)
          Dim line_r2e As vec2d = New vec2d(vecPointsWorld((q + 1) Mod vecPointsWorld.Count).X, vecPointsWorld((q + 1) Mod vecPointsWorld.Count).Y)

          ' Standard "off the shelf" line segment intersection
          Dim h As Single = (line_r2e.x - line_r2s.x) * (line_r1s.y - line_r1e.y) - (line_r1s.x - line_r1e.x) * (line_r2e.y - line_r2s.y)
          Dim t1 As Single = ((line_r2s.y - line_r2e.y) * (line_r1s.x - line_r2s.x) + (line_r2e.x - line_r2s.x) * (line_r1s.y - line_r2s.y)) / h
          Dim t2 As Single = ((line_r1s.y - line_r1e.y) * (line_r1s.x - line_r2s.x) + (line_r1e.x - line_r1s.x) * (line_r1s.y - line_r2s.y)) / h

          If t1 >= 0.0F AndAlso t1 <= 1.0F AndAlso t2 >= 0.0F AndAlso t2 <= 1.0F Then
            If bResolveStatic Then
              displacement.x += (1.0F - t1) * (line_r1e.x - line_r1s.x)
              displacement.y += (1.0F - t1) * (line_r1e.y - line_r1s.y)
              bCollision = True
            Else
              Return True
            End If
          End If
        Next q

        pos.X += displacement.x
        pos.Y += displacement.y
      Next p

      Return bCollision

    End Function

  End Class

End Class