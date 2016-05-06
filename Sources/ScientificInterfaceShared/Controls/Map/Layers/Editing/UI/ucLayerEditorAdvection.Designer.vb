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
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorAdvection
        Inherits ucLayerEditor

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucLayerEditorAdvection))
            Me.m_lblAngle = New System.Windows.Forms.Label
            Me.m_lblVelocity = New System.Windows.Forms.Label
            Me.m_nudAngle = New cEwENumericUpDown
            Me.m_nudVelocity = New cEwENumericUpDown
            Me.m_pbSample = New System.Windows.Forms.PictureBox
            CType(Me.m_nudAngle, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudVelocity, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbSample, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblAngle
            '
            resources.ApplyResources(Me.m_lblAngle, "m_lblAngle")
            Me.m_lblAngle.Name = "m_lblAngle"
            '
            'm_lblVelocity
            '
            resources.ApplyResources(Me.m_lblVelocity, "m_lblVelocity")
            Me.m_lblVelocity.Name = "m_lblVelocity"
            '
            'm_nudAngle
            '
            resources.ApplyResources(Me.m_nudAngle, "m_nudAngle")
            Me.m_nudAngle.Maximum = New Decimal(New Integer() {359, 0, 0, 0})
            Me.m_nudAngle.Name = "m_nudAngle"
            '
            'm_nudVelocity
            '
            resources.ApplyResources(Me.m_nudVelocity, "m_nudVelocity")
            Me.m_nudVelocity.Name = "m_nudVelocity"
            '
            'm_pbSample
            '
            resources.ApplyResources(Me.m_pbSample, "m_pbSample")
            Me.m_pbSample.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbSample.Name = "m_pbSample"
            Me.m_pbSample.TabStop = False
            '
            'ucLayerEditorAdvection
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblAngle)
            Me.Controls.Add(Me.m_nudVelocity)
            Me.Controls.Add(Me.m_nudAngle)
            Me.Controls.Add(Me.m_lblVelocity)
            Me.Controls.Add(Me.m_pbSample)
            Me.Name = "ucLayerEditorAdvection"
            Me.Controls.SetChildIndex(Me.m_pbSample, 0)
            Me.Controls.SetChildIndex(Me.m_lblVelocity, 0)
            Me.Controls.SetChildIndex(Me.m_nudAngle, 0)
            Me.Controls.SetChildIndex(Me.m_nudVelocity, 0)
            Me.Controls.SetChildIndex(Me.m_lblAngle, 0)
            CType(Me.m_nudAngle, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudVelocity, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbSample, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudAngle As System.Windows.Forms.NumericUpDown
        Private WithEvents m_pbSample As System.Windows.Forms.PictureBox
        Private WithEvents m_lblAngle As System.Windows.Forms.Label
        Private WithEvents m_lblVelocity As System.Windows.Forms.Label
        Private WithEvents m_nudVelocity As System.Windows.Forms.NumericUpDown

    End Class

End Namespace
