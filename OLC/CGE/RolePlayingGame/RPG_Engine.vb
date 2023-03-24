'Option Explicit On
'Option Strict On
'Option Infer On

'Imports ConsoleGameEngine

'Public Class RPG_Engine
'  Inherits ConsoleGameEngine.ConsoleGameEngine

'  Private m_pCurrentMap As Map = Nothing
'  Private m_pPlayer As Dynamic_Creature = Nothing
'  Private m_vecDynamics As New List(Of Dynamic)()
'  Private m_vecProjectiles As New List(Of Dynamic)()
'  Private m_script As ScriptProcessor = New ScriptProcessor()

'  Private m_listQuests As New List(Of Quest)()
'  Private m_listItems As New List(Of Item)()

'  Private fCameraPosX As Single = 0.0F
'  Private fCameraPosY As Single = 0.0F

'  Private m_sprFont As Sprite = Nothing

'  Private m_vecDialogToShow As New List(Of String)()
'  Private m_bShowDialog As Boolean = False
'  Private m_fDialogX As Single = 0.0F
'  Private m_fDialogY As Single = 0.0F

'  Private Enum GameMode
'    MODE_TITLE
'    MODE_LOCAL_MAP
'    MODE_WORLD_MAP
'    MODE_INVENTORY
'    MODE_SHOP
'  End Enum

'  Private m_nGameMode As GameMode = GameMode.MODE_LOCAL_MAP

'  Private m_nInvSelectX As Integer = 0
'  Private m_nInvSelectY As Integer = 0

'  Public Sub New()
'    Me.m_sAppName = "Top Down Role Playing Game"
'  End Sub

'  Public Overrides Function OnUserCreate() As Boolean

'    cCommand.g_engine = Me
'    cMap.g_script = m_script

'    cQuest.g_script = m_script
'    cQuest.g_engine = Me

'    cDynamic.g_engine = Me

'    cItem.g_engine = Me

'    RPG_Assets.Get().LoadSprites()
'    RPG_Assets.Get().LoadMaps()
'    RPG_Assets.Get().LoadItems()

'    m_sprFont = RPG_Assets.Get().GetSprite("font")

'    m_listQuests.push_front(New Quest_MainQuest())

'    m_pPlayer = New Dynamic_Creature_Witty()

'    m_listItems.push_back(RPG_Assets.Get().GetItem("Basic Sword"))

'    ChangeMap("coder town", 5, 5)

'    Return True

'  End Function

'  Public Overrides Function OnUserUpdate(fElapsedTime As Single) As Boolean

'    Select Case m_nGameMode
'        'Case MODE_TITLE
'            'Return UpdateTitleScreen(fElapsedTime)
'      Case MODE_LOCAL_MAP
'        Return UpdateLocalMap(fElapsedTime)
'        'Case MODE_WORLD_MAP
'        '	Return UpdateWorldMap(fElapsedTime)
'      Case MODE_INVENTORY
'        Return UpdateInventory(fElapsedTime)
'        'Case MODE_SHOP
'        'Return UpdateShop(fElapsedTime)
'    End Select

'    Return True

'  End Function

'  Private Function UpdateLocalMap(fElapsedTime As Single) As Boolean

'    ' Update script
'    m_script.ProcessCommands(fElapsedTime)

'    ' Erase and delete redundant projectiles	
'    'm_vecProjectiles.erase(
'    'remove_if(m_vecProjectiles.begin(), m_vecProjectiles.end(),
'    '    Function(ByVal d As cDynamic) CType(d, cDynamic_Projectile).bRedundant), m_vecProjectiles.end())

'    If m_script.bUserControlEnabled Then

'      m_pPlayer.vx = 0.0F
'      m_pPlayer.vy = 0.0F

'      If Not m_bShowDialog AndAlso m_pPlayer.bControllable Then

'        ' Handle Input
'        If IsFocused() Then

'          If GetKey(VK_UP).bHeld Then
'            m_pPlayer.vy = -4.0F
'          End If

'          If GetKey(VK_DOWN).bHeld Then
'            m_pPlayer.vy = 4.0F
'          End If

'          If GetKey(VK_LEFT).bHeld Then
'            m_pPlayer.vx = -4.0F
'          End If

'          If GetKey(VK_RIGHT).bHeld Then
'            m_pPlayer.vx = 4.0F
'          End If

'          If GetKey(&H5A).bReleased Then
'            m_nGameMode = MODE_INVENTORY
'          End If

'          If GetKey(VK_SPACE).bReleased Then ' Interaction requested
'            ' Grab a point from the direction the player is facing and check for interactions										
'            Dim fTestX, fTestY As Single

