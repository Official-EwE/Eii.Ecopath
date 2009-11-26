#Region " Imports "

Option Strict On
Imports System

#End Region ' Imports

Namespace Win32Api

    ''' <summary>
    ''' Helper class providing access to a selection of Win32 GDI+ API calls.
    ''' </summary>
    Public Class GDIplus

        Public Enum TernaryRasterOperations
            ''' <summary>dest = source.</summary>
            SRCCOPY = &HCC0020
            ''' <summary>dest = source OR dest.</summary>
            SRCPAINT = &HEE0086
            ''' <summary>dest = source AND dest</summary>
            SRCAND = &H8800C6
            ''' <summary>dest = source XOR dest</summary>
            SRCINVERT = &H660046
            ''' <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = &H440328
            ''' <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = &H330008
            ''' <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = &H1100A6
            ''' <summary>dest = (source AND pattern)</summary>
            MERGECOPY = &HC000CA
            ''' <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = &HBB0226
            ''' <summary>dest = pattern</summary>
            PATCOPY = &HF00021
            ''' <summary>dest = DPSnoo</summary>
            PATPAINT = &HFB0A09
            ''' <summary>dest = pattern XOR dest</summary>
            PATINVERT = &H5A0049
            ''' <summary>dest = (NOT dest)</summary>
            DSTINVERT = &H550009
            ''' <summary>dest = BLACK</summary>
            BLACKNESS = &H42
            ''' <summary>dest = WHITE</summary>
            WHITENESS = &HFF0062
        End Enum

        Public Enum StretchMode
            STRETCH_ANDSCANS = 1
            STRETCH_ORSCANS = 2
            STRETCH_DELETESCANS = 3
            STRETCH_HALFTONE = 4
        End Enum

        Public Enum eSysColorType As Integer
            COLOR_SCROLLBAR = 0
            COLOR_DESKTOP = 1
            COLOR_ACTIVECAPTION = 2
            COLOR_INACTIVECAPTION = 3
            COLOR_MENU = 4
            COLOR_WINDOW = 5
            COLOR_WINDOWFRAME = 6
            COLOR_MENUTEXT = 7
            COLOR_WINDOWTEXT = 8
            COLOR_CAPTIONTEXT = 9
            COLOR_ACTIVEBORDER = 10
            COLOR_INACTIVEBORDER = 11
            COLOR_APPWORKSPACE = 12
            COLOR_HIGHLIGHT = 13
            COLOR_HIGHLIGHTTEXT = 14
            COLOR_BTNFACE = 15
            COLOR_BTNSHADOW = 16
            COLOR_GRAYTEXT = 17
            COLOR_BTNTEXT = 18
            COLOR_INACTIVECAPTIONTEXT = 19
            COLOR_BTNHIGHLIGHT = 20
            COLOR_GRADIENTACTIVECAPTION = 27
            COLOR_GRADIENTINACTIVECAPTION = 28
        End Enum

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="hdc"></param>
        ''' <param name="nXDest"></param>
        ''' <param name="nYDest"></param>
        ''' <param name="nWidth"></param>
        ''' <param name="nHeight"></param>
        ''' <param name="hdcSrc"></param>
        ''' <param name="nXSrc"></param>
        ''' <param name="nYSrc"></param>
        ''' <param name="dwRop"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <para>An example on using BitBlt and StretchBlt in a .NET environment:</para>
        ''' <code>
        ''' ' Get HDC to bitmap
        ''' Dim hbmp As IntPtr = bmp.GetHbitmap()
        ''' ' Get handle to device context (DC) of target graphics. Handle needs to be released afterward
        ''' Dim hdcTarget As IntPtr = e.Graphics.GetHdc()
        ''' ' Create new DC that is compatible with target DC
        ''' Dim hdcSource As IntPtr = GDIplus.CreateCompatibleDC(hdcTarget)
        ''' ' Select bitmap to render in the new source DC
        ''' Dim hobjOrig As IntPtr = GDIplus.SelectObject(hdcSource, hbmp)
        ''' 
        ''' ' Stretch blit w/o dithering
        ''' GDIplus.StretchBlt(hdcTarget, 0, 0, Me.Width, Me.Height, hdcSource, _
        '''     0, 0, m_bmp.Width, m_bmp.Height, GDIplus.TernaryRasterOperations.SRCCOPY)
        ''' 
        ''' ' Cleanup:
        ''' ' Select original gdi obj back into source DC
        ''' Dim hobjDummy As IntPtr = GDIplus.SelectObject(hdcSource, hobjOrig)
        ''' ' Delete the dummy
        ''' GDIplus.DeleteObject(hobjDummy)
        ''' ' Delete source HDC
        ''' GDIplus.DeleteDC(hdcSource)
        ''' ' Release target HDC
        ''' e.Graphics.ReleaseHdc(hdcTarget)
        ''' </code>
        ''' </remarks>
        Public Declare Function BitBlt Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, _
            ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As TernaryRasterOperations) As Boolean

        Public Declare Function StretchBlt Lib "gdi32.dll" (ByVal hdcDest As IntPtr, ByVal nXOriginDest As Integer, ByVal nYOriginDest As Integer, _
            ByVal nWidthDest As Integer, ByVal nHeightDest As Integer, ByVal hdcSrc As IntPtr, ByVal nXOriginSrc As Integer, _
            ByVal nYOriginSrc As Integer, ByVal nWidthSrc As Integer, ByVal nHeightSrc As Integer, ByVal dwRop As TernaryRasterOperations) As Boolean

        Public Declare Function CreateCompatibleDC Lib "gdi32.dll" (ByVal hdc As IntPtr) As IntPtr

        Public Declare Function DeleteDC Lib "gdi32.dll" (ByVal hdc As IntPtr) As Boolean

        Public Declare Function SelectObject Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal hgdiobj As IntPtr) As IntPtr

        Public Declare Function DeleteObject Lib "gdi32.dll" (ByVal hgdiobj As IntPtr) As Boolean

        Public Declare Function SetStretchBltMode Lib "gdi32.dll" (ByVal hdc As IntPtr, ByVal iStretchMode As StretchMode) As Boolean

        Public Declare Function GetSysColor Lib "user32.dll" (ByVal sysColor As eSysColorType) As Integer

    End Class

End Namespace
