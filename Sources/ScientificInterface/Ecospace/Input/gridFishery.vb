#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridFishery
        : Inherits EwEGrid

        Private m_ah As New SourceGrid2.BehaviorModels.CustomEvents

        Public Sub New()

            MyBase.New()
            AddHandler m_ah.ValueChanged, New SourceGrid2.PositionEventHandler(AddressOf m_ahValueChanged)
            Me.FixedColumnWidths = False

        End Sub

#Region "Grid Overriden methods"

        Protected Overrides Sub InitStyle()

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Me.Core.nFleets + 1, Me.Core.nHabitats + Me.Core.nMPAs + 4)

            'Set header cells #(0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_FLEETHABUSE)

            'Dynamic row header - fleet name
            For i As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcospaceFleets(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                '# Fleet name header 
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            'Dynamic column header - Habitats and MPAs
            For j As Integer = 0 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                Me(0, j + 2) = New EwEColumnHeaderCell(source.Name)
            Next

            'Dynamic column header - MPAs
            For k As Integer = 1 To Me.Core.nMPAs
                source = Me.Core.EcospaceMPAs(k)
                Me(0, Me.Core.nHabitats + 1 + k) = New EwEColumnHeaderCell(source.Name)
            Next

            'Column header cell - Effective power
            Me(0, Me.ColumnsCount - 2) = New EwEColumnHeaderCell(SharedResources.HEADER_EFFPOWER)
            'Column header cell - Tot.Eff.Multip.
            Me(0, Me.ColumnsCount - 1) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTEFFMULTI)

        End Sub

        Protected Overrides Sub FillData()

            For i As Integer = 1 To Me.Core.nFleets

                Dim source As cEcospaceFleet = Me.Core.EcospaceFleets(i)

                'Fleet / habitat assignments
                For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                    Me(i, iHabitat + 2) = New Cells.Real.CheckBox(source.HabitatFishery(iHabitat))
                    Me(i, iHabitat + 2).Behaviors.Add(m_ah)
                Next

                For iMPA As Integer = 1 To Me.Core.nMPAs
                    Me(i, Me.Core.nHabitats + 1 + iMPA) = New Cells.Real.CheckBox(CBool(source.MPAFishery(iMPA)))
                    Me(i, Me.Core.nHabitats + 1 + iMPA).Behaviors.Add(m_ah)
                Next

                Me(i, Me.ColumnsCount - 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EffectivePower)
                Me(i, Me.ColumnsCount - 1) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SEmult)

            Next

        End Sub

#End Region

        Private Sub m_ahValueChanged(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim fleet As cEcospaceFleet = Me.Core.EcospaceFleets(e.Position.Row)
            Dim col As Integer = e.Position.Column - 2
            Dim bChecked As Boolean = CBool(e.Cell.GetValue(e.Position))

            If col <= Me.Core.nHabitats - 1 Then
                ' Core will prevent conflicts in assigning habitats
                fleet.HabitatFishery(col) = bChecked
            ElseIf col <= Me.Core.nHabitats + Me.Core.nMPAs - 1 Then
                fleet.MPAFishery(col - Me.Core.nHabitats + 1) = bChecked
            End If

            UpdateRow(e.Position.Row)

        End Sub

        ''' <summary>
        ''' Update check boxes in a row
        ''' </summary>
        ''' <param name="i">Row number to update.</param>
        Private Sub UpdateRow(ByVal i As Integer)

            Dim source As cEcospaceFleet = Me.Core.EcospaceFleets(i)

            'Fleet / habitat assignments
            For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                Me(i, iHabitat + 2).Behaviors.Remove(m_ah)
                Me(i, iHabitat + 2).Value = CBool(source.HabitatFishery(iHabitat))
                Me(i, iHabitat + 2).Behaviors.Add(m_ah)
            Next

            For iMPA As Integer = 1 To Me.Core.nMPAs
                Me(i, Me.Core.nHabitats + 1 + iMPA).Behaviors.Remove(m_ah)
                Me(i, Me.Core.nHabitats + 1 + iMPA).Value = CBool(source.MPAFishery(iMPA))
                Me(i, Me.Core.nHabitats + 1 + iMPA).Behaviors.Add(m_ah)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSources() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

    End Class

End Namespace