'            Select Case m_pPlayer.GetFacingDirection()
'              Case 0 ' South
'                fTestX = m_pPlayer.px + 0.5F
'                fTestY = m_pPlayer.py + 1.5F
'              Case 1 ' West
'                fTestX = m_pPlayer.px - 0.5F
'                fTestY = m_pPlayer.py + 0.5F
'              Case 2 ' North
'                fTestX = m_pPlayer.px + 0.5F
'                fTestY = m_pPlayer.py - 0.5F
'              Case 3 ' East
'                fTestX = m_pPlayer.px + 1.5F
'                fTestY = m_pPlayer.py + 0.5F
'            End Select

'            ' Check if test point has hit a dynamic object
'            Dim bHitSomething As Boolean = False
'            For Each dyns As cDynamic In m_vecDynamics
'              If fTestX > dyns.px AndAlso fTestX < (dyns.px + 1.0F) AndAlso fTestY > dyns.py AndAlso fTestY < (dyns.py + 1.0F) Then
'                If dyns.bFriendly Then
'                  bHitSomething = True

'                  ' Iterate through quest stack until something responds, the base quests should capture
'                  ' interactions that are not specfied in other quests
'                  For Each quest As cQuest In m_listQuests
'                    If quest.OnInteraction(m_vecDynamics, dyns, cQuest.TALK) Then
'                      Exit For
'                    End If
'                  Next

'                  ' Some objects just do stuff when you interact with them
'                  dyns.OnInteract(m_pPlayer)

'                  ' Then check if it is map related
'                  m_pCurrentMap.OnInteraction(m_vecDynamics, dyns, cMap.TALK)
'                Else
'                  ' Interaction was with something not friendly - only enemies
'                  ' are not friendly, so perfrom attack
'                  m_pPlayer.PerformAttack()
'                End If
'              End If
'            Next

'            If Not bHitSomething Then ' Default action is attack
'              m_pPlayer.PerformAttack()
'            End If

'          End If
'        End If
'      End If
'    Else
'      ' Scripting system is in control
'      If m_bShowDialog Then
'        If GetKey(VK_SPACE).Released Then
'          m_bShowDialog = False
'          m_script.CompleteCommand()
'        End If
'      End If
'    End If

'    Dim bWorkingWithProjectiles As Boolean = False
'    For Each source In {m_vecDynamics, m_vecProjectiles}
'      For Each o In source
'        Dim fNewObjectPosX As Single = o.px + o.vx * fElapsedTime
'        Dim fNewObjectPosY As Single = o.py + o.vy * fElapsedTime

'        ' Collision
'        Dim fBorder As Single = 0.1F
'        Dim bCollisionWithMap As Boolean = False

'        If o.vx <= 0 Then
'          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder, o.py + fBorder + 0.0F) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder, o.py + (1.0F - fBorder)) Then
'            fNewObjectPosX = CInt(fNewObjectPosX) + 1
'            o.vx = 0
'            bCollisionWithMap = True
'          End If
'        Else
'          If m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), o.py + fBorder + 0.0F) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), o.py + (1.0F - fBorder)) Then
'            fNewObjectPosX = CInt(fNewObjectPosX)
'            o.vx = 0
'            bCollisionWithMap = True
'          End If
'        End If

'        If o.vy <= 0 Then
'          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder + 0.0F, fNewObjectPosY + fBorder) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), fNewObjectPosY + fBorder) Then
'            fNewObjectPosY = CInt(fNewObjectPosY) + 1
'            o.vy = 0
'            bCollisionWithMap = True
'          End If
'        Else
'          If m_pCurrentMap.GetSolid(fNewObjectPosX + fBorder + 0.0F, fNewObjectPosY + (1.0F - fBorder)) OrElse m_pCurrentMap.GetSolid(fNewObjectPosX + (1.0F - fBorder), fNewObjectPosY + (1.0F - fBorder)) Then
'            fNewObjectPosY = CInt(fNewObjectPosY)
'            o.vy = 0
'            bCollisionWithMap = True
'          End If
'        End If

'        If o.bIsProjectile AndAlso bCollisionWithMap Then
'          o.bRedundant = True
'        End If

'        Dim fDynamicObjectPosX As Single = fNewObjectPosX
'        Dim fDynamicObjectPosY As Single = fNewObjectPosY

