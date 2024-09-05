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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict Off
Imports EwECore
Imports ScientificInterfaceShared.Controls


Public Class cResultsHolder

    Implements EwEPlugin.IMenuItemPlugin

    Implements EwEPlugin.IEcosimModifyTimeseriesPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.ICorePlugin

    Implements EwEPlugin.IUIContextPlugin

    Implements EwEPlugin.IHelpPlugin

    Private ResultsForm As frmResults = Nothing
    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private mTimeSeries As cTimeSeriesDataStructures = Nothing
    Private mDataStructure As cEcosimDatastructures = Nothing
    Private ZStat(,) As Single
    Private DatSumZ() As Single
    Private DatNobs() As Single
    Private DataQ() As Single
    Private logdiff(,) As Single
    Private sumSS() As Single
    Private mEcosimModel As Ecosim.cEcosimModel = Nothing

    Public ReadOnly Property ControlImage() As Object Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property DisplayName() As String Implements EwEPlugin.IPlugin.DisplayName
        Get
            Return My.Resources.PLUGIN_NAME
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Dim bHasForm As Boolean = False

        If Me.ResultsForm IsNot Nothing Then
            bHasForm = Not Me.ResultsForm.IsDisposed
        End If

        If Not bHasForm Then
            Me.ResultsForm = New frmResults
            Me.ResultsForm.Initialize(Me.m_uic)
            Me.ResultsForm.StartForm(sender, e, frmPlugin, Me.logdiff, Me.mTimeSeries, Me.mEcosimModel)
        End If

        Me.ResultsForm.DataStructure = Me.mDataStructure

        ' JS 04 Mar 2011: Let EwE framework deal with this
        'ResultsForm.Show()

    End Sub

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevlowestoft@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return My.Resources.PLUGIN_DESCRIPTION
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = core
    End Sub

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwEResultExtractorResultHolderPlugin"
        End Get
    End Property

    Public Sub EcosimModifyTimeseries(TimeSeriesDataStructures As Object) Implements EwEPlugin.IEcosimModifyTimeseriesPlugin.EcosimModifyTimeseries
        Me.mTimeSeries = TimeSeriesDataStructures
    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Dim iDyear As Integer
        Dim DataStructure As cEcosimDatastructures = EcosimDatastructures
        Dim zest As Single
        Dim iYear As Integer

        'Only runs for the 5th month of every year
        If Not ((iTime + 7) Mod 12 = 0) Then Exit Sub
        iYear = Int((iTime + 7) / 12) - 1

        'Check whether data exists for this year
        For i = 1 To Me.mTimeSeries.AppliedDatPoints
            If Me.mTimeSeries.AppliedDatYear(i) - Me.mTimeSeries.AppliedDatYear(1) = iYear Then iDyear = i : Exit For
        Next

        If iDyear <> 0 Then

            For j = 1 To Me.mTimeSeries.AppliedNdatType

                If Me.mTimeSeries.AppliedDatVal(iDyear, j) > 0 And _
                                (Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.BiomassRel Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.BiomassAbs Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.TotalMortality Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.AverageWeight Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.Catches Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.CatchesRel Or _
                                 Me.mTimeSeries.AppliedDatType(j) = eTimeSeriesType.CatchesForcing) Then

                    Select Case Me.mTimeSeries.AppliedDatType(j)

                        '0,1    
                        Case eTimeSeriesType.BiomassAbs, eTimeSeriesType.BiomassRel
                            Me.ZStat(j, iDyear) = CSng(Math.Log(Me.mTimeSeries.AppliedDatVal(iDyear, j) / BiomassAtTimestep(Me.mTimeSeries.AppliedDatPool(j))))

                        Case eTimeSeriesType.TotalMortality
                            zest = DataStructure.loss(Me.mTimeSeries.AppliedDatPool(j)) / BiomassAtTimestep(Me.mTimeSeries.AppliedDatPool(j))
                            Me.ZStat(j, iDyear) = CSng(Math.Log(Me.mTimeSeries.AppliedDatVal(iDyear, j) / zest))

                        Case eTimeSeriesType.Catches, eTimeSeriesType.CatchesForcing, eTimeSeriesType.CatchesRel
                            If DataStructure.FishTime(Me.mTimeSeries.AppliedDatPool(j)) > 0 Then
                                Me.ZStat(j, iDyear) = CSng(Math.Log(Me.mTimeSeries.AppliedDatVal(iDyear, j) / (BiomassAtTimestep(Me.mTimeSeries.AppliedDatPool(j)) * DataStructure.FishTime(Me.mTimeSeries.AppliedDatPool(j)))))
                            End If

                        Case eTimeSeriesType.AverageWeight
                            '7 Mean body weith data Martell, Jan 02
                            'Assuming user knows this data type is for split pools only.
                            'and is treated as a relative index
                            If DataStructure.ResultsOverTime IsNot Nothing Then
                                Dim iti As Integer = iDyear * 12 - 7
                                Dim iGroup As Integer = Me.mTimeSeries.AppliedDatPool(j)
                                zest = DataStructure.ResultsOverTime(cEcosimDatastructures.eEcosimResults.AvgWeight, Me.mTimeSeries.AppliedDatPool(j), iti)
                                If zest > 0 Then
                                    Me.ZStat(j, iDyear) = CSng(Math.Log(Me.mTimeSeries.AppliedDatVal(iDyear, j) / zest))
                                End If
                            End If

                    End Select

                End If

            Next

        End If

    End Sub

    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunCompletedPlugin.EcosimRunCompleted
        Dim iYear As Integer

        Me.mDataStructure = EcosimDatastructures

        For i = 1 To Me.mTimeSeries.AppliedDatPoints
            iYear = Me.mTimeSeries.AppliedDatYear(i) - Me.mTimeSeries.AppliedDatYear(1)
            For j = 1 To Me.mTimeSeries.AppliedNdatType
                If Me.mTimeSeries.AppliedDatVal(i, j) = 0 Then
                    Me.logdiff(j, i) = 0
                Else
                    Me.logdiff(j, i) = Me.ZStat(j, i) - Me.mTimeSeries.AppliedDatQ(j)
                End If
            Next
        Next

    End Sub



    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        ReDim Me.ZStat(Me.mTimeSeries.AppliedNdatType, Me.mTimeSeries.AppliedDatPoints)
        ReDim Me.logdiff(Me.mTimeSeries.AppliedNdatType, Me.mTimeSeries.AppliedDatPoints)
        ReDim Me.sumSS(Me.mTimeSeries.AppliedNdatType)
    End Sub

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Me.mEcosimModel = objEcoSim
    End Sub

    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public ReadOnly Property HelpTopic As String Implements EwEPlugin.IHelpPlugin.HelpTopic
        Get
            Return ".\UserGuide\ResultsExtractorPlug.pdf"
        End Get
    End Property

    Public ReadOnly Property HelpURL As String Implements EwEPlugin.IHelpPlugin.HelpURL
        Get
            Return Me.HelpTopic
        End Get
    End Property

End Class
