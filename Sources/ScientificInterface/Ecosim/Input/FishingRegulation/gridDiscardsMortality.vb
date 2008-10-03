'==============================================================================
'
' $Log: gridDiscardsMortality.vb,v $
' Revision 1.2  2008/10/03 21:55:03  jeroens
' Mock-up improved
'
' Revision 1.1  2008/10/02 18:48:49  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports directive

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridDiscardsMortality
        Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim src As cCoreInputOutputBase = Nothing

            Me.Redim(1, 2 + core.nFleets)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            For iFleet As Integer = 1 To core.nFleets
                src = core.FleetInputs(iFleet)
                Me(0, 1 + iFleet) = New PropertyColumnHeaderCell(src, _
                    eVarNameFlags.Name, Nothing, _
                    "{0} ({1})", StyleGuide.eUnitType.Currency)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim fleet As cCoreInputOutputBase = Nothing
            Dim group As cCoreInputOutputBase = Nothing
            Dim cell As ICell = Nothing

            ' For each group
            For iRow As Integer = 1 To core.nGroups

                Me.AddRow()

                'Get the group info
                group = core.EcoPathGroupInputs(iRow)

                ' Fleet name As row header
                Me(iRow, 0) = New EwERowHeaderCell(iRow)
                Me(iRow, 1) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To core.nFleets
                    'assigned it to destined cell
                    Me(iRow, 1 + iFleet) = New SourceGrid2.Cells.Real.Cell(0.3!, GetType(Single))
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoSim
            End Get
        End Property

    End Class

End Namespace ' Ecosim
