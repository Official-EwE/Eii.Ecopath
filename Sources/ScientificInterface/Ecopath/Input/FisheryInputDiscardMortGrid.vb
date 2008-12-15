'==============================================================================
'
' $Log: FisheryInputDiscardMortGrid.vb,v $
' Revision 1.2  2008/12/15 15:53:39  jeroens
' no message
'
' Revision 1.1  2008/10/08 17:57:17  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class FisheryInputDiscardMortGrid
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
        Dim group As cCoreInputOutputBase = Nothing
        Dim fleet As cFleetInput = Nothing
        Dim cell As ICell = Nothing

        ' For each group
        For iGroup As Integer = 1 To core.nGroups

            Me.AddRow()

            'Get the group info
            group = core.EcoPathGroupInputs(iGroup)

            ' Fleet name As row header
            Me(iGroup, 0) = New EwERowHeaderCell(iGroup)
            Me(iGroup, 1) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)

            ' Fleet cells
            For iFleet As Integer = 1 To core.nFleets
                fleet = core.FleetInputs(iFleet)
                Me(iGroup, 1 + iFleet) = New PropertyCell(fleet, eVarNameFlags.DiscardMortality, group)
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
        Get
            Return eMessageSource.EcoPath
        End Get
    End Property

End Class

