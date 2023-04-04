Option Explicit On
Option Strict On
Option Infer On
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports Olc

Friend Module Program

  Sub Main() 'args As String())
    Dim demo As New ShadowCasting2D
    If demo.Construct(640, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class sEdge
  Public sx, sy As Integer 'Single ' Start coordinate
  Public ex, ey As Integer 'Single ' End coordinate
End Class

Friend Class sCell
  Public edge_id(3) As Integer
  Public edge_exist(3) As Boolean
  Public exist As Boolean
End Class

Friend Class ShadowCasting2D
  Inherits Olc.PixelGameEngine

  Private Const NORTH = 0
  Private Const SOUTH = 1
  Private Const EAST = 2
  Private Const WEST = 3

  Private world As sCell()
  Private nWorldWidth As Integer = 40
  Private nWorldHeight As Integer = 30

  Private sprLightCast As Olc.Sprite
  Private buffLightRay As Olc.Sprite
  Private buffLightTex As Olc.Sprite

  Private vecEdges As New List(Of sEdge)

  '			angle	x	y
  Private vecVisibilityPolygonPoints As New List(Of (Angle As Single, X As Integer, Y As Integer))

  Friend Sub New()
    sAppName = "ShadowCasting2D"
  End Sub

  Private Sub ConvertTileMapToPolyMap(sx As Integer, sy As Integer, w As Integer, h As Integer, blockWidth As Integer, pitch As Integer)

    ' Clear "PolyMap"
    vecEdges.Clear()

    For x = 0 To w - 1
      For y = 0 To h - 1
        For j = 0 To 3
          world((y + sy) * pitch + (x + sx)).edge_exist(j) = False
          world((y + sy) * pitch + (x + sx)).edge_id(j) = 0
        Next
      Next
    Next

    ' Iterate through region from top left to bottom right
    For x = 1 To w - 2
      For y = 1 To h - 2

        ' Create some convenient indices
        Dim i = (y + sy) * pitch + (x + sx) ' This
        Dim n = (y + sy - 1) * pitch + (x + sx) ' Northern Neighbour
        Dim s = (y + sy + 1) * pitch + (x + sx) ' Southern Neighbour
        Dim we = (y + sy) * pitch + (x + sx - 1) ' Western Neighbour
        Dim e = (y + sy) * pitch + (x + sx + 1) ' Eastern Neighbour

        ' If this cell exists, check if it needs edges
        If world(i).exist Then

          ' If this cell has no western neighbour, it needs a western edge
          If Not world(we).exist Then

            ' It can either extend it from its northern neighbour if they have
            ' one, or It can start a new one.
            If world(n).edge_exist(WEST) Then
              ' Northern neighbour has a western edge, so grow it downwards
              vecEdges(world(n).edge_id(WEST)).ey += blockWidth
              world(i).edge_id(WEST) = world(n).edge_id(WEST)
              world(i).edge_exist(WEST) = True
            Else

              ' Northern neighbour does not have one, so create one
              Dim edge As New sEdge
              edge.sx = (sx + x) * blockWidth : edge.sy = (sy + y) * blockWidth
              edge.ex = edge.sx : edge.ey = edge.sy + blockWidth

              ' Add edge to Polygon Pool
              Dim edge_id = vecEdges.Count
              vecEdges.Add(edge)

              ' Update tile information with edge information
              world(i).edge_id(WEST) = edge_id
              world(i).edge_exist(WEST) = True

            End If
          End If

          '' If this cell dont have an eastern neignbour, It needs a eastern edge
          'If Not world(e).exist Then
          '  ' It can either extend it from its northern neighbour if they have
          '  ' one, or It can start a new one.
          '  If world(n).edge_exist(EAST) Then
          '    ' Northern neighbour has one, so grow it downwards
          '    vecEdges(world(n).edge_id(EAST)).ey += blockWidth
          '    world(i).edge_id(EAST) = world(n).edge_id(EAST)
          '    world(i).edge_exist(EAST) = True
          '  Else

          '    ' Northern neighbour does not have one, so create one
          '    Dim edge As New sEdge
          '    edge.sx = (sx + x) * blockWidth : edge.sy = (sy + y) * blockWidth
          '    edge.ex = edge.sx : edge.ey = edge.sy + blockWidth

          '    ' Add edge to Polygon Pool
          '    Dim edge_id = vecEdges.Count
          '    vecEdges.Add(edge)

          '    ' Update tile information with edge information
          '    world(i).edge_id(WEST) = edge_id
          '    world(i).edge_exist(WEST) = True

          '  End If
          'End If

          ' If this cell dont have an eastern neignbour, It needs a eastern edge
          If Not world(e).exist Then
            ' It can either extend it from its northern neighbour if they have
            ' one, or It can start a new one.
            If world(n).edge_exist(EAST) Then
              ' Northern neighbour has one, so grow it downwards
              vecEdges(world(n).edge_id(EAST)).ey += blockWidth
              world(i).edge_id(EAST) = world(n).edge_id(EAST)
              world(i).edge_exist(EAST) = True
            Else

              ' Northern neighbour does not have one, so create one
              Dim edge As New sEdge
              edge.sx = (sx + x + 1) * blockWidth : edge.sy = (sy + y) * blockWidth
              edge.ex = edge.sx : edge.ey = edge.sy + blockWidth

              ' Add edge to Polygon Pool
              Dim edge_id = vecEdges.Count
              vecEdges.Add(edge)

              ' Update tile information with edge information
              world(i).edge_id(EAST) = edge_id
              world(i).edge_exist(EAST) = True

            End If
          End If

          ' If this cell doesnt have a northern neignbour, It needs a northern edge
          If Not world(n).exist Then
            ' It can either extend it from its western neighbour if they have
            ' one, or It can start a new one.
            If world(we).edge_exist(NORTH) Then
              ' Western neighbour has one, so grow it eastwards
              vecEdges(world(we).edge_id(NORTH)).ex += blockWidth
              world(i).edge_id(NORTH) = world(we).edge_id(NORTH)
              world(i).edge_exist(NORTH) = True
            Else

              ' Western neighbour does not have one, so create one
              Dim edge As New sEdge
              edge.sx = (sx + x) * blockWidth : edge.sy = (sy + y) * blockWidth
              edge.ex = edge.sx + blockWidth : edge.ey = edge.sy

              ' Add edge to Polygon Pool
              Dim edge_id = vecEdges.Count
              vecEdges.Add(edge)

              ' Update tile information with edge information
              world(i).edge_id(NORTH) = edge_id
              world(i).edge_exist(NORTH) = True

            End If
          End If

          ' If this cell doesnt have a southern neignbour, It needs a southern edge
          If Not world(s).exist Then
            ' It can either extend it from its western neighbour if they have
            ' one, or It can start a new one.
            If world(we).edge_exist(SOUTH) Then
              ' Western neighbour has one, so grow it eastwards
              vecEdges(world(we).edge_id(SOUTH)).ex += blockWidth
              world(i).edge_id(SOUTH) = world(we).edge_id(SOUTH)
              world(i).edge_exist(SOUTH) = True
            Else

              ' Western neighbour does not have one, so I need to create one
              Dim edge As New sEdge
              edge.sx = (sx + x) * blockWidth : edge.sy = (sy + y + 1) * blockWidth
              edge.ex = edge.sx + blockWidth : edge.ey = edge.sy

              ' Add edge to Polygon Pool
              Dim edge_id = vecEdges.Count
              vecEdges.Add(edge)

              ' Update tile information with edge information
              world(i).edge_id(SOUTH) = edge_id
              world(i).edge_exist(SOUTH) = True

            End If
          End If

        End If

      Next
    Next

  End Sub

  Private Sub CalculateVisibilityPolygon(ox As Single, oy As Single, radius As Single)

    ' Get rid of existing polygon
    vecVisibilityPolygonPoints.Clear()

    ' For each edge in PolyMap
    For Each e1 In vecEdges

      ' Take the start point, then the end point (we could use a pool of
      ' non-duplicated points here, it would be more optimal)
      For i = 0 To 1

        Dim rdx = If(i = 0, e1.sx, e1.ex) - ox
        Dim rdy = If(i = 0, e1.sy, e1.ey) - oy

        Dim base_ang = CSng(Math.Atan2(rdy, rdx))

        Dim ang = 0.0F

        ' For each point, cast 3 rays, 1 directly at point
        ' and 1 a little bit either side
        For j = 0 To 2

          If j = 0 Then ang = base_ang - 0.0001F
          If j = 1 Then ang = base_ang
          If j = 2 Then ang = base_ang + 0.0001F

          ' Create ray along angle for required distance
          rdx = radius * CSng(Math.Cos(ang))
          rdy = radius * CSng(Math.Sin(ang))

          Dim min_t1 = Single.PositiveInfinity
          Dim min_px = 0.0F, min_py = 0.0F, min_ang = 0.0F
          Dim bValid = False

          ' Check for ray intersection with all edges
          For Each e2 In vecEdges

            ' Create line segment vector
            Dim sdx = e2.ex - e2.sx
            Dim sdy = e2.ey - e2.sy

            If Math.Abs(sdx - rdx) > 0.0F AndAlso Math.Abs(sdy - rdy) > 0.0F Then

              ' t2 is normalised distance from line segment start to line segment end of intersect point
              Dim t2 = (rdx * (e2.sy - oy) + (rdy * (ox - e2.sx))) / (sdx * rdy - sdy * rdx)
              ' t1 is normalised distance from source along ray to ray length of intersect point
              Dim t1 = (e2.sx + sdx * t2 - ox) / rdx

              ' If intersect point exists along ray, and along line 
              ' segment then intersect point is valid
              If t1 > 0 AndAlso t2 >= 0 AndAlso t2 <= 1.0F Then
                ' Check if this intersect point is closest to source. If
                ' it is, then store this point and reject others
                If t1 < min_t1 Then
                  min_t1 = t1
                  min_px = ox + rdx * t1
                  min_py = oy + rdy * t1
                  min_ang = CSng(Math.Atan2(min_py - oy, min_px - ox))
                  bValid = True
                End If
              End If

            End If
          Next

          If bValid Then ' Add intersection point to visibility polygon perimeter
            vecVisibilityPolygonPoints.Add((min_ang, CInt(Fix(min_px)), CInt(Fix(min_py))))
          End If

        Next
      Next
    Next

    ' Sort perimeter points by angle from source. This will allow
    ' us to draw a triangle fan.
    vecVisibilityPolygonPoints.Sort(Function(t1 As (Angle As Single, X As Single, Y As Single), t2 As (Angle As Single, X As Single, Y As Single)) As Integer
                                      If t1.Angle < t2.Angle Then
                                        Return 1
                                      ElseIf t1.Angle > t2.Angle Then
                                        Return -1
                                      Else
                                        Return 0
                                      End If
                                    End Function)

  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    ReDim world(nWorldWidth * nWorldHeight - 1) : For index = 0 To world.Length - 1 : world(index) = New sCell : Next

    ' Add a boundary to the world
    For x = 1 To nWorldWidth - 2
      world(1 * nWorldWidth + x).exist = True
      world((nWorldHeight - 2) * nWorldWidth + x).exist = True
    Next

    For y = 1 To nWorldHeight - 2
      world(y * nWorldWidth + 1).exist = True
      world(y * nWorldWidth + (nWorldWidth - 2)).exist = True
    Next

    sprLightCast = New Olc.Sprite("light_cast.png")

    ' Create some screen-sized off-screen buffers for lighting effect
    buffLightTex = New Olc.Sprite(ScreenWidth, ScreenHeight)
    buffLightRay = New Olc.Sprite(ScreenWidth, ScreenHeight)

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim fBlockWidth = 16
    Dim fSourceX = GetMouseX()
    Dim fSourceY = GetMouseY()

    ' Set tile map blocks to on or off
    If GetMouse(0).bReleased Then
      ' i = y * width + x
      Dim i = (fSourceY \ fBlockWidth) * nWorldWidth + (fSourceX \ fBlockWidth)
      world(i).exist = Not world(i).exist
    End If

    ' Take a region of "TileMap" and convert it to "PolyMap" - This is done
    ' every frame here, but could be a pre-processing stage depending on
    ' how your final application interacts with tilemaps
    ConvertTileMapToPolyMap(0, 0, 40, 30, fBlockWidth, nWorldWidth)

    If GetMouse(1).bHeld Then
      CalculateVisibilityPolygon(fSourceX, fSourceY, 1000.0F)
    End If

    ' Drawing
    SetDrawTarget(Nothing)
    Clear(Presets.Black)

    Dim nRaysCast = vecVisibilityPolygonPoints.Count

    ' Remove duplicate (or simply similar) points from polygon
    vecVisibilityPolygonPoints = vecVisibilityPolygonPoints _
                                 .GroupBy(Function(t) New Tuple(Of Single, Single)(t.X, t.Y)) _
                                 .Select(Function(g) g.First()) _
                                 .ToList()

    Dim nRaysCast2 As Integer = vecVisibilityPolygonPoints.Count
    DrawString(4, 4, "Rays Cast: " & nRaysCast.ToString() & " Rays Drawn: " & nRaysCast2.ToString())

    ' If drawing rays, set an offscreen texture as our target buffer
    If (GetMouse(1).bHeld AndAlso vecVisibilityPolygonPoints.Count > 1) Then

      ' Clear offscreen buffer for sprite
      SetDrawTarget(buffLightTex)
      Clear(Presets.Black)

      ' Draw "Radial Light" sprite to offscreen buffer, centered around 
      ' source location (the mouse coordinates, buffer is 512x512)
      DrawSprite(fSourceX - 255, fSourceY - 255, sprLightCast)

      ' Clear offsecreen buffer for rays
      SetDrawTarget(buffLightRay)
      Clear(Presets.Black)

      ' Draw each triangle in fan
      For i = 0 To vecVisibilityPolygonPoints.Count - 2
        FillTriangle(fSourceX,
                     fSourceY,
                     vecVisibilityPolygonPoints(i).X,
                     vecVisibilityPolygonPoints(i).Y,
                     vecVisibilityPolygonPoints(i + 1).X,
                     vecVisibilityPolygonPoints(i + 1).Y)
      Next

      ' Fan will have one open edge, so draw last point of fan to first
      FillTriangle(fSourceX,
                   fSourceY,
                   vecVisibilityPolygonPoints(vecVisibilityPolygonPoints.Count - 1).X,
                   vecVisibilityPolygonPoints(vecVisibilityPolygonPoints.Count - 1).Y,
                   vecVisibilityPolygonPoints(0).X,
                   vecVisibilityPolygonPoints(0).Y)

      ' Wherever rays exist in ray sprite, copy over radial light sprite pixels
      SetDrawTarget(Nothing)
      For x = 0 To ScreenWidth() - 1
        For y = 0 To ScreenHeight() - 1
          If buffLightRay.GetPixel(x, y).R > 0 Then
            Draw(x, y, buffLightTex.GetPixel(x, y))
          End If
        Next
      Next

    End If

    ' Draw Blocks from TileMap
    For x = 0 To nWorldWidth - 1
      For y = 0 To nWorldHeight - 1
        If world(y * nWorldWidth + x).exist Then
          FillRect(x * fBlockWidth, y * fBlockWidth, fBlockWidth, fBlockWidth, Presets.Blue)
        End If
      Next
    Next

    ' Draw Edges from PolyMap
    'For Each e In vecEdges
    '  DrawLine(e.sx, e.sy, e.ex, e.ey)
    '  FillCircle(e.sx, e.sy, 3, Presets.Red)
    '  FillCircle(e.ex, e.ey, 3, Presets.Red)
    'Next

    Return True

  End Function

  '' Define ToleranceComparer class to handle floating point precision differences
  'Public Class ToleranceComparer
  '  Implements IEqualityComparer(Of (Single, Integer, Integer))
  '  Private ReadOnly tolerance As Single

  '  Public Sub New(tolerance As Single)
  '    Me.tolerance = tolerance
  '  End Sub

  '  Public Function Equals(x As (Single, Integer, Integer), y As (Single, Integer, Integer)) As Boolean Implements IEqualityComparer(Of (Single, Integer, Integer)).Equals
  '    Return Math.Abs(x.Item2 - y.Item2) < tolerance AndAlso Math.Abs(x.Item3 - y.Item3) < tolerance
  '  End Function

  '  Public Function GetHashCode(obj As (Single, Integer, Integer)) As Integer Implements IEqualityComparer(Of (Single, Integer, Integer)).GetHashCode
  '    Return obj.GetHashCode()
  '  End Function
  'End Class

End Class
