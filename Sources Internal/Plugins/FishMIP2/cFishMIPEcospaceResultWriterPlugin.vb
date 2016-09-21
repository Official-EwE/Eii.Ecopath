' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cFishMIPEcospaceResultWriterPlugin
    Inherits cEcospaceASCMapResultsWriter
    Implements IUIContextPlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcospaceResultWriterPlugin
    Implements INavigationTreeItemPlugin

#Region " Private variables "

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmConfig = Nothing

#End Region ' Private variables

    Friend Property Configuration As cConfiguration = Nothing

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ecopathinternational@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return Me.DataName
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceTools"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        If (Me.m_uic IsNot Nothing) Then
            Me.m_core = Nothing
        End If

        Me.m_uic = DirectCast(uic, cUIContext)

        If (Me.m_uic IsNot Nothing) Then
            Me.m_core = Me.m_uic.Core
        End If
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) _
        Implements IEcospaceInitializedPlugin.EcospaceInitialized

        Me.Configuration = New cConfiguration(Me.m_core)
        Me.Configuration.Load()

    End Sub

    Public Sub InitEcoOcean()

        Dim core As cCore = Me.m_uic.Core
        Dim smalluns As Integer() = New Integer() {1, 4, 7, 10, 13, 16}
        For Each cat As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))

            For igroup As Integer = 1 To core.nGroups

                Dim bChecked As Boolean = False
                Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(igroup)
                Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(igroup)
                Dim name As String = grp.Name.ToLower()

                Select Case cat
                    Case cConfiguration.eResultTypes.tsb
                        bChecked = grp.IsProducer() Or grp.IsConsumer()
                    Case cConfiguration.eResultTypes.tcb
                        bChecked = grp.IsConsumer() And grpOut.TTLX() > 1
                    Case cConfiguration.eResultTypes.b10cm
                        bChecked = grp.Index <= 24
                    Case cConfiguration.eResultTypes.b30cm
                        bChecked = grp.Index <= 24 And Array.IndexOf(smalluns, grp.Index) = -1
                    Case cConfiguration.eResultTypes.tc
                        bChecked = grp.IsFished()
                    Case cConfiguration.eResultTypes.tcb10cm
                        bChecked = grp.IsFished() And grp.Index <= 24
                    Case cConfiguration.eResultTypes.tcb30cm
                        bChecked = grp.IsFished() And grp.Index <= 24 And Array.IndexOf(smalluns, grp.Index) = -1
                End Select

                Me.Configuration(igroup, cat) = bChecked
            Next
        Next
        Me.Configuration.Save()

    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

#Region " UI "

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return Not Me.m_ui.IsDisposed()
    End Function

    Public Function GetUI() As frmConfig
        If Not HasUI() Then Return New frmConfig(Me.m_uic, Me)
        Return m_ui
    End Function

#End Region ' UI

#Region " Writing "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.WriteResults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        Dim core As cCore = Me.m_uic.Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim depth As cEcospaceLayerDepth = bm.LayerDepth
        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)
        Dim strm As StreamWriter = Nothing
        Dim strFile As String = ""

        If tsData.iTimeStep < Me.FirstOutputTimeStep Then Return

        If (cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then
            For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
                strFile = Path.Combine(Me.OutputDirectory, "FishMip_" & result.ToString & "_" & tsData.iTimeStep & ".asc")

                Dim data(bm.InRow, bm.InCol) As Single
                For iGrp As Integer = 1 To core.nGroups

                    If Me.Configuration(iGrp, result) Then
                        For iRow As Integer = 1 To bm.InRow
                            For icol As Integer = 1 To bm.InCol
                                If (depth.IsWaterCell(iRow, icol)) Then
                                    Dim val As Single = cCore.NULL_VALUE
                                    Select Case result
                                        Case cConfiguration.eResultTypes.tc,
                                             cConfiguration.eResultTypes.tcb,
                                             cConfiguration.eResultTypes.tcb30cm
                                            val = tsData.CatchMap(iRow, icol, iGrp)
                                        Case cConfiguration.eResultTypes.tsb,
                                             cConfiguration.eResultTypes.b10cm,
                                             cConfiguration.eResultTypes.b30cm
                                            val = tsData.BiomassMap(iRow, icol, iGrp)
                                        Case Else
                                            Debug.Assert(False, "Result type not supported")
                                    End Select
                                    If (val <> cCore.NULL_VALUE) Then
                                        data(iRow, icol) += val
                                    End If
                                End If
                            Next
                        Next
                    End If

                    Try
                        strm = New StreamWriter(strFile, False)
                        If (strm IsNot Nothing) Then
                            Me.SaveASCFile(strm, data)
                            strm.Flush()
                            strm.Close()
                            strm = Nothing
                        End If
                    Catch ex As IOException
                        cLog.Write(ex)
                    End Try
                Next
            Next
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.FileExtension"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function FileExtension() As String
        Return ".asc"
    End Function

#End Region ' Base writer overrides

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write the run information file to accompany the run results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub WriteRunInfoFile()

        Try
            Dim strFN As String = Path.Combine(Me.OutputDirectory, "Ecospace RunInfo.txt")
            Dim strm As New StreamWriter(strFN, False)

            strm.WriteLine("EcoSpace .asc map output")
            Me.WriteRunInfo(strm)

            strm.Flush()
            strm.Close()
            strm = Nothing

        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write an entire ASCII file for a group, time step and variable.
    ''' </summary>
    ''' <param name="strm"></param>
    ''' -----------------------------------------------------------------------
    Protected Overloads Sub SaveASCFile(ByVal strm As StreamWriter, data As Single(,))
        Try
            Me.WriteASCIIHeader(strm)
            Me.WriteASCIIBody(strm, data)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write ESRI ASCII body block.
    ''' </summary>
    ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
    ''' -----------------------------------------------------------------------
    Protected Overloads Sub WriteASCIIBody(ByVal writer As StreamWriter, ByVal data(,) As Single)

        Dim value As Double = 0
        Dim strValue As String = ""

        Debug.Assert(data IsNot Nothing)

        For ir As Integer = 1 To Me.EcospaceData.InRow
            For ic As Integer = 1 To Me.EcospaceData.InCol
                If ic > 1 Then writer.Write(" ")
                If Me.EcospaceData.Depth(ir, ic) > 0 Then
                    value = data(ir, ic)
                Else
                    value = cCore.NULL_VALUE
                End If

                strValue = cStringUtils.FormatNumber(value)
                If (ir = 1 And ic = 1) Then
                    If (strValue.IndexOf("."c) = -1) Then
                        strValue = strValue + ".0"
                    End If
                End If

                writer.Write(strValue)
            Next
            writer.WriteLine("")
        Next

    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "dataFishMP"
        End Get
    End Property

#End Region ' Internals

End Class
