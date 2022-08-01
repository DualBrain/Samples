' Copyright (c) Cory Smith. All rights reserved.
' Licensed under the MIT license.

Imports System.Console

Module Program

  Sub Main()

    Dim Q, M As Single

    Randomize(Timer)

    WriteLine(Space(26) & "ACEY DUCEY CARD GAME")
    WriteLine(Space(15) & "CREATIVE COMPUTING  MORRISTOWN, NEW JERSEY")
    WriteLine()
    WriteLine()
    WriteLine("ACEY-DUCEY IS PLAYED IN THE FOLLOWING MANNER ")
    WriteLine("THE DEALER (COMPUTER) DEALS TWO CARDS FACE UP")
    WriteLine("YOU HAVE AN OPTION TO BET OR NOT BET DEPENDING")
    WriteLine("ON WHETHER OR NOT YOU FEEL THE CARD WILL HAVE")
    WriteLine("A VALUE BETWEEN THE FIRST TWO.")
    WriteLine("IF YOU DO NOT WANT TO BET, INPUT A 0")
    'N = 100
Start:
    Q = 100
DisplayPool:
    WriteLine("YOU NOW HAVE " & Q & " DOLLARS.")
    WriteLine()
    GoTo DisplayCards
IncreasePool:
    Q += M
    GoTo DisplayPool
DecreasePool:
    Q -= M
    GoTo DisplayPool
DisplayCards:
    WriteLine("HERE ARE YOUR NEXT TWO CARDS: ")
DetermineFirstCard:
    Dim A = Int(14 * Rnd(1)) + 2
    If A < 2 Then GoTo DetermineFirstCard
    If A > 14 Then GoTo DetermineFirstCard
DetermineSecondCard:
    Dim B = Int(14 * Rnd(1)) + 2
    If B < 2 Then GoTo DetermineSecondCard
    If B > 14 Then GoTo DetermineSecondCard
    If A >= B Then GoTo DetermineFirstCard
    If A < 11 Then WriteLine(A)
    If A = 11 Then WriteLine("JACK")
    If A = 12 Then WriteLine("QUEEN")
    If A = 13 Then WriteLine("KING")
    If A = 14 Then WriteLine("ACE")
    If B < 11 Then WriteLine(B)
    If B = 11 Then WriteLine("JACK")
    If B = 12 Then WriteLine("QUEEN")
    If B = 13 Then WriteLine("KING")
    If B = 14 Then WriteLine("ACE") : WriteLine()
EnterBet:
    WriteLine()
    Write("WHAT IS YOUR BET? ") : Dim valid = Single.TryParse(ReadLine(), M)
    If M <> 0 Then GoTo ValidateBet
    WriteLine("CHICKEN!!")
    WriteLine()
    GoTo DisplayCards
ValidateBet:
    If M <= Q Then GoTo DetermineFinalCard
    WriteLine("SORRY, MY FRIEND, BUT YOU BET TOO MUCH.")
    WriteLine("YOU HAVE ONLY " & Q & " DOLLARS TO BET.")
    GoTo EnterBet
DetermineFinalCard:
    Dim C = Int(14 * Rnd(1)) + 2
    If C < 2 Then GoTo DetermineFinalCard
    If C > 14 Then GoTo DetermineFinalCard
    If C < 11 Then WriteLine(C)
    If C = 11 Then WriteLine("JACK")
    If C = 12 Then WriteLine("QUEEN")
    If C = 13 Then WriteLine("KING")
    If C = 14 Then WriteLine("ACE") : WriteLine()
    If C > A Then GoTo WinCheck
    GoTo Lose
WinCheck:
    If C >= B Then GoTo Lose
    WriteLine("YOU WIN!!!")
    GoTo IncreasePool
Lose:
    WriteLine("SORRY, YOU LOSE")
    If M < Q Then GoTo DecreasePool
    WriteLine() : WriteLine()
    WriteLine("SORRY, FRIEND, BUT YOU BLEW YOUR WAD.")
    WriteLine() : WriteLine()
    Write("TRY AGAIN (YES OR NO)? ") : Dim yesNo$ = ReadLine()
    WriteLine() : WriteLine()
    If yesNo$?.ToUpper = "YES" Then GoTo Start
    WriteLine("O.K., HOPE YOU HAD FUN!")

  End Sub

End Module