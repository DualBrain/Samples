' Copyright (c) Cory Smith. All rights reserved.
' Licensed under the MIT license.

Imports System.Console

Module Program

  Sub Main()

    Dim C, H, Q, R, S, V, Z, X As Single

    WriteLine(Space(28) & "AMAZING PROGRAM")
    WriteLine(Space(15) & "CREATIVE COMPUTING  MORRISTOWN, NEW JERSEY")
    WriteLine() ': WriteLine() : WriteLine() : WriteLine()
100:
    Write("WHAT ARE YOUR WIDTH AND LENGTH? ")
    Dim response = ReadLine()
    Dim values = Split(response, ","c)
    If values.Count <> 2 Then GoTo 100
    H = values(0) : V = values(1)
    If H <> 1 AndAlso V <> 1 Then GoTo 110
    WriteLine("MEANINGLESS DIMENSIONS.  TRY AGAIN.") : GoTo 100
110:
    Dim W(H, V), VV(H, V)
    WriteLine()
    'WriteLine()
    'WriteLine()
    'WriteLine()
    Q = 0 : Z = 0 : X = Int(Rnd(1) * H + 1)
    For I = 1 To H
      If I = X Then GoTo 173
      Write(".--") : GoTo 180
173:
      Write(".  ")
180:
    Next I
    WriteLine(".")
    C = 1 : W(X, 1) = C : C = C + 1
    R = X : S = 1 : GoTo 260
210:
    If R <> H Then GoTo 240
    If S <> V Then GoTo 230
    R = 1 : S = 1 : GoTo 250
230:
    R = 1 : S = S + 1 : GoTo 250
240:
    R = R + 1
250:
    If W(R, S) = 0 Then GoTo 210
260:
    If R - 1 = 0 Then GoTo 530
    If W(R - 1, S) <> 0 Then GoTo 530
    If S - 1 = 0 Then GoTo 390
    If W(R, S - 1) <> 0 Then GoTo 390
    If R = H Then GoTo 330
    If W(R + 1, S) <> 0 Then GoTo 330
    X = Int(Rnd(1) * 3 + 1)
    'On X GoTo 790,820,860
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 820
      Case 3 : GoTo 860
      Case Else
        Stop
    End Select
330:
    If S <> V Then GoTo 340
    If Z = 1 Then GoTo 370
    Q = 1 : GoTo 350
340:
    If W(R, S + 1) <> 0 Then GoTo 370
350:
    X = Int(Rnd(1) * 3 + 1)
    'On X GoTo 790,820,910
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 820
      Case 3 : GoTo 910
      Case Else
        Stop
    End Select
370:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 790,820
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 820
      Case Else
        Stop
    End Select
390:
    If R = H Then GoTo 470
    If W(R + 1, S) <> 0 Then GoTo 470
    If S <> V Then GoTo 420
    If Z = 1 Then GoTo 450
    Q = 1 : GoTo 430
420:
    If W(R, S + 1) <> 0 Then GoTo 450
430:
    X = Int(Rnd(1) * 3 + 1)
    'On X GoTo 790,860,910
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 860
      Case 3 : GoTo 910
      Case Else
        Stop
    End Select
450:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 790,860
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 860
      Case Else
        Stop
    End Select
470:
    If S <> V Then GoTo 490
    If Z = 1 Then GoTo 520
    Q = 1 : GoTo 500
490:
    If W(R, S + 1) <> 0 Then GoTo 520
500:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 790,910
    Select Case X
      Case 1 : GoTo 790
      Case 2 : GoTo 910
      Case Else
        Stop
    End Select
520:
    GoTo 790
530:
    If S - 1 = 0 Then GoTo 670
    If W(R, S - 1) <> 0 Then GoTo 670
    If R = H Then GoTo 610
    If W(R + 1, S) <> 0 Then GoTo 610
    If S <> V Then GoTo 560
    If Z = 1 Then GoTo 590
    Q = 1 : GoTo 570
