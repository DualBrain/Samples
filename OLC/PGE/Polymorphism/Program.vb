' Inspired by "Practical Polymorphism C++" -- @javidx9
' https://youtu.be/kxKKHKSMGIg

Option Explicit On
Option Strict On
Option Infer On

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
  Inherits Olc.PixelGameEngine

  ' Pan & Zoom variables
  Dim vOffset As New Olc.Vf2d(0.0F, 0.0F)
  Dim vStartPan As New Olc.Vf2d(0.0F, 0.0F)
  Dim fScale As Single = 10.0F
  Dim fGrid As Single = 1.0F

  ' Convert coordinates from World Space --> Screen Space
  Private Sub WorldToScreen(v As Olc.Vf2d, ByRef nScreenX As Integer, ByRef nScreenY As Integer)
    nScreenX = CInt(Fix((v.x - vOffset.X) * fScale))
    nScreenY = CInt(Fix((v.y - vOffset.Y) * fScale))
  End Sub

  ' Convert coordinates from Screen Space --> World Space
  Private Sub ScreenToWorld(nScreenX As Integer, nScreenY As Integer, ByRef v As Olc.Vf2d)
    v.x = nScreenX / fScale + vOffset.x
    v.y = nScreenY / fScale + vOffset.y
  End Sub

  ' A pointer to a shape that is currently being
  ' defined by the placement of nodes
  Dim tempShape As sShape = Nothing

  ' A list of pointers to all shapes which have been drawn so far
  Dim listShapes As New List(Of sShape)

  ' A pointer to a node that is currently selected. Selected
  ' nodes follow the mouse cursor
  Dim selectedNode As sNode = Nothing

  ' "Snapped" mouse location
  Dim vCursor As New Olc.Vf2d(0, 0)

  ' NOTE! No direct instances of lines, circles, boxes or curves,
  ' the application is only aware of the existence of shapes!
  ' THIS iS THE POWER OF POLYMORPHISM!

  Friend Sub New()
    AppName = "Polymorphism"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean
    ' Configure world space (0,0) to be middle of screen space
    vOffset = New Olc.Vf2d(-(CSng(ScreenWidth()) / 2) / fScale, -(CSng(ScreenHeight()) / 2) / fScale)
    Return True
  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Get mouse location this frame
    Dim vMouse As New Olc.Vf2d(GetMouseX(), GetMouseY())

    ' Handle Pan & Zoom
    If GetMouse(2).Pressed Then
      vStartPan = vMouse
    End If

    If GetMouse(2).Held Then
      vOffset -= (vMouse - vStartPan) / fScale
      vStartPan = vMouse
    End If

    Dim vMouseBeforeZoom As New Olc.Vf2d
    ScreenToWorld(CInt(vMouse.X), CInt(vMouse.Y), vMouseBeforeZoom)

    If GetKey(Key.Q).Held OrElse GetMouseWheel() > 0 Then
      fScale *= 1.1F
    End If

    If GetKey(Key.A).Held OrElse GetMouseWheel() < 0 Then
      fScale *= 0.9F
    End If

    Dim vMouseAfterZoom As Olc.Vf2d
    ScreenToWorld(CInt(vMouse.X), CInt(vMouse.Y), vMouseAfterZoom)
    vOffset += (vMouseBeforeZoom - vMouseAfterZoom)

    ' Snap mouse cursor to nearest grid interval
    vCursor.X = MathF.Floor((vMouseAfterZoom.X + 0.5F) * fGrid)
    vCursor.Y = MathF.Floor((vMouseAfterZoom.Y + 0.5F) * fGrid)

    If GetKey(Key.L).Pressed Then
      tempShape = New sLine()
      ' Place first node at location of keypress
      selectedNode = tempShape.GetNextNode(vCursor)
      ' Get Second node
      selectedNode = tempShape.GetNextNode(vCursor)
    End If

    If GetKey(Key.B).Pressed Then
      tempShape = New sBox()
      ' Place first node at location of keypress
      selectedNode = tempShape.GetNextNode(vCursor)
      ' Get Second node
      selectedNode = tempShape.GetNextNode(vCursor)
    End If

    If GetKey(Key.C).Pressed Then
      ' Create new shape as a temporary
      tempShape = New sCircle()
      ' Place first node at location of keypress
      selectedNode = tempShape.GetNextNode(vCursor)
      ' Get Second node
      selectedNode = tempShape.GetNextNode(vCursor)
    End If

    If GetKey(Key.S).Pressed Then
      ' Create new shape as a temporary
      tempShape = New sCurve()
      ' Place first node at location of keypress
      selectedNode = tempShape.GetNextNode(vCursor)
      ' Get Second node
      selectedNode = tempShape.GetNextNode(vCursor)
    End If

    ' Search for any node that exists under the cursor, if one
    ' is found then select it
    If GetKey(Key.M).Pressed Then
      selectedNode = Nothing
      For Each shape In listShapes
        selectedNode = shape.HitNode(vCursor)
        If selectedNode IsNot Nothing Then
          Exit For
        End If
      Next
    End If

    ' If a node is selected, make it follow the mouse cursor
    ' by updating its position
    If selectedNode IsNot Nothing Then
      selectedNode.pos = vCursor
    End If

    ' As the user left clicks to place nodes, the shape can grow
    ' until it requires no more nodes, at which point it is completed
    ' and added to the list of completed shapes.
    If GetMouse(0).Released Then
      If tempShape IsNot Nothing Then
        selectedNode = tempShape.GetNextNode(vCursor)
        If selectedNode Is Nothing Then
          tempShape.col = WHITE
          listShapes.Add(tempShape)
          tempShape = Nothing ' Thanks @howlevergreen /Disord
        End If
      Else
        selectedNode = Nothing
      End If
    End If

    ' Clear Screen
    Clear(New Olc.Pixel(0, 0, 64)) 'VERY_DARK_BLUE)

    Dim sx, sy As Integer
    Dim ex, ey As Integer

    ' Get visible world
    Dim vWorldTopLeft, vWorldBottomRight As Olc.Vf2d
    ScreenToWorld(0, 0, vWorldTopLeft)
    ScreenToWorld(ScreenWidth(), ScreenHeight(), vWorldBottomRight)

    ' Get values just beyond screen boundaries
    vWorldTopLeft.x = MathF.Floor(vWorldTopLeft.x)
    vWorldTopLeft.y = MathF.Floor(vWorldTopLeft.y)
    vWorldBottomRight.x = MathF.Ceiling(vWorldBottomRight.x)
    vWorldBottomRight.y = MathF.Ceiling(vWorldBottomRight.y)

    ' Draw Grid dots
    For x As Single = vWorldTopLeft.x To vWorldBottomRight.x Step fGrid
      For y As Single = vWorldTopLeft.y To vWorldBottomRight.y Step fGrid
        WorldToScreen(New Olc.Vf2d(x, y), sx, sy)
        Draw(sx, sy, Blue)
      Next
    Next

    ' Draw World Axis
    WorldToScreen(New Olc.Vf2d(0, vWorldTopLeft.y), sx, sy)
    WorldToScreen(New Olc.Vf2d(0, vWorldBottomRight.y), ex, ey)
    DrawLine(sx, sy, ex, ey, Grey, &HF0F0F0F0UI)
    WorldToScreen(New Olc.Vf2d(vWorldTopLeft.x, 0), sx, sy)
    WorldToScreen(New Olc.Vf2d(vWorldBottomRight.x, 0), ex, ey)
    DrawLine(sx, sy, ex, ey, Grey, &HF0F0F0F0UI)

    ' Update shape translation coefficients
    sShape.fWorldScale = fScale
    sShape.vWorldOffset = vOffset

    ' Draw All Existing Shapes
    For Each shape In listShapes
      shape.DrawYourself(Me)
      shape.DrawNodes(Me)
    Next

    ' Draw shape currently being defined
    If tempShape IsNot Nothing Then
      tempShape.DrawYourself(Me)
      tempShape.DrawNodes(Me)
    End If

    ' Draw "Snapped" Cursor
    WorldToScreen(vCursor, sx, sy)
    DrawCircle(sx, sy, 3, Yellow)

    ' Draw Cursor Position
    DrawString(10, 10, "X=" + vCursor.X.ToString() + ", Y=" + vCursor.X.ToString(), Yellow, 2)

    Return True

  End Function

