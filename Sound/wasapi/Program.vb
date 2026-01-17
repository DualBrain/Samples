Imports System.Runtime.InteropServices

Module Program

  <ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
  Interface IMMDeviceEnumerator
    Function EnumAudioEndpoints(dataFlow As Integer, stateMask As Integer, <Out> ByRef devices As Object) As Integer
    Function GetDefaultAudioEndpoint(dataFlow As Integer, role As Integer, <Out> ByRef ppDevice As IMMDevice) As Integer
    Function GetDevice(pwstrId As String, <Out> ByRef device As IMMDevice) As Integer
    Function RegisterEndpointNotificationCallback(client As Object) As Integer
    Function UnregisterEndpointNotificationCallback(client As Object) As Integer
  End Interface

  <ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
  Interface IMMDevice
    Function Activate(<[In]> ByRef iid As Guid, dwClsCtx As UInteger, pActivationParams As IntPtr, <Out> ByRef ppInterface As IntPtr) As Integer
  End Interface

  <ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")>
  Class MMDeviceEnumerator
  End Class

  Sub Main1()
    Console.WriteLine("Initializing COM and getting default audio endpoint...")

    Dim enumerator = CType(New MMDeviceEnumerator(), IMMDeviceEnumerator)
    Dim device As IMMDevice = Nothing
    Dim hr = enumerator.GetDefaultAudioEndpoint(0, 1, device) ' 0 = eRender, 1 = eMultimedia

    If hr <> 0 OrElse device Is Nothing Then
      Console.WriteLine("GetDefaultAudioEndpoint failed: 0x" & hr.ToString("X8"))
      Return
    End If

    Console.WriteLine("IMMDevice obtained successfully!")

    ' Prepare for IMMDevice.Activate
    Dim IID_IAudioClient As Guid = New Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")
    Dim audioClientPtr As IntPtr = IntPtr.Zero

    hr = device.Activate(IID_IAudioClient, &H1, IntPtr.Zero, audioClientPtr) ' CLSCTX_INPROC_SERVER = 0x1

    If hr <> 0 OrElse audioClientPtr = IntPtr.Zero Then
      Console.WriteLine("Activate for IAudioClient failed: 0x" & hr.ToString("X8"))
      Return
    End If

    Console.WriteLine("IAudioClient activated successfully.")

    ' Clean up
    Marshal.Release(audioClientPtr)
    Console.WriteLine("Released IAudioClient. Press any key to exit.")
    Console.ReadKey()
  End Sub

End Module
