#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Text
Imports System.IO
Imports System.Windows.Forms
Imports System.Globalization
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ZedGraph

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cCatchPyramid
    Inherits cContentManager

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean
        Return MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
    End Function

    Public Overrides Sub DisplayData()

        Dim model As cEwEModel = Me.NetworkManager.Core.EwEModel
        Dim sw As StreamWriter = Nothing
        Dim strOutputFile As String = ""
        Dim iMaxTL As Integer
        Dim iFlag As Integer
        Dim core As cCore = Me.NetworkManager.Core
        Dim ciEnUSLocale As New CultureInfo("en-US")
        Dim bSucces As Boolean = True

        ' Prepare directories
        strOutputFile = modUtility.PyramidTempFile(model.Name, ePyramidTypes.Catch, ".txt")
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

            sw.WriteLine(NetworkManager.TotalCatch.ToString("00000000.000", ciEnUSLocale))

            For i As Integer = 1 To iMaxTL
                Dim sngTemp As Single
                sngTemp = NetworkManager.CatchByTrophicLevel(i)
                If Math.Abs(sngTemp) > 0.001 Then
                    sw.Write(sngTemp.ToString("00000000.000", ciEnUSLocale))
                Else
                    sw.Write(0.ToString("00000000.000", ciEnUSLocale))
                End If

                If i < iMaxTL And sngTemp > 0 Then
                    sngTemp = NetworkManager.CatchByTrophicLevel(i) / sngTemp
                Else
                    sngTemp = 0
                End If
                sw.WriteLine(sngTemp.ToString("00000000.000", ciEnUSLocale))
            Next i

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
            bSucces = SystemUtilities.AppExec("pyramid.exe", """" & strOutputFile & """", "", "EwENetworkAnalysis")
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

        'File.Delete(strOutputFile)

    End Sub

End Class