'        ' Object V Object collisions
'        For Each dyn In m_vecDynamics
'          If dyn IsNot o Then
'            ' If the object is solid then the player must not overlap it
'            If dyn.bSolidVsDyn AndAlso o.bSolidVsDyn Then
'              ' Check if bounding rectangles overlap
'              If fDynamicObjectPosX < (dyn.px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.px AndAlso o.py < (dyn.py + 1.0F) AndAlso (o.py + 1.0F) > dyn.py Then
'                ' First Check Horizontally - Check Left
'                If o.vx <= 0 Then
'                  fDynamicObjectPosX = dyn.px + 1.0F
'                Else
'                  fDynamicObjectPosX = dyn.px - 1.0F
'                End If
'              End If
'              If fDynamicObjectPosX < (dyn.px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.px AndAlso fDynamicObjectPosY < (dyn.py + 1.0F) AndAlso (fDynamicObjectPosY + 1.0F) > dyn.py Then

'                ' First Check Vertically - Check Left
'                If o.vy <= 0 Then
'                  fDynamicObjectPosY = dyn.py + 1.0F
'                Else
'                  fDynamicObjectPosY = dyn.py - 1.0F
'                End If
'              End If

'            Else
'              If o Is m_vecDynamics(0) Then
'                ' Object is player and can interact with things
'                If fDynamicObjectPosX < (dyn.px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.px AndAlso o.py < (dyn.py + 1.0F) AndAlso (o.py + 1.0F) > dyn.py Then

'                  ' First check if object is part of a quest
'                  For Each quest In m_listQuests
'                    If quest.OnInteraction(m_vecDynamics, dyn, cQuest.WALK) Then
'                      Exit For
'                    End If
'                  Next

'                  ' Then check if it is map related
'                  m_pCurrentMap.OnInteraction(m_vecDynamics, dyn, cMap.WALK)

'                  ' Finally just check the object
'                  dyn.OnInteract(Of o)

'                End If

'              Else
'                If bWorkingWithProjectiles Then
'                  If fDynamicObjectPosX < (dyn.px + 1.0F) AndAlso (fDynamicObjectPosX + 1.0F) > dyn.px AndAlso
'                    fDynamicObjectPosY < (dyn.py + 1.0F) AndAlso (fDynamicObjectPosY + 1.0F) > dyn.py Then
'                    If dyn.bFriendly <> o.bFriendly Then
'                      ' We know object is a projectile, so dyn is something
'                      ' opposite that it has overlapped with											
'                      If dyn.bIsAttackable Then
'                        ' Dynamic object is a creature
'                        Damage(DirectCast(o, Dynamic_Projectile), DirectCast(dyn, Dynamic_Creature))
'                      End If
'                    End If
'                  End If
'                End If
'              End If
'            End If
'          End If
'        Next

'        o.px = fDynamicObjectPosX
'        o.py = fDynamicObjectPosY

'      Next

'      bWorkingWithProjectiles = True

'    Next

'    For Each source As List(Of Dynamic) In {m_vecDynamics, m_vecProjectiles}
'      For Each dyn As Dynamic In source
'        dyn.Update(fElapsedTime, m_pPlayer)
'      Next
'    Next

'    ' Remove quests that have been completed
'    Dim i = m_listQuests.RemoveAll(Function(q As Quest) q.bCompleted)

'    fCameraPosX = m_pPlayer.px
'    fCameraPosY = m_pPlayer.py

'    ' Draw Level
'    Dim nTileWidth As Integer = 16
'    Dim nTileHeight As Integer = 16
'    Dim nVisibleTilesX As Integer = ScreenWidth() \ nTileWidth
'    Dim nVisibleTilesY As Integer = ScreenHeight() \ nTileHeight

'    ' Calculate Top-Leftmost visible tile
'    Dim fOffsetX As Single = fCameraPosX - CSng(nVisibleTilesX) / 2.0F
'    Dim fOffsetY As Single = fCameraPosY - CSng(nVisibleTilesY) / 2.0F

'    ' Clamp camera to game boundaries
'    If fOffsetX < 0 Then fOffsetX = 0
'    If fOffsetY < 0 Then fOffsetY = 0
'    If fOffsetX > m_pCurrentMap.nWidth - nVisibleTilesX Then fOffsetX = m_pCurrentMap.nWidth - nVisibleTilesX
'    If fOffsetY > m_pCurrentMap.nHeight - nVisibleTilesY Then fOffsetY = m_pCurrentMap.nHeight - nVisibleTilesY

'    ' Get offsets for smooth movement
'    Dim fTileOffsetX As Single = (fOffsetX - Fix(fOffsetX)) * nTileWidth
'    Dim fTileOffsetY As Single = (fOffsetY - Fix(fOffsetY)) * nTileHeight

