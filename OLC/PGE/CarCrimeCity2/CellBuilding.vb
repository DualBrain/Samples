Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Class cCell_Building
  Inherits cCell

  Private sName As String
  Private texture As Sprite = Nothing
  Private mesh As Gfx3D.Mesh = Nothing
  Private transform As Gfx3D.Mat4x4

  Public Sub New(name As String, map As cCityMap, x As Integer, y As Integer)
    MyBase.New(map, x, y)
    sName = name
  End Sub

  'Public Overrides Sub CalculateAdjacency()

  'End Sub

  Public Overrides Function LinkAssets(mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4)) As Boolean
    texture = mapTextures(sName)
    mesh = mapMesh(sName)
    transform = mapTransforms(sName)
    Return False
  End Function

  Public Overrides Function Update(fElapsedTime As Single) As Boolean
    Return False
  End Function

  Public Overrides Function DrawBase(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean

    Dim matTranslate = Gfx3D.Math.Mat_MakeTranslation(nWorldX, nWorldY, 0.0F)
    Dim matWorld = Gfx3D.Math.Mat_MultiplyMatrix(transform, matTranslate)
    pipe.SetTransform(matWorld)

    If texture IsNot Nothing Then
      pipe.SetTexture(texture)
      pipe.Render(mesh.Tris, Gfx3D.RenderFlags.RenderCullCw Or Gfx3D.RenderFlags.RenderDepth Or Gfx3D.RenderFlags.RenderTextured Or Gfx3D.RenderFlags.RenderLights)
    Else
      pipe.Render(mesh.Tris, Gfx3D.RenderFlags.RenderCullCw Or Gfx3D.RenderFlags.RenderDepth Or Gfx3D.RenderFlags.RenderFlat Or Gfx3D.RenderFlags.RenderLights)
    End If

    Return False

  End Function

  Public Overrides Function DrawAlpha(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

End Class