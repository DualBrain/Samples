Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Class cCell_Water
  Inherits cCell

  Private meshUnitQuad As Gfx3D.Mesh = Nothing
  Private meshWalls As Gfx3D.Mesh = Nothing
  Private sprWater As Sprite = Nothing
  Private sprSides As Sprite = Nothing
  Private sprClouds As Sprite = Nothing
  Private bNeighboursAreWater(3) As Boolean

  Public Sub New(map As cCityMap, x As Integer, y As Integer)
    MyBase.New(map, x, y)
    nCellType = CellType.CELL_WATER
    bNeighboursAreWater(0) = False
    bNeighboursAreWater(1) = False
    bNeighboursAreWater(2) = False
    bNeighboursAreWater(3) = False
  End Sub

  Public Overrides Sub CalculateAdjacency()

    Dim r = Function(i As Integer, j As Integer)
              If pMap.Cell(nWorldX + i, nWorldY + j) IsNot Nothing Then
                Return pMap.Cell(nWorldX + i, nWorldY + j).nCellType = CellType.CELL_WATER
              Else
                Return False
              End If
            End Function

    bNeighboursAreWater(0) = r(0, -1)
    bNeighboursAreWater(1) = r(+1, 0)
    bNeighboursAreWater(2) = r(0, +1)
    bNeighboursAreWater(3) = r(-1, 0)

  End Sub

  Public Overrides Function LinkAssets(mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4)) As Boolean
    meshUnitQuad = mapMesh("UnitQuad")
    meshWalls = mapMesh("WallsOut")
    sprWater = mapTextures("Water")
    sprSides = mapTextures("WaterSide")
    sprClouds = mapTextures("Clouds")
    Return False
  End Function

  Public Overrides Function Update(fElapsedTime As Single) As Boolean
    Return False
  End Function

  Public Overrides Function DrawBase(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean
    Dim matWorld = Gfx3D.Math.Mat_MakeTranslation(nWorldX, nWorldY, 0.0F)
    pipe.SetTransform(matWorld)
    pipe.SetTexture(sprSides)
    If Not bNeighboursAreWater(1) Then pipe.Render(meshWalls.Tris, Gfx3D.RenderFlags.RenderLights Or Gfx3D.RenderFlags.RenderCullCcw Or Gfx3D.RenderFlags.RenderTextured Or Gfx3D.RenderFlags.RenderDepth, 0, 2)
    If Not bNeighboursAreWater(3) Then pipe.Render(meshWalls.Tris, Gfx3D.RenderFlags.RenderLights Or Gfx3D.RenderFlags.RenderCullCcw Or Gfx3D.RenderFlags.RenderTextured Or Gfx3D.RenderFlags.RenderDepth, 2, 2)
    If Not bNeighboursAreWater(2) Then pipe.Render(meshWalls.Tris, Gfx3D.RenderFlags.RenderLights Or Gfx3D.RenderFlags.RenderCullCcw Or Gfx3D.RenderFlags.RenderTextured Or Gfx3D.RenderFlags.RenderDepth, 4, 2)
    If Not bNeighboursAreWater(0) Then pipe.Render(meshWalls.Tris, Gfx3D.RenderFlags.RenderLights Or Gfx3D.RenderFlags.RenderCullCcw Or Gfx3D.RenderFlags.RenderTextured Or Gfx3D.RenderFlags.RenderDepth, 6, 2)
    Return False
  End Function

  Private Function RenderWater(x As Integer, y As Integer, pSource As Pixel, pDest As Pixel) As Pixel
    Dim a = CSng(pSource.A / 255.0F) * 0.6F
    Dim c = 1.0F - a
    Dim r = a * CSng(pSource.R) + c * CSng(pDest.R)
    Dim g = a * CSng(pSource.G) + c * CSng(pDest.G)
    Dim b = a * CSng(pSource.B) + c * CSng(pDest.B)
    a = 0.4F
    c = 1.0F - a
    Dim sky = sprClouds.GetPixel(x, y)
    Dim sr = a * CSng(sky.R) + c * r
    Dim sg = a * CSng(sky.G) + c * g
    Dim sb = a * CSng(sky.B) + c * b
    Return New Pixel(CByte(sr), CByte(sg), CByte(sb))
  End Function


  Public Overrides Function DrawAlpha(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean

    'Dim renderWater = Function(x As Integer, y As Integer, pSource As Pixel, pDest As Pixel) As Pixel
    '                    Dim a = CSng(pSource.A / 255.0F) * 0.6F
    '                    Dim c = 1.0F - a
    '                    Dim r = a * CSng(pSource.R) + c * CSng(pDest.R)
    '                    Dim g = a * CSng(pSource.G) + c * CSng(pDest.G)
    '                    Dim b = a * CSng(pSource.B) + c * CSng(pDest.B)
    '                    a = 0.4F
    '                    c = 1.0F - a
    '                    Dim sky = sprClouds.GetPixel(x, y)
    '                    Dim sr = a * CSng(sky.R) + c * r
    '                    Dim sg = a * CSng(sky.G) + c * g
    '                    Dim sb = a * CSng(sky.B) + c * b
    '                    Return New Pixel(CByte(sr), CByte(sg), CByte(sb))
    '                  End Function

    pge.SetPixelMode(AddressOf RenderWater)
    Dim matWorld = Gfx3D.Math.Mat_MakeTranslation(CSng(nWorldX), CSng(nWorldY), 0.07F)
    pipe.SetTransform(matWorld)
    pipe.SetTexture(sprWater)
    pipe.Render(meshUnitQuad.Tris, Gfx3D.RenderFlags.RenderCullCw Or Gfx3D.RenderFlags.RenderDepth Or Gfx3D.RenderFlags.RenderTextured)
    pge.SetPixelMode(Pixel.Mode.Normal)

    Return False

  End Function

End Class