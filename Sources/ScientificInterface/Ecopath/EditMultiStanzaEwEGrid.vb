'=============================================================================
'
' $Log: EditMultiStanzaEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.33  2008/08/11 16:13:56  jeroens
' Generalized EndEditHandler
'
' Revision 1.32  2008/08/02 03:04:18  jeroens
' Renamed resources
'
' Revision 1.31  2008/06/02 00:01:38  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.30  2008/05/29 22:22:59  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.29  2008/04/07 02:31:17  jeroens
' Cleaning up resources
'
' Revision 1.28  2008/02/01 01:17:25  jeroens
' -9999 values hidden from view
'
' Revision 1.27  2007/10/15 01:42:23  jeroens
' * Neatified
'
' Revision 1.26  2007/09/10 18:08:16  jeroens
' + Added option to apply grid values to core
'
' Revision 1.25  2007/08/03 17:17:59  jeroens
' * Uses cell-based unit support
'
' Revision 1.24  2007/08/02 17:46:57  joeb
' Redim data in RefreshGraphData
'
' Revision 1.23  2007/07/12 16:01:32  jeroens
' * Reorganized shapes, ff, ts
'
' Revision 1.22  2007/07/06 20:11:19  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.21  2007/06/29 23:29:10  joeh
' Comment out sg.Apply()
'
' Revision 1.20  2007/06/29 23:18:40  joeh
' Add hard coded strings to resource file
' Make form re-sizable
'
' Revision 1.19  2007/06/28 04:03:04  jeroens
' - Start age cannot be changed from this interface anymore
'
' Revision 1.18  2007/05/31 13:11:22  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.17  2007/05/20 01:04:07  jeroens
' * RefreshBasicInputGrid merged with SetStanzaGroupValues
' * SetStanzaGroupValues will now always Apply values to the core
'
' Revision 1.16  2007/04/18 01:07:11  joeh
' *Fine tune EditMultiStanza UI
'
' Revision 1.15  2007/04/17 01:12:35  joeh
' *Fine tune Edit Multi Stanza
'
' Revision 1.14  2007/04/14 00:20:11  joeh
' *add "Edit Multi Stanza" submenu
'
' Revision 1.13  2007/04/13 01:00:07  joeh
' *Implement combo box for Forcing Function
'
' Revision 1.12  2007/04/12 01:00:38  joeh
' *Implement combo box for Name of Species
'
' Revision 1.11  2007/04/11 21:16:09  joeh
' *Implement combo box for Name of Species
'
' Revision 1.10  2007/04/06 01:02:12  joeh
' *Implement Calculate button functionality
'
' Revision 1.9  2007/04/03 23:19:07  joeh
' *Implement the graphics of Number, Weight and Biomass
'
' Revision 1.8  2007/03/30 18:23:17  joeh
' *Expose the index of the stanza config that is clicked in the Basic Input grid
'
' Revision 1.7  2007/03/30 13:39:07  jeroens
' * Changed datatypes to Single
' * Fixed silly CurvParam rounding bug
'
'=============================================================================

#Region "Imports directive"
Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2
Imports EwEUtils.Core

#End Region

<CLSCompliant(False)> _
Public Class EditMultiStanzaEwEGrid
    : Inherits EwEGrid

#Region "Private variables"
    Private m_StanzaGroupName() As String
    Private m_NStanzaGroup As Integer
    Private m_StanzaClicked As cEcoPathGroupInput 'Clicked in the Basic Input form
    Private m_ClickedStanzaGroupIndex As Integer
    Private m_ClickedStanzaGroupName As String
    Private m_CurvParam As Single = 0.0
    Private m_RecruitPower As Single = 0.0
    Private m_RelBiomassAccumRate As Single = 0.0
    Private m_WmatWinf As Single = 0.0
    Private m_ForcingFunctName() As String
    Private m_NForcingFunct As Integer
    Private m_ClickedForcingFunctName As String
    Private m_ClickedForcingFunctNum As Integer
    Private m_FixedFecundity As Boolean
    'these variables are set only once during dialog box load
    'thus, will be used to reset sg to its original value
    Private m_StartAge() As Integer
    'Private m_Biomass() As Single
    'Private m_Mortality() As Single
    'Private m_CB() As Single
    'end of variables
    Private m_NStanza As Integer
    Private m_NumberAtAge() As Single
    Private m_WeightAtAge() As Single
    Private m_BiomassAtAge() As Single
    Private m_MaxAge As Integer

    'Private m_SelectedStanzaGroupName As String 'Selected from the combo box
    Private m_bm As SourceGrid2.BehaviorModels.IBehaviorModel = New EndEditHandler(Me)


    Private Enum eColumnTypes
        Index = 0
        Name = 1
        StartAge = 2
        BiomassAreaInput = 3
        PBInput = 4
        QBInput = 5
    End Enum
