'==============================================================================
'
' $Log: SaveModelAs.vb,v $
' Revision 1.1  2008/09/26 07:31:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/07/21 18:26:29  jeroens
' Structured Win32 info
'
' Revision 1.5  2007/10/22 01:31:09  jeroens
' * Fixed failing Windows calls
'
' Revision 1.4  2007/09/18 12:22:00  jeroens
' * Warping - now only get WM_GETTEXT to work...
'
' Revision 1.3  2007/09/17 02:46:00  jeroens
' * It invokes! ..but is not correct yet...
'
' Revision 1.2  2007/09/17 01:11:47  jeroens
' * Getting ready for the real thing
'
' Revision 1.1  2007/09/16 23:04:27  jeroens
' Initial version, scammed from C#, translated and fitted into EwE framework
'
'==============================================================================

Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.Runtime.InteropServices

Imports EwEUtils.Win32Api.Win32
Imports EwEUtils.Win32Api.ComDlg32
Imports EwEUtils.Win32Api.User32

Public Class dlgSaveModelAs

    Private m_hLabel As IntPtr = Nothing
    Private m_hEditBox As IntPtr = Nothing

    Private m_strFilter As String = ""
    Private m_strDefaultExt As String = ""

    Private m_strFileName As String = ""
    Private m_strModelName As String = ""

    Private m_ActiveScreen As Screen

    Private Function HookProc(ByVal hdlg As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer

        Select Case msg

            Case WM.WM_INITDIALOG

                'we need to centre the dialog
                Dim sr As Rectangle = m_ActiveScreen.Bounds
                Dim cr As New RECT()
                Dim parent As IntPtr = GetParent(hdlg)
                GetWindowRect(parent, cr)

                Dim x As Integer = CInt((sr.Right + sr.Left - (cr.Right - cr.Left)) / 2)
                Dim y As Integer = CInt((sr.Bottom + sr.Top - (cr.Bottom - cr.Top)) / 2)

                SetWindowPos(parent, 0, x, y, cr.Right - cr.Left, cr.Bottom - cr.Top + 32, SWP_NOZORDER)

                ' Find the label to position our new label under
                Dim iFileTypeDlgItem As IntPtr = GetDlgItem(parent, 1089)
                Dim iComboDlgItem As IntPtr = GetDlgItem(parent, 1136)
                Dim rcAboveFileTypeDlgItem As New RECT()
                Dim rcAboveComboDlgItem As New RECT()
                Dim hFont As Integer = SendMessage(iFileTypeDlgItem, WM.WM_GETFONT, 0, 0)
                Dim ptLabel As New EwEUtils.Win32Api.Win32.POINT()
                Dim ptEdit As New EwEUtils.Win32Api.Win32.POINT()
                Dim ptRight As New EwEUtils.Win32Api.Win32.POINT()

                GetWindowRect(iFileTypeDlgItem, rcAboveFileTypeDlgItem)
                GetWindowRect(iComboDlgItem, rcAboveComboDlgItem)

                ' Convert the label's screen co-ordinates to client co-ordinates
                ptLabel.X = rcAboveFileTypeDlgItem.Left
                ptLabel.Y = rcAboveFileTypeDlgItem.Bottom
                ScreenToClient(parent, ptLabel)

                ' Convert the combo's screen co-ordinates to client co-ordinates
                ptEdit.X = rcAboveComboDlgItem.Left
                ptEdit.Y = rcAboveComboDlgItem.Bottom
                ScreenToClient(parent, ptEdit)

                ptRight.X = rcAboveComboDlgItem.Right
                ptRight.Y = rcAboveComboDlgItem.Top
                ScreenToClient(parent, ptRight)

                'Create the label
                m_hLabel = CreateWindowEx(0, "STATIC", "lblModelName", CUInt(WS_VISIBLE Or WS_CHILD Or WS_TABSTOP), ptLabel.X, ptLabel.Y + 15, ptEdit.X - ptLabel.X - 1, 22, parent, Nothing, Nothing, 0)
                SetWindowText(m_hLabel, "&Model name:")
                SendMessage(m_hLabel, WM.WM_SETFONT, hFont, 0)

                ' Create model name textbox
                m_hEditBox = CreateWindowEx(WS_EX_CLIENTEDGE, "EDIT", "tbModelName", CUInt(WS_VISIBLE Or WS_CHILD Or WS_TABSTOP Or WS_BORDER), ptEdit.X, ptEdit.Y + 8, ptRight.X - ptEdit.X, 22, parent, Nothing, Nothing, 0)
                SendMessage(m_hEditBox, WM.WM_SETFONT, hFont, 0)

                ' Set current model name
                SendMessage(m_hEditBox, EM.EM_LIMITTEXT, 255, 0)
                SendMessage(m_hEditBox, WM_SETTEXT, 0, Me.m_strModelName)

                Exit Select

            Case WM_DESTROY
                ' Cleanup
                If m_hEditBox <> Nothing Then DestroyWindow(m_hEditBox) : m_hEditBox = Nothing
                If m_hLabel <> Nothing Then DestroyWindow(m_hLabel) : m_hLabel = Nothing
                Exit Select

            Case WM_NOTIFY
                Dim nmhdr As NMHDR = DirectCast(Marshal.PtrToStructure(New IntPtr(lParam), GetType(NMHDR)), NMHDR)

                If nmhdr.code = CDN_FILEOK Then
                    Dim iLen As Integer = SendMessage(Me.m_hEditBox, WM_GETTEXTLENGTH, 0, 0)
                    Dim strNameBuff As New String(" "c, iLen)
                    Dim ipBuff As IntPtr = Marshal.StringToHGlobalAnsi(strNameBuff)
                    ' A file has been selected, get the selected model name
                    SendMessage(Me.m_hEditBox, WM_GETTEXT, iLen, ipBuff.ToInt32)
                    Me.m_strModelName = Marshal.PtrToStringAnsi(ipBuff, iLen)
                    Marshal.FreeHGlobal(ipBuff)
                End If
                Exit Select

        End Select
        Return 0
    End Function

    Public Property DefaultExt() As String
        Get
            Return m_strDefaultExt
        End Get
        Set(ByVal value As String)
            m_strDefaultExt = Value
        End Set
    End Property

    Public Property Filter() As String
        Get
            Return m_strFilter
        End Get
        Set(ByVal value As String)
            m_strFilter = Value
        End Set
    End Property

    Public Property FileName() As String
        Get
            Return m_strFileName
        End Get
        Set(ByVal value As String)
            m_strFileName = Value
        End Set
    End Property

    Public Property ModelName() As String
        Get
            Return Me.m_strModelName
        End Get
        Set(ByVal value As String)
            Me.m_strModelName = value
        End Set
    End Property

    Public Function ShowDialog(ByVal form As Form) As DialogResult

        'set up the struct and populate it

        Dim ofn As New OPENFILENAME()
        Dim strFileName As String = (m_strFileName + New String(" "c, 255))

        ofn.lStructSize = Marshal.SizeOf(ofn)
        ofn.lpstrFilter = Marshal.StringToHGlobalAnsi(m_strFilter.Replace("|"c, Chr(0)) + Chr(0))

        ofn.lpstrFile = Marshal.StringToHGlobalAnsi(strFileName)
        ofn.nMaxFile = strFileName.Length + 1
        ofn.lpstrFileTitle = Marshal.StringToHGlobalAnsi(System.IO.Path.GetFileName(m_strFileName))
        ofn.nMaxFileTitle = System.IO.Path.GetFileName(m_strFileName).Length
        ofn.lpstrTitle = Marshal.StringToHGlobalAnsi("Save Ecopath Model As")
        ofn.lpstrDefExt = Marshal.StringToHGlobalAnsi(m_strDefaultExt)

        ' Position the dialog above the active window
        ofn.hwndOwner = form.Handle

        ' Find the active screen so the dialog box is centred on the correct display
        m_ActiveScreen = Screen.FromControl(form)

        ' Set up some sensible flags
        ofn.Flags = OFN_EXPLORER Or OFN_PATHMUSTEXIST Or OFN_NOTESTFILECREATE Or OFN_ENABLEHOOK Or OFN_HIDEREADONLY Or OFN_OVERWRITEPROMPT

        ' Set the hook. Note use of a C delegate instead of a C function pointer
        ofn.lpfnHook = New OFNHookProcDelegate(AddressOf HookProc)

        ' The struct is smaller when running on Windows 98/ME 
        If System.Environment.OSVersion.Platform <> PlatformID.Win32NT Then
            ofn.lStructSize -= 12
        End If

        'show the dialog

        If Not GetSaveFileName(ofn) Then
            Dim ret As Integer = CommDlgExtendedError()

            If ret <> 0 Then
                Throw New ApplicationException("Couldn't show file open dialog - " + ret.ToString())
            End If

            Return DialogResult.Cancel
        End If

        ' Grab file name
        Me.m_strFileName = Marshal.PtrToStringAnsi(ofn.lpstrFile, ofn.nMaxFile).Trim()
        ' Release mem buffer
        Marshal.FreeHGlobal(ofn.lpstrFile)

        Return DialogResult.OK
    End Function

End Class