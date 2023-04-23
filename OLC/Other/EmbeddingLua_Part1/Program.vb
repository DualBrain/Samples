' Inspired by "Embedding Lua in C++ #1" -- @javidx9
' https://youtu.be/4l5HdmPoynw

' NOTE: Uses NLua (via nuget package) instead of raw Lua C libraries; so major differences in approach
'       but achieves the same results. This also means that there is a slight difference in the version
'       of Lua supported, but thusfar hasn't presented as being any sort of issue.

Option Explicit On
Option Strict On
Option Infer On

Imports NLua
Imports NLua.Exceptions

Module Program

  Private Structure Player
    Public Title As String
    Public Name As String
    Public Family As String
    Public Level As Integer
  End Structure

  Private m_player As Player

  Sub Main()

    Using lua = New Lua

      lua.State.OpenLibs()
      lua.RegisterFunction("HostFunction", Nothing, GetType(Program).GetMethod("LuaHostFunction"))

      Try
        Dim result = lua.DoFile("VideoExample.lua")

        Dim f = TryCast(lua("DoAThing"), LuaFunction)
        If f IsNot Nothing Then

          Dim res = f.Call(5.0F, 6.0F).First

        End If

        'Dim f = TryCast(lua("GetPlayer"), LuaFunction)
        'If f IsNot Nothing Then

        '  Dim res = f.Call(1).First

        '  Dim t = TryCast(res, LuaTable)
        '  If t IsNot Nothing Then
        '    m_player.Title = CStr(t.Item("Title"))
        '    m_player.Name = CStr(t.Item("Name"))
        '    m_player.Family = CStr(t.Item("Family"))
        '    m_player.Level = CInt(t.Item("Level"))
        '  End If

        '  Console.WriteLine($"{m_player.Title} {m_player.Name} of {m_player.Family} [Lvl: {m_player.Level}]")

        'End If

        'Dim f = TryCast(lua("AddStuff"), LuaFunction)
        'If f IsNot Nothing Then
        '  Dim res = CSng(f.Call(3.5F, 7.1F).First)
        '  Console.WriteLine($"[VB] Called in Lua 'AddStuff(3.5F, 7.1F)', got {res}")
        'End If

        'If IsString(lua("PlayerTitle")) Then m_player.Title = lua.GetString("PlayerTitle")
        'If IsString(lua("PlayerName")) Then m_player.Name = lua.GetString("PlayerName")
        'If IsString(lua("PlayerFamily")) Then m_player.Family = lua.GetString("PlayerFamily")
        'If IsNumber(lua("PlayerLevel")) Then m_player.Level = CInt(Fix(lua.GetNumber("PlayerLevel")))

        'Dim t = TryCast(lua("player"), LuaTable)
        'If t IsNot Nothing Then
        '  m_player.Title = CStr(t.Item("Title"))
        '  m_player.Name = CStr(t.Item("Name"))
        '  m_player.Family = CStr(t.Item("Family"))
        '  m_player.Level = CInt(t.Item("Level"))
        'End If

        'Console.WriteLine($"{m_player.Title} {m_player.Name} of {m_player.Family} [Lvl: {m_player.Level}]")

      Catch ex As LuaScriptException
        Console.WriteLine(ex.Message)
      End Try

      lua.Close()

    End Using

  End Sub

  Public Function LuaHostFunction(a As Single, b As Single) As Single
    Dim c = a * b
    Console.WriteLine($"[VB] HostFunction({a}, {b}) called, got {c}")
    Return c
  End Function

End Module