'==============================================================================
'
' $Log: gridFishingQuotas.vb,v $
' Revision 1.7  2009/05/28 12:37:39  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.6  2009/05/21 18:53:45  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.5  2009/01/16 18:30:43  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.4  2008/12/15 15:55:35  jeroens
' no message
'
' Revision 1.3  2008/10/04 00:49:09  jeroens
' Connected v1
'
' Revision 1.2  2008/10/03 21:55:03  jeroens
' Mock-up improved
'
' Revision 1.1  2008/10/02 18:48:49  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridFishingQuotas
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
                    "{0} ({1})", cStyleGuide.eUnitType.Currency)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim reg As cEcosimFisheriesRegulation = Nothing
            Dim group As cCoreInputOutputBase = Nothing
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
                    reg = core.EcosimFisheriesRegulations(iFleet)
                    Me(iGroup, 1 + iFleet) = New PropertyCell(reg, eVarNameFlags.Quota, group)
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace ' Ecosim
