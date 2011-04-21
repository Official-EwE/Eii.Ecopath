Imports ScientificInterfaceShared.Forms

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmMediationFunction
        Inherits frmEwE

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMediationFunction))
            Me.plSketchPad = New System.Windows.Forms.Panel
            Me.tlpSketchPad = New System.Windows.Forms.TableLayoutPanel
            Me.m_sketchPadToolbar = New ScientificInterfaceShared.Controls.ucSketchPadToolbar
            Me.m_sketchPad = New ScientificInterfaceShared.Controls.ucMediationSketchPad
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
            Me.m_tlpBiopercent = New System.Windows.Forms.TableLayoutPanel
            Me.m_assignmentsToolbar = New ScientificInterfaceShared.Controls.ucMediationAssignmentsToolbar
            Me.m_assignments = New ScientificInterfaceShared.Controls.ucMediationAssignments
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_shapeToolBox = New ScientificInterfaceShared.Controls.ucShapeToolbox
            Me.m_shapeToolboxToolbar = New ScientificInterfaceShared.Controls.ucShapeToolboxToolbar
            Me.plSketchPad.SuspendLayout()
            Me.tlpSketchPad.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SplitContainer2.Panel1.SuspendLayout()
            Me.SplitContainer2.Panel2.SuspendLayout()
            Me.SplitContainer2.SuspendLayout()
            Me.m_tlpBiopercent.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'plSketchPad
            '
            Me.plSketchPad.Controls.Add(Me.tlpSketchPad)
            resources.ApplyResources(Me.plSketchPad, "plSketchPad")
            Me.plSketchPad.Name = "plSketchPad"
            '
            'tlpSketchPad
            '
            resources.ApplyResources(Me.tlpSketchPad, "tlpSketchPad")
            Me.tlpSketchPad.Controls.Add(Me.m_sketchPadToolbar, 0, 0)
            Me.tlpSketchPad.Controls.Add(Me.m_sketchPad, 0, 1)
            Me.tlpSketchPad.Name = "tlpSketchPad"
            '
            'm_sketchPadToolbar
            '
            Me.m_sketchPadToolbar.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.m_sketchPadToolbar, "m_sketchPadToolbar")
            Me.m_sketchPadToolbar.Handler = Nothing
            Me.m_sketchPadToolbar.Name = "m_sketchPadToolbar"
            '
            'm_sketchPad
            '
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.Cursor = System.Windows.Forms.Cursors.Cross
            Me.m_sketchPad.DisplayAxis = True
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.ShowValueTooltip = True
            Me.m_sketchPad.ShowXMark = True
            Me.m_sketchPad.ShowYMark = True
            Me.m_sketchPad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
            Me.m_sketchPad.UIContext = Nothing
            Me.m_sketchPad.XAxisLabel = "Mediating group (weighted) biomass"
            Me.m_sketchPad.XMarkValue = -9999.0!
            Me.m_sketchPad.YAxisAutoScaleMode = ScientificInterfaceShared.Definitions.eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = -9999.0!
            Me.m_sketchPad.YMarkLabel = ""
            Me.m_sketchPad.YMarkValue = -9999.0!
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel1)
            '
            'SplitContainer2
            '
            Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.SplitContainer2, "SplitContainer2")
            Me.SplitContainer2.Name = "SplitContainer2"
            '
            'SplitContainer2.Panel1
            '
            Me.SplitContainer2.Panel1.Controls.Add(Me.plSketchPad)
            '
            'SplitContainer2.Panel2
            '
            Me.SplitContainer2.Panel2.Controls.Add(Me.m_tlpBiopercent)
            '
            'm_tlpBiopercent
            '
            resources.ApplyResources(Me.m_tlpBiopercent, "m_tlpBiopercent")
            Me.m_tlpBiopercent.Controls.Add(Me.m_assignmentsToolbar, 0, 0)
            Me.m_tlpBiopercent.Controls.Add(Me.m_assignments, 0, 1)
            Me.m_tlpBiopercent.Name = "m_tlpBiopercent"
            '
            'm_assignmentsToolbar
            '
            resources.ApplyResources(Me.m_assignmentsToolbar, "m_assignmentsToolbar")
            Me.m_assignmentsToolbar.BackColor = System.Drawing.SystemColors.Control
            Me.m_assignmentsToolbar.DefineMediationLabel = "<prompt>"
            Me.m_assignmentsToolbar.Handler = Nothing
            Me.m_assignmentsToolbar.IsMenuVisible = True
            Me.m_assignmentsToolbar.Name = "m_assignmentsToolbar"
            '
            'm_assignments
            '
            Me.m_assignments.Data = Nothing
            resources.ApplyResources(Me.m_assignments, "m_assignments")
            Me.m_assignments.Name = "m_assignments"
            Me.m_assignments.Shape = Nothing
            Me.m_assignments.UIContext = Nothing
            Me.m_assignments.XAxisLabel = "Mediating group"
            Me.m_assignments.YAxisLabel = "Relative weight"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolBox, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.m_shapeToolboxToolbar, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_shapeToolBox
            '
            Me.m_shapeToolBox.AllowCheckboxes = False
            Me.m_shapeToolBox.Color = System.Drawing.Color.Empty
            resources.ApplyResources(Me.m_shapeToolBox, "m_shapeToolBox")
            Me.m_shapeToolBox.Handler = Nothing
            Me.m_shapeToolBox.Name = "m_shapeToolBox"
            Me.m_shapeToolBox.Selection = New EwECore.cShapeData(-1) {}
            Me.m_shapeToolBox.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
            Me.m_shapeToolBox.UIContext = Nothing
            Me.m_shapeToolBox.YAxisMinValue = -9999.0!
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Handler = Nothing
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
            '
            'frmMediationFunction
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmMediationFunction"
            Me.plSketchPad.ResumeLayout(False)
            Me.tlpSketchPad.ResumeLayout(False)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.SplitContainer2.Panel1.ResumeLayout(False)
            Me.SplitContainer2.Panel2.ResumeLayout(False)
            Me.SplitContainer2.ResumeLayout(False)
            Me.m_tlpBiopercent.ResumeLayout(False)
            Me.m_tlpBiopercent.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents plSketchPad As System.Windows.Forms.Panel
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
        Private WithEvents tlpSketchPad As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_shapeToolBox As ucShapeToolbox
        Private WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar
        Private WithEvents m_sketchPad As ucMediationSketchPad
        Private WithEvents m_assignments As ucMediationAssignments
        Private WithEvents m_assignmentsToolbar As ucMediationAssignmentsToolbar
        Private WithEvents m_tlpBiopercent As System.Windows.Forms.TableLayoutPanel

    End Class
End Namespace

