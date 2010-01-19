#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports ZedGraph
Imports System.Globalization
Imports EwECore
Imports EwEUtils.Core
Imports System.Windows.Forms
Imports System.Text
Imports EwEUtils.SystemUtilities

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cFlowPyramid
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean
        Return MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
    End Function

    Public Overrides Sub DisplayData()

        Dim sw As StreamWriter = Nothing
        Dim strOutputFile As String = ""
        Dim strAnswer As String = ""
        Dim iMaxTL As Integer
        Dim iFlag As Integer
        Dim ciEnUSLocale As New CultureInfo("en-US")
        Dim bShowItem As Boolean = True
        Dim bSucces As Boolean = True

        ' Prepare directories
        strOutputFile = SystemUtilities.MakeTempFile("NA-pyramid-flow.txt")
        sw = New StreamWriter(strOutputFile, False, New System.Text.UTF8Encoding())
        Try

            iFlag = 1
            sw.Write(Format(iFlag, "0"))

            iMaxTL = CInt(IIf(NetworkManager.nTrophicLevels > 9, 9, NetworkManager.nTrophicLevels))
            sw.WriteLine(Format(iMaxTL, "0"))

            'If Not (currUnitIndex = 6 Or currUnitIndex = 9) Then
            'Print #fnum, Trim(currUnitName);
            'Else
            'Print #fnum, Trim(currUnitName);
            'End If
            'Print #fnum, "/";
            'Print #fnum, Trim(TimeUnitName)
            sw.WriteLine("t/km²/year")

            sw.WriteLine(NetworkManager.TotalThroughput.ToString("00000000.000", ciEnUSLocale))

            For i As Integer = 1 To NetworkManager.nGroups
                bShowItem = Me.StyleGuide.GroupVisible(i)
                If (bShowItem = False) Then 'There is at least one hidden
                    strAnswer = CStr(MsgBox(My.Resources.PROMPT_DISPLAY_ALL_HIDDEN_GROUPS, MsgBoxStyle.YesNo, My.Resources.CAPTION))
                    Exit For
                Else
                    strAnswer = CStr(vbYes)
                End If
            Next

            Select Case strAnswer
                Case CStr(vbYes) 'all groups
                    For i As Integer = 1 To iMaxTL
                        Dim sngTemp As Single
                        sngTemp = CSng(IIf(Math.Abs(NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i)) > 0.001, _
                            NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i), 0))
                        sw.Write(sngTemp.ToString("00000000.000", ciEnUSLocale))
                        'the values from transfer eff table
                        Dim Tr1 As Single
                        Tr1 = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i)
                        If Tr1 > 0 Then
                            If NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i) > 0 Then
                                'TrEm1(i) = Tr1 / (m_NetworkManager.PPThroughtput(i) + m_NetworkManager.DetThroughtput(i))
                                NetworkManager.TrEm1(i) = Tr1 / (NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i))
                            End If
                        End If
                        Dim TotTr As Single
                        TotTr = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i) + _
                            NetworkManager.CA(i) + NetworkManager.CatchDetritus(i)

                        If NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i) > 0 Then
                            TotTr = TotTr / (NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i))
                            sngTemp = CSng(IIf(Math.Abs(100 * TotTr) > 0.001, 100 * TotTr, 0))
                        Else
                            'TrEm1(i) = 0
                            NetworkManager.TrEm1(i) = 0
                            sngTemp = 0
                        End If
                        sw.WriteLine(sngTemp.ToString("00000000.000", ciEnUSLocale))
                    Next
                Case CStr(vbNo) 'hidden groups only
                    For i As Integer = 1 To iMaxTL
                        Dim sngTemp As Single
                        sngTemp = CSng(IIf(NetworkManager.ThroughtputShow(i) > 0.001, NetworkManager.ThroughtputShow(i), 0))
                        sw.Write(sngTemp.ToString("00000000.000", ciEnUSLocale))
                        'the values from transfer eff table
                        Dim Tr1 As Single
                        Tr1 = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i)
                        If Tr1 > 0 Then
                            If NetworkManager.ThroughtputShow(i) > 0 Then
                                'TrEm1(i) = Tr1 / (m_NetworkManager.ThroughtputShow(i))
                                NetworkManager.TrEm1(i) = Tr1 / (NetworkManager.ThroughtputShow(i))
                            End If
                        End If
                        Dim TotTr As Single
                        TotTr = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i) + _
                            NetworkManager.CA(i) + NetworkManager.CatchDetritus(i)

                        If NetworkManager.ThroughtputShow(i) > 0 Then
                            TotTr = TotTr / (NetworkManager.ThroughtputShow(i))
                            sngTemp = CSng(IIf(Math.Abs(100 * TotTr) > 0.001, 100 * TotTr, 0))
                        Else
                            'TrEm1(i) = 0
                            NetworkManager.TrEm1(i) = 0
                            sngTemp = 0
                        End If
                        sw.WriteLine(sngTemp.ToString("00000000.000", ciEnUSLocale))
                    Next
            End Select

        Catch ex As Exception
            bSucces = False
        End Try
        sw.Close()

        If Not bSucces Then Return

        ''Call pyramid.exe using the file written above
        'If IsPlotActive("ECOPATH 3.0 - Pyramid") Then
        '    AppActivate("ECOPATH 3.0 - Pyramid")
        '    System.Windows.Forms.SendKeys.Send("%{F4}")
        'End If

        Try
            'Execute the external application through the general function on EwEUtils
            bSucces = SystemUtilities.AppExec("pyramid.exe", strOutputFile, "", "EwENetworkAnalysis")
        Catch ex As Exception
            bSucces = False
        End Try

        If Not bSucces Then
            Dim sb As New StringBuilder
            For Each str As String In SystemUtilities.ApplicationLaunchLocations
                If sb.Length > 0 Then sb.Append(", ")
                sb.Append(str)
            Next
            Dim msg As New cMessage(String.Format(My.Resources.PROMPT_APPLAUNCH_FAILED, "pyramid.exe", sb.ToString), _
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
            Me.NetworkManager.Core.Messages.SendMessage(msg)
        End If

        File.Delete(strOutputFile)

    End Sub

End Class
