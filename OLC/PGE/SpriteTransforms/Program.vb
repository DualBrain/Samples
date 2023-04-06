' Inspired by "2D Sprite Affine Transformations" -- @javidx9
' https://youtu.be/zxwLN2blwbQ

Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Module Program
  Sub Main()
		Dim demo As New SpriteTransforms
		If demo.Construct(256, 240, 4, 4) Then
			demo.Start()
		End If
	End Sub
End Module

Class SpriteTransforms
	Inherits PixelGameEngine

	Sub New()
		AppName = "Sprite Transforms"
	End Sub

	Private m_car As Sprite
	Private m_rotate As Single = 0.0F

	Private Class Matrix3x3
		Public m(2, 2) As Single
	End Class

	Protected Overrides Function OnUserCreate() As Boolean

		m_car = New Sprite("car_top1.png")

		Return True

	End Function

	Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

		If GetKey(Key.Z).Held Then m_rotate -= 2.0F * elapsedTime
		If GetKey(Key.X).Held Then m_rotate += 2.0F * elapsedTime

		Clear(Presets.DarkCyan)

		SetPixelMode(Pixel.Mode.Alpha)

#Region "Replace it with..."

		Dim t As New Gfx2D.Transform2D
		t.Translate(-100, -50)
		t.Rotate(m_rotate)
		t.Translate(CSng(ScreenWidth / 2), CSng(ScreenHeight / 2))
		Gfx2D.DrawSprite(m_car, t)

#End Region

		SetPixelMode(Pixel.Mode.Normal)

		Return True

	End Function

End Class