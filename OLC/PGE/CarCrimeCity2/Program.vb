Module Program

  Sub Main()

    ' Load the settings singleton
    If Not Config.LoadConfigFile("assets/config.lua") Then
      Console.WriteLine("Failed to load '/assets/config.lua'")
      Console.WriteLine("  -> Using default configuration")
    End If

    ' Start the PixelGameEngine
    Dim game As New cCarCrimeCity()
    If game.Construct(Config.nScreenWidth, Config.nScreenHeight, Config.nPixelWidth, Config.nPixelHeight, Config.bFullScreen) Then
      game.Start()
    End If

  End Sub

End Module