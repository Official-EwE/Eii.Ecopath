'==============================================================================
'
' $Log: ApplyEPEwEGrid.vb,v $
' Revision 1.2  2008/12/15 16:01:58  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.20  2008/08/11 16:13:55  jeroens
' Generalized EndEditHandler
'
' Revision 1.19  2008/06/02 00:07:45  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.18  2008/05/11 03:16:02  jeroens
' Standardized series of resource strings
'
' Revision 1.17  2008/04/07 02:31:05  jeroens
' Cleaning up resources
'
' Revision 1.16  2008/02/28 16:55:49  jeroens
' Fixed bug 382
'
' Revision 1.15  2008/02/19 13:08:05  jeroens
' Exposed column types enum for local hack in form
'
' Revision 1.14  2008/01/27 03:05:02  jeroens
' Fixed bug 341
' Fixed ID=0 / null test bug when refreshing grid
'
' Revision 1.13  2007/11/13 15:35:08  jeroens
' * Fixed bug 227
'
' Revision 1.12  2007/10/20 02:50:39  jeroens
' * ApplyEP form did not update to shape updates
'
' Revision 1.11  2007/10/15 01:31:10  jeroens
' * Fixed bug 293
'
' Revision 1.10  2007/10/15 00:48:26  jeroens
' * FF display ID, ICoreInputOutput display Index on numbered labels
'
' Revision 1.9  2007/10/04 14:06:52  jeroens
' * Fixed bug 185
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports SourceGrid2

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class ApplyEPEwEGrid
        Inherits EwEGrid

        Friend Enum eColumnTypes As Integer
            Index = 0
            Name
            Shape
        End Enum

        Private m_Core As cCore = Nothing
        Private m_EPManager As cEggProductionManager = Nothing
        Private m_astrShapes() As String = Nothing
        Private m_ceCellClick As New BehaviorModels.CustomEvents
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

        Public Sub New()

            MyBase.New()
            m_Core = cCore.GetInstance()
            m_EPManager = m_Core.EggProdShapeManager

        End Sub

        Public Sub ResetData()

            Dim cnt As Integer = Me.RowsCount
            If cnt > 1 Then
                Me.Rows.RemoveRange(1, cnt - 1)
            End If
            Me.FillData()

        End Sub

        Public Function GetEPShapeNames() As String()

            Dim astrShapeNames As New List(Of String)

            ' Add empty string as first item
            ' JS26Jan08: SourceGrid will refuse to cancel a combo edit operation when the text box part is empty.
            '            By providing an 'empty' value of " " (instead of "") the text box is never empty, and 
            '            sourcegrid will thus allow cancellation of edit operations on an empty value. Sheesh...
            '            There must be a better way to do this!
            astrShapeNames.Add(" ")
            If m_EPManager.Count > 0 Then

                For Each shapeFunc As cForcingFunction In m_EPManager
                    Dim tmpStr As String = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (shapeFunc.ID + 1), shapeFunc.Name)
                    astrShapeNames.Add(tmpStr)
                Next

            End If

            Return astrShapeNames.ToArray()

        End Function

        Public Sub SelectShapeName(ByVal strName As String)

            Dim r As Range = Me.Selection.GetRange()
            Dim iRow As Integer = 1
            Dim iShape As Integer = -1
            Dim pair As cGroupShapePair = Nothing
            Dim iID As Integer = 0

            If r.IsEmpty Then Return
            If (r.ContainsColumn(eColumnTypes.Shape) = False) Then Return

            ' Resolve shape index
            For iShapeTest As Integer = 0 To Me.m_astrShapes.Length - 1
                If m_astrShapes(iShapeTest) = strName Then iShape = iShapeTest - 1 : Exit For
            Next

            Try

                For iRow = r.Start.Row To r.End.Row
                    If r.Contains(New Position(iRow, eColumnTypes.Shape)) Then
                        pair = DirectCast(Me(iRow, eColumnTypes.Shape).Tag, cGroupShapePair)
                        If pair IsNot Nothing Then
                            If (iShape = -1) Then
                                pair.ShapeID = cCore.NULL_VALUE
                            Else
                                iID = Me.m_EPManager(iShape).ID
                                If pair.ShapeID <> iID Then
                                    pair.ShapeID = iID
                                End If
                            End If
                            Me(iRow, eColumnTypes.Shape).Value = strName
                        End If
                    End If
                Next iRow

            Catch ex As Exception

            End Try

        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Dock = DockStyle.Fill

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.Shape) = New EwEColumnHeaderCell(My.Resources.HEADER_SHAPE)

        End Sub

        Protected Overrides Sub FillData()

            Dim cmb As Cells.Real.ComboBox = Nothing
            Dim pair As cGroupShapePair = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = 1

            m_astrShapes = GetEPShapeNames()

            For Each pair In m_EPManager.GroupShapeList
                Me.Rows.Insert(iRow)
                sg = m_Core.StanzaGroups(pair.iStanzaGroup)

                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(iRow)
                Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(sg.Name)

                ' Combo box with strings, no text box
                If pair.ShapeID < 0 Then
                    cmb = New Cells.Real.ComboBox(m_astrShapes(0), GetType(String), m_astrShapes, True)
                Else
                    ' JS bug 293: shape names are 1-based
                    cmb = New Cells.Real.ComboBox(m_astrShapes(pair.ShapeID + 1), GetType(String), m_astrShapes, True)
                End If
                cmb.DataModel.AllowStringConversion = False
                cmb.EditableMode = EditableMode.SingleClick

                Me(iRow, eColumnTypes.Shape) = cmb
                Me(iRow, eColumnTypes.Shape).Tag = pair
                Me(iRow, eColumnTypes.Shape).Behaviors.Add(m_bm)

                iRow += 1
            Next

        End Sub

        Protected Overrides Sub FinishStyle()

            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.Columns(eColumnTypes.Shape).Width = 200

        End Sub

        Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

            Dim iRow As Integer = p.Row
            Dim iCol As Integer = p.Column
            Dim pair As cGroupShapePair = Nothing
            Dim strValue As String = ""
            Dim iID As Integer = cCore.NULL_VALUE

            ' Ignore header row
            If (iRow = 0) Then Return False
            ' Ignore non-combo changes
            If (iCol <> eColumnTypes.Shape) Then Return False

            ' Get pair
            If Me(iRow, eColumnTypes.Shape).Tag IsNot Nothing Then
                If TypeOf Me(iRow, eColumnTypes.Shape).Tag Is cGroupShapePair Then
                    pair = CType(Me(iRow, eColumnTypes.Shape).Tag, cGroupShapePair)
                End If
            End If

            ' Hahaha
            If pair Is Nothing Then Return False

            ' Get value
            strValue = CStr(cell.GetValue(p))
            ' Assume the worst...
            iID = cCore.NULL_VALUE

            ' Cell value not empty?
            If Not String.IsNullOrEmpty(strValue) Then
                ' #Yes: find shape
                For Each shapeFunc As cForcingFunction In m_EPManager
                    Dim tmpStr As String = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (shapeFunc.ID + 1), shapeFunc.Name)
                    If tmpStr = strValue Then
                        ' Shape manager needs position in list, not shape index!
                        iID = shapeFunc.ID
                    End If
                Next
            End If

            ' Need to change?
            If (pair.ShapeID <> iID) Then
                ' Update
                pair.ShapeID = iID
                Me(iRow, eColumnTypes.Shape).Value = strValue
            End If

            Return True

        End Function

    End Class

End Namespace


