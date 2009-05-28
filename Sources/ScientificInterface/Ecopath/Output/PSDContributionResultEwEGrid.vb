' =============================================================================
'
' $Log: PSDContributionResultEwEGrid.vb,v $
' Revision 1.13  2009/05/28 12:36:57  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.12  2009/05/21 18:53:46  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.11  2009/04/28 00:24:17  joeh
' Add handling if PSDEnabled is false
'
' Revision 1.10  2009/04/02 16:24:54  jeroens
' PSD run integrated w Ecopath
'
' Revision 1.9  2009/04/02 01:47:44  joeh
' Pass GroupSelected boolean array to cCore.RunPSD and psdModel.Run
'
' Revision 1.8  2009/04/01 15:21:17  joeh
' Call core.RunPSD() in the Constructor
'
' Revision 1.7  2009/03/18 13:32:05  jeroens
' Uses implemented PSD classes
'
' Revision 1.6  2009/03/17 23:37:34  joeh
' Add codes for the Selected Group feature
'
' Revision 1.5  2009/03/13 22:52:37  joeh
' Add code to sum the PSD values of a row and to sum the PSD values in a column
'
' Revision 1.4  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
' Revision 1.3  2009/03/11 00:14:28  joeh
' Add PSD calculation
'
' Revision 1.2  2009/02/21 00:23:07  jeroens
' Added headers
'
' =============================================================================

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

            m_frm = CType(Me.Parent, Form)
            AddHandler m_frm.Shown, AddressOf OnFormShown

        End Sub

        Protected Overrides Sub FillData()

            Dim groupOutput As cEcoPathGroupOutput = Nothing
            Dim iRow As Integer = -1
            Dim parms As cPSDParameters = Nothing
            Dim str As String = ""
            Dim msg As cMessage = Nothing

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

            parms = Me.m_core.ParticleSizeDistributionParameters
            If parms.PSDEnabled = False Then
                str = My.Resources.PSD_MSG_PSDDISABLED
                msg = New cMessage(str, eMessageType.TooManyMissingParameters, eCoreComponentType.EcoPath, eMessageImportance.Warning)
                Me.m_core.Messages.SendMessage(msg)
            End If
        End Sub

        Private Sub FillRows(ByVal iRow As Integer, ByVal source As cCoreGroupBase)

            Dim sourceSec As cCoreGroupBase = Nothing
            Dim propManager As cPropertyManager = cPropertyManager.GetInstance()
            Dim propPSD As cProperty = Nothing
            Dim propCell As PropertyCell = Nothing

            Dim alSumRow As ArrayList = New ArrayList
            Dim opSumRow As cMultiOperation = Nothing
            Dim propSumRow As cFormulaProperty = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

            alSumRow.Clear()
            ' For each weight class (each column) 
            For wtClassIndex As Integer = 1 To m_core.nWeightClasses
                sourceSec = m_core.EcoPathGroupOutputs(wtClassIndex)
                propPSD = propManager.GetProperty(source, eVarNameFlags.PSD, sourceSec)
                propCell = New PropertyCell(CType(propPSD, cProperty))
                ' Configure the cell
                propCell.SuppressZero = True
                ' Set the cell
                Me(iRow, wtClassIndex + 1) = propCell

                'Sum values in a row
                alSumRow.Add(propPSD)
            Next

            'Display the sum of quantities in a row
            opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
            propSumRow = New cFormulaProperty(CType(opSumRow, cExpression))
            propCell = New PropertyCell(CType(propSumRow, cProperty))
            propCell.SuppressZero = True
            Me(iRow, Me.ColumnsCount - 1) = propCell
        End Sub

        Private Sub FillTotalValueRow()

            Dim groupInput As cEcoPathGroupInput = Nothing
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim pmPropertyManager As cPropertyManager = cPropertyManager.GetInstance()
            Dim propPSD As cProperty = Nothing
            Dim iRow As Integer
            '    Dim propMarketPrice As cProperty = Nothing
            '    Dim alProdLandingsMarketPrice As ArrayList = New ArrayList()
            '    Dim opProdLandingsMarketPrice As cMultiOperation = Nothing
            '    Dim propProdLandingsMarketPrice As cFormulaProperty = Nothing

            Dim alSumCol As New ArrayList()
            Dim opSumCol As cMultiOperation = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            Dim propCell As PropertyCell = Nothing

            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell("")
            Me(iRow, 1) = New EwERowHeaderCell("Sum") 'My.Resources.HEADER_TOTALVALUE, StyleGuide.eUnitType.Monetary)

            alSumAll.Clear()
            For wtClassIndex As Integer = 1 To m_core.nWeightClasses
                source = m_core.EcoPathGroupOutputs(wtClassIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To m_core.nLivingGroups
                    If IsGroupSelected(rowIndex) Then
                        sourceSec = m_core.EcoPathGroupOutputs(rowIndex)
                        ' Get the index PSD property
                        propPSD = pmPropertyManager.GetProperty(sourceSec, eVarNameFlags.PSD, source)

                        'Sum values in a column
                        alSumCol.Add(propPSD)

                        'Sum all values
                        alSumAll.Add(propPSD)
                    End If
                Next

                'Display the sum of values in a column
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
                propCell = New PropertyCell(CType(propSumCol, cProperty))
                propCell.SuppressZero = True
                Me(Me.RowsCount - 1, wtClassIndex + 1) = propCell
            Next

            'Display the sum of all values
            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
            propCell = New PropertyCell(CType(propSumAll, cProperty))
            propCell.SuppressZero = True
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = propCell

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

        Private Sub OnFormShown(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim parms As cPSDParameters = Nothing

            parms = Me.m_core.ParticleSizeDistributionParameters
            If parms.PSDEnabled = False Then m_frm.Close()
        End Sub

    End Class

End Namespace
