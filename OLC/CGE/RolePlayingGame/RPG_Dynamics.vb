Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine

Public MustInherit Class Dynamic

  Public Property Px As Single
  Public Property Py As Single
  Public Property Vx As Single
  Public Property Vy As Single

  Public bSolidVsMap As Boolean
  Public bSolidVsDyn As Boolean
  Public bFriendly As Boolean
  Public sName As String

  'Public bRedundant As Boolean
  'Public bIsProjectile As Boolean
  'Public bIsAttackable As Boolean = True

  'Public Shared g_engine As RPG_Engine = Nothing ' Protected?

  Public Sub New(n As String)
    sName = n
    px = 0.0F
    py = 0.0F
    vx = 0.0F
    vy = 0.0F
    bSolidVsMap = True
    bSolidVsDyn = True
    bFriendly = True

    'bRedundant = False
    'bIsAttackable = False
    'bIsProjectile = False
  End Sub

  Public MustOverride Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)

  Public MustOverride Sub Update(fElapsedTime As Single, Optional player As Dynamic = Nothing)

  Public Overridable Sub OnInteract(Optional player As Dynamic = Nothing)
    ' Do nothing?
  End Sub

End Class

Public Class Dynamic_Creature
  Inherits Dynamic

  Private m_pSprite As Sprite = Nothing
  Private m_nGraphicCounter As Integer = 0
  Private m_fTimer As Single = 0.0F

  Public Enum FacingDirection
    SOUTH = 0
    WEST = 1
    NORTH = 2
    EAST = 3
  End Enum
  Private m_nFacingDirection As FacingDirection = FacingDirection.SOUTH

  Public Enum GraphicState
    STANDING
    WALKING
    CELEBRATING
    DEAD
  End Enum
  Private m_nGraphicState As GraphicState = GraphicState.STANDING

  Public nHealth As Integer = 10
  Public nHealthMax As Integer = 10

  '---

  Public bControllable As Boolean = True
  Protected m_fStateTick As Single
  Protected m_fKnockBackTimer As Single = 0.0F
  Protected m_fKnockBackDX As Single = 0.0F
  Protected m_fKnockBackDY As Single = 0.0F

  'Public pEquipedWeapon As Weapon = Nothing

  Public Sub New(name As String, sprite As Sprite)
    MyBase.New(name)
    m_pSprite = sprite
    nHealth = 10
    nHealthMax = 10
    m_nFacingDirection = FacingDirection.SOUTH
    m_nGraphicState = GraphicState.STANDING
    m_fTimer = 0.0F
    m_nGraphicCounter = 0
    'bIsAttackable = True
  End Sub

  Public Overrides Sub Update(fElapsedTime As Single, Optional player As Dynamic = Nothing)

    If m_fKnockBackTimer > 0.0F Then
      vx = m_fKnockBackDX * 10.0F
      vy = m_fKnockBackDY * 10.0F
      'bIsAttackable = False
      m_fKnockBackTimer -= fElapsedTime
      If m_fKnockBackTimer <= 0.0F Then
        m_fStateTick = 0.0F
        bControllable = True
        'bIsAttackable = True
      End If
    Else
      bSolidVsDyn = True
      m_fTimer += fElapsedTime
      If m_fTimer >= 0.2F Then
        m_fTimer -= 0.2F
        m_nGraphicCounter += 1
        m_nGraphicCounter = m_nGraphicCounter Mod 2
      End If
      If Math.Abs(vx) > 0 OrElse Math.Abs(vy) > 0 Then m_nGraphicState = GraphicState.WALKING Else m_nGraphicState = GraphicState.STANDING
      If nHealth <= 0 Then m_nGraphicState = GraphicState.DEAD
      If vx < 0.0F Then m_nFacingDirection = FacingDirection.WEST
      If vx > 0.0F Then m_nFacingDirection = FacingDirection.EAST
      If vy < -0.0F Then m_nFacingDirection = FacingDirection.NORTH
      If vy > 0.0F Then m_nFacingDirection = FacingDirection.SOUTH
      Behaviour(fElapsedTime, player)
    End If

  End Sub

  Public Sub KnockBack(dx As Single, dy As Single, dist As Single)
    m_fKnockBackDX = dx
    m_fKnockBackDY = dy
    m_fKnockBackTimer = dist
    bSolidVsDyn = False
    bControllable = False
    'bIsAttackable = False
  End Sub

  Public Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)

    Dim sheetOffsetX = 0
    Dim sheetOffsetY = 0

    Select Case m_nGraphicState
      Case GraphicState.STANDING : sheetOffsetX = m_nFacingDirection * 16
      Case GraphicState.WALKING : sheetOffsetX = m_nFacingDirection * 16 : sheetOffsetY = m_nGraphicCounter * 16
      Case GraphicState.CELEBRATING : sheetOffsetX = 4 * 16
      Case GraphicState.DEAD : sheetOffsetX = 4 * 16 : sheetOffsetY = 1 * 16
    End Select

    gfx.DrawPartialSprite((px - ox) * 16.0F, (py - oy) * 16.0F, m_pSprite, sheetOffsetX, sheetOffsetY, 16, 16)

  End Sub

  Public Overridable Sub Behaviour(fElapsedTime As Single, Optional player As Dynamic = Nothing)
    ' No default behaviour
  End Sub

  Public Function GetFacingDirection() As Integer
    Return m_nFacingDirection
  End Function

  Public Overridable Sub PerformAttack()
  End Sub

