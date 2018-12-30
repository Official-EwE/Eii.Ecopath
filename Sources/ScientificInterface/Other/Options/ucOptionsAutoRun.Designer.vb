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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Other

    Partial Class ucOptionsAutoRun
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsAutoRun))
            Me.m_plAutorun = New System.Windows.Forms.Panel()
            Me.m_hdrMain = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.SuspendLayout()
            '
            'm_plAutorun
            '
            resources.ApplyResources(Me.m_plAutorun, "m_plAutorun")
            Me.m_plAutorun.Name = "m_plAutorun"
            '
            'm_hdrMain
            '
            resources.ApplyResources(Me.m_hdrMain, "m_hdrMain")
            Me.m_hdrMain.CanCollapseParent = False
            Me.m_hdrMain.CollapsedParentHeight = 0
            Me.m_hdrMain.IsCollapsed = False
            Me.m_hdrMain.Name = "m_hdrMain"
            '
            'ucOptionsAutoRun
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_plAutorun)
            Me.Controls.Add(Me.m_hdrMain)
            Me.Name = "ucOptionsAutoRun"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_hdrMain As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_plAutorun As System.Windows.Forms.Panel

    End Class

End Namespace
