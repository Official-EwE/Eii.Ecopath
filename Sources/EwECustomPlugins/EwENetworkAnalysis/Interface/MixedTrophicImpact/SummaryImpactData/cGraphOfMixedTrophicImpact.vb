'==============================================================================
'
' $Log: cGraphOfMixedTrophicImpact.vb,v $
' Revision 1.11  2009/05/01 17:42:58  jeroens
' Inherited from cContentManager
'
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
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                ByVal datagrid As DataGridView, _
                                ByVal graph As ZedGraphControl, _
                                ByVal plot As ucPlot)

        MyBase.Attach(manager, datagrid, graph, plot)

    End Sub

    Public Overrides Sub DisplayData()

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
        For i As Integer = 1 To Me.NetworkManager.nGroups
            ' JS: group hiding has not yet been enabled
            'bShowItem = sg.GroupVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        For i As Integer = 1 To Me.NetworkManager.nFleets
            ' JS: fleet hiding has not yet been enabled
            'bShowItem = sg.FleetVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        PrintLine(FileNumber, Format(NoDisplay, "00"))

        For i As Integer = 1 To Me.NetworkManager.nGroups + Me.NetworkManager.nFleets
            If i <= Me.NetworkManager.nGroups Then
                ' JS: group hiding has not yet been enabled
                'bShowItem = sg.GroupVisible(i)
            Else
                ' JS: group hiding has not yet been enabled
                'bShowItem = sg.FleetVisible(i - Me.NetworkManager.nGroups)
            End If
            If bShowItem Then
                ZeroString = "                    "
                If i <= Me.NetworkManager.nGroups Then
                    Mid$(ZeroString, 1) = Me.NetworkManager.GroupName(i)
                Else
                    Mid(ZeroString, 1) = Me.NetworkManager.FleetName(i - Me.NetworkManager.nGroups)
                End If
                Print(FileNumber, ZeroString)

                For j As Integer = 1 To Me.NetworkManager.nGroups + Me.NetworkManager.nFleets
                    If i <= Me.NetworkManager.nGroups Then
                        ' JS: group hiding has not yet been enabled
                        'bShowItem = sg.GroupVisible(i)
                    Else
                        ' JS: fleet hiding has not yet been enabled
                        'bShowItem = sg.FleetVisible(i - Me.NetworkManager.nGroups)
                    End If
                    If bShowItem Then
                        If Me.NetworkManager.MixedTrophicImpacts(i, j) >= 0.0 Then
                            Print(FileNumber, Me.NetworkManager.MixedTrophicImpacts(i, j).ToString("000.00", EnUSLocale))
                        Else
                            Dim TmpString As String
                            TmpString = Me.NetworkManager.MixedTrophicImpacts(i, j).ToString("00.00", EnUSLocale)
                            If TmpString = "00.00" Then TmpString = "000.00"
                            Print(FileNumber, TmpString)
                        End If
                    End If
                Next j

                PrintLine(FileNumber, "")
            End If
        Next i
        FileClose(FileNumber)

        ''Call impacts.exe using the file written above
        'If IsPlotActive("ECOPATH 3.0 - Impacts") Then
        '    AppActivate("ECOPATH 3.0 - Impacts")
        '    System.Windows.Forms.SendKeys.Send("%{F4}")
        '    'My.Computer.Keyboard.SendKeys("%{F4}", True)
        'End If

        'Execute the external application through the general function on EwEUtils
        If Not EwEUtils.SystemUtilities.AppExec("impacts.exe", Path.Combine(strOutputFileDir, strOutputFileName), "", "EwENetworkAnalysis") Then
            Dim sb As New StringBuilder
            For Each str As String In EwEUtils.SystemUtilities.ApplicationLaunchLocations
                If sb.Length > 0 Then sb.Append(", ")
                sb.Append(str)
            Next
            Dim msg As New cMessage(String.Format(My.Resources.PROMPT_APPLAUNCH_FAILED, "impacts.exe", sb.ToString), _
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
            Me.NetworkManager.Core.Messages.SendMessage(msg)
        End If
    End Sub

End Class
