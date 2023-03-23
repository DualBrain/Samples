Option Explicit On
Option Strict On
Option Infer On

Imports ConsoleGameEngine

' A Singleton...
Public Class RPG_Assets

  Private Shared ReadOnly m_instance As New RPG_Assets()

  Public Shared Function [Get]() As RPG_Assets
    Return m_instance
  End Function

  Private Sub New()
  End Sub

  Private ReadOnly m_mapSprites As New Dictionary(Of String, Sprite)()
  Private ReadOnly m_mapMaps As New Dictionary(Of String, Map)()
  'Private ReadOnly m_mapItems As New Dictionary(Of String, Item)()

  Public Function GetSprite(name As String) As Sprite
    Return m_mapSprites(name)
  End Function

  Public Function GetMap(name As String) As Map
    Return m_mapMaps(name)
  End Function

  'Public Function GetItem(name As String) As Item
  '  Return m_mapItems(name)
  'End Function

  Public Sub LoadSprites()

    Dim load As Action(Of String, String) = Sub(name As String, fileName As String)
                                              Dim s As New Sprite(fileName)
                                              m_mapSprites(name) = s
                                            End Sub

    load("village", "./rpgdata/gfx/toml_spritesheetdark.spr")

    load("skelly", "./rpgdata/gfx/toml_Char001.png.spr")
    load("player", "./rpgdata/gfx/toml_CharacterSprites.spr")
    load("font", "./rpgdata/gfx/javidx9_nesfont8x8.spr")
    load("worldmap", "./rpgdata/gfx/worldmap1.png.spr")
    load("skymap", "./rpgdata/gfx/sky1.png.spr")
    load("title", "./rpgdata/gfx/title3.png.spr")
    load("balloon", "./rpgdata/gfx/balloon1.png.spr")
    load("sword", "./rpgdata/gfx/Sword.spr")
    load("hitech", "./rpgdata/gfx/toml_modernish.spr")

    load("purple", "./rpgdata/gfx/toml_purple.spr")

    load("health", "./rpgdata/gfx/item_health.spr")
    load("healthboost", "./rpgdata/gfx/item_healthboost.spr")

    load("Basic Sword", "./rpgdata/gfx/weapon_basic_sword.spr")

  End Sub

  Public Sub LoadMaps()

    Dim load = Sub(m As Map)
                 m_mapMaps(m.sName) = m
               End Sub

    load(New Map_Village1())
    load(New Map_Home1())

  End Sub

  'Public Sub LoadItems()

  '  Dim load = Sub(i As Item)
  '               m_mapItems(i.sName) = i
  '             End Sub

  '  load(New Item_Health())
  '  load(New Item_HealthBoost())

  '  load(New Weapon_Sword())

  'End Sub

End Class