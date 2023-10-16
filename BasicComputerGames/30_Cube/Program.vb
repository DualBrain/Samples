' Copyright (c) Cory Smith. All rights reserved.
' Licensed under the MIT license.

Imports System.Console

Module Program

	Sub Main() 'args As String())

		Dim A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, W, X, Y, Z As Single
		Dim B7, A1, Z1, Z2 As Single

		WriteLine($"{TAB(34)}CUBE")
		WriteLine($"{TAB(15)}CREATIVE COMPUTING  MORRISTOWN, NEW JERSEY")
		WriteLine() : WriteLine() : WriteLine()
		WriteLine("DO YOU WANT TO SEE THE INSTRUCTIONS? (YES--1,NO--0)")
		Input(B7)
		If B7 = 0 Then GoTo 370
		WriteLine("THIS IS A GAME IN WHICH YOU WILL BE PLAYING AGAINST THE")
		WriteLine("RANDOM DECISION OF THE COMPUTER. THE FIELD OF PLAY IS A")
		WriteLine("CUBE OF SIDE 3. ANY OF THE 27 LOCATIONS CAN BE DESIGNATED")
		WriteLine("BY INPUTING THREE NUMBERS SUCH AS 2,3,1. AT THE START,")
		WriteLine("YOU ARE AUTOMATICALLY AT LOCATION 1,1,1. THE OBJECT OF")
		WriteLine("THE GAME IS TO GET TO LOCATION 3,3,3. ONE MINOR DETAIL:")
		WriteLine("THE COMPUTER WILL PICK, AT RANDOM, 5 LOCATIONS AT WHICH")
		WriteLine("IT WILL PLANT LAND MINES. IF YOU HIT ONE OF THESE LOCATIONS")
		WriteLine("YOU LOSE. ONE OTHER DETAIL: YOU MAY MOVE ONLY ONE SPACE ")
		WriteLine("IN ONE DIRECTION EACH MOVE. FOR  EXAMPLE: FROM 1,1,2 YOU")
		WriteLine("MAY MOVE TO 2,1,2 OR 1,1,3. YOU MAY NOT CHANGE")
		WriteLine("TWO OF THE NUMBERS ON THE SAME MOVE. IF YOU MAKE AN ILLEGAL")
		WriteLine("MOVE, YOU LOSE AND THE COMPUTER TAKES THE MONEY YOU MAY")
		WriteLine("HAVE BET ON THAT ROUND.")
		WriteLine()
		WriteLine()
		WriteLine("ALL YES OR NO QUESTIONS WILL BE ANSWERED BY A 1 FOR YES")
		WriteLine("OR A 0 (ZERO) FOR NO.")
		WriteLine()
		WriteLine("WHEN STATING THE AMOUNT OF A WAGER, PRINT ONLY THE NUMBER")
		WriteLine("OF DOLLARS (EXAMPLE: 250)  YOU ARE AUTOMATICALLY STARTED WITH")
		WriteLine("500 DOLLARS IN YOUR ACCOUNT.")
		WriteLine()
		WriteLine("GOOD LUCK!")
370:
		A1 = 500
380:
		A = Int(3 * (Rnd(X)))
		If A <> 0 Then GoTo 410
		A = 3
410:
		B = Int(3 * (Rnd(X)))
		If B <> 0 Then GoTo 440
		B = 2
440:
		C = Int(3 * (Rnd(X)))
		If C <> 0 Then GoTo 470
		C = 3
470:
		D = Int(3 * (Rnd(X)))
		If D <> 0 Then GoTo 500
		D = 1
500:
		E = Int(3 * (Rnd(X)))
		If E <> 0 Then GoTo 530
		E = 3
530:
		F = Int(3 * (Rnd(X)))
		If F <> 0 Then GoTo 560
		F = 3
560:
		G = Int(3 * (Rnd(X)))
		If G <> 0 Then GoTo 590
		G = 3
590:
		H = Int(3 * (Rnd(X)))
		If H <> 0 Then GoTo 620
		H = 3
620:
		I = Int(3 * (Rnd(X)))
		If I <> 0 Then GoTo 650
		I = 2
650:
		J = Int(3 * (Rnd(X)))
		If J <> 0 Then GoTo 680
		J = 3
