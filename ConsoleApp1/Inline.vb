Option Explicit On
Option Strict On
Option Infer On

Imports System.Runtime.CompilerServices

Namespace Global.Community.VisualBasic.Inline

  <HideModuleNameAttribute()>
  Public Module Inline

    ' Assign

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Assign(Of T)(ByRef value As T, replacement As T) As T
      value = replacement : Return value
    End Function

    ' Byte

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Byte) As Byte
      value = CByte(value + 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Byte) As Byte
      IncrAfter = value
      value = CByte(value + 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Byte) As Byte
      value = CByte(value - 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Byte) As Byte
      DecrAfter = value
      value = CByte(value - 1)
    End Function

    ' SByte

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As SByte) As SByte
      value = CSByte(value + 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As SByte) As SByte
      IncrAfter = value
      value = CSByte(value + 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As SByte) As SByte
      value = CSByte(value - 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As SByte) As SByte
      DecrAfter = value
      value = CSByte(value - 1)
    End Function

    ' Char

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Char) As Char
      value = ChrW(AscW(value) + 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Char) As Char
      IncrAfter = value
      value = ChrW(AscW(value) + 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Char) As Char
      value = ChrW(AscW(value) - 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Char) As Char
      DecrAfter = value
      value = ChrW(AscW(value) - 1)
    End Function

    ' Short

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Short) As Short
      value = CShort(value + 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Short) As Short
      IncrAfter = value
      value = CShort(value + 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Short) As Short
      value = CShort(value - 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Short) As Short
      DecrAfter = value
      value = CShort(value - 1)
    End Function

    ' UShort

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As UShort) As UShort
      value = CUShort(value + 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As UShort) As UShort
      IncrAfter = value
      value = CUShort(value + 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As UShort) As UShort
      value = CUShort(value - 1)
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As UShort) As UShort
      DecrAfter = value
      value = CUShort(value - 1)
    End Function

    ' Integer

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Integer) As Integer
      Return Threading.Interlocked.Increment(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Integer) As Integer
      Return Math.Min(Threading.Interlocked.Increment(value), value - 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Integer) As Integer
      Return Threading.Interlocked.Decrement(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Integer) As Integer
      Return Math.Max(Threading.Interlocked.Decrement(value), value + 1)
    End Function

    ' UInteger

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As UInteger) As UInteger
      Return Threading.Interlocked.Increment(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As UInteger) As UInteger
      Return CUInt(Math.Min(Threading.Interlocked.Increment(value), value - 1))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As UInteger) As UInteger
      Return Threading.Interlocked.Decrement(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As UInteger) As UInteger
      Return CUInt(Math.Max(Threading.Interlocked.Decrement(value), value + 1))
    End Function

    ' Long

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Long) As Long
      Return Threading.Interlocked.Increment(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Long) As Long
      Return Math.Min(Threading.Interlocked.Increment(value), value - 1)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Long) As Long
      Return Threading.Interlocked.Decrement(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Long) As Long
      Return Math.Max(Threading.Interlocked.Decrement(value), value + 1)
    End Function

    ' ULong

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As ULong) As ULong
      Return Threading.Interlocked.Increment(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As ULong) As ULong
      Return CULng(Math.Min(Threading.Interlocked.Increment(value), value - 1))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As ULong) As ULong
      Return Threading.Interlocked.Decrement(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As ULong) As ULong
      Return CULng(Math.Max(Threading.Interlocked.Decrement(value), value + 1))
    End Function

    ' Single

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Single) As Single
      value += 1
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Single) As Single
      IncrAfter = value
      value += 1
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Single) As Single
      value -= 1
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Single) As Single
      DecrAfter = value
      value -= 1
    End Function

    ' Double

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrBefore(ByRef value As Double) As Double
      value += 1
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function IncrAfter(ByRef value As Double) As Double
      IncrAfter = value
      value += 1
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrBefore(ByRef value As Double) As Double
      value -= 1
      Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DecrAfter(ByRef value As Double) As Double
      DecrAfter = value
      value -= 1
    End Function

  End Module

End Namespace