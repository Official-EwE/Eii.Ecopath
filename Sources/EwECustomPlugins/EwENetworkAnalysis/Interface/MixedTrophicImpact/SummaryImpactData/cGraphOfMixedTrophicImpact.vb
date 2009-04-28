'==============================================================================
'
' $Log: cGraphOfMixedTrophicImpact.vb,v $
' Revision 1.10  2009/04/28 19:00:31  jeroens
' Revamped to be able to use styleguide hide groups, rather than an isolated hidegroups interface
'
' Revision 1.9  2009/04/19 13:30:07  jeroens
' Formatted app launch error
'
' Revision 1.8  2009/04/17 03:17:06  jeroens
' Removed global message box
'
' Revision 1.7  2009/04/17 01:07:04  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.6  2009/04/15 23:37:38  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.5  2009/04/15 18:14:53  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.4  2009/04/09 20:04:47  joeh
' Add "Bar graph" button to plot bar graph for MTI
'
' Revision 1.3  2008/12/10 20:56:19  joeh
' Finalize the Suitability Plot
'
' Revision 1.2  2008/12/03 20:49:19  joeh
' Incorportate Functional Response into Network Analysis - Take three
'
' Revision 1.1  2008/09/26 07:30:53  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.IO
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Text
Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

'MTI graph with bars
Public Class cGraphOfMixedTrophicImpact

    Private Shared g_GraphOfMixedTrophicImpactInstance As cGraphOfMixedTrophicImpact
    Private Shared g_Panel As Panel

    Private m_NetworkManager As cNetworkManager
    Private m_core As cCore = Nothing

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cGraphOfMixedTrophicImpact

        cGraphOfMixedTrophicImpact.g_Panel = Panel
        If g_GraphOfMixedTrophicImpactInstance Is Nothing Then
            g_GraphOfMixedTrophicImpactInstance = New cGraphOfMixedTrophicImpact(NetworkManager, Panel)
        End If
        Return g_GraphOfMixedTrophicImpactInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        g_Panel = Panel
        Me.m_core = cCore.GetInstance()
    End Sub

    Public Sub CreatePlot()

        Dim strOutputFileDir As String
        Dim strOutputFileName As String
        Dim FileNumber As Integer
        Dim ZeroString As String
        Dim NoDisplay As Integer
        Dim EnUSLocale As New CultureInfo("en-US")
        Dim sg As StyleGuide = StyleGuide.GetInstance()
        Dim bShowItem As Boolean = True

        'Write data to file
        strOutputFileDir = System.IO.Path.GetTempPath
        strOutputFileName = "MTI.txt"
        If Dir(strOutputFileDir + "\") = "" Then MkDir(strOutputFileDir)
        FileNumber = FreeFile()
        FileOpen(FileNumber, strOutputFileDir & "\" & strOutputFileName, OpenMode.Output)

        NoDisplay = 0
        For i As Integer = 1 To m_NetworkManager.nGroups
            ' JS: group hiding has not yet been enabled
            'bShowItem = sg.GroupVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        For i As Integer = 1 To m_NetworkManager.nFleets
            ' JS: fleet hiding has not yet been enabled
            'bShowItem = sg.FleetVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        PrintLine(FileNumber, Format(NoDisplay, "00"))

        For i As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
            If i <= m_NetworkManager.nGroups Then
                ' JS: group hiding has not yet been enabled
                'bShowItem = sg.GroupVisible(i)
            Else
                ' JS: group hiding has not yet been enabled
                'bShowItem = sg.FleetVisible(i - m_NetworkManager.nGroups)
            End If
            If bShowItem Then
                ZeroString = "                    "
                If i <= m_NetworkManager.nGroups Then
                    Mid$(ZeroString, 1) = m_NetworkManager.GroupName(i)
                Else
                    Mid(ZeroString, 1) = m_NetworkManager.FleetName(i - m_NetworkManager.nGroups)
                End If
                Print(FileNumber, ZeroString)

                For j As Integer = 1 To m_NetworkManager.nGroups + m_NetworkManager.nFleets
                    If i <= m_NetworkManager.nGroups Then
                        ' JS: group hiding has not yet been enabled
                        'bShowItem = sg.GroupVisible(i)
                    Else
                        ' JS: fleet hiding has not yet been enabled
                        'bShowItem = sg.FleetVisible(i - m_NetworkManager.nGroups)
                    End If
                    If bShowItem Then
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
            Dim sb As New StringBuilder
            For Each str As String In EwEUtils.SystemUtilities.ApplicationLaunchLocations
                If sb.Length > 0 Then sb.Append(", ")
                sb.Append(str)
            Next
            Dim msg As New cMessage(String.Format(My.Resources.PROMPT_APPLAUNCH_FAILED, "impacts.exe", sb.ToString), _
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
            Me.m_core.Messages.SendMessage(msg)
        End If
    End Sub

    Public Sub SetUpPanel()
        'RemoveToolStrip()

        SetUpGrid()
    End Sub

    Private Sub SetUpGrid()
        Dim DataGrid As DataGridView = _
            DirectCast(g_Panel.Controls("dgvNetworkAnalysis"), DataGridView)
        Dim GraphPane As ZedGraphControl = _
            DirectCast(g_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As TableLayoutPanel = _
            DirectCast(g_Panel.Controls("tlpNetworkAnalysis"), TableLayoutPanel)

        g_Panel.AutoScroll = False
        LogoPanel.Visible = False
        DataGrid.Visible = False
        GraphPane.Visible = False
        'No need to set MixedTrophicImpactUC.Visible = False
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As ToolStrip = _
            DirectCast(g_Panel.Controls("tsNetworkAnalysis"), ToolStrip)
        Dim DataGrid As DataGridView = _
            DirectCast(g_Panel.Controls("dgvNetworkAnalysis"), DataGridView)

        If Not ToolStrip Is Nothing Then
            g_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = DockStyle.Fill
        End If
    End Sub

End Class
