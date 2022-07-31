' Copyright (c) Cory Smith. All rights reserved.
' Licensed under the MIT license.

Imports Microsoft.ClearScript
Imports Microsoft.ClearScript.Windows.Core

Module VbScriptRepl

  Public Sub Main() 'args As String())

    Using engine = New VBScriptEngine(NameOf(VbScriptRepl), Windows.WindowsScriptEngineFlags.EnableDebugging, NullSyncInvoker.Instance)

      engine.AddHostType("Api", GetType(Api))
      engine.AddHostObject("Host", New ExtendedHostFunctions())
      engine.AddHostObject("Lib", HostItemFlags.GlobalMembers, New HostTypeCollection("mscorlib", "System", "System.Core", "System.Numerics", "ClearScript.Core", "ClearScript.Windows.Core"))
      'engine.SuppressExtensionMethodEnumerication = True
      engine.AllowReflection = True

      Initialize(engine)
      Execute(engine)

    End Using

  End Sub

  Private Sub Initialize(engine As ScriptEngine)
    Try
      Dim filename = IO.Path.ChangeExtension("Startup", engine.FileNameExtension)
      Dim path = IO.Path.Combine(IO.Path.GetDirectoryName(Environment.ProcessPath), filename)
      If IO.File.Exists(path) Then
        engine.Execute(New DocumentInfo(New Uri(path)), IO.File.ReadAllText(path))
      End If
    Catch ex As Exception
      Console.WriteLine($"Error: {ex.GetBaseException.Message}")
    End Try
  End Sub

  Private Sub Execute(engine As ScriptEngine)
    Do
      Console.Write("> ")
      Dim command = Console.ReadLine()
      If String.Compare(command, "exit", True) = 0 Then Exit Do
      If String.IsNullOrWhiteSpace(command) Then Continue Do
      If command.StartsWith("?") Then command = command.Replace("?", "api.print")
      Try
        Dim result = engine.ExecuteCommand(command)
        If result IsNot Nothing Then Console.WriteLine(result)
      Catch ex As Exception
        Console.WriteLine($"Error {ex.GetBaseException.Message}")
      End Try
    Loop
  End Sub

End Module

Public NotInheritable Class Api

  Private Sub New()
  End Sub

  Public Shared Sub Print(o As Object)
    Console.WriteLine(o)
  End Sub

  Public Shared Sub Print(value As Double)
    Console.WriteLine(value)
  End Sub

  Public Shared Sub Print(value As String)
    Console.WriteLine(value)
  End Sub

End Class