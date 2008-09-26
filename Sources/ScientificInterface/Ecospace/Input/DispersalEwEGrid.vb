'==============================================================================
'
' $Log: DispersalEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:55  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/07/02 18:02:39  jeroens
' Updates on new Ecospace scenario load
'
' Revision 1.11  2008/06/02 00:01:23  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.10  2008/05/29 22:22:39  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.9  2008/04/07 02:31:11  jeroens
' Cleaning up resources
'
' Revision 1.8  2007/11/07 21:40:49  jeroens
' * True/False (Advecting, Migratory) columns changed to checkboxes
'
' Revision 1.7  2007/10/16 15:21:11  jeroens
' * Responds to Ecopath group changes
'
' Revision 1.6  2007/07/26 18:43:58  jeroens
' * Woopsy
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class DispersalEwEGrid
        : Inherits EwEGrid

        Private m_ah As New SourceGrid2.BehaviorModels.CustomEvents
        Private m_core As cCore = cCore.GetInstance()

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            DispersalRate
            RelDisp
            RelVul
            RelFeedRate
            Advected
            Migrating
            NSCont
            EWCont
            BarrierAvoidance
        End Enum

        Public Sub New()
            MyBase.New()
            AddHandler m_ah.ValueChanged, New SourceGrid2.PositionEventHandler(AddressOf m_ahValueChanged)
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            'Add column headers
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.DispersalRate) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_BASEDISPRATE)
            Me(0, eColumnTypes.RelDisp) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELDISP)
            Me(0, eColumnTypes.RelVul) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELVUL)
            Me(0, eColumnTypes.RelFeedRate) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_RELFEEDRATE)
            Me(0, eColumnTypes.Advected) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_ADVECTED)
            Me(0, eColumnTypes.Migrating) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_MIGRATING)
            Me(0, eColumnTypes.NSCont) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_NSCONT)
            Me(0, eColumnTypes.EWCont) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_EWCONT)
            Me(0, eColumnTypes.BarrierAvoidance) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_DISPERSAL_BARRIERAVOIDANCEWT)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcospaceGroup = Nothing

            For iGroup As Integer = 1 To Me.m_core.nGroups
                Me.Rows.Insert(iGroup)

                source = Me.m_core.EcospaceGroups(iGroup)
                Me(iGroup, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

                'MVel - Base dispersal rate
                Me(iGroup, eColumnTypes.DispersalRate) = New PropertyCell(source, eVarNameFlags.MVel)
                'Rel dispersal in bad habitat
                Me(iGroup, eColumnTypes.RelDisp) = New PropertyCell(source, eVarNameFlags.RelMoveBad)
                ' Rel. vul.to pred. in bad habitat
                Me(iGroup, eColumnTypes.RelVul) = New PropertyCell(source, eVarNameFlags.RelVulBad)
                'Rel. feed.rate in bad habitat
                Me(iGroup, eColumnTypes.RelFeedRate) = New PropertyCell(source, eVarNameFlags.EatEffBad)
                'Advected?
                Me(iGroup, eColumnTypes.Advected) = New Cells.Real.CheckBox(source.IsAdvected())
                Me(iGroup, eColumnTypes.Advected).Behaviors.Add(m_ah)
                'Migrating?
                Me(iGroup, eColumnTypes.Migrating) = New Cells.Real.CheckBox(source.IsMigratory())
                Me(iGroup, eColumnTypes.Migrating).Behaviors.Add(m_ah)
                'North/south concentration
                Me(iGroup, eColumnTypes.NSCont) = New PropertyCell(source, eVarNameFlags.MigrationConcRow)
                'East/west concentration
                Me(iGroup, eColumnTypes.EWCont) = New PropertyCell(source, eVarNameFlags.MigrationConcCol)
                'Barrier avoidance weight
                Me(iGroup, eColumnTypes.BarrierAvoidance) = New PropertyCell(source, eVarNameFlags.BarrierAvoidanceWeight)

                Me.UpdateRow(iGroup)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSources() As EwECore.eMessageSource()
            Get
                ' Refresh on Ecopath notifications
                Return New eMessageSource() {eMessageSource.EcoPath, eMessageSource.EcoSpace}
            End Get
        End Property

        Private Sub m_ahValueChanged(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim iGroup As Integer = e.Position.Row
            Dim group As cEcospaceGroup = Me.m_core.EcospaceGroups(iGroup)
            Dim colType As eColumnTypes = DirectCast(e.Position.Column, eColumnTypes)
            Dim bChecked As Boolean = CBool(e.Cell.GetValue(e.Position))

            Select Case colType
                Case eColumnTypes.Advected
                    group.IsAdvected = bChecked
                Case eColumnTypes.Migrating
                    group.IsMigratory = bChecked
            End Select

            UpdateRow(iGroup)

        End Sub


        ''' <summary>
        ''' Update check boxes in a row
        ''' </summary>
        ''' <param name="i">Row number to update.</param>
        Private Sub UpdateRow(ByVal i As Integer)

            Dim group As cEcospaceGroup = Me.m_core.EcospaceGroups(i)

            Me(i, eColumnTypes.Advected).Behaviors.Remove(m_ah)
            Me(i, eColumnTypes.Advected).Value = CBool(group.IsAdvected())
            Me(i, eColumnTypes.Advected).Behaviors.Add(m_ah)

            Me(i, eColumnTypes.Migrating).Behaviors.Remove(m_ah)
            Me(i, eColumnTypes.Migrating).Value = CBool(group.IsMigratory())
            Me(i, eColumnTypes.Migrating).Behaviors.Add(m_ah)

        End Sub

        Private Sub DispersalEwEGrid_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Me.m_ah = Nothing
        End Sub

    End Class

End Namespace
