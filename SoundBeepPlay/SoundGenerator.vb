Option Explicit On
Option Strict On
Option Infer On

Namespace Global.Audio

  Public Class SoundGenerator
    Implements IDisposable

    'Friend Shared SAMPLE_RATE As Integer = 11025

    Private m_stream As System.IO.MemoryStream
    Private m_sampleRate As Integer = 44100
    Private m_frequency As Single
    Private m_duration As Single ' Measured in ticks (18.2 times per second or 55 milliseconds.)

    Public ReadOnly Property Stream As System.IO.Stream
      Get
        Return m_stream
      End Get
    End Property

    Public ReadOnly Property Duration As Integer
      Get
        Return CInt(m_duration * 55)
      End Get
    End Property

    Public Sub New(sampleRate As Integer,
                   frequency As Single,
                   duration As Single)

      InternalNew(sampleRate, frequency, duration)

    End Sub

    Private Sub InternalNew(sampleRate As Integer,
                            frequency As Single,
                            duration As Single)

      m_sampleRate = sampleRate
      m_frequency = frequency
      m_duration = duration

      m_stream = New System.IO.MemoryStream()

      Dim wave = New Wave(m_sampleRate)
      wave.Prepare(m_stream)

      If False Then

#Region "GenerateTone"

        Dim buffer = GenerateTone(frequency, 1)

        Dim length As Long = 0

        Do
          For Each entry In buffer
            m_stream.Write(BitConverter.GetBytes(entry), 0, 2)
            length += 2
            If length = CInt(sampleRate * (duration / 18.2)) * 2 Then
              Exit Do
            End If
          Next
        Loop

#End Region

      Else

        Dim o = New Oscillator(m_sampleRate) With {
          .Frequency = frequency
        }

        Dim length As Long = 0
        Dim sample As Short = 0

        Do

          If frequency < 20000 Then ' Anything above this number is "silent" (according to usage in DONKEY.BAS)
            sample = o.GetNextSample
          End If
          m_stream.Write(BitConverter.GetBytes(sample), 0, 2)

          length += 2

          If length = CInt(sampleRate * (duration / 18.2)) * 2 Then
            Exit Do
          End If

        Loop

      End If

      wave.UpdateSizes(m_stream)

      m_stream.Seek(0, System.IO.SeekOrigin.Begin)

    End Sub

    Private Function GenerateTone(frequency As Single,
                                  Optional amplitude As Single = 1,
                                  Optional samplesPerSec As Integer = 44100,
                                  Optional startPos As Integer = 0,
                                  Optional length As Integer = -1) As Short()

      Dim buffer(44099) As Short

      Const PI As Double = 3.14159265358979

      Dim v1 = samplesPerSec / (PI * 2 * frequency)

      If length = -1 Then
        length = (buffer.Length - 1) - startPos
      End If

      If frequency >= 20000 Then ' Anything above this number is "silent" (according to usage in DONKEY.BAS)
        For K = startPos To startPos + length
          buffer(K) = 0
        Next
      Else
        For K = startPos To startPos + length
          buffer(K) = CShort(Fix(Math.Sin(K / v1) * (32766.5 * amplitude)))
        Next
      End If

      Return buffer

    End Function

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
      If Not disposedValue Then
        If disposing Then
          ' TODO: dispose managed state (managed objects).
          If m_stream IsNot Nothing Then
            m_stream.Dispose()
            m_stream = Nothing
          End If
        End If

        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        ' TODO: set large fields to null.
      End If
      disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '  ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '  Dispose(False)
    '  MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

