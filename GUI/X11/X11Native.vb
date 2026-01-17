Imports System.Runtime.InteropServices

Module X11Native

  Public Const ExposureMask As Long = &H8000
  Public Const KeyPressMask As Long = &H1
  Public Const StructureNotifyMask As Long = &H20000

  ' <StructLayout(LayoutKind.Explicit, Size:=24)>
  ' Public Structure XEvent
  '   <FieldOffset(0)>
  '   Public type As Integer
  ' End Structure

  ' <StructLayout(LayoutKind.Explicit, Size:=192)> ' size large enough for all XEvent types
  ' Public Structure XEvent
  '     <FieldOffset(0)>
  '     Public type As Integer
  ' End Structure

<StructLayout(LayoutKind.Explicit, Size:=192)>
Public Structure XEvent
    <FieldOffset(0)>
    Public type As Integer
    <FieldOffset(8)> ' offset to keycode field in XKeyEvent union
    Public keycode As Byte
End Structure

<StructLayout(LayoutKind.Sequential)>
Public Structure XKeyEvent
    Public type As Integer
    Public serial As ULong
    Public send_event As Boolean
    Public display As IntPtr
    Public window As IntPtr
    Public root As IntPtr
    Public subwindow As IntPtr
    Public time As ULong
    Public x As Integer
    Public y As Integer
    Public x_root As Integer
    Public y_root As Integer
    Public state As UInteger
    Public keycode As UInteger
    Public same_screen As Boolean
End Structure

<StructLayout(LayoutKind.Sequential)>
Public Structure XConfigureEvent
    Public type As Integer
    Public serial As ULong
    Public send_event As Boolean
    Public display As IntPtr
    Public event_ As IntPtr
    Public window As IntPtr
    Public x As Integer
    Public y As Integer
    Public width As Integer
    Public height As Integer
    Public border_width As Integer
    Public above As IntPtr
    Public override_redirect As Boolean
End Structure

  <DllImport("libX11.so.6")>
  Public Function XOpenDisplay(name As IntPtr) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Function XDefaultScreen(display As IntPtr) As Integer
  End Function

  <DllImport("libX11.so.6")>
  Public Function XRootWindow(display As IntPtr, screen As Integer) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Function XCreateSimpleWindow(
    display As IntPtr,
    parent As IntPtr,
    x As Integer,
    y As Integer,
    width As Integer,
    height As Integer,
    border_width As Integer,
    border As Long,
    background As Long) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Sub XMapWindow(display As IntPtr, window As IntPtr)
  End Sub

  <DllImport("libX11.so.6")>
  Public Sub XSelectInput(display As IntPtr, window As IntPtr, mask As Long)
  End Sub

  <DllImport("libX11.so.6")>
  Public Function XPending(display As IntPtr) As Integer
  End Function

  <DllImport("libX11.so.6")>
  Public Sub XNextEvent(display As IntPtr, ByRef ev As XEvent)
  End Sub

  <DllImport("libX11.so.6")>
  Public Sub XFlush(display As IntPtr)
  End Sub

  <DllImport("libX11.so.6")>
  Public Function XDefaultGC(display As IntPtr, screen As Integer) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Function XDefaultVisual(display As IntPtr, screen As Integer) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Function XDefaultDepth(display As IntPtr, screen As Integer) As Integer
  End Function

  <DllImport("libX11.so.6")>
  Public Function XCreateImage(
    display As IntPtr,
    visual As IntPtr,
    depth As Integer,
    format As Integer,
    offset As Integer,
    data As IntPtr,
    width As Integer,
    height As Integer,
    bitmap_pad As Integer,
    bytes_per_line As Integer) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Function XCreatePixmap(display As IntPtr, drawable As IntPtr, width As Integer, height As Integer, depth As Integer) As IntPtr
  End Function

  <DllImport("libX11.so.6")>
  Public Sub XFreePixmap(display As IntPtr, pixmap As IntPtr)
  End Sub

  <DllImport("libX11.so.6")>
  Public Sub XClearWindow(display As IntPtr, window As IntPtr)
  End Sub

  <DllImport("libX11.so.6")>
  Public Sub XDestroyImage(image As IntPtr)
  End Sub

 <DllImport("libX11.so.6")>
   Public Sub XPutImage(
     display As IntPtr,
     drawable As IntPtr,
     gc As IntPtr,
     image As IntPtr,
     src_x As Integer,
     src_y As Integer,
     dest_x As Integer,
     dest_y As Integer,
     width As Integer,
     height As Integer)
   End Sub

   <DllImport("libX11.so.6")>
   Public Sub XFillRectangle(display As IntPtr, drawable As IntPtr, gc As IntPtr, x As Integer, y As Integer, width As Integer, height As Integer)
   End Sub

   <DllImport("libX11.so.6")>
   Public Sub XSetForeground(display As IntPtr, gc As IntPtr, foreground As ULong)
   End Sub

   <DllImport("libX11.so.6")>
   Public Sub XCopyArea(display As IntPtr, src As IntPtr, dst As IntPtr, gc As IntPtr, src_x As Integer, src_y As Integer, width As Integer, height As Integer, dst_x As Integer, dst_y As Integer)
   End Sub

<DllImport("libX11.so.6")>
Public Function XLookupString(ByRef key_event As XKeyEvent,
                              buffer As IntPtr,
                              bytes_buffer As Integer,
                              keysym As IntPtr,
                              status As IntPtr) As Integer
End Function

' XRender extension functions
<DllImport("libXrender.so.1")>
Public Function XRenderQueryExtension(display As IntPtr, ByRef event_base As Integer, ByRef error_base As Integer) As Boolean
End Function

<DllImport("libXrender.so.1")>
Public Function XRenderFindVisualFormat(display As IntPtr, visual As IntPtr) As IntPtr
End Function

<DllImport("libXrender.so.1")>
Public Function XRenderCreatePicture(display As IntPtr, drawable As IntPtr, format As IntPtr, valuemask As ULong, attributes As IntPtr) As IntPtr
End Function

<DllImport("libXrender.so.1")>
Public Sub XRenderComposite(display As IntPtr, op As Integer, src As IntPtr, mask As IntPtr, dst As IntPtr, src_x As Integer, src_y As Integer, mask_x As Integer, mask_y As Integer, dst_x As Integer, dst_y As Integer, width As Integer, height As Integer)
End Sub

  <DllImport("libXrender.so.1")>
  Public Sub XRenderSetPictureTransform(display As IntPtr, picture As IntPtr, transform As IntPtr)
  End Sub

  <DllImport("libXrender.so.1")>
  Public Function XRenderFindStandardFormat(display As IntPtr, format As Integer) As IntPtr
  End Function

  <DllImport("libXrender.so.1")>
  Public Sub XRenderFreePicture(display As IntPtr, picture As IntPtr)
  End Sub

' XTransform structure for scaling
<StructLayout(LayoutKind.Sequential)>
Public Structure XTransform
    Public m11 As Integer
    Public m12 As Integer
    Public m13 As Integer
    Public m21 As Integer
    Public m22 As Integer
    Public m23 As Integer
    Public m31 As Integer
    Public m32 As Integer
    Public m33 As Integer
End Structure

' Constants
Public Const PictStandardARGB32 As Integer = 0
Public Const PictStandardRGB24 As Integer = 1
Public Const PictOpSrc As Integer = 0
Public Const PictOpOver As Integer = 1

End Module

