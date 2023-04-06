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

	Private Shared Sub Identity(mat As Matrix3x3)
		mat.m(0, 0) = 1.0F : mat.m(1, 0) = 0.0F : mat.m(2, 0) = 0.0F
		mat.m(0, 1) = 0.0F : mat.m(1, 1) = 1.0F : mat.m(2, 1) = 0.0F
		mat.m(0, 2) = 0.0F : mat.m(1, 2) = 0.0F : mat.m(2, 2) = 1.0F
	End Sub

	Private Shared Sub Translate(mat As Matrix3x3, ox As Single, oy As Single)
		mat.m(0, 0) = 1.0F : mat.m(1, 0) = 0.0F : mat.m(2, 0) = ox
		mat.m(0, 1) = 0.0F : mat.m(1, 1) = 1.0F : mat.m(2, 1) = oy
		mat.m(0, 2) = 0.0F : mat.m(1, 2) = 0.0F : mat.m(2, 2) = 1.0F
	End Sub

	Private Shared Sub Rotate(mat As Matrix3x3, fTheta As Single)
		mat.m(0, 0) = CSng(Math.Cos(fTheta)) : mat.m(1, 0) = CSng(Math.Sin(fTheta)) : mat.m(2, 0) = 0.0F
		mat.m(0, 1) = -CSng(Math.Sin(fTheta)) : mat.m(1, 1) = CSng(Math.Cos(fTheta)) : mat.m(2, 1) = 0.0F
		mat.m(0, 2) = 0.0F : mat.m(1, 2) = 0.0F : mat.m(2, 2) = 1.0F
	End Sub

	Private Shared Sub Scale(mat As Matrix3x3, sx As Single, sy As Single)
		mat.m(0, 0) = sx : mat.m(1, 0) = 0.0F : mat.m(2, 0) = 0.0F
		mat.m(0, 1) = 0.0F : mat.m(1, 1) = sy : mat.m(2, 1) = 0.0F
		mat.m(0, 2) = 0.0F : mat.m(1, 2) = 0.0F : mat.m(2, 2) = 1.0F
	End Sub

	Private Shared Sub Shear(mat As Matrix3x3, sx As Single, sy As Single)
		mat.m(0, 0) = 1.0F : mat.m(1, 0) = sx : mat.m(2, 0) = 0.0F
		mat.m(0, 1) = sy : mat.m(1, 1) = 1.0F : mat.m(2, 1) = 0.0F
		mat.m(0, 2) = 0.0F : mat.m(1, 2) = 0.0F : mat.m(2, 2) = 1.0F
	End Sub

	Private Shared Sub MatrixMultiply(matResult As Matrix3x3, matA As Matrix3x3, matB As Matrix3x3)
		For c = 0 To 2
			For r = 0 To 2
				matResult.m(c, r) = matA.m(0, r) * matB.m(c, 0) +
														matA.m(1, r) * matB.m(c, 1) +
														matA.m(2, r) * matB.m(c, 2)
			Next
		Next
	End Sub

	Private Shared Sub Forward(mat As Matrix3x3, in_x As Single, in_y As Single, ByRef out_x As Single, ByRef out_y As Single)
		out_x = in_x * mat.m(0, 0) + in_y * mat.m(1, 0) + mat.m(2, 0)
		out_y = in_x * mat.m(0, 1) + in_y * mat.m(1, 1) + mat.m(2, 1)
	End Sub

	Private Shared Sub Invert(matIn As Matrix3x3, matOut As Matrix3x3)

		Dim det = matIn.m(0, 0) * (matIn.m(1, 1) * matIn.m(2, 2) - matIn.m(1, 2) * matIn.m(2, 1)) -
							matIn.m(1, 0) * (matIn.m(0, 1) * matIn.m(2, 2) - matIn.m(2, 1) * matIn.m(0, 2)) +
							matIn.m(2, 0) * (matIn.m(0, 1) * matIn.m(1, 2) - matIn.m(1, 1) * matIn.m(0, 2))

		Dim idet = 1.0F / det
		matOut.m(0, 0) = (matIn.m(1, 1) * matIn.m(2, 2) - matIn.m(1, 2) * matIn.m(2, 1)) * idet
		matOut.m(1, 0) = (matIn.m(2, 0) * matIn.m(1, 2) - matIn.m(1, 0) * matIn.m(2, 2)) * idet
		matOut.m(2, 0) = (matIn.m(1, 0) * matIn.m(2, 1) - matIn.m(2, 0) * matIn.m(1, 1)) * idet
		matOut.m(0, 1) = (matIn.m(2, 1) * matIn.m(0, 2) - matIn.m(0, 1) * matIn.m(2, 2)) * idet
		matOut.m(1, 1) = (matIn.m(0, 0) * matIn.m(2, 2) - matIn.m(2, 0) * matIn.m(0, 2)) * idet
		matOut.m(2, 1) = (matIn.m(0, 1) * matIn.m(2, 0) - matIn.m(0, 0) * matIn.m(2, 1)) * idet
		matOut.m(0, 2) = (matIn.m(0, 1) * matIn.m(1, 2) - matIn.m(0, 2) * matIn.m(1, 1)) * idet
		matOut.m(1, 2) = (matIn.m(0, 2) * matIn.m(1, 0) - matIn.m(0, 0) * matIn.m(1, 2)) * idet
		matOut.m(2, 2) = (matIn.m(0, 0) * matIn.m(1, 1) - matIn.m(0, 1) * matIn.m(1, 0)) * idet

	End Sub

	Protected Overrides Function OnUserCreate() As Boolean

		m_car = New Sprite("car_top1.png")

		Return True

	End Function

	Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

		If GetKey(Key.Z).Held Then m_rotate -= 2.0F * elapsedTime
		If GetKey(Key.X).Held Then m_rotate += 2.0F * elapsedTime

		Clear(Presets.DarkCyan)

		SetPixelMode(Pixel.Mode.Alpha)
		'DrawSprite(0, 0, m_ar, 3)

