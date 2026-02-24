' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Other

    Partial Class ucAutosaveOption
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAutosaveOption))
            Me.m_cbOption = New System.Windows.Forms.CheckBox()
            Me.m_lblPath = New System.Windows.Forms.Label()
            Me.m_btnVisitFolder = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'm_cbOption
            '
            resources.ApplyResources(Me.m_cbOption, "m_cbOption")
            Me.m_cbOption.Name = "m_cbOption"
            Me.m_cbOption.UseVisualStyleBackColor = True
            '
            'm_lblPath
            '
            resources.ApplyResources(Me.m_lblPath, "m_lblPath")
            Me.m_lblPath.Name = "m_lblPath"
            '
            'm_btnVisitFolder
            '
            resources.ApplyResources(Me.m_btnVisitFolder, "m_btnVisitFolder")
            Me.m_btnVisitFolder.Name = "m_btnVisitFolder"
            Me.m_btnVisitFolder.UseVisualStyleBackColor = True
            '
            'ucAutosaveOption
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_btnVisitFolder)
            Me.Controls.Add(Me.m_lblPath)
            Me.Controls.Add(Me.m_cbOption)
            Me.Name = "ucAutosaveOption"
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_cbOption As System.Windows.Forms.CheckBox
        Private WithEvents m_lblPath As System.Windows.Forms.Label
        Private WithEvents m_btnVisitFolder As System.Windows.Forms.Button

    End Class

End Namespace
