Imports EwEUtils.SystemUtilities
Imports System.Runtime.InteropServices

Namespace Controls

    Public Class cThemedTreeView
        Inherits TreeView

        ''' <summary>
        ''' And here we thought to be rid of P/invoke!
        ''' </summary>
        ''' <param name="hWnd"></param>
        ''' <param name="pszSubAppName"></param>
        ''' <param name="pszSubIdList"></param>
        ''' <returns></returns>
        Public Declare Unicode Function SetWindowTheme Lib "uxtheme.dll" (ByVal hWnd As IntPtr, ByVal pszSubAppName As String, ByVal pszSubIdList As String) As Integer

        Protected Overrides Sub CreateHandle()
            MyBase.CreateHandle()
            If cSystemUtils.IsWindows And cSystemUtils.IsRunningWin7OrHigher Then
                Try
                    SetWindowTheme(Me.Handle, "explorer", Nothing)
                Catch ex As Exception
                    ' Whoah!
                End Try
            End If
        End Sub

    End Class

End Namespace
