' Inspired by "Practical Polymorphism C++" -- @javidx9
' https://youtu.be/kxKKHKSMGIg

Option Explicit On
Option Strict On
Option Infer On

Imports Olc
Imports Olc.Presets

Friend Module Program

  Sub Main()
    Dim demo As New Polymorphism
    If demo.Construct(800, 480, 1, 1, False) Then
      demo.Start()
    End If
  End Sub

End Module

Friend Class Polymorphism
  Inherits PixelGameEngine

  ' Pan & Zoom variables
  Private m_offset As New Vf2d(0.0F, 0.0F)
  Private m_startPan As New Vf2d(0.0F, 0.0F)
  Private m_scale As Single = 10.0F
  Private m_grid As Single = 1.0F

  ' Convert coordinates from World Space --> Screen Space
  Private Sub WorldToScreen(v As Olc.Vf2d, ByRef nScreenX As Integer, ByRef nScreenY As Integer)
    nScreenX = CInt(Fix((v.x - m_offset.x) * m_scale))
    nScreenY = CInt(Fix((v.y - m_offset.y) * m_scale))
  End Sub

  ' Convert coordinates from Screen Space --> World Space
  Private Sub ScreenToWorld(nScreenX As Integer, nScreenY As Integer, ByRef v As Olc.Vf2d)
    v.x = nScreenX / m_scale + m_offset.x
    v.y = nScreenY / m_scale + m_offset.y
  End Sub

  ' A pointer to a shape that is currently being
  ' defined by the placement of nodes
  Private m_tempShape As Shape = Nothing

  ' A list of pointers to all shapes which have been drawn so far
  Private ReadOnly m_shapes As New List(Of Shape)

  ' A pointer to a node that is currently selected. Selected
  ' nodes follow the mouse cursor
  Private m_selectedNode As Node = Nothing

  ' "Snapped" mouse location
  Private m_cursor As New Vf2d(0, 0)

  ' NOTE! No direct instances of lines, circles, boxes or curves,
  ' the application is only aware of the existence of shapes!
  ' THIS iS THE POWER OF POLYMORPHISM!

  Friend Sub New()
    AppName = "Polymorphism"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean
    ' Configure world space (0,0) to be middle of screen space
    m_offset = New Vf2d(-(CSng(ScreenWidth()) / 2) / m_scale, -(CSng(ScreenHeight()) / 2) / m_scale)
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Get mouse location this frame
    Dim mouse As New Vf2d(GetMouseX(), GetMouseY())

    ' Handle Pan & Zoom
    If GetMouse(2).Pressed Then
      m_startPan = mouse
    End If

    If GetMouse(2).Held Then
      m_offset -= (mouse - m_startPan) / m_scale
      m_startPan = mouse
    End If

    Dim mouseBeforeZoom As New Vf2d
    ScreenToWorld(CInt(mouse.x), CInt(mouse.y), mouseBeforeZoom)

    If GetKey(Key.Q).Held OrElse GetMouseWheel() > 0 Then
      m_scale *= 1.1F
    End If

    If GetKey(Key.A).Held OrElse GetMouseWheel() < 0 Then
      m_scale *= 0.9F
    End If

    Dim mouseAfterZoom As Vf2d
    ScreenToWorld(CInt(mouse.x), CInt(mouse.y), mouseAfterZoom)
    m_offset += (mouseBeforeZoom - mouseAfterZoom)

    ' Snap mouse cursor to nearest grid interval
    m_cursor.x = MathF.Floor((mouseAfterZoom.x + 0.5F) * m_grid)
    m_cursor.y = MathF.Floor((mouseAfterZoom.y + 0.5F) * m_grid)

    If GetKey(Key.L).Pressed Then
      m_tempShape = New Line()
      ' Place first node at location of keypress
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
      ' Get Second node
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
    End If

    If GetKey(Key.B).Pressed Then
      m_tempShape = New Box()
      ' Place first node at location of keypress
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
      ' Get Second node
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
    End If

    If GetKey(Key.C).Pressed Then
      ' Create new shape as a temporary
      m_tempShape = New Circle()
      ' Place first node at location of keypress
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
      ' Get Second node
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
    End If

    If GetKey(Key.S).Pressed Then
      ' Create new shape as a temporary
      m_tempShape = New Curve()
      ' Place first node at location of keypress
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
      ' Get Second node
      m_selectedNode = m_tempShape.GetNextNode(m_cursor)
    End If

    ' Search for any node that exists under the cursor, if one
    ' is found then select it
    If GetKey(Key.M).Pressed Then
      m_selectedNode = Nothing
      For Each shape In m_shapes
        m_selectedNode = shape.HitNode(m_cursor)
        If m_selectedNode IsNot Nothing Then
          Exit For
        End If
      Next
    End If

    ' If a node is selected, make it follow the mouse cursor
    ' by updating its position
    If m_selectedNode IsNot Nothing Then
      m_selectedNode.Pos = m_cursor
    End If

    ' As the user left clicks to place nodes, the shape can grow
    ' until it requires no more nodes, at which point it is completed
    ' and added to the list of completed shapes.
    If GetMouse(0).Released Then
      If m_tempShape IsNot Nothing Then
        m_selectedNode = m_tempShape.GetNextNode(m_cursor)
        If m_selectedNode Is Nothing Then
          m_tempShape.Col = White
          m_shapes.Add(m_tempShape)
          m_tempShape = Nothing
        End If
      Else
        m_selectedNode = Nothing
      End If
    End If

    ' Clear Screen
    Clear(New Pixel(0, 0, 64)) ' VERY_DARK_BLUE

    Dim sx, sy As Integer
    Dim ex, ey As Integer

    ' Get visible world
    Dim worldTopLeft, worldBottomRight As Vf2d
    ScreenToWorld(0, 0, worldTopLeft)
    ScreenToWorld(ScreenWidth(), ScreenHeight(), worldBottomRight)

    ' Get values just beyond screen boundaries
    worldTopLeft.x = MathF.Floor(worldTopLeft.x)
    worldTopLeft.y = MathF.Floor(worldTopLeft.y)
    worldBottomRight.x = MathF.Ceiling(worldBottomRight.x)
    worldBottomRight.y = MathF.Ceiling(worldBottomRight.y)

    ' Draw Grid dots
    For x = worldTopLeft.x To worldBottomRight.x Step m_grid
      For y = worldTopLeft.y To worldBottomRight.y Step m_grid
        WorldToScreen(New Vf2d(x, y), sx, sy)
        Draw(sx, sy, Blue)
      Next
    Next

    ' Draw World Axis
    WorldToScreen(New Olc.Vf2d(0, worldTopLeft.y), sx, sy)
    WorldToScreen(New Olc.Vf2d(0, worldBottomRight.y), ex, ey)
    DrawLine(sx, sy, ex, ey, Grey, &HF0F0F0F0UI)
    WorldToScreen(New Olc.Vf2d(worldTopLeft.x, 0), sx, sy)
    WorldToScreen(New Olc.Vf2d(worldBottomRight.x, 0), ex, ey)
    DrawLine(sx, sy, ex, ey, Grey, &HF0F0F0F0UI)

    ' Update shape translation coefficients
    Shape.WorldScale = m_scale
    Shape.WorldOffset = m_offset

    ' Draw All Existing Shapes
    For Each shape In m_shapes
      shape.DrawYourself(Me)
      shape.DrawNodes(Me)
    Next

    ' Draw shape currently being defined
    If m_tempShape IsNot Nothing Then
      m_tempShape.DrawYourself(Me)
      m_tempShape.DrawNodes(Me)
    End If

    ' Draw "Snapped" Cursor
    WorldToScreen(m_cursor, sx, sy)
    DrawCircle(sx, sy, 3, Yellow)

    ' Draw Cursor Position
    DrawString(10, 10, $"X={m_cursor.x}, Y={m_cursor.x}", Yellow, 2)

    Return True

  End Function

