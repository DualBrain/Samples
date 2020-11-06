Option Explicit On
Option Infer On
Option Strict On

Imports NAudio.Wave

Class CachedSoundSampleProvider
  Implements ISampleProvider

  Private ReadOnly m_cachedSound As CachedSound
  Private m_position As Long

  Public Sub New(cachedSound As CachedSound)
    m_cachedSound = cachedSound
  End Sub

  Public Function Read(buffer As Single(), offset As Integer, count As Integer) As Integer Implements ISampleProvider.Read
    Dim availableSamples = m_cachedSound.AudioData.Length - m_position
    Dim samplesToCopy = Math.Min(availableSamples, count)
    Array.Copy(m_cachedSound.AudioData, m_position, buffer, offset, samplesToCopy)
    m_position += samplesToCopy
    Return CInt(Fix(samplesToCopy))
  End Function

  Public ReadOnly Property WaveFormat As WaveFormat Implements ISampleProvider.WaveFormat
    Get
      Return m_cachedSound.WaveFormat
    End Get
  End Property

End Class
