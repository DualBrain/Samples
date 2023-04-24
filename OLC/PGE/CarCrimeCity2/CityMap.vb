Option Explicit On
Option Strict On
Option Infer On

Imports System.Numerics
Imports Olc

Public Class cCityMap

  Private nWidth As Integer = 0
  Private nHeight As Integer = 0
  Private pCells() As cCell = Nothing
  Private pNodes() As cAuto_Node = Nothing

  Public Sub New(w As Integer, h As Integer, mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4))
    CreateCity(w, h, mapTextures, mapMesh, mapTransforms)
  End Sub

  Public ReadOnly Property Nodes(index As Integer) As cAuto_Node
    Get
      Return pNodes(index)
    End Get
  End Property

  Public Function SaveCity(sFilename As String) As Boolean

    'Dim file As std::ofstream = New std::ofstream(sFilename, std::ios.out Or std::ios.binary)
    'If Not file.is_open() Then Return False

    'file.write(CType(m_nWidth, Char()), sizeof(Integer))
    'file.write(CType(m_nHeight, Char()), sizeof(Integer))
    'For x As Integer = 0 To m_nWidth - 1
    '    For y As Integer = 0 To m_nHeight - 1
    '        file.write(CType(Cell(x, y), Char()), sizeof(cCityCell))
    '    Next
    'Next

    Return True

  End Function

  Public Function LoadCity(sFilename As String) As Boolean

    'Dim file As std::ifstream = New std::ifstream(sFilename, std::ios.in Or std::ios.binary)
    'If Not file.is_open() Then Return False
    'Dim w As Integer, h As Integer
    'file.read(CType(w, Char()), sizeof(Integer))
    'file.read(CType(h, Char()), sizeof(Integer))
    'CreateCity(w, h)

    'For x As Integer = 0 To m_nWidth - 1
    '    For y As Integer = 0 To m_nHeight - 1
    '        file.read(CType(Cell(x, y), Char()), sizeof(cCityCell))
    '    Next
    'Next

    Return True

  End Function

  Public Function GetWidth() As Integer
    Return nWidth
  End Function

  Public Function GetHeight() As Integer
    Return nHeight
  End Function

  Public Function Cell(x As Integer, y As Integer) As cCell
    If x >= 0 AndAlso x < nWidth AndAlso y >= 0 AndAlso y < nHeight Then
      Return pCells(y * nWidth + x)
    Else
      Return Nothing
    End If
  End Function

  Public Function Replace(x As Integer, y As Integer, cell As cCell) As cCell

    If cell Is Nothing Then
      Return Nothing
    End If

    If pCells(y * nWidth + x) IsNot Nothing Then
      pCells(y * nWidth + x) = Nothing
    End If

    pCells(y * nWidth + x) = cell

    Return cell

  End Function

  Public Function GetAutoNodeBase(x As Integer, y As Integer) As Integer 'cAuto_Node
    'Return pNodes + (y * nWidth + x) * 49
    Return (y * nWidth + x) * 49
  End Function

  Public Sub RemoveAllTracks()
    For i = 0 To (nWidth * nHeight * 49) - 1
      pNodes(i).listTracks.Clear()
    Next
  End Sub

  Private Sub CreateCity(w As Integer, h As Integer, mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4))

    ReleaseCity()
    nWidth = w
    nHeight = h
    ReDim pCells(nHeight * nWidth - 1)

    ' Create Navigation Node Pool, assumes 5 nodes on east and south
    ' side of each cell. The City owns these nodes, and cells in the
    ' city borrow them and link to them as required
    ReDim pNodes(nHeight * nWidth * 49 - 1)

    ' The cell has 49 nodes, though some are simply unused. This is less memory
    ' efficient certainly, but makes code more intuitive and easier to write

    For x = 0 To nWidth - 1

      For y = 0 To nHeight - 1

        ' Nodes sit between cells, therefore each create nodes along
        ' the east and southern sides of the cell. This assumes that
        ' navigation along the top and left boundaries of the map
        ' will not occur. And it shouldnt, as its water

        Dim idx = (y * nWidth + x) * 49

        For dx = 0 To 6

          Dim off_x As Single
          Dim off_y As Single

          Select Case dx
            Case 0 : off_x = 0.0F
            Case 1 : off_x = 0.083F
            Case 2 : off_x = 0.333F
            Case 3 : off_x = 0.5F
            Case 4 : off_x = 0.667F
            Case 5 : off_x = 0.917F
            Case 6 : off_x = 1.0F
          End Select

          For dy As Integer = 0 To 6

            Select Case dy
              Case 0 : off_y = 0.0F
              Case 1 : off_y = 0.083F
              Case 2 : off_y = 0.333F
              Case 3 : off_y = 0.5F
              Case 4 : off_y = 0.667F
              Case 5 : off_y = 0.917F
              Case 6 : off_y = 1.0F
            End Select

            pNodes(idx + dy * 7 + dx) = New cAuto_Node
            pNodes(idx + dy * 7 + dx).pos = New Vf2d(x + off_x, y + off_y)
            pNodes(idx + dy * 7 + dx).bBlock = False

          Next

        Next

      Next

    Next

    ' Now create default Cell
    For x = 0 To nWidth - 1
      For y = 0 To nHeight - 1
        ' Default city, everything is grass
        pCells(y * nWidth + x) = New cCell_Plane(Me, x, y, CELL_PLANE.PLANE_GRASS)
        ' Give the cell the opportunity to locally reference the resources it needs
        pCells(y * nWidth + x).LinkAssets(mapTextures, mapMesh, mapTransforms)
      Next
    Next

  End Sub

  Private Sub ReleaseCity()

    For x = 0 To nWidth - 1
      For y = 0 To nHeight - 1

        'Erase any tracks attached to nodes
        For i = 0 To 48
          Cell(x, y).pNaviNodes(i).listTracks.Clear()
        Next

        'Release individual cell objects
        If pCells(y * nWidth + x) IsNot Nothing Then
          pCells(y * nWidth + x) = Nothing
        End If

      Next
    Next

    'Release array of cell pointers
    If pCells IsNot Nothing Then
      pCells = Nothing
    End If

    'Release array of automata navigation nodes
    If pNodes IsNot Nothing Then
      pNodes = Nothing
    End If

    nWidth = 0
    nHeight = 0

  End Sub

End Class