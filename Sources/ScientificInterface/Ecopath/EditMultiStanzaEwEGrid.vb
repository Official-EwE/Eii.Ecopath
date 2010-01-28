#Region " Imports "
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

    Private m_stanzagroup As cStanzaGroup = Nothing
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
    End Sub

#End Region

#Region "Properties"

    Public Property StanzaGroup() As cStanzaGroup
        Get
            Return Me.m_stanzagroup
        End Get
        Set(ByVal value As cStanzaGroup)
            Me.m_stanzagroup = value
            Me.RefreshContent()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' No dockink! No dockink!
    ''' </summary>
    ''' -----------------------------------------------------------------------
     Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

#End Region

    Public Sub CalculateStanzaParameters()
        ' Sanity check
        If (Me.m_stanzagroup Is Nothing) Then Return
        Me.m_stanzagroup.CalculateParameters()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 6)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.StartAge) = New EwEColumnHeaderCell(My.Resources.HEADER_STARTAGE)
        Me(0, eColumnTypes.BiomassAreaInput) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS_UNIT, cStyleGuide.eUnitType.Currency)
        Me(0, eColumnTypes.PBInput) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, cStyleGuide.eUnitType.Time)
        Me(0, eColumnTypes.QBInput) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, cStyleGuide.eUnitType.Time)

        Me.FixedColumnWidths = False
        Me.AutoStretchColumnsToFitWidth = True
    End Sub

    Protected Overrides Sub FillData()

        Dim source As cEcoPathGroupInput = Nothing
        Dim ewec As EwECell = Nothing
        Dim bReadOnly As Boolean
        Dim iRow As Integer
        Dim bIsEcosimLoaded As Boolean = (core.ActiveEcosimScenarioIndex > -1)

        ' Remove existing rows
        Me.RowsCount = 1

        If (Me.m_stanzagroup Is Nothing) Then Return

        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas

            source = core.EcoPathGroupInputs(Me.m_stanzagroup.iGroups(iStanza))
            iRow = Me.AddRow

            'Index
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)

            'Name
            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            'Start age
            ewec = New EwECell(0, GetType(Integer))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.GetVariable(eVarNameFlags.StartAge, iStanza)
            ' JS 27jun07: start ages only editable from EditGroups interface
            ewec.Style = cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.StartAge) = ewec

            'Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Biomass(iStanza)
            'Ignore core read-only status; only leading group can edit
            bReadOnly = (Me.m_stanzagroup.LeadingB <> iStanza)
            If bReadOnly Then
                ewec.Style = cStyleGuide.eStyleFlags.NotEditable
            Else
                ewec.Style = cStyleGuide.eStyleFlags.OK
            End If
            Me(iRow, eColumnTypes.BiomassAreaInput) = ewec
            Me(iRow, eColumnTypes.BiomassAreaInput).Behaviors.Add(m_bm)

            'Total Mortality
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.Mortality(iStanza)
            Me(iRow, eColumnTypes.PBInput) = ewec
            Me(iRow, eColumnTypes.PBInput).Behaviors.Add(m_bm)

            'Consumption/Biomass
            ewec = New EwECell(0, GetType(Single))
            ewec.SuppressZero(cCore.NULL_VALUE) = True
            ewec.Value = Me.m_stanzagroup.CB(iStanza)
            bReadOnly = (Me.m_stanzagroup.LeadingCB <> iStanza)
            If bReadOnly Then
                ewec.Style = cStyleGuide.eStyleFlags.NotEditable
            Else
                ewec.Style = cStyleGuide.eStyleFlags.OK
            End If
            Me(iRow, eColumnTypes.QBInput) = ewec
            Me(iRow, eColumnTypes.QBInput).Behaviors.Add(m_bm)
        Next
    End Sub

    Public Sub SetStanzaGroupValues(ByVal bApplyToCore As Boolean)

        For iStanza As Integer = 1 To Me.m_stanzagroup.NStanzas

            ' JS 27jun07: stanza ages only editable from EditGroups interface
            ''Start age
            'Me.m_stanzagroup.SetVariable(eVarNameFlags.StartAge, Me(iStanza, eColumnTypes.StartAge).Value, iStanza)
            'Biomass
            Me.m_stanzagroup.Biomass(iStanza) = CSng(Me(iStanza, eColumnTypes.BiomassAreaInput).Value)
            'Total Mortality
            Me.m_stanzagroup.Mortality(iStanza) = CSng(Me(iStanza, eColumnTypes.PBInput).Value)
            'Consumption/Biomass
            Me.m_stanzagroup.CB(iStanza) = CSng(Me(iStanza, eColumnTypes.QBInput).Value)

        Next

        If bApplyToCore Then
            ' JS 090826: apply changes for all stanza groups, not only the last used stanza group
            For iIndex As Integer = 0 To core.nStanzas - 1
                core.StanzaGroups(iIndex).Apply()
            Next
        End If

    End Sub

    Public Sub ResetStanzaGroupValues()
        Me.m_stanzagroup.Cancel()
    End Sub

End Class