End Class

' Define a node
Friend Class Node
  Public Parent As Shape
  Public Pos As Vf2d
End Class

' Our BASE class, defines the interface for all shapes
Friend MustInherit Class Shape

  ' Shapes are defined by the placment of nodes
  Public Nodes As New List(Of Node)()
  Private Protected m_maxNodes As Integer = 0

  ' The colour of the shape
  Public Col As Pixel = Green

  ' All shapes share word to screen transformation
  ' coefficients, so share them staically
  Public Shared WorldScale As Single
  Public Shared WorldOffset As Vf2d

  ' Convert coordinates from World Space --> Screen Space
  Protected Shared Sub WorldToScreen(v As Vf2d, ByRef screenX As Integer, ByRef screenY As Integer)
    screenX = CInt((v.x - WorldOffset.x) * WorldScale)
    screenY = CInt((v.y - WorldOffset.y) * WorldScale)
  End Sub

  ' This is a PURE function, which makes this class abstract. A sub-class
  ' of this class must provide an implementation of this function by
  ' overriding it
  Public MustOverride Sub DrawYourself(pge As PixelGameEngine)

  ' Shapes are defined by nodes, the shape is responsible
  ' for issuing nodes that get placed by the user. The shape may
  ' change depending on how many nodes have been placed. Once the
  ' maximum number of nodes for a shape have been placed, it returns
  ' Nothing
  Public Function GetNextNode(p As Vf2d) As Node

    If Nodes.Count = m_maxNodes Then
      Return Nothing ' Shape is complete so no new nodes to be issued
    End If
    ' else create new node and add to shapes node vector
    Dim n As New Node With {.Parent = Me,
                            .Pos = p}
    Nodes.Add(n)

    ' Beware! - This normally is bad! But see sub classes
    Return Nodes(Nodes.Count - 1)

  End Function

  ' Test to see if supplied coordinate exists at same location
  ' as any of the nodes for this shape. Return a pointer to that
  ' node if it does
  Public Function HitNode(p As Vf2d) As Node
    For Each n In Nodes
      If (p - n.Pos).Mag() < 0.01F Then
        Return n
      End If
    Next
    Return Nothing
  End Function

  ' Draw all of the nodes that define this shape so far
  Public Sub DrawNodes(pge As PixelGameEngine)
    For Each n In Nodes
      Dim sx, sy As Integer
      WorldToScreen(n.Pos, sx, sy)
      pge.FillCircle(sx, sy, 2, Red)
    Next
  End Sub

