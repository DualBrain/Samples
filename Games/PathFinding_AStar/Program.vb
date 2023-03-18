' Inspired by: "Path Planning - A* (A-Star)" -- @javidx9
' https://youtu.be/icZj67PTFhc

Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour

Module Program

  Sub Main() 'args As String())
    Dim game As New PathFinding_AStar
    game.ConstructConsole(160, 160, 6, 6)
    game.Start()
  End Sub

End Module

Class PathFinding_AStar
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private Class Node
    Public Obstacle As Boolean                 ' Is the node an obstruction?
    Public Visited As Boolean                  ' Have we searched this node before?
    Public GlobalGoal As Double                ' Distance to goal so far
    Public LocalGoal As Double                 ' Distance to goal if we took the alternate route
    Public X As Integer                         ' Nodes position in 2D space
    Public Y As Integer
    Public Neighbours As New List(Of Node)  ' Connections to neighbours
    Public Parent As Node                      ' Node connecting to this node that offers shortest parent
  End Class

  Private m_nodes() As Node
  Private ReadOnly m_mapWidth As Integer = 16
  Private ReadOnly m_mapHeight As Integer = 16

  Private m_nodeStart As Node = Nothing
  Private m_nodeEnd As Node = Nothing

  Public Overrides Function OnUserCreate() As Boolean

    ' Create a 2D array of nodes - this is for convenience of rendering and construction
    ' and is not required for the algorithm to work - the nodes could be placed anywhere
    ' in any space, in multiple dimensions...
    ReDim m_nodes(m_mapWidth * m_mapHeight)

    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        m_nodes(y * m_mapWidth + x) = New Node With {.X = x, ' ...because we give each node its own coordinates
                                                     .Y = y,
                                                     .Obstacle = False,
                                                     .Parent = Nothing,
                                                     .Visited = False}
      Next
    Next

    ' Create connections - in this case nodes are on a regular grid
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        If y > 0 Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y - 1) * m_mapWidth + (x + 0)))
        If y < m_mapHeight - 1 Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y + 1) * m_mapWidth + (x + 0)))
        If x > 0 Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y + 0) * m_mapWidth + (x - 1)))
        If x < m_mapWidth - 1 Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y + 0) * m_mapWidth + (x + 1)))
        ' We can also connect diagonally
        If (y > 0 AndAlso x > 0) Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y - 1) * m_mapWidth + (x - 1)))
        If (y < m_mapHeight - 1 AndAlso x > 0) Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y + 1) * m_mapWidth + (x - 1)))
        If (y > 0 AndAlso x < m_mapWidth - 1) Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y - 1) * m_mapWidth + (x + 1)))
        If (y < m_mapHeight - 1 AndAlso x < m_mapWidth - 1) Then m_nodes(y * m_mapWidth + x).Neighbours.Add(m_nodes((y + 1) * m_mapWidth + (x + 1)))
      Next
    Next

    ' Manually position the start and end markers so they are not Nothing
    m_nodeStart = m_nodes((m_mapHeight \ 2) * m_mapWidth + 1)
    m_nodeEnd = m_nodes((m_mapHeight \ 2) * m_mapWidth + m_mapWidth - 2)

    Return True

  End Function

  Private Sub Solve_AStar()

    ' Reset Navigation Graph - default all node states
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        m_nodes(y * m_mapWidth + x).Visited = False
        m_nodes(y * m_mapWidth + x).GlobalGoal = Double.MaxValue
        m_nodes(y * m_mapWidth + x).LocalGoal = Double.MaxValue
        m_nodes(y * m_mapWidth + x).Parent = Nothing ' No parent
      Next
    Next

    Dim distance = Function(a As Node, b As Node) ' For convenience
                     Return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y))
                   End Function

    Dim heuristic = Function(a As Node, b As Node) ' So we can experiement with heuristics
                      Return distance(a, b)
                    End Function

    ' Setup starting conditions
    Dim nodeCurrent = m_nodeStart
    m_nodeStart.LocalGoal = 0.0
    m_nodeStart.GlobalGoal = heuristic(m_nodeStart, m_nodeEnd)

    ' Add start node to not tested list - this will ensure it gets tested.
    ' As the algorithm progresses, newly discovered nodes get added to this
    ' list, and will themselves be tested later
    Dim notTestedNodes As New List(Of Node) From {m_nodeStart}

    ' if the Not tested list contains nodes, there may be better paths
    ' which have Not yet been explored. However, we will also stop 
    ' searching when we reach the target - there may well be better
    ' paths but this one will do - it wont be the longest.
    While notTestedNodes.Any AndAlso nodeCurrent IsNot m_nodeEnd ' Find absolutely shortest path ' AndAlso nodeCurrent IsNot nodeEnd)

      ' Sort Untested nodes by global goal, so lowest is first
      notTestedNodes.Sort(Function(lhs, rhs) lhs.GlobalGoal.CompareTo(rhs.GlobalGoal))

      ' Front of listNotTestedNodes is potentially the lowest distance node. Our
      ' list may also contain nodes that have been visited, so ditch these...
      While notTestedNodes.Any AndAlso notTestedNodes(0).Visited
        notTestedNodes.RemoveAt(0)
      End While

      ' ...or abort because there are no valid nodes left to test
      If Not notTestedNodes.Any Then
        Exit While
      End If

      nodeCurrent = notTestedNodes(0)
      nodeCurrent.Visited = True ' We only explore a node once

      ' Check each of this node's neighbours...
      For Each nodeNeighbour In nodeCurrent.Neighbours

        ' ... and only if the neighbour is not visited and is 
        ' not an obstacle, add it to NotTested List
        If Not nodeNeighbour.Visited AndAlso Not nodeNeighbour.Obstacle Then
          notTestedNodes.Add(nodeNeighbour)
        End If

        ' Calculate the neighbours potential lowest parent distance
        Dim possiblyLowerGoal = nodeCurrent.LocalGoal + distance(nodeCurrent, nodeNeighbour)

        ' If choosing to path through this node is a lower distance than what 
        ' the neighbour currently has set, update the neighbour to use this node
        ' as the path source, and set its distance scores as necessary
        If possiblyLowerGoal < nodeNeighbour.LocalGoal Then
          nodeNeighbour.Parent = nodeCurrent
          nodeNeighbour.LocalGoal = possiblyLowerGoal
          ' The best path length to the neighbour being tested has changed, so
          ' update the neighbour's score. The heuristic is used to globally bias
          ' the path algorithm, so it knows if its getting better Or worse. At some
          ' point the algo will realise this path Is worse and abandon it, and then go
          ' and search along the next best path.
          nodeNeighbour.GlobalGoal = nodeNeighbour.LocalGoal + heuristic(nodeNeighbour, m_nodeEnd)
        End If

      Next

    End While

  End Sub

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Dim nodeSize = 9
    Dim nodeBorder = 2

    ' Use integer division to nicely get cursor position in node space
    Dim selectedNodeX = m_mousePosX \ nodeSize
    Dim selectedNodeY = m_mousePosY \ nodeSize

    If m_mouse(0).Released Then ' Use mouse to draw maze, shift and ctrl to place start and end
      If selectedNodeX >= 0 AndAlso selectedNodeX < m_mapWidth Then
        If selectedNodeY >= 0 AndAlso selectedNodeY < m_mapHeight Then

          If m_keys(VK_SHIFT).bHeld Then
            m_nodeStart = m_nodes(selectedNodeY * m_mapWidth + selectedNodeX)
          ElseIf m_keys(VK_CONTROL).bHeld Then
            m_nodeEnd = m_nodes(selectedNodeY * m_mapWidth + selectedNodeX)
          Else
            m_nodes(selectedNodeY * m_mapWidth + selectedNodeX).Obstacle = Not m_nodes(selectedNodeY * m_mapWidth + selectedNodeX).Obstacle
          End If

          Solve_AStar() ' Solve in "real-time" gives a nice effect

        End If
      End If
    End If

    ' Draw Connections First - lines from this nodes position to its
    ' connected neighbour node positions
    Cls()
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        For Each n In m_nodes(y * m_mapWidth + x).Neighbours
          DrawLine(x * nodeSize + nodeSize \ 2, y * nodeSize + nodeSize \ 2, n.X * nodeSize + nodeSize \ 2, n.Y * nodeSize + nodeSize \ 2, PIXEL_SOLID, FG_DARK_BLUE)
        Next
      Next
    Next

    ' Draw Nodes on top
    For x = 0 To m_mapWidth - 1
      For y = 0 To m_mapHeight - 1
        Fill(x * nodeSize + nodeBorder,
             y * nodeSize + nodeBorder,
             (x + 1) * nodeSize - nodeBorder,
             (y + 1) * nodeSize - nodeBorder,
             PIXEL_HALF,
             If(m_nodes(y * m_mapWidth + x).Obstacle, FG_WHITE, FG_BLUE))

        If m_nodes(y * m_mapWidth + x).Visited Then
          Fill(x * nodeSize + nodeBorder, y * nodeSize + nodeBorder, (x + 1) * nodeSize - nodeBorder, (y + 1) * nodeSize - nodeBorder, PIXEL_SOLID, FG_BLUE)
        End If

        If m_nodes(y * m_mapWidth + x) Is m_nodeStart Then
          Fill(x * nodeSize + nodeBorder, y * nodeSize + nodeBorder, (x + 1) * nodeSize - nodeBorder, (y + 1) * nodeSize - nodeBorder, PIXEL_SOLID, FG_GREEN)
        End If

        If m_nodes(y * m_mapWidth + x) Is m_nodeEnd Then
          Fill(x * nodeSize + nodeBorder, y * nodeSize + nodeBorder, (x + 1) * nodeSize - nodeBorder, (y + 1) * nodeSize - nodeBorder, PIXEL_SOLID, FG_RED)
        End If

      Next
    Next

    ' Draw Path by starting at the end, and following the parent node trail
    ' back to the start - the start node will not have a parent path to follow

    If m_nodeEnd IsNot Nothing Then
      Dim p = m_nodeEnd
      While p.Parent IsNot Nothing
        DrawLine(p.X * nodeSize + nodeSize \ 2, p.Y * nodeSize + nodeSize \ 2,
                 p.Parent.X * nodeSize + nodeSize \ 2, p.Parent.Y * nodeSize + nodeSize \ 2, PIXEL_SOLID, FG_YELLOW)
        ' Set next node to this node's parent
        p = p.Parent
      End While
    End If

    Return True

  End Function

End Class