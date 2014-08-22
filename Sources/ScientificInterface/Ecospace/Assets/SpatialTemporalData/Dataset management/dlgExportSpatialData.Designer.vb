Namespace Ecospace.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgExportSpatialData
        Inherits System.Windows.Forms.Form

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgExportSpatialData))
            Me.m_clbDatsets = New System.Windows.Forms.CheckedListBox()
            Me.m_btnAll = New System.Windows.Forms.Button()
            Me.m_btnNone = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.m_btnExport = New System.Windows.Forms.Button()
            Me.m_lblName = New System.Windows.Forms.Label()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            Me.m_btnUsed = New System.Windows.Forms.Button()
            Me.m_lblFolderPreview = New System.Windows.Forms.Label()
            Me.m_lblFolder = New System.Windows.Forms.Label()
            Me.m_hdrDestination = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrLabel = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.SuspendLayout()
            '
            'm_clbDatsets
            '
            resources.ApplyResources(Me.m_clbDatsets, "m_clbDatsets")
            Me.m_clbDatsets.CheckOnClick = True
            Me.m_clbDatsets.FormattingEnabled = True
            Me.m_clbDatsets.Name = "m_clbDatsets"
            Me.m_clbDatsets.ThreeDCheckBoxes = True
            '
            'm_btnAll
            '
            resources.ApplyResources(Me.m_btnAll, "m_btnAll")
            Me.m_btnAll.Name = "m_btnAll"
            Me.m_btnAll.UseVisualStyleBackColor = True
            '
            'm_btnNone
            '
            resources.ApplyResources(Me.m_btnNone, "m_btnNone")
            Me.m_btnNone.Name = "m_btnNone"
            Me.m_btnNone.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_btnExport
            '
            resources.ApplyResources(Me.m_btnExport, "m_btnExport")
            Me.m_btnExport.Name = "m_btnExport"
            Me.m_btnExport.UseVisualStyleBackColor = True
            '
            'm_lblName
            '
            resources.ApplyResources(Me.m_lblName, "m_lblName")
            Me.m_lblName.Name = "m_lblName"
            '
            'm_tbxName
            '
            resources.ApplyResources(Me.m_tbxName, "m_tbxName")
            Me.m_tbxName.Name = "m_tbxName"
            '
            'm_btnUsed
            '
            resources.ApplyResources(Me.m_btnUsed, "m_btnUsed")
            Me.m_btnUsed.Name = "m_btnUsed"
            Me.m_btnUsed.UseVisualStyleBackColor = True
            '
            'm_lblFolderPreview
            '
            resources.ApplyResources(Me.m_lblFolderPreview, "m_lblFolderPreview")
            Me.m_lblFolderPreview.Name = "m_lblFolderPreview"
            '
            'm_lblFolder
            '
            resources.ApplyResources(Me.m_lblFolder, "m_lblFolder")
            Me.m_lblFolder.Name = "m_lblFolder"
            '
            'm_hdrDestination
            '
            resources.ApplyResources(Me.m_hdrDestination, "m_hdrDestination")
            Me.m_hdrDestination.CanCollapseParent = False
            Me.m_hdrDestination.CollapsedParentHeight = 0
            Me.m_hdrDestination.IsCollapsed = False
            Me.m_hdrDestination.Name = "m_hdrDestination"
            '
            'm_hdrLabel
            '
            resources.ApplyResources(Me.m_hdrLabel, "m_hdrLabel")
            Me.m_hdrLabel.CanCollapseParent = False
            Me.m_hdrLabel.CollapsedParentHeight = 0
            Me.m_hdrLabel.IsCollapsed = False
            Me.m_hdrLabel.Name = "m_hdrLabel"
            '
            'dlgExportSpatialData
            '
            Me.AcceptButton = Me.m_btnExport
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.ControlBox = False
            Me.Controls.Add(Me.m_lblFolder)
            Me.Controls.Add(Me.m_lblFolderPreview)
            Me.Controls.Add(Me.m_tbxName)
            Me.Controls.Add(Me.m_lblName)
            Me.Controls.Add(Me.m_hdrDestination)
            Me.Controls.Add(Me.m_hdrLabel)
            Me.Controls.Add(Me.m_btnExport)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnUsed)
            Me.Controls.Add(Me.m_btnNone)
            Me.Controls.Add(Me.m_btnAll)
            Me.Controls.Add(Me.m_clbDatsets)
            Me.Name = "dlgExportSpatialData"
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_btnExport As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_btnNone As System.Windows.Forms.Button
        Private WithEvents m_btnAll As System.Windows.Forms.Button
        Private WithEvents m_clbDatsets As System.Windows.Forms.CheckedListBox
        Private WithEvents m_hdrLabel As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_hdrDestination As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Friend WithEvents m_lblName As System.Windows.Forms.Label
        Private WithEvents m_tbxName As System.Windows.Forms.TextBox
        Private WithEvents m_btnUsed As System.Windows.Forms.Button
        Private WithEvents m_lblFolderPreview As System.Windows.Forms.Label
        Friend WithEvents m_lblFolder As System.Windows.Forms.Label
    End Class

End Namespace
