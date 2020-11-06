Imports System
Imports System.Management
Imports NAudio.CoreAudioApi
Imports NAudio.Wave

Module Program

  Sub Main(args As String())

    Console.WriteLine("******* Using WMI ******* ")

    Dim objSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice")
    Dim objCollection = objSearcher.Get()
    For Each d In objCollection
      For Each p In d.Properties
        Select Case p.Name
          Case "Name"
            Console.WriteLine($"{p.Value}")
          Case Else
        End Select
      Next
    Next

    Console.WriteLine("******* Using WaveOut ******* ")

    For n = -1 To WaveOut.DeviceCount - 1
      Dim caps = WaveOut.GetCapabilities(n)
      Console.WriteLine($"{n}: {caps.ProductName}")
    Next

    Console.WriteLine("******* Using WaveIn ******* ")

    For n = -1 To WaveIn.DeviceCount - 1
      Dim caps = WaveIn.GetCapabilities(n)
      Console.WriteLine($"{n}: {caps.ProductName}")
    Next

    Console.WriteLine("******* Using DirectSound ******* ")

    For Each dev In DirectSoundOut.Devices
      Console.WriteLine($"{dev.Guid} {dev.ModuleName} {dev.Description}")
    Next

    Console.WriteLine("******* Using WASAPI ******* ")

    Dim enumerator = New MMDeviceEnumerator
    For Each wasapi In enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All)
      Try
        Console.WriteLine($"{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}")
      Catch ex As Runtime.InteropServices.COMException
        Console.WriteLine(ex.Message)
      End Try
    Next

    Console.WriteLine("******* Using ASIO ******* ")

    For Each a In AsioOut.GetDriverNames
      Console.WriteLine($"{a}")
    Next

  End Sub

End Module
