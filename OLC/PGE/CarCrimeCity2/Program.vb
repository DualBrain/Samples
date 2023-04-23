Module Program

  Sub Main()

    ' Load the settings singleton
    Dim config = New cGameSettings
    If Not config.LoadConfigFile("assets/config.lua") Then
      Console.WriteLine("Failed to load '/assets/config.lua'")
      Console.WriteLine("  -> Using default configuration")
    End If

    ' Start the PixelGameEngine
    Dim game As New cCarCrimeCity()
    If game.Construct(cGameSettings.nScreenWidth, cGameSettings.nScreenHeight, cGameSettings.nPixelWidth, cGameSettings.nPixelHeight, cGameSettings.bFullScreen) Then
      game.Start()
    End If

  End Sub

End Module