'==============================================================================
'
' $Log: ElectivityEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/08/02 03:04:11  jeroens
' Renamed resources
'
' Revision 1.16  2008/07/31 19:02:47  jeroens
' Fixed issue 526
'
' Revision 1.15  2008/07/29 13:06:43  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.14  2008/06/02 00:01:26  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.13  2008/05/29 22:22:39  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.12  2007/10/10 02:59:11  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.11  2007/07/03 07:08:45  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.10  2007/06/22 02:57:58  jeroens
' * Selection state of cell now considered when drawing background
'
' Revision 1.9  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.8  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.7  2006/09/29 21:17:48  sherman
' Changed Color and created Grid Cell Viewer for PredationMortalityGrid
'
' Revision 1.6  2006/09/21 01:00:24  jeroens
' * Updated to cCoreGroupBase
'
' Revision 1.5  2006/08/22 04:07:07  jeroens
' + Populated, including ugly cell colours
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class ElectivityEwEGrid
        : Inherits EwEGrid

#Region " Helper classes "

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' A <see cref="cProperty">cProperty</see>-driven cell that reflects the 
        ''' property value by varying the colour intensity of the cell background.
        ''' </summary>
        ''' <remarks>This is a Hack'n'slash solution; no value range testing is 
        ''' performed when calculating the background colour.</remarks>
        ''' ---------------------------------------------------------------------------
        Public Class ElectivityGridCell
            : Inherits PropertyCell

#Region " Private visualizer "

            ''' ---------------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' ---------------------------------------------------------------------------
            Private Class ElectivityGridCellVisualizer
                : Inherits EwECellVisualizerBase

                Protected Overrides Sub DrawCell_Background(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, _
                        ByVal p_CellPosition As SourceGrid2.Position, _
                        ByVal e As System.Windows.Forms.PaintEventArgs, _
                        ByVal p_ClientRectangle As System.Drawing.Rectangle, _
                        ByVal p_Status As SourceGrid2.DrawCellStatus)

                    If (p_Status = SourceGrid2.DrawCellStatus.Normal) Then

                        If (TypeOf p_Cell Is ElectivityGridCell) Then
                            ' #Yes: obtain rich info
                            Dim cell As ElectivityGridCell = DirectCast(p_Cell, ElectivityGridCell)
                            ' Get the property
                            Dim prop As cProperty = cell.GetProperty()
                            ' Is this a property with a numerical value?
                            If TypeOf prop Is cSingleProperty Then
                                ' #Yes: get its value
                                Dim sValue As Single = CSng(prop.GetValue())
                                ' Calc back colour
                                Dim rgbColor As Integer = CInt(Math.Max(0, Math.Min(255, 255 * (1 - (1 + sValue) / 4))))
                                Dim clrBack As Color = Color.FromArgb(255, 255, rgbColor, rgbColor)
                                ' Render back colour
                                ' Draw the background
                                Using br As New SolidBrush(clrBack)
                                    e.Graphics.FillRectangle(br, p_ClientRectangle)
                                End Using
                                ' Done
                                Return
                            End If
                        End If
                    End If

                    ' Rever to default
                    MyBase.DrawCell_Background(p_Cell, p_CellPosition, e, p_ClientRectangle, p_Status)
                End Sub
            End Class

#End Region ' Private visualizer

            ''' <summary>Default visualizer for EwECells.</summary>
            Private Shared g_visualizer As New ElectivityGridCellVisualizer()

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Constructor.
            ''' </summary>
            ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source.</param>
            ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces.</param>
            ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                    Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
                MyBase.new(Source, VarName, SourceSec)
                ' Set shared visualizer
                Me.VisualModel = g_visualizer
            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Constructor.
            ''' </summary>
            ''' <param name="prop">The property to assign to the cell.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal prop As cProperty)
                ' Call baseclass constructor
                MyBase.New(prop)
                ' Set shared visualizer
                Me.VisualModel = g_visualizer
            End Sub

        End Class

#End Region ' Helper classes

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            Me.Redim(core.nGroups + 1, 2)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To core.nGroups
                ' Column displays mixed consumer/producer groups ( PP < 1)
                source = core.EcoPathGroupOutputs(i)
                ' Group index header cell
                Me(i, 0) = New EwERowHeaderCell(i)
                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If

            Next

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim cell As PropertyCell = Nothing
            Dim columnIndex As Integer = 2

            ' For each column
            For groupIndex As Integer = 1 To core.nGroups
                ' Get the group
                source = core.EcoPathGroupOutputs(groupIndex)
                If source.PP < 1 Then
                    ' For each row
                    For rowIndex As Integer = 1 To core.nGroups
                        ' Get index group
                        sourceSec = core.EcoPathGroupOutputs(rowIndex)
                        ' Create cell
                        cell = New ElectivityGridCell(source, eVarNameFlags.Alpha, sourceSec)
                        ' Cells suppress zeroes to increase legibility of the grid
                        cell.SuppressZero(-1) = True
                        ' Activate the cell
                        Me(rowIndex, columnIndex) = cell
                    Next rowIndex
                    columnIndex = columnIndex + 1
                End If
            Next groupIndex

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace
