Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcosimResults
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcosimResults))
            Me.m_lblNumTimeSteps = New System.Windows.Forms.Label
            Me.udNumTimeSteps = New System.Windows.Forms.NumericUpDown
            Me.m_nudSumEnd = New System.Windows.Forms.NumericUpDown
            Me.m_nudSumStart = New System.Windows.Forms.NumericUpDown
            Me.m_lblBegin = New System.Windows.Forms.Label
            Me.m_lblEnd = New System.Windows.Forms.Label
            Me.m_cmbFleets = New System.Windows.Forms.ComboBox
            Me.m_rbGroup = New System.Windows.Forms.RadioButton
            Me.m_rbIndices = New System.Windows.Forms.RadioButton
            Me.m_rbGear = New System.Windows.Forms.RadioButton
            Me.m_plResultsGrid = New System.Windows.Forms.Panel
            Me.m_hdrYear = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_hdrShow = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSumEnd, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudSumStart, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblNumTimeSteps
            '
            resources.ApplyResources(Me.m_lblNumTimeSteps, "m_lblNumTimeSteps")
            Me.m_lblNumTimeSteps.Name = "m_lblNumTimeSteps"
            '
            'udNumTimeSteps
            '
            resources.ApplyResources(Me.udNumTimeSteps, "udNumTimeSteps")
            Me.udNumTimeSteps.Name = "udNumTimeSteps"
            '
            'm_nudSumEnd
            '
            resources.ApplyResources(Me.m_nudSumEnd, "m_nudSumEnd")
            Me.m_nudSumEnd.Name = "m_nudSumEnd"
            '
            'm_nudSumStart
            '
            resources.ApplyResources(Me.m_nudSumStart, "m_nudSumStart")
            Me.m_nudSumStart.Name = "m_nudSumStart"
            '
            'm_lblBegin
            '
            resources.ApplyResources(Me.m_lblBegin, "m_lblBegin")
            Me.m_lblBegin.Name = "m_lblBegin"
            '
            'm_lblEnd
            '
            resources.ApplyResources(Me.m_lblEnd, "m_lblEnd")
            Me.m_lblEnd.Name = "m_lblEnd"
            '
            'm_cmbFleets
            '
            Me.m_cmbFleets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbFleets.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbFleets, "m_cmbFleets")
            Me.m_cmbFleets.Name = "m_cmbFleets"
            '
            'm_rbGroup
            '
            resources.ApplyResources(Me.m_rbGroup, "m_rbGroup")
            Me.m_rbGroup.Name = "m_rbGroup"
            Me.m_rbGroup.TabStop = True
            Me.m_rbGroup.UseVisualStyleBackColor = True
            '
            'm_rbIndices
            '
            resources.ApplyResources(Me.m_rbIndices, "m_rbIndices")
            Me.m_rbIndices.Name = "m_rbIndices"
            Me.m_rbIndices.TabStop = True
            Me.m_rbIndices.UseVisualStyleBackColor = True
            '
            'm_rbGear
            '
            resources.ApplyResources(Me.m_rbGear, "m_rbGear")
            Me.m_rbGear.Name = "m_rbGear"
            Me.m_rbGear.TabStop = True
            Me.m_rbGear.UseVisualStyleBackColor = True
            '
            'm_plResultsGrid
            '
            resources.ApplyResources(Me.m_plResultsGrid, "m_plResultsGrid")
            Me.m_plResultsGrid.Name = "m_plResultsGrid"
            '
            'm_hdrYear
            '
            resources.ApplyResources(Me.m_hdrYear, "m_hdrYear")
            Me.m_hdrYear.Name = "m_hdrYear"
            '
            'm_hdrShow
            '
            resources.ApplyResources(Me.m_hdrShow, "m_hdrShow")
            Me.m_hdrShow.Name = "m_hdrShow"
            '
            'EcosimResults
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_hdrShow)
            Me.Controls.Add(Me.m_cmbFleets)
            Me.Controls.Add(Me.m_lblNumTimeSteps)
            Me.Controls.Add(Me.m_rbGroup)
            Me.Controls.Add(Me.m_hdrYear)
            Me.Controls.Add(Me.m_rbIndices)
            Me.Controls.Add(Me.udNumTimeSteps)
            Me.Controls.Add(Me.m_rbGear)
            Me.Controls.Add(Me.m_nudSumEnd)
            Me.Controls.Add(Me.m_nudSumStart)
            Me.Controls.Add(Me.m_plResultsGrid)
            Me.Controls.Add(Me.m_lblBegin)
            Me.Controls.Add(Me.m_lblEnd)
            Me.Name = "EcosimResults"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            CType(Me.udNumTimeSteps, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSumEnd, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudSumStart, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblBegin As System.Windows.Forms.Label
        Private WithEvents m_lblEnd As System.Windows.Forms.Label
        Private WithEvents m_rbGroup As System.Windows.Forms.RadioButton
        Private WithEvents m_rbIndices As System.Windows.Forms.RadioButton
        Private WithEvents m_rbGear As System.Windows.Forms.RadioButton
        Private WithEvents m_nudSumEnd As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudSumStart As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblNumTimeSteps As System.Windows.Forms.Label
        Private WithEvents udNumTimeSteps As System.Windows.Forms.NumericUpDown
        Private WithEvents m_cmbFleets As System.Windows.Forms.ComboBox
        Private WithEvents m_hdrYear As cEwEHeaderLabel
        Private WithEvents m_hdrShow As cEwEHeaderLabel
        Protected WithEvents m_plResultsGrid As System.Windows.Forms.Panel
    End Class

End Namespace

