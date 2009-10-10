#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class PSDContributionResult
        : Inherits EwEGrid

        Private m_core As cCore = Nothing
        Private m_frm As Form = Nothing

        Public Sub New()
            MyBase.new()

            m_core = cCore.GetInstance
            'Don't manually run! The core execution states take care of this!
            'm_core.RunPSD(IsGroupSelected)
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            'Define grid dimensions
            Dim parms As cPSDParameters = m_core.ParticleSizeDistributionParameters
            Me.Redim(1, m_core.nWeightClasses + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAMEWEIGHT_UNIT)

            ' Dynamic column header - weight class
            For wtClassIndex As Integer = 1 To m_core.nWeightClasses
                Me(0, wtClassIndex + 1) = New EwEColumnHeaderCell((parms.FirstWeightClass * 2 ^ (wtClassIndex - 1)).ToString)
            Next

            ' Sum value column
            Me(0, m_core.nWeightClasses + 2) = New EwEColumnHeaderCell(My.Resources.HEADER_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim groupOutput As cEcoPathGroupOutput = Nothing
            Dim iRow As Integer = -1

            ' Remove existing rows
            Me.RowsCount = 1

            ' Done?
            'If core.nWeightClasses = 0 Then Return

            ' Create rows for groups and sum values in each row
            For iGroup As Integer = 1 To m_core.nLivingGroups
                If IsGroupSelected(iGroup) Then
                    groupOutput = m_core.EcoPathGroupOutputs(iGroup)
                    iRow = Me.AddRow()
                    FillRows(iRow, groupOutput)
                End If
            Next iGroup

            'Create "Sum" row (sum values in each column)
            FillTotalValueRow()

           End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cCoreGroupBase)

            Dim sValue As Single = 0.0!
            Dim sTotal As Single = 0.0!
            Dim cell As EwECell = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            ' For each weight class (each column) 
            For wtClassIndex As Integer = 1 To m_core.nWeightClasses
                sValue = CSng(source.GetVariable(eVarNameFlags.PSD, wtClassIndex))
                cell = New EwECell(sValue, GetType(Single))
                cell.SuppressZero = True
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, wtClassIndex + 1) = cell

                'Sum values in a row
                sTotal = sTotal + sValue 'sTotal += sValue
            Next

            'Display the sum of quantities in a row
            cell = New EwECell(sTotal, GetType(Single))
            cell.SuppressZero = True
            cell.Style = cStyleGuide.eStyleFlags.Sum
            Me(iRow, Me.ColumnsCount - 1) = cell
        End Sub

        Private Sub FillTotalValueRow()

            Dim iRow As Integer
            Dim source As cCoreGroupBase = Nothing
            Dim sValue As Single = 0.0!
            Dim sTotal(m_core.nWeightClasses) As Single
            Dim sSumTotal As Single = 0.0!
            Dim cell As EwECell = Nothing

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                sTotal(iWtClass) = 0.0!
            Next

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell(My.Resources.HEADER_SUM)
            For iGroup As Integer = 1 To m_core.nLivingGroups
                If IsGroupSelected(iGroup) Then
                    source = m_core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To m_core.nWeightClasses
                        sValue = CSng(source.GetVariable(eVarNameFlags.PSD, iWtClass))
                        sTotal(iWtClass) = STotal(iWtClass) + sValue
                    Next
                End If
            Next

            'Display the sum of values in a column
            For iWtClass As Integer = 1 To m_core.nWeightClasses
                cell = New EwECell(sTotal(iWtClass), GetType(Single))
                cell.SuppressZero = True
                cell.Style = cStyleGuide.eStyleFlags.Sum
                Me(Me.RowsCount - 1, iWtClass + 1) = cell
            Next

            'Display the sum of all values
            For iWtClass As Integer = 1 To m_core.nWeightClasses
                sSumTotal = sSumTotal + sTotal(iWtClass)
            Next
            cell = New EwECell(sSumTotal, GetType(Single))
            cell.SuppressZero = True
            cell.Style = cStyleGuide.eStyleFlags.Sum
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = cell

        End Sub

        Private Function IsGroupSelected() As Boolean()
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim bGroupSelected(m_core.nLivingGroups) As Boolean

            For i As Integer = 1 To m_core.nLivingGroups
                bGroupSelected(i) = sg.GroupVisible(i)
            Next
            Return bGroupSelected
        End Function

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

        'Private Sub OnFormShown(ByVal sender As Object, ByVal e As System.EventArgs)
        '    Dim parms As cPSDParameters = Nothing

        '    parms = Me.m_core.ParticleSizeDistributionParameters
        '    If parms.PSDEnabled = False Then m_frm.Close()
        'End Sub

    End Class

End Namespace
