Option Explicit On
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text.Json

Module Program

  Sub Main() 'args As String())

    'Dim documents = {"https://api.github.com/repos/qb64official/QB64/releases",
    '                 "https://api.github.com/repos/qb64team/QB64/",
    '                 "https://api.github.com/repos/Galleondragon/qb64/",
    '                 "https://api.github.com/repos/QB64-Phoenix-Edition/QB64pe/releases"}

    'Dim documents = {"https://api.github.com/repos/OneLoneCoder/olcPixelGameEngine/"}

    'Dim documents = {"https://api.github.com/repos/RetroNick2020/raster-master/"}

    Dim documents = {"https://api.github.com/repos/gooofy/aqb/",
                     "https://api.github.com/repos/robhagemans/pcbasic/",
                     "https://api.github.com/repos/freebasic/fbc/"}

    'Dim documents = {"https://api.github.com/repos/opentk/opentk/"}

    'Dim documents = {"https://api.github.com/repos/FNA-XNA/FNA/",
    '                 "https://api.github.com/repos/raysan5/raylib/",
    '                 "https://api.github.com/repos/libsdl-org/SDL/"}

    For index = 0 To documents.Count - 1

      Dim totalTotal = 0

      ' Create an instance of HttpClient
      Using client As New HttpClient()

        ' Set the base address for the API
        client.BaseAddress = New Uri(documents(index))

        ' Set the user agent header required by GitHub API
        client.DefaultRequestHeaders.Add("User-Agent", "My-App")

        ' Send a GET request to the releases endpoint
        Using response = client.GetAsync("releases").Result

          ' Check if the request was successful
          If response.IsSuccessStatusCode Then

            ' Read the response content as JSON string
            Dim json = response.Content.ReadAsStringAsync().Result

            ' Print the JSON string
            'Console.WriteLine(json)

            ' Parse the JSON string
            Dim releases = JsonDocument.Parse(json).RootElement

            ' Iterate through the releases
            For Each release In releases.EnumerateArray()

              ' Access and print the values
              Dim tagName = release.GetProperty("tag_name").GetString()
              'Dim name = release.GetProperty("name").GetString()
              Dim createdAt = release.GetProperty("created_at").GetDateTime()

              Console.WriteLine($"{tagName} ({createdAt})")

              ' Check if assets property exists
              Dim assets As JsonElement = Nothing
              If release.TryGetProperty("assets", assets) Then

                ' Iterate through the assets
                Dim total = 0
                For Each asset As JsonElement In assets.EnumerateArray()
                  Dim downloadCount = asset.GetProperty("download_count").GetInt32()
                  Dim assetName = asset.GetProperty("name").GetString()
                  Console.WriteLine($"  {assetName}: {downloadCount}")
                  total += downloadCount
                Next

                Console.WriteLine($"  total: {total}")

                totalTotal += total

              End If

              Console.WriteLine()

            Next

            Console.WriteLine("===============================")
            Console.WriteLine($"Combined: {totalTotal}")
            Console.WriteLine("-------------------------------")

            Console.WriteLine()

          Else
            ' Print the error message if request failed
            Console.WriteLine("Error: " & response.StatusCode)
          End If

        End Using

      End Using

    Next

  End Sub

End Module
