Imports System.Net.Http
Imports System.Text.Json

Module Program

  Sub Main() 'args As String())

    Dim documents As New List(Of (Name As String, Url As String)) From {
      ("Community.VisualBasic", "https://api.github.com/repos/DualBrain/Community.VisualBasic/releases"),
      ("vbPixelGameEngine", "https://api.github.com/repos/DualBrain/vbPixelGameEngine/releases"),
      ("vbConsoleGameEngine", "https://api.github.com/repos/DualBrain/vbConsoleGameEngine/releases"),
      ("Solitaire", "https://api.github.com/repos/DualBrain/Solitaire/releases"),
      ("CheckersSolitaire", "https://api.github.com/repos/DualBrain/CheckersSolitaire/releases"),
      ("Nibbles", "https://api.github.com/repos/DualBrain/Nibbles/releases"),
      ("Raster-Master", "https://api.github.com/repos/RetroNick2020/raster-master/"),
      ("olcPixelGameEngine", "https://api.github.com/repos/OneLoneCoder/olcPixelGameEngine/"),
      ("QB64 (Official)", "https://api.github.com/repos/qb64official/QB64/releases"),
      ("QB64 (Team)", "https://api.github.com/repos/qb64team/QB64/"),
      ("QB64 (Galleon)", "https://api.github.com/repos/Galleondragon/qb64/"),
      ("QB64pe", "https://api.github.com/repos/QB64-Phoenix-Edition/QB64pe/releases")}

    For index = 0 To documents.Count - 1

      Console.WriteLine("*****************************************************************")
      Console.WriteLine($"* {documents(index).Name}")
      Console.WriteLine("*****************************************************************")

      Dim totalTotal = 0

      ' Create an instance of HttpClient
      Using client As New HttpClient()

        ' Set the base address for the API
        client.BaseAddress = New Uri(documents(index).Url)

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
            Dim releaseCount = 0

            ' Iterate through the releases
            For Each release In releases.EnumerateArray()

              releaseCount += 1

              ' Access and print the values
              Dim tagName = release.GetProperty("tag_name").GetString()
              'Dim name = release.GetProperty("name").GetString()
              Dim createdAt = release.GetProperty("created_at").GetDateTime()

              Console.Write($"{tagName} ({createdAt})") '; ")

              ' Check if assets property exists
              Dim assets As JsonElement = Nothing
              If release.TryGetProperty("assets", assets) Then

                ' Iterate through the assets
                Dim total = 0
                For Each asset As JsonElement In assets.EnumerateArray()
                  Dim downloadCount = asset.GetProperty("download_count").GetInt32()
                  Dim assetName = asset.GetProperty("name").GetString()
                  'Console.WriteLine($"  {assetName}: {downloadCount}")
                  total += downloadCount
                Next

                'Console.WriteLine($"total: {total}")
                Console.WriteLine($": {total}")

                totalTotal += total

              End If

            Next

            If releaseCount > 1 AndAlso totalTotal > 0 Then
              'Console.WriteLine("=================================================================")
              Console.WriteLine($"(combined: {totalTotal})")
              'Console.WriteLine("-------------------------------")
            End If

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