#End Region

#Region "Constructors"
    Public Sub New()
        'nothing
    End Sub

    Public Sub New(ByVal objStanzaClicked As cEcoPathGroupInput)
        'Me.New()
        m_StanzaClicked = objStanzaClicked
    End Sub
#End Region

#Region "Properties"

    Public ReadOnly Property StanzaGroupName(ByVal intStanzaGroupNum As Integer) As String
        Get
            Return m_StanzaGroupName(intStanzaGroupNum)
        End Get
    End Property

    Public ReadOnly Property StanzaIndex() As Integer
        Get
            Return m_ClickedStanzaGroupIndex
        End Get
    End Property

    Public Property ClickedStanzaGroupName() As String
        Get
            Return m_ClickedStanzaGroupName
        End Get
        Set(ByVal value As String)
            m_ClickedStanzaGroupName = value
        End Set
    End Property

    Public Property CurvParam() As Single
        Get
            Return m_CurvParam
        End Get
        Set(ByVal value As Single)
            m_CurvParam = value
        End Set
    End Property

    Public Property RecruitPower() As Single
        Get
            Return m_RecruitPower
        End Get
        Set(ByVal value As Single)
            m_RecruitPower = value
        End Set
    End Property

    Public Property RelBiomassAccumRate() As Single
        Get
            Return m_RelBiomassAccumRate
        End Get
        Set(ByVal value As Single)
            m_RelBiomassAccumRate = value
        End Set
    End Property

    Public Property WmatWinf() As Single
        Get
            Return m_WmatWinf
        End Get
        Set(ByVal value As Single)
            m_WmatWinf = value
        End Set
    End Property

    Public ReadOnly Property ForcingFunctionName(ByVal intForcingFunctionNum As Integer) As String
        Get
            Return m_ForcingFunctName(intForcingFunctionNum)
        End Get
    End Property

    Public Property ClickedForcingFunctName() As String
        Get
            Return m_ClickedForcingFunctName
        End Get
        Set(ByVal value As String)
            m_ClickedForcingFunctName = value
        End Set
    End Property

    Public Property FixedFecundity() As Boolean
        Get
            Return m_FixedFecundity
        End Get
        Set(ByVal value As Boolean)
            m_FixedFecundity = value
        End Set
    End Property

    Public ReadOnly Property NumberAtAge(ByVal intAge As Integer) As Single
        Get
            Return m_NumberAtAge(intAge)
        End Get
    End Property

    Public ReadOnly Property WeightAtAge(ByVal intAge As Integer) As Single
        Get
            Return m_WeightAtAge(intAge)
        End Get
    End Property

    Public ReadOnly Property BiomassAtAge(ByVal intAge As Integer) As Single
        Get
            Return m_BiomassAtAge(intAge)
        End Get
    End Property

    Public ReadOnly Property StartAge(ByVal intAge As Integer) As Integer
        Get
            Return m_StartAge(intAge)
        End Get
    End Property

    Public ReadOnly Property MaxAge() As Integer
        Get
            Return m_MaxAge
        End Get
    End Property

    Public ReadOnly Property NStanza() As Integer
        Get
            Return m_NStanza
        End Get
    End Property

    Public ReadOnly Property NStanzaGroup() As Integer
        Get
            Return m_NStanzaGroup
        End Get
    End Property

    Public ReadOnly Property NForcingFunction() As Integer
        Get
            Return m_NForcingFunct
        End Get
    End Property

    'Public WriteOnly Property SelectedStanzaGroupName() As String
    '    Set(ByVal value As String)
    '        m_SelectedStanzaGroupName = value
    '    End Set
    'End Property

    'Public WriteOnly Property StanzaClicked() As cEcoPathGroupInput
    '    Set(ByVal value As cEcoPathGroupInput)
    '        m_StanzaClicked = value
    '    End Set
    'End Property
