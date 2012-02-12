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
Imports ScientificInterfaceShared

Namespace Ecosim

    <CLSCompliant(False)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmForcingFunction
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmForcingFunction))
            Me.m_split = New System.Windows.Forms.SplitContainer
            Me.m_tlpShapeToolbox = New System.Windows.Forms.TableLayoutPanel
            Me.m_shapeToolbox = New ucShapeToolbox
            Me.m_shapeToolboxToolbar = New ucShapeToolboxToolbar
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_sketchPadToolbar = New ucSketchPadToolbar
            Me.m_sketchPad = New ucForcingSketchPad
            Me.m_split.Panel1.SuspendLayout()
            Me.m_split.Panel2.SuspendLayout()
            Me.m_split.SuspendLayout()
            Me.m_tlpShapeToolbox.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_split
            '
            Me.m_split.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_split, "m_split")
            Me.m_split.Name = "m_split"
            '
            'm_split.Panel1
            '
            Me.m_split.Panel1.Controls.Add(Me.TableLayoutPanel1)
            '
            'm_split.Panel2
            '
            Me.m_split.Panel2.Controls.Add(Me.m_tlpShapeToolbox)
            '
            'm_tlpShapeToolbox
            '
            resources.ApplyResources(Me.m_tlpShapeToolbox, "m_tlpShapeToolbox")
            Me.m_tlpShapeToolbox.Controls.Add(Me.m_shapeToolbox, 0, 1)
            Me.m_tlpShapeToolbox.Controls.Add(Me.m_shapeToolboxToolbar, 0, 0)
            Me.m_tlpShapeToolbox.Name = "m_tlpShapeToolbox"
            '
            'm_shapeToolbox
            '
            Me.m_shapeToolbox.Color = System.Drawing.Color.Empty
            resources.ApplyResources(Me.m_shapeToolbox, "m_shapeToolbox")
            Me.m_shapeToolbox.Handler = Nothing
            Me.m_shapeToolbox.Name = "m_shapeToolbox"
            Me.m_shapeToolbox.Selection = Nothing
            '
            'm_shapeToolboxToolbar
            '
            resources.ApplyResources(Me.m_shapeToolboxToolbar, "m_shapeToolboxToolbar")
            Me.m_shapeToolboxToolbar.Handler = Nothing
            Me.m_shapeToolboxToolbar.Name = "m_shapeToolboxToolbar"
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
            resources.ApplyResources(Me.m_sketchPadToolbar, "m_sketchPadToolbar")
            Me.m_sketchPadToolbar.Name = "m_sketchPadToolbar"
            '
            'm_sketchPad
            '
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.Name = "m_sketchPad"
            '
            'frmForcingFunction
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_split)
            Me.Name = "frmForcingFunction"
            Me.m_split.Panel1.ResumeLayout(False)
            Me.m_split.Panel2.ResumeLayout(False)
            Me.m_split.ResumeLayout(False)
            Me.m_tlpShapeToolbox.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_split As System.Windows.Forms.SplitContainer
        Private WithEvents m_shapeToolbox As ucShapeToolbox
        Private WithEvents m_tlpShapeToolbox As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_shapeToolboxToolbar As ucShapeToolboxToolbar
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_sketchPadToolbar As ucSketchPadToolbar
        Private WithEvents m_sketchPad As ucForcingSketchPad
    End Class
End Namespace

