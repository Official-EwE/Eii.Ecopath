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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Commands
Imports ZedGraph

#End Region ' Imports

Namespace Ecopath

    Public Class EditMultiStanza

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_fpK As cEwEFormatProvider = Nothing
        Private m_fpRecPwr As cEwEFormatProvider = Nothing
        Private m_fpBab As cEwEFormatProvider = Nothing
        Private m_fpWmatWinf As cEwEFormatProvider = Nothing
        Private m_fpFFHatchStocking As cEwEFormatProvider = Nothing
        Private m_fpStanza As cEwEFormatProvider = Nothing
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_groupInitial As cEcoPathGroupInput = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext, _
                       Optional ByVal group As cEcoPathGroupInput = Nothing)

            Me.InitializeComponent()

            Me.m_uic = uic
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.m_uic, Me.m_zgc)
            Me.m_groupInitial = group

        End Sub

#End Region

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            ' Need to center myself, no?
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim bEcosimLoaded As Boolean = (Me.m_uic.Core.ActiveEcosimScenarioIndex > -1)
            Dim bEcospaceLoaded As Boolean = (Me.m_uic.Core.ActiveEcospaceScenarioIndex > -1)
            Dim mgr As cForcingFunctionShapeManager = Me.m_uic.Core.ForcingShapeManager
            Dim lItems As New List(Of Object)

            ' Gather stanza names
            lItems.Clear()
            For iIndex As Integer = 0 To Me.m_uic.Core.nStanzas - 1
                lItems.Add(Me.m_uic.Core.StanzaGroups(iIndex))
            Next
            Me.m_fpStanza = New cEwEFormatProvider(Me.m_uic, Me.m_cmbStanzaGroups, GetType(Integer), lItems.ToArray())

            ' Find stanza for initial group
            If Me.m_groupInitial IsNot Nothing Then
                Me.m_fpStanza.Value = Me.m_groupInitial.iStanza
            Else
                Me.m_fpStanza.Value = 0
            End If
            Me.m_grid.StanzaGroup = DirectCast(Me.m_fpStanza.Items(CInt(Me.m_fpStanza.Value)), cStanzaGroup)
            Me.m_grid.UIContext = Me.m_uic

            Me.m_fpK = New cEwEFormatProvider(Me.m_uic, Me.m_txtK, GetType(Single))
            Me.m_fpRecPwr = New cEwEFormatProvider(Me.m_uic, Me.m_txtRecPwr, GetType(Single))
            Me.m_fpBab = New cEwEFormatProvider(Me.m_uic, Me.m_txtBAB, GetType(Single))
            Me.m_fpWmatWinf = New cEwEFormatProvider(Me.m_uic, Me.m_txtWmatWinf, GetType(Single))

            ' Gather forcing functions
            lItems.Clear()
            If bEcosimLoaded Then
                lItems.Add(SharedResources.GENERIC_VALUE_NONE)
                For iIndex As Integer = 0 To mgr.Count - 1
                    lItems.Add(mgr(iIndex))
                Next
            Else
                lItems.Add(My.Resources.PROMPT_ECOSIM_REQUIRED)
            End If
            Me.m_fpFFHatchStocking = New cEwEFormatProvider(Me.m_uic, Me.m_cmbFF, GetType(Integer), lItems.ToArray)
            If bEcosimLoaded Then
                Me.m_fpFFHatchStocking.Style = cStyleGuide.eStyleFlags.OK
            Else
                Me.m_fpFFHatchStocking.Style = cStyleGuide.eStyleFlags.NotEditable
                Me.m_fpFFHatchStocking.Value = 0
            End If

            Me.m_cbEggAtSpawn.Enabled = bEcospaceLoaded

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            ' Clean up
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing

            Me.m_fpK.Release()
            Me.m_fpRecPwr.Release()
            Me.m_fpBab.Release()
            Me.m_fpWmatWinf.Release()
            Me.m_fpFFHatchStocking.Release()
            Me.m_fpStanza.Release()

            MyBase.OnFormClosed(e)

        End Sub

