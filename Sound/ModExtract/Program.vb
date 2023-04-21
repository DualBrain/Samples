Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.InteropServices
Imports System.Text

Module Program

  Private Const MOD_SOUNDTRACKER As UInteger = &H2E4B2E4DUI
  Private Const RATE As Integer = 11025

  <StructLayout(LayoutKind.Sequential, Pack:=1)>
  Private Structure Sample
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=22)>
    Public Name As String
    Public Length As Short
    Public Tune As Byte
    Public Volume As Byte
    Public LoopStart As Short
    Public LoopLength As Short
  End Structure

  <StructLayout(LayoutKind.Sequential, Pack:=1)>
  Private Structure PatternList
    Public Length As Byte
    Public Ignored As Byte
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=128)>
    Public Table As Byte()
    Public Type As UInteger
  End Structure

  <StructLayout(LayoutKind.Sequential, Pack:=1)>
  Private Structure WavHeader
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
    Public RiffHeader As Byte()
    Public WavSize As Integer
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
    Public WaveHeader As Byte()
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
    Public FmtHeader As Byte()
    Public FmtChunkSize As Integer
    Public AudioFormat As Short
    Public NumChannels As Short
    Public SampleRate As Integer
    Public ByteRate As Integer
    Public SampleAlignment As Short
    Public BitDepth As Short
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
    Public DataHeader As Byte()
    Public DataBytes As Integer
    'Public bytes As Byte()
  End Structure

  Private Sub Usage()
    Console.WriteLine("Usage: modextract [-r rate] mod.filename")
  End Sub

  'Private Function Be16ToH(value As Short) As Short
  '  Return ((value And &HFFS) << 8S) Or ((value And &HFF00S) >> 8S)
  'End Function

  Private Function StructureToByteArray(obj As Object) As Byte()
    Dim size = Marshal.SizeOf(obj)
    Dim arr(size - 1) As Byte
    Dim ptr = Marshal.AllocHGlobal(size)
    Marshal.StructureToPtr(obj, ptr, True)
    Marshal.Copy(ptr, arr, 0, size)
    Marshal.FreeHGlobal(ptr)
    Return arr
  End Function

  Private Function MotorolaToIntelShort(buffer As Byte(), offset As Integer) As Short
    Dim sb(1) As Byte
    sb(0) = buffer(offset + 1) : sb(1) = buffer(offset)
    Return BitConverter.ToInt16(sb)
  End Function

  Private Function TrimZ(value As String) As String
    If value.Contains(ChrW(0)) Then
      Return value.Substring(0, value.IndexOf(ChrW(0))).Trim
    Else
      Return value.Trim
    End If
  End Function

  Sub Main(args As String())

    'Dim args As String() = Environment.GetCommandLineArgs()

    'If args.Length < 2 Then
    '  Usage()
    '  Return
    'End If

    'Dim rate As Integer = rate
    'Dim filename As String = ""

    'For i = 1 To args.Length - 1
    '  If args(i) = "-r" AndAlso i < args.Length - 1 Then
    '    Dim result = Integer.TryParse(args(i + 1), rate)
    '    i += 1
    '  Else
    '    filename = args(i)
    '  End If
    'Next

    'Dim rate As Integer = rate
    'Dim module1 = ""

    'For Each arg In args
    '  Dim parts = arg.Split(":"c)
    '  If parts.Length = 2 AndAlso parts(0) = "-r" Then
    '    rate = Integer.Parse(parts(1))
    '  Else
    '    module1 = arg
    '  End If
    'Next

    'If module1 = "" Then
    '  Usage()
    '  Return '-1
    'End If

    'Dim rate = 11025
    'Dim module1 = "d:\mod\twilight zone [2 unlimited].mod"

    'If Not IO.File.Exists(module1) Then
    '  Console.WriteLine($"Error: {New System.ComponentModel.Win32Exception().Message}")
    '  Return '-1
    'End If

    Dim files = IO.Directory.GetFiles("d:\mod", "*.mod")

    For Each module1 In files

      Dim s(30) As Sample
      Dim modname = ""

      Using fs = IO.File.OpenRead(module1)

        Dim modname_buffer(20) As Byte
        fs.Read(modname_buffer, 0, 20)
        modname = TrimZ(Encoding.ASCII.GetString(modname_buffer))
        For Each ch In IO.Path.GetInvalidFileNameChars
          If modname.Contains(ch) Then modname = modname.Replace(ch, "_")
        Next
        modname = modname.Replace("."c, "_"c)
        modname = modname.Replace("{"c, "_"c)
        modname = modname.Replace("}"c, "_"c)

        If String.IsNullOrWhiteSpace(modname) Then modname = "_noname"

        Console.WriteLine($"Module name {modname}")

        For i = 0 To 30
          Dim buffer(29) As Byte : fs.Read(buffer, 0, 30)
          s(i) = New Sample With {.Name = Encoding.ASCII.GetString(buffer, 0, 22).TrimEnd,
                                  .Length = MotorolaToIntelShort(buffer, 22),
                                  .Tune = buffer(22),
                                  .Volume = buffer(23),
                                  .LoopStart = MotorolaToIntelShort(buffer, 24),
                                  .LoopLength = MotorolaToIntelShort(buffer, 26)}
          Console.WriteLine($"Sample {i:D2}: {s(i).Name} ({s(i).Length} words, {s(i).Tune} tune, {s(i).Volume} volume)")
        Next

        Dim pl As New PatternList
        If True Then
          Dim buffer(133) As Byte : fs.Read(buffer, 0, 134)
          pl.Length = buffer(0)
          pl.Ignored = buffer(1)
          ReDim pl.Table(133)
          For index = 0 To 127
            pl.Table(index) = buffer(index + 2)
          Next
          pl.Type = BitConverter.ToUInt32(buffer, 130)
        End If

        Select Case pl.Type
          Case MOD_SOUNDTRACKER
            Console.WriteLine("Type Soundtracker")
          Case Else
            Console.WriteLine($"Type Unknown (0x{pl.Type:x8})")
            Console.WriteLine("Can't extract this type of module. Aborting.")
            Continue For 'Return '-1
        End Select

        Dim maxpat As Integer = 0

        For i = 0 To pl.Length - 1
          If pl.Table(i) > maxpat Then maxpat = pl.Table(i)
        Next

        Dim pattern(1023) As Byte

        For i = 0 To maxpat
          fs.Read(pattern, 0, 1024)
        Next

        Console.WriteLine("Samples start at {0}", fs.Seek(0, IO.SeekOrigin.Current))

        Dim header As WavHeader
        header.RiffHeader = Encoding.ASCII.GetBytes("RIFF")
        header.WaveHeader = Encoding.ASCII.GetBytes("WAVE")
        header.FmtHeader = Encoding.ASCII.GetBytes("fmt ")
        header.DataHeader = Encoding.ASCII.GetBytes("data")
        header.FmtChunkSize = 16
        header.AudioFormat = 1
        header.NumChannels = 1
        header.SampleRate = rate
        header.ByteRate = header.SampleRate * 2
        header.SampleAlignment = 2
        header.BitDepth = 16

        IO.Directory.CreateDirectory("samples")
        IO.Directory.CreateDirectory(IO.Path.Combine("samples", modname))

        For i = 0 To 30

          If s(i).Length = 0 Then Continue For

          Dim sampleName = TrimZ(s(i).Name)

          For Each ch In IO.Path.GetInvalidFileNameChars
            If sampleName.Contains(ch) Then sampleName = sampleName.Replace(ch, "_")
          Next
          sampleName = sampleName.Replace("."c, "_"c)
          sampleName = sampleName.Replace("{"c, "_"c)
          sampleName = sampleName.Replace("}"c, "_"c)

          Dim filename = String.Format($"samples/{modname}/{i:D2} - {sampleName}.wav")
          Console.WriteLine($"Saving: {filename}")

          ' Size of the wav portion of the file, which follows the first 8 bytes. File size - 8
          header.WavSize = s(i).Length * 4 + Marshal.SizeOf(GetType(WavHeader)) - 8
          ' Number of bytes in data. Number of samples * num_channels * sample byte size
          header.DataBytes = s(i).Length * 4

          Using fd2 = IO.File.Open(IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), IO.FileMode.OpenOrCreate, IO.FileAccess.Write, IO.FileShare.ReadWrite)

            Dim headerBytes = StructureToByteArray(header)
            fd2.Write(headerBytes, 0, headerBytes.Length)

            For j = 0 To s(i).Length * 2 - 1
              Dim value = fs.ReadByte()
              If value = -1 Then Exit For
              Dim b = CByte(value)
              Dim v8 = CSByte(If(b > 127, b - 256, b))
              Dim v16 = v8 * 256S
              fd2.Write(BitConverter.GetBytes(v16))
            Next

            fd2.Flush() : fd2.Close()

          End Using

          filename = String.Format($"samples/{modname}/{i:D2} - {sampleName}.txt")

          Using f As New IO.StreamWriter(IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename))
            f.WriteLine("Sample name: {0}", TrimZ(s(i).Name))
            f.WriteLine("Sample length: {0} samples", s(i).Length * 2)
            If s(i).LoopLength > 1 Then
              f.WriteLine("Loop start: {0}", s(i).LoopStart * 2)
              f.WriteLine("Loop length: {0}", s(i).LoopLength * 2)
              f.WriteLine("Loop end: {0}", s(i).LoopStart * 2 + s(i).LoopLength * 2)
            End If
            f.Close()
          End Using

        Next

      End Using

    Next

  End Sub

End Module