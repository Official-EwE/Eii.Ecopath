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

Option Strict On
#Region " Imports "

Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cFishMIPResultWriterPlugin
    Inherits cEcospaceASCMapResultsWriter
    Implements IUIContextPlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcospaceResultWriterPlugin
    Implements INavigationTreeItemPlugin

    Public Enum eResultTypes As Integer
        tsb
        tcb
        b10cm
        b30cm
        tc
        ' tla
    End Enum

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmUI = Nothing

    Friend Property Configuration As Boolean(,)

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
            Return "Configure FishMIP output writer"
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
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) _
        Implements IEcospaceInitializedPlugin.EcospaceInitialized

        ReDim Me.Configuration(Me.m_uic.Core.nGroups, [Enum].GetValues(GetType(eResultTypes)).Length)
        Me.LoadConfig()

    End Sub

    Public Sub ConfigChanged()
        Me.SaveConfig()
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize

    End Sub

#Region " UI "

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return Not Me.m_ui.IsDisposed()
    End Function

    Public Function GetUI() As frmUI
        If Not HasUI() Then Return New frmUI(Me.m_uic, Me)
        Return m_ui
    End Function

#End Region ' UI

#Region " Config "

    Private Sub LoadConfig()

        Dim groups As New Dictionary(Of String, Integer)
        Dim core As cCore = Me.m_uic.Core

        For i As Integer = 1 To core.nGroups
            groups(Key(core.EcoPathGroupInputs(i).Name)) = i
            For j As Integer = 0 To [Enum].GetValues(GetType(eResultTypes)).Length - 1
                Me.Configuration(i, j) = False
            Next
        Next

        Dim sections As String() = My.Settings.targets.Split("|"c)
        For i As Integer = 0 To Math.Min(sections.Length, [Enum].GetValues(GetType(eResultTypes)).Length) - 1
            If (Not String.IsNullOrWhiteSpace(sections(i))) Then
                For Each strGroup As String In sections(i).Split(";"c)
                    If groups.ContainsKey(Key(strGroup)) Then
                        Configuration(groups(Key(strGroup)), i) = True
                    End If
                Next
            End If
        Next

    End Sub

    Private Sub SaveConfig()

        Dim core As cCore = Me.m_uic.Core
        Dim sb As New StringBuilder()
        Dim vals As Array = [Enum].GetValues(GetType(eResultTypes))

        For i As Integer = 0 To [Enum].GetValues(GetType(eResultTypes)).Length - 1
            If (i > 0) Then sb.Append("|"c)
            Dim b As Boolean = False
            For j As Integer = 1 To core.nGroups
                If (Me.Configuration(j, i)) Then
                    If (b) Then sb.Append(";")
                    sb.Append(Key(core.EcoPathGroupInputs(j).Name))
                    b = True
                End If
            Next
        Next
        My.Settings.targets = sb.ToString()
        My.Settings.Save()

    End Sub

    Private Function Key(strGroup As String) As String
        Return strGroup.ToLower().Replace(";", "").Replace("=", "").Replace("|", "")
    End Function

#End Region ' Config

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
            For Each result As eResultTypes In [Enum].GetValues(GetType(eResultTypes))
                strFile = Path.Combine(Me.OutputDirectory, "FishMip_" & result.ToString & "_" & tsData.iTimeStep & ".asc")

                Dim data(bm.InRow, bm.InCol) As Single
                For iGrp As Integer = 1 To core.nGroups

                    If Me.Configuration(iGrp, result) Then
                        For iRow As Integer = 1 To bm.InRow
                            For icol As Integer = 1 To bm.InCol
                                If (depth.IsWaterCell(iRow, icol)) Then
                                    Dim val As Single = cCore.NULL_VALUE
                                    Select Case result
                                        Case eResultTypes.tc
                                            val = tsData.CatchMap(iRow, icol, iGrp)
                                        Case Else
                                            val = tsData.BiomassMap(iRow, icol, iGrp)
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
            Return "FishMIP output writer"
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "dataFishMP"
        End Get
    End Property

#End Region ' Internals

End Class