End Class

' Define a node
Friend Class sNode
  Public parent As sShape
  Public pos As Olc.Vf2d
End Class

' Our BASE class, defines the interface for all shapes
Friend MustInherit Class sShape

  ' Shapes are defined by the placment of nodes
  Public vecNodes As New List(Of sNode)()
  Private Protected nMaxNodes As Integer = 0

  ' The colour of the shape
  Public col As Olc.Pixel = Green

  ' All shapes share word to screen transformation
  ' coefficients, so share them staically
  Public Shared fWorldScale As Single
  Public Shared vWorldOffset As Olc.Vf2d

  ' Convert coordinates from World Space --> Screen Space
  Protected Shared Sub WorldToScreen(v As Olc.Vf2d, ByRef nScreenX As Integer, ByRef nScreenY As Integer)
    nScreenX = CInt((v.x - vWorldOffset.x) * fWorldScale)
    nScreenY = CInt((v.y - vWorldOffset.y) * fWorldScale)
  End Sub

  ' This is a PURE function, which makes this class abstract. A sub-class
  ' of this class must provide an implementation of this function by
  ' overriding it
  Public MustOverride Sub DrawYourself(pge As Olc.PixelGameEngine)

  ' Shapes are defined by nodes, the shape is responsible
  ' for issuing nodes that get placed by the user. The shape may
  ' change depending on how many nodes have been placed. Once the
  ' maximum number of nodes for a shape have been placed, it returns
  ' Nothing
  Public Function GetNextNode(p As Olc.Vf2d) As sNode
    If vecNodes.Count = nMaxNodes Then
      Return Nothing ' Shape is complete so no new nodes to be issued
    End If
    ' else create new node and add to shapes node vector
    Dim n As New sNode()
    n.parent = Me
    n.pos = p
    vecNodes.Add(n)
    ' Beware! - This normally is bad! But see sub classes
    Return vecNodes(vecNodes.Count - 1)
  End Function

  ' Test to see if supplied coordinate exists at same location
  ' as any of the nodes for this shape. Return a pointer to that
  ' node if it does
  Public Function HitNode(p As Olc.Vf2d) As sNode
    For Each n In vecNodes
      If (p - n.pos).Mag() < 0.01F Then
        Return n
      End If
    Next
    Return Nothing
  End Function

  ' Draw all of the nodes that define this shape so far
  Public Sub DrawNodes(pge As Olc.PixelGameEngine)
    For Each n As sNode In vecNodes
      Dim sx, sy As Integer
      WorldToScreen(n.pos, sx, sy)
      pge.FillCircle(sx, sy, 2, Red)
    Next
  End Sub

