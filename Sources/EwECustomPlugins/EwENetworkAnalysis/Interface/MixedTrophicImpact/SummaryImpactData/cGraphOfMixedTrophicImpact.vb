'==============================================================================
'
' $Log: cGraphOfMixedTrophicImpact.vb,v $
' Revision 1.1  2008/09/26 07:30:53  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.20  2008/09/09 14:44:48  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.19  2008/08/20 23:30:29  sherman
' Used Updated System.Utilitis to launch 16bit applications
'
' Revision 1.18  2008/07/17 23:52:39  joeh
' Add comments
'
' Revision 1.17  2008/07/15 21:26:17  joeh
' Fix bug 452 - use en-US locale when writing numerical data to mti.txt, flow.txt, biomass.txt and catch.txt.
'
' Revision 1.16  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.15  2008/06/24 00:52:27  joeh
' Ecosim NA indice plots are no longer displayed in a pop up form, rather they are displayed in the same form where  we have the NA tree view
'
' Revision 1.14  2008/06/05 19:56:10  sherman
' Moved Plugins to \Plugins\EwENetworkAnalysis\
'
' Revision 1.13  2007/09/25 19:01:33  joeb
' Fixed bug that cause IsGroupShown(strKey) to explode.
'
' Revision 1.12  2007/06/23 00:05:21  joeh
' Change the OutputFileDir to System.IO.Path.GetTempPath
'
' Revision 1.11  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.9  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.IO
Imports System.Globalization

Public Class cGraphOfMixedTrophicImpact
    Private Shared m_GraphOfMixedTrophicImpactInstance As cGraphOfMixedTrophicImpact

    Private m_NetworkManager As cNetworkManager
    Private m_HideGroups As frmHideGroups
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal HideGroups As frmHideGroups, ByVal Panel As Windows.Forms.Panel) As cGraphOfMixedTrophicImpact
        m_Panel = Panel

        If m_GraphOfMixedTrophicImpactInstance Is Nothing Then m_GraphOfMixedTrophicImpactInstance = New cGraphOfMixedTrophicImpact(NetworkManager, HideGroups, Panel)
        Return m_GraphOfMixedTrophicImpactInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal HideGroups As frmHideGroups, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_HideGroups = HideGroups
        m_Panel = Panel
    End Sub

    Public Sub CreatePlot()

        Dim strOutputFileDir As String
        Dim strOutputFileName As String
        Dim FileNumber As Integer
        Dim ZeroString As String
        Dim NoDisplay As Integer
        Dim EnUSLocale As New CultureInfo("en-US")

        'Write data to file
        strOutputFileDir = System.IO.Path.GetTempPath
        strOutputFileName = "MTI.txt"
        If Dir(strOutputFileDir + "\") = "" Then MkDir(strOutputFileDir)
        FileNumber = FreeFile()
        FileOpen(FileNumber, strOutputFileDir & "\" & strOutputFileName, OpenMode.Output)

        NoDisplay = 0
        For i As Integer = 1 To m_NetworkManager.nGroups
            If m_HideGroups.IsGroupShown(m_NetworkManager.GroupName(i)) Then NoDisplay = NoDisplay + 1
        Next
        For i As Integer = 1 To m_NetworkManager.nFleets
            'add the fleet prefix the the fleet name so that the correct fleet will be found in the dictionary
            If m_HideGroups.IsGroupShown(frmHideGroups.FLEET_PREFIX & m_NetworkManager.FleetName(i)) Then NoDisplay = NoDisplay + 1
        Next
        PrintLine(FileNumber, Format(NoDisplay, "00"))

        For i As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
            Dim strKey As String
            Dim intKey As Integer
            If i <= m_NetworkManager.nGroups Then
                strKey = m_NetworkManager.GroupName(i)
            Else
                intKey = i - m_NetworkManager.nGroups
                strKey = frmHideGroups.FLEET_PREFIX & m_NetworkManager.FleetName(intKey)
            End If
            If m_HideGroups.IsGroupShown(strKey) Then
                ZeroString = "                    "
                If i <= m_NetworkManager.nGroups Then
                    Mid$(ZeroString, 1) = m_NetworkManager.GroupName(i)
                Else
                    Mid(ZeroString, 1) = m_NetworkManager.FleetName(i - m_NetworkManager.nGroups)
                End If
                Print(FileNumber, ZeroString)

                For j As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
                    If j <= m_NetworkManager.nGroups Then
                        strKey = m_NetworkManager.GroupName(j)
                    Else
                        intKey = j - m_NetworkManager.nGroups
                        strKey = frmHideGroups.FLEET_PREFIX & m_NetworkManager.FleetName(intKey)
                    End If
                    If m_HideGroups.IsGroupShown(strKey) Then
                        If m_NetworkManager.MixedTrophicImpacts(i, j) >= 0.0 Then
                            Print(FileNumber, m_NetworkManager.MixedTrophicImpacts(i, j).ToString("000.00", EnUSLocale))
                        Else
                            Dim TmpString As String
                            TmpString = m_NetworkManager.MixedTrophicImpacts(i, j).ToString("00.00", EnUSLocale)
                            If TmpString = "00.00" Then TmpString = "000.00"
                            Print(FileNumber, TmpString)
                        End If
                    End If
                Next j

                PrintLine(FileNumber, "")
            End If
        Next i
        FileClose(FileNumber)

        'Call impacts.exe using the file written above
        If IsPlotActive("ECOPATH 3.0 - Impacts") Then
            AppActivate("ECOPATH 3.0 - Impacts")
            System.Windows.Forms.SendKeys.Send("%{F4}")
            'My.Computer.Keyboard.SendKeys("%{F4}", True)
        End If

        'Execute the external application through the general function on EwEUtils
        If Not EwEUtils.SystemUtilities.AppExec("impacts.exe", Path.Combine(strOutputFileDir, strOutputFileName), "", "EwENetworkAnalysis") Then
            EwEUtils.SystemUtilities.PrintFileNotFoundError()
        End If
    End Sub

    Public Sub SetUpPanel()
        RemoveToolStrip()

        SetUpGrid()
    End Sub

    Private Sub SetUpGrid()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        LogoPanel.Visible = False
        DataGrid.Visible = False
        GraphPane.Visible = False
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
        End If
    End Sub

End Class