'    ' Draw visible tile map
'    For x As Integer = -1 To nVisibleTilesX + 1 Step 1
'      For y As Integer = -1 To nVisibleTilesY + 1 Step 1
'        Dim idx As Integer = m_pCurrentMap.GetIndex(x + fOffsetX, y + fOffsetY)
'        Dim sx As Integer = idx Mod 10
'        Dim sy As Integer = idx \ 10
'        DrawPartialSprite(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, m_pCurrentMap.pSprite, sx * nTileWidth, sy * nTileHeight, nTileWidth, nTileHeight)
'      Next
'    Next

'    ' Draw Object
'    For Each source In New Object() {m_vecDynamics, m_vecProjectiles}
'      For Each dyns In source
'        dyns.DrawSelf(Me, fOffsetX, fOffsetY)
'      Next
'    Next

'    m_pPlayer.DrawSelf(Me, fOffsetX, fOffsetY)

'    Dim sHealth As String = "HP: " & m_pPlayer.nHealth & "/" & m_pPlayer.nHealthMax
'    DisplayDialog(sHealth, 160, 10)

'    ' Draw any dialog being displayed
'    If m_bShowDialog Then
'      DisplayDialog(m_vecDialogToShow, 20, 20)
'    End If

'    Return True

'  End Function

'  Public Sub ShowDialog(vecLines As List(Of String))
'    m_vecDialogToShow = vecLines
'    m_bShowDialog = True
'  End Sub

'  Sub DisplayDialog(ByVal vecText As List(Of String), ByVal x As Integer, ByVal y As Integer)
'    Dim nMaxLineLength As Integer = 0
'    Dim nLines As Integer = vecText.Count
'    For Each l As String In vecText
'      If l.Length > nMaxLineLength Then nMaxLineLength = l.Length
'    Next

'    ' Draw Box
'    Fill(x - 1, y - 1, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1, PIXEL_SOLID, FG_DARK_BLUE)
'    DrawLine(x - 2, y - 2, x - 2, y + nLines * 8 + 1)
'    DrawLine(x + nMaxLineLength * 8 + 1, y - 2, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1)
'    DrawLine(x - 2, y - 2, x + nMaxLineLength * 8 + 1, y - 2)
'    DrawLine(x - 2, y + nLines * 8 + 1, x + nMaxLineLength * 8 + 1, y + nLines * 8 + 1)

'    For l As Integer = 0 To vecText.Count - 1
'      DrawBigText(vecText(l), x, y + l * 8)
'    Next

'  End Sub

'  Private Sub DrawBigText(ByVal sText As String, ByVal x As Integer, ByVal y As Integer)
'    Dim i As Integer = 0
'    For Each c As Char In sText
'      Dim sx As Integer = ((AscW(c) - 32) Mod 16) * 8
'      Dim sy As Integer = ((AscW(c) - 32) \ 16) * 8
'      DrawPartialSprite(x + i * 8, y, m_sprFont, sx, sy, 8, 8)
'      i += 1
'    Next
'  End Sub

'  Public Sub ChangeMap(ByVal sMapName As String, ByVal x As Single, ByVal y As Single)
'    ' Destroy all dynamics
'    m_vecDynamics.Clear()
'    m_vecDynamics.Add(m_pPlayer)

'    ' Set current map
'    m_pCurrentMap = RPG_Assets.Get().GetMap(sMapName)

'    ' Update player location
'    m_pPlayer.px = x
'    m_pPlayer.py = y

'    ' Create new dynamics from map
'    m_pCurrentMap.PopulateDynamics(m_vecDynamics)

'    ' Create new dynamics from quests
'    For Each q As cQuest In m_listQuests
'      q.PopulateDynamics(m_vecDynamics, m_pCurrentMap.sName)
'    Next
'  End Sub

'  Public Sub AddQuest(ByVal quest As cQuest)
'    m_listQuests.AddFirst(quest)
'  End Sub

'  Public Function GiveItem(ByVal item As cItem) As Boolean
'    'm_script.AddCommand(new cCommand_ShowDialog({ "You have found a" , item->sName }));
'    m_listItems.Add(item)
'    Return True
'  End Function

'  Public Function TakeItem(ByVal item As cItem) As Boolean
'    If item IsNot Nothing Then
'      m_listItems.Remove(item)
'      Return True
'    Else
'      Return False
'    End If
'  End Function

'  Public Function HasItem(ByVal item As cItem) As Boolean
'    If item IsNot Nothing Then
'      Return m_listItems.Contains(item)
'    Else
'      Return False
'    End If
'  End Function

