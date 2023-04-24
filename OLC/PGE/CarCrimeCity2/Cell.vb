Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Enum CellType
  CELL_BLANK
  CELL_GRASS
  CELL_CONCRETE
  CELL_WATER
  CELL_BUILDING
  CELL_ROAD
  CELL_PAVEMENT
End Enum

Public Class cCell

  Protected pMap As cCityMap = Nothing

  Public nWorldX As Integer = 0
  Public nWorldY As Integer = 0
  Public bSolid As Boolean = False
  Public nCellType As CellType = CellType.CELL_BLANK

  ' This cell may actually be occupied by a multi-cell body
  ' so this pointer points to the host cell that contains
  ' that body
  Public pHostCell As cCell = Nothing

  ' Each cell links to 20 automata transport nodes, 5 on each side
  Public pNaviNodes(48) As cAuto_Node

  ' Each cell can have a number of automata transport tracks, it owns them
  ' These connect nodes together as determined by the cell
  Public listTracks As New List(Of cAuto_Track)

  Public Sub New()
    ' Cells own a list of automata navigation tracks
    ' but this will be destroyed when the cell Is deleted
  End Sub

  Public Sub New(map As cCityMap, x As Integer, y As Integer)

    pMap = [map]
    nWorldX = x
    nWorldY = y
    nCellType = CellType.CELL_BLANK

    ' Connect internal nodes
    For i = 0 To 48
      pNaviNodes(i) = pMap.Nodes(pMap.GetAutoNodeBase(x, y) + i)
    Next

    ' Link cell into maps node pool
    If y > 0 Then
      For i = 0 To 6
        pNaviNodes(i) = pMap.Nodes(pMap.GetAutoNodeBase(x, y - 1) + 42 + i)
      Next
    Else
      For i = 0 To 6
        pNaviNodes(i) = Nothing
      Next
    End If

    If x > 0 Then
      ' Link West side
      For i = 0 To 6
        pNaviNodes(i * 7) = pMap.Nodes(pMap.GetAutoNodeBase(x - 1, y) + 6 + i * 7)
      Next
    Else
      For i = 0 To 6
        pNaviNodes(i * 7) = Nothing
      Next
    End If

    ' South Side
    If y < pMap.GetHeight() - 1 Then

    Else
      For i = 0 To 6
        pNaviNodes(42 + i) = Nothing
      Next
    End If

    ' East Side
    If x < pMap.GetWidth() - 1 Then

    Else
      For i = 0 To 6
        pNaviNodes(6 + i * 7) = Nothing
      Next
    End If

    ' Unused Nodes
    pNaviNodes(9) = Nothing
    pNaviNodes(11) = Nothing
    pNaviNodes(15) = Nothing
    pNaviNodes(19) = Nothing
    pNaviNodes(29) = Nothing
    pNaviNodes(33) = Nothing
    pNaviNodes(37) = Nothing
    pNaviNodes(39) = Nothing
    pNaviNodes(0) = Nothing
    pNaviNodes(6) = Nothing
    pNaviNodes(42) = Nothing
    pNaviNodes(48) = Nothing

  End Sub

  Public Overridable Function LinkAssets(mapTextures As Dictionary(Of String, Sprite),
                                         mapMesh As Dictionary(Of String, Gfx3D.Mesh),
                                         mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4)) As Boolean
    Return False
  End Function

  Public Overridable Function Update(fElapsedTime As Single) As Boolean
    Return False
  End Function

  Public Overridable Function DrawBase(pge As PixelGameEngine,
                                       pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

  Public Overridable Function DrawAlpha(pge As PixelGameEngine,
                                        pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

  Public Overridable Function DrawDebug(pge As PixelGameEngine,
                                        pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

  Public Overridable Sub CalculateAdjacency()

  End Sub

End Class