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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmTransectWriter
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTransectWriter))
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.m_mapzoom = New ScientificInterfaceShared.Controls.Map.ucMapZoom()
        Me.m_toolstrip = New ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar()
        Me.m_tlpPoints = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblXY1 = New System.Windows.Forms.Label()
        Me.m_tbxX2 = New System.Windows.Forms.TextBox()
        Me.m_tbxY2 = New System.Windows.Forms.TextBox()
        Me.m_lblXY2 = New System.Windows.Forms.Label()
        Me.m_tbxX1 = New System.Windows.Forms.TextBox()
        Me.m_tbxY1 = New System.Windows.Forms.TextBox()
        Me.m_lblXY1Units = New System.Windows.Forms.Label()
        Me.m_lblXY2Units = New System.Windows.Forms.Label()
        Me.m_btnTDelete = New System.Windows.Forms.Button()
        Me.m_btnTRename = New System.Windows.Forms.Button()
        Me.m_btnTAdd = New System.Windows.Forms.Button()
        Me.m_tbxTName = New System.Windows.Forms.TextBox()
        Me.m_lbxTransects = New System.Windows.Forms.ListBox()
        Me.m_cbAutosave = New System.Windows.Forms.CheckBox()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tlpPoints.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_mapzoom)
        Me.m_scMain.Panel1.Controls.Add(Me.m_toolstrip)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_tlpPoints)
        Me.m_scMain.Panel2.Controls.Add(Me.m_btnTDelete)
        Me.m_scMain.Panel2.Controls.Add(Me.m_btnTRename)
        Me.m_scMain.Panel2.Controls.Add(Me.m_btnTAdd)
        Me.m_scMain.Panel2.Controls.Add(Me.m_tbxTName)
        Me.m_scMain.Panel2.Controls.Add(Me.m_lbxTransects)
        Me.m_scMain.Panel2.Controls.Add(Me.m_cbAutosave)
        '
        'm_mapzoom
        '
        resources.ApplyResources(Me.m_mapzoom, "m_mapzoom")
        Me.m_mapzoom.Name = "m_mapzoom"
        Me.m_mapzoom.PositionMode = ScientificInterfaceShared.Controls.Map.ucMapZoom.ePositionModeTypes.Center
        Me.m_mapzoom.UIContext = Nothing
        Me.m_mapzoom.ZoomPercentage = 100.0!
        '
        'm_toolstrip
        '
        resources.ApplyResources(Me.m_toolstrip, "m_toolstrip")
        Me.m_toolstrip.Name = "m_toolstrip"
        Me.m_toolstrip.PositionMode = ScientificInterfaceShared.Controls.Map.ucMapZoom.ePositionModeTypes.Center
        Me.m_toolstrip.UIContext = Nothing
        '
        'm_tlpPoints
        '
        resources.ApplyResources(Me.m_tlpPoints, "m_tlpPoints")
        Me.m_tlpPoints.Controls.Add(Me.m_lblXY1, 0, 0)
        Me.m_tlpPoints.Controls.Add(Me.m_tbxX2, 1, 1)
        Me.m_tlpPoints.Controls.Add(Me.m_tbxY2, 2, 1)
        Me.m_tlpPoints.Controls.Add(Me.m_lblXY2, 0, 1)
        Me.m_tlpPoints.Controls.Add(Me.m_tbxX1, 1, 0)
        Me.m_tlpPoints.Controls.Add(Me.m_tbxY1, 2, 0)
        Me.m_tlpPoints.Controls.Add(Me.m_lblXY1Units, 3, 0)
        Me.m_tlpPoints.Controls.Add(Me.m_lblXY2Units, 3, 1)
        Me.m_tlpPoints.Name = "m_tlpPoints"
        '
        'm_lblXY1
        '
        resources.ApplyResources(Me.m_lblXY1, "m_lblXY1")
        Me.m_lblXY1.Name = "m_lblXY1"
        '
        'm_tbxX2
        '
        resources.ApplyResources(Me.m_tbxX2, "m_tbxX2")
        Me.m_tbxX2.Name = "m_tbxX2"
        '
        'm_tbxY2
        '
        resources.ApplyResources(Me.m_tbxY2, "m_tbxY2")
        Me.m_tbxY2.Name = "m_tbxY2"
        '
        'm_lblXY2
        '
        resources.ApplyResources(Me.m_lblXY2, "m_lblXY2")
        Me.m_lblXY2.Name = "m_lblXY2"
        '
        'm_tbxX1
        '
        resources.ApplyResources(Me.m_tbxX1, "m_tbxX1")
        Me.m_tbxX1.Name = "m_tbxX1"
        '
        'm_tbxY1
        '
        resources.ApplyResources(Me.m_tbxY1, "m_tbxY1")
        Me.m_tbxY1.Name = "m_tbxY1"
        '
        'm_lblXY1Units
        '
        resources.ApplyResources(Me.m_lblXY1Units, "m_lblXY1Units")
        Me.m_lblXY1Units.Name = "m_lblXY1Units"
        '
        'm_lblXY2Units
        '
        resources.ApplyResources(Me.m_lblXY2Units, "m_lblXY2Units")
        Me.m_lblXY2Units.Name = "m_lblXY2Units"
        '
        'm_btnTDelete
        '
        resources.ApplyResources(Me.m_btnTDelete, "m_btnTDelete")
        Me.m_btnTDelete.Name = "m_btnTDelete"
        Me.m_btnTDelete.UseVisualStyleBackColor = True
        '
        'm_btnTRename
        '
        resources.ApplyResources(Me.m_btnTRename, "m_btnTRename")
        Me.m_btnTRename.Name = "m_btnTRename"
        Me.m_btnTRename.UseVisualStyleBackColor = True
        '
        'm_btnTAdd
        '
        resources.ApplyResources(Me.m_btnTAdd, "m_btnTAdd")
        Me.m_btnTAdd.Name = "m_btnTAdd"
        Me.m_btnTAdd.UseVisualStyleBackColor = True
        '
        'm_tbxTName
        '
        resources.ApplyResources(Me.m_tbxTName, "m_tbxTName")
        Me.m_tbxTName.Name = "m_tbxTName"
        '
        'm_lbxTransects
        '
        resources.ApplyResources(Me.m_lbxTransects, "m_lbxTransects")
        Me.m_lbxTransects.FormattingEnabled = True
        Me.m_lbxTransects.Name = "m_lbxTransects"
        '
        'm_cbAutosave
        '
        resources.ApplyResources(Me.m_cbAutosave, "m_cbAutosave")
        Me.m_cbAutosave.Name = "m_cbAutosave"
        Me.m_cbAutosave.UseVisualStyleBackColor = True
        '
        'frmTransectWriter
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scMain)
        Me.Name = "frmTransectWriter"
        Me.TabText = ""
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.Panel2.PerformLayout()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.m_tlpPoints.ResumeLayout(False)
        Me.m_tlpPoints.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_scMain As Windows.Forms.SplitContainer
    Private WithEvents m_mapzoom As ScientificInterfaceShared.Controls.Map.ucMapZoom
    Private WithEvents m_toolstrip As ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar
    Private WithEvents m_btnTDelete As Windows.Forms.Button
    Private WithEvents m_btnTRename As Windows.Forms.Button
    Private WithEvents m_btnTAdd As Windows.Forms.Button
    Private WithEvents m_tbxTName As Windows.Forms.TextBox
    Friend WithEvents m_lbxTransects As Windows.Forms.ListBox
    Private WithEvents m_cbAutosave As Windows.Forms.CheckBox
    Private WithEvents m_tbxX1 As Windows.Forms.TextBox
    Private WithEvents m_tbxY1 As Windows.Forms.TextBox
    Private WithEvents m_tbxX2 As Windows.Forms.TextBox
    Private WithEvents m_tbxY2 As Windows.Forms.TextBox
    Private WithEvents m_tlpPoints As Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblXY1 As Windows.Forms.Label
    Private WithEvents m_lblXY2 As Windows.Forms.Label
    Private WithEvents m_lblXY1Units As Windows.Forms.Label
    Private WithEvents m_lblXY2Units As Windows.Forms.Label
End Class
