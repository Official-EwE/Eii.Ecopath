Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <CLSCompliant(False)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmTimeSeries
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTimeSeries))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.plSketchPad = New System.Windows.Forms.Panel
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_sketchPadToolbar = New ScientificInterfaceShared.Controls.ucSketchPadToolbar
            Me.m_sketchPad = New ScientificInterfaceShared.Controls.ucTimeSeriesSketchPad
            Me.m_tlbShapeToolBox = New System.Windows.Forms.TableLayoutPanel
            Me.m_shapeToolbox = New ScientificInterfaceShared.Controls.ucShapeToolbox
            Me.m_shapeToolboxToolbar = New ScientificInterfaceShared.Controls.ucShapeToolboxToolbar
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.plSketchPad.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_tlbShapeToolBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.plSketchPad)
            '
            'SplitContainer1.Panel2
            '
            resources.ApplyResources(Me.SplitContainer1.Panel2, "SplitContainer1.Panel2")
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_tlbShapeToolBox)
            '
            'plSketchPad
            '
            Me.plSketchPad.Controls.Add(Me.TableLayoutPanel1)
            resources.ApplyResources(Me.plSketchPad, "plSketchPad")
            Me.plSketchPad.Name = "plSketchPad"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_sketchPadToolbar, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.m_sketchPad, 0, 1)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
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
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.DisplayAxis = True
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.Editable = False
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.ShowXMark = False
            Me.m_sketchPad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Dots
            Me.m_sketchPad.XMarkLabel = ""
            Me.m_sketchPad.XMarkValue = -9999.0!
            Me.m_sketchPad.YAxisAutoScaleMode = ScientificInterfaceShared.Definitions.eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = -9999.0!
            Me.m_sketchPad.YMarkLabel = ""
            Me.m_sketchPad.YMarkValue = -9999.0!
            '
            'm_tlbShapeToolBox
            '
            resources.ApplyResources(Me.m_tlbShapeToolBox, "m_tlbShapeToolBox")
            Me.m_tlbShapeToolBox.Controls.Add(Me.m_shapeToolbox, 0, 1)
            Me.m_tlbShapeToolBox.Controls.Add(Me.m_shapeToolboxToolbar, 0, 0)
            Me.m_tlbShapeToolBox.Name = "m_tlbShapeToolBox"
            '
            'm_shapeToolbox
            '
            Me.m_shapeToolbox.AllowCheckboxes = False
            Me.m_shapeToolbox.Color = System.Drawing.Color.Empty
            resources.ApplyResources(Me.m_shapeToolbox, "m_shapeToolbox")
            Me.m_shapeToolbox.Handler = Nothing
            Me.m_shapeToolbox.Name = "m_shapeToolbox"
            Me.m_shapeToolbox.Selection = New EwECore.cShapeData(-1) {}
            Me.m_shapeToolbox.YAxisMinValue = -9999.0!
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Handler = Nothing
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
            '
            'frmTimeSeries
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmTimeSeries"
            Me.TabText = ""
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.plSketchPad.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.m_tlbShapeToolBox.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plSketchPad As System.Windows.Forms.Panel
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Friend WithEvents m_sketchPad As ucTimeSeriesSketchPad
        Friend WithEvents m_tlbShapeToolBox As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_shapeToolbox As ucShapeToolbox
        Friend WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar

    End Class
End Namespace

