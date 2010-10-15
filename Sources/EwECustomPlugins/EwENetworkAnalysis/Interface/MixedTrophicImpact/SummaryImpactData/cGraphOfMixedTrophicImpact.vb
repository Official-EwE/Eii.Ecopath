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
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Win32Api

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' MTI graph with bars.
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class cGraphOfMixedTrophicImpact
    Inherits cContentManager

    Public Sub New()
        ' NOP
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip, _
                                     ByVal uic As cUIContext) As Boolean
        Return MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)
    End Function

    Public Overrides Sub DisplayData()

        Dim strOutputFileDir As String
        Dim strOutputFileName As String
        Dim FileNumber As Integer
        Dim ZeroString As String
        Dim NoDisplay As Integer
        Dim EnUSLocale As New CultureInfo("en-US")
        Dim sg As cStyleGuide = Me.StyleGuide
        Dim bShowItem As Boolean = True

        'Write data to file
        strOutputFileDir = System.IO.Path.GetTempPath
        strOutputFileName = "MTI.txt"
        If Dir(strOutputFileDir + "\") = "" Then MkDir(strOutputFileDir)
        FileNumber = FreeFile()
        FileOpen(FileNumber, strOutputFileDir & "\" & strOutputFileName, OpenMode.Output)

        NoDisplay = 0
        For i As Integer = 1 To Me.NetworkManager.nGroups
            ' Only show visible groups
            bShowItem = sg.GroupVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        For i As Integer = 1 To Me.NetworkManager.nFleets
            ' Only show visible fleets
            bShowItem = sg.FleetVisible(i)
            If bShowItem Then NoDisplay += 1
        Next
        PrintLine(FileNumber, Format(NoDisplay, "00"))

        For i As Integer = 1 To Me.NetworkManager.nGroups + Me.NetworkManager.nFleets
            If i <= Me.NetworkManager.nGroups Then
                ' Only show visible groups
                bShowItem = sg.GroupVisible(i)
            Else
                ' Only show visible fleets
                bShowItem = sg.FleetVisible(i - Me.NetworkManager.nGroups)
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
                        ' Only show visible groups
                        bShowItem = sg.GroupVisible(i)
                    Else
                        ' Only show visible fleets
                        bShowItem = sg.FleetVisible(i - Me.NetworkManager.nGroups)
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

        ' Convert text file location to a 8.3 file
        Dim strPath As String = Path.Combine(strOutputFileDir, strOutputFileName)
        Dim strPath83 As String = Space(255)
        Dim strError As String = ""
        Kernel32.GetShortPathName(strPath, strPath83, 255)

        'Execute the external application through the general function on EwEUtils
        If Not cSystemUtils.AppExec("impacts.exe", strPath83, strError) Then
            Dim msg As New cMessage(String.Format(My.Resources.PROMPT_APPLAUNCH_FAILED, "impacts.exe", strError), _
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
            Me.NetworkManager.Core.Messages.SendMessage(msg)
        End If
    End Sub

End Class
