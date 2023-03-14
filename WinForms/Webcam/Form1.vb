Imports OpenCvSharp

Public Class Form1

  Private ReadOnly m_capture As VideoCapture
  Private ReadOnly m_frame As Mat
  Private m_image As Bitmap

  Sub New()

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    m_frame = New Mat()
    m_capture = New VideoCapture(0)
    m_capture.Open(0)

  End Sub

  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

  End Sub

  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    If (m_capture.IsOpened()) Then
      m_capture.Read(m_frame)
      'm_image = BitmapConverter.ToBitmap(m_frame)
      m_image = MatToBitmap(m_frame)
      PictureBox1.Image?.Dispose()
      PictureBox1.Image = m_image
    End If
  End Sub

  Private Shared Function MatToBitmap(mat As Mat) As Bitmap
    Using ms = mat.ToMemoryStream()
      Return Image.FromStream(ms)
    End Using
  End Function

End Class
