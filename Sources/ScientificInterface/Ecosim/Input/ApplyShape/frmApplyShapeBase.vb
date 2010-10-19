#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Forms

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form baseclass for implementing an Ecosim 'Apply Forcing' or 'Apply 
    ''' Mediation' interface.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
     Public Class frmApplyShapeBase
        Inherits frmEwE

        Private WithEvents m_grid As ScientificInterface.Ecosim.ApplyShapeEwEGrid

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region " Baseclass overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            ' Config grid
            Me.m_grid.ApplyTargetMode = Me.ApplyTargetMode
            Me.m_grid.ApplyShapeMode = Me.ApplyShapeMode
            Me.m_grid.UIContext = Me.UIContext

            ' Hook up to core messages
            ' * Shapes manager to refresh lists of avialable FFs
            ' * Ecopath to refresh lists of available groups
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager, eCoreComponentType.EcoPath, eCoreComponentType.PPIManager}
        End Sub

#End Region ' Baseclass overrides

#Region " Base functionality "

        Protected Sub ClearAllPairs()
            Me.Grid.ClearAllPairs()
        End Sub

        Protected Sub SetAllPairs()
            Me.Grid.SetAllPairs()
        End Sub

#End Region ' Base functionality

#Region " Mandatory overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            Dim bMustRedimension As Boolean = False
            Dim bMustUpdate As Boolean = False

            If (msg.Source = eCoreComponentType.ShapesManager) Then
                If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                    ' Redimension when number of shapes has changed
                    bMustRedimension = True
                End If
            End If

            ' Refresh when Ecopath number of groups has changed
            If (msg.Source = eCoreComponentType.EcoPath) Then
                If ((msg.Type = eMessageType.DataAddedOrRemoved) And (msg.DataType = eDataTypes.EcoPathGroupInput)) Then
                    bMustRedimension = True
                ElseIf (msg.Type = eMessageType.DietComp) Then
                    bMustUpdate = True
                End If
            End If

            If (msg.Source = eCoreComponentType.PPIManager) Then
                ' Update content to show new assignments
                bMustUpdate = True
            End If

            If bMustRedimension Then
                Me.Grid.RefreshContent()
            Else
                If bMustUpdate Then
                    Me.Grid.UpdateContent()
                End If
            End If
        End Sub

        Protected Overridable Function ApplyTargetMode() As eApplyTargetTypes
            Return eApplyTargetTypes.NotSet
        End Function

        Protected Overridable Function ApplyShapeMode() As eApplyShapeTypes
            Return eApplyShapeTypes.NotSet
        End Function

        Protected Function Grid() As ApplyShapeEwEGrid
            Return Me.m_grid
        End Function

#End Region ' Mandatory overrides

        Private Sub InitializeComponent()
            Me.m_grid = New ScientificInterface.Ecosim.ApplyShapeEwEGrid
            Me.SuspendLayout()
            '
            'm_grid
            '
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            Me.m_grid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Location = New System.Drawing.Point(0, 0)
            Me.m_grid.Name = "m_grid"
            Me.m_grid.Size = New System.Drawing.Size(292, 273)
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_grid.TabIndex = 0
            Me.m_grid.TrackPropertySelection = True
            '
            'frmApplyShapeBase
            '
            Me.ClientSize = New System.Drawing.Size(292, 273)
            Me.Controls.Add(Me.m_grid)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmApplyShapeBase"
            Me.ResumeLayout(False)

        End Sub
    End Class

End Namespace
