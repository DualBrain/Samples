Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine

Friend Class Dynamic

  Friend Property Px As Single
  Friend Property Py As Single
  Friend Property Vx As Single
  Friend Property Vy As Single

  Friend Property SolidVsMap As Boolean
  Friend Property SolidVsDyn As Boolean
  Friend Property Friendly As Boolean
  Friend Property Redundant As Boolean
  Friend Property IsProjectile As Boolean
  Friend Property IsAttackable As Boolean = True
  Friend Property Name As String

  Friend Shared g_engine As RPG_Engine

  Friend Sub New(name As String)
    Me.Name = name
    Px = 0.0F
    Py = 0.0F
    Vx = 0.0F
    Vy = 0.0F
    SolidVsMap = True
    SolidVsDyn = True
    Friendly = True
    Redundant = False
    IsAttackable = False
    IsProjectile = False
  End Sub

  Friend Overridable Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
    ' Do nothing?
  End Sub

  Friend Overridable Sub Update(elapsedTime As Single, Optional player As Dynamic = Nothing)
    ' Do nothing?
  End Sub

  Friend Overridable Sub OnInteract(Optional player As Dynamic = Nothing)
    ' Do nothing?
  End Sub

End Class

Friend Class Dynamic_Creature
  Inherits Dynamic

  Friend Enum FacingDirection
    SOUTH = 0
    WEST = 1
    NORTH = 2
    EAST = 3
  End Enum

  Friend Enum GraphicState
    STANDING
    WALKING
    CELEBRATING
    DEAD
  End Enum

  Friend Property Health As Integer = 10
  Friend Property HealthMax As Integer = 10
  Friend Property Controllable As Boolean = True

  Protected m_stateTick As Single
  Protected m_equipedWeapon As Weapon = Nothing

  Private ReadOnly m_pSprite As Sprite = Nothing
  Private m_nGraphicCounter As Integer = 0
  Private m_fTimer As Single = 0.0F
  Private m_nFacingDirection As FacingDirection = FacingDirection.SOUTH
  Private m_nGraphicState As GraphicState = GraphicState.STANDING
  Private m_knockBackTimer As Single = 0.0F
  Private m_knockBackDX As Single = 0.0F
  Private m_knockBackDY As Single = 0.0F

  Friend Sub New(name As String, sprite As Sprite)
    MyBase.New(name)
    m_pSprite = sprite
    Health = 10
    HealthMax = 10
    m_nFacingDirection = FacingDirection.SOUTH
    m_nGraphicState = GraphicState.STANDING
    m_nGraphicCounter = 0
    m_fTimer = 0.0F
    IsAttackable = True
  End Sub

  Friend Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)

    Dim sheetOffsetX = 0
    Dim sheetOffsetY = 0

    Select Case m_nGraphicState
      Case GraphicState.STANDING : sheetOffsetX = m_nFacingDirection * 16
      Case GraphicState.WALKING : sheetOffsetX = m_nFacingDirection * 16 : sheetOffsetY = m_nGraphicCounter * 16
      Case GraphicState.CELEBRATING : sheetOffsetX = 4 * 16
      Case GraphicState.DEAD : sheetOffsetX = 4 * 16 : sheetOffsetY = 1 * 16
    End Select

    gfx.DrawPartialSprite((Px - ox) * 16.0F, (Py - oy) * 16.0F, m_pSprite, sheetOffsetX, sheetOffsetY, 16, 16)

  End Sub

  Friend Overrides Sub Update(elapsedTime As Single, Optional player As Dynamic = Nothing)

    If m_knockBackTimer > 0.0F Then
      Vx = m_knockBackDX * 10.0F
      Vy = m_knockBackDY * 10.0F
      IsAttackable = False
      m_knockBackTimer -= elapsedTime
      If m_knockBackTimer <= 0.0F Then
        m_stateTick = 0.0F
        Controllable = True
        IsAttackable = True
      End If
    Else
      SolidVsDyn = True
      m_fTimer += elapsedTime
      If m_fTimer >= 0.2F Then
        m_fTimer -= 0.2F
        m_nGraphicCounter += 1
        m_nGraphicCounter = m_nGraphicCounter Mod 2
      End If
      If Math.Abs(Vx) > 0 OrElse Math.Abs(Vy) > 0 Then m_nGraphicState = GraphicState.WALKING Else m_nGraphicState = GraphicState.STANDING
      If Health <= 0 Then m_nGraphicState = GraphicState.DEAD
      If Vx < 0.0F Then m_nFacingDirection = FacingDirection.WEST
      If Vx > 0.0F Then m_nFacingDirection = FacingDirection.EAST
      If Vy < -0.0F Then m_nFacingDirection = FacingDirection.NORTH
      If Vy > 0.0F Then m_nFacingDirection = FacingDirection.SOUTH
      Behaviour(elapsedTime, player)
    End If

  End Sub

  Friend Overridable Sub Behaviour(elapsedTime As Single, Optional player As Dynamic = Nothing)
    ' No default behaviour
  End Sub

  Friend Function GetFacingDirection() As Integer
    Return m_nFacingDirection
  End Function

  Friend Overridable Sub PerformAttack()
  End Sub

  Friend Sub KnockBack(dx As Single, dy As Single, dist As Single)
    m_knockBackDX = dx
    m_knockBackDY = dy
    m_knockBackTimer = dist
    SolidVsDyn = False
    Controllable = False
    IsAttackable = False
  End Sub