#End Region

  End Class

  Friend Class Oscillator

    Private ReadOnly m_sampleRate As Integer
    Private m_frequency As Double
    Private m_phaseAngleIncrement As UInteger
    Private m_phaseAngle As UInteger = 0

    Public Sub New(sampleRate As Integer)
      m_sampleRate = sampleRate
    End Sub

    Public Property Frequency() As Double
      Set(value As Double)
        m_frequency = value
        m_phaseAngleIncrement = CUInt(Fix(m_frequency * UInteger.MaxValue / m_sampleRate))
      End Set
      Get
        Return m_frequency
      End Get
    End Property

    Public Function GetNextSample() As Short 'Implements ISampleProvider.GetNextSample

      Dim wholePhaseAngle = CUShort(m_phaseAngle >> 16)

      'Select Case Waveform
      '  Case Waveform.Sine
      '    'amplitude = (short)(short.MaxValue * Math.Sin(2 * Math.PI * wholePhaseAngle / ushort.MaxValue));

      '    'amplitude = CShort(Fix(Short.MaxValue * Math.Sin(CULng(2 * Math.PI * wholePhaseAngle) \ UShort.MaxValue)))
      '    amplitude = CShort(Fix(Short.MaxValue * Math.Sin(2 * Math.PI * wholePhaseAngle / UShort.MaxValue)))

      '  Case Waveform.Square
      Dim amplitude = If(wholePhaseAngle < CUShort(Short.MaxValue), Short.MinValue, Short.MaxValue)

      '  Case Waveform.Triangle
      ''amplitude = (short)(wholePhaseAngle < (ushort.MaxValue) ? 1 * short.MinValue + 2 * wholePhaseAngle :
      ''                                                          3 * short.MaxValue - 2 * wholePhaseAngle);
      'If wholePhaseAngle < UShort.MaxValue Then
      '  Dim value = 1 * CLng(Short.MinValue) + 2 * wholePhaseAngle
      '  If value > Short.MaxValue Then
      '    Do Until value <= Short.MaxValue
      '      value = value - Short.MaxValue
      '    Loop
      '  ElseIf value < Short.MinValue Then
      '    Do Until value >= Short.MinValue
      '      value = value - Short.MinValue
      '    Loop
      '  End If
      '  amplitude = CShort(Fix(value))
      'Else
      '  Dim value = 3 * CLng(Short.MaxValue) - 2 * wholePhaseAngle
      '  If value > Short.MaxValue Then
      '    Do Until value <= Short.MaxValue
      '      value = value - Short.MaxValue
      '    Loop
      '  ElseIf value < Short.MinValue Then
      '    Do Until value >= Short.MinValue
      '      value = value - Short.MinValue
      '    Loop
      '  End If
      '  amplitude = CShort(Fix(value))
      'End If

      '  Case Waveform.Sawtooth
      'amplitude = CShort(Short.MinValue + wholePhaseAngle)

      '  Case Waveform.ReverseSawtooth
      'amplitude = CShort(Short.MaxValue - wholePhaseAngle)

      'End Select

      'm_phaseAngle += m_phaseAngleIncrement
      If CULng(m_phaseAngle) + CULng(m_phaseAngleIncrement) > UInteger.MaxValue Then
        m_phaseAngle = CUInt(CULng(m_phaseAngle) + CULng(m_phaseAngleIncrement) - UInteger.MaxValue)
      Else
        m_phaseAngle += m_phaseAngleIncrement
      End If

      Return amplitude

    End Function

  End Class

  Friend Class Wave

    Private ReadOnly m_sampleRate As Integer = 44100
    Private ReadOnly ChannelCount As Short = 1
    Private ReadOnly BitsPerSample As Integer = 16
    Private ReadOnly m_byteRate As Integer = CInt(m_sampleRate * ChannelCount * BitsPerSample / 8)

    Private m_dataSizeOffset As Long
    Private m_riffSizeOffset As Long

    Public Sub New(sampleRate As Integer)
      m_sampleRate = sampleRate
      m_byteRate = CInt(m_sampleRate * ChannelCount * BitsPerSample / 8)
    End Sub

    Public Sub Prepare(s As System.IO.Stream)

      Dim waveFormat As WaveFormatEx

      waveFormat = New WaveFormatEx With {
        .BitsPerSample = 16,
        .AvgBytesPerSec = m_byteRate,
        .Channels = ChannelCount,
        .BlockAlign = CShort(ChannelCount * (BitsPerSample \ 8)),
        .Ext = Nothing,
        .FormatTag = WaveFormatEx.FormatPCM,
        .SamplesPerSec = m_sampleRate,
        .Size = 0 ' must be zero
        }
      waveFormat.ValidateWaveFormat()

      s.Seek(0, IO.SeekOrigin.Begin)

      '0         4   ChunkID          Contains the letters "RIFF" in ASCII form
      '                               (0x52494646 big-endian form).

      'Dim b As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes("RIFF")
      Dim b As Byte() = {AscW("R"c), AscW("I"c), AscW("F"c), AscW("F"c)}
      'Dim b = System.Text.ASCIIEncoding.ASCII.GetBytes("RIFF")
      s.Write(b, 0, 4)
      '4         4   ChunkSize        36 + SubChunk2Size, or more precisely:
      '                               4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
      '                               This is the size of the rest of the chunk 
      '                               following this number.  This is the size of the 
      '                               entire file in bytes minus 8 bytes for the
      '                               two fields not included in this count:
      '                               ChunkID and ChunkSize.
      b = BitConverter.GetBytes(CInt(0))
      m_riffSizeOffset = s.Position
      s.Write(b, 0, 4)
      '8         4   Format           Contains the letters "WAVE"
      '                               (0x57415645 big-endian form).
      b = {AscW("W"c), AscW("A"c), AscW("V"c), AscW("E"c)}
      'b = System.Text.ASCIIEncoding.ASCII.GetBytes("WAVE")
      s.Write(b, 0, 4)

      'The "WAVE" format consists of two subchunks: "fmt " and "data":
      'The "fmt " subchunk describes the sound data's format:

      '12        4   Subchunk1ID      Contains the letters "fmt "
      '                               (0x666d7420 big-endian form).
      'b = System.Text.ASCIIEncoding.ASCII.GetBytes("fmt ")
      b = {AscW("f"c), AscW("m"c), AscW("t"c), AscW(" "c)}
      s.Write(b, 0, 4)
      '16        4   Subchunk1Size    16 for PCM.  This is the size of the
      '                               rest of the Subchunk which follows this number.
      b = BitConverter.GetBytes(CInt(16))
      s.Write(b, 0, 4)
      '20        2   AudioFormat      PCM = 1 (i.e. Linear quantization)
      '                               Values other than 1 indicate some 
      '                               form of compression.
      '22        2   NumChannels      Mono = 1, Stereo = 2, etc.
      '24        4   SampleRate       8000, 44100, etc.
      '28        4   ByteRate         == SampleRate * NumChannels * BitsPerSample/8
      '32        2   BlockAlign       == NumChannels * BitsPerSample/8
      '                               The number of bytes for one sample including
      '                               all channels. I wonder what happens when
      '                               this number isn't an integer?
      '34        2   BitsPerSample    8 bits = 8, 16 bits = 16, etc.
      '          2   ExtraParamSize   if PCM, then doesn't exist
      '          X   ExtraParams      space for extra parameters
      b = waveFormat.ToBytes()
      s.Write(b, 0, b.Length)

      'The "data" subchunk contains the size of the data and the actual sound:

      '36        4   Subchunk2ID      Contains the letters "data"
      '                               (0x64617461 big-endian form).
      'b = System.Text.ASCIIEncoding.ASCII.GetBytes("data")
      b = {AscW("d"c), AscW("a"c), AscW("t"c), AscW("a"c)}
      s.Write(b, 0, 4)
      '40        4   Subchunk2Size    == NumSamples * NumChannels * BitsPerSample/8
      '                               This is the number of bytes in the data.
      '                               You can also think of this as the size
      '                               of the read of the subchunk following this 
      '                               number.
      b = BitConverter.GetBytes(CInt(0))
      m_dataSizeOffset = s.Position
      s.Write(b, 0, 4)
      '44        *   Data             The actual sound data.

      ' Ready to write data...

    End Sub

    Public Sub UpdateSizes(s As System.IO.Stream)

      Dim fileSize = s.Length

      s.Seek(m_dataSizeOffset, System.IO.SeekOrigin.Begin)
      Dim b = BitConverter.GetBytes(CInt((fileSize - m_dataSizeOffset) - 4))
      s.Write(b, 0, 4)

      s.Seek(m_riffSizeOffset, System.IO.SeekOrigin.Begin)
      b = BitConverter.GetBytes(CInt(fileSize - 8))
      s.Write(b, 0, 4)

    End Sub

  End Class

  Friend Class WaveFormatEx

    Public Const FormatPCM As Short = 1
    Public Const FormatIEEE As Short = 3
    Public Const SizeOf As UInteger = 18

    Private m_FormatTag As Short
    Private m_Channels As Short
    Private m_SamplesPerSec As Integer
    Private m_AvgBytesPerSec As Integer
    Private m_BlockAlign As Short
    Private m_BitsPerSample As Short
    Private m_Size As Short
    Private m_ext As Byte()

