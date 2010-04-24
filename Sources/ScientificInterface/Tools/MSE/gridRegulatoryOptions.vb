#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports EwECore.MSE

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

        'WARNING-INFO 8-march-2010
        'MaxEffort and OptionEffort have been removed from the grid because they are not implemented by MSE at this time
        'the code has been left in place to make them easier to implement them in the future.

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            ' MaxEffort
            OptionNotUsed
            'OptionEffort
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

            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            ' Me(0, eColumnTypes.MaxEffort) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXEFFORT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.OptionNotUsed) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_NOTUSED)
            '  Me(0, eColumnTypes.OptionEffort) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_EFFORT)
            Me(0, eColumnTypes.OptionWeakest) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_WEAKESTSTOCK)
            Me(0, eColumnTypes.OptionStrongest) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_STRONGESTSTOCK)
            Me(0, eColumnTypes.OptionSelective) = New EwEColumnHeaderCell(My.Resources.HEADER_QUOTA_SELECTIVEFISHING)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True

        End Sub

        Protected Overrides Sub FillData()

            Dim fleetMSE As cMSEFleetInput = Nothing

            ' For each flt
            For iFleet As Integer = 1 To Core.nFleets

                'Get the flt info
                fleetMSE = Core.MSEManager.FleetInputs(iFleet)

                Me.AddRow()

                Me(iFleet, eColumnTypes.Index) = New EwERowHeaderCell(iFleet)
                Me(iFleet, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, fleetMSE, eVarNameFlags.Name)

                Me(iFleet, eColumnTypes.OptionNotUsed) = New SourceGrid2.Cells.Real.CheckBox(True)
                Me(iFleet, eColumnTypes.OptionNotUsed).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionWeakest) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionWeakest).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionStrongest) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionStrongest).Behaviors.Add(m_bm)

                Me(iFleet, eColumnTypes.OptionSelective) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iFleet, eColumnTypes.OptionSelective).Behaviors.Add(m_bm)

                Me.Rows(iFleet).Tag = fleetMSE

                Me.UpdateRow(iFleet)

            Next iFleet

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
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

            Dim flt As cMSEFleetInput = Nothing
            Dim ri As RowInfo = Nothing

            ri = Me.Rows(iRow)
            flt = DirectCast(ri.Tag, cMSEFleetInput)

            Me.AllowUpdates = False

            ' Set option checks
            Me(iRow, eColumnTypes.OptionNotUsed).Value = (flt.QuotaType = eQuotaTypes.NotUsed)
            Me(iRow, eColumnTypes.OptionWeakest).Value = (flt.QuotaType = eQuotaTypes.Weakest)
            Me(iRow, eColumnTypes.OptionStrongest).Value = (flt.QuotaType = eQuotaTypes.HighestValue)
            Me(iRow, eColumnTypes.OptionSelective).Value = (flt.QuotaType = eQuotaTypes.Selective)

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

            Dim flt As cMSEFleetInput = Nothing
            Dim ri As RowInfo = Nothing

            ri = Me.Rows(p.Row)
            flt = DirectCast(ri.Tag, cMSEFleetInput)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.OptionNotUsed
                    flt.QuotaType = eQuotaTypes.NotUsed
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionSelective
                    flt.QuotaType = eQuotaTypes.Selective
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionStrongest
                    flt.QuotaType = eQuotaTypes.HighestValue
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.OptionWeakest
                    flt.QuotaType = eQuotaTypes.Weakest
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
