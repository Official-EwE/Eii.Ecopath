#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports System.IO
Imports ZedGraph
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Text
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports EwEUtils.Win32Api
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cBiomassPyramid
    Inherits cContentManager

    Public Sub New()
        '
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

        Dim model As cEwEModel = Me.NetworkManager.Core.EwEModel
        Dim sw As StreamWriter = Nothing
        Dim strOutputFile As String = ""
        Dim strOutputFile83 As String = Space(255)
        Dim iMaxTL As Integer
        Dim sTotalBiomass As Single
        Dim iFlag As Integer
        Dim core As cCore = Me.NetworkManager.Core
        Dim ciEnUSLocale As New CultureInfo("en-US")
        Dim bSucces As Boolean = True
        Dim intTemp As Integer

        Dim asB() As Single
        Dim asBRel() As Single

        ' Prepare directories
        strOutputFile = modUtility.PyramidTempFile(model.Name, ePyramidTypes.Biomass, ".txt")
        sw = New StreamWriter(strOutputFile, False, New System.Text.UTF8Encoding())
        Try
            iFlag = 2
            sw.Write(Format(iFlag, "0"))

            iMaxTL = CInt(IIf(NetworkManager.nTrophicLevels > 9, 9, NetworkManager.nTrophicLevels))
            ReDim asB(iMaxTL)
            ReDim asBRel(iMaxTL)

            sw.WriteLine(Format(iMaxTL, "0"))
            sw.WriteLine("t/km²")

            sTotalBiomass = 0
            If NetworkManager.nTrophicLevels < core.nLivingGroups Then
                intTemp = NetworkManager.nTrophicLevels
            Else
                intTemp = core.nLivingGroups
            End If
            For i As Integer = 1 To intTemp
                sTotalBiomass = sTotalBiomass + CSng(IIf(NetworkManager.BiomassByTrophicLevel(i) > 0.001, _
                    NetworkManager.BiomassByTrophicLevel(i), 0))
            Next
            sw.WriteLine(sTotalBiomass.ToString("00000000.000", ciEnUSLocale))

            For i As Integer = 1 To iMaxTL
                Dim sngTemp As Single
                'row = i '(MaxTL - i) + 1
                sngTemp = NetworkManager.BiomassByTrophicLevel(i)
                If Math.Abs(sngTemp) > 0.001 Then
                    sw.Write(sngTemp.ToString("00000000.000", ciEnUSLocale))
                Else
                    sw.Write(0.ToString("00000000.000", ciEnUSLocale))
                End If
                asB(i) = sngTemp

                If i < iMaxTL And sngTemp > 0 Then
                    sngTemp = NetworkManager.BiomassByTrophicLevel(i) / sngTemp
                Else
                    sngTemp = 0
                End If
                sw.WriteLine(sngTemp.ToString("00000000.000", ciEnUSLocale))
                asBRel(i) = sngTemp

            Next i

            'modUtility.WritePyramidFile(Me.NetworkManager.Core.EwEModel.Name, _
            '                ePyramidTypes.Biomass, "t/km²", _
            '                iMaxTL, sTotalBiomass, _
            '                asB, asBRel)

            bSucces = True
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

        'Execute the external application through the general function on EwEUtils
        Kernel32.GetShortPathName(strOutputFile, strOutputFile83, 255)
        If Not SystemUtilities.AppExec("pyramid.exe", strOutputFile, "", "EwENetworkAnalysis") Then
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