End Class

Friend Class Dynamic_Creature_Skelly
  Inherits Dynamic_Creature

  Friend Sub New()
    MyBase.New("Skelly", RPG_Assets.Get.GetSprite("skelly"))
    Friendly = False
    Health = 10
    HealthMax = 10
    m_stateTick = 2.0F
    m_equipedWeapon = DirectCast(RPG_Assets.Get().GetItem("Basic Sword"), Weapon)
  End Sub

  Friend Overrides Sub Behaviour(elapsedTime As Single, Optional player As Dynamic = Nothing)

    If Health <= 0 Then
      Vx = 0
      Vy = 0
      SolidVsDyn = False
      IsAttackable = False
      Return
    End If

    ' Check if player is nearby
    Dim targetX = player.Px - Px
    Dim targetY = player.Py - Py
    Dim distance = CSng(Math.Sqrt(targetX * targetX + targetY * targetY))

    m_stateTick -= elapsedTime

    If m_stateTick <= 0.0F Then

      If distance < 6.0F Then
        Vx = (targetX / distance) * 2.0F
        Vy = (targetY / distance) * 2.0F

        If distance < 1.5F Then
          PerformAttack()
        End If
      Else
        Vx = 0
        Vy = 0
      End If

      m_stateTick += 1.0F

    End If

  End Sub

  Friend Overrides Sub PerformAttack()
    If m_equipedWeapon Is Nothing Then Return
    m_equipedWeapon.OnUse(Me)
  End Sub

End Class

Friend Class Dynamic_Creature_Witty
  Inherits Dynamic_Creature

  Friend Sub New()
    MyBase.New("witty", RPG_Assets.Get().GetSprite("player"))
    Friendly = True
    Health = 9
    HealthMax = 10
    m_stateTick = 2.0F
    m_equipedWeapon = DirectCast(RPG_Assets.Get().GetItem("Basic Sword"), Weapon)
  End Sub

  Friend Overrides Sub PerformAttack()
    If m_equipedWeapon Is Nothing Then Return
    m_equipedWeapon.OnUse(Me)
  End Sub

End Class

Friend Class Dynamic_Teleport
  Inherits Dynamic

  Friend Property MapPosX As Single
  Friend Property MapPosY As Single
  Friend Property MapName As String

  Friend Sub New(x As Single, y As Single, mapName As String, tx As Single, ty As Single)
    MyBase.New("Teleport")
    Px = x
    Py = y
    MapPosX = tx
    MapPosY = ty
    Me.MapName = mapName
    SolidVsDyn = False
    SolidVsMap = False
  End Sub

  Friend Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
    ' Does Nothing
    gfx.DrawCircle((Px + 0.5F - ox) * 16.0F, (Py + 0.5F - oy) * 16.0F, 0.5F * 16.0F) ' For debugging
  End Sub

  Friend Overrides Sub Update(elapsedTime As Single, Optional player As Dynamic = Nothing)
    ' Do Nothing
  End Sub

End Class

Friend Class Dynamic_Item
  Inherits Dynamic

  Private ReadOnly m_item As Item
  Private m_collected As Boolean = False

  Friend Sub New(x As Single, y As Single, item As Item)
    MyBase.New("pickup")
    Px = x
    Py = y
    SolidVsDyn = False
    SolidVsMap = False
    Friendly = True
    m_collected = False
    Me.m_item = item
  End Sub

  Friend Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
    If m_collected Then Return
    gfx.DrawPartialSprite((Px - ox) * 16.0F, (Py - oy) * 16.0F, m_item.Sprite, 0, 0, 16, 16)
  End Sub

  Friend Overrides Sub OnInteract(Optional player As Dynamic = Nothing)

    If m_collected Then Return

    If m_item.OnInteract(player) Then
      ' Add item to inventory
      g_engine.GiveItem(m_item)
    End If

    m_collected = True

  End Sub

End Class

Friend Class Dynamic_Projectile
  Inherits Dynamic

  Private ReadOnly m_sprite As Sprite = Nothing
  Private ReadOnly m_spriteX As Single
  Private ReadOnly m_spriteY As Single
  Private m_duration As Single

  Friend Property OneHit As Boolean = True
  Friend Property Damage As Integer = 0

  Friend Sub New(ox As Single, oy As Single, [friend] As Boolean, velX As Single, velY As Single, duration As Single, sprite As Sprite, tx As Single, ty As Single)
    MyBase.New("projectile")
    m_sprite = sprite
    m_spriteX = tx
    m_spriteY = ty
    m_duration = duration
    Px = ox
    Py = oy
    Vx = velX
    Vy = velY
    SolidVsDyn = False
    SolidVsMap = True
    IsProjectile = True
    IsAttackable = False
    Friendly = [friend]
  End Sub

  Friend Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
    gfx.DrawPartialSprite((Px - ox) * 16, (Py - oy) * 16, m_sprite, m_spriteX * 16, m_spriteY * 16, 16, 16)
  End Sub

  Friend Overrides Sub Update(elapsedTime As Single, Optional player As Dynamic = Nothing)
    m_duration -= elapsedTime
    If (m_duration <= 0.0F) Then Redundant = True
  End Sub

End Class