#Region "Data"

    Public Property FormatTag() As Short
      Get
        Return m_FormatTag
      End Get
      Set(value As Short)
        m_FormatTag = value
      End Set
    End Property

    Public Property Channels() As Short
      Get
        Return m_Channels
      End Get
      Set(value As Short)
        m_Channels = value
      End Set
    End Property

    Public Property SamplesPerSec() As Integer
      Get
        Return m_SamplesPerSec
      End Get
      Set(value As Integer)
        m_SamplesPerSec = value
      End Set
    End Property

    Public Property AvgBytesPerSec() As Integer
      Get
        Return m_AvgBytesPerSec
      End Get
      Set(value As Integer)
        m_AvgBytesPerSec = value
      End Set
    End Property

    Public Property BlockAlign() As Short
      Get
        Return m_BlockAlign
      End Get
      Set(value As Short)
        m_BlockAlign = value
      End Set
    End Property

    Public Property BitsPerSample() As Short
      Get
        Return m_BitsPerSample
      End Get
      Set(value As Short)
        m_BitsPerSample = value
      End Set
    End Property

    Public Property Size() As Short
      Get
        Return m_Size
      End Get
      Set(value As Short)
        m_Size = value
      End Set
    End Property

    Public Property Ext() As Byte()
      Get
        Return m_ext
      End Get
      Set(value As Byte())
        m_ext = value
      End Set
    End Property

