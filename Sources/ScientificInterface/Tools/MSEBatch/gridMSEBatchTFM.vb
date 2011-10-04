#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SourceGrid2.Cells.Real

#End Region

<CLSCompliant(False)> _
Public Class gridMSEBatchTFM
    Inherits EwEGrid


    Private m_iter As Integer

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        BLimLow
        BLimValue
        BLimUp
        BBaseLow
        BBaseValue
        BBaseUp
        FOptLow
        FOptValue
        FOptUp
    End Enum

#End Region ' Internal defs


    Public Sub New()
        MyBase.new()
        Me.iCurIter = 1
    End Sub

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

        Me.Redim(1, iNumCols)

        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.BLimLow) = New EwEColumnHeaderCell("Min Biomass lower limit")
        Me(0, eColumnTypes.BLimValue) = New EwEColumnHeaderCell("Min Biomass value")
        Me(0, eColumnTypes.BLimUp) = New EwEColumnHeaderCell("Min Biomass upper limit")

        Me(0, eColumnTypes.BBaseLow) = New EwEColumnHeaderCell("Base Biomass lower limit")
        Me(0, eColumnTypes.BBaseValue) = New EwEColumnHeaderCell("Base Biomass value")
        Me(0, eColumnTypes.BBaseUp) = New EwEColumnHeaderCell("Base Biomass upper limit")

        Me(0, eColumnTypes.FOptLow) = New EwEColumnHeaderCell("F lower limit")
        Me(0, eColumnTypes.FOptValue) = New EwEColumnHeaderCell("F value")
        Me(0, eColumnTypes.FOptUp) = New EwEColumnHeaderCell("F upper limit")


        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim group As MSE.cMSETFMGroup = Nothing

        ' For each group
        For iGroup As Integer = 1 To Core.nLivingGroups

            'Get the group info
            group = Core.MSEBatchManager.TFMGroups(iGroup)

            Me.AddRow()

            Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(CStr(iGroup))
            Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

            Me(iGroup, eColumnTypes.BLimLow) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBLimLower)
            Me(iGroup, eColumnTypes.BLimValue) = New EwECell(group.BLimValue(iCurIter), GetType(Integer))
            Me(iGroup, eColumnTypes.BLimUp) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBLimUpper)

            Me(iGroup, eColumnTypes.BBaseLow) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBBaseLower)
            Me(iGroup, eColumnTypes.BBaseValue) = New EwECell(group.BBaseValue(iCurIter), GetType(Integer))
            Me(iGroup, eColumnTypes.BBaseUp) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBBaseUpper)

            Me(iGroup, eColumnTypes.FOptLow) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMFOptLower)
            Me(iGroup, eColumnTypes.FOptValue) = New EwECell(group.BBaseValue(iCurIter), GetType(Integer))
            Me(iGroup, eColumnTypes.FOptUp) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMFOptUpper)


            Me.Rows(iGroup).Tag = group

        Next iGroup

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Selection.SelectionMode = GridSelectionMode.Row
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.EcoSim
        End Get
    End Property


    Public Property iCurIter As Integer
        Get
            Return m_iter
        End Get
        Set(value As Integer)
            m_iter = value
        End Set
    End Property




    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Called when the user has finished editing a cell. Handled to update 
    ''' local admin based on cell value changes.
    ''' </summary>
    ''' <returns>
    ''' True if the edit operation is allowed, False to cancel the edit operation.
    ''' </returns>
    ''' <remarks>
    ''' This method differs from OnCellValueChanged; at the end of an edit
    ''' operation it is once again safe to alter the value of the cell that was
    ''' just edited for text and combo box controls. *sigh*
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean



        Dim val As Object = Me(p.Row, p.Column).Value

        'Select Case DirectCast(p.Column, eColumnTypes)
        '    Case eColumnTypes.Name : ti.Common = CStr(val)
        '    Case eColumnTypes.Class : ti.Class = CStr(val)
        '    Case eColumnTypes.Family : ti.Family = CStr(val)
        '    Case eColumnTypes.Order : ti.Order = CStr(val)
        '    Case eColumnTypes.Genus : ti.Genus = CStr(val)
        '    Case eColumnTypes.Species : ti.Species = CStr(val)
        '    Case eColumnTypes.Phylum : ti.Phylum = CStr(val)
        '    Case eColumnTypes.Proportion : ti.Proportion = CSng(val)
        '    Case eColumnTypes.Code : ti.CodeTaxon = CStr(val)
        'End Select

        '' Perhaps redundant but hey
        'Me.UpdateRow(p.Row)

        Return True

    End Function


#End Region ' Overrides


End Class
