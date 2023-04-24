Option Explicit On
Option Strict On
Option Infer On

Imports Olc

Public Class cAuto_Node

  Public pos As Vf2d
  Public bBlock As Boolean
  Public listTracks As New List(Of cAuto_Track)

  Public Sub New()
    pos = New Vf2d(0, 0)
  End Sub

  Public Sub New(worldPos As Vf2d)
    pos = New Vf2d(worldPos.x, worldPos.y)
  End Sub

End Class

Public Class cAuto_Track

  Public node(1) As cAuto_Node ' Two end nodes
  Public cell As cCell ' Pointer to host cell
  Public listAutos As New List(Of cAuto_Body)
  Public fTrackLength As Single = 1.0F

  Public Function GetPosition(t As Single, pStart As cAuto_Node) As Vf2d
    ' pStart indicates the node the automata first encounted this track
    If node(0) Is pStart Then
      Return node(0).pos + (node(1).pos - node(0).pos) * (t / fTrackLength)
    Else
      Return node(1).pos + (node(0).pos - node(1).pos) * (t / fTrackLength)
    End If
  End Function

End Class

Public Class cAuto_Body

  Public vAutoPos As New Vf2d(0.0F, 0.0F)
  Public fAutoPos As Single = 0.0F ' Location of automata along track
  Public fAutoLength As Single = 0.0F ' Physical length of automata
  Public pCurrentTrack As cAuto_Track = Nothing
  Public pTrackOriginNode As cAuto_Node = Nothing

  Public Sub UpdateAuto(fElapsedTime As Single)

    ' Work out which node is the target destination
    Dim pExitNode = pCurrentTrack.node(0)
    If pExitNode Is pTrackOriginNode Then
      pExitNode = pCurrentTrack.node(1)
    End If

    Dim bAutomataCanMove = True
    Dim fDistanceToAutoInFront = 1.0F

    ' First check if the vehicle overlaps with the one in front of it

    ' Get an iterator for this automata
    Dim itThisAutomata = pCurrentTrack.listAutos.Find(Function(automata) automata Is Me)

    If itThisAutomata Is Nothing Then Return

    ' If this automata is at the front of this track segment
    If itThisAutomata Is pCurrentTrack.listAutos.First() Then
      ' Then check all the following track segments. Take the position of
      ' each vehicle at the back of the track segments auto list
      For Each track In pExitNode.listTracks
        If track IsNot pCurrentTrack AndAlso track.listAutos.Any Then
          ' Get Auto at back
          Dim fDistanceFromTrackStartToAutoRear = track.listAutos.Last().fAutoPos - track.listAutos.Last().fAutoLength

          If itThisAutomata.fAutoPos < (pCurrentTrack.fTrackLength + fDistanceFromTrackStartToAutoRear - fAutoLength) Then
            ' Move Automata along track, as there is space
            ' bAutomataCanMove = True
            fDistanceToAutoInFront = (pCurrentTrack.fTrackLength + fDistanceFromTrackStartToAutoRear - 0.1F) - itThisAutomata.fAutoPos
          Else
            ' No space, so do not move automata
            bAutomataCanMove = False
          End If
        Else
          ' Track in front was empty, node is clear to pass through so
          ' bAutomataCanMove = True
        End If
      Next
    Else

      ' Get the automata in front
      ' auto itAutomataInFront = itThisAutomata;
      ' itAutomataInFront--;

      Dim itAutomataInFront As cAuto_Body = itThisAutomata
      For index = 0 To pCurrentTrack.listAutos.Count - 1
        If pCurrentTrack.listAutos(index) Is itThisAutomata Then
          itAutomataInFront = pCurrentTrack.listAutos(index - 1) : Exit For
        End If
      Next

      ' If the distance between the front of the automata in front and the fornt of this automata
      ' is greater than the length of the automata in front, then there is space for this automata
      ' to enter
      If Math.Abs(itAutomataInFront.fAutoPos - itThisAutomata.fAutoPos) > (itAutomataInFront.fAutoLength + 0.1F) Then
        ' Move Automata along track
        ' bAutomataCanMove = True
        fDistanceToAutoInFront = (itAutomataInFront.fAutoPos - itAutomataInFront.fAutoLength - 0.1F) - itThisAutomata.fAutoPos
      Else
        ' No space, so do not move automata
        bAutomataCanMove = False
      End If
    End If

    If bAutomataCanMove Then
      If fDistanceToAutoInFront > pCurrentTrack.fTrackLength Then fDistanceToAutoInFront = pCurrentTrack.fTrackLength
      fAutoPos += fElapsedTime * Math.Max(fDistanceToAutoInFront, 1.0F) * If(fAutoLength < 0.1F, 0.3F, 0.5F)
    End If

    If (fAutoPos >= pCurrentTrack.fTrackLength) Then

      'Automata has reached end of current track

      'Check if it can transition beyond node
      If (Not pExitNode.bBlock) Then
        'It can, so reset position along track back to start
        fAutoPos -= pCurrentTrack.fTrackLength

        'Choose a track from the node not equal to this one, that has an unblocked exit node
        'For now choose at random
        Dim pNewTrack As cAuto_Track = Nothing

        If (pExitNode.listTracks.Count = 2) Then
          'Automata is travelling along straight joined sections, one of the 
          'tracks is the track its just come in on, the other is the exit, so
          'choose the exit.
          'Dim it = pExitNode.listTracks.GetEnumerator()
          'pNewTrack = it.Current
          'If pCurrentTrack Is pNewTrack Then
          '  If it.MoveNext() Then
          '    pNewTrack = it.Current
          '  End If
          'End If
          For Each entry In pExitNode.listTracks
            If pCurrentTrack IsNot entry Then
              pNewTrack = entry
              Exit For
            End If
          Next
        Else
          'Automata has reached a junction with several exits
          While (pNewTrack Is Nothing)
            Dim i As Integer = Pge.Rand() Mod pExitNode.listTracks.Count
            Dim j As Integer = 0
            For Each it As cAuto_Track In pExitNode.listTracks
              Dim track As cAuto_Track = it

              'Work out which node is the target destination
              Dim pNewExitNode As cAuto_Node = track.node(0)
              If (pNewExitNode Is pExitNode) Then
                pNewExitNode = track.node(1)
              End If

              If (j = i AndAlso track IsNot pCurrentTrack AndAlso Not pNewExitNode.bBlock) Then
                pNewTrack = track
                Exit For
              End If

              j += 1
            Next
          End While
        End If

        'Change to new track, the origin node of the next
        'track is the same as the exit node to the current track
        pTrackOriginNode = pExitNode

        'Remove the automata from the front of the queue
        'on the current track
        pCurrentTrack.listAutos.RemoveAt(0)

        'Switch the automatas track link to the new track
        pCurrentTrack = pNewTrack

        'Push the automata onto the back of the new track queue
        pCurrentTrack.listAutos.Add(Me)

      Else
        'It cant pass the node, so clamp automata at this location
        fAutoPos = pCurrentTrack.fTrackLength
      End If

    Else
      'Automata is travelling
      vAutoPos = pCurrentTrack.GetPosition(fAutoPos, pTrackOriginNode)
    End If

  End Sub

End Class