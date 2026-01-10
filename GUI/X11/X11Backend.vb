Imports System.Runtime.InteropServices

Module X11Backend

  Public Display As IntPtr
  Public Window As IntPtr
  Public GC As IntPtr
  Private XImage As IntPtr
  Private PixelHandle As GCHandle
  Private ScreenNum As Integer
  Private DisplayWidth As Integer
  Private DisplayHeight As Integer
  Private DisplayPixels() As Integer
  Private DisplayHandle As GCHandle
  Private DisplayXImage As IntPtr
  Private SrcPixmap As IntPtr
  Private SrcPicture As IntPtr
  Private DestPicture As IntPtr

  ' Keyboard state tracking
  Private KeyStates(255) As Boolean
  Private LastKey As String = ""

  Public Sub Init(width As Integer, height As Integer)

    Display = XOpenDisplay(IntPtr.Zero)
    If Display = IntPtr.Zero Then
      Throw New Exception("Cannot open X11 display")
    End If

    ScreenNum = XDefaultScreen(Display)
    DisplayWidth = width
    DisplayHeight = height
    Dim root = XRootWindow(Display, ScreenNum)

    Window = XCreateSimpleWindow(
      Display, root,
      0, 0,
      width, height,
      0, 0, 0)

    XSelectInput(Display, Window,
      ExposureMask Or
      KeyPressMask Or
      StructureNotifyMask)

    XMapWindow(Display, Window)

    GC = XDefaultGC(Display, ScreenNum)

    ' ---- Create XImage bound to framebuffer ----
    PixelHandle = GCHandle.Alloc(FrameBuffer.Pixels, GCHandleType.Pinned)

    XImage = XCreateImage(
      Display,
      XDefaultVisual(Display, ScreenNum),
      XDefaultDepth(Display, ScreenNum),
      2, ' ZPixmap
      0,
      PixelHandle.AddrOfPinnedObject(),
      FrameBuffer.Width,
      FrameBuffer.Height,
      32,
      FrameBuffer.Width * 4)

    ' Check XRender extension
    Dim event_base As Integer, error_base As Integer
    If Not XRenderQueryExtension(Display, event_base, error_base) Then
      Throw New Exception("XRender extension not available")
    End If

    ' Create source pixmap and pictures
    DisplayWidth = width
    DisplayHeight = height
    SrcPixmap = XCreatePixmap(Display, Window, FrameBuffer.Width, FrameBuffer.Height, XDefaultDepth(Display, ScreenNum))
    Dim format = XRenderFindVisualFormat(Display, XDefaultVisual(Display, ScreenNum))
    SrcPicture = XRenderCreatePicture(Display, SrcPixmap, format, 0, IntPtr.Zero)
    DestPicture = XRenderCreatePicture(Display, Window, format, 0, IntPtr.Zero)

    ' Setup display buffer
    ReDim DisplayPixels(DisplayWidth * DisplayHeight - 1)
    DisplayHandle = GCHandle.Alloc(DisplayPixels, GCHandleType.Pinned)
    DisplayXImage = XCreateImage(
      Display,
      XDefaultVisual(Display, ScreenNum),
      XDefaultDepth(Display, ScreenNum),
      2, ' ZPixmap
      0,
      DisplayHandle.AddrOfPinnedObject(),
      DisplayWidth,
      DisplayHeight,
      32,
      DisplayWidth * 4)

    XFlush(Display)
  End Sub

'   Public Sub PollEvents(ByRef running As Boolean)
'     While XPending(Display) > 0
'       Dim ev As New XEvent
'       XNextEvent(Display, ev)

'       ' TODO: handle close, keys, resize
'     End While
'   End Sub

  ' Map X11 keycodes to ASCII (simplified for letters/numbers/ESC)
  Private Function KeycodeToAscii(keycode As Integer) As Char
    Select Case keycode
      Case 9 : Return Chr(27)      ' ESC
      Case 10 To 35
        Return Chr(keycode + 0)   ' placeholder for simplicity
      Case Else
        Return Chr(0)
    End Select
  End Function

  Public Function GetKey(key As Integer) As Boolean
    Return KeyStates(key)
  End Function

