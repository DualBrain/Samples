Option Explicit On
Option Strict On
Option Infer On

Imports System.IO
Imports System.Runtime.InteropServices

'=============================================================
' Resource Packs - Allows you to store files in one large 
' scrambled file - Thanks MaGetzUb for debugging a null char in std:stringstream bug
Public Class ResourcePack

  Private Structure sResourceFile
    Public nSize As Integer
    Public nOffset As Integer
  End Structure

  Private baseFile As IO.FileStream
  Private mapFiles As New Dictionary(Of String, sResourceFile)

  Public Sub New()
  End Sub

  'Protected Overrides Sub Finalize()
  '  MyBase.Finalize()
  '  baseFile.Close()
  'End Sub

  Public Function AddFile(sFile As String) As Boolean

    Dim file As String = MakePosix(sFile)

    If IO.File.Exists(file) Then
      Dim e As New sResourceFile()
      Dim fileInfo As New IO.FileInfo(file)
      e.nSize = CInt(fileInfo.Length)
      e.nOffset = 0 ' Unknown at this stage
      mapFiles(file) = e
      Return True
    End If

    Return False

  End Function

  Public Function LoadPack(sFile As String, sKey As String) As Boolean

    ' Open the resource file
    baseFile = New IO.FileStream(sFile, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
    If Not baseFile.CanRead Then Return False

    ' 1) Read Scrambled index
    Dim nIndexSize As Integer = 0
    baseFile.Read(BitConverter.GetBytes(nIndexSize), 0, 4)

    Dim buffer(nIndexSize - 1) As Byte
    baseFile.Read(buffer, 0, nIndexSize)

    Dim decoded = Scramble(buffer, sKey)
    Dim pos As Integer = 0
    Dim read = Sub(dst As Byte(), size As Integer)
                 Array.Copy(decoded, pos, dst, 0, size)
                 pos += size
               End Sub

    Dim [get] = Function() As Integer
                  Dim c = New Byte(0) {}
                  read(c, 1)
                  Return c(0)
                End Function

    ' 2) Read Map
    Dim nMapEntries As Integer = 0
    read(BitConverter.GetBytes(nMapEntries), 4)
    For i As Integer = 0 To nMapEntries - 1
      Dim nFilePathSize As Integer = 0
      read(BitConverter.GetBytes(nFilePathSize), 4)

      Dim sFileName As String = ""
      For j As Integer = 0 To nFilePathSize - 1
        sFileName &= ChrW([get]())
      Next

      Dim e As New sResourceFile()
      read(BitConverter.GetBytes(e.nSize), 4)
      read(BitConverter.GetBytes(e.nOffset), 4)
      mapFiles(sFileName) = e
    Next

    ' Don't close base file! we will provide a stream
    ' pointer when the file is requested
    Return True

  End Function

  Public Function SavePack(sFile As String, sKey As String) As Boolean
    ' Create/Overwrite the resource file
    Dim ofs As New System.IO.FileStream(sFile, IO.FileMode.Create)
    If Not ofs.CanWrite Then Return False
    ' Iterate through map
    Dim nIndexSize As Integer = 0 ' Unknown for now
    Dim nIndexSizeBytes As Byte() = BitConverter.GetBytes(nIndexSize)
    ofs.Write(nIndexSizeBytes, 0, nIndexSizeBytes.Length)

    Dim nMapSize As Integer = CInt(mapFiles.Count)
    Dim nMapSizeBytes As Byte() = BitConverter.GetBytes(nMapSize)
    ofs.Write(nMapSizeBytes, 0, nMapSizeBytes.Length)

    For Each kvp As KeyValuePair(Of String, sResourceFile) In mapFiles
      Dim e As sResourceFile = kvp.Value

      ' Write the path of the file
      Dim nPathSize As Integer = CInt(kvp.Key.Length)
      Dim nPathSizeBytes As Byte() = BitConverter.GetBytes(nPathSize)
      ofs.Write(nPathSizeBytes, 0, nPathSizeBytes.Length)
      Dim pathBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(kvp.Key)
      ofs.Write(pathBytes, 0, pathBytes.Length)

      ' Write the file entry properties
      Dim nSizeBytes As Byte() = BitConverter.GetBytes(e.nSize)
      ofs.Write(nSizeBytes, 0, nSizeBytes.Length)

      Dim nOffsetBytes As Byte() = BitConverter.GetBytes(e.nOffset)
      ofs.Write(nOffsetBytes, 0, nOffsetBytes.Length)
    Next

    ' 2) Write the individual Data
    Dim offset As Long = ofs.Position
    nIndexSize = CInt(offset)
    For Each kvp As KeyValuePair(Of String, sResourceFile) In mapFiles
      Dim e As sResourceFile = kvp.Value

      ' Store beginning of file offset within resource pack file
      e.nOffset = CInt(offset)

      ' Load the file to be added
      Dim vBuffer(e.nSize - 1) As Byte
      Using i As New System.IO.FileStream(kvp.Key, IO.FileMode.Open)
        i.Read(vBuffer, 0, e.nSize)
      End Using

      ' Write the loaded file into resource pack file
      ofs.Write(vBuffer, 0, e.nSize)
      offset += e.nSize
    Next

    ' 3) Scramble Index
    Dim stream As New List(Of Byte)()
    Dim write As Action(Of Byte(), Integer) = Sub(data, size)
                                                Dim sizeNow As Integer = stream.Count()
                                                stream.AddRange(data.Take(size))
                                              End Sub

    ' Iterate through map
    Dim nMapSizeBytes2 As Byte() = BitConverter.GetBytes(nMapSize)
    write(nMapSizeBytes2, Marshal.SizeOf(GetType(Integer)))
    For Each kvp As KeyValuePair(Of String, sResourceFile) In mapFiles
      Dim e As sResourceFile = kvp.Value

      ' Write the path of the file
      Dim nPathSizeBytes2 As Byte() = BitConverter.GetBytes(kvp.Key.Length)
      write(nPathSizeBytes2, Marshal.SizeOf(GetType(Integer)))
      Dim pathBytes2 As Byte() = System.Text.Encoding.ASCII.GetBytes(kvp.Key)
      write(pathBytes2, kvp.Key.Length)

      ' Write the file entry properties
      Dim nSizeBytes2 As Byte() = BitConverter.GetBytes(e.nSize)
      write(nSizeBytes2, Marshal.SizeOf(GetType(Integer)))

      Dim nOffsetBytes2 As Byte() = BitConverter.GetBytes(e.nOffset)
      write(nOffsetBytes2, Marshal.SizeOf(GetType(Integer)))
    Next

    Dim sIndexString As Byte() = Scramble(stream.ToArray, sKey)
    Dim nIndexStringLen As Integer = CInt(sIndexString.Count)
    ' 4) Rewrite Map (it has been updated with offsets now)
    ' at start of file
    ofs.Seek(0, SeekOrigin.Begin)
    ofs.Write(BitConverter.GetBytes(nIndexStringLen), 0, Marshal.SizeOf(GetType(Integer)))
    ofs.Write(sIndexString.ToArray(), 0, sIndexString.Count)
    ofs.Close()
    Return True

  End Function

  Friend Function GetFileBuffer(sFile As String) As ResourceBuffer
    Dim e As sResourceFile = mapFiles(sFile)
    Return New ResourceBuffer(baseFile, e.nOffset, e.nSize)
  End Function

  Public Function Loaded() As Boolean
    Return baseFile IsNot Nothing
  End Function

  Private Shared Function Scramble(data As Byte(), key As String) As Byte()
    If String.IsNullOrEmpty(key) Then Return data
    Dim o As New List(Of Byte)
    Dim c As Integer = 0
    For Each s In data
      o.Add(s Xor CByte(AscW(key(c Mod key.Length))))
      c += 1
    Next
    Return o.ToArray
  End Function

  Private Shared Function MakePosix(path As String) As String
    Dim o As String = ""
    For Each s As Char In path
      o += If(s = "\", "/", s)
    Next
    Return o
  End Function

End Class