680:
		K = Int(3 * (Rnd(X)))
		If K <> 0 Then GoTo 710
		K = 2
710:
		L = Int(3 * (Rnd(X)))
		If L <> 0 Then GoTo 740
		L = 3
740:
		M = Int(3 * (Rnd(X)))
		If M <> 0 Then GoTo 770
		M = 3
770:
		N = Int(3 * (Rnd(X)))
		If N <> 0 Then GoTo 800
		N = 1
800:
		O = Int(3 * (Rnd(X)))
		If O <> 0 Then GoTo 830
		O = 3
830:
		WriteLine("WANT TO MAKE A WAGER?")
		Input(Z)
		If Z = 0 Then GoTo 880
		Write("HOW MUCH ")
870:
		Input(Z1)
		If A1 < Z1 Then GoTo 1522
880:
		W = 1
		X = 1
		Y = 1
		WriteLine()
		Write("IT'S YOUR MOVE:  ")
930:
		Input(P, Q, R)
		If P > W + 1 Then GoTo 1030
		If P = W + 1 Then GoTo 1000
		If Q > X + 1 Then GoTo 1030
		If Q = (X + 1) Then GoTo 1010
		If R > (Y + 1) Then GoTo 1030
		GoTo 1050
1000:
		If Q >= X + 1 Then GoTo 1030
1010:
		If R >= Y + 1 Then GoTo 1030
		GoTo 1050
1030:
		WriteLine() : WriteLine("ILLEGAL MOVE. YOU LOSE.")
		GoTo 1440
1050:
		W = P
		X = Q
		Y = R
		If P = 3 Then GoTo 1100
		GoTo 1130
1100:
		If Q = 3 Then GoTo 1120
		GoTo 1130
1120:
		If R = 3 Then GoTo 1530
1130:
		If P = A Then GoTo 1150
		GoTo 1180
1150:
		If Q = B Then GoTo 1170
		GoTo 1180
1170:
		If R = C Then GoTo 1400
1180:
		If P = D Then GoTo 1200
		GoTo 1230
1200:
		If Q = E Then GoTo 1220
		GoTo 1230
1220:
		If R = F Then GoTo 1400
1230:
		If P = G Then GoTo 1250
		GoTo 1280
1250:
		If Q = H Then GoTo 1270
		GoTo 1280
1270:
		If R = I Then GoTo 1400
1280:
		If P = J Then GoTo 1300
		GoTo 1330
1300:
		If Q = K Then GoTo 1320
		GoTo 1330
1320:
		If R = L Then GoTo 1400
1330:
		If P = M Then GoTo 1350
		GoTo 1380
1350:
		If Q = N Then GoTo 1370
		GoTo 1380
1370:
		If R = O Then GoTo 1400
1380:
		Write("NEXT MOVE: ")
		GoTo 930
1400:
		WriteLine("******BANG******")
		WriteLine("YOU LOSE!")
		WriteLine()
		WriteLine()
1440:
		If Z = 0 Then GoTo 1580
		WriteLine()
		Z2 = A1 - Z1
		If Z2 > 0 Then GoTo 1500
		WriteLine("YOU BUST.")
		GoTo 1610
1500:
		WriteLine($" YOU NOW HAVE {Z2} DOLLARS.")
		A1 = Z2
		GoTo 1580
1522:
		Write("TRIED TO FOOL ME; BET AGAIN")
		GoTo 870
1530:
		WriteLine("CONGRATULATIONS!")
		If Z = 0 Then GoTo 1580
		Z2 = A1 + Z1
		WriteLine($"YOU NOW HAVE {Z2} DOLLARS.")
		A1 = Z2
1580:
		Write("DO YOU WANT TO TRY AGAIN ")
		Input(S)
		If S = 1 Then GoTo 380
1610:
		WriteLine("TOUGH LUCK!")
		WriteLine()
		WriteLine("GOODBYE.")
		End

	End Sub

	Private Sub Input(ByRef value As Single)
		value = Val(ReadLine())
	End Sub

	Private Sub Input(ByRef x As Single, ByRef y As Single, ByRef z As Single)
		Dim response = ReadLine()
		Dim values = Split(response, ","c)
		If values.Length = 3 Then
			x = Val(values(0))
			y = Val(values(1))
			z = Val(values(2))
		Else
			Stop
		End If
	End Sub

End Module