560:
    If W(R, S + 1) <> 0 Then GoTo 590
570:
    X = Int(Rnd(1) * 3 + 1)
    'On X GoTo 820,860,910
    Select Case X
      Case 1 : GoTo 820
      Case 2 : GoTo 860
      Case 3 : GoTo 910
      Case Else
        Stop
    End Select
590:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 820,860
    Select Case X
      Case 1 : GoTo 820
      Case 2 : GoTo 860
      Case Else
        Stop
    End Select
610:
    If S <> V Then GoTo 630
    If Z = 1 Then GoTo 660
    Q = 1 : GoTo 640
630:
    If W(R, S + 1) <> 0 Then GoTo 660
640:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 820,910
    Select Case X
      Case 1 : GoTo 820
      Case 2 : GoTo 910
      Case Else
        Stop
    End Select
660:
    GoTo 820
670:
    If R = H Then GoTo 740
    If W(R + 1, S) <> 0 Then GoTo 740
    If S <> V Then GoTo 700
    If Z = 1 Then GoTo 730
    Q = 1 : GoTo 710
700:
    If W(R, S + 1) <> 0 Then GoTo 730
710:
    X = Int(Rnd(1) * 2 + 1)
    'On X GoTo 860,910
    Select Case X
      Case 1 : GoTo 860
      Case 2 : GoTo 910
      Case Else
        Stop
    End Select
730:
    GoTo 860
740:
    If S <> V Then GoTo 760
    If Z = 1 Then GoTo 780
    Q = 1 : GoTo 770
760:
    If W(R, S + 1) <> 0 Then GoTo 780
770:
    GoTo 910
780:
    GoTo 1000
790:
    W(R - 1, S) = C
    C = C + 1 : VV(R - 1, S) = 2 : R = R - 1
    If C = H * V + 1 Then GoTo 1010
    Q = 0 : GoTo 260
820:
    W(R, S - 1) = C
    C = C + 1
    VV(R, S - 1) = 1 : S = S - 1 : If C = H * V + 1 Then GoTo 1010
    Q = 0 : GoTo 260
860:
    W(R + 1, S) = C
    C = C + 1 : If VV(R, S) = 0 Then GoTo 880
    VV(R, S) = 3 : GoTo 890
880:
    VV(R, S) = 2
890:
    R = R + 1
    If C = H * V + 1 Then GoTo 1010
    GoTo 530
910:
    If Q = 1 Then GoTo 960
    W(R, S + 1) = C : C = C + 1 : If VV(R, S) = 0 Then GoTo 940
    VV(R, S) = 3 : GoTo 950
940:
    VV(R, S) = 1
950:
    S = S + 1 : If C = H * V + 1 Then GoTo 1010
    GoTo 260
960:
    Z = 1
    If VV(R, S) = 0 Then GoTo 980
    VV(R, S) = 3 : Q = 0 : GoTo 1000
980:
    VV(R, S) = 1 : Q = 0 : R = 1 : S = 1 : GoTo 250
1000:
    GoTo 210
1010:
    If Z = 1 Then GoTo 1015
    X = Int(Rnd(1) * H + 1)
    If VV(X, V) = 0 Then GoTo 1014
    VV(X, V) = 3 : GoTo 1015
1014:
    VV(X, V) = 1
1015:
    For J = 1 To V
      Write("I")
      For I = 1 To H
        If VV(I, J) < 2 Then GoTo 1030
        Write("   ")
        GoTo 1040
1030:
        Write("  I")
1040:
      Next I
      WriteLine()

      For I = 1 To H
        If VV(I, J) = 0 Then GoTo 1060
        If VV(I, J) = 2 Then GoTo 1060
        Write(":  ")
        GoTo 1070
1060:
        Write(":--")
1070:
      Next I
      WriteLine(".")
    Next J

  End Sub

End Module