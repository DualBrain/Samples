Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Enum RoadType
  ROAD_H
  ROAD_V
  ROAD_C1
  ROAD_C2
  ROAD_C3
  ROAD_C4
  ROAD_T1
  ROAD_T2
  ROAD_T3
  ROAD_T4
  ROAD_X
End Enum

Public Class cCell_Road
  Inherits cCell

  Private bNeighboursAreRoads(3) As Boolean

  Private meshUnitQuad As Gfx3D.Mesh = Nothing
  Private sprRoadTex(10) As Sprite

  Private Class StopPattern
    Public bStop(48) As Boolean
  End Class

  Private vStopPattern As New List(Of StopPattern)
  Private nCurrentStopPattern As Integer = 0
  Private fStopPatternTimer As Single = 0.0F

  Public nRoadType As RoadType = RoadType.ROAD_X
  Public pSafeCarTrack As cAuto_Track = Nothing
  Public pSafePedestrianTrack As cAuto_Track = Nothing
  Public pSafeChaseTrack As cAuto_Track = Nothing

  Public Sub New(map As cCityMap, x As Integer, y As Integer)
    MyBase.New(map, x, y)
    bSolid = False
    nCellType = CellType.CELL_ROAD
  End Sub

  Public Overrides Sub CalculateAdjacency()

    ' Calculate suitable road junction type
    Dim r = Function(i As Integer, j As Integer) As Boolean
              Return (pMap.Cell(nWorldX + i, nWorldY + j) IsNot Nothing AndAlso pMap.Cell(nWorldX + i, nWorldY + j).nCellType = CellType.CELL_ROAD)
            End Function

    If r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso Not r(+1, 0) Then nRoadType = RoadType.ROAD_V
    If Not r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_H
    If Not r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_C1
    If Not r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_T1
    If Not r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then nRoadType = RoadType.ROAD_C2
    If r(0, -1) AndAlso r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_T2
    If r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_X
    If r(0, -1) AndAlso r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then nRoadType = RoadType.ROAD_T3
    If r(0, -1) AndAlso Not r(0, +1) AndAlso Not r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_C3
    If r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso r(+1, 0) Then nRoadType = RoadType.ROAD_T4
    If r(0, -1) AndAlso Not r(0, +1) AndAlso r(-1, 0) AndAlso Not r(+1, 0) Then nRoadType = RoadType.ROAD_C4

    ' Add navigation tracks based on type

    Dim addTrack = Function(n1 As Integer, n2 As Integer) As cAuto_Track

                     If pNaviNodes(n1) Is Nothing OrElse pNaviNodes(n2) Is Nothing Then
                       ' Can't add track
                       Return Nothing
                     Else

                       ' Nodes exist so add track
                       Dim t As New cAuto_Track()
                       t.node(0) = pNaviNodes(n1)
                       t.node(1) = pNaviNodes(n2)
                       t.cell = Me
                       t.fTrackLength = (pNaviNodes(n1).pos - pNaviNodes(n2).pos).Mag()
                       listTracks.Add(t)

                       ' Add pointers to track to start and end nodes
                       pNaviNodes(n1).listTracks.Add(listTracks.Last())
                       pNaviNodes(n2).listTracks.Add(listTracks.Last())

                       Return listTracks.Last()

                     End If

                   End Function

    ' Ensure list of tracks for this cell is clear
    listTracks.Clear()

    ' Add tracks depending on junction type
    pSafePedestrianTrack = Nothing
    pSafeCarTrack = Nothing
    pSafeChaseTrack = Nothing

    ' Add Pedestrian Tracks
    Select Case nRoadType
      Case RoadType.ROAD_H
        pSafePedestrianTrack = addTrack(7, 13)
        addTrack(41, 35)
      Case RoadType.ROAD_V
        pSafePedestrianTrack = addTrack(1, 43)
        addTrack(5, 47)
      Case RoadType.ROAD_C1
        pSafePedestrianTrack = addTrack(43, 8)
        addTrack(8, 13)
        addTrack(47, 40)
        addTrack(40, 41)
      Case RoadType.ROAD_C2
        addTrack(7, 12)
        addTrack(12, 47)
        pSafePedestrianTrack = addTrack(35, 36)
        addTrack(36, 43)
      Case RoadType.ROAD_C3
        addTrack(1, 36)
        pSafePedestrianTrack = addTrack(36, 41)
        addTrack(5, 12)
        addTrack(12, 13)
      Case RoadType.ROAD_C4
        addTrack(35, 40)
        addTrack(40, 5)
        pSafePedestrianTrack = addTrack(7, 8)
        addTrack(8, 1)

      Case RoadType.ROAD_T1
        pSafePedestrianTrack = addTrack(7, 8)
        addTrack(8, 12)
        addTrack(12, 13)
        addTrack(35, 36)
        addTrack(36, 38)
        addTrack(38, 40)
        addTrack(40, 41)
        addTrack(8, 22)
        addTrack(22, 36)
        addTrack(36, 43)
        addTrack(12, 26)
        addTrack(26, 40)
        addTrack(40, 47)
      Case RoadType.ROAD_T2
        pSafePedestrianTrack = addTrack(1, 8)
        addTrack(8, 36)
        addTrack(36, 43)
        addTrack(5, 12)
        addTrack(12, 26)
        addTrack(26, 40)
        addTrack(40, 47)
        addTrack(8, 10)
        addTrack(10, 12)
        addTrack(12, 13)
        addTrack(36, 38)
        addTrack(38, 40)
        addTrack(40, 41)
      Case RoadType.ROAD_T3
        pSafePedestrianTrack = addTrack(5, 12)
        addTrack(12, 40)
        addTrack(40, 47)
        addTrack(1, 8)
        addTrack(8, 22)
        addTrack(22, 36)
        addTrack(36, 43)
        addTrack(12, 10)
        addTrack(10, 8)
        addTrack(8, 7)
        addTrack(40, 38)
        addTrack(38, 36)
        addTrack(36, 35)
      Case RoadType.ROAD_T4
        pSafePedestrianTrack = addTrack(35, 36)
        addTrack(36, 40)
        addTrack(40, 41)
        addTrack(7, 8)
        addTrack(8, 10)
        addTrack(10, 12)
        addTrack(12, 13)
        addTrack(36, 22)
        addTrack(22, 8)
        addTrack(8, 1)
        addTrack(40, 26)
        addTrack(26, 12)
        addTrack(12, 5)
      Case RoadType.ROAD_X
        addTrack(35, 36)
        addTrack(36, 38)
        addTrack(38, 40)
        addTrack(40, 41)
        addTrack(7, 8)
        addTrack(8, 10)
        addTrack(10, 12)
        addTrack(12, 13)
        addTrack(36, 22)
        addTrack(22, 8)
        addTrack(8, 1)
        addTrack(40, 26)
        addTrack(26, 12)
        addTrack(12, 5)
        pSafePedestrianTrack = addTrack(36, 43)
        addTrack(40, 47)
    End Select

    ' Add Chase Tracks
    Select Case nRoadType
      Case RoadType.ROAD_H
        addTrack(21, 27)
      Case RoadType.ROAD_V
        addTrack(3, 45)
      Case RoadType.ROAD_C1
        addTrack(45, 24)
        addTrack(24, 27)
      Case RoadType.ROAD_C2
        addTrack(21, 24)
        addTrack(24, 45)
      Case RoadType.ROAD_C3
        addTrack(3, 24)
        addTrack(24, 27)
      Case RoadType.ROAD_C4
        addTrack(21, 24)
        addTrack(24, 3)
      Case RoadType.ROAD_T1
        addTrack(21, 24)
        addTrack(24, 27)
        addTrack(24, 45)
      Case RoadType.ROAD_T2
        addTrack(3, 24)
        addTrack(24, 45)
        addTrack(24, 27)
      Case RoadType.ROAD_T3
        addTrack(3, 24)
        addTrack(24, 45)
        addTrack(24, 21)
      Case RoadType.ROAD_T4
        addTrack(21, 24)
        addTrack(24, 27)
        addTrack(24, 3)
      Case RoadType.ROAD_X
        addTrack(3, 24)
        addTrack(27, 24)
        addTrack(45, 24)
        addTrack(21, 24)
    End Select

    ' Road traffic tracks
    Select Case nRoadType
      Case RoadType.ROAD_H
        pSafeCarTrack = addTrack(14, 20)
        addTrack(28, 34)
      Case RoadType.ROAD_V
        addTrack(2, 44)
        pSafeCarTrack = addTrack(4, 46)
      Case RoadType.ROAD_C1
        pSafeCarTrack = addTrack(44, 16)
        addTrack(16, 20)
        addTrack(46, 32)
        addTrack(32, 34)
      Case RoadType.ROAD_C2
        pSafeCarTrack = addTrack(14, 18)
        addTrack(18, 46)
        addTrack(28, 30)
        addTrack(30, 44)
      Case RoadType.ROAD_C3
        addTrack(2, 30)
        addTrack(30, 34)
        pSafeCarTrack = addTrack(4, 18)
        addTrack(18, 20)
      Case RoadType.ROAD_C4
        addTrack(2, 16)
        addTrack(16, 14)
        pSafeCarTrack = addTrack(4, 32)
        addTrack(32, 28)
      Case RoadType.ROAD_T1
        addTrack(14, 16)
        addTrack(16, 18)
        addTrack(18, 20)
        addTrack(28, 30)
        addTrack(30, 32)
        addTrack(32, 34)
        addTrack(16, 30)
        addTrack(30, 44)
        addTrack(18, 32)
        addTrack(32, 46)
      Case RoadType.ROAD_T4
        addTrack(14, 16)
        addTrack(16, 18)
        addTrack(18, 20)
        addTrack(28, 30)
        addTrack(30, 32)
        addTrack(32, 34)
        addTrack(16, 30)
        addTrack(16, 2)
        addTrack(18, 32)
        addTrack(18, 4)
      Case RoadType.ROAD_T2
        addTrack(2, 16)
        addTrack(16, 30)
        addTrack(30, 44)
        addTrack(4, 18)
        addTrack(18, 32)
        addTrack(32, 46)
        addTrack(16, 18)
        addTrack(18, 20)
        addTrack(30, 32)
        addTrack(32, 34)
      Case RoadType.ROAD_T3
        addTrack(2, 16)
        addTrack(16, 30)
        addTrack(30, 44)
        addTrack(4, 18)
        addTrack(18, 32)
        addTrack(32, 46)
        addTrack(14, 16)
        addTrack(16, 18)
        addTrack(28, 30)
        addTrack(30, 32)
      Case RoadType.ROAD_X
        addTrack(2, 16)
        addTrack(16, 30)
        addTrack(30, 44)
        addTrack(4, 18)
        addTrack(18, 32)
        addTrack(32, 46)
        addTrack(14, 16)
        addTrack(16, 18)
        addTrack(18, 20)
        addTrack(28, 30)
        addTrack(30, 32)
        addTrack(32, 34)
    End Select

    ' Stop Patterns
    ' .PO.OP.
    ' PP.P.PP
    ' O.O.O.O
    ' .P...P.
    ' O.O.O.O
    ' PP.P.PP
    ' .PO.OP.

    ' .PO.OP.
    ' PP.P.PP
    ' O.X.X.O
    ' .P...P.
    ' O.X.X.O
    ' PP.P.PP
    ' .PO.OP.

    ' .PO.OP.
    ' PP.X.PP
    ' O.X.X.O
    ' .X...X.
    ' O.X.X.O
    ' PP.X.PP
    ' .PO.OP.

    Dim stopmap = Function(s As String) As StopPattern
                    Dim p As New StopPattern()
                    For i = 0 To s.Length - 1
                      p.bStop(i) = (s(i) = "X")
                    Next
                    Return p
                  End Function

    Select Case nRoadType
      Case RoadType.ROAD_H, RoadType.ROAD_V, RoadType.ROAD_C1, RoadType.ROAD_C2, RoadType.ROAD_C3, RoadType.ROAD_C4
        ' Allow all
        ' vStopPattern.push_back(stopmap(".PO.OP." "PP.P.PP" "O.O.O.O" ".P...P." "O.O.O.O" "PP.P.PP" ".PO.OP."))
      Case RoadType.ROAD_X
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "O.O.O.O" &
                                         ".X...X." &
                                         "X.X.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "X.X.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow North Traffic
        vStopPattern.Add(stopmap(".PX.OP." &
                                         "PP.X.PP" &
                                         "X.X.O.O" &
                                         ".X...X." &
                                         "O.O.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain North Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.O.O" &
                                         ".X...X." &
                                         "O.O.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow EAST Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.X.X" &
                                         ".X...X." &
                                         "O.O.O.O" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain East Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.X.X" &
                                         ".X...X." &
                                         "O.O.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "O.O.X.X" &
                                         "PP.X.PP" &
                                         ".PO.XP."))
        ' Drain SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "O.O.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
      Case RoadType.ROAD_T1
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow West Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "O.O.O.O" &
                                         ".X...X." &
                                         "X.X.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain West Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "X.X.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow EAST Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "O.O.O.O" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain East Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "O.O.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow SOUTH Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "O.O.X.X" &
                                         "PP.X.PP" &
                                         ".PO.XP."))
        ' Drain SOUTH Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.O.O.O" &
                                         ".X...X." &
                                         "O.O.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
      Case RoadType.ROAD_T2
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.X.X" &
                                         ".P...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow North Traffic
        vStopPattern.Add(stopmap(".PX.OP." &
                                        "PP.X.PP" &
                                        "X.X.O.O" &
                                        ".P...X." &
                                        "X.X.O.X" &
                                        "PP.X.PP" &
                                        ".PX.OP."))
        ' Drain North Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.O.O" &
                                         ".P...X." &
                                         "X.X.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.P.PP" &
                                         "X.X.X.X" &
                                         ".P...P." &
                                         "X.X.X.X" &
                                         "PP.P.PP" &
                                         ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                         "PP.X.PP" &
                                         "X.X.X.X" &
                                         ".X...X." &
                                         "X.X.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
        ' Allow EAST Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.X.X" &
                                         ".P...X." &
                                         "X.O.O.O" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Drain East Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.X.X" &
                                         ".P...X." &
                                         "X.O.O.X" &
                                         "PP.X.PP" &
                                         ".PX.OP."))
        ' Allow SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.O.O" &
                                         ".P...X." &
                                         "X.O.X.X" &
                                         "PP.X.PP" &
                                         ".PO.XP."))
        ' Drain SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                         "PP.X.PP" &
                                         "X.O.O.O" &
                                         ".P...X." &
                                         "X.O.X.X" &
                                         "PP.X.PP" &
                                         ".PX.XP."))
      Case RoadType.ROAD_T3
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.P.PP" &
                                      "X.X.X.X" &
                                      ".P...P." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.X.X" &
                                      ".X...P." &
                                      "X.X.X.X" &
                                      "PP.X.PP" &
                                      ".PX.XP."))
        ' Allow West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "O.O.O.X" &
                                      ".X...P." &
                                      "X.X.O.X" &
                                      "PP.X.PP" &
                                      ".PX.OP."))
        ' Drain West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.O.X" &
                                      ".X...P." &
                                      "X.X.O.X" &
                                      "PP.X.PP" &
                                      ".PX.OP."))
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.P.PP" &
                                      "X.X.X.X" &
                                      ".P...P." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.X.X" &
                                      ".X...X." &
                                      "X.X.X.X" &
                                      "PP.X.PP" &
                                      ".PX.XP."))
        ' Allow North Traffic
        vStopPattern.Add(stopmap(".PX.OP." &
                                      "PP.X.PP" &
                                      "X.X.O.X" &
                                      ".X...P." &
                                      "O.O.O.X" &
                                      "PP.X.PP" &
                                      ".PX.OP."))
        ' Drain North Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.O.X" &
                                      ".X...P." &
                                      "O.O.O.X" &
                                      "PP.X.PP" &
                                      ".PX.OP."))
        ' Allow SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.X.X" &
                                      ".X...P." &
                                      "O.O.X.X" &
                                      "PP.X.PP" &
                                      ".PO.XP."))
        ' Drain SOUTH Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.X.X" &
                                      ".X...P." &
                                      "O.O.X.X" &
                                      "PP.X.PP" &
                                      ".PX.XP."))
      Case RoadType.ROAD_T4
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.P.PP" &
                                      "X.X.X.X" &
                                      ".P...P." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.X.X" &
                                      ".X...X." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Allow West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "O.O.O.O" &
                                      ".X...X." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain West Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.O.O" &
                                      ".X...X." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Allow North Traffic
        vStopPattern.Add(stopmap(".PX.OP." &
                                      "PP.X.PP" &
                                      "X.X.O.O" &
                                      ".X...X." &
                                      "O.O.O.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain North Traffic
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.O.O" &
                                      ".X...X." &
                                      "O.O.O.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Allow Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.P.PP" &
                                      "X.X.X.X" &
                                      ".P...P." &
                                      "X.X.X.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain Pedestrians
        vStopPattern.Add(stopmap(".PX.XP." &
                                      "PP.X.PP" &
                                      "X.X.X.X" &
                                      ".X...X." &
                                      "X.X.X.X" &
                                      "PP.X.PP" &
                                      ".PX.XP."))
        ' Allow EAST Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.X.X" &
                                      ".X...X." &
                                      "O.O.O.O" &
                                      "PP.P.PP" &
                                      ".PX.XP."))
        ' Drain East Traffic
        vStopPattern.Add(stopmap(".PO.XP." &
                                      "PP.X.PP" &
                                      "X.O.X.X" &
                                      ".X...X." &
                                      "O.O.O.X" &
                                      "PP.P.PP" &
                                      ".PX.XP."))

    End Select

  End Sub

  Public Overrides Function LinkAssets(mapTextures As Dictionary(Of String, Sprite), mapMesh As Dictionary(Of String, Gfx3D.Mesh), mapTransforms As Dictionary(Of String, Gfx3D.Mat4x4)) As Boolean
    meshUnitQuad = mapMesh("UnitQuad")
    sprRoadTex(RoadType.ROAD_V) = mapTextures("Road_V")
    sprRoadTex(RoadType.ROAD_H) = mapTextures("Road_H")
    sprRoadTex(RoadType.ROAD_C1) = mapTextures("Road_C1")
    sprRoadTex(RoadType.ROAD_T1) = mapTextures("Road_T1")
    sprRoadTex(RoadType.ROAD_C2) = mapTextures("Road_C2")
    sprRoadTex(RoadType.ROAD_T2) = mapTextures("Road_T2")
    sprRoadTex(RoadType.ROAD_X) = mapTextures("Road_X")
    sprRoadTex(RoadType.ROAD_T3) = mapTextures("Road_T3")
    sprRoadTex(RoadType.ROAD_C3) = mapTextures("Road_C3")
    sprRoadTex(RoadType.ROAD_T4) = mapTextures("Road_T4")
    sprRoadTex(RoadType.ROAD_C4) = mapTextures("Road_C4")
    Return False
  End Function

  Public Overrides Function Update(fElapsedTime As Single) As Boolean

    If Not vStopPattern.Any Then
      Return False
    End If

    fStopPatternTimer += fElapsedTime
    If fStopPatternTimer >= 5.0F Then
      fStopPatternTimer -= 5.0F
      nCurrentStopPattern += 1
      nCurrentStopPattern = nCurrentStopPattern Mod vStopPattern.Count
      For i = 0 To 48
        If pNaviNodes(i) IsNot Nothing Then
          pNaviNodes(i).bBlock = vStopPattern(nCurrentStopPattern).bStop(i)
        End If
      Next
    End If

    Return False

  End Function

  Public Overrides Function DrawBase(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean
    Dim matWorld = Gfx3D.Math.Mat_MakeTranslation(nWorldX, nWorldY, 0.0F)
    pipe.SetTransform(matWorld)
    pipe.SetTexture(sprRoadTex(nRoadType))
    pipe.Render(meshUnitQuad.Tris, Gfx3D.RenderFlags.RenderCullCw Or Gfx3D.RenderFlags.RenderDepth Or Gfx3D.RenderFlags.RenderTextured)
    Return False
  End Function

  Public Overrides Function DrawAlpha(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean
    Return False
  End Function

  Public Overrides Function DrawDebug(pge As PixelGameEngine, pipe As Gfx3D.PipeLine) As Boolean

    ' Draw Automata navigation tracks
    For Each track In listTracks
      Dim p1 = New Gfx3D.Vec3d(track.node(0).pos.x, track.node(0).pos.y, 0.0F)
      Dim p2 = New Gfx3D.Vec3d(track.node(1).pos.x, track.node(1).pos.y, 0.0F)
      pipe.RenderLine(p1, p2, Presets.Cyan)
    Next

    For i As Integer = 0 To 48
      If pNaviNodes(i) IsNot Nothing Then
        Dim p1 = New Gfx3D.Vec3d(pNaviNodes(i).pos.x, pNaviNodes(i).pos.y, 0.01F)
        pipe.RenderCircleXZ(p1, 0.03F, If(pNaviNodes(i).bBlock, Presets.Red, Presets.Green))
      End If
    Next

    Return False

  End Function

End Class