End Class

'Public Class Dynamic_Creature_Skelly
'  Inherits Dynamic_Creature

'  Public Sub New()
'    MyBase.New("Skelly", RPG_Assets.Get.GetSprite("skelly"))
'    bFriendly = False
'    nHealth = 10
'    nHealthMax = 10
'    m_fStateTick = 2.0F
'    'pEquipedWeapon = DirectCast(RPG_Assets.Get().GetItem("Basic Sword"), Weapon)
'  End Sub

'  Public Overrides Sub Behaviour(fElapsedTime As Single, Optional player As Dynamic = Nothing)
'  End Sub

'  Public Overrides Sub PerformAttack()
'  End Sub

'End Class

'Public Class Dynamic_Creature_Witty
'  Inherits Dynamic_Creature

'  Public Sub New()
'    MyBase.New("witty", RPG_Assets.Get().GetSprite("player"))
'    bFriendly = True
'    nHealth = 9
'    nHealthMax = 10
'    m_fStateTick = 2.0F
'    'pEquipedWeapon = DirectCast(RPG_Assets.Get().GetItem("Basic Sword"), Weapon)
'  End Sub

'  Public Overrides Sub Behaviour(fElapsedTime As Single, Optional player As Dynamic = Nothing)

'    If nHealth <= 0 Then
'      vx = 0
'      vy = 0
'      bSolidVsDyn = False
'      bIsAttackable = False
'      Return
'    End If

'    ' Check if player is nearby
'    Dim fTargetX = player.px - px
'    Dim fTargetY = player.py - py
'    Dim fDistance = CSng(Math.Sqrt(fTargetX * fTargetX + fTargetY * fTargetY))

'    m_fStateTick -= fElapsedTime

'    If m_fStateTick <= 0.0F Then

'      If fDistance < 6.0F Then
'        vx = (fTargetX / fDistance) * 2.0F
'        vy = (fTargetY / fDistance) * 2.0F

'        If fDistance < 1.5F Then
'          PerformAttack()
'        End If
'      Else
'        vx = 0
'        vy = 0
'      End If

'      m_fStateTick += 1.0F

'    End If

'  End Sub

'  Public Overrides Sub PerformAttack()
'    If pEquipedWeapon Is Nothing Then Return
'    pEquipedWeapon.OnUse(Me)
'  End Sub

'End Class

'Public Class Dynamic_Teleport
'  Inherits Dynamic

'  Public Sub New(x As Single, y As Single, sMapName As String, tx As Single, ty As Single)
'    MyBase.New(x, y)
'    Me.sMapName = sMapName
'    Me.fMapPosX = tx
'    Me.fMapPosY = ty
'  End Sub

'  Public Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
'    'TODO: Implement the drawing code here
'  End Sub

'  Public Overrides Sub Update(fElapsedTime As Single, Optional player As Dynamic = Nothing)
'    'TODO: Implement the update logic here
'  End Sub

'  Public sMapName As String
'  Public fMapPosX As Single
'  Public fMapPosY As Single

'End Class

'Public Class Dynamic_Projectile
'  Inherits Dynamic

'  Public pSprite As Sprite = Nothing
'  Public fSpriteX As Single
'  Public fSpriteY As Single
'  Public fDuration As Single
'  Public bOneHit As Boolean = True
'  Public nDamage As Integer = 0

'  Public Sub New(ox As Single, oy As Single, bFriend As Boolean, velx As Single, vely As Single, duration As Single, sprite As Sprite, tx As Single, ty As Single)
'    MyBase.New(ox, oy, bFriend, velx, vely)
'    fDuration = duration
'    pSprite = sprite
'    fSpriteX = tx
'    fSpriteY = ty
'  End Sub

'  Public Overrides Sub DrawSelf(gfx As ConsoleGameEngine.ConsoleGameEngine, ox As Single, oy As Single)
'    ' TODO: Implement drawing logic
'  End Sub

'  Public Overrides Sub Update(fElapsedTime As Single, Optional player As Dynamic = Nothing)
'    ' TODO: Implement update logic
'  End Sub

'End Class