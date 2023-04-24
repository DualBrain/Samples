Option Explicit On
Option Strict On
Option Infer On

' This is a singleton that stores all the game's configuration settings.
' These settings are loaded on game start up and are to be considered read-only.
Public Class sAssetModel
  Public sCreator As String
  Public sDescription As String
  Public sModelOBJ As String
  Public Property sModelPNG As String
  Public fRotate(2) As Single
  Public fScale(2) As Single
  Public fTranslate(2) As Single
End Class

Public Class sAssetTexture
  Public sName As String
  Public sFile As String
End Class

Public Class cGameSettings

  Public Shared Property nScreenWidth As Integer = 768
  Public Shared Property nScreenHeight As Integer = 480
  Public Shared Property nPixelWidth As Integer = 2
  Public Shared Property nPixelHeight As Integer = 2
  Public Shared Property bFullScreen As Boolean = False

  Public Shared Property nDefaultMapWidth As Integer = 64
  Public Shared Property nDefaultMapHeight As Integer = 32
  Public Shared Property sDefaultCityFile As String = ""

  Public Shared Property vecAssetTextures As New List(Of sAssetTexture)()
  Public Shared Property vecAssetBuildings As New List(Of sAssetModel)()
  Public Shared Property vecAssetVehicles As New List(Of sAssetModel)()

  Public Sub New()
  End Sub

  Public Function LoadConfigFile(sFile As String) As Boolean

    Dim lua = New NLua.Lua
    lua.State.OpenLibs()

    If Not IO.File.Exists(sFile) Then
      Console.WriteLine($"'{sFile}' missing")
      Return False
    End If

    Try
      Dim luaResults = lua.DoFile(sFile)
    Catch ex As NLua.Exceptions.LuaScriptException
      Console.WriteLine(ex.Message)
      Return False
    End Try

    Dim L = lua.State

    L.GetGlobal("PixelWidth") : If L.IsInteger(-1) Then cGameSettings.nPixelWidth = CInt(L.ToInteger(-1))
    L.GetGlobal("PixelHeight") : If L.IsInteger(-1) Then cGameSettings.nPixelHeight = CInt(L.ToInteger(-1))
    L.GetGlobal("ScreenWidth") : If L.IsInteger(-1) Then cGameSettings.nScreenWidth = CInt(L.ToInteger(-1))
    L.GetGlobal("ScreenHeight") : If L.IsInteger(-1) Then cGameSettings.nScreenHeight = CInt(L.ToInteger(-1))
    L.GetGlobal("DefaultMapWidth") : If L.IsInteger(-1) Then cGameSettings.nDefaultMapWidth = CInt(L.ToInteger(-1))
    L.GetGlobal("DefaultMapHeight") : If L.IsInteger(-1) Then cGameSettings.nDefaultMapHeight = CInt(L.ToInteger(-1))
    L.GetGlobal("DefaultCityFile") : If L.IsString(-1) Then cGameSettings.sDefaultCityFile = L.ToString(-1)
    L.GetGlobal("FullScreen") : If L.IsBoolean(-1) Then cGameSettings.bFullScreen = L.ToBoolean(-1)

    ' Load System Texture files

    ' Load Texture Assets
    L.GetGlobal("Textures") ' -1 Table "Teams"
    If L.IsTable(-1) Then
      L.PushNil() ' -2 Key Nil : -1 Table "Teams"

      While L.Next(-2)  ' -1 Table : -2 Key "TeamName" : -3 Table "Teams"
        Dim texture As sAssetTexture = New sAssetTexture()
        Dim stage As Integer = 0
        If L.IsTable(-1) Then
          L.GetTable(-1) ' -1 Table : -2 Table Value : -3 Key "TeamName" : -4 Table "Teams" 
          L.PushNil() ' -1 Key Nil : -2 Table : -3 Table Value : -4 Key "TeamName" : -5 Table "Teams" 
          While L.Next(-2)  ' -1 Value "BotFile" : -2 Key Nil : -3 Table : -4 Table Value : -5 Key "TeamName" : -6 Table "Teams" 
            If stage = 0 Then
              texture.sName = L.ToString(-1)
            End If
            If stage = 1 Then
              texture.sFile = L.ToString(-1)
            End If
            L.Pop(1) ' -1 Key Nil : -2 Table : -3 Table Value : -4 Key "TeamName" : -5 Table "Teams"
            stage += 1
          End While
        End If
        L.Pop(1) ' -1 Table : -2 Table Value : -3 Key "TeamName" : -4 Table "Teams"
        vecAssetTextures.Add(texture)
      End While
    End If

    Dim GroupLoadAssets = Sub(group As String, vec As List(Of sAssetModel))
                            L.GetGlobal(group)
                            If L.IsTable(-1) Then
                              L.PushNil()
                              While L.Next(-2)
                                Dim model As New sAssetModel()
                                Dim stage = 0
                                If L.IsTable(-1) Then
                                  L.GetTable(-1)
                                  L.PushNil()
                                  While L.Next(-2)
                                    If stage = 0 Then model.sCreator = L.ToString(-1)
                                    If stage = 1 Then model.sDescription = L.ToString(-1)
                                    If stage = 2 Then model.sModelOBJ = L.ToString(-1)
                                    If stage = 3 Then model.sModelPNG = L.ToString(-1)
                                    If stage = 4 Then model.fRotate(0) = CSng(L.ToNumber(-1))
                                    If stage = 5 Then model.fRotate(1) = CSng(L.ToNumber(-1))
                                    If stage = 6 Then model.fRotate(2) = CSng(L.ToNumber(-1))
                                    If stage = 7 Then model.fScale(0) = CSng(L.ToNumber(-1))
                                    If stage = 8 Then model.fScale(1) = CSng(L.ToNumber(-1))
                                    If stage = 9 Then model.fScale(2) = CSng(L.ToNumber(-1))
                                    If stage = 10 Then model.fTranslate(0) = CSng(L.ToNumber(-1))
                                    If stage = 11 Then model.fTranslate(1) = CSng(L.ToNumber(-1))
                                    If stage = 12 Then model.fTranslate(2) = CSng(L.ToNumber(-1))
                                    L.Pop(1)
                                    stage += 1
                                  End While
                                End If
                                L.Pop(1)
                                vec.Add(model)
                              End While
                            End If
                          End Sub

    ' Load Building Assets
    GroupLoadAssets("Buildings", vecAssetBuildings)

    ' Load Vehicle Assets
    GroupLoadAssets("Vehicles", vecAssetVehicles)

    lua.Close()

    Return True

  End Function

End Class