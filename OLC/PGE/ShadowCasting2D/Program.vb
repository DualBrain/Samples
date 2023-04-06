' Inspired by "Line Of Sight or Shadow Casting in 2D" -- @javidx9
' https://youtu.be/fc3nnG2CG8U

'https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
'https://www.redblobgames.com/articles/visibility/
'https://ncase.me/sight-and-light/

Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Friend Module Program

  Sub Main()
    Dim demo As New ShadowCasting2D
    If demo.Construct(640, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class Edge
  Public Sx, Sy As Integer 'Single ' Start coordinate
  Public Ex, Ey As Integer 'Single ' End coordinate
End Class

Friend Class Cell
  Public EdgeId(3) As Integer
  Public EdgeExist(3) As Boolean
  Public Exist As Boolean
End Class

Friend Class ShadowCasting2D
  Inherits PixelGameEngine

  Private Const NORTH = 0
  Private Const SOUTH = 1
  Private Const EAST = 2
  Private Const WEST = 3

  Private m_world As Cell()
  Private ReadOnly m_worldWidth As Integer = 40
  Private ReadOnly m_worldHeight As Integer = 30

  Private m_sprLightCast As Sprite
  Private m_buffLightRay As Sprite
  Private m_buffLightTex As Sprite

  Private ReadOnly m_vecEdges As New List(Of Edge)

  Private m_vecVisibilityPolygonPoints As New List(Of (Angle As Single, X As Integer, Y As Integer))

  Friend Sub New()
    AppName = "ShadowCasting2D"
  End Sub

  Private Sub ConvertTileMapToPolyMap(sx As Integer, sy As Integer, w As Integer, h As Integer, blockWidth As Integer, pitch As Integer)

    ' Clear "PolyMap"
    m_vecEdges.Clear()

    For x = 0 To w - 1
      For y = 0 To h - 1
        For j = 0 To 3
          m_world((y + sy) * pitch + (x + sx)).EdgeExist(j) = False
          m_world((y + sy) * pitch + (x + sx)).EdgeId(j) = 0
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
        If m_world(i).Exist Then

          ' If this cell has no western neighbour, it needs a western edge
          If Not m_world(we).Exist Then

            ' It can either extend it from its northern neighbour if they have
            ' one, or It can start a new one.
            If m_world(n).EdgeExist(WEST) Then
              ' Northern neighbour has a western edge, so grow it downwards
              m_vecEdges(m_world(n).EdgeId(WEST)).Ey += blockWidth
              m_world(i).EdgeId(WEST) = m_world(n).EdgeId(WEST)
              m_world(i).EdgeExist(WEST) = True
            Else

              ' Northern neighbour does not have one, so create one
              Dim edge As New Edge
              edge.Sx = (sx + x) * blockWidth : edge.Sy = (sy + y) * blockWidth
              edge.Ex = edge.Sx : edge.Ey = edge.Sy + blockWidth

              ' Add edge to Polygon Pool
              Dim edgeId = m_vecEdges.Count
              m_vecEdges.Add(edge)

              ' Update tile information with edge information
              m_world(i).EdgeId(WEST) = edgeId
              m_world(i).EdgeExist(WEST) = True

            End If
          End If

          ' If this cell dont have an eastern neignbour, It needs a eastern edge
          If Not m_world(e).Exist Then
            ' It can either extend it from its northern neighbour if they have
            ' one, or It can start a new one.
            If m_world(n).EdgeExist(EAST) Then
              ' Northern neighbour has one, so grow it downwards
              m_vecEdges(m_world(n).EdgeId(EAST)).Ey += blockWidth
              m_world(i).EdgeId(EAST) = m_world(n).EdgeId(EAST)
              m_world(i).EdgeExist(EAST) = True
            Else

              ' Northern neighbour does not have one, so create one
              Dim edge As New Edge
              edge.Sx = (sx + x + 1) * blockWidth : edge.Sy = (sy + y) * blockWidth
              edge.Ex = edge.Sx : edge.Ey = edge.Sy + blockWidth

              ' Add edge to Polygon Pool
              Dim edgeId = m_vecEdges.Count
              m_vecEdges.Add(edge)

              ' Update tile information with edge information
              m_world(i).EdgeId(EAST) = edgeId
              m_world(i).EdgeExist(EAST) = True

            End If
          End If

          ' If this cell doesnt have a northern neignbour, It needs a northern edge
          If Not m_world(n).Exist Then
            ' It can either extend it from its western neighbour if they have
            ' one, or It can start a new one.
            If m_world(we).EdgeExist(NORTH) Then
              ' Western neighbour has one, so grow it eastwards
              m_vecEdges(m_world(we).EdgeId(NORTH)).Ex += blockWidth
              m_world(i).EdgeId(NORTH) = m_world(we).EdgeId(NORTH)
              m_world(i).EdgeExist(NORTH) = True
            Else

              ' Western neighbour does not have one, so create one
              Dim edge As New Edge
              edge.Sx = (sx + x) * blockWidth : edge.Sy = (sy + y) * blockWidth
              edge.Ex = edge.Sx + blockWidth : edge.Ey = edge.Sy

              ' Add edge to Polygon Pool
              Dim edgeId = m_vecEdges.Count
              m_vecEdges.Add(edge)

              ' Update tile information with edge information
              m_world(i).EdgeId(NORTH) = edgeId
              m_world(i).EdgeExist(NORTH) = True

            End If
          End If

          ' If this cell doesnt have a southern neignbour, It needs a southern edge
          If Not m_world(s).Exist Then
            ' It can either extend it from its western neighbour if they have
            ' one, or It can start a new one.
            If m_world(we).EdgeExist(SOUTH) Then
              ' Western neighbour has one, so grow it eastwards
              m_vecEdges(m_world(we).EdgeId(SOUTH)).Ex += blockWidth
              m_world(i).EdgeId(SOUTH) = m_world(we).EdgeId(SOUTH)
              m_world(i).EdgeExist(SOUTH) = True
            Else

              ' Western neighbour does not have one, so I need to create one
              Dim edge As New Edge
              edge.Sx = (sx + x) * blockWidth : edge.Sy = (sy + y + 1) * blockWidth
              edge.Ex = edge.Sx + blockWidth : edge.Ey = edge.Sy

              ' Add edge to Polygon Pool
              Dim edgeId = m_vecEdges.Count
              m_vecEdges.Add(edge)

              ' Update tile information with edge information
              m_world(i).EdgeId(SOUTH) = edgeId
              m_world(i).EdgeExist(SOUTH) = True

            End If
          End If

        End If

      Next
    Next

  End Sub

  Private Sub CalculateVisibilityPolygon(ox As Single, oy As Single, radius As Single)

    ' Get rid of existing polygon
    m_vecVisibilityPolygonPoints.Clear()

    ' For each edge in PolyMap
    For Each e1 In m_vecEdges

      ' Take the start point, then the end point (we could use a pool of
      ' non-duplicated points here, it would be more optimal)
      For i = 0 To 1

        Dim rdx = If(i = 0, e1.Sx, e1.Ex) - ox
        Dim rdy = If(i = 0, e1.Sy, e1.Ey) - oy

        Dim base_ang = MathF.Atan2(rdy, rdx)

        Dim ang = 0.0F

        ' For each point, cast 3 rays, 1 directly at point
        ' and 1 a little bit either side
        For j = 0 To 2

          If j = 0 Then ang = base_ang - 0.0001F
          If j = 1 Then ang = base_ang
          If j = 2 Then ang = base_ang + 0.0001F

          ' Create ray along angle for required distance
          rdx = radius * MathF.Cos(ang)
          rdy = radius * MathF.Sin(ang)

          Dim minT1 = Single.PositiveInfinity
          Dim minPx = 0.0F, minPy = 0.0F, minAng = 0.0F
          Dim valid = False

          ' Check for ray intersection with all edges
          For Each e2 In m_vecEdges

            ' Create line segment vector
            Dim sdx = e2.Ex - e2.Sx
            Dim sdy = e2.Ey - e2.Sy

            If Math.Abs(sdx - rdx) > 0.0F AndAlso Math.Abs(sdy - rdy) > 0.0F Then

              ' t2 is normalised distance from line segment start to line segment end of intersect point
              Dim t2 = (rdx * (e2.Sy - oy) + (rdy * (ox - e2.Sx))) / (sdx * rdy - sdy * rdx)
              ' t1 is normalised distance from source along ray to ray length of intersect point
              Dim t1 = (e2.Sx + sdx * t2 - ox) / rdx

              ' If intersect point exists along ray, and along line 
              ' segment then intersect point is valid
              If t1 > 0 AndAlso t2 >= 0 AndAlso t2 <= 1.0F Then
                ' Check if this intersect point is closest to source. If
                ' it is, then store this point and reject others
                If t1 < minT1 Then
                  minT1 = t1
                  minPx = ox + rdx * t1
                  minPy = oy + rdy * t1
                  minAng = MathF.Atan2(minPy - oy, minPx - ox)
                  valid = True
                End If
              End If

            End If
          Next

          If valid Then ' Add intersection point to visibility polygon perimeter
            m_vecVisibilityPolygonPoints.Add((minAng, CInt(Fix(minPx)), CInt(Fix(minPy))))
          End If

        Next
      Next
    Next

    ' Sort perimeter points by angle from source. This will allow
    ' us to draw a triangle fan.
    m_vecVisibilityPolygonPoints.Sort(Function(t1 As (Angle As Single, X As Single, Y As Single), t2 As (Angle As Single, X As Single, Y As Single)) As Integer
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

    ReDim m_world(m_worldWidth * m_worldHeight - 1) : For index = 0 To m_world.Length - 1 : m_world(index) = New Cell : Next

    ' Add a boundary to the world
    For x = 1 To m_worldWidth - 2
      m_world(1 * m_worldWidth + x).Exist = True
      m_world((m_worldHeight - 2) * m_worldWidth + x).Exist = True
    Next

    For y = 1 To m_worldHeight - 2
      m_world(y * m_worldWidth + 1).Exist = True
      m_world(y * m_worldWidth + (m_worldWidth - 2)).Exist = True
    Next

    m_sprLightCast = New Olc.Sprite("light_cast.png")

    ' Create some screen-sized off-screen buffers for lighting effect
    m_buffLightTex = New Olc.Sprite(ScreenWidth, ScreenHeight)
    m_buffLightRay = New Olc.Sprite(ScreenWidth, ScreenHeight)

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim blockWidth = 16
    Dim sourceX = GetMouseX()
    Dim sourceY = GetMouseY()

    ' Set tile map blocks to on or off
    If GetMouse(0).Released Then
      ' i = y * width + x
      Dim i = (sourceY \ blockWidth) * m_worldWidth + (sourceX \ blockWidth)
      m_world(i).Exist = Not m_world(i).Exist
    End If

    ' Take a region of "TileMap" and convert it to "PolyMap" - This is done
    ' every frame here, but could be a pre-processing stage depending on
    ' how your final application interacts with tilemaps
    ConvertTileMapToPolyMap(0, 0, 40, 30, blockWidth, m_worldWidth)

    If GetMouse(1).Held Then
      CalculateVisibilityPolygon(sourceX, sourceY, 1000.0F)
    End If

    ' Drawing
    SetDrawTarget(Nothing)
    Clear(Presets.Black)

    Dim raysCast = m_vecVisibilityPolygonPoints.Count

    ' Remove duplicate (or simply similar) points from polygon
    m_vecVisibilityPolygonPoints = m_vecVisibilityPolygonPoints _
                                 .GroupBy(Function(t) New Tuple(Of Single, Single)(t.X, t.Y)) _
                                 .Select(Function(g) g.First()) _
                                 .ToList()

    Dim raysCast2 As Integer = m_vecVisibilityPolygonPoints.Count
    DrawString(4, 4, "Rays Cast: " & raysCast.ToString() & " Rays Drawn: " & raysCast2.ToString())

    ' If drawing rays, set an offscreen texture as our target buffer
    If (GetMouse(1).Held AndAlso m_vecVisibilityPolygonPoints.Count > 1) Then

      ' Clear offscreen buffer for sprite
      SetDrawTarget(m_buffLightTex)
      Clear(Presets.Black)

      ' Draw "Radial Light" sprite to offscreen buffer, centered around 
      ' source location (the mouse coordinates, buffer is 512x512)
      DrawSprite(sourceX - 255, sourceY - 255, m_sprLightCast)

      ' Clear offsecreen buffer for rays
      SetDrawTarget(m_buffLightRay)
      Clear(Presets.Black)

      ' Draw each triangle in fan
      For i = 0 To m_vecVisibilityPolygonPoints.Count - 2
        FillTriangle(sourceX,
                     sourceY,
                     m_vecVisibilityPolygonPoints(i).X,
                     m_vecVisibilityPolygonPoints(i).Y,
                     m_vecVisibilityPolygonPoints(i + 1).X,
                     m_vecVisibilityPolygonPoints(i + 1).Y)
      Next

      ' Fan will have one open edge, so draw last point of fan to first
      FillTriangle(sourceX,
                   sourceY,
                   m_vecVisibilityPolygonPoints(m_vecVisibilityPolygonPoints.Count - 1).X,
                   m_vecVisibilityPolygonPoints(m_vecVisibilityPolygonPoints.Count - 1).Y,
                   m_vecVisibilityPolygonPoints(0).X,
                   m_vecVisibilityPolygonPoints(0).Y)

      ' Wherever rays exist in ray sprite, copy over radial light sprite pixels
      SetDrawTarget(Nothing)
      For x = 0 To ScreenWidth() - 1
        For y = 0 To ScreenHeight() - 1
          If m_buffLightRay.GetPixel(x, y).R > 0 Then
            Draw(x, y, m_buffLightTex.GetPixel(x, y))
          End If
        Next
      Next

    End If

    ' Draw Blocks from TileMap
    For x = 0 To m_worldWidth - 1
      For y = 0 To m_worldHeight - 1
        If m_world(y * m_worldWidth + x).Exist Then
          FillRect(x * blockWidth, y * blockWidth, blockWidth, blockWidth, Presets.Blue)
        End If
      Next
    Next

    ' Draw Edges from PolyMap
    'For Each e In m_vecEdges
    '  DrawLine(e.Sx, e.Sy, e.Ex, e.Ey)
    '  FillCircle(e.Sx, e.Sy, 3, Presets.Red)
    '  FillCircle(e.Ex, e.Ey, 3, Presets.Red)
    'Next

    Return True

  End Function

End Class