'==============================================================================
'
' $Log: cBiomassPyramid.vb,v $
' Revision 1.1  2009/06/15 14:15:27  jeroens
' Flattened directory structure
'
' Revision 1.13  2009/05/30 00:00:48  jeroens
' Toolstrip usage centralized
'
' Revision 1.12  2009/05/21 18:53:33  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.11  2009/05/19 13:41:07  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.10  2009/05/01 17:42:52  jeroens
' Inherited from cContentManager
'
' Revision 1.9  2009/04/19 13:29:17  jeroens
' Formatted app launch error
'
' Revision 1.8  2009/04/17 03:17:05  jeroens
' Removed global message box
'
' Revision 1.7  2009/04/17 01:06:59  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.6  2009/04/15 23:22:26  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.5  2009/04/15 18:14:47  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.4  2008/12/10 20:56:18  joeh
' Finalize the Suitability Plot
'
' Revision 1.3  2008/12/04 01:14:47  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.2  2008/12/03 20:49:18  joeh
' Incorportate Functional Response into Network Analysis - Take three
'
' Revision 1.1  2008/09/26 07:30:55  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports System.IO
Imports ZedGraph
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Text

Public Class cBiomassPyramid
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
        strOutputFile = EwEUtils.SystemUtilities.MakeTempFile("NA-pyramid-biomass.txt")
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

            modUtility.WritePyramidFile(Me.NetworkManager.Core.EwEModel.Name, _
                            ePyramidTypes.Biomass, "t/km²", _
                            iMaxTL, sTotalBiomass, _
                            asB, asBRel)

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

        Try
            'Execute the external application through the general function on EwEUtils
            bSucces = EwEUtils.SystemUtilities.AppExec("pyramid.exe", strOutputFile, "", "EwENetworkAnalysis")
        Catch ex As Exception
            bSucces = False
        End Try

        If Not bSucces Then
            Dim sb As New StringBuilder
            For Each str As String In EwEUtils.SystemUtilities.ApplicationLaunchLocations
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