#Region "Event handlers "

        Private Sub OnCalculate(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCalculate.Click

            Me.SaveChanges(False)
            Me.m_grid.CalculateStanzaParameters()
            Me.m_grid.RefreshContent()
            Me.UpdateGraph(m_zgc)

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click

            Me.SaveChanges(True)
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click

            Me.m_grid.ResetStanzaGroupValues()
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnSelectStanza(ByVal sender As Object, ByVal e As System.EventArgs) _
             Handles m_cmbStanzaGroups.SelectionChangeCommitted

            Me.SaveChanges(False)
            Me.m_grid.StanzaGroup = DirectCast(Me.m_fpStanza.Items(Me.m_cmbStanzaGroups.SelectedIndex), cStanzaGroup)
            Me.UpdateControls()

        End Sub

#End Region ' Event handlers

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the multi-stanza graph by plotting number, weight and biomass
        ''' for all life stages in a stanza configuration.
        ''' </summary>
        ''' <param name="zgc"></param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateGraph(ByVal zgc As ZedGraphControl)

            Dim sg As cStanzaGroup = Me.m_grid.StanzaGroup

            ' Sanity check
            If (sg Is Nothing) Then Return

            Dim pane As GraphPane = Me.m_zgh.ConfigurePane("", _
                SharedResources.HEADER_AGE, 0, sg.MaxAge - 1, _
                SharedResources.HEADER_NORMALIZED_VALUE, 0, 1, True)

            Dim pplNumber As New PointPairList()
            Dim pplWeight As New PointPairList()
            Dim pplB As New PointPairList()
            Dim pplSep As PointPairList = Nothing
            Dim strLabel As String = ""

            Dim sMaxNumber As Single = 0.0
            Dim sMaxWeight As Single = 0.0
            Dim sMaxBiomass As Single = 0.0

            'don't show the last value
            'For i As Integer = 1 To sg.MaxAge - 1
            For i As Integer = 0 To sg.MaxAge - 1
                sMaxNumber = Math.Max(sMaxNumber, sg.NumberAtAge(i))
                sMaxWeight = Math.Max(sMaxWeight, sg.WeightAtAge(i))
                sMaxBiomass = Math.Max(sMaxBiomass, sg.BiomassAtAge(i))
            Next

            If sMaxNumber = 0 Then sMaxNumber = 1
            If sMaxWeight = 0 Then sMaxWeight = 1
            If sMaxBiomass = 0 Then sMaxBiomass = 1

            ' NB: All curves are scaled to 1
            zgc.GraphPane.CurveList.Clear()
            ' For i As Integer = 1 To sg.MaxAge - 1
            For i As Integer = 0 To sg.MaxAge - 1
                pplNumber.Add(i - 1, sg.NumberAtAge(i) / sMaxNumber)
                pplWeight.Add(i - 1, sg.WeightAtAge(i) / sMaxWeight)
                pplB.Add(i - 1, sg.BiomassAtAge(i) / sMaxBiomass)
            Next i

            ' Generate curves
            pane.AddCurve(SharedResources.HEADER_NUMBER, pplNumber, Color.Red, SymbolType.None)
            pane.AddCurve(SharedResources.HEADER_INDIVIDUAL_WEIGHT, pplWeight, Color.Blue, SymbolType.None)
            pane.AddCurve(SharedResources.HEADER_POPULATIONBIOMASS, pplB, Color.Black, SymbolType.None)

            ' Generate vertical separator curves
            For i As Integer = 2 To sg.nLifeStages

                ' First vertical separator?
                If (i = 2) Then
                    ' #Yes: name this curve
                    strLabel = My.Resources.ECOPATH_GRAPH_LEGEND_STANZA_SEP
                Else
                    ' #No: do not not name this curve (we do not want to flood the legend)
                    strLabel = ""
                End If

                pplSep = New PointPairList
                pplSep.Add(sg.StartAge(i) - 1, 0)
                pplSep.Add(sg.StartAge(i) - 1, 1)
                pane.AddCurve(strLabel, pplSep, Color.Green, SymbolType.None)
            Next

            ' Calculate the Axis Scale Ranges
            zgc.AxisChange()
            zgc.Refresh()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update format providers to match the current stanza group selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            Dim bEcosimLoaded As Boolean = Me.m_uic.Core.StateMonitor.HasEcosimLoaded()
            Dim stanza As cStanzaGroup = Me.m_grid.StanzaGroup
            Dim source As cEcoPathGroupInput = Nothing

            If (stanza.LeadingB > 0) Then
                source = Me.m_uic.Core.EcoPathGroupInputs(stanza.iGroups(stanza.LeadingB))
            End If

            Me.m_fpStanza.Value = stanza.Index
            If (source IsNot Nothing) Then
                Me.m_fpK.Value = source.VBK
                Me.m_fpK.Enabled = True
            Else
                Me.m_fpK.Value = cCore.NULL_VALUE
                Me.m_fpK.Enabled = False
            End If

            Me.m_fpRecPwr.Value = stanza.RecruitmentPower
            Me.m_fpBab.Value = stanza.BiomassAccumulationRate
            Me.m_fpWmatWinf.Value = stanza.WmatWinf

            If bEcosimLoaded Then
                ' Only sync FF when sim scenario is loaded
                Me.m_fpFFHatchStocking.Value = stanza.HatchCode
            End If

            Me.m_cbFFecun.Checked = stanza.FixedFecundity
            Me.m_cbEggAtSpawn.Checked = stanza.EggAtSpawn

            Me.m_grid.CalculateStanzaParameters()
            Me.m_grid.RefreshContent()

            Me.UpdateGraph(m_zgc)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply format provider values to the selected stanza group.
        ''' </summary>
        ''' <param name="bApplyToCore"></param>
        ''' -------------------------------------------------------------------
        Private Sub SaveChanges(ByVal bApplyToCore As Boolean)

            Dim bEcosimLoaded As Boolean = Me.m_uic.Core.StateMonitor.HasEcosimLoaded()
            Dim stanza As cStanzaGroup = Me.m_grid.StanzaGroup
            Dim groupLeading As cEcoPathGroupInput = Me.m_uic.Core.EcoPathGroupInputs(stanza.iGroups(stanza.LeadingB))

            ' vbK obtained from leading group in stanza config
            groupLeading.VBK = CSng(Me.m_fpK.Value)
            stanza.RecruitmentPower = CSng(Me.m_fpRecPwr.Value)
            stanza.BiomassAccumulationRate = CSng(Me.m_fpBab.Value)
            stanza.WmatWinf = CSng(Me.m_fpWmatWinf.Value)

            If bEcosimLoaded Then
                ' Only update FF when scenario is loaded
                Me.m_grid.StanzaGroup.HatchCode = CInt(Me.m_fpFFHatchStocking.Value)
            End If

            stanza.FixedFecundity = Me.m_cbFFecun.Checked
            stanza.EggAtSpawn = Me.m_cbEggAtSpawn.Checked

            ' Make the grid apply its values
            Me.m_grid.SetStanzaGroupValues(bApplyToCore)

        End Sub

#End Region ' Internals

    End Class

End Namespace