#End Region

    Public Sub CalculateStanzaParametrs()
        Dim core As cCore = cCore.GetInstance()
        Dim sg As cStanzaGroup = Nothing

        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)
        sg.CalculateParameters()
    End Sub

    Public Sub RefreshMultiStanzaGrid()

        Dim core As cCore = cCore.GetInstance()
        Dim sg As cStanzaGroup = Nothing
        Dim ewec As EwECell = Nothing
        Dim bReadOnly As Boolean
        Dim source As cCoreInputOutputBase = Nothing
        Dim iRow As Integer

        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)

        ' Remove existing rows
        Me.RowsCount = 1

        For iStanza As Integer = 1 To m_NStanza

            source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
            iRow = Me.AddRow

            'Index
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)

            'Name
            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            'Start age
            ewec = New EwECell(0, GetType(Integer))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = sg.GetVariable(eVarNameFlags.StartAge, iStanza)
            ' JS 27jun07: start ages only editable from EditGroups interface
            ewec.Style = StyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.StartAge) = ewec
            m_StartAge(iStanza) = CInt(sg.GetVariable(eVarNameFlags.StartAge, iStanza))

            'Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = sg.Biomass(iStanza)
            'Ignore core read-only status; only leading group can edit
            bReadOnly = (sg.LeadingB <> iStanza)
            If bReadOnly Then
                ewec.Style = StyleGuide.eStyleFlags.NotEditable
            Else
                ewec.Style = StyleGuide.eStyleFlags.OK
            End If
            Me(iRow, eColumnTypes.BiomassAreaInput) = ewec
            Me(iRow, eColumnTypes.BiomassAreaInput).Behaviors.Add(m_bm)

            'Total Mortality
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = sg.Mortality(iStanza)
            Me(iRow, eColumnTypes.PBInput) = ewec
            Me(iRow, eColumnTypes.PBInput).Behaviors.Add(m_bm)

            'Consumption/Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = sg.CB(iStanza)
            bReadOnly = (sg.LeadingCB <> iStanza)
            If bReadOnly Then
                ewec.Style = StyleGuide.eStyleFlags.NotEditable
            Else
                ewec.Style = StyleGuide.eStyleFlags.OK
            End If
            Me(iRow, eColumnTypes.QBInput) = ewec
            Me(iRow, eColumnTypes.QBInput).Behaviors.Add(m_bm)
        Next

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 6)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.StartAge) = New EwEColumnHeaderCell(My.Resources.HEADER_STARTAGE)
        Me(0, eColumnTypes.BiomassAreaInput) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS_UNIT, StyleGuide.eUnitType.Currency)
        Me(0, eColumnTypes.PBInput) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, StyleGuide.eUnitType.Time)
        Me(0, eColumnTypes.QBInput) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, StyleGuide.eUnitType.Time)

        Me.AutoStretchColumnsToFitWidth = True
    End Sub

    Protected Overrides Sub FillData()
        Dim multiStanza As cEcoPathGroupInput = Nothing
        Dim core As cCore = cCore.GetInstance()
        Dim source As cCoreInputOutputBase = Nothing
        Dim sg As cStanzaGroup = Nothing
        'Dim iRow As Integer
        Dim ewec As EwECell = Nothing
        'Dim bReadOnly As Boolean
        Dim bEcosimLoaded As Boolean = core.StateMonitor.HasEcosimLoaded()


        'Me.RowsCount = 1

        m_NStanzaGroup = core.nStanzas
        ReDim m_StanzaGroupName(m_NStanzaGroup - 1)
        For i As Integer = 0 To m_NStanzaGroup - 1
            sg = core.StanzaGroups(i)
            m_StanzaGroupName(i) = sg.Name
        Next

        If bEcosimLoaded Then
            GetAvailableForcingFunction()
        End If

        If m_StanzaClicked Is Nothing Then
            m_ClickedStanzaGroupIndex = 0
        Else
            m_ClickedStanzaGroupIndex = m_StanzaClicked.StanzaID
        End If

        DetermineClickedStanzaGroup()
    End Sub

    Public Sub SetStanzaGroupValues(ByVal bApplyToCore As Boolean)
        Dim multiStanza As cEcoPathGroupInput = Nothing
        Dim core As cCore = cCore.GetInstance()
        'Dim source As cCoreInputOutputBase = Nothing
        Dim sg As cStanzaGroup = Nothing

        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)

        'sg.Name = m_ClickedStanzaGroupName
        sg.VBGF = m_CurvParam
        sg.RecruitmentPower = m_RecruitPower
        sg.BiomassAccumulationRate = m_RelBiomassAccumRate
        sg.WmatWinf = m_WmatWinf
        sg.HatchCode = m_ClickedForcingFunctNum
        sg.FixedFecundity = m_FixedFecundity

        For iStanza As Integer = 1 To m_NStanza

            'source = core.EcoPathGroupInputs(sg.iGroups(iStanza))

            ' JS 27jun07: stanza ages only editable from EditGroups interface
            ''Start age
            'sg.SetVariable(eVarNameFlags.StartAge, Me(iStanza, eColumnTypes.StartAge).Value, iStanza)

            'Biomass
            'source.SetVariable(eVarNameFlags.BiomassAreaInput, Me(iStanza, eColumnTypes.BiomassAreaInput).Value)
            'sg.SetVariable(eVarNameFlags.BiomassAreaInput, Me(iStanza, eColumnTypes.BiomassAreaInput).Value, iStanza)
            sg.Biomass(iStanza) = CSng(Me(iStanza, eColumnTypes.BiomassAreaInput).Value)

            'Total Mortality
            'source.SetVariable(eVarNameFlags.PBInput, Me(iStanza, eColumnTypes.PBInput).Value)
            'sg.SetVariable(eVarNameFlags.PBInput, Me(iStanza, eColumnTypes.PBInput).Value, iStanza)
            sg.Mortality(iStanza) = CSng(Me(iStanza, eColumnTypes.PBInput).Value)

            'Consumption/Biomass
            'source.SetVariable(eVarNameFlags.QBInput, Me(iStanza, eColumnTypes.QBInput).Value)
            'sg.SetVariable(eVarNameFlags.QBInput, Me(iStanza, eColumnTypes.QBInput).Value, iStanza)
            sg.CB(iStanza) = CSng(Me(iStanza, eColumnTypes.QBInput).Value)
        Next

        If bApplyToCore Then sg.Apply()

    End Sub

    Public Sub RefreshGraphData()
        Dim core As cCore = cCore.GetInstance()
        'Dim source As cCoreInputOutputBase = Nothing
        Dim sg As cStanzaGroup = Nothing

        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)
        m_MaxAge = sg.MaxAge
        'jb the max age could have changes during CalculateStanzaParametrs()
        ReDim m_NumberAtAge(m_MaxAge)
        ReDim m_WeightAtAge(m_MaxAge)
        ReDim m_BiomassAtAge(m_MaxAge)
        For intIndex As Integer = 0 To m_MaxAge
            m_NumberAtAge(intIndex) = sg.NumberAtAge(intIndex)
            m_WeightAtAge(intIndex) = sg.WeightAtAge(intIndex)
            m_BiomassAtAge(intIndex) = sg.BiomassAtAge(intIndex)
        Next
    End Sub

    Public Sub ResetStanzaGroupValues()
        Dim core As cCore = cCore.GetInstance()
        'Dim source As cCoreInputOutputBase = Nothing
        Dim sg As cStanzaGroup = Nothing

        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)
        sg.Cancel()
        'For iIndex As Integer = 1 To sg.NStanzas
        '    sg.StartAge(iIndex) = m_StartAge(iIndex)
        '    sg.Biomass(iIndex) = m_Biomass(iIndex)
        '    sg.Mortality(iIndex) = m_Mortality(iIndex)
        '    sg.CB(iIndex) = m_CB(iIndex)
        'Next
    End Sub

    Public Sub DetermineClickedStanzaGroupIndex()
        Dim core As cCore = cCore.GetInstance()
        Dim sg As cStanzaGroup = Nothing

        For iIndex As Integer = 0 To m_NStanzaGroup - 1
            sg = core.StanzaGroups(iIndex)
            'If sg.Name = m_SelectedStanzaGroupName Then
            If sg.Name = m_ClickedStanzaGroupName Then
                m_ClickedStanzaGroupIndex = sg.Index - 1
                Return
            End If
        Next
    End Sub

    Public Sub DetermineClickedStanzaGroup()
        Dim multiStanza As cEcoPathGroupInput = Nothing
        Dim core As cCore = cCore.GetInstance()
        Dim source As cCoreInputOutputBase = Nothing
        Dim sg As cStanzaGroup = Nothing
        'Dim iRow As Integer
        Dim ewec As EwECell = Nothing
        'Dim bReadOnly As Boolean

        'Me.RowsCount = 1

        'm_NStanzaGroup = core.StanzaGroups.Count
        'ReDim m_StanzaGroupName(m_NStanzaGroup - 1)
        'For i As Integer = 0 To m_NStanzaGroup - 1
        '    sg = core.StanzaGroups(i)
        '    m_StanzaGroupName(i) = sg.Name
        'Next

        'If m_StanzaClicked Is Nothing Then
        '    m_ClickedStanzaGroupIndex = 0
        'Else
        '    m_ClickedStanzaGroupIndex = m_StanzaClicked.StanzaID
        'End If
        sg = core.StanzaGroups(m_ClickedStanzaGroupIndex)

        m_ClickedStanzaGroupName = sg.Name
        m_CurvParam = sg.VBGF
        m_RecruitPower = sg.RecruitmentPower
        m_RelBiomassAccumRate = sg.BiomassAccumulationRate
        m_WmatWinf = sg.WmatWinf
        m_ClickedForcingFunctNum = sg.HatchCode
        m_FixedFecundity = sg.FixedFecundity

        m_MaxAge = sg.MaxAge
        ReDim m_NumberAtAge(m_MaxAge)
        ReDim m_WeightAtAge(m_MaxAge)
        ReDim m_BiomassAtAge(m_MaxAge)
        For intIndex As Integer = 0 To m_MaxAge
            m_NumberAtAge(intIndex) = sg.NumberAtAge(intIndex)
            m_WeightAtAge(intIndex) = sg.WeightAtAge(intIndex)
            m_BiomassAtAge(intIndex) = sg.BiomassAtAge(intIndex)
        Next

        m_NStanza = sg.NStanzas
        ReDim m_StartAge(m_NStanza)

    End Sub

    Private Sub GetAvailableForcingFunction()
        Dim core As cCore = cCore.GetInstance()
        'Dim bEcosimLoaded As Boolean = core.StateMonitor.HasEcosimLoaded()
        'Dim EcoSimScenario As cEcoSimScenario = Nothing
        Dim ForcingFunctionManager As cForcingFunctionManager = Nothing
        ''Dim ForcingFunction As cForcingFunction = Nothing

        'If bEcosimLoaded Then
        'EcoSimScenario = core.EcosimScenarios(core.ActiveEcosimScenarioIndex)

        ForcingFunctionManager = core.ForcingShapeManager


        m_NForcingFunct = ForcingFunctionManager.Count
        ReDim m_ForcingFunctName(m_NForcingFunct - 1)
        For iIndex As Integer = 0 To m_NForcingFunct - 1
            m_ForcingFunctName(iIndex) = ForcingFunctionManager.Item(iIndex).Name
        Next

        'For iIndex As Integer = 0 To ForcingFunctionManager.Count - 1
        '    ForcingFunction = ForcingFunctionManager.Item(iIndex)
        '    MsgBox(ForcingFunction.Name, MsgBoxStyle.Information)
        'Next
        'End If
    End Sub

    Public Sub DetermineClickedForcingFunctionNumber()
        Dim core As cCore = cCore.GetInstance()
        Dim ForcingFunctionManager As cForcingFunctionManager = Nothing

        ForcingFunctionManager = core.ForcingShapeManager

        If m_ClickedForcingFunctName <> "" Then
            For iIndex As Integer = 0 To m_NForcingFunct - 1
                If ForcingFunctionManager.Item(iIndex).Name = m_ClickedForcingFunctName Then
                    m_ClickedForcingFunctNum = ForcingFunctionManager.Item(iIndex).Index
                    Return
                End If
            Next
            'None is selected
            m_ClickedForcingFunctNum = 0
        Else
            m_ClickedForcingFunctNum = 0
        End If
    End Sub
End Class