'  Public Function UpdateInventory(fElapsedTime As Single) As Boolean

'    Fill(0, 0, ScreenWidth(), ScreenHeight(), &H2588)

'    DrawBigText("INVENTORY", 4, 4)
'    Dim i As Integer = 0
'    Dim highlighted As Item = Nothing

'    ' Draw Consumables
'    For Each item In m_listItems
'      Dim x As Integer = i Mod 4
'      Dim y As Integer = i \ 4
'      i += 1

'      DrawPartialSprite(8 + x * 20, 20 + y * 20, item.pSprite, 0, 0, 16, 16)

'      If m_nInvSelectX = x AndAlso m_nInvSelectY = y Then
'        highlighted = item
'      End If
'    Next

'    ' Draw selection reticule
'    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20)
'    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20)
'    DrawLine(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20)
'    DrawLine(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20, 6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20)

'    If GetKey(VK_LEFT).Released Then m_nInvSelectX -= 1
'    If GetKey(VK_RIGHT).Released Then m_nInvSelectX += 1
'    If GetKey(VK_UP).Released Then m_nInvSelectY -= 1
'    If GetKey(VK_DOWN).Released Then m_nInvSelectY += 1

'    If m_nInvSelectX < 0 Then
'      m_nInvSelectX = 3
'    End If

'    If m_nInvSelectX >= 4 Then
'      m_nInvSelectX = 0
'    End If

'    If m_nInvSelectY < 0 Then
'      m_nInvSelectY = 3
'    End If

'    If m_nInvSelectY >= 4 Then
'      m_nInvSelectY = 0
'    End If

'    If GetKey(AscW("Z"c)).Released Then
'      m_nGameMode = MODE_LOCAL_MAP
'    End If

'    DrawBigText("SELECTED:", 8, 160)

'    If highlighted IsNot Nothing Then
'      DrawBigText("SELECTED:", 8, 160)
'      DrawBigText(highlighted.sName, 8, 170)

'      DrawBigText("DESCRIPTION:", 8, 190)
'      DrawBigText(highlighted.sDescription, 8, 200)

'      If Not highlighted.bKeyItem Then
'        DrawBigText("(Press SPACE to use)", 80, 160)
'      End If

'      If GetKey(VK_SPACE).Released Then
'        ' Use selected item 
'        If Not highlighted.bKeyItem Then
'          If highlighted.OnUse(m_pPlayer) Then
'            ' Item has signalled it must be consumed, so remove it
'            TakeItem(highlighted)
'          End If
'        Else

'        End If
'      End If
'    End If

'    DrawBigText("LOCATION:", 128, 8)
'    DrawBigText(m_pCurrentMap.sName, 128, 16)

'    DrawBigText("HEALTH: " + m_pPlayer.nHealth.ToString(), 128, 32)
'    DrawBigText("MAX HEALTH: " + m_pPlayer.nHealthMax.ToString(), 128, 40)

'    Return True

'  End Function

'  Private Sub Attack(aggressor As Dynamic_Creature, weapon As Weapon)
'    weapon.OnUse(aggressor)
'  End Sub

'  Private Sub AddProjectile(proj As Dynamic_Projectile)
'    m_vecProjectiles.Add(proj)
'  End Sub

'  Private Sub Damage(projectile As Dynamic_Projectile, victim As Dynamic_Creature)

'    If victim IsNot Nothing Then

'      ' Attack victim with damage
'      victim.nHealth -= projectile.nDamage

'      ' Knock victim back
'      Dim tx = victim.px - projectile.px
'      Dim ty = victim.py - projectile.py
'      Dim d = CSng(Math.Sqrt(tx * tx + ty * ty))

'      If d < 1 Then d = 1.0F

'      ' After a hit, they object experiences knock back, where it is temporarily
'      ' under system control. This delivers two functions, the first being
'      ' a visual indicator to the player that something has happened, and the second
'      ' it stops the ability to spam attacks on a single creature
'      victim.KnockBack(tx / d, ty / d, 0.2F)

'      If victim IsNot m_pPlayer Then
'        victim.OnInteract(m_pPlayer)
'      Else
'        ' We must ensure the player is never pushed out of bounds by the physics engine. This
'        ' is a bit of a hack, but it allows knockbacks to occur providing there is an exit
'        ' point for the player to be knocked back into. If the player is "mobbed" then they
'        ' become trapped, and must fight their way out
'        victim.bSolidVsDyn = True
'      End If

'      If projectile.bOneHit Then projectile.bRedundant = True

'    End If

'  End Sub

'End Class