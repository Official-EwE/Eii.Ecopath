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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports System.Text
Imports EwECore
Imports EwECore.Auxiliary
Imports EwECore.Common
Imports EwEUtils.Utilities



Public Class cData

    Private ReadOnly Property Plugin As cEwEBiomassEmitterPlugin = Nothing
    Private m_strTrendFileName As String = ""
    'Private m_metadata As New List(Of cEmpericalRuleData)
    Private m_rulemaxeffects As New Dictionary(Of eProtectionType, Single)

    Public Sub New(plugin As cEwEBiomassEmitterPlugin, core As cCore)
        Me.Plugin = plugin
        Me.Core = core
    End Sub

    Public ReadOnly Property Core As cCore = Nothing

    Public ReadOnly Property EcopathDS As cEcopathDataStructures
        Get
            Return Me.Plugin.EcopathDS
        End Get
    End Property

    Public ReadOnly Property EcospaceDS As cEcospaceDataStructures
        Get
            Return Me.Plugin.EcospaceDS
        End Get
    End Property

    Public Property TargetType As eTargetType = eTargetType.MPA

    Public Property ApplicationType As eApplicationType = eApplicationType.Relative

    Public Property Enabled As Boolean = True

    Public ReadOnly Property TimeSeries As New List(Of cEmissionTimeSeries)
    Public ReadOnly Property EmissionRules As New List(Of cEmissionRule)

    Public ReadOnly Property TrendFileName As String
        Get
            Return Me.m_strTrendFileName
        End Get
    End Property

    'Public ReadOnly Property RuleTrendData As cEmpericalRuleData()
    '    Get
    '        Return Me.m_metadata.ToArray()
    '    End Get
    'End Property

    'Public ReadOnly Property Metadata(i As Integer) As cEmpericalRuleData
    '    Get
    '        Return Me.m_metadata(i - 1)
    '    End Get
    'End Property

    Public ReadOnly Property CanRun As Boolean
        Get
            Dim bValid As Boolean = True
            For Each t As cEmission In Me.TimeSeries
                bValid = bValid And t.Enable
            Next
            For Each t As cEmission In Me.EmissionRules
                bValid = bValid And t.Enable
            Next
            Return bValid
        End Get
    End Property

    Public Property RuleMaxEffect(prot As eProtectionType) As Single
        Get
            If Me.m_rulemaxeffects.ContainsKey(prot) Then Return Me.m_rulemaxeffects(prot)
            Return 0
        End Get
        Set(value As Single)
            Me.m_rulemaxeffects(prot) = value
        End Set
    End Property

#Region " Loading "

    Public Sub Clear()
        Me.TimeSeries.Clear()
        Me.EmissionRules.Clear()
        Me.m_strTrendFileName = ""
        Me.TargetType = eTargetType.Region
    End Sub

    Friend Sub LoadTimeSeries(files() As String)

        Me.TimeSeries.Clear()
        Me.m_strTrendFileName = ""

        Dim IO As New cEmissionTimeSeriesReader()
        If IO.Load(Me.Core, files, Me) Then
            If (files.Count = 1) Then
                Me.m_strTrendFileName = files(0)
            Else
                Me.m_strTrendFileName = "(multiple files)"
            End If
            'Me.Validate()
        Else
            Me.TimeSeries.Clear()
            Me.m_strTrendFileName = ""
        End If

    End Sub

    Private Const SETTING_YEAR As String = "year"
    Private Const SETTING_SIZE As String = "size"
    Private Const SETTING_PROT As String = "protection"
    Private Const SETTING_USE As String = "in_use"

    Private Const KEY_RULES As String = "rule_config"
    Private Const SETTING_MAXEFFECTS As String = "max_effects"

    Friend Sub LoadEcospaceScenario()

        Dim ad As cAuxiliaryData = Me.Core.AuxillaryData(Me.DataName())

        Me.EmissionRules.Clear()
        For i As Integer = 1 To Me.Core.nMPAs
            Dim rule As New cEmissionRule(Me, Me.Core.EcospaceMPAs(i))
            Dim key As String = Me.SectionName(rule.MPA)

            'meta.YearEstablished = ad.Settings.ReadSetting(key, SETTING_YEAR, 2000)
            rule.Protection = ad.Settings.ReadSetting(key, SETTING_PROT, eProtectionType.Moderate)
            rule.Enable = ad.Settings.ReadSetting(key, SETTING_USE, False)
            Me.EmissionRules.Add(rule)
        Next

        ' Defaults
        Me.m_rulemaxeffects.Clear()
        Me.m_rulemaxeffects(eProtectionType.Full) = 0.05!
        Me.m_rulemaxeffects(eProtectionType.High) = 0.03!
        Me.m_rulemaxeffects(eProtectionType.Moderate) = 0.01!

        ' Load
        Dim str As String = ad.Settings.ReadSetting(KEY_RULES, SETTING_MAXEFFECTS, "")
        If (Not String.IsNullOrWhiteSpace(str)) Then
            Dim prots() As String = str.Split(" "c)
            For i As Integer = 0 To prots.Length - 1
                Dim s As Single
                If Single.TryParse(prots(i), s) Then
                    Me.m_rulemaxeffects(CType(i, eProtectionType)) = s
                End If
            Next
        End If

    End Sub

#End Region ' Loading

#Region " Saving "

    Friend Sub SaveModelChanges()

        Dim ad As cAuxiliaryData = Me.Core.AuxillaryData(Me.DataName())

        For Each rule As cEmissionRule In Me.EmissionRules
            Dim key As String = Me.SectionName(rule.MPA)
            'ad.Settings.WriteSetting(key, SETTING_YEAR, meta.YearEstablished)
            ad.Settings.WriteSetting(key, SETTING_PROT, rule.Protection)
            ad.Settings.WriteSetting(key, SETTING_USE, rule.Enable)
        Next

        Dim sb As New StringBuilder()
        Dim prots As eProtectionType() = CType([Enum].GetValues(GetType(eProtectionType)), eProtectionType())
        For i As Integer = 0 To prots.Length - 1
            If sb.Length > 0 Then sb.Append(" ")
            If Me.m_rulemaxeffects.ContainsKey(prots(i)) Then
                sb.Append(cStringUtils.FormatSingle(Me.m_rulemaxeffects(prots(i))))
            End If
        Next
        ad.Settings.WriteSetting(KEY_RULES, SETTING_MAXEFFECTS, sb.ToString())

        ad.Settings.Flush()
        ad.Update()

    End Sub

#End Region ' Saving

#Region " Helpers "

    Private Function DataName() As String
        Dim sc As cEcospaceScenario = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)
        Return String.Format("BiomassEmitter_{0}", sc.DBID)
    End Function

    Private Function SectionName(source As cEcospaceMPA) As String
        Return String.Format("mpa_{0}", source.DBID)
    End Function

#End Region ' Helpers

End Class
