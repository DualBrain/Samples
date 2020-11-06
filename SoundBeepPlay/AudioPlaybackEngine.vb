'Option Explicit On
'Option Infer On
'Option Strict On

'Imports NAudio.Wave
'Imports NAudio.Wave.SampleProviders

'Class AudioPlaybackEngine
'  Implements IDisposable

'  Private ReadOnly _outputDevice As IWavePlayer
'  Private ReadOnly _mixer As MixingSampleProvider

'  Public Sub New(Optional sampleRate As Integer = 44100, Optional channelCount As Integer = 2)
'    _outputDevice = New WaveOutEvent
'    _mixer = New MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount))
'    _mixer.ReadFully = True
'    _outputDevice.Init(_mixer)
'    _outputDevice.Play()
'  End Sub

'  Public Sub PlaySound(fileName As String)
'    Dim input = New AudioFileReader(fileName)
'    AddMixerInput(New AutoDisposeFileReader(input))
'  End Sub

'  Public Sub PlaySound(s As IO.Stream)
'    Dim input = New AudioFileReader(fileName)
'    AddMixerInput(New AutoDisposeFileReader(input))
'  End Sub

'  Private Function ConvertToRightChannelCount(input As ISampleProvider) As ISampleProvider
'    If input.WaveFormat.Channels = _mixer.WaveFormat.Channels Then
'      Return input
'    End If
'    If input.WaveFormat.Channels = 1 AndAlso _mixer.WaveFormat.Channels = 2 Then
'      Return New MonoToStereoSampleProvider(input)
'    End If
'    Throw New NotImplementedException("Not yet implemented this channel count conversion")
'  End Function

'  Public Sub PlaySound(sound As CachedSound)
'    AddMixerInput(New CachedSoundSampleProvider(sound))
'  End Sub

'  Private Sub AddMixerInput(input As ISampleProvider)
'    _mixer.AddMixerInput(ConvertToRightChannelCount(input))
'  End Sub

'  Public Sub Dispose() Implements IDisposable.Dispose
'    _outputDevice.Dispose()
'  End Sub

'  Public Shared ReadOnly Instance As New AudioPlaybackEngine(44100, 2)

'End Class