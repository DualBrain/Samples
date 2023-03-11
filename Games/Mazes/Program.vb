Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine

Module Program
  Sub Main() 'args As String())
    Dim game As New Mazes
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class Mazes
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_mazeWidth As Integer
  Private m_mazeHeight As Integer
  Private m_maze() As Integer

  Private Enum CellPath
    North = &H1
    East = &H2
    South = &H4
    West = &H8
    Visited = &H10
  End Enum

  Private m_visitedCells As Integer
  Private m_stack As New Stack(Of (X As Integer, Y As Integer))
  Private m_pathWidth As Integer

  Public Overrides Function OnUserCreate() As Boolean

    ' Maze parameters
    m_mazeWidth = 40
    m_mazeHeight = 25
    ReDim m_maze(m_mazeWidth * m_mazeHeight)
    m_pathWidth = 3

    ' Choose a starting cell
    Dim x = Rand.Next(0, m_mazeWidth - 1)
    Dim y = Rand.Next(0, m_mazeHeight - 1)
    m_stack.Push((x, y))
    m_maze(y * m_mazeWidth + x) = CellPath.Visited
    m_visitedCells = 1

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    'Threading.Thread.Sleep(10)

    ' Little lambda function to calculate index in a readable way
    Dim offset As Func(Of Integer, Integer, Integer) = Function(x As Integer, y As Integer) ((m_stack.Peek.Y + y) * m_mazeWidth + (m_stack.Peek.X + x))

    ' Do Maze Algorithm
    If m_visitedCells < m_mazeWidth * m_mazeHeight Then

      ' Create a set of unvisted neighbours
      Dim neighbours As New List(Of Integer)

      ' North neighbour
      If m_stack.Peek.Y > 0 AndAlso (m_maze(offset(0, -1)) And CellPath.Visited) = 0 Then
        neighbours.Add(0)
      End If
      ' East neighbour
      If m_stack.Peek.X < m_mazeWidth - 1 AndAlso (m_maze(offset(1, 0)) And CellPath.Visited) = 0 Then
        neighbours.Add(1)
      End If
      ' South neighbour
      If m_stack.Peek.Y < m_mazeHeight - 1 AndAlso (m_maze(offset(0, 1)) And CellPath.Visited) = 0 Then
        neighbours.Add(2)
      End If
      ' West neighbour
      If m_stack.Peek.X > 0 AndAlso (m_maze(offset(-1, 0)) And CellPath.Visited) = 0 Then
        neighbours.Add(3)
      End If

      ' Are there any neighbours available?
      If neighbours.Any() Then
        ' Choose one available neighbour at random
        Dim next_cell_dir As Integer = neighbours(Rand.Next(neighbours.Count))

        ' Create a path between the neighbour and the current cell
        Select Case next_cell_dir
          Case 0 ' North
            m_maze(offset(0, -1)) = m_maze(offset(0, -1)) Or CellPath.Visited Or CellPath.South
            m_maze(offset(0, 0)) = m_maze(offset(0, 0)) Or CellPath.North
            m_stack.Push((m_stack.Peek.X + 0, m_stack.Peek.Y - 1))
          Case 1 ' East
            m_maze(offset(+1, 0)) = m_maze(offset(+1, 0)) Or CellPath.Visited Or CellPath.West
            m_maze(offset(0, 0)) = m_maze(offset(0, 0)) Or CellPath.East
            m_stack.Push((m_stack.Peek.X + 1, m_stack.Peek.Y + 0))
          Case 2 ' South
            m_maze(offset(0, +1)) = m_maze(offset(0, +1)) Or CellPath.Visited Or CellPath.North
            m_maze(offset(0, 0)) = m_maze(offset(0, 0)) Or CellPath.South
            m_stack.Push((m_stack.Peek.X + 0, m_stack.Peek.Y + 1))
          Case 3 ' West
            m_maze(offset(-1, 0)) = m_maze(offset(-1, 0)) Or CellPath.Visited Or CellPath.East
            m_maze(offset(0, 0)) = m_maze(offset(0, 0)) Or CellPath.West
            m_stack.Push((m_stack.Peek.X - 1, m_stack.Peek.Y + 0))
          Case Else
        End Select

        m_visitedCells += 1

      Else

        ' No available neighbors so backtrack!
        m_stack.Pop()

      End If

    End If

    '=== DRAWING STUFF ===

    ' Clear Screen by drawing 'spaces' everywhere
    Fill(0, 0, ScreenWidth(), ScreenHeight(), AscW(" "))

    ' Draw Maze
    For x = 0 To m_mazeWidth - 1
      For y = 0 To m_mazeHeight - 1
        ' Each cell Is inflated by m_pathWidth, so fill it in
        For py = 0 To m_pathWidth - 1
          For px = 0 To m_pathWidth - 1
            If (m_maze(y * m_mazeWidth + x) And CellPath.Visited) <> 0 Then
              Draw(x * (m_pathWidth + 1) + px, y * (m_pathWidth + 1) + py, PIXEL_SOLID, FG_WHITE) ' Draw Cell
            Else
              Draw(x * (m_pathWidth + 1) + px, y * (m_pathWidth + 1) + py, PIXEL_SOLID, FG_BLUE) ' Draw Cell
            End If
          Next
        Next
        ' Draw passageways between cells
        For p = 0 To m_pathWidth - 1
          If (m_maze(y * m_mazeWidth + x) And CellPath.South) <> 0 Then Draw(x * (m_pathWidth + 1) + p, y * (m_pathWidth + 1) + m_pathWidth) ' Draw South Passage
          If (m_maze(y * m_mazeWidth + x) And CellPath.East) <> 0 Then Draw(x * (m_pathWidth + 1) + m_pathWidth, y * (m_pathWidth + 1) + p) ' Draw East Passage
        Next
      Next
    Next

    ' Draw Unit - the top of the stack
    For py = 0 To m_pathWidth - 1
      For px = 0 To m_pathWidth - 1
        Draw(m_stack.Peek.X * (m_pathWidth + 1) + px, m_stack.Peek.Y * (m_pathWidth + 1) + py, &H2588, FG_GREEN) ' Draw Cell
      Next
    Next

    Return True

  End Function

End Class