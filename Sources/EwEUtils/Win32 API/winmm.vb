#Region " Imports "

Option Strict On
Imports System
Imports EwEUtils.Win32Api.Win32
Imports System.Runtime.InteropServices

#End Region ' Imports

Namespace Win32Api

    <CLSCompliant(False)> _
    Public Class Winmm

        Public Enum PlaySoundFlags As Integer
            ''' <summary>Loop the sound until next sndPlaySound.</summary>
            SND_LOOP = &H8
            SND_FILENAME = &H20000
            SND_NODEFAULT = &H2
        End Enum

        Public Declare Auto Function PlaySound Lib "winmm.dll" (ByVal lpszSoundName As String, _
                                                                ByVal hModule As Integer, _
                                                                ByVal dwFlags As Integer) As Integer

    End Class

End Namespace