End Class

' LINE sub class, inherits from sShape
Friend Class sLine
  Inherits sShape

  Public Sub New()
    nMaxNodes = 2
    vecNodes.Capacity = nMaxNodes
    ' We're gonna be getting pointers to vector elements
    ' though we have defined already how much capacity our vector will have. This makes
    ' it safe to do this as we know the vector will not be maniupulated as we add nodes
    ' to it. Is this bad practice? Possibly, but as with all thing programming, if you
    ' know what you are doing, it's ok :D
  End Sub

  ' Implements custom DrawYourself Function, meaning the shape
  ' is no longer abstract
  Public Overrides Sub DrawYourself(pge As Olc.PixelGameEngine)
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(vecNodes(0).pos, sx, sy)
    WorldToScreen(vecNodes(1).pos, ex, ey)
    pge.DrawLine(sx, sy, ex, ey, col)
  End Sub

End Class

' BOX
Friend Class sBox
  Inherits sShape

  Public Sub New()
    nMaxNodes = 2
    'vecNodes.reserve(nMaxNodes)
  End Sub

  Public Overrides Sub DrawYourself(pge As Olc.PixelGameEngine)
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(vecNodes(0).pos, sx, sy)
    WorldToScreen(vecNodes(1).pos, ex, ey)
    pge.DrawRect(sx, sy, ex - sx, ey - sy, col)
  End Sub
End Class

' CIRCLE
Friend Class sCircle
  Inherits sShape

  Public Sub New()
    nMaxNodes = 2
    'vecNodes.reserve(nMaxNodes)
  End Sub

  Public Overrides Sub DrawYourself(ByVal pge As Olc.PixelGameEngine)
    Dim fRadius As Single = (vecNodes(0).pos - vecNodes(1).pos).Mag()
    Dim sx, sy, ex, ey As Integer
    WorldToScreen(vecNodes(0).pos, sx, sy)
    WorldToScreen(vecNodes(1).pos, ex, ey)
    pge.DrawLine(sx, sy, ex, ey, col, &HFF00FF00UI)
    ' Note the radius is also scaled so it is drawn appropriately
    pge.DrawCircle(sx, sy, CInt(fRadius * fWorldScale), col)
  End Sub

End Class

' BEZIER SPLINE - requires 3 nodes to be defined fully
Friend Class sCurve
  Inherits sShape

  Public Sub New()
    nMaxNodes = 3
    'vecNodes.reserve(nMaxNodes)
  End Sub

  Public Overrides Sub DrawYourself(ByVal pge As Olc.PixelGameEngine)
    Dim sx, sy, ex, ey As Integer

    If vecNodes.Count < 3 Then
      ' Can only draw line from first to second
      WorldToScreen(vecNodes(0).pos, sx, sy)
      WorldToScreen(vecNodes(1).pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, col, &HFF00FF00UI)
    End If

    If vecNodes.Count = 3 Then
      ' Can draw line from first to second
      WorldToScreen(vecNodes(0).pos, sx, sy)
      WorldToScreen(vecNodes(1).pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, col, &HFF00FF00UI)

      ' Can draw second structural line
      WorldToScreen(vecNodes(1).pos, sx, sy)
      WorldToScreen(vecNodes(2).pos, ex, ey)
      pge.DrawLine(sx, sy, ex, ey, col, &HFF00FF00UI)

      ' And bezier curve
      Dim op As Olc.Vf2d = vecNodes(0).pos
      Dim np As Olc.Vf2d = op
      For t As Single = 0 To 0.99 Step 0.01
        np = (1.0F - t) * (1.0F - t) * vecNodes(0).pos + 2.0F * (1.0F - t) * t * vecNodes(1).pos + t * t * vecNodes(2).pos
        WorldToScreen(op, sx, sy)
        WorldToScreen(np, ex, ey)
        pge.DrawLine(sx, sy, ex, ey, col)
        op = np
      Next
    End If
  End Sub

End Class