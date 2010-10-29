#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.BehaviorModels

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to set dispersal rates.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
     Public Class DispersalEwEGrid
        : Inherits EwEGrid

        Private m_ah As CustomEvents = Nothing

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

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            m_ah = New CustomEvents
            AddHandler m_ah.ValueChanged, New SourceGrid2.PositionEventHandler(AddressOf m_ahValueChanged)
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (Me.m_ah IsNot Nothing) Then
                RemoveHandler m_ah.ValueChanged, New SourceGrid2.PositionEventHandler(AddressOf m_ahValueChanged)
                Me.m_ah = Nothing
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction / destruction

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            'Add column headers
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
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

            For iGroup As Integer = 1 To Me.Core.nGroups
                Me.Rows.Insert(iGroup)

                source = Me.Core.EcospaceGroups(iGroup)
                Me(iGroup, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                'MVel - Base dispersal rate
                Me(iGroup, eColumnTypes.DispersalRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MVel)
                'Rel dispersal in bad habitat
                Me(iGroup, eColumnTypes.RelDisp) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RelMoveBad)
                ' Rel. vul.to pred. in bad habitat
                Me(iGroup, eColumnTypes.RelVul) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.RelVulBad)
                'Rel. feed.rate in bad habitat
                Me(iGroup, eColumnTypes.RelFeedRate) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EatEffBad)
                'Advected?
                Me(iGroup, eColumnTypes.Advected) = New Cells.Real.CheckBox(source.IsAdvected())
                Me(iGroup, eColumnTypes.Advected).Behaviors.Add(m_ah)
                'Migrating?
                Me(iGroup, eColumnTypes.Migrating) = New Cells.Real.CheckBox(source.IsMigratory())
                Me(iGroup, eColumnTypes.Migrating).Behaviors.Add(m_ah)
                'North/south concentration
                Me(iGroup, eColumnTypes.NSCont) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MigrationConcRow)
                'East/west concentration
                Me(iGroup, eColumnTypes.EWCont) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.MigrationConcCol)
                'Barrier avoidance weight
                Me(iGroup, eColumnTypes.BarrierAvoidance) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BarrierAvoidanceWeight)

                Me.UpdateRow(iGroup)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSources() As eCoreComponentType()
            Get
                ' Refresh on Ecopath notifications
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

        Private Sub m_ahValueChanged(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim iGroup As Integer = e.Position.Row
            Dim group As cEcospaceGroup = Me.Core.EcospaceGroups(iGroup)
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

            Dim group As cEcospaceGroup = Me.Core.EcospaceGroups(i)

            Me(i, eColumnTypes.Advected).Behaviors.Remove(m_ah)
            Me(i, eColumnTypes.Advected).Value = CBool(group.IsAdvected())
            Me(i, eColumnTypes.Advected).Behaviors.Add(m_ah)

            Me(i, eColumnTypes.Migrating).Behaviors.Remove(m_ah)
            Me(i, eColumnTypes.Migrating).Value = CBool(group.IsMigratory())
            Me(i, eColumnTypes.Migrating).Behaviors.Add(m_ah)

        End Sub

    End Class

End Namespace
