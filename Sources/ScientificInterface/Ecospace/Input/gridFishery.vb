'==============================================================================
'
' $Log: gridFishery.vb,v $
' Revision 1.2  2008/12/15 15:55:35  jeroens
' no message
'
' Revision 1.1  2008/11/04 04:58:44  jeroens
' Renamed
'
' Revision 1.1  2008/09/26 07:31:56  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridFishery
        : Inherits EwEGrid

        'Core reference
        Private m_Core As cCore = cCore.GetInstance()

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

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(m_Core.nFleets + 1, m_Core.nHabitats + m_Core.nMPAs + 4)

            'Set header cells #(0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_FLEETHABUSE)

            'Dynamic row header - fleet name
            For i As Integer = 1 To m_Core.nFleets
                source = m_Core.EcospaceFleets(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                '# Fleet name header 
                Me(i, 1) = New EwERowHeaderCell(source.Name)
            Next

            'Dynamic column header - Habitats and MPAs
            For j As Integer = 0 To m_Core.nHabitats - 1
                source = m_Core.EcospaceHabitats(j)
                Me(0, j + 2) = New EwEColumnHeaderCell(source.Name)
            Next

            'Dynamic column header - MPAs
            For k As Integer = 1 To m_Core.nMPAs
                source = m_Core.EcospaceMPAs(k)
                Me(0, m_Core.nHabitats + 1 + k) = New EwEColumnHeaderCell(source.Name)
            Next

            'Column header cell - Effective power
            Me(0, Me.ColumnsCount - 2) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFPOWER)
            'Column header cell - Tot.Eff.Multip.
            Me(0, Me.ColumnsCount - 1) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTEFFMULTI)

        End Sub


        Protected Overrides Sub FillData()

            For i As Integer = 1 To m_Core.nFleets

                Dim source As cEcospaceFleet = m_Core.EcospaceFleets(i)

                'Fleet / habitat assignments
                For iHabitat As Integer = 0 To m_Core.nHabitats - 1
                    Me(i, iHabitat + 2) = New Cells.Real.CheckBox(source.HabitatFishery(iHabitat))
                    Me(i, iHabitat + 2).Behaviors.Add(m_ah)
                Next

                For iMPA As Integer = 1 To m_Core.nMPAs
                    Me(i, m_Core.nHabitats + 1 + iMPA) = New Cells.Real.CheckBox(CBool(source.MPAFishery(iMPA)))
                    Me(i, m_Core.nHabitats + 1 + iMPA).Behaviors.Add(m_ah)
                Next

                Me(i, Me.ColumnsCount - 2) = New PropertyCell(source, eVarNameFlags.EffectivePower)
                Me(i, Me.ColumnsCount - 1) = New PropertyCell(source, eVarNameFlags.SEmult)

            Next

        End Sub

#End Region

        Private Sub m_ahValueChanged(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim fleet As cEcospaceFleet = m_Core.EcospaceFleets(e.Position.Row)
            Dim col As Integer = e.Position.Column - 2
            Dim bChecked As Boolean = CBool(e.Cell.GetValue(e.Position))

            If col <= m_Core.nHabitats - 1 Then
                ' Core will prevent conflicts in assigning habitats
                fleet.HabitatFishery(col) = bChecked
            ElseIf col <= m_Core.nHabitats + m_Core.nMPAs - 1 Then
                fleet.MPAFishery(col - m_Core.nHabitats + 1) = bChecked
            End If

            UpdateRow(e.Position.Row)

        End Sub

        ''' <summary>
        ''' Update check boxes in a row
        ''' </summary>
        ''' <param name="i">Row number to update.</param>
        Private Sub UpdateRow(ByVal i As Integer)

            Dim source As cEcospaceFleet = m_Core.EcospaceFleets(i)

            'Fleet / habitat assignments
            For iHabitat As Integer = 0 To m_Core.nHabitats - 1
                Me(i, iHabitat + 2).Behaviors.Remove(m_ah)
                Me(i, iHabitat + 2).Value = CBool(source.HabitatFishery(iHabitat))
                Me(i, iHabitat + 2).Behaviors.Add(m_ah)
            Next

            For iMPA As Integer = 1 To m_Core.nMPAs
                Me(i, m_Core.nHabitats + 1 + iMPA).Behaviors.Remove(m_ah)
                Me(i, m_Core.nHabitats + 1 + iMPA).Value = CBool(source.MPAFishery(iMPA))
                Me(i, m_Core.nHabitats + 1 + iMPA).Behaviors.Add(m_ah)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSources() As EwECore.eMessageSource()
            Get
                Return New eMessageSource() {eMessageSource.EcoPath, eMessageSource.EcoSpace}
            End Get
        End Property

    End Class

End Namespace

