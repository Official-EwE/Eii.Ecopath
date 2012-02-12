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
Namespace Controls.Map.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorSailCost
        Inherits ucLayerEditorFleet

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorSailCost))
            Me.m_btnCalculate = New System.Windows.Forms.Button
            Me.m_btnSmooth = New System.Windows.Forms.Button
            Me.m_tlp = New System.Windows.Forms.TableLayoutPanel
            Me.m_tlp.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnCalculate
            '
            resources.ApplyResources(Me.m_btnCalculate, "m_btnCalculate")
            Me.m_btnCalculate.Name = "m_btnCalculate"
            Me.m_btnCalculate.UseVisualStyleBackColor = True
            '
            'm_btnSmooth
            '
            resources.ApplyResources(Me.m_btnSmooth, "m_btnSmooth")
            Me.m_btnSmooth.Name = "m_btnSmooth"
            Me.m_btnSmooth.UseVisualStyleBackColor = True
            '
            'm_tlp
            '
            resources.ApplyResources(Me.m_tlp, "m_tlp")
            Me.m_tlp.Controls.Add(Me.m_btnCalculate, 0, 0)
            Me.m_tlp.Controls.Add(Me.m_btnSmooth, 1, 0)
            Me.m_tlp.Name = "m_tlp"
            '
            'ucLayerEditorSailCost
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlp)
            Me.Name = "ucLayerEditorSailCost"
            Me.Controls.SetChildIndex(Me.m_tlp, 0)
            Me.m_tlp.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button
        Private WithEvents m_btnSmooth As System.Windows.Forms.Button
        Private WithEvents m_tlp As System.Windows.Forms.TableLayoutPanel

    End Class

End Namespace
