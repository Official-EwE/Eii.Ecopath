'==============================================================================
'
' $Log: DetritusFateEwEGrid.vb,v $
' Revision 1.5  2009/05/28 12:36:59  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.4  2009/05/21 19:27:15  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:09  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:54:29  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.28  2008/08/02 03:04:12  jeroens
' Renamed resources
'
' Revision 1.27  2008/07/29 13:06:44  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.26  2008/06/02 00:01:28  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.25  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.24  2008/05/18 01:13:06  jeroens
' Nitty-gritty
'
' Revision 1.23  2008/05/13 18:48:05  jeroens
' Fixed bug 466
'
' Revision 1.22  2008/04/07 02:31:08  jeroens
' Cleaning up resources
'
' Revision 1.21  2008/01/31 17:08:23  jeroens
' Made fleet column headers live updating
'
' Revision 1.20  2008/01/24 14:45:22  jeroens
' Fixed detritus group selection description
'
' Revision 1.19  2008/01/11 12:33:19  jeroens
' Fixed bug 299
'
' Revision 1.18  2007/10/10 02:59:13  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.17  2007/07/06 20:11:17  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.16  2007/07/03 07:08:47  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.15  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.14  2007/06/05 02:45:49  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.13  2007/05/31 13:11:21  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.12  2007/04/29 03:45:11  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.11  2007/03/21 19:12:15  joeh
' *Implement stanza hierarchy
'
' Revision 1.10  2006/08/15 15:40:29  jeroens
' * Fixed spelling error
'
' Revision 1.9  2006/06/28 13:59:22  jeroens
' * Renamed iGroup member vars, properties to Index
' * Renamed GroupName vartype and usage to Name where applicable
' * Merged usage of varName Name (fleet) with GroupName
'
' Revision 1.8  2006/06/20 22:55:48  fgao
' Grids update
'
' Revision 1.7  2006/06/16 03:52:05  cvsuser
' + JS: Sum cell now represented by simple cSingleProperty rather than top-heavy cFormulaProperty
'
' Revision 1.6  2006/06/15 02:28:28  cvsuser
' + JS: Sum(1) column populated via FormulaProperty to gain consistent colour and decimal display feedback
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class DetritusFateEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            'Me.Redim(core.nGroups + 1, 3 + core.nDetritusGroups)
            Me.Redim(core.nGroups + 1, 4 + core.nDetritusGroups)

            'Header cell (0,0) Source \ fate
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_SOURCEFATE)

            ' Detritus column header cells
            For i As Integer = 1 To core.nDetritusGroups
                source = core.EcoPathGroupInputs(core.nGroups - core.nDetritusGroups + i)
                Me(0, i + 1) = New PropertyColumnHeaderCell(source, eVarNameFlags.Name)
            Next

            ' The export header cell
            Me(0, core.nDetritusGroups + 2) = New EwEColumnHeaderCell(My.Resources.GENERIC_HEADER_EXPORT)
            ' The sum header cell
            Me(0, core.nDetritusGroups + 3) = New EwEColumnHeaderCell(My.Resources.HEADER_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim prop As cProperty = Nothing
            Dim propSum As cSingleProperty = Nothing
            Dim propExport As cFormulaProperty = Nothing

            Dim alProp As New ArrayList()
            Dim propSumAll As cFormulaProperty = Nothing
            Dim opSumAll As cMultiOperation = Nothing
            Dim opMinus As cBinaryOperation = Nothing

            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim blnStanza(core.nGroups) As Boolean
            Dim aiStanza(core.nGroups) As Integer 'Hold the stanza group number
            Dim iStanzaPrev As Integer = -1

            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nGroups : aiStanza(i) = -1 : Next

            'Remove existing rows
            Me.RowsCount = 1

            'Tag stanza group first
            For iStanzaGroup As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(iStanzaGroup)
                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    blnStanza(source.Index) = True
                    aiStanza(source.Index) = iStanzaGroup
                Next
            Next

            ' Configure static SUM prop
            propSum = New cSingleProperty("")
            propSum.SetValue(1.0)
            propSum.SetStyle(cStyleGuide.eStyleFlags.Sum Or cStyleGuide.eStyleFlags.NotEditable)

            'Create rows for all groups
            For rowIndex As Integer = 1 To core.nGroups

                source = core.EcoPathGroupInputs(rowIndex)
                alProp.Clear()

                If aiStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    For iCol As Integer = 1 To core.nDetritusGroups

                        sourceSec = core.EcoPathGroupInputs(core.nGroups - core.nDetritusGroups + iCol)

                        Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                        Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                        prop = pm.GetProperty(source, eVarNameFlags.DetritusFate, sourceSec, True, core.nGroups - core.nDetritusGroups)
                        Me(iRow, iCol + 1) = New PropertyCell(prop)
                        alProp.Add(prop)
                    Next

                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alProp.ToArray)
                    propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
                    opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, _
                                                CType(propSum, Object), CType(propSumAll, Object))
                    propExport = New cFormulaProperty(CType(opMinus, cExpression))

                    ' Export column 
                    Me(iRow, Me.ColumnsCount - 2) = New PropertyCell(CType(propExport, cProperty))

                    ' JS 140606: Use static single property here. Seems overkill where a simple Cell(1.0) would have
                    '            been sufficient, but this way the cell inherits StyleGuide colour and decimals feedback.
                    Me(iRow, Me.ColumnsCount - 1) = New PropertyCell(propSum)
                Else 'Group is stanza
                    sg = core.StanzaGroups(aiStanza(source.Index))
                    If aiStanza(source.Index) <> iStanzaPrev Then 'If stanza group appears the first time Then display + control
                        iRow = Me.AddRow()
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        'Complete row with dummy cells
                        For i As Integer = 2 To 4 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        iStanzaPrev = aiStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow()
                    hgcStanza.AddChildRow(iRow)
                    For iCol As Integer = 1 To core.nDetritusGroups

                        sourceSec = core.EcoPathGroupInputs(core.nGroups - core.nDetritusGroups + iCol)

                        Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                        Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
                        prop = pm.GetProperty(source, eVarNameFlags.DetritusFate, sourceSec, True, core.nGroups - core.nDetritusGroups)
                        Me(iRow, iCol + 1) = New PropertyCell(prop)
                        alProp.Add(prop)
                    Next

                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alProp.ToArray)
                    propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
                    opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, _
                                                CType(propSum, Object), CType(propSumAll, Object))
                    propExport = New cFormulaProperty(CType(opMinus, cExpression))

                    ' Export column 
                    Me(iRow, Me.ColumnsCount - 2) = New PropertyCell(CType(propExport, cProperty))
                    Me(iRow, Me.ColumnsCount - 1) = New PropertyCell(propSum)
                End If
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
