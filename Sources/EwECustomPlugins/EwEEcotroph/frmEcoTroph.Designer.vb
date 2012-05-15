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

Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcotroph
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotroph))
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.modeldescription = New System.Windows.Forms.TextBox()
        Me.Modelname = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.commentaires = New System.Windows.Forms.TextBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.smooth_graph = New System.Windows.Forms.CheckBox()
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
        Me.smooth_pdf = New System.Windows.Forms.WebBrowser()
        Me.datasmooth = New System.Windows.Forms.DataGridView()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.getgraphs = New System.Windows.Forms.CheckBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.panel_result = New System.Windows.Forms.TabControl()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.grille_ET_main = New System.Windows.Forms.DataGridView()
        Me.result_pdf = New System.Windows.Forms.WebBrowser()
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
        Me.getgraph_diag = New System.Windows.Forms.CheckBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.tabgrille_flow_mf = New System.Windows.Forms.TabControl()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.grille_ET_main_diagnose = New System.Windows.Forms.DataGridView()
        Me.result_pdf_et_diag = New System.Windows.Forms.WebBrowser()
        Me.TabPage11 = New System.Windows.Forms.TabPage()
        Me.grille_biom_mf = New System.Windows.Forms.DataGridView()
        Me.TabPage12 = New System.Windows.Forms.TabPage()
        Me.grille_catches = New System.Windows.Forms.DataGridView()
        Me.TabPage13 = New System.Windows.Forms.TabPage()
        Me.grille_flow_mf = New System.Windows.Forms.DataGridView()
        CType(Me.ETgridinput, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.inputdata.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.parameters_cst.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.datasmooth, System.ComponentModel.ISupportInitialize).BeginInit()
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
        resources.ApplyResources(Me.ETgridinput, "ETgridinput")
        Me.ETgridinput.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        Me.ETgridinput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.ETgridinput.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Group_name, Me.TTL, Me.Biomass, Me.Production, Me.accessibilty, Me.OI})
        Me.ETgridinput.Name = "ETgridinput"
        '
        'Group_name
        '
        resources.ApplyResources(Me.Group_name, "Group_name")
        Me.Group_name.Name = "Group_name"
        '
        'TTL
        '
        resources.ApplyResources(Me.TTL, "TTL")
        Me.TTL.Name = "TTL"
        '
        'Biomass
        '
        resources.ApplyResources(Me.Biomass, "Biomass")
        Me.Biomass.Name = "Biomass"
        '
        'Production
        '
        resources.ApplyResources(Me.Production, "Production")
        Me.Production.Name = "Production"
        '
        'accessibilty
        '
        resources.ApplyResources(Me.accessibilty, "accessibilty")
        Me.accessibilty.Name = "accessibilty"
        '
        'OI
        '
        resources.ApplyResources(Me.OI, "OI")
        Me.OI.Name = "OI"
        '
        'Load_from_ecopath
        '
        resources.ApplyResources(Me.Load_from_ecopath, "Load_from_ecopath")
        Me.Load_from_ecopath.Name = "Load_from_ecopath"
        Me.Load_from_ecopath.UseVisualStyleBackColor = True
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Save_ETdata
        '
        resources.ApplyResources(Me.Save_ETdata, "Save_ETdata")
        Me.Save_ETdata.Name = "Save_ETdata"
        Me.Save_ETdata.UseVisualStyleBackColor = True
        '
        'inputdata
        '
        Me.inputdata.Controls.Add(Me.TabPage1)
        Me.inputdata.Controls.Add(Me.TabPage2)
        Me.inputdata.Controls.Add(Me.TabPage3)
        Me.inputdata.Controls.Add(Me.TabPage9)
        resources.ApplyResources(Me.inputdata, "inputdata")
        Me.inputdata.Name = "inputdata"
        Me.inputdata.SelectedIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.TableLayoutPanel1)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.modeldescription)
        Me.TabPage1.Controls.Add(Me.Modelname)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.commentaires)
        Me.TabPage1.Controls.Add(Me.ETgridinput)
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.Load_from_ecopath, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Save_ETdata, 2, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'modeldescription
        '
        resources.ApplyResources(Me.modeldescription, "modeldescription")
        Me.modeldescription.Name = "modeldescription"
        '
        'Modelname
        '
        resources.ApplyResources(Me.Modelname, "Modelname")
        Me.Modelname.Name = "Modelname"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'commentaires
        '
        Me.commentaires.AcceptsReturn = True
        resources.ApplyResources(Me.commentaires, "commentaires")
        Me.commentaires.Name = "commentaires"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.smooth_graph)
        Me.TabPage2.Controls.Add(Me.GroupBox1)
        Me.TabPage2.Controls.Add(Me.smooth_pdf)
        Me.TabPage2.Controls.Add(Me.datasmooth)
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'smooth_graph
        '
        resources.ApplyResources(Me.smooth_graph, "smooth_graph")
        Me.smooth_graph.Name = "smooth_graph"
        Me.smooth_graph.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.Reset_smooth)
        Me.GroupBox1.Controls.Add(Me.parameters_cst)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.Button2)
        Me.GroupBox1.Controls.Add(Me.type_smooth3)
        Me.GroupBox1.Controls.Add(Me.type_smooth2)
        Me.GroupBox1.Controls.Add(Me.type_smooth1)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'Reset_smooth
        '
        resources.ApplyResources(Me.Reset_smooth, "Reset_smooth")
        Me.Reset_smooth.Name = "Reset_smooth"
        Me.Reset_smooth.UseVisualStyleBackColor = True
        Me.Reset_smooth.UseWaitCursor = True
        '
        'parameters_cst
        '
        Me.parameters_cst.Controls.Add(Me.Label6)
        Me.parameters_cst.Controls.Add(Me.smooth_param_1)
        resources.ApplyResources(Me.parameters_cst, "parameters_cst")
        Me.parameters_cst.Name = "parameters_cst"
        Me.parameters_cst.TabStop = False
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'smooth_param_1
        '
        resources.ApplyResources(Me.smooth_param_1, "smooth_param_1")
        Me.smooth_param_1.Name = "smooth_param_1"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.decalage)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.smooth_param)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'decalage
        '
        resources.ApplyResources(Me.decalage, "decalage")
        Me.decalage.Name = "decalage"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'smooth_param
        '
        resources.ApplyResources(Me.smooth_param, "smooth_param")
        Me.smooth_param.Name = "smooth_param"
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.Name = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'type_smooth3
        '
        resources.ApplyResources(Me.type_smooth3, "type_smooth3")
        Me.type_smooth3.Name = "type_smooth3"
        Me.type_smooth3.UseVisualStyleBackColor = True
        '
        'type_smooth2
        '
        resources.ApplyResources(Me.type_smooth2, "type_smooth2")
        Me.type_smooth2.Name = "type_smooth2"
        Me.type_smooth2.UseVisualStyleBackColor = True
        '
        'type_smooth1
        '
        resources.ApplyResources(Me.type_smooth1, "type_smooth1")
        Me.type_smooth1.Checked = True
        Me.type_smooth1.Name = "type_smooth1"
        Me.type_smooth1.TabStop = True
        Me.type_smooth1.UseVisualStyleBackColor = True
        '
        'smooth_pdf
        '
        resources.ApplyResources(Me.smooth_pdf, "smooth_pdf")
        Me.smooth_pdf.MinimumSize = New System.Drawing.Size(20, 20)
        Me.smooth_pdf.Name = "smooth_pdf"
        Me.smooth_pdf.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        '
        'datasmooth
        '
        resources.ApplyResources(Me.datasmooth, "datasmooth")
        Me.datasmooth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.datasmooth.Name = "datasmooth"
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.getgraphs)
        Me.TabPage3.Controls.Add(Me.Button3)
        Me.TabPage3.Controls.Add(Me.panel_result)
        resources.ApplyResources(Me.TabPage3, "TabPage3")
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'getgraphs
        '
        resources.ApplyResources(Me.getgraphs, "getgraphs")
        Me.getgraphs.Name = "getgraphs"
        Me.getgraphs.UseVisualStyleBackColor = True
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'panel_result
        '
        resources.ApplyResources(Me.panel_result, "panel_result")
        Me.panel_result.Controls.Add(Me.TabPage4)
        Me.panel_result.Controls.Add(Me.TabPage5)
        Me.panel_result.Controls.Add(Me.TabPage6)
        Me.panel_result.Controls.Add(Me.TabPage7)
        Me.panel_result.Controls.Add(Me.TabPage8)
        Me.panel_result.Controls.Add(Me.Y)
        Me.panel_result.Name = "panel_result"
        Me.panel_result.SelectedIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.grille_ET_main)
        Me.TabPage4.Controls.Add(Me.result_pdf)
        resources.ApplyResources(Me.TabPage4, "TabPage4")
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'grille_ET_main
        '
        Me.grille_ET_main.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_ET_main, "grille_ET_main")
        Me.grille_ET_main.Name = "grille_ET_main"
        '
        'result_pdf
        '
        resources.ApplyResources(Me.result_pdf, "result_pdf")
        Me.result_pdf.MinimumSize = New System.Drawing.Size(20, 20)
        Me.result_pdf.Name = "result_pdf"
        Me.result_pdf.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.grille_biomass)
        resources.ApplyResources(Me.TabPage5, "TabPage5")
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'grille_biomass
        '
        Me.grille_biomass.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_biomass, "grille_biomass")
        Me.grille_biomass.Name = "grille_biomass"
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.grille_biomass_acc)
        resources.ApplyResources(Me.TabPage6, "TabPage6")
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'grille_biomass_acc
        '
        Me.grille_biomass_acc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_biomass_acc, "grille_biomass_acc")
        Me.grille_biomass_acc.Name = "grille_biomass_acc"
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.grille_flow_p)
        resources.ApplyResources(Me.TabPage7, "TabPage7")
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'grille_flow_p
        '
        Me.grille_flow_p.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_flow_p, "grille_flow_p")
        Me.grille_flow_p.Name = "grille_flow_p"
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.grille_flow_p_acc)
        resources.ApplyResources(Me.TabPage8, "TabPage8")
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'grille_flow_p_acc
        '
        Me.grille_flow_p_acc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_flow_p_acc, "grille_flow_p_acc")
        Me.grille_flow_p_acc.Name = "grille_flow_p_acc"
        '
        'Y
        '
        Me.Y.Controls.Add(Me.grille_y)
        resources.ApplyResources(Me.Y, "Y")
        Me.Y.Name = "Y"
        Me.Y.UseVisualStyleBackColor = True
        '
        'grille_y
        '
        Me.grille_y.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_y, "grille_y")
        Me.grille_y.Name = "grille_y"
        '
        'TabPage9
        '
        resources.ApplyResources(Me.TabPage9, "TabPage9")
        Me.TabPage9.Controls.Add(Me.reset_param_diag)
        Me.TabPage9.Controls.Add(Me.GroupBox3)
        Me.TabPage9.Controls.Add(Me.getgraph_diag)
        Me.TabPage9.Controls.Add(Me.Button4)
        Me.TabPage9.Controls.Add(Me.tabgrille_flow_mf)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'reset_param_diag
        '
        resources.ApplyResources(Me.reset_param_diag, "reset_param_diag")
        Me.reset_param_diag.Name = "reset_param_diag"
        Me.reset_param_diag.UseVisualStyleBackColor = True
        Me.reset_param_diag.UseWaitCursor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.mull_eff)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.formd)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.beta)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.TopD)
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'mull_eff
        '
        resources.ApplyResources(Me.mull_eff, "mull_eff")
        Me.mull_eff.Name = "mull_eff"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'formd
        '
        resources.ApplyResources(Me.formd, "formd")
        Me.formd.Name = "formd"
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'beta
        '
        resources.ApplyResources(Me.beta, "beta")
        Me.beta.Name = "beta"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'TopD
        '
        resources.ApplyResources(Me.TopD, "TopD")
        Me.TopD.Name = "TopD"
        '
        'getgraph_diag
        '
        resources.ApplyResources(Me.getgraph_diag, "getgraph_diag")
        Me.getgraph_diag.Name = "getgraph_diag"
        Me.getgraph_diag.UseVisualStyleBackColor = True
        '
        'Button4
        '
        resources.ApplyResources(Me.Button4, "Button4")
        Me.Button4.Name = "Button4"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'tabgrille_flow_mf
        '
        resources.ApplyResources(Me.tabgrille_flow_mf, "tabgrille_flow_mf")
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage10)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage11)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage12)
        Me.tabgrille_flow_mf.Controls.Add(Me.TabPage13)
        Me.tabgrille_flow_mf.Name = "tabgrille_flow_mf"
        Me.tabgrille_flow_mf.SelectedIndex = 0
        '
        'TabPage10
        '
        Me.TabPage10.Controls.Add(Me.grille_ET_main_diagnose)
        Me.TabPage10.Controls.Add(Me.result_pdf_et_diag)
        resources.ApplyResources(Me.TabPage10, "TabPage10")
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'grille_ET_main_diagnose
        '
        Me.grille_ET_main_diagnose.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_ET_main_diagnose, "grille_ET_main_diagnose")
        Me.grille_ET_main_diagnose.Name = "grille_ET_main_diagnose"
        '
        'result_pdf_et_diag
        '
        resources.ApplyResources(Me.result_pdf_et_diag, "result_pdf_et_diag")
        Me.result_pdf_et_diag.MinimumSize = New System.Drawing.Size(20, 20)
        Me.result_pdf_et_diag.Name = "result_pdf_et_diag"
        Me.result_pdf_et_diag.Url = New System.Uri("about:blank", System.UriKind.Absolute)
        '
        'TabPage11
        '
        Me.TabPage11.Controls.Add(Me.grille_biom_mf)
        resources.ApplyResources(Me.TabPage11, "TabPage11")
        Me.TabPage11.Name = "TabPage11"
        Me.TabPage11.UseVisualStyleBackColor = True
        '
        'grille_biom_mf
        '
        Me.grille_biom_mf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_biom_mf, "grille_biom_mf")
        Me.grille_biom_mf.Name = "grille_biom_mf"
        '
        'TabPage12
        '
        Me.TabPage12.Controls.Add(Me.grille_catches)
        resources.ApplyResources(Me.TabPage12, "TabPage12")
        Me.TabPage12.Name = "TabPage12"
        Me.TabPage12.UseVisualStyleBackColor = True
        '
        'grille_catches
        '
        Me.grille_catches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_catches, "grille_catches")
        Me.grille_catches.Name = "grille_catches"
        '
        'TabPage13
        '
        Me.TabPage13.Controls.Add(Me.grille_flow_mf)
        resources.ApplyResources(Me.TabPage13, "TabPage13")
        Me.TabPage13.Name = "TabPage13"
        Me.TabPage13.UseVisualStyleBackColor = True
        '
        'grille_flow_mf
        '
        Me.grille_flow_mf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.grille_flow_mf, "grille_flow_mf")
        Me.grille_flow_mf.Name = "grille_flow_mf"
        '
        'frmEcotroph
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.inputdata)
        Me.Name = "frmEcotroph"
        CType(Me.ETgridinput, System.ComponentModel.ISupportInitialize).EndInit()
        Me.inputdata.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.parameters_cst.ResumeLayout(False)
        Me.parameters_cst.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.datasmooth, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents grille_ET_main As System.Windows.Forms.DataGridView
End Class
