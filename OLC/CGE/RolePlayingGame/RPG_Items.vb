Option Strict On
Option Explicit On
Option Infer On

Imports ConsoleGameEngine

Friend Class Item

  Friend Shared g_engine As RPG_Engine

  Friend Property Name As String
  Friend Property Description As String
  Friend Property Sprite As Sprite
  Friend Property KeyItem As Boolean = False
  'Private bEquipable As Boolean = False

  Friend Sub New(name As String, sprite As Sprite, desc As String)
    Me.Name = name
    Me.Sprite = sprite
    Description = desc
  End Sub

  Friend Overridable Function OnInteract(obj As Dynamic) As Boolean
    Return False
  End Function

  Friend Overridable Function OnUse(obj As Dynamic) As Boolean
    Return False
  End Function

End Class

Friend Class Item_Health
  Inherits Item

  Friend Sub New()
    MyBase.New("Small Health", RPG_Assets.Get().GetSprite("health"), "Restores 10 health")
  End Sub

  Friend Overrides Function OnInteract(obj As Dynamic) As Boolean
    OnUse(obj)
    Return False ' Just absorb
  End Function

  Friend Overrides Function OnUse(obj As Dynamic) As Boolean
    If obj IsNot Nothing Then
      Dim dyn = DirectCast(obj, Dynamic_Creature)
      dyn.Health = Math.Min(dyn.Health + 10, dyn.HealthMax)
    End If
    Return True
  End Function

End Class

Friend Class Item_HealthBoost
  Inherits Item

  Friend Sub New()
    MyBase.New("Health Boost", RPG_Assets.Get().GetSprite("healthboost"), "Increases Max Health by 10")
  End Sub

  Friend Overrides Function OnInteract(obj As Dynamic) As Boolean
    Return True ' Add to inventory
  End Function

  Friend Overrides Function OnUse(obj As Dynamic) As Boolean
    If obj IsNot Nothing Then
      Dim dyn = DirectCast(obj, Dynamic_Creature)
      dyn.HealthMax += 10
      dyn.Health = dyn.HealthMax
    End If
    Return True ' Remove from inventory
  End Function

End Class

Friend Class Weapon
  Inherits Item

  Private nDamage As Integer = 0

  Friend Sub New(name As String, sprite As Sprite, desc As String, dmg As Integer)
    MyBase.New(name, sprite, desc)
    nDamage = dmg
  End Sub

  Friend Overrides Function OnInteract(obj As Dynamic) As Boolean
    Return False
  End Function

  Friend Overrides Function OnUse(obj As Dynamic) As Boolean
    Return False
  End Function

End Class

Friend Class Weapon_Sword
  Inherits Weapon

  Friend Sub New()
    MyBase.New("Basic Sword", RPG_Assets.Get().GetSprite("Basic Sword"), "A wooden sword, 5 dmg", 5)
  End Sub

  Friend Overrides Function OnUse(obj As Dynamic) As Boolean

    'When weapons are used, they are used on the object that owns the weapon, i.e.
    'the attacker. However, this does not imply the attacker attacks themselves

    'Get direction of attacker
    Dim aggressor = CType(obj, Dynamic_Creature)

    'Determine attack origin
    Dim x, y, vx, vy As Single
    Select Case aggressor.GetFacingDirection()
      Case 0 'South
        x = aggressor.Px
        y = aggressor.Py + 1.0F
        vx = 0.0F
        vy = 1.0F
      Case 1 'East
        x = aggressor.Px - 1.0F
        y = aggressor.Py
        vx = -1.0F
        vy = 0.0F
      Case 2 'North
        x = aggressor.Px
        y = aggressor.Py - 1.0F
        vx = 0.0F
        vy = -1.0F
      Case 3 'West
        x = aggressor.Px + 1.0F
        y = aggressor.Py
        vx = 1.0F
        vy = 0.0F
    End Select

    If aggressor.Health = aggressor.HealthMax Then
      'Beam sword
      Dim p = New Dynamic_Projectile(x, y, aggressor.Friendly, vx * 15.0F, vy * 15.0F, 1.0F, RPG_Assets.Get().GetSprite("Basic Sword"), (aggressor.GetFacingDirection() + 3) Mod 4 + 1, 1.0F)
      p.SolidVsMap = True
      p.SolidVsDyn = False
      p.Damage = 5
      p.OneHit = False
      g_engine.AddProjectile(p)
    End If

    Dim p1 = New Dynamic_Projectile(x, y, aggressor.Friendly, aggressor.Vx, aggressor.Vy, 0.1F, RPG_Assets.Get().GetSprite("Basic Sword"), (aggressor.GetFacingDirection() + 3) Mod 4 + 1, 0.0F)
    p1.SolidVsMap = False
    p1.SolidVsDyn = False
    p1.Damage = 5
    p1.OneHit = True

    g_engine.AddProjectile(p1)

    Return False

  End Function

End Class