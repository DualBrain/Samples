Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Graphics

Module Program

  <STAThread>
  Sub Main() 'args As String())
    Using g = New FNAGame()
      g.Run()
    End Using
  End Sub

End Module

Class FNAGame
  Inherits Game

  Private m_keyboardPrev As New KeyboardState
  Private m_mousePrev As New MouseState
  Private m_gpPrev As New GamePadState

  Private m_batch As SpriteBatch
  Private m_texture As Texture2D

  Private m_gdm As GraphicsDeviceManager

  Friend Sub New()

    m_gdm = New GraphicsDeviceManager(Me)

    ' Typically you would load a config here...

    m_gdm.PreferredBackBufferWidth = 1280
    m_gdm.PreferredBackBufferHeight = 720
    m_gdm.IsFullScreen = False
    'gdm.SynchronizeWithVerticalRetrace = True
    m_gdm.ApplyChanges()

    ' All content loaded will be in a "Content" folder
    Content.RootDirectory = "Content"

  End Sub

  Protected Overrides Sub Initialize()
    ' This Is a nice place to start up the engine, after
    ' loading configuration stuff in the constructor
    MyBase.Initialize()
  End Sub

  Protected Overrides Sub LoadContent()
    ' Load textures, sounds, And so on in here...
    'MyBase.LoadContent()

    ' Create the batch...
    m_batch = New SpriteBatch(GraphicsDevice)

    ' ... then load a texture from ./Content/FNATexture.png
    m_texture = Content.Load(Of Texture2D)("FNATexture")

  End Sub

  Protected Overrides Sub UnloadContent()
    ' Clean up after yourself!
    'MyBase.UnloadContent()

    m_batch.Dispose()
    m_texture.Dispose()

  End Sub

  Protected Overrides Sub Update(gameTime As GameTime)

    ' Poll input
    Dim keyboardCur = Keyboard.GetState()
    Dim mouseCur = Mouse.GetState()
    Dim gpCur = GamePad.GetState(PlayerIndex.One)

    ' Check for presses
    If keyboardCur.IsKeyDown(Keys.Space) AndAlso
       m_keyboardPrev.IsKeyUp(Keys.Space) Then
      System.Console.WriteLine("Space bar was pressed!")
    End If
    If keyboardCur.IsKeyDown(Keys.Escape) AndAlso
       m_keyboardPrev.IsKeyUp(Keys.Escape) Then
      System.Console.WriteLine("Escape was pressed!")
      Me.Exit()
    End If
    If keyboardCur.IsKeyDown(Keys.F11) AndAlso
       m_keyboardPrev.IsKeyUp(Keys.F11) Then
      m_gdm.ToggleFullScreen()
    End If

    If mouseCur.RightButton = ButtonState.Released AndAlso m_mousePrev.RightButton = ButtonState.Pressed Then
      System.Console.WriteLine("Right mouse button was released!")
    End If
    If gpCur.Buttons.A = ButtonState.Pressed AndAlso m_gpPrev.Buttons.A = ButtonState.Pressed Then
      System.Console.WriteLine("A button is being held!")
    End If

    ' Current is now previous!
    m_keyboardPrev = keyboardCur
    m_mousePrev = mouseCur
    m_gpPrev = gpCur

    ' Run game logic in here. Do Not render anything here!
    MyBase.Update(gameTime)

  End Sub

  Protected Overrides Sub Draw(gameTime As GameTime)
    ' Render stuff in here. Do Not run game logic in here!
    'GraphicsDevice.Clear(Color.CornflowerBlue)
    'MyBase.Draw(gameTime)

    GraphicsDevice.Clear(Color.CornflowerBlue)

    ' Draw the texture to the corner of the screen
    m_batch.Begin()
    m_batch.Draw(m_texture, Vector2.Zero, Color.White)
    m_batch.End()

    MyBase.Draw(gameTime)

  End Sub

End Class