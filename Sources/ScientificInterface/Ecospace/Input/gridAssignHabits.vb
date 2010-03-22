#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports SourceGrid2
Imports EwEUtils.Core
Imports SourceGrid2.BehaviorModels

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to assign species to habitats.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridAssignHabits
        : Inherits EwEGrid

#Region " Privates "

        ''' <summary>Checkbox cell interaction.</summary>
        Private m_ah As CustomEvents = Nothing
        Private m_delegatePositionEvent As SourceGrid2.PositionEventHandler = Nothing

#End Region ' Privates

#Region " Construction / destruction "

        Public Sub New()

            MyBase.New()

            Me.m_delegatePositionEvent = New SourceGrid2.PositionEventHandler(AddressOf m_ahValueChanged)

            Me.m_ah = New CustomEvents()
            AddHandler m_ah.ValueChanged, m_delegatePositionEvent

            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)

            If (m_ah IsNot Nothing) Then
                RemoveHandler m_ah.ValueChanged, m_delegatePositionEvent
                Me.m_ah = Nothing
                Me.m_delegatePositionEvent = Nothing
            End If

            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction / destruction

#Region " Grid Overriden methods "

        Protected Overrides Sub InitStyle()

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Me.Core.nGroups + 2, Me.Core.nHabitats + 4)

            'Set header cells # (0,0)
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_GROUP_HABITAT)
            Me(0, 0).ColumnSpan = 2

            'Dynamic row header - group name 
            For i As Integer = 1 To Me.Core.nGroups
                source = Me.Core.EcospaceGroups(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)
            Next

            'Row header cell - Habitat area
            Me(Me.Core.nGroups + 1, 0) = New EwERowHeaderCell(Me.Core.nGroups + 1)
            Me(Me.Core.nGroups + 1, 1) = New EwERowHeaderCell(My.Resources.ECOSPACE_HEADER_HABITAT_AREA)

            'Dynamic column header - Habitat name
            For j As Integer = 0 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                ' +1 to compensate for header column, +1 to compensate for zero-based habitat index.
                Me(0, j + 2) = New EwEColumnHeaderCell(source.Name)
            Next

            'Column header cell - Ecospace area
            Me(0, Me.Core.nHabitats + 2) = New EwEColumnHeaderCell(My.Resources.HEADER_ECOSPACE_AREA)

            'Column header cell - Ecopath area
            Me(0, Me.Core.nHabitats + 3) = New EwEColumnHeaderCell(My.Resources.HEADER_ECOPATH_AREA)
            Me(0, Me.Core.nHabitats + 3).VisualModel.TextAlignment = ContentAlignment.MiddleLeft

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim groupEcospace As cEcospaceGroup = Nothing
            Dim groupEcopath As cEcoPathGroupInput = Nothing
            Dim cell As EwECellBase = Nothing

            ' Raster of formulas for check box cells
            Dim exFormulas(Me.Core.nGroups, Me.Core.nHabitats) As cExpression
            Dim exFormula As cExpression = Nothing
            Dim propFormula As cFormulaProperty = Nothing

            For iGroup As Integer = 1 To Me.Core.nGroups

                ' Get sources
                groupEcospace = Me.Core.EcospaceGroups(iGroup)
                groupEcopath = Me.Core.EcoPathGroupInputs(iGroup)

                For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                    ' Create check box cells
                    Me(iGroup, iHabitat + 2) = New Cells.Real.CheckBox(groupEcospace.PreferredHabitat(iHabitat))
                    Me(iGroup, iHabitat + 2).Behaviors.Add(m_ah)

                    ' Store formula that calculates the habitat area for this particular (group, habitat)
                    exFormulas(iGroup, iHabitat) = HabAreaFormula(iGroup, iHabitat)
                Next

                ' Ecospace Area Sum cell
                ' 1. Build formula that sums the total habitat area for this group
                exFormula = SumHabAreaFormula(iGroup, exFormulas)
                ' 2. Wrap formula in a property. This property is named to allow users to add remarks to it.
                propFormula = New cFormulaProperty(cValueID.Generate(groupEcospace.getID, "SumHabArea"), exFormula)
                ' 3. Apply formula to cell.
                Me(iGroup, Me.Core.nHabitats + 2) = New PropertyCell(propFormula)

                ' Ecopath area
                cell = New PropertyCell(Me.PropertyManager, groupEcopath, eVarNameFlags.Area)
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iGroup, Me.Core.nHabitats + 3) = cell

            Next

            ' Ecospace Area Sum cells - column habitat summaries
            For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                ' 1. Build formula that averages the total habitat of preferred areas for this habitat
                exFormula = AvgHabAreaFormula(iHabitat, exFormulas)
                ' 2. Wrap formula in a property. This property is named to allow users to add remarks to it.
                propFormula = New cFormulaProperty(cValueID.Generate(groupEcospace.getID, "AvgHabArea"), exFormula)
                ' 3. Apply formula to cell.
                Me(Me.Core.nGroups + 1, iHabitat + 2) = New PropertyCell(propFormula)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSources() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

#End Region ' Grid Overriden methods

#Region " Dumb bits "

        Private Sub m_ahValueChanged(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim group As cEcospaceGroup = Me.Core.EcospaceGroups(e.Position.Row)
            Dim iHabitat As Integer = e.Position.Column - 2

            ' Set new preferred habitat
            group.PreferredHabitat(iHabitat) = CBool(e.Cell.GetValue(e.Position))
            ' This may have affected other habitat assignments for this group: update the row
            Me.UpdateRow(e.Position.Row)

        End Sub

        Private Sub UpdateRow(ByVal iRow As Integer)
            Dim group As cEcospaceGroup = Me.Core.EcospaceGroups(iRow)

            For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                ' Updating from within code: do not throw value changed events
                Me(iRow, 2 + iHabitat).Behaviors.Remove(m_ah)
                ' Update value
                Me(iRow, 2 + iHabitat).Value = group.PreferredHabitat(iHabitat)
                ' Restore value changed event handler
                Me(iRow, 2 + iHabitat).Behaviors.Add(m_ah)
            Next

        End Sub

#End Region ' Dumb bits

#Region " Smart bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <para>Helper method; returns a live formula that returns the Habitat area proportion
        ''' for a given group and habitat, taking into account whether the group prefers 
        ''' that habitat.</para>
        ''' <para>In pseude code, this formula reads:</para>
        ''' <code>
        ''' if (
        ''' </code>
        ''' </summary>
        ''' <param name="iGroup">The group index to get the formula for.</param>
        ''' <param name="iHabitat">The habitat index to get the formula for.</param>
        ''' <returns>A cExpression containing the formula.</returns>
        ''' -------------------------------------------------------------------
        Private Function HabAreaFormula(ByVal iGroup As Integer, ByVal iHabitat As Integer) As cExpression

            Dim pm As cPropertyManager = Me.PropertyManager
            Dim group As cEcospaceGroup = Nothing
            Dim habitat As cEcospaceHabitat = Nothing
            Dim bopGroupPrefersHabitat As cBooleanOperand = Nothing
            Dim copHabArea As cConditionalOperation = Nothing

            group = Me.Core.EcospaceGroups(iGroup)
            habitat = Me.Core.EcospaceHabitats(iHabitat)

            ' 1. Construct PrefHab T/F test [group.PrefHab(iHabitat) = true]
            bopGroupPrefersHabitat = New cBooleanOperand( _
                cOperatorManager.getOperator(eOperators.EqualTo), _
                pm.GetProperty(group, eVarNameFlags.PreferredHabitat, habitat), _
                True)

            ' 2. Calculate area based on outcome of PrefHab T/F test [IIF(bopPreferredHabitat, habitat.HabAreaProportion(), 0)]
            copHabArea = New cConditionalOperation( _
                bopGroupPrefersHabitat, _
                pm.GetProperty(habitat, eVarNameFlags.HabAreaProportion), _
                0)

            ' 3. Return formula
            Return copHabArea

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; returns a formula that sums the preferred habitat area
        ''' for a given group.
        ''' </summary>
        ''' <param name="iGroup">The group index to get the formula for.</param>
        ''' <param name="exFormulas">Matrix (group, habitat) with 
        ''' <see cref="HabAreaFormula">habitat area formulas</see>.</param>
        ''' <returns>A cExpression containing the formula.</returns>
        ''' -------------------------------------------------------------------
        Private Function SumHabAreaFormula(ByVal iGroup As Integer, ByRef exFormulas(,) As cExpression) As cExpression

            Dim exSum(Me.Core.nHabitats - 1) As cExpression
            For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                exSum(iHabitat) = exFormulas(iGroup, iHabitat)
            Next iHabitat
            ' Return the sum of all preferred habitats
            Return New cMultiOperation(cMultiOperation.eOperatorType.Sum, exSum)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; returns a formula that averages the preferred habitat
        ''' area for a given habitat.
        ''' </summary>
        ''' <param name="iHabitat">The habitat index to get the formula for.</param>
        ''' <param name="exFormulas">Matrix (group, habitat) with 
        ''' <see cref="HabAreaFormula">habitat area formulas</see>.</param>
        ''' <returns>A cExpression containing the formula.</returns>
        ''' -------------------------------------------------------------------
        Private Function AvgHabAreaFormula(ByVal iHabitat As Integer, ByRef exFormulas(,) As cExpression) As cExpression

            Dim exSum(Me.Core.nGroups - 1) As cExpression
            For iGroup As Integer = 1 To Me.Core.nGroups
                exSum(iGroup - 1) = exFormulas(iGroup, iHabitat)
            Next iGroup
            ' Return the average of all preferred habitats.
            Return New cMultiOperation(cMultiOperation.eOperatorType.AvgNonZero, exSum)

        End Function

#End Region ' Smart bits

    End Class

End Namespace

