'==============================================================================
'
' $Log: gridRegulatoryOptions.vb,v $
' Revision 1.3  2009/01/16 18:30:43  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:55:35  jeroens
' no message
'
' Revision 1.1  2008/10/09 00:09:38  jeroens
' Renamed
'
' Revision 1.7  2008/10/08 22:33:56  jeroens
' Localizing
'
' Revision 1.6  2008/10/07 21:28:48  jeroens
' Localized
'
' Revision 1.5  2008/10/06 16:47:51  jeroens
' NotSet -> NotUsed
'
' Revision 1.4  2008/10/04 00:49:09  jeroens
' Connected v1
'
' Revision 1.3  2008/10/03 23:09:07  jeroens
' Hooked up MaxEffort
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
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridRegulatoryOptions
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            MaxEffort
            OptionNotSet
            OptionWeakest
            OptionStrongest
            OptionSelective
        End Enum

        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local MPA administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length
            Dim src As cCoreInputOutputBase = Nothing

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            Me(0, eColumnTypes.MaxEffort) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXEFFORT, StyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.OptionNotSet) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_EFFORT)
            Me(0, eColumnTypes.OptionWeakest) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_WEAKESTSTOCK)
            Me(0, eColumnTypes.OptionStrongest) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_STRONGESTSTOCK)
            Me(0, eColumnTypes.OptionSelective) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_SELECTIVEFISHING)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim fleet As cFleetInput = Nothing
            Dim reg As cEcosimFisheriesRegulation = Nothing

            ' For each fleet
            For iFleet As Integer = 1 To core.nFleets

                'Get the fleet info
                fleet = core.FleetInputs(iFleet)
                reg = core.EcosimFisheriesRegulations(iFleet)

                Me.AddRow()

                Me(iFleet, eColumnTypes.Index) = New EwERowHeaderCell(iFleet)
                Me(iFleet, eColumnTypes.Name) = New PropertyRowHeaderCell(fleet, eVarNameFlags.Name)
                Me(iFleet, eColumnTypes.MaxEffort) = New PropertyCell(reg, eVarNameFlags.MaxEffort)

                Me(iFleet, eColumnTypes.OptionNotSet) = New SourceGrid2.Cells.Real.CheckBox(True)
                Me(iFleet, eColumnTypes.OptionNotSet).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionWeakest) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionWeakest).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionStrongest) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionStrongest).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionSelective) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionSelective).Behaviors.Add(m_bm)

                Me.Rows(iFleet).Tag = reg

                Me.UpdateRow(iFleet)

            Next iFleet

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of the Row with the given index.
        ''' </summary>
        ''' <param name="iRow">The index of the row to refresh.</param>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateRow(ByVal iRow As Integer)

            Dim reg As cEcosimFisheriesRegulation = Nothing
            Dim ri As RowInfo = Nothing

            ri = Me.Rows(iRow)
            reg = DirectCast(ri.Tag, cEcosimFisheriesRegulation)

            Me.AllowUpdates = False

            ' Set option checks
            Me(iRow, eColumnTypes.OptionNotSet).Value = (reg.QuotaType = eQuotaTypes.NotUsed)
            Me(iRow, eColumnTypes.OptionWeakest).Value = (reg.QuotaType = eQuotaTypes.Weakest)
            Me(iRow, eColumnTypes.OptionStrongest).Value = (reg.QuotaType = eQuotaTypes.Strongest)
            Me(iRow, eColumnTypes.OptionSelective).Value = (reg.QuotaType = eQuotaTypes.Selective)

            Me.AllowUpdates = True

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Called Update local admin based on cell value changes.
        ''' </summary>
        ''' <returns>
        ''' True if the value change is allowed, False to block the value change.
        ''' </returns>
        ''' <remarks>
        ''' This method differs from OnCellValueEdited; during a cell value 
        ''' change notification (at the end of an edit operation) it is unsafe
        ''' to modify the value of the cell being edited. However, the end edit 
        ''' event will not be triggered for particular specialized cells which
        ''' makes this method mandatory.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

            If Not Me.AllowUpdates Then Return True

            Dim reg As cEcosimFisheriesRegulation = Nothing
            Dim ri As RowInfo = Nothing

            ri = Me.Rows(p.Row)
            reg = DirectCast(ri.Tag, cEcosimFisheriesRegulation)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.OptionNotSet
                    reg.QuotaType = eQuotaTypes.NotUsed
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionSelective
                    reg.QuotaType = eQuotaTypes.Selective
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionStrongest
                    reg.QuotaType = eQuotaTypes.Strongest
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionWeakest
                    reg.QuotaType = eQuotaTypes.Weakest
                    Me.UpdateRow(p.Row)

            End Select

            Return True

        End Function

#Region " Admin "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update lock, should be set when modifying cell values from the code
        ''' to prevent recursive update/notification loops.
        ''' </summary>
        ''' <returns>True when no update lock is active.</returns>
        ''' <remarks>
        ''' Update locks are cumulative: setting this lock twice will require 
        ''' clearing it twice to allow updates to happen.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Property AllowUpdates() As Boolean
            Get
                Return (Me.m_iUpdateLock = 0)
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Me.m_iUpdateLock += 1
                Else
                    Me.m_iUpdateLock -= 1
                End If
            End Set
        End Property

#End Region ' Admin

    End Class

End Namespace ' Ecosim
