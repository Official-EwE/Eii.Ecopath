' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

Namespace Ecosim

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTimeSeries))
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.plSketchPad = New System.Windows.Forms.Panel()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.m_sketchPadToolbar = New ScientificInterfaceShared.Controls.ucSketchPadToolbar()
            Me.m_sketchPad = New ScientificInterfaceShared.Controls.ucTimeSeriesSketchPad()
            Me.m_tlbShapeToolBox = New System.Windows.Forms.TableLayoutPanel()
            Me.m_shapeToolbox = New ScientificInterfaceShared.Controls.ucShapeToolbox()
            Me.m_shapeToolboxToolbar = New ScientificInterfaceShared.Controls.ucShapeToolboxToolbar()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.plSketchPad.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_tlbShapeToolBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.plSketchPad)
            '
            'm_scMain.Panel2
            '
            resources.ApplyResources(Me.m_scMain.Panel2, "m_scMain.Panel2")
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlbShapeToolBox)
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
            Me.m_sketchPadToolbar.UIContext = Nothing
            '
            'm_sketchPad
            '
            Me.m_sketchPad.AllowedEdits = ScientificInterfaceShared.Controls.ucSketchPad.eMouseInteractionMode.DrawShape
            Me.m_sketchPad.BackColor = System.Drawing.SystemColors.Window
            Me.m_sketchPad.Cursor = System.Windows.Forms.Cursors.Cross
            Me.m_sketchPad.DisplayAxis = True
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.Editable = False
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.NumDataPoints = 0
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.ShowValueTooltip = True
            Me.m_sketchPad.ShowXMark = False
            Me.m_sketchPad.ShowYMark = False
            Me.m_sketchPad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Dots
            Me.m_sketchPad.UIContext = Nothing
            Me.m_sketchPad.XAxisMaxValue = -9999
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
            Me.m_shapeToolbox.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
            Me.m_shapeToolbox.UIContext = Nothing
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
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "frmTimeSeries"
            Me.TabText = ""
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.plSketchPad.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.m_tlbShapeToolBox.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plSketchPad As System.Windows.Forms.Panel
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Friend WithEvents m_sketchPad As ucTimeSeriesSketchPad
        Friend WithEvents m_tlbShapeToolBox As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_shapeToolbox As ucShapeToolbox
        Friend WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer

    End Class
End Namespace