#Region "Remove after finishing GFX2D PGE Extension"

		'Dim matFinal = New Matrix3x3
		'Dim matA = New Matrix3x3
		'Dim matB = New Matrix3x3
		'Dim matC = New Matrix3x3
		'Dim matFinalInv = New Matrix3x3

		''Translate(matFinal, 100, 100)

		''Scale(matFinal, 0.5, 0.5)

		''Scale(matFinal, 2, 2) ' with gaps?

		''Rotate(matFinal, 0.2) ' with the pesky gaps???

		'Translate(matA, -100, -50)
		'Rotate(matB, m_rotate)
		'MatrixMultiply(matC, matB, matA)
		'Translate(matA, ScreenWidth \ 2, ScreenHeight \ 2)
		'MatrixMultiply(matFinal, matA, matC)

		'Invert(matFinal, matFinalInv)

		''For x = 0 To m_car.Width - 1
		''	For y = 0 To m_car.Height - 1
		''		Dim p = m_car.GetPixel(x, y)
		''		Dim nx, ny As Single
		''		Forward(matFinal, x, y, nx, ny)
		''		Draw(CInt(Fix(nx)), CInt(Fix(ny)), p)
		''	Next
		''Next

		'' Work out bounding box of sprite post-transformation
		'' by passing through sprite corner locations into
		'' transformation matrix
		'Dim ex, ey As Single
		'Dim sx, sy As Single
		'Dim px, py As Single

		'Forward(matFinal, 0.0F, 0.0F, px, py)
		'sx = px : sy = py
		'ex = px : ey = py

		'Forward(matFinal, m_car.Width, m_car.Height, px, py)
		'sx = Math.Min(sx, px) : sy = Math.Min(sy, py)
		'ex = Math.Max(ex, px) : ey = Math.Max(ey, py)

		'Forward(matFinal, 0.0F, m_car.Height, px, py)
		'sx = Math.Min(sx, px) : sy = Math.Min(sy, py)
		'ex = Math.Max(ex, px) : ey = Math.Max(ey, py)

		'Forward(matFinal, m_car.Width, 0.0F, px, py)
		'sx = Math.Min(sx, px) : sy = Math.Min(sy, py)
		'ex = Math.Max(ex, px) : ey = Math.Max(ey, py)

		'' Use transformed corner locations in screen space to establish
		'' region of pixels to fill, using inverse transform to sample
		'' sprite at suitable locations.
		'For x = sx To ex - 1
		'	For y = sy To ey - 1
		'		Dim nx, ny As Single
		'		Forward(matFinalInv, x, y, nx, ny)
		'		Dim p = m_car.GetPixel(CInt(Fix(nx + 0.5F)), CInt(Fix(ny + 0.5F)))
		'		Draw(CInt(Fix(x)), CInt(Fix(y)), p)
		'	Next
		'Next

#End Region

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