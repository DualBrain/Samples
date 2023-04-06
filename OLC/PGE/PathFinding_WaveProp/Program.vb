' Inpsired by "Path Planning #2 Wave Propagation, Potential Fields & Modern(ish) C++" -- @javidx9
' https://youtu.be/0ihciMKlcP8

Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Module Program
  Sub Main()
    Dim demo = New PathFinding_FlowFields
    If demo.Construct(512, 480, 2, 2) Then
      demo.Start()
    End If
  End Sub
End Module

Class PathFinding_FlowFields
  Inherits PixelGameEngine

  Private m_mapWidth As Integer
  Private m_mapHeight As Integer
  Private m_cellSize As Integer
  Private m_borderWidth As Integer

  Private m_obstacleMap() As Boolean

  Private m_flowFieldZ() As Integer
  Private m_flowFieldY() As Single
  Private m_flowFieldX() As Single

  Private m_startX As Integer
  Private m_startY As Integer
  Private m_endX As Integer
  Private m_endY As Integer

  Private m_wave As Integer = 1

  Sub New()
    AppName = "PathFinding - Flow Fields"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    m_borderWidth = 4
    m_cellSize = 32
    m_mapWidth = ScreenWidth \ m_cellSize
    m_mapHeight = ScreenHeight \ m_cellSize
    ReDim m_obstacleMap((m_mapWidth * m_mapHeight) - 1)
    ReDim m_flowFieldZ((m_mapWidth * m_mapHeight) - 1)
    ReDim m_flowFieldX((m_mapWidth * m_mapHeight) - 1)
    ReDim m_flowFieldY((m_mapWidth * m_mapHeight) - 1)

    m_startX = 3
    m_startY = 7
    m_endX = 12
    m_endY = 7

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Little convenience lambda 2D -> 3D
    Dim p = Function(x As Integer, y As Integer) As Integer
              Return y * m_mapWidth + x
            End Function

    ' User Input
    Dim selectedCellX = GetMouseX \ m_cellSize
    Dim selectedCellY = GetMouseY \ m_cellSize

    If GetMouse(0).Released Then
      ' Toggle Obstacle if mouse left clicked
      m_obstacleMap(p(selectedCellX, selectedCellY)) = Not m_obstacleMap(p(selectedCellX, selectedCellY))
    End If

    If GetMouse(1).Released Then
      m_startX = selectedCellX
      m_startY = selectedCellY
    End If

    If GetKey(Key.Q).Released Then
      m_wave += 1
    End If

    If GetKey(Key.A).Released Then
      m_wave -= 1 : If m_wave = 0 Then m_wave = 1
    End If

    ' 1) Prepare flow field, add a boundary, and add obstacles
    '    by setting the flow Field Height (Z) to -1
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        ' Set border or obstacle
        If x = 0 OrElse y = 0 OrElse x = (m_mapWidth - 1) OrElse y = (m_mapHeight - 1) OrElse m_obstacleMap(p(x, y)) Then
          m_flowFieldZ(p(x, y)) = -1
        Else
          m_flowFieldZ(p(x, y)) = 0
        End If
      Next
    Next

    ' 2) Propagate a wave (i.e. flood-fill) from target location. Here I use
    '    a tuple, of {x, y, distance} - though you could use a struct or similar.
    Dim nodes = New List(Of (X As Integer, Y As Integer, Distance As Integer))

    ' Add the first discovered node - the target location, with a distance of 1
    nodes.Add((m_endX, m_endY, 1))

    While nodes.Any

      ' Each iteration through the discovered nodes may create newly discovered
      ' nodes, so I maintain a second list. It's important not to contaminate
      ' the list being iterated through.
      Dim newNodes = New List(Of (X As Integer, Y As Integer, Distance As Integer))

      ' Now iterate through each discovered node. If it has neighbouring nodes
      ' that are empty space and undiscovered, add those locations to the
      ' new nodes list
      For Each n In nodes

        Dim x = n.X ' Map X-Coordinate
        Dim y = n.Y ' Map Y-Coordinate
        Dim d = n.Distance ' Distance From Target Location

        '	Set distance count for this node. NOte that when we add nodes we add 1
        ' to this distance. This emulates propagating a wave across the map, where
        ' the front of that wave increments each iteration. In this way, we can 
        ' propagate distance information 'away from target location'

        m_flowFieldZ(p(x, y)) = d

        '	Add neigbour nodes if unmarked, i.e their "height" is 0. Any discovered
        ' node or obstacle will be non-zero

        ' Check East
        If x + 1 < m_mapWidth AndAlso m_flowFieldZ(p(x + 1, y)) = 0 Then newNodes.Add((x + 1, y, d + 1))
        ' Check West
        If x - 1 > 0 AndAlso m_flowFieldZ(p(x - 1, y)) = 0 Then newNodes.Add((x - 1, y, d + 1))
        ' Check South
        If y + 1 < m_mapHeight AndAlso m_flowFieldZ(p(x, y + 1)) = 0 Then newNodes.Add((x, y + 1, d + 1))
        ' Check North
        If y - 1 > 0 AndAlso m_flowFieldZ(p(x, y - 1)) = 0 Then newNodes.Add((x, y - 1, d + 1))

      Next

      ' We will now have potentially multiple nodes for a single location. This means our
      ' algorithm will never complete! So we must remove duplicates form our new node list.
      ' Im doing this with some clever code - but it Is Not performant(!) - it is merely
      ' convenient. I'd suggest doing away with overhead structures like linked lists and sorts
      ' if you are aiming for fastest path finding.

      ' Sort the nodes - This will stack up nodes that are similar: A, B, B, B, B, C, D, D, E, F, F
      newNodes.Sort(Function(t1 As (X As Integer, Y As Integer, Depth As Integer), t2 As (X As Integer, Y As Integer, Depth As Integer)) As Integer
                      ' In this instance I dont care how the values are sorted, so long as nodes that
                      ' represent the same location are adjacent in the list. I can use the p() lambda
                      ' to generate a unique 1D value for a 2D coordinate, so I'll sort by that.
                      If p(t1.X, t1.Y) < p(t2.X, t2.Y) Then
                        Return -1
                      ElseIf p(t1.X, t1.Y) > p(t2.X, t2.Y) Then
                        Return 1
                      Else
                        Return 0
                      End If
                    End Function)

      ' Use "unique" function to remove adjacent duplicates       : A, B, -, -, -, C, D, -, E, F -
      ' and also erase them                                       : A, B, C, D, E, F
      newNodes = newNodes.Distinct.ToList

      ' We've now processed all the discoverd nodes, so clear the list, and add the newly
      ' discovered nodes for processing on the next iteration
      nodes.Clear()
      nodes.AddRange(newNodes)

      ' When there are no more newly discovered nodes, we have "flood filled" the entire
      ' map. The propagation phase of the algorithm Is complete

    End While

    ' 3) Create Path. Starting a start location, create a path of nodes until you reach target
    '    location. At each node find the neighbour with the lowest "distance" score.
    Dim path As New List(Of (X As Integer, Y As Integer))
    path.Add((m_startX, m_startY))
    Dim locX = m_startX
    Dim locY = m_startY
    Dim noPath = False

    While Not (locX = m_endX AndAlso locY = m_endY) AndAlso Not noPath

      Dim neighbours As New List(Of (X As Integer, Y As Integer, Distance As Integer))

      ' 4-Way Connectivity
      If (locY - 1) >= 0 AndAlso m_flowFieldZ(p(locX, locY - 1)) > 0 Then
        neighbours.Add((locX, locY - 1, m_flowFieldZ(p(locX, locY - 1))))
      End If

      If (locX + 1) < m_mapWidth AndAlso m_flowFieldZ(p(locX + 1, locY)) > 0 Then
        neighbours.Add((locX + 1, locY, m_flowFieldZ(p(locX + 1, locY))))
      End If

      If (locY + 1) < m_mapHeight AndAlso m_flowFieldZ(p(locX, locY + 1)) > 0 Then
        neighbours.Add((locX, locY + 1, m_flowFieldZ(p(locX, locY + 1))))
      End If

      If (locX - 1) >= 0 AndAlso m_flowFieldZ(p(locX - 1, locY)) > 0 Then
        neighbours.Add((locX - 1, locY, m_flowFieldZ(p(locX - 1, locY))))
      End If

      ' 8-Way Connectivity
      If (locY - 1) >= 0 AndAlso (locX - 1) >= 0 AndAlso m_flowFieldZ(p(locX - 1, locY - 1)) > 0 Then
        neighbours.Add((locX - 1, locY - 1, m_flowFieldZ(p(locX - 1, locY - 1))))
      End If

      If (locY - 1) >= 0 AndAlso (locX + 1) < m_mapWidth AndAlso m_flowFieldZ(p(locX + 1, locY - 1)) > 0 Then
        neighbours.Add((locX + 1, locY - 1, m_flowFieldZ(p(locX + 1, locY - 1))))
      End If

      If (locY + 1) < m_mapHeight AndAlso (locX - 1) >= 0 AndAlso m_flowFieldZ(p(locX - 1, locY + 1)) > 0 Then
        neighbours.Add((locX - 1, locY + 1, m_flowFieldZ(p(locX - 1, locY + 1))))
      End If

      If (locY + 1) < m_mapHeight AndAlso (locX + 1) < m_mapWidth AndAlso m_flowFieldZ(p(locX + 1, locY + 1)) > 0 Then
        neighbours.Add((locX + 1, locY + 1, m_flowFieldZ(p(locX + 1, locY + 1))))
      End If

      ' Sort neigbours based on height, so lowest neighbour is at front of list
      'listNeighbours.Sort(Function(n1, n2) CInt(Math.Sign(CInt(n1.Item3.CompareTo(n2.Item3)))))
      neighbours.Sort(Function(t1 As (X As Integer, Y As Integer, Depth As Integer), t2 As (X As Integer, Y As Integer, Depth As Integer)) As Integer
                        If t1.Depth < t2.Depth Then
                          Return -1
                        ElseIf t1.Depth > t2.Depth Then
                          Return 1
                        Else
                          Return 0
                        End If
                      End Function)

      If neighbours.Count = 0 Then ' Neighbour is invalid or no possible path
        noPath = True
      Else
        locX = neighbours(0).X
        locY = neighbours(0).Y
        path.Add((locX, locY))
      End If

    End While

    ' 4) Create Flow "Field"
    For x = 1 To m_mapWidth - 2
      For y = 1 To m_mapHeight - 2

        Dim vx = 0.0F
        Dim vy = 0.0F

        vy -= If(m_flowFieldZ(p(x, y + 1)) <= 0, m_flowFieldZ(p(x, y)), m_flowFieldZ(p(x, y + 1))) - m_flowFieldZ(p(x, y))
        vx -= If(m_flowFieldZ(p(x + 1, y)) <= 0, m_flowFieldZ(p(x, y)), m_flowFieldZ(p(x + 1, y))) - m_flowFieldZ(p(x, y))
        vy += If(m_flowFieldZ(p(x, y - 1)) <= 0, m_flowFieldZ(p(x, y)), m_flowFieldZ(p(x, y - 1))) - m_flowFieldZ(p(x, y))
        vx += If(m_flowFieldZ(p(x - 1, y)) <= 0, m_flowFieldZ(p(x, y)), m_flowFieldZ(p(x - 1, y))) - m_flowFieldZ(p(x, y))

        ' Had to extend this a bit to protect against NaN and Infinity due to 0.
        Dim sq = If(vx = 0 AndAlso vy = 0, 0, Math.Sqrt(vx * vx + vy * vy))
        Dim r = If(sq = 0, 0, CSng(1.0F / sq))

        m_flowFieldX(p(x, y)) = vx * r
        m_flowFieldY(p(x, y)) = vy * r

      Next
    Next

    ' Draw Map
    Clear()

    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1

        Dim colour = Presets.Blue

        If m_obstacleMap(p(x, y)) Then colour = Presets.Grey
        If m_wave = m_flowFieldZ(p(x, y)) Then colour = Presets.DarkCyan
        If x = m_startX AndAlso y = m_startY Then colour = Presets.Green
        If x = m_endX AndAlso y = m_endY Then colour = Presets.Red

        ' Draw Base
        FillRect(x * m_cellSize, y * m_cellSize, m_cellSize - m_borderWidth, m_cellSize - m_borderWidth, colour)

        ' Draw "potential" or "distance" or "height" :D
        'DrawString(x * m_cellSize, y * m_cellSize, m_flowFieldZ(p(x, y)).ToString, Presets.White)

        If m_flowFieldZ(p(x, y)) > 0 Then

          Dim ax(3), ay(3) As Single
          Dim angle = MathF.Atan2(m_flowFieldY(p(x, y)), m_flowFieldX(p(x, y)))
          Dim radius = (m_cellSize - m_borderWidth) / 2.0F
          Dim offsetX = CSng(x * m_cellSize + ((m_cellSize - m_borderWidth) / 2))
          Dim offsetY = CSng(y * m_cellSize + ((m_cellSize - m_borderWidth) / 2))

          ax(0) = MathF.Cos(angle) * radius + offsetX
          ay(0) = MathF.Sin(angle) * radius + offsetY
          ax(1) = MathF.Cos(angle) * -radius + offsetX
          ay(1) = MathF.Sin(angle) * -radius + offsetY
          ax(2) = MathF.Cos(angle + 0.1F) * radius * 0.7F + offsetX
          ay(2) = MathF.Sin(angle + 0.1F) * radius * 0.7F + offsetY
          ax(3) = MathF.Cos(angle - 0.1F) * radius * 0.7F + offsetX
          ay(3) = MathF.Sin(angle - 0.1F) * radius * 0.7F + offsetY

          DrawLine(CInt(Fix(ax(0))), CInt(Fix(ay(0))), CInt(Fix(ax(1))), CInt(Fix(ay(1))), Presets.Cyan)
          DrawLine(CInt(Fix(ax(0))), CInt(Fix(ay(0))), CInt(Fix(ax(2))), CInt(Fix(ay(2))), Presets.Cyan)
          DrawLine(CInt(Fix(ax(0))), CInt(Fix(ay(0))), CInt(Fix(ax(3))), CInt(Fix(ay(3))), Presets.Cyan)

        End If

      Next
    Next

    Dim firstPoint = True
    Dim ox, oy As Integer

    For Each a In path

      If firstPoint Then
        ox = a.X
        oy = a.Y
        firstPoint = False
      Else

        DrawLine(ox * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                 oy * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                 a.X * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                 a.Y * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                 Presets.Yellow)

        ox = a.X
        oy = a.Y

        FillCircle(ox * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                   oy * m_cellSize + ((m_cellSize - m_borderWidth) \ 2),
                   10, Presets.Yellow)

      End If

    Next

    Return True

  End Function

End Class