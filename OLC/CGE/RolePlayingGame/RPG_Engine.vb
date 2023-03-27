Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine.Colour
Imports ConsoleGameEngine.PIXEL_TYPE
Imports ConsoleGameEngine

Friend Class RPG_Engine
  Inherits ConsoleGameEngine.ConsoleGameEngine

  Private m_pCurrentMap As Map = Nothing
  Private m_pPlayer As Dynamic_Creature = Nothing
  Private m_vecDynamics As New List(Of Dynamic)()
  Private m_vecProjectiles As New List(Of Dynamic)()
  Private m_script As ScriptProcessor = New ScriptProcessor()

  Private m_listQuests As New List(Of Quest)()
  Private m_listItems As New List(Of Item)()

  Private fCameraPosX As Single = 0.0F
  Private fCameraPosY As Single = 0.0F

  Private m_sprFont As Sprite = Nothing

  Private m_vecDialogToShow As New List(Of String)()
  Private m_bShowDialog As Boolean = False
  Private m_fDialogX As Single = 0.0F
  Private m_fDialogY As Single = 0.0F

  Private Enum GameMode
    MODE_TITLE
    MODE_LOCAL_MAP
    MODE_WORLD_MAP
    MODE_INVENTORY
    MODE_SHOP
  End Enum

  Private m_nGameMode As GameMode = GameMode.MODE_LOCAL_MAP

  Private m_nInvSelectX As Integer = 0
  Private m_nInvSelectY As Integer = 0

  Public Sub New()
    Me.m_sAppName = "Top Down Role Playing Game"
  End Sub

  Public Overrides Function OnUserCreate() As Boolean

    Command.g_engine = Me
    Map.g_script = m_script

    Quest.g_script = m_script
    Quest.g_engine = Me

    Dynamic.g_engine = Me

    Item.g_engine = Me

    RPG_Assets.Get().LoadSprites()
    RPG_Assets.Get().LoadMaps()
    RPG_Assets.Get().LoadItems()

    m_sprFont = RPG_Assets.Get().GetSprite("font")

    m_listQuests.Insert(0, New Quest_MainQuest())

    m_pPlayer = New Dynamic_Creature_Witty()

    m_listItems.Add(RPG_Assets.Get().GetItem("Basic Sword"))

    ChangeMap("coder town", 5, 5)

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    Select Case m_nGameMode
        'Case GameMode.MODE_TITLE
            'Return UpdateTitleScreen(fElapsedTime)
      Case GameMode.MODE_LOCAL_MAP
        Return UpdateLocalMap(elapsedTime)
        'Case GameMode.MODE_WORLD_MAP
        '	Return UpdateWorldMap(fElapsedTime)
      Case GameMode.MODE_INVENTORY
        Return UpdateInventory(elapsedTime)
        'Case GameMode.MODE_SHOP
        'Return UpdateShop(fElapsedTime)
    End Select

    Return True

  End Function

  Private Function UpdateLocalMap(elapsedTime As Single) As Boolean

    ' Update script
    m_script.ProcessCommands(elapsedTime)

    ' Erase and delete redundant projectiles	
    Dim i = m_vecProjectiles.RemoveAll(Function(d) d.Redundant)

    If m_script.UserControlEnabled Then

      m_pPlayer.Vx = 0.0F
      m_pPlayer.Vy = 0.0F

      If Not m_bShowDialog AndAlso m_pPlayer.Controllable Then

        ' Handle Input
        If IsFocused() Then

          If GetKey(VK_UP).Held OrElse GetKey(AscW("W"c)).Held Then m_pPlayer.Vy = -4.0F
          If GetKey(VK_DOWN).Held OrElse GetKey(AscW("S"c)).Held Then m_pPlayer.Vy = 4.0F
          If GetKey(VK_LEFT).Held OrElse GetKey(AscW("A"c)).Held Then m_pPlayer.Vx = -4.0F
          If GetKey(VK_RIGHT).Held OrElse GetKey(AscW("D"c)).Held Then m_pPlayer.Vx = 4.0F
          If GetKey(&H5A).Released OrElse GetKey(AscW("I"c)).Released Then m_nGameMode = GameMode.MODE_INVENTORY

          If GetKey(VK_SPACE).Released Then ' Interaction requested
            ' Grab a point from the direction the player is facing and check for interactions										
            Dim fTestX, fTestY As Single

            Select Case m_pPlayer.GetFacingDirection()
              Case 0 ' South
                fTestX = m_pPlayer.Px + 0.5F
                fTestY = m_pPlayer.Py + 1.5F
              Case 1 ' West
                fTestX = m_pPlayer.Px - 0.5F
                fTestY = m_pPlayer.Py + 0.5F
              Case 2 ' North
                fTestX = m_pPlayer.Px + 0.5F
                fTestY = m_pPlayer.Py - 0.5F
              Case 3 ' East
                fTestX = m_pPlayer.Px + 1.5F
                fTestY = m_pPlayer.Py + 0.5F
            End Select

            ' Check if test point has hit a dynamic object
            Dim bHitSomething = False
            For Each dyns In m_vecDynamics

              If fTestX > dyns.Px AndAlso fTestX < (dyns.Px + 1.0F) AndAlso fTestY > dyns.Py AndAlso fTestY < (dyns.Py + 1.0F) Then

                If dyns.Friendly Then

                  bHitSomething = True

                  ' Iterate through quest stack until something responds, the base quests should capture
                  ' interactions that are not specfied in other quests
                  For Each quest In m_listQuests
                    If quest.OnInteraction(m_vecDynamics, dyns, Quest.NATURE.TALK) Then
                      Exit For
                    End If
                  Next

                  ' Some objects just do stuff when you interact with them
                  dyns.OnInteract(m_pPlayer)

                  ' Then check if it is map related
                  m_pCurrentMap.OnInteraction(m_vecDynamics, dyns, Quest.NATURE.TALK)

                Else
                  ' Interaction was with something not friendly - only enemies
                  ' are not friendly, so perfrom attack
                  m_pPlayer.PerformAttack()
                End If

              End If

            Next

            If Not bHitSomething Then ' Default action is attack
              m_pPlayer.PerformAttack()
            End If

          End If
        End If
      End If
    Else
      ' Scripting system is in control
      If m_bShowDialog Then
        If GetKey(VK_SPACE).Released Then
          m_bShowDialog = False
          m_script.CompleteCommand()
        End If
      End If
    End If

    Dim bWorkingWithProjectiles = False
    For Each source In {m_vecDynamics, m_vecProjectiles}

      For Each o In source

        Dim fNewObjectPosX = o.Px + o.Vx * elapsedTime
        Dim fNewObjectPosY = o.Py + o.Vy * elapsedTime

        ' Collision
        Dim fBorder = 0.1F
        Dim bCollisionWithMap = False

        If o.Vx <= 0 Then
          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder, o.Py + fBorder + 0.0F) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder, o.Py + (1.0F - fBorder)) Then
            fNewObjectPosX = CInt(Fix(fNewObjectPosX)) + 1
            o.Vx = 0
            bCollisionWithMap = True
          End If
        Else
          If m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), o.Py + fBorder + 0.0F) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), o.Py + (1.0F - fBorder)) Then
            fNewObjectPosX = CInt(Fix(fNewObjectPosX))
            o.Vx = 0
            bCollisionWithMap = True
          End If
        End If

        If o.Vy <= 0 Then
          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder + 0.0F, fNewObjectPosY + fBorder) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), fNewObjectPosY + fBorder) Then
            fNewObjectPosY = CInt(Fix(fNewObjectPosY)) + 1
            o.Vy = 0
            bCollisionWithMap = True
          End If
        Else
          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder + 0.0F, fNewObjectPosY + (1.0F - fBorder)) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), fNewObjectPosY + (1.0F - fBorder)) Then
            fNewObjectPosY = CInt(Fix(fNewObjectPosY))
            o.Vy = 0
            bCollisionWithMap = True
          End If
        End If

        If o.IsProjectile AndAlso bCollisionWithMap Then
          o.Redundant = True
        End If

        Dim fDynamicObjectPosX = fNewObjectPosX
        Dim fDynamicObjectPosY = fNewObjectPosY

        ' Object V Object collisions
        For Each dyn In m_vecDynamics
          If dyn IsNot o Then
            ' If the object is solid then the player must not overlap it
            If dyn.SolidVsDyn AndAlso o.SolidVsDyn Then
              ' Check if bounding rectangles overlap
              If fDynamicObjectPosX < (dyn.Px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.Px AndAlso o.Py < (dyn.Py + 1.0F) AndAlso (o.Py + 1.0F) > dyn.Py Then
                ' First Check Horizontally - Check Left
                If o.Vx <= 0 Then
                  fDynamicObjectPosX = dyn.Px + 1.0F
                Else
                  fDynamicObjectPosX = dyn.Px - 1.0F
                End If
              End If
              If fDynamicObjectPosX < (dyn.Px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.Px AndAlso fDynamicObjectPosY < (dyn.Py + 1.0F) AndAlso (fDynamicObjectPosY + 1.0F) > dyn.Py Then
                ' First Check Vertically - Check Left
                If o.Vy <= 0 Then
                  fDynamicObjectPosY = dyn.Py + 1.0F
                Else
                  fDynamicObjectPosY = dyn.Py - 1.0F
                End If
              End If

            Else
              If o Is m_vecDynamics(0) Then
                ' Object is player and can interact with things
                If fDynamicObjectPosX < (dyn.Px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.Px AndAlso o.Py < (dyn.Py + 1.0F) AndAlso (o.Py + 1.0F) > dyn.Py Then

                  ' First check if object is part of a quest
                  For Each quest In m_listQuests
                    If quest.OnInteraction(m_vecDynamics, dyn, Quest.NATURE.WALK) Then
                      Exit For
                    End If
                  Next

                  ' Then check if it is map related
                  m_pCurrentMap.OnInteraction(m_vecDynamics, dyn, Quest.NATURE.WALK)

                  ' Finally just check the object
                  dyn.OnInteract(o)

                End If

              Else
                If bWorkingWithProjectiles Then
                  If fDynamicObjectPosX < (dyn.Px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.Px AndAlso
                    fDynamicObjectPosY < (dyn.Py + 1.0F) AndAlso (fDynamicObjectPosY + 1.0F) > dyn.Py Then
                    If dyn.Friendly <> o.Friendly Then
                      ' We know object is a projectile, so dyn is something
                      ' opposite that it has overlapped with											
                      If dyn.IsAttackable Then
                        ' Dynamic object is a creature
                        Damage(DirectCast(o, Dynamic_Projectile), DirectCast(dyn, Dynamic_Creature))
                      End If
                    End If
                  End If
                End If
              End If
            End If
          End If
        Next

        o.Px = fDynamicObjectPosX
        o.Py = fDynamicObjectPosY

      Next

      bWorkingWithProjectiles = True

    Next

    For Each source In {m_vecDynamics, m_vecProjectiles}
      For Each dyn In source
        dyn.Update(elapsedTime, m_pPlayer)
      Next
    Next

    ' Remove quests that have been completed
    Dim discard = m_listQuests.RemoveAll(Function(q As Quest) q.Completed)

    fCameraPosX = m_pPlayer.Px
    fCameraPosY = m_pPlayer.Py

    ' Draw Level
    Dim nTileWidth = 16
    Dim nTileHeight = 16
    Dim nVisibleTilesX = ScreenWidth() \ nTileWidth
    Dim nVisibleTilesY = ScreenHeight() \ nTileHeight

    ' Calculate Top-Leftmost visible tile
    Dim fOffsetX = fCameraPosX - CSng(nVisibleTilesX) / 2.0F
    Dim fOffsetY = fCameraPosY - CSng(nVisibleTilesY) / 2.0F

    ' Clamp camera to game boundaries
    If fOffsetX < 0 Then fOffsetX = 0
    If fOffsetY < 0 Then fOffsetY = 0
    If fOffsetX > m_pCurrentMap.Width - nVisibleTilesX Then fOffsetX = m_pCurrentMap.Width - nVisibleTilesX
    If fOffsetY > m_pCurrentMap.Height - nVisibleTilesY Then fOffsetY = m_pCurrentMap.Height - nVisibleTilesY

    ' Get offsets for smooth movement
    Dim fTileOffsetX = (fOffsetX - Fix(fOffsetX)) * nTileWidth
    Dim fTileOffsetY = (fOffsetY - Fix(fOffsetY)) * nTileHeight

    ' Draw visible tile map
    For x = -1 To nVisibleTilesX + 1 Step 1
      For y = -1 To nVisibleTilesY + 1 Step 1
        Dim idx = m_pCurrentMap.GetIndex(x + fOffsetX, y + fOffsetY)
        Dim sx = idx Mod 10
        Dim sy = idx \ 10
        DrawPartialSprite(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, m_pCurrentMap.Sprite, sx * nTileWidth, sy * nTileHeight, nTileWidth, nTileHeight)
      Next
    Next

    ' Draw Object
    For Each source In {m_vecDynamics, m_vecProjectiles}
      For Each dyns In source
        dyns.DrawSelf(Me, fOffsetX, fOffsetY)
      Next
    Next

    m_pPlayer.DrawSelf(Me, fOffsetX, fOffsetY)

    Dim sHealth = $"HP: {m_pPlayer.Health}/{m_pPlayer.HealthMax}"
    DisplayDialog(sHealth, 160, 10)

    ' Draw any dialog being displayed
    If m_bShowDialog Then
      DisplayDialog(m_vecDialogToShow, 20, 20)
    End If

    Return True

  End Function

  Friend Sub ShowDialog(vecLines As List(Of String))
    m_vecDialogToShow = vecLines
    m_bShowDialog = True
  End Sub

  Private Sub DisplayDialog(message As String, x As Integer, y As Integer)
    DisplayDialog(New List(Of String) From {message}, x, y)
  End Sub

  Private Sub DisplayDialog(vecText As List(Of String), x As Integer, y As Integer)

    Dim nMaxLineLength = 0
    Dim nLines = vecText.Count

    For Each l In vecText
      If l.Length > nMaxLineLength Then nMaxLineLength = l.Length
    Next

    ' Draw Box
    Fill(x - 1, y - 1, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1, PIXEL_SOLID, FG_DARK_BLUE)
    DrawLine(x - 2, y - 2, x - 2, y + nLines * 8 + 1)
    DrawLine(x + nMaxLineLength * 8 + 1, y - 2, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1)
    DrawLine(x - 2, y - 2, x + nMaxLineLength * 8 + 1, y - 2)
    DrawLine(x - 2, y + nLines * 8 + 1, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1)

    For l = 0 To vecText.Count - 1
      DrawBigText(vecText(l), x, y + l * 8)
    Next

  End Sub

  Private Sub DrawBigText(sText As String, x As Integer, y As Integer)
    Dim i = 0
    For Each c In sText
      Dim sx = ((AscW(c) - 32) Mod 16) * 8
      Dim sy = ((AscW(c) - 32) \ 16) * 8
      DrawPartialSprite(x + i * 8, y, m_sprFont, sx, sy, 8, 8)
      i += 1
    Next
  End Sub

  Friend Sub ChangeMap(sMapName As String, x As Single, y As Single)

    ' Destroy all dynamics
    m_vecDynamics.Clear()
    m_vecDynamics.Add(m_pPlayer)

    ' Set current map
    m_pCurrentMap = RPG_Assets.Get().GetMap(sMapName)

    ' Update player location
    m_pPlayer.Px = x
    m_pPlayer.Py = y

    ' Create new dynamics from map
    m_pCurrentMap.PopulateDynamics(m_vecDynamics)

    ' Create new dynamics from quests
    For Each q In m_listQuests
      q.PopulateDynamics(m_vecDynamics, m_pCurrentMap.Name)
    Next

  End Sub

  Friend Sub AddQuest(quest As Quest)
    m_listQuests.Insert(0, quest)
  End Sub

  Friend Function GiveItem(item As Item) As Boolean
    'm_script.AddCommand(new cCommand_ShowDialog({ "You have found a" , item->sName }));
    m_listItems.Add(item)
    Return True
  End Function

  Private Function TakeItem(item As Item) As Boolean
    If item IsNot Nothing Then
      m_listItems.Remove(item)
      Return True
    Else
      Return False
    End If
  End Function

  Friend Function HasItem(item As Item) As Boolean
    If item IsNot Nothing Then
      Return m_listItems.Contains(item)
    Else
      Return False
    End If
  End Function

  Private Function UpdateInventory(fElapsedTime As Single) As Boolean

    'Fill(0, 0, ScreenWidth(), ScreenHeight(), &H2588)
    Cls()

    DrawBigText("INVENTORY", 4, 4)
    Dim i = 0
    Dim highlighted As Item = Nothing

    ' Draw Consumables
    For Each item In m_listItems

      Dim x = i Mod 4
      Dim y = i \ 4
      i += 1

      DrawPartialSprite(8 + x * 20, 20 + y * 20, item.Sprite, 0, 0, 16, 16)

      If m_nInvSelectX = x AndAlso m_nInvSelectY = y Then
        highlighted = item
      End If

    Next

    ' Draw selection reticule
    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20)
    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20)
    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20)
    DrawLine(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20)

    If GetKey(VK_LEFT).Released Then m_nInvSelectX -= 1
    If GetKey(VK_RIGHT).Released Then m_nInvSelectX += 1
    If GetKey(VK_UP).Released Then m_nInvSelectY -= 1
    If GetKey(VK_DOWN).Released Then m_nInvSelectY += 1

    If m_nInvSelectX < 0 Then m_nInvSelectX = 3
    If m_nInvSelectX >= 4 Then m_nInvSelectX = 0
    If m_nInvSelectY < 0 Then m_nInvSelectY = 3
    If m_nInvSelectY >= 4 Then m_nInvSelectY = 0

    If GetKey(AscW("Z"c)).Released Then
      m_nGameMode = GameMode.MODE_LOCAL_MAP
    End If

    DrawBigText("SELECTED:", 8, 160)

    If highlighted IsNot Nothing Then

      DrawBigText("SELECTED:", 8, 160)
      DrawBigText(highlighted.Name, 8, 170)

      DrawBigText("DESCRIPTION:", 8, 190)
      DrawBigText(highlighted.Description, 8, 200)

      If Not highlighted.KeyItem Then
        DrawBigText("(Press SPACE to use)", 80, 160)
      End If

      If GetKey(VK_SPACE).Released Then
        ' Use selected item 
        If Not highlighted.KeyItem Then
          If highlighted.OnUse(m_pPlayer) Then
            ' Item has signalled it must be consumed, so remove it
            TakeItem(highlighted)
          End If
        Else
          ' ????
        End If
      End If

    End If

    DrawBigText("LOCATION:", 128, 8)
    DrawBigText(m_pCurrentMap.Name, 128, 16)

    DrawBigText("HEALTH: " + m_pPlayer.Health.ToString(), 128, 32)
    DrawBigText("MAX HEALTH: " + m_pPlayer.HealthMax.ToString(), 128, 40)

    Return True

  End Function

  Private Sub Attack(aggressor As Dynamic_Creature, weapon As Weapon)
    weapon.OnUse(aggressor)
  End Sub

  Friend Sub AddProjectile(proj As Dynamic_Projectile)
    m_vecProjectiles.Add(proj)
  End Sub

  Private Sub Damage(projectile As Dynamic_Projectile, victim As Dynamic_Creature)

    If victim IsNot Nothing Then

      ' Attack victim with damage
      victim.Health -= projectile.Damage

      ' Knock victim back
      Dim tx = victim.Px - projectile.Px
      Dim ty = victim.Py - projectile.Py
      Dim d = CSng(Math.Sqrt(tx * tx + ty * ty))

      If d < 1 Then d = 1.0F

      ' After a hit, they object experiences knock back, where it is temporarily
      ' under system control. This delivers two functions, the first being
      ' a visual indicator to the player that something has happened, and the second
      ' it stops the ability to spam attacks on a single creature
      victim.KnockBack(tx / d, ty / d, 0.2F)

      If victim IsNot m_pPlayer Then
        victim.OnInteract(m_pPlayer)
      Else
        ' We must ensure the player is never pushed out of bounds by the physics engine. This
        ' is a bit of a hack, but it allows knockbacks to occur providing there is an exit
        ' point for the player to be knocked back into. If the player is "mobbed" then they
        ' become trapped, and must fight their way out
        victim.SolidVsDyn = True
      End If

      If projectile.OneHit Then projectile.Redundant = True

    End If

  End Sub

End Class