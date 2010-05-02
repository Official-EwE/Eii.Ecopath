Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' A <see cref="cProperty">cProperty</see>-driven cell that reflects the 
''' property value by varying the colour intensity of the cell background.
''' </summary>
''' <remarks>This is a Hack'n'slash solution; no value range testing is 
''' performed when calculating the background colour.</remarks>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class NichePropertyColourCell
    : Inherits PropertyCell

#Region " Private visualizer "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(False)> _
    Private Class NicePropertyColourCellVisualizer
        : Inherits EwECellVisualizerBase

        Protected Overrides Sub DrawCell_Background(ByVal p_Cell As SourceGrid2.Cells.ICellVirtual, ByVal p_CellPosition As SourceGrid2.Position, ByVal e As System.Windows.Forms.PaintEventArgs, ByVal p_ClientRectangle As System.Drawing.Rectangle, ByVal p_Status As SourceGrid2.DrawCellStatus)

            If (p_Status And SourceGrid2.DrawCellStatus.Selected) = 0 Then
                If (TypeOf p_Cell Is NichePropertyColourCell) Then
                    ' #Yes: obtain rich info
                    Dim cell As NichePropertyColourCell = DirectCast(p_Cell, NichePropertyColourCell)
                    ' Get the property
                    Dim prop As cProperty = cell.GetProperty()
                    ' Is this a property with a numerical value?
                    If TypeOf prop Is cSingleProperty Then
                        ' #Yes: get its value
                        Dim sValue As Single = CSng(prop.GetValue())
                        ' Calc back colour

                        ' Render back colour
                        Dim clrBack As Color = Color.FromArgb(255, CInt(255 * (1 - sValue)), 255, 255)
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
    Private Shared g_visualizer As New NicePropertyColourCellVisualizer()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> data source.</param>
    ''' <param name="VarName">The <see cref="eVarNameFlags">VarName flag</see> that defines which aspect of the Source to acces.</param>
    ''' <param name="SourceSec">An optional secundary index in the VarName, or Nothing when irrelevant.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal pm As cPropertyManager, _
                   ByVal Source As cCoreInputOutputBase, _
                   ByVal VarName As eVarNameFlags, _
                   Optional ByVal SourceSec As cCoreInputOutputBase = Nothing)
        MyBase.new(pm, Source, VarName, SourceSec)
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to suppress the cell from displaying any values when 
    ''' flagged as read-only.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property DisplayText() As String
        Get
            ' Is a read-only cell?
            If (Me.Style And cStyleGuide.eStyleFlags.NotEditable) = cStyleGuide.eStyleFlags.NotEditable Then
                ' #Yes: suppress any value output
                Return ""
            End If
            ' No read-only cell: show default cell value 
            Return MyBase.DisplayText()
        End Get
    End Property

End Class