'   Public Function INKEY() As String
'     For i = 0 To 255
'       If KeyStates(i) Then Return Chr(i).ToString()
'     Next
'     Return ""
'   End Function

'   Public Sub PollEvents(ByRef running As Boolean)
'     While XPending(Display) > 0
'       Dim ev As New XEvent
'       XNextEvent(Display, ev)

'       Select Case ev.type
'         Case 2 ' KeyPress
'           ' Simple mapping: just read first byte (ASCII)
'           Dim kc = Marshal.ReadByte(IntPtr.Zero) ' TODO: proper keycode handling
'           If kc = 27 Then running = False ' ESC
'           If kc >= 0 AndAlso kc < 256 Then KeyStates(kc) = True

'         Case 3 ' KeyRelease
'           Dim kc = Marshal.ReadByte(IntPtr.Zero) ' TODO: proper keycode
'           If kc >= 0 AndAlso kc < 256 Then KeyStates(kc) = False

'         Case 17 ' DestroyNotify
'           running = False
'       End Select
'     End While
'   End Sub

    ' Public Sub PollEvents(ByRef running As Boolean)
    '     While XPending(Display) > 0
    '         Dim ev As New XEvent
    '         XNextEvent(Display, ev)

    '         Select Case ev.type
    '             Case 2 ' KeyPress
    '                 Dim keyEvent As XKeyEvent = Marshal.PtrToStructure(Of XKeyEvent)(Marshal.AllocHGlobal(Marshal.SizeOf(GetType(XKeyEvent))))
    '                 ' Copy memory from ev into keyEvent
    '                 Marshal.StructureToPtr(ev, Marshal.AllocHGlobal(Marshal.SizeOf(GetType(XEvent))), True)
    '                 ' Actually, safer: use fixed size union
    '                 ' For simplicity, we can read the keycode using a GCHandle

    '             Case 3 ' KeyRelease
    '                 ' Similar
    '             Case 17 ' DestroyNotify
    '                 running = False
    '         End Select
    '     End While
    ' End Sub

  Public Function INKEY() As String
    Dim key = LastKey
    LastKey = ""
    Return key
  End Function

'   Public Sub PollEvents(ByRef running As Boolean)
'     While XPending(Display) > 0
'       Dim ev As New XEvent
'       XNextEvent(Display, ev)

'       Select Case ev.type
'         Case 2 ' KeyPress
'           Dim keyEvent As XKeyEvent = Marshal.PtrToStructure(Of XKeyEvent)(Marshal.AllocHGlobal(Marshal.SizeOf(GetType(XKeyEvent))))
'           Marshal.StructureToPtr(ev, Marshal.AllocHGlobal(Marshal.SizeOf(GetType(XEvent))), True)
'           ' Actually copy the bytes from ev to keyEvent
'           ' Simpler: use unsafe cast with GCHandle
'           ' For simplicity here, we'll re-interpret memory directly:

'           ' Allocate buffer for XLookupString
'           Dim buffer As IntPtr = Marshal.AllocHGlobal(10)
'           Dim count = XLookupString(keyEvent, buffer, 10, IntPtr.Zero, IntPtr.Zero)
'           If count > 0 Then
'               Dim chars = Marshal.PtrToStringAnsi(buffer, count)
'               If Not String.IsNullOrEmpty(chars) Then
'                   LastKey = chars.Substring(0, 1)
'                   Dim c = Asc(LastKey(0))
'                   If c >= 0 AndAlso c < 256 Then KeyStates(c) = True
'                   If c = 27 Then running = False ' ESC
'               End If
'           End If
'           Marshal.FreeHGlobal(buffer)

'         Case 3 ' KeyRelease
'           ' Could implement release tracking if needed

'         Case 17 ' DestroyNotify
'           running = False
'       End Select
'     End While
'   End Sub

Public Sub PollEvents(ByRef running As Boolean)
    While XPending(Display) > 0
        Dim ev As New XEvent
        XNextEvent(Display, ev)

        ' Pin the ev struct in memory
        Dim handle As GCHandle = GCHandle.Alloc(ev, GCHandleType.Pinned)
        Dim ptr As IntPtr = handle.AddrOfPinnedObject()

        Select Case ev.type            
            Case 2 ' KeyPress
                ' Read XKeyEvent from ev memory
                Dim keyEvent As XKeyEvent = Marshal.PtrToStructure(Of XKeyEvent)(ptr)

                ' Use XLookupString to convert to ASCII
                Dim buffer As IntPtr = Marshal.AllocHGlobal(10)
                Dim count = XLookupString(keyEvent, buffer, 10, IntPtr.Zero, IntPtr.Zero)
                If count > 0 Then
                    Dim chars = Marshal.PtrToStringAnsi(buffer, count)
                    If Not String.IsNullOrEmpty(chars) Then
                        LastKey = chars.Substring(0, 1)
                        Dim c = Asc(LastKey(0))
                        If c = 27 Then running = False ' ESC
                    End If
                End If

                Marshal.FreeHGlobal(buffer)

            Case 22 ' ConfigureNotify
                Dim configureEvent = Marshal.PtrToStructure(Of XConfigureEvent)(ptr)
                Dim oldWidth = DisplayWidth
                Dim oldHeight = DisplayHeight
                DisplayWidth = configureEvent.width
                DisplayHeight = configureEvent.height
                ' Resize display buffer if necessary
                Dim newSize = DisplayWidth * DisplayHeight
                If newSize <> DisplayPixels.Length Then
                  If DisplayHandle.IsAllocated Then DisplayHandle.Free()
                  ReDim DisplayPixels(newSize - 1)
                  DisplayHandle = GCHandle.Alloc(DisplayPixels, GCHandleType.Pinned)
                  DisplayXImage = XCreateImage(
                    Display,
                    XDefaultVisual(Display, ScreenNum),
                    XDefaultDepth(Display, ScreenNum),
                    2, ' ZPixmap
                    0,
                    DisplayHandle.AddrOfPinnedObject(),
                    DisplayWidth,
                    DisplayHeight,
                    32,
                    DisplayWidth * 4)
                End If

            Case 17 ' DestroyNotify
                running = False
        End Select

        handle.Free()
    End While
End Sub


  Public Sub Present()
    ' Put framebuffer image to source pixmap
    XPutImage(
      Display,
      SrcPixmap,
      GC,
      XImage,
      0, 0,
      0, 0,
      FrameBuffer.Width,
      FrameBuffer.Height)

    ' Clear display pixels to black
    For i = 0 To DisplayPixels.Length - 1
      DisplayPixels(i) = &HFF000000
    Next

    ' Fit the window while maintaining aspect ratio (add black bars)
    Dim srcW = FrameBuffer.Width
    Dim srcH = FrameBuffer.Height
    Dim scaleX = DisplayWidth / srcW
    Dim scaleY = DisplayHeight / srcH
    Dim scale = Math.Min(scaleX, scaleY)
    Dim scaledW = CInt(srcW * scale)
    Dim scaledH = CInt(srcH * scale)
    Dim offsetX = CInt((DisplayWidth - scaledW) / 2)
    Dim offsetY = CInt((DisplayHeight - scaledH) / 2)

    ' Manually scale the source to fit the display
    For destY = 0 To scaledH - 1
      Dim srcY = destY / scale
      For destX = 0 To scaledW - 1
        Dim srcX = destX / scale
        Dim pixel = FrameBuffer.GetPixel(Math.Round(srcX), Math.Round(srcY))
        DisplayPixels((offsetY + destY) * DisplayWidth + (offsetX + destX)) = pixel
      Next
    Next

    ' Put the fitted display image
    XPutImage(
      Display,
      Window,
      GC,
      DisplayXImage,
      0, 0,
      0, 0,
      DisplayWidth,
      DisplayHeight)

    XFlush(Display)

    XFlush(Display)
  End Sub

  Public Sub Shutdown()
    If PixelHandle.IsAllocated Then PixelHandle.Free()
    If DisplayHandle.IsAllocated Then DisplayHandle.Free()
    If SrcPicture <> IntPtr.Zero Then XRenderFreePicture(Display, SrcPicture)
    If DestPicture <> IntPtr.Zero Then XRenderFreePicture(Display, DestPicture)
    If SrcPixmap <> IntPtr.Zero Then XFreePixmap(Display, SrcPixmap)
  End Sub

End Module