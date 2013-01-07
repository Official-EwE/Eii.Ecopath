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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class autre
    Inherits DockContent

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ETgridinput = New System.Windows.Forms.DataGridView()
        Me.Group_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TTL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Biomass = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Production = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.accessibilty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OI = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Load_from_ecopath = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Save_ETdata = New System.Windows.Forms.Button()
        Me.inputdata = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.modeldescription = New System.Windows.Forms.TextBox()
        Me.Modelname = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.commentaires = New System.Windows.Forms.TextBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.smooth_pdf = New System.Windows.Forms.WebBrowser()
        Me.smooth_graph = New System.Windows.Forms.CheckBox()
        Me.datasmooth = New System.Windows.Forms.DataGridView()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Reset_smooth = New System.Windows.Forms.Button()
        Me.parameters_cst = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.smooth_param_1 = New System.Windows.Forms.MaskedTextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.decalage = New System.Windows.Forms.MaskedTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.smooth_param = New System.Windows.Forms.MaskedTextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.type_smooth3 = New System.Windows.Forms.RadioButton()
        Me.type_smooth2 = New System.Windows.Forms.RadioButton()
        Me.type_smooth1 = New System.Windows.Forms.RadioButton()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.result_pdf = New System.Windows.Forms.WebBrowser()
        Me.getgraphs = New System.Windows.Forms.CheckBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.panel_result = New System.Windows.Forms.TabControl()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.grille_ET_main = New System.Windows.Forms.DataGridView()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.grille_biomass = New System.Windows.Forms.DataGridView()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.grille_biomass_acc = New System.Windows.Forms.DataGridView()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.grille_flow_p = New System.Windows.Forms.DataGridView()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.grille_flow_p_acc = New System.Windows.Forms.DataGridView()
        Me.Y = New System.Windows.Forms.TabPage()
        Me.grille_y = New System.Windows.Forms.DataGridView()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.reset_param_diag = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.mull_eff = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.formd = New System.Windows.Forms.MaskedTextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.beta = New System.Windows.Forms.MaskedTextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TopD = New System.Windows.Forms.MaskedTextBox()
        Me.result_pdf_et_diag = New System.Windows.Forms.WebBrowser()
        Me.tabgrille_flow_mf = New System.Windows.Forms.TabControl()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.grille_ET_main_diagnose = New System.Windows.Forms.DataGridView()
        Me.TabPage11 = New System.Windows.Forms.TabPage()
        Me.grille_biom_mf = New System.Windows.Forms.DataGridView()
        Me.TabPage12 = New System.Windows.Forms.TabPage()
        Me.grille_catches = New System.Windows.Forms.DataGridView()
        Me.TabPage13 = New System.Windows.Forms.TabPage()
        Me.grille_flow_mf = New System.Windows.Forms.DataGridView()
        Me.getgraph_diag = New System.Windows.Forms.CheckBox()
        Me.Button4 = New System.Windows.Forms.Button()
        CType(Me.ETgridinput, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.inputdata.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.datasmooth, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.parameters_cst.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.panel_result.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        CType(Me.grille_ET_main, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage5.SuspendLayout()
        CType(Me.grille_biomass, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage6.SuspendLayout()
        CType(Me.grille_biomass_acc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage7.SuspendLayout()
        CType(Me.grille_flow_p, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage8.SuspendLayout()
        CType(Me.grille_flow_p_acc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Y.SuspendLayout()
        CType(Me.grille_y, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage9.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.tabgrille_flow_mf.SuspendLayout()
        Me.TabPage10.SuspendLayout()
        CType(Me.grille_ET_main_diagnose, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage11.SuspendLayout()
        CType(Me.grille_biom_mf, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage12.SuspendLayout()
        CType(Me.grille_catches, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage13.SuspendLayout()
        CType(Me.grille_flow_mf, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ETgridinput
        '
        Me.ETgridinput.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        Me.ETgridinput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.ETgridinput.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Group_name, Me.TTL, Me.Biomass, Me.Production, Me.accessibilty, Me.OI})
        Me.ETgridinput.Location = New System.Drawing.Point(6, 171)
        Me.ETgridinput.Name = "ETgridinput"
        Me.ETgridinput.Size = New System.Drawing.Size(807, 428)
        Me.ETgridinput.TabIndex = 0
        '
        'Group_name
        '
        Me.Group_name.HeaderText = "Group name"
        Me.Group_name.Name = "Group_name"
        '
        'TTL
        '
        Me.TTL.HeaderText = "Trophic Level"
        Me.TTL.Name = "TTL"
        '
        'Biomass
        '
        Me.Biomass.HeaderText = "Biomass"
        Me.Biomass.Name = "Biomass"
        '
        'Production
        '
        Me.Production.HeaderText = "Production"
        Me.Production.Name = "Production"
        '
        'accessibilty
        '
        Me.accessibilty.HeaderText = "accessibilty"
        Me.accessibilty.Name = "accessibilty"
        '
        'OI
        '
        Me.OI.HeaderText = "Omnivory index"
        Me.OI.Name = "OI"
        '
        'Load_from_ecopath
        '
        Me.Load_from_ecopath.Location = New System.Drawing.Point(6, 20)
        Me.Load_from_ecopath.Name = "Load_from_ecopath"
        Me.Load_from_ecopath.Size = New System.Drawing.Size(246, 24)
        Me.Load_from_ecopath.TabIndex = 1
        Me.Load_from_ecopath.Text = "Load data from Ecopath"
        Me.Load_from_ecopath.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(278, 20)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(246, 24)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Load data from file"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Save_ETdata
        '
        Me.Save_ETdata.Location = New System.Drawing.Point(546, 20)
        Me.Save_ETdata.Name = "Save_ETdata"
        Me.Save_ETdata.Size = New System.Drawing.Size(246, 26)
        Me.Save_ETdata.TabIndex = 3
        Me.Save_ETdata.Text = "Save input data..."
        Me.Save_ETdata.UseVisualStyleBackColor = True
        '
        'inputdata
        '
        Me.inputdata.Controls.Add(Me.TabPage1)
        Me.inputdata.Controls.Add(Me.TabPage2)
        Me.inputdata.Controls.Add(Me.TabPage3)
        Me.inputdata.Controls.Add(Me.TabPage9)
        Me.inputdata.Dock = System.Windows.Forms.DockStyle.Fill
        Me.inputdata.Location = New System.Drawing.Point(0, 0)
        Me.inputdata.Name = "inputdata"
        Me.inputdata.SelectedIndex = 0
        Me.inputdata.Size = New System.Drawing.Size(1284, 746)
        Me.inputdata.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight
        Me.inputdata.TabIndex = 5
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.modeldescription)
        Me.TabPage1.Controls.Add(Me.Modelname)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.commentaires)
        Me.TabPage1.Controls.Add(Me.ETgridinput)
        Me.TabPage1.Controls.Add(Me.Save_ETdata)
        Me.TabPage1.Controls.Add(Me.Load_from_ecopath)
        Me.TabPage1.Controls.Add(Me.Button1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1276, 720)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Input data"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 58)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(149, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Model name and description : "
        '
        'modeldescription
        '
        Me.modeldescription.Location = New System.Drawing.Point(7, 111)
        Me.modeldescription.Multiline = True
        Me.modeldescription.Name = "modeldescription"
        Me.modeldescription.Size = New System.Drawing.Size(228, 45)
        Me.modeldescription.TabIndex = 7
        '
        'Modelname
        '
        Me.Modelname.Location = New System.Drawing.Point(7, 74)
        Me.Modelname.Name = "Modelname"
        Me.Modelname.Size = New System.Drawing.Size(228, 20)
        Me.Modelname.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(238, 58)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Comments :"
        '
        'commentaires
        '
        Me.commentaires.AcceptsReturn = True
        Me.commentaires.Location = New System.Drawing.Point(241, 74)
        Me.commentaires.Multiline = True
        Me.commentaires.Name = "commentaires"
        Me.commentaires.Size = New System.Drawing.Size(569, 82)
        Me.commentaires.TabIndex = 4
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.smooth_pdf)
        Me.TabPage2.Controls.Add(Me.smooth_graph)
        Me.TabPage2.Controls.Add(Me.datasmooth)
        Me.TabPage2.Controls.Add(Me.GroupBox1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1276, 720)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Smooth parameters"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'smooth_pdf
        '
        Me.smooth_pdf.Location = New System.Drawing.Point(24, 190)
        Me.smooth_pdf.MinimumSize = New System.Drawing.Size(20, 20)
        Me.smooth_pdf.Name = "smooth_pdf"
        Me.smooth_pdf.Size = New System.Drawing.Size(789, 425)
        Me.smooth_pdf.TabIndex = 8
        Me.smooth_pdf.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        Me.smooth_pdf.Visible = False
        '
        'smooth_graph
        '
        Me.smooth_graph.AutoSize = True
        Me.smooth_graph.Location = New System.Drawing.Point(769, 16)
        Me.smooth_graph.Name = "smooth_graph"
        Me.smooth_graph.Size = New System.Drawing.Size(84, 17)
        Me.smooth_graph.TabIndex = 7
        Me.smooth_graph.Text = "View graphs"
        Me.smooth_graph.UseVisualStyleBackColor = True
        '
        'datasmooth
        '
        Me.datasmooth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.datasmooth.Location = New System.Drawing.Point(25, 190)
        Me.datasmooth.Name = "datasmooth"
        Me.datasmooth.Size = New System.Drawing.Size(770, 352)
        Me.datasmooth.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.Reset_smooth)
        Me.GroupBox1.Controls.Add(Me.parameters_cst)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.Button2)
        Me.GroupBox1.Controls.Add(Me.type_smooth3)
        Me.GroupBox1.Controls.Add(Me.type_smooth2)
        Me.GroupBox1.Controls.Add(Me.type_smooth1)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(703, 163)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Smooth type"
        '
        'Reset_smooth
        '
        Me.Reset_smooth.Location = New System.Drawing.Point(420, 116)
        Me.Reset_smooth.Name = "Reset_smooth"
        Me.Reset_smooth.Size = New System.Drawing.Size(150, 28)
        Me.Reset_smooth.TabIndex = 10
        Me.Reset_smooth.Text = "Reset parameters"
        Me.Reset_smooth.UseVisualStyleBackColor = True
        Me.Reset_smooth.UseWaitCursor = True
        '
        'parameters_cst
        '
        Me.parameters_cst.Controls.Add(Me.Label6)
        Me.parameters_cst.Controls.Add(Me.smooth_param_1)
        Me.parameters_cst.Location = New System.Drawing.Point(282, 10)
        Me.parameters_cst.Name = "parameters_cst"
        Me.parameters_cst.Size = New System.Drawing.Size(288, 102)
        Me.parameters_cst.TabIndex = 9
        Me.parameters_cst.TabStop = False
        Me.parameters_cst.Text = "parameters"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(31, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(99, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Smooth parameter :"
        '
        'smooth_param_1
        '
        Me.smooth_param_1.Location = New System.Drawing.Point(179, 23)
        Me.smooth_param_1.Mask = "0.##"
        Me.smooth_param_1.Name = "smooth_param_1"
        Me.smooth_param_1.Size = New System.Drawing.Size(100, 20)
        Me.smooth_param_1.TabIndex = 4
        Me.smooth_param_1.Text = "012"
        Me.smooth_param_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.decalage)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.smooth_param)
        Me.GroupBox2.Location = New System.Drawing.Point(282, 10)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(288, 102)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "parameters"
        Me.GroupBox2.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(31, 67)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(109, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Decalage parameter :"
        '
        'decalage
        '
        Me.decalage.Location = New System.Drawing.Point(179, 67)
        Me.decalage.Mask = "0.##"
        Me.decalage.Name = "decalage"
        Me.decalage.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.decalage.Size = New System.Drawing.Size(100, 20)
        Me.decalage.TabIndex = 7
        Me.decalage.Text = "095"
        Me.decalage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Smooth parameter :"
        '
        'smooth_param
        '
        Me.smooth_param.Location = New System.Drawing.Point(179, 23)
        Me.smooth_param.Mask = "0.##"
        Me.smooth_param.Name = "smooth_param"
        Me.smooth_param.Size = New System.Drawing.Size(100, 20)
        Me.smooth_param.TabIndex = 4
        Me.smooth_param.Text = "007"
        Me.smooth_param.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Button2
        '
        Me.Button2.Enabled = False
        Me.Button2.Location = New System.Drawing.Point(22, 116)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(223, 28)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "Create smooth"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'type_smooth3
        '
        Me.type_smooth3.AutoSize = True
        Me.type_smooth3.Location = New System.Drawing.Point(22, 66)
        Me.type_smooth3.Name = "type_smooth3"
        Me.type_smooth3.Size = New System.Drawing.Size(118, 17)
        Me.type_smooth3.TabIndex = 2
        Me.type_smooth3.Text = "Lognorm Sigma =OI"
        Me.type_smooth3.UseVisualStyleBackColor = True
        '
        'type_smooth2
        '
        Me.type_smooth2.AutoSize = True
        Me.type_smooth2.Location = New System.Drawing.Point(22, 43)
        Me.type_smooth2.Name = "type_smooth2"
        Me.type_smooth2.Size = New System.Drawing.Size(174, 17)
        Me.type_smooth2.TabIndex = 1
        Me.type_smooth2.Text = "Function defined lognorm sigma"
        Me.type_smooth2.UseVisualStyleBackColor = True
        '
        'type_smooth1
        '
        Me.type_smooth1.AutoSize = True
        Me.type_smooth1.Checked = True
        Me.type_smooth1.Location = New System.Drawing.Point(22, 20)
        Me.type_smooth1.Name = "type_smooth1"
        Me.type_smooth1.Size = New System.Drawing.Size(139, 17)
        Me.type_smooth1.TabIndex = 0
        Me.type_smooth1.TabStop = True
        Me.type_smooth1.Text = "Constant lognorm Sigma"
        Me.type_smooth1.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.result_pdf)
        Me.TabPage3.Controls.Add(Me.getgraphs)
        Me.TabPage3.Controls.Add(Me.Button3)
        Me.TabPage3.Controls.Add(Me.panel_result)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(1276, 720)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "ET transpose"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'result_pdf
        '
        Me.result_pdf.Location = New System.Drawing.Point(24, 127)
        Me.result_pdf.MinimumSize = New System.Drawing.Size(20, 20)
        Me.result_pdf.Name = "result_pdf"
        Me.result_pdf.Size = New System.Drawing.Size(789, 491)
        Me.result_pdf.TabIndex = 6
        Me.result_pdf.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        Me.result_pdf.Visible = False
        '
        'getgraphs
        '
        Me.getgraphs.AutoSize = True
        Me.getgraphs.Location = New System.Drawing.Point(769, 16)
        Me.getgraphs.Name = "getgraphs"
        Me.getgraphs.Size = New System.Drawing.Size(84, 17)
        Me.getgraphs.TabIndex = 6
        Me.getgraphs.Text = "View graphs"
        Me.getgraphs.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Enabled = False
        Me.Button3.Location = New System.Drawing.Point(3, 3)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(407, 28)
        Me.Button3.TabIndex = 1
        Me.Button3.Text = "Launch ET transpose"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'panel_result
        '
        Me.panel_result.Controls.Add(Me.TabPage4)
        Me.panel_result.Controls.Add(Me.TabPage5)
        Me.panel_result.Controls.Add(Me.TabPage6)
        Me.panel_result.Controls.Add(Me.TabPage7)
        Me.panel_result.Controls.Add(Me.TabPage8)
        Me.panel_result.Controls.Add(Me.Y)
        Me.panel_result.Location = New System.Drawing.Point(24, 99)
        Me.panel_result.Name = "panel_result"
        Me.panel_result.SelectedIndex = 0
        Me.panel_result.Size = New System.Drawing.Size(789, 426)
        Me.panel_result.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.grille_ET_main)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(781, 400)
        Me.TabPage4.TabIndex = 0
        Me.TabPage4.Text = "ET_main"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'grille_ET_main
        '
        Me.grille_ET_main.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_ET_main.Location = New System.Drawing.Point(6, 6)
        Me.grille_ET_main.Name = "grille_ET_main"
        Me.grille_ET_main.Size = New System.Drawing.Size(779, 388)
        Me.grille_ET_main.TabIndex = 0
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.grille_biomass)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(781, 400)
        Me.TabPage5.TabIndex = 1
        Me.TabPage5.Text = "Biomass"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'grille_biomass
        '
        Me.grille_biomass.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_biomass.Location = New System.Drawing.Point(6, 6)
        Me.grille_biomass.Name = "grille_biomass"
        Me.grille_biomass.Size = New System.Drawing.Size(779, 388)
        Me.grille_biomass.TabIndex = 0
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.grille_biomass_acc)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(781, 400)
        Me.TabPage6.TabIndex = 2
        Me.TabPage6.Text = "Biomasse accessible"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'grille_biomass_acc
        '
        Me.grille_biomass_acc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_biomass_acc.Location = New System.Drawing.Point(7, 7)
        Me.grille_biomass_acc.Name = "grille_biomass_acc"
        Me.grille_biomass_acc.Size = New System.Drawing.Size(778, 387)
        Me.grille_biomass_acc.TabIndex = 0
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.grille_flow_p)
        Me.TabPage7.Location = New System.Drawing.Point(4, 22)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage7.Size = New System.Drawing.Size(781, 400)
        Me.TabPage7.TabIndex = 3
        Me.TabPage7.Text = "Flow_p"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'grille_flow_p
        '
        Me.grille_flow_p.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_flow_p.Location = New System.Drawing.Point(6, 6)
        Me.grille_flow_p.Name = "grille_flow_p"
        Me.grille_flow_p.Size = New System.Drawing.Size(775, 388)
        Me.grille_flow_p.TabIndex = 0
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.grille_flow_p_acc)
        Me.TabPage8.Location = New System.Drawing.Point(4, 22)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(781, 400)
        Me.TabPage8.TabIndex = 4
        Me.TabPage8.Text = "Flow_P_acc"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'grille_flow_p_acc
        '
        Me.grille_flow_p_acc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_flow_p_acc.Location = New System.Drawing.Point(6, 6)
        Me.grille_flow_p_acc.Name = "grille_flow_p_acc"
        Me.grille_flow_p_acc.Size = New System.Drawing.Size(779, 388)
        Me.grille_flow_p_acc.TabIndex = 0
        '
        'Y
        '
        Me.Y.Controls.Add(Me.grille_y)
        Me.Y.Location = New System.Drawing.Point(4, 22)
        Me.Y.Name = "Y"
        Me.Y.Padding = New System.Windows.Forms.Padding(3)
        Me.Y.Size = New System.Drawing.Size(781, 400)
        Me.Y.TabIndex = 5
        Me.Y.Text = "Y"
        Me.Y.UseVisualStyleBackColor = True
        '
        'grille_y
        '
        Me.grille_y.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_y.Location = New System.Drawing.Point(3, 6)
        Me.grille_y.Name = "grille_y"
        Me.grille_y.Size = New System.Drawing.Size(782, 391)
        Me.grille_y.TabIndex = 0
        '
        'TabPage9
        '
        Me.TabPage9.AutoScroll = True
        Me.TabPage9.Controls.Add(Me.reset_param_diag)
        Me.TabPage9.Controls.Add(Me.GroupBox3)
        Me.TabPage9.Controls.Add(Me.result_pdf_et_diag)
        Me.TabPage9.Controls.Add(Me.tabgrille_flow_mf)
        Me.TabPage9.Controls.Add(Me.getgraph_diag)
        Me.TabPage9.Controls.Add(Me.Button4)
        Me.TabPage9.Location = New System.Drawing.Point(4, 22)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage9.Size = New System.Drawing.Size(1276, 720)
        Me.TabPage9.TabIndex = 3
        Me.TabPage9.Text = "ET diagnosis"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'reset_param_diag
        '
        Me.reset_param_diag.Location = New System.Drawing.Point(436, 3)
        Me.reset_param_diag.Name = "reset_param_diag"
        Me.reset_param_diag.Size = New System.Drawing.Size(150, 28)
        Me.reset_param_diag.TabIndex = 11
        Me.reset_param_diag.Text = "Reset parameters"
        Me.reset_param_diag.UseVisualStyleBackColor = True
        Me.reset_param_diag.UseWaitCursor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox3.Controls.Add(Me.mull_eff)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.formd)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.beta)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.TopD)
        Me.GroupBox3.Location = New System.Drawing.Point(16, 37)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(570, 90)
        Me.GroupBox3.TabIndex = 11
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "parameters"
        '
        'mull_eff
        '
        Me.mull_eff.Location = New System.Drawing.Point(262, 54)
        Me.mull_eff.Name = "mull_eff"
        Me.mull_eff.Size = New System.Drawing.Size(297, 20)
        Me.mull_eff.TabIndex = 11
        Me.mull_eff.Text = "0.0,0.2,0.4,0.7,1.0,1.5,2.0,2.5,3.0,4.0,5.0"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(210, 54)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(46, 13)
        Me.Label9.TabIndex = 10
        Me.Label9.Text = "MulEff  :"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(210, 22)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(47, 13)
        Me.Label8.TabIndex = 9
        Me.Label8.Text = "FormD  :"
        '
        'formd
        '
        Me.formd.Location = New System.Drawing.Point(262, 18)
        Me.formd.Mask = "0.##"
        Me.formd.Name = "formd"
        Me.formd.Size = New System.Drawing.Size(178, 20)
        Me.formd.TabIndex = 8
        Me.formd.Text = "05"
        Me.formd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(31, 58)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(29, 13)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Beta"
        '
        'beta
        '
        Me.beta.Location = New System.Drawing.Point(101, 55)
        Me.beta.Mask = "0.##"
        Me.beta.Name = "beta"
        Me.beta.Size = New System.Drawing.Size(87, 20)
        Me.beta.TabIndex = 6
        Me.beta.Text = "01"
        Me.beta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(31, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Top down  :"
        '
        'TopD
        '
        Me.TopD.Location = New System.Drawing.Point(101, 22)
        Me.TopD.Mask = "0.##"
        Me.TopD.Name = "TopD"
        Me.TopD.Size = New System.Drawing.Size(87, 20)
        Me.TopD.TabIndex = 4
        Me.TopD.Text = "02"
        Me.TopD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'result_pdf_et_diag
        '
        Me.result_pdf_et_diag.Location = New System.Drawing.Point(25, 145)
        Me.result_pdf_et_diag.MinimumSize = New System.Drawing.Size(20, 20)
        Me.result_pdf_et_diag.Name = "result_pdf_et_diag"
        Me.result_pdf_et_diag.Size = New System.Drawing.Size(788, 480)
        Me.result_pdf_et_diag.TabIndex = 8
        Me.result_pdf_et_diag.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        Me.result_pdf_et_diag.Visible = False
        '
        'tabgrille_flow_mf
        '
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage10)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage11)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage12)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage13)
        Me.tabgrille_flow_mf.Location = New System.Drawing.Point(25, 145)
        Me.tabgrille_flow_mf.Name = "tabgrille_flow_mf"
        Me.tabgrille_flow_mf.SelectedIndex = 0
        Me.tabgrille_flow_mf.Size = New System.Drawing.Size(788, 392)
        Me.tabgrille_flow_mf.TabIndex = 10
        '
        'TabPage10
        '
        Me.TabPage10.Controls.Add(Me.grille_ET_main_diagnose)
        Me.TabPage10.Location = New System.Drawing.Point(4, 22)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage10.Size = New System.Drawing.Size(780, 366)
        Me.TabPage10.TabIndex = 0
        Me.TabPage10.Text = "ET_Main_diagnose"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'grille_ET_main_diagnose
        '
        Me.grille_ET_main_diagnose.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_ET_main_diagnose.Location = New System.Drawing.Point(6, 6)
        Me.grille_ET_main_diagnose.Name = "grille_ET_main_diagnose"
        Me.grille_ET_main_diagnose.Size = New System.Drawing.Size(778, 388)
        Me.grille_ET_main_diagnose.TabIndex = 0
        '
        'TabPage11
        '
        Me.TabPage11.Controls.Add(Me.grille_biom_mf)
        Me.TabPage11.Location = New System.Drawing.Point(4, 22)
        Me.TabPage11.Name = "TabPage11"
        Me.TabPage11.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage11.Size = New System.Drawing.Size(780, 366)
        Me.TabPage11.TabIndex = 1
        Me.TabPage11.Text = "BIOM_MF"
        Me.TabPage11.UseVisualStyleBackColor = True
        '
        'grille_biom_mf
        '
        Me.grille_biom_mf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_biom_mf.Location = New System.Drawing.Point(6, 6)
        Me.grille_biom_mf.Name = "grille_biom_mf"
        Me.grille_biom_mf.Size = New System.Drawing.Size(775, 388)
        Me.grille_biom_mf.TabIndex = 0
        '
        'TabPage12
        '
        Me.TabPage12.Controls.Add(Me.grille_catches)
        Me.TabPage12.Location = New System.Drawing.Point(4, 22)
        Me.TabPage12.Name = "TabPage12"
        Me.TabPage12.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage12.Size = New System.Drawing.Size(780, 366)
        Me.TabPage12.TabIndex = 2
        Me.TabPage12.Text = "Catches"
        Me.TabPage12.UseVisualStyleBackColor = True
        '
        'grille_catches
        '
        Me.grille_catches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_catches.Location = New System.Drawing.Point(7, 7)
        Me.grille_catches.Name = "grille_catches"
        Me.grille_catches.Size = New System.Drawing.Size(777, 387)
        Me.grille_catches.TabIndex = 0
        '
        'TabPage13
        '
        Me.TabPage13.Controls.Add(Me.grille_flow_mf)
        Me.TabPage13.Location = New System.Drawing.Point(4, 22)
        Me.TabPage13.Name = "TabPage13"
        Me.TabPage13.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage13.Size = New System.Drawing.Size(780, 366)
        Me.TabPage13.TabIndex = 3
        Me.TabPage13.Text = "Flow_MF"
        Me.TabPage13.UseVisualStyleBackColor = True
        '
        'grille_flow_mf
        '
        Me.grille_flow_mf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grille_flow_mf.Location = New System.Drawing.Point(6, 6)
        Me.grille_flow_mf.Name = "grille_flow_mf"
        Me.grille_flow_mf.Size = New System.Drawing.Size(778, 388)
        Me.grille_flow_mf.TabIndex = 0
        '
        'getgraph_diag
        '
        Me.getgraph_diag.AutoSize = True
        Me.getgraph_diag.Location = New System.Drawing.Point(769, 16)
        Me.getgraph_diag.Name = "getgraph_diag"
        Me.getgraph_diag.Size = New System.Drawing.Size(84, 17)
        Me.getgraph_diag.TabIndex = 9
        Me.getgraph_diag.Text = "View graphs"
        Me.getgraph_diag.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Enabled = False
        Me.Button4.Location = New System.Drawing.Point(3, 3)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(407, 28)
        Me.Button4.TabIndex = 7
        Me.Button4.Text = "Launch ET diagnosis"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'autre
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1284, 746)
        Me.Controls.Add(Me.inputdata)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "autre"
        Me.Text = "EcoTroph plugin"
        CType(Me.ETgridinput, System.ComponentModel.ISupportInitialize).EndInit()
        Me.inputdata.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.datasmooth, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.parameters_cst.ResumeLayout(False)
        Me.parameters_cst.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.panel_result.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        CType(Me.grille_ET_main, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage5.ResumeLayout(False)
        CType(Me.grille_biomass, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage6.ResumeLayout(False)
        CType(Me.grille_biomass_acc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage7.ResumeLayout(False)
        CType(Me.grille_flow_p, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage8.ResumeLayout(False)
        CType(Me.grille_flow_p_acc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Y.ResumeLayout(False)
        CType(Me.grille_y, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage9.ResumeLayout(False)
        Me.TabPage9.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.tabgrille_flow_mf.ResumeLayout(False)
        Me.TabPage10.ResumeLayout(False)
        CType(Me.grille_ET_main_diagnose, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage11.ResumeLayout(False)
        CType(Me.grille_biom_mf, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage12.ResumeLayout(False)
        CType(Me.grille_catches, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage13.ResumeLayout(False)
        CType(Me.grille_flow_mf, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ETgridinput As System.Windows.Forms.DataGridView
    Friend WithEvents Load_from_ecopath As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Save_ETdata As System.Windows.Forms.Button
    Friend WithEvents Group_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TTL As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Biomass As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Production As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents accessibilty As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OI As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents inputdata As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents type_smooth1 As System.Windows.Forms.RadioButton
    Friend WithEvents type_smooth3 As System.Windows.Forms.RadioButton
    Friend WithEvents type_smooth2 As System.Windows.Forms.RadioButton
    Friend WithEvents smooth_param As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents datasmooth As System.Windows.Forms.DataGridView
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents panel_result As System.Windows.Forms.TabControl
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents grille_ET_main As System.Windows.Forms.DataGridView
    Friend WithEvents grille_biomass As System.Windows.Forms.DataGridView
    Friend WithEvents grille_biomass_acc As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents grille_flow_p As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents grille_flow_p_acc As System.Windows.Forms.DataGridView
    Friend WithEvents Y As System.Windows.Forms.TabPage
    Friend WithEvents grille_y As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Friend WithEvents getgraphs As System.Windows.Forms.CheckBox
    Friend WithEvents result_pdf As System.Windows.Forms.WebBrowser
    Friend WithEvents result_pdf_et_diag As System.Windows.Forms.WebBrowser
    Friend WithEvents getgraph_diag As System.Windows.Forms.CheckBox
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents tabgrille_flow_mf As System.Windows.Forms.TabControl
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents grille_ET_main_diagnose As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage11 As System.Windows.Forms.TabPage
    Friend WithEvents grille_biom_mf As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage12 As System.Windows.Forms.TabPage
    Friend WithEvents grille_catches As System.Windows.Forms.DataGridView
    Friend WithEvents TabPage13 As System.Windows.Forms.TabPage
    Friend WithEvents grille_flow_mf As System.Windows.Forms.DataGridView
    Friend WithEvents smooth_pdf As System.Windows.Forms.WebBrowser
    Friend WithEvents smooth_graph As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents commentaires As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents modeldescription As System.Windows.Forms.TextBox
    Friend WithEvents Modelname As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents decalage As System.Windows.Forms.MaskedTextBox
    Friend WithEvents parameters_cst As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents smooth_param_1 As System.Windows.Forms.MaskedTextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TopD As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents formd As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents beta As System.Windows.Forms.MaskedTextBox
    Friend WithEvents mull_eff As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Reset_smooth As System.Windows.Forms.Button
    Friend WithEvents reset_param_diag As System.Windows.Forms.Button
End Class