#End Region

    ''' <summary>
    ''' Convert the data to a hex string
    ''' </summary>
    ''' <returns></returns>
    Public Function ToHexString() As String

      Dim s As String = ""

      s += ToLittleEndianString(String.Format("{0:X4}", FormatTag))
      s += ToLittleEndianString(String.Format("{0:X4}", Channels))
      s += ToLittleEndianString(String.Format("{0:X8}", SamplesPerSec))
      s += ToLittleEndianString(String.Format("{0:X8}", AvgBytesPerSec))
      s += ToLittleEndianString(String.Format("{0:X4}", BlockAlign))
      s += ToLittleEndianString(String.Format("{0:X4}", BitsPerSample))
      s += ToLittleEndianString(String.Format("{0:X4}", Size))

      Return s

    End Function

    ''' <summary>
    ''' Set the data from a byte array (usually read from a file)
    ''' </summary>
    ''' <param name="byteArray"></param>
    Public Sub SetFromByteArray(byteArray As Byte())

      If (byteArray.Length + 2) < SizeOf Then
        Throw New ArgumentException("Byte array is too small")
      End If

      FormatTag = BitConverter.ToInt16(byteArray, 0)
      Channels = BitConverter.ToInt16(byteArray, 2)
      SamplesPerSec = BitConverter.ToInt32(byteArray, 4)
      AvgBytesPerSec = BitConverter.ToInt32(byteArray, 8)
      BlockAlign = BitConverter.ToInt16(byteArray, 12)
      BitsPerSample = BitConverter.ToInt16(byteArray, 14)
      If byteArray.Length >= SizeOf Then
        Size = BitConverter.ToInt16(byteArray, 16)
      Else
        Size = 0
      End If

      If byteArray.Length > WaveFormatEx.SizeOf Then
        Ext = New Byte(CShort(byteArray.Length - WaveFormatEx.SizeOf - 1)) {}
        Array.Copy(byteArray, CShort(WaveFormatEx.SizeOf), Ext, 0, Ext.Length)
      Else
        Ext = Nothing
      End If

    End Sub

    ''' <summary>
    ''' Convert a BigEndian string to a LittleEndian string
    ''' </summary>
    ''' <param name="bigEndianString"></param>
    ''' <returns></returns>
    Public Shared Function ToLittleEndianString(bigEndianString As String) As String

      If bigEndianString Is Nothing Then
        Return ""
      End If

      Dim bigEndianChars As Char() = bigEndianString.ToCharArray()

      ' Guard
      If bigEndianChars.Length Mod 2 <> 0 Then
        Return ""
      End If

      Dim i As Short, ai As Short, bi As Short, ci As Short, di As Short
      Dim a As Char, b As Char, c As Char, d As Char

      For i = 0 To CShort(bigEndianChars.Length \ 2 - 1) Step 2
        ' front byte
        ai = i
        bi = CShort(i + 1)

        ' back byte
        ci = CShort(bigEndianChars.Length - 2 - i)
        di = CShort(bigEndianChars.Length - 1 - i)

        a = bigEndianChars(ai)
        b = bigEndianChars(bi)
        c = bigEndianChars(ci)
        d = bigEndianChars(di)

        bigEndianChars(ci) = a
        bigEndianChars(di) = b
        bigEndianChars(ai) = c
        bigEndianChars(bi) = d

      Next

      Return New String(bigEndianChars)

    End Function

    ''' <summary>
    ''' Ouput the data into a string.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String

      Dim rawData As Char() = New Char(17) {}

      BitConverter.GetBytes(FormatTag).CopyTo(rawData, 0)
      BitConverter.GetBytes(Channels).CopyTo(rawData, 2)
      BitConverter.GetBytes(SamplesPerSec).CopyTo(rawData, 4)
      BitConverter.GetBytes(AvgBytesPerSec).CopyTo(rawData, 8)
      BitConverter.GetBytes(BlockAlign).CopyTo(rawData, 12)
      BitConverter.GetBytes(BitsPerSample).CopyTo(rawData, 14)
      BitConverter.GetBytes(Size).CopyTo(rawData, 16)

      Return New String(rawData)

    End Function

    Public Function ToBytes() As Byte()

      Dim rawData As Byte() = New Byte(15) {}

      '20        2   AudioFormat      PCM = 1 (i.e. Linear quantization)
      '                               Values other than 1 indicate some 
      '                               form of compression.
      '22        2   NumChannels      Mono = 1, Stereo = 2, etc.
      '24        4   SampleRate       8000, 44100, etc.
      '28        4   ByteRate         == SampleRate * NumChannels * BitsPerSample/8
      '32        2   BlockAlign       == NumChannels * BitsPerSample/8
      '                               The number of bytes for one sample including
      '                               all channels. I wonder what happens when
      '                               this number isn't an integer?
      '34        2   BitsPerSample    8 bits = 8, 16 bits = 16, etc.
      '          2   ExtraParamSize   if PCM, then doesn't exist
      '          X   ExtraParams      space for extra parameters

      BitConverter.GetBytes(FormatTag).CopyTo(rawData, 0)
      BitConverter.GetBytes(Channels).CopyTo(rawData, 2)
      BitConverter.GetBytes(SamplesPerSec).CopyTo(rawData, 4)
      BitConverter.GetBytes(AvgBytesPerSec).CopyTo(rawData, 8)
      BitConverter.GetBytes(BlockAlign).CopyTo(rawData, 12)
      BitConverter.GetBytes(BitsPerSample).CopyTo(rawData, 14)
      'BitConverter.GetBytes(Size).CopyTo(rawData, 16)

      Return rawData

    End Function

    ''' <summary>
    ''' Calculate the duration of audio based on the size of the buffer
    ''' </summary>
    ''' <param name="cbAudioDataSize"></param>
    ''' <returns></returns>
    Public Function AudioDurationFromBufferSize(cbAudioDataSize As UInteger) As Long

      If AvgBytesPerSec = 0 Then
        Return 0
      End If

      Return CType(cbAudioDataSize * 10000000 / AvgBytesPerSec, Long)

    End Function

    ''' <summary>
    ''' Calculate the buffer size necessary for a duration of audio
    ''' </summary>
    ''' <param name="duration"></param>
    ''' <returns></returns>
    Public Function BufferSizeFromAudioDuration(duration As Long) As Long

      Dim size As Long = duration * AvgBytesPerSec \ 10000000
      Dim remainder As UInteger = CType(size Mod BlockAlign, UInteger)

      If remainder <> 0 Then
        size += BlockAlign - remainder
      End If

      Return size

    End Function

    ''' <summary>
    ''' Validate that the Wave format is consistent.
    ''' </summary>
    Public Sub ValidateWaveFormat()

      If FormatTag <> FormatPCM Then
        Throw New ArgumentException("Only PCM format is supported")
      End If

      If Channels <> 1 AndAlso Channels <> 2 Then
        Throw New ArgumentException("Only 1 or 2 channels are supported")
      End If

      If BitsPerSample <> 8 AndAlso BitsPerSample <> 16 Then
        Throw New ArgumentException("Only 8 or 16 bit samples are supported")
      End If

      If Size <> 0 Then
        Throw New ArgumentException("Size must be 0")
      End If

      If BlockAlign <> Channels * (BitsPerSample \ 8) Then
        Throw New ArgumentException("Block Alignment is incorrect")
      End If

      If SamplesPerSec > (UInteger.MaxValue \ BlockAlign) Then
        Throw New ArgumentException("SamplesPerSec overflows")
      End If

      If AvgBytesPerSec <> SamplesPerSec * BlockAlign Then
        Throw New ArgumentException("AvgBytesPerSec is wrong")
      End If

    End Sub

  End Class

End Namespace