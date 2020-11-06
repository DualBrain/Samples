Option Explicit On
Option Strict On
Option Infer On

Namespace Global.PlayString

  Module Program

    Sub Main(args As String())

      Console.WriteLine("PLAYground - GW-BASIC/QB 'PLAY' Macro Language Interpreter.")
      Console.WriteLine()

      If args.Length = 1 OrElse args.Length = 2 Then
        Dim outfilefile As String = If(args.Length = 2, args(1), Nothing)
        Dim _str2wav As New ParsePlayMacro(args(0))
        _str2wav.Generate()
        _str2wav.PlayOrSaveWav(outfilefile)
      ElseIf args.Length = 0 Then

        ' Jingle Bells
        '        Dim input = "t200l4o2mneel2el4eel2el4egl3cl8dl1el4ffl3fl8fl4fel2el8eel4edde
        'l2dgl4eel2el4eel2el4egl3cl8dl1el4ffl3fl8fl4fel2el8efl4ggfdl2c"
        ' Nibbles
        'Dim input = "MBT160O1L8CDEDCDL4ECC"
        'Dim input = "T160O1>L20CDEDCDL10ECC"
        'Dim input = "MBO0L16>CCCE"
        'Dim input = "MBO1L32EFGEFDC" 'TODO: This is supposed to be O0, not O1.
        ' Gorilla
        'Dim input = "MBO0L32EFGEFDC"
        'Dim input = "MBO0L16EFGEFDC"
        '        Dim input = "t120o1l16b9n0baan0bn0bn0baaan0b9n0baan0b
        'o2l16e-9n0e-d-d-n0e-n0e-n0e-d-d-d-n0e-9n0e-d-d-n0e-
        'o2l16g-9n0g-een0g-n0g-n0g-eeen0g-9n0g-een0g-
        'o2l16b9n0baan0g-n0g-n0g-eeen0o1b9n0baan0b
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC
        'T160O0L32EFGEFDC"
        ' Beatles...
        Dim input = "
        T255 
        O3 
        < < L2 A > > > L4 C# < A B > L2 C#.
        L2 N0 L4 C# < L2 B A F#
        A B A F# L1 E.
        < < L2 A > > > L4 C# < A B > L2 C#.
        L2 N0 L4 C# < L2 B A L2 F#.
        > L4 C# < L2 B A L1 B B L1 N0
        < < L2 A > > > L4 C# < L2 B > L2 C# < L2 A.
        > L4 C# < A B > L2 C#.
        < < L2 A > > > L4 C# < L2 B > C# < L2 A.
        L4 N0 A L2 B A
        L2 N0 > L2 C# < B A N0
        L4 F# A B E A B D A B A G# F# E"

        Do

          Console.WriteLine($"PLAY ""{input}""")

          Dim str2wav As New ParsePlayMacro(input)
          Try
            str2wav.Generate()
            str2wav.PlayOrSaveWav(Nothing)
          Catch ex As Exception
            Console.WriteLine(ex.Message)
          End Try

          Console.Write("> ")
          input = Console.ReadLine

          If input = "" Then Exit Do

        Loop

      Else
        Console.WriteLine("Usage: PLAYground {[string]} {[file]}" & vbCrLf)
        Console.WriteLine("Examples:")
        Console.WriteLine("> PLAYground")
        Console.WriteLine("> PLAYground MBT160O1L8CDEDCDL4ECC")
        Console.WriteLine("> PLAYground MBT160O1L8CDEDCDL4ECC test.wav")

      End If

    End Sub

  End Module

End Namespace