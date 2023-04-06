' Inspired by "BIG PROJECT! Top Down City Based Car Crime Game #1" -- @javidx9
' https://youtu.be/mD6b_hP17WI

' current at 19:07 in the video...

Option Explicit On
Option Strict On
Option Infer On

Imports Olc
Imports Olc.Gfx3D

Friend Module Program

  Sub Main()
    Dim game As New CarCrime
    If game.Construct(768, 480, 2, 2) Then
      game.Start()
    End If
  End Sub

End Module

Friend Class CarCrime
  Inherits PixelGameEngine

  Private meshCube As New Gfx3D.Mesh

  Private vUp As New Gfx3D.Vec3d(0, 1, 0)
  Private vEye As New Gfx3D.Vec3d(0, 0, -10)
  Private vLookDir As New Gfx3D.Vec3d(0, 0, 1)

  Private pipeRender As New Gfx3D.PipeLine

  Private fTheta As Single

  Friend Sub New()
    AppName = "Car Crime City"
  End Sub

  Protected Overrides Function OnUserCreate() As Boolean

    '    ' A Full cube - Always useful for debugging
    meshCube.Tris = New List(Of Triangle) From {New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' SOUTH
                                                New Triangle({0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}),
                                                                                                                                                                                             _
                                                New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' EAST
                                                New Triangle({1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                              _
                                                New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' NORTH
                                                New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                              _
                                                New Triangle({0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' WEST
                                                New Triangle({0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                              _
                                                New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' TOP
                                                New Triangle({0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F}), _
                                                                                                                                                                                              _
                                                New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 1.0F}), ' BOTTOM
                                                New Triangle({1.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 1.0F, 1.0F, 1.0F, 1.0F})}


    ' Initialize the 3D Graphics PGE Extension. This is required
    ' to setup internal buffers to the same size as the main output
    Gfx3D.ConfigureDisplay()

    ' Configure the rendering pipeline with projection and viewport properties
    pipeRender.SetProjection(90.0F, CSng(ScreenHeight / ScreenWidth), 0.1F, 1000.0F, 0.0F, 0.0F, ScreenWidth, ScreenHeight)

    Return True

  End Function

  Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    fTheta += elapsedTime

    Clear()

    Dim vLookTarget = Gfx3D.Math.Vec_Add(vEye, vLookDir)

    ' Setup the camera properties for the pipeline - aka "view" transform
    pipeRender.SetCamera(vEye, vLookTarget, vUp)

    Dim matRotateX = Gfx3D.Math.Mat_MakeRotationX(fTheta)
    Dim matRotateZ = Gfx3D.Math.Mat_MakeRotationZ(fTheta / 3.0F)
    Dim matWorld = Gfx3D.Math.Mat_MultiplyMatrix(matRotateX, matRotateZ)

    pipeRender.SetTransform(matWorld)

    pipeRender.Render(meshCube.Tris, Gfx3D.RenderFlags.RenderWire)

    Return True

  End Function

End Class