Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Enum CELL_PLANE
  PLANE_GRASS
  PLANE_ASPHALT
End Enum

Public Class cCell_Plane
  Inherits cCell

  Protected nType As CELL_PLANE = CELL_PLANE.PLANE_GRASS

  Private meshUnitQuad As Gfx3D.Mesh = Nothing
  Private sprGrass As Sprite = Nothing
  Private sprPavement As Sprite = Nothing

  Public Sub New(map As cCityMap, x As Integer, y As Integer, type As CELL_PLANE)
    MyBase.New(map, x, y)
    bSolid = False
    nType = type
    If nType = CELL_PLANE.PLANE_GRASS Then nCellType = CellType.CELL_GRASS
    If nType = CELL_PLANE.PLANE_ASPHALT Then nCellType = CellType.CELL_PAVEMENT
  End Sub

  Public Overrides Function LinkAssets(mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4)) As Boolean
    sprGrass = mapTextures("Grass")
    sprPavement = mapTextures("Pavement")
    meshUnitQuad = mapMesh("UnitQuad")
    Return True
  End Function

  Public Overrides Function Update(fElapsedTime As Single) As Boolean
    Return False
  End Function

  Public Overrides Function DrawBase(pge As Olc.PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean

    Dim matWorld = Gfx3D.Math.Mat_MakeTranslation(nWorldX, nWorldY, 0.0F)
    pipe.SetTransform(matWorld)

    If nType = CELL_PLANE.PLANE_GRASS Then
      pipe.SetTexture(sprGrass)
    Else
      pipe.SetTexture(sprPavement)
    End If

    pipe.Render(meshUnitQuad.Tris, Gfx3D.RenderFlags.RenderCullCw Or Gfx3D.RenderFlags.RenderDepth Or Gfx3D.RenderFlags.RenderTextured)

    Return False

  End Function

  Public Overrides Function DrawAlpha(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

End Class