End Class

Friend Class Line
  Inherits Shape

  Public Sub New()
    m_maxNodes = 2
    Nodes.Capacity = m_maxNodes
    ' We're gonna be getting pointers to vector elements
    ' though we have defined already how much capacity our vector will have. This makes
    ' it safe to do this as we know the vector will not be maniupulated as we add nodes
    ' to it. Is this bad practice? Possibly, but as with all thing programming, if you
    ' know what you are doing, it's ok :D
  End Sub

  ' Implements custom DrawYourself Function, meaning the shape
  ' is no longer abstract
  Public Overrides Sub DrawYourself(pge As PixelGameEngine)
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(Nodes(0).Pos, sx, sy)
    WorldToScreen(Nodes(1).Pos, ex, ey)
    pge.DrawLine(sx, sy, ex, ey, Col)
  End Sub

End Class

Friend Class Box
  Inherits Shape

  Public Sub New()
    m_maxNodes = 2
  End Sub

  Public Overrides Sub DrawYourself(pge As PixelGameEngine)
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(Nodes(0).Pos, sx, sy)
    WorldToScreen(Nodes(1).Pos, ex, ey)
    pge.DrawRect(sx, sy, ex - sx, ey - sy, Col)
  End Sub

End Class

Friend Class Circle
  Inherits Shape

  Public Sub New()
    m_maxNodes = 2
  End Sub

  Public Overrides Sub DrawYourself(pge As PixelGameEngine)
    Dim fRadius = (Nodes(0).Pos - Nodes(1).Pos).Mag()
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(Nodes(0).Pos, sx, sy)
    WorldToScreen(Nodes(1).Pos, ex, ey)
    pge.DrawLine(sx, sy, ex, ey, Col, &HFF00FF00UI)
    ' Note the radius is also scaled so it is drawn appropriately
    pge.DrawCircle(sx, sy, CInt(fRadius * WorldScale), Col)
  End Sub

End Class

' BEZIER SPLINE - requires 3 nodes to be defined fully
Friend Class Curve
  Inherits Shape

  Public Sub New()
    m_maxNodes = 3
  End Sub

  Public Overrides Sub DrawYourself(pge As PixelGameEngine)

    Dim sx, sy, ex, ey As Integer

    If Nodes.Count < 3 Then
      ' Can only draw line from first to second
      WorldToScreen(Nodes(0).Pos, sx, sy)
      WorldToScreen(Nodes(1).Pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, Col, &HFF00FF00UI)
    End If

    If Nodes.Count = 3 Then

      ' Can draw line from first to second
      WorldToScreen(Nodes(0).Pos, sx, sy)
      WorldToScreen(Nodes(1).Pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, Col, &HFF00FF00UI)

      ' Can draw second structural line
      WorldToScreen(Nodes(1).Pos, sx, sy)
      WorldToScreen(Nodes(2).Pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, Col, &HFF00FF00UI)

      ' And bezier curve
      Dim op = Nodes(0).Pos
      'Dim np As Olc.Vf2d = op
      For t = 0.0F To 0.99F Step 0.01F
        Dim np = (1.0F - t) * (1.0F - t) * Nodes(0).Pos + 2.0F * (1.0F - t) * t * Nodes(1).Pos + t * t * Nodes(2).Pos
        WorldToScreen(op, sx, sy)
        WorldToScreen(np, ex, ey)
        pge.DrawLine(sx, sy, ex, ey, Col)
        op = np
      Next

    End If

  End Sub

End Class