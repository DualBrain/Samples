' Copyright (c) Cory Smith. All rights reserved.
' Licensed under the MIT license.

Imports System.Console

Module Program

	Sub Main() 'args As String())

		Dim A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, W, X, Y, Z As Single
		Dim B7, A1, Z1, Z2 As Single

		WriteLine($"{Space(34)}CUBE")
		WriteLine($"{Space(15)}CREATIVE COMPUTING  MORRISTOWN, NEW JERSEY")
		WriteLine() : WriteLine() : WriteLine()
		WriteLine("DO YOU WANT TO SEE THE INSTRUCTIONS? (YES--1,NO--0)")
		Input(B7)
		If B7 = 0 Then GoTo Start
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
Start:
		A1 = 500
Again:
		A = Int(3 * (Rnd(X)))
		B = Int(3 * (Rnd(X)))
		C = Int(3 * (Rnd(X)))
		D = Int(3 * (Rnd(X)))
		E = Int(3 * (Rnd(X)))
		F = Int(3 * (Rnd(X)))
		G = Int(3 * (Rnd(X)))
		H = Int(3 * (Rnd(X)))
		I = Int(3 * (Rnd(X)))
		J = Int(3 * (Rnd(X)))
		K = Int(3 * (Rnd(X)))
		L = Int(3 * (Rnd(X)))
		M = Int(3 * (Rnd(X)))
		N = Int(3 * (Rnd(X)))
		O = Int(3 * (Rnd(X)))
		If A = 0 Then A = 3
		If B = 0 Then B = 2
		If C = 0 Then C = 3
		If D = 0 Then D = 1
		If E = 0 Then E = 3
		If F = 0 Then F = 3
		If G = 0 Then G = 3
		If H = 0 Then H = 3
		If I = 0 Then I = 2
		If J = 0 Then J = 3
		If K = 0 Then K = 2
		If L = 0 Then L = 3
		If M = 0 Then M = 3
		If N = 0 Then N = 1
		If O = 0 Then O = 3

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
		If S = 1 Then GoTo Again
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