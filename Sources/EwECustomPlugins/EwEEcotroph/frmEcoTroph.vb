
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
Option Strict On
Imports System.IO
Imports System.Windows.Forms
Imports EcoTroph.cEcotrophPlugin
Imports EwECore
Imports EwEUtils.Utilities
Imports Microsoft.VisualBasic

' ToDo: remove reference to Microsoft.VisualBasic

''' <summary>
''' ToDo: comment this class
''' </summary>
Public Class frmEcotroph

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Load_from_ecopath.Click

        'a retester ou alors tester si les données sont dispo
        EcoTroph.cEcotrophPlugin.etCore.RunEcoPath()

        If Not (IsNothing(ETinputdatafromEP.TL)) Then

            Dim DataGrid As DataGridView = Me.ETgridinput

            For igrp As Integer = 0 To ETinputdatafromEP.TL.Length - 2
                If (DataGrid.RowCount < ETinputdatafromEP.TL.Length) Then
                    DataGrid.Rows.Add()
                End If
                DataGrid.Item(0, igrp).Value() = ETinputdatafromEP.groupname(igrp + 1)
                DataGrid.Item(1, igrp).Value() = ETinputdatafromEP.TL(igrp + 1)
                DataGrid.Item(2, igrp).Value() = ETinputdatafromEP.B(igrp + 1)
                DataGrid.Item(3, igrp).Value() = ETinputdatafromEP.PROD(igrp + 1)
                DataGrid.Item(4, igrp).Value() = ETinputdatafromEP.accessibility(igrp + 1)
                DataGrid.Item(5, igrp).Value() = ETinputdatafromEP.OI(igrp + 1)
            Next
            commentaires.Text = ETinputdata.numfleet.ToString

            DataGrid.ColumnCount = 6 + ETinputdatafromEP.numfleet
            For ifleet As Integer = 0 To ETinputdatafromEP.numfleet - 1
                DataGrid.Columns(6 + ifleet).Name = ETinputdatafromEP.fleetname(ifleet + 1)
                For igrp As Integer = 0 To ETinputdatafromEP.TL.Length - 2
                    DataGrid.Item(6 + ifleet, igrp).Value() = ETinputdatafromEP.catches(ifleet)(igrp + 1)
                Next
            Next

            ETinputdata.numfleet = ETinputdatafromEP.numfleet
            If Not (IsNothing(ETinputdata.comments)) Then commentaires.Text = ETinputdata.comments Else commentaires.Text = ""
            If Not (IsNothing(ETinputdata.ModelName)) Then Modelname.Text = ETinputdata.ModelName Else Modelname.Text = ""
            If Not (IsNothing(ETinputdata.Modeldescription)) Then modeldescription.Text = ETinputdata.Modeldescription Else modeldescription.Text = ""
            Button2.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True

        Else
            ' ToDo: provide a better error
            ' ToDo: globalize this
            Me.ReportMessage("There is no model loaded, we can't find EcoTroph input data")
        End If

        ' frmET.ETgridinput.DataSource = ETinput
        ' frmET.ETgridinput.Show()
    End Sub

    Private Sub Save_ETdata_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save_ETdata.Click
        Dim saveFileDialog1 As New SaveFileDialog()

        ' ToDo: globalize this method
        ' ToDo: obtain file filter from resources

        saveFileDialog1.Filter = "xml files (*.xml)|*.xml"
        saveFileDialog1.Title = "Save an EcoTroph input data file"
        saveFileDialog1.ShowDialog()
        ETinputdata.comments = commentaires.Text
        ETinputdata.ModelName = Modelname.Text
        ETinputdata.Modeldescription = modeldescription.Text

        ' If the file name is not an empty string open it for saving.
        If saveFileDialog1.FileName <> "" Then
            ' Saves the Image via a FileStream created by the OpenFile method.
            Dim writer As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))
            Dim file As New System.IO.StreamWriter(saveFileDialog1.FileName)

            writer.Serialize(file, ETinputdata)
            file.Close()
        End If

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        ' ToDo: globalize this method
        ' ToDo: obtain file filter from resources

        Dim myStream As Stream = Nothing
        Dim openFileDialog1 As New OpenFileDialog()
        Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))
        openFileDialog1.InitialDirectory = "c:\"
        openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try

                Dim file As New System.IO.StreamReader(openFileDialog1.FileName)
                If (openFileDialog1.FileName <> "") Then
                    ETinputdata = CType(reader.Deserialize(file), ETinputtot)
                End If
            Catch Ex As Exception
                ' ToDo: use core messages here!
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If

        If (openFileDialog1.FileName <> "") Then


            Dim DataGrid As DataGridView = Me.ETgridinput
            'List faut une procédure pour afficher cela
            For igrp As Integer = 0 To ETinputdata.TL.Length - 2
                If (DataGrid.RowCount < ETinputdata.TL.Length) Then
                    DataGrid.Rows.Add()
                End If

                DataGrid.Item(0, igrp).Value() = ETinputdata.groupname(igrp + 1)
                DataGrid.Item(1, igrp).Value() = ETinputdata.TL(igrp + 1)
                DataGrid.Item(2, igrp).Value() = ETinputdata.B(igrp + 1)
                DataGrid.Item(3, igrp).Value() = ETinputdata.PROD(igrp + 1)

                If Not (IsNothing(ETinputdata.accessibility)) Then DataGrid.Item(4, igrp).Value() = ETinputdata.accessibility(igrp + 1)
                If Not (IsNothing(ETinputdata.OI)) Then DataGrid.Item(5, igrp).Value() = ETinputdata.OI(igrp + 1)

            Next
            If Not (IsNothing(ETinputdata.comments)) Then commentaires.Text = ETinputdata.comments Else commentaires.Text = ""
            If Not (IsNothing(ETinputdata.ModelName)) Then Modelname.Text = ETinputdata.ModelName Else Modelname.Text = ""
            If Not (IsNothing(ETinputdata.Modeldescription)) Then modeldescription.Text = ETinputdata.Modeldescription Else modeldescription.Text = ""
            DataGrid.ColumnCount = 6 + ETinputdata.numfleet
            For ifleet As Integer = 0 To ETinputdata.numfleet - 1
                DataGrid.Columns(6 + ifleet).Name = ETinputdata.fleetname(ifleet)
                For igrp As Integer = 0 To ETinputdata.TL.Length - 2
                    DataGrid.Item(6 + ifleet, igrp).Value() = ETinputdata.catches(ifleet)(igrp + 1)
                Next

            Next
        End If
        Button2.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
    End Sub

    Private Sub ETgridinput_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ETgridinput.CellContentClick

    End Sub

    Private Sub ETgridinput_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ETgridinput.CellEndEdit

    End Sub

    Private Sub ETgridinput_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ETgridinput.CellValueChanged
        'MsgBox("on est sur " & e.ColumnIndex & e.ColumnIndex)
        If (e.ColumnIndex >= 0 And e.RowIndex >= 0) Then
            '  MsgBox(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).ToString)

            Select Case e.ColumnIndex
                Case 0
                    ETinputdata.groupname(e.RowIndex + 1) = CStr(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                Case 1
                    ETinputdata.TL(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                Case 2
                    ETinputdata.B(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                Case 3
                    ETinputdata.PROD(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                Case 4
                    ETinputdata.accessibility(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                Case 5
                    ETinputdata.OI(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
                    'Then it's fleet catches
                Case Is > 5

                    ETinputdata.catches(e.ColumnIndex - 6)(e.RowIndex + 1) = CSng(Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value)
            End Select


        End If

    End Sub

    Private Sub RadioButton1_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles type_smooth1.CheckedChanged
        Me.GroupBox2.Visible = False
        Me.parameters_cst.Visible = True
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles type_smooth2.CheckedChanged
        Me.GroupBox2.Visible = True
        Me.parameters_cst.Visible = False
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles type_smooth3.CheckedChanged
        Me.GroupBox2.Visible = False
        Me.parameters_cst.Visible = False
    End Sub

    Public Shared Function execute_r(ByVal code As String()) As String
        'Cette fonction execute un code R et renvoie le nom d'un fichier resultat
        Dim myProcess As New Process()
        myProcess.StartInfo.UseShellExecute = False ' A remettre à false

        myProcess.StartInfo.RedirectStandardInput = True
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.RedirectStandardError = True

        ' Get the path that stores user documents.

        'myProcess.StartInfo.UseShellExecute = False
        ' You can start any process, HelloWorld is a do-nothing example.
        'myProcess.StartInfo.FileName = "C:\Program Files\R\R-2.13.2\bin\i386\r.exe"

        myProcess.StartInfo.FileName = Environment.CurrentDirectory() & "\R\bin\i386\r.exe"
        myProcess.StartInfo.Arguments = "--slave"
        myProcess.StartInfo.CreateNoWindow = True


        myProcess.Start()
        Dim myStreamWriter As StreamWriter = myProcess.StandardInput

        For icod As Integer = 0 To code.Count - 1
            myStreamWriter.WriteLine(code(icod))
            Debug.Print(code(icod))
        Next
        myStreamWriter.Close()

        Dim Output As String = myProcess.StandardOutput.ReadToEnd()
        Dim output2 As String = myProcess.StandardError.ReadToEnd()

        myProcess.WaitForExit()

        Return (output2)

    End Function

    Public Shared Function execute_rplot(ByVal code As String()) As String

        'Cette fonction execute un code R et renvoie le nom d'un fichier resultat
        Dim myProcess As New Process()
        myProcess.StartInfo.RedirectStandardInput = False
        myProcess.StartInfo.UseShellExecute = True ' A remettre à false
        myProcess.StartInfo.FileName = Environment.CurrentDirectory() & "\R\bin\i386\r.exe"
        myProcess.StartInfo.Arguments = "--slave"
        myProcess.StartInfo.CreateNoWindow = False
        myProcess.Start()

        For icod As Integer = 0 To code.Count - 1
            My.Computer.Keyboard.SendKeys(code(icod))
            'myStreamWriter.WriteLine(code(icod))
            'MsgBox(myProcess.Threads.Count & "pour " & code(icod))
        Next

        'Dim Output As Object = myProcess.StandardOutput.ReadToEnd()
        'Dim output2 As String = myProcess.StandardError.ReadToEnd()
        'myStreamWriter.Close()

        Return "1" ' ??

    End Function

    Public Shared Function sauve_datagrid_xml(ByVal grille As ETinputtot, ByVal filename As String) As Boolean

        Dim fichier_data_transfert As String = System.IO.Path.GetTempPath() & filename
        Dim writer As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))
        Dim file_data As New System.IO.StreamWriter(System.IO.Path.GetTempPath() & "\" & filename)

        writer.Serialize(file_data, ETinputdata)
        file_data.Close()
        Return True

    End Function


    Public Shared Function charge_grid(ByVal donnees As String(), ByRef grille As DataGridView) As Integer

        Dim tab_trans(,) As String
        Dim uneligne() As String

        donnees(0) = vbTab & donnees(0)
        Dim nbl As Integer = donnees.Length
        Dim nbcol As Integer = (donnees(0).Split(CChar(cStringUtils.vbTab)).Length)

        ReDim tab_trans(nbcol, nbl)
        ReDim uneligne(nbcol)
        Dim deci_sep As String

        'Une astuce pour obtenir le sep décimal
        deci_sep = Mid$(CStr(1 / 2), 2, 1)

        ' La partie suivante est a mettre en fonction(donnees as data,sheet as )
        grille.ColumnCount = nbcol

        For igrp As Integer = 0 To nbl - 1

            If (grille.Rows.Count < nbl) Then grille.Rows.Add()

            uneligne = donnees(igrp).Split(CChar(cStringUtils.vbTab))
            For ielt As Integer = 0 To (uneligne.Count - 1)

                uneligne(ielt) = Replace(uneligne(ielt), ".", deci_sep)
                tab_trans(ielt, igrp) = uneligne(ielt)
                ' Ajout de l'arrondi mle 09/05/2012 sur 4 chiffre
                If (IsNumeric(uneligne(ielt))) Then
                    grille.Item(ielt, igrp).Value = Math.Round(CDbl(uneligne(ielt)), 4)
                Else
                    grille.Item(ielt, igrp).Value = uneligne(ielt)
                End If
                'grille.Item(ielt, igrp).Value = uneligne(ielt) si je veux vraiment voir les vrai chiffre 
            Next
        Next
        grille.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.Gray
        grille.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.Gray
        grille.RowCount = nbl
        Return (vbOK)
    End Function


    Public Shared Function get_params(ByVal type_smooth As Integer, Optional ByVal smooth_parameter As Double = Nothing, Optional ByVal decalage As Double = Nothing) As String
        'Cette fonction doit récupérer les paramètre du smooth
        Dim output2 As String = ""

        Select Case type_smooth
            Case 1
                output2 = ",sigmaLN_cst=" & cStringUtils.FormatNumber(smooth_parameter)
            Case 2
                output2 = ",smooth_type=2,smooth_param=" & cStringUtils.FormatNumber(smooth_parameter) & ",shift=" & cStringUtils.FormatNumber(decalage)
            Case 3
                output2 = ",smooth_type=3"
        End Select
        Return (output2)

    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'On commence par sauver le fichier de données 

        Dim commandes() As String
        Dim fichierpdf_init As String = "mysmooth.pdf"
        Dim fichier_erase As String

        Dim fichierpdf As String = ""
        Dim fichier_svg As String = "mysmooth.svg"
        Dim fichier_data_transfert As String = "transfert_data.xml"
        'Dim type_smooth As Integer

        Dim test As String
        test = Replace(System.IO.Path.GetTempPath() & fichierpdf_init, "\", "/")
        test = Replace(test, " ", "%20")
        If smooth_pdf.Url.AbsoluteUri = "file:///" & test Then

            fichierpdf = "2" & fichierpdf_init
            fichier_erase = fichierpdf_init
        Else
            fichierpdf = fichierpdf_init
            fichier_erase = "2" & fichierpdf_init
        End If
        smooth_pdf.GoHome()

        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

        'on charge les différents paramètres du create.smooth
        Dim param_pas As String = ""
        ' JS: Option strict on, make sure numbers are correctly parsed
        Try
            If (type_smooth1.Checked) Then
                param_pas = get_params(1, Double.Parse(smooth_param_1.Text))
            ElseIf (type_smooth2.Checked) Then
                param_pas = get_params(2, Double.Parse(smooth_param.Text), Double.Parse(decalage.Text))
            Else
                Debug.Assert(type_smooth3.Checked)
                param_pas = get_params(3)
            End If
        Catch ex As Exception
            cLog.Write(ex, "Ecotroph.Button2_Click")
        End Try

        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)
        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)

        'Le code R en lui même
        Dim fichier As String = System.IO.Path.GetTempPath() & "transfert.txt"
        'First i need to delete past files
        If My.Computer.FileSystem.FileExists(fichier) Then My.Computer.FileSystem.DeleteFile(fichier)


        fichier = Replace(fichier, "\", "\\")
        Dim pathfile As String = System.IO.Path.GetTempPath()
        ReDim commandes(9)
        commandes(0) = ""
        commandes(1) = "library(EcoTroph)"
        commandes(2) = "ecopath<-read.ecopath.model('" & Replace(System.IO.Path.GetTempPath() & "\" & fichier_data_transfert, "\", "\\") & "')"
        commandes(3) = "A<-create.smooth(ecopath" & param_pas & ")"
        commandes(4) = ""
        commandes(5) = "write.table(A, file ='" & fichier & "', sep = '\t',quote=FALSE)"
        commandes(6) = "pdf(file='" & Replace(System.IO.Path.GetTempPath(), "\", "\\") & fichierpdf & "')"

        'commandes(4) = "library(Cairo)"
        'commandes(5) = "Cairo(600, 600, File='" & fichier_svg & "', Type = 'svg', bg = 'white')"
        commandes(7) = "plot_smooth(A)"
        commandes(8) = "dev.off()"
        commandes(9) = "quit('yes')"

        'on execute ce code R
        Dim output2 As String = execute_r(commandes)

        If (Len(output2) > 2) Then
            'MsgBox(output2)
        End If

        smooth_pdf.Navigate(System.IO.Path.GetTempPath() & fichierpdf)


        'smooth_pdf.Refresh()
        If My.Computer.FileSystem.FileExists(fichier) Then
            Dim recup() As String = File.ReadAllLines(fichier)

            charge_grid(recup, datasmooth)
        Else
            ' ToDo: provide a better error
            ' ToDo: globalize this
            Me.ReportMessage("The procedure did not yield any results")
        End If

        Cursor.Current = Cursors.Default
        'Test de la partie graphique, pour voir
    End Sub

    Private Sub pas_MaskInputRejected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MaskInputRejectedEventArgs)

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim commandes() As String

        Dim fichier_data_transfert As String = "transfert_data.xml"
        Dim fichierpdf_init As String = "myplot.pdf"
        Dim fichierpdf As String = ""
        'Dim islocked As Boolean
        'Dim lurl As String
        'Dim type_smooth As Integer

        Dim test As String
        test = Replace(System.IO.Path.GetTempPath() & fichierpdf_init, "\", "/")
        test = Replace(test, " ", "%20")
        If result_pdf.Url.AbsoluteUri = "file:///" & test Then fichierpdf = "2" & fichierpdf_init Else fichierpdf = fichierpdf_init

        result_pdf.GoHome()

        ' ToDo: Use cApplicationStatusNotifier
        Cursor.Current = Cursors.WaitCursor

        Try

            'Juste pour attendre que le composant web ne bloque pas le fichier qui doit être mis à jour
            'on charge les différents paramètres du create.smooth
            Dim param_pas As String

            ' JS: Option strict on, make sure numbers are correctly parsed
            Try
                If (type_smooth1.Checked) Then
                    param_pas = get_params(1, Double.Parse(smooth_param_1.Text))
                ElseIf (type_smooth2.Checked) Then
                    param_pas = get_params(2, Double.Parse(smooth_param.Text), Double.Parse(decalage.Text))
                Else
                    Debug.Assert(type_smooth3.Checked)
                    param_pas = get_params(3)
                End If
            Catch ex As Exception
                cLog.Write(ex, "Ecotroph.Button3_Click")
            End Try

            ' MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)

            If (My.Computer.FileSystem.FileExists(fichierpdf)) Then My.Computer.FileSystem.DeleteFile(Environment.CurrentDirectory() & "\" & fichierpdf)

            sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

            'on charge les différents paramètres du create.smooth


            'Le code R en lui même
            Dim fichier As String = System.IO.Path.GetTempPath() & "\transfert.txt"
            If My.Computer.FileSystem.FileExists(fichier) Then My.Computer.FileSystem.DeleteFile(fichier)

            fichier = fichier.Replace("\", "\\")
            Dim pathfile As String = Environment.CurrentDirectory()
            ReDim commandes(21)
            commandes(0) = "options(warn=0)"
            commandes(1) = "library(EcoTroph)"
            commandes(2) = "ecopath<-read.ecopath.model('" & CStr(System.IO.Path.GetTempPath() & "\" & fichier_data_transfert).Replace("\", "\\") & "')"
            commandes(3) = "A<-create.ETmain(ecopath" & param_pas & ")"
            commandes(4) = "write.table(A$ET_Main, file ='" & fichier & "', sep = '\t',quote=FALSE)"
            commandes(5) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
            commandes(6) = "write.table(A$biomass, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
            commandes(7) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
            commandes(8) = "write.table(A$biomass_acc, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
            commandes(9) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
            commandes(10) = "write.table(A$flowP, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
            commandes(11) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
            commandes(12) = "write.table(A$flowP_acc, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
            commandes(13) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
            commandes(14) = "write.table(A$Y, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
            commandes(15) = "pdf(file='" & System.IO.Path.GetTempPath().Replace("\", "\\") & fichierpdf & "')"
            commandes(16) = "plot_ETmain(A)"
            commandes(17) = "dev.off()"

            commandes(18) = " "
            commandes(19) = " "
            commandes(20) = " "
            commandes(21) = " quit('yes')"

            'on execute ce code R
            Dim output2 As String = execute_r(commandes)

            If (Len(output2) > 2) Then
                ' MsgBox(output2)
            End If

            result_pdf.Navigate(System.IO.Path.GetTempPath() & fichierpdf)

            If My.Computer.FileSystem.FileExists(fichier) Then
                Dim recup() As String = File.ReadAllLines(fichier)

                Dim totales As String = Join(recup, vbNewLine)
                Dim matrices() As String = Split(totales, "-----")

                For imat As Integer = 0 To matrices.Count

                    If (imat = 0) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_ET_main)
                    If (imat = 1) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_biomass)
                    If (imat = 2) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_biomass_acc)
                    If (imat = 3) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_flow_p)
                    If (imat = 4) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_flow_p_acc)
                    If (imat = 5) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_y)
                Next
            Else
                ' ToDo: provide a better error
                ' ToDo: globalize this
                Me.ReportMessage("The procedure did not yield any results.")
            End If

        Catch ex As Exception
            cLog.Write(ex)
        End Try

        ' ToDo: Use cApplicationStatusNotifier
        Cursor.Current = Cursors.Default

    End Sub

    Private Sub Button4_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim commandes() As String
        Dim type_smooth As Integer

        Dim fichier_data_transfert As String = "transfert_data.xml"

        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

        result_pdf.Navigate(Environment.CurrentDirectory() & "\null.html")

        'MsgBox("attente")
        My.Computer.FileSystem.DeleteFile(Environment.CurrentDirectory() & "\myplot.pdf")
        If (type_smooth1.Checked) Then type_smooth = 1
        If (type_smooth2.Checked) Then type_smooth = 2
        If (type_smooth3.Checked) Then type_smooth = 3

        Dim param_pas As String = get_params(type_smooth, Double.Parse(smooth_param.Text), Double.Parse(decalage.Text))
        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)

        Try
            ReDim commandes(9)
            commandes(0) = "pdf(file='myplot.pdf')"
            commandes(1) = "library(EcoTroph)"
            commandes(2) = "ecopath<-read.ecopath.model('" & Replace(fichier_data_transfert, "\", "\\") & "')"
            commandes(3) = "plot.ETmain(create.ETmain(ecopath))"
            commandes(4) = "dev.off()"
            commandes(5) = " "
            commandes(6) = " "
            commandes(7) = " "
            commandes(8) = " "
            commandes(9) = " quit('yes')"
            Dim output2 As String = execute_r(commandes)
            'myim = New Drawing.Bitmap(Environment.CurrentDirectory() & "\myplot.png")

            'Process.Start(Environment.CurrentDirectory() & "\myplot.pdf")
            'graph_results.Image = CType(myim, Drawing.Bitmap)
            result_pdf.Navigate(Environment.CurrentDirectory() & "\myplot.pdf")
        Catch ex As Exception
            cLog.Write(ex, "")
        End Try

    End Sub

    Private Sub getgraphs_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getgraphs.CheckedChanged
        'If getgraphs.Checked = True Then
        '    result_pdf.Visible = True
        'Else
        '    result_pdf.Visible = False
        'End If
        result_pdf.Visible = getgraphs.Checked
    End Sub

    Private Sub Button4_Click_4(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim commandes() As String

        Dim fichier_data_transfert As String = "transfert_data.xml"
        Dim fichierpdf_init As String = "myplot_diag.pdf"
        Dim fichierpdf As String = ""
        Dim type_smooth As Integer

        Cursor.Current = Cursors.WaitCursor


        result_pdf_et_diag.GoHome()

        Dim test As String
        test = Replace(System.IO.Path.GetTempPath() & fichierpdf_init, "\", "/")
        test = Replace(test, " ", "%20")
        If result_pdf_et_diag.Url.AbsoluteUri = "file:///" & test Then fichierpdf = "2" & fichierpdf_init Else fichierpdf = fichierpdf_init

        'Juste pour attendre que le composant web ne bloque pas le fichier qui doit être mis à jour
        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

        'on charge les différents paramètres du create.smooth
        Dim param_pas As String
        ' JS: Option strict on, make sure numbers are correctly parsed
        Try
            If (type_smooth1.Checked) Then
                param_pas = get_params(1, Double.Parse(smooth_param_1.Text))
            ElseIf (type_smooth2.Checked) Then
                param_pas = get_params(2, Double.Parse(smooth_param.Text), Double.Parse(decalage.Text))
            Else
                Debug.Assert(type_smooth3.Checked)
                param_pas = get_params(3)
            End If
        Catch ex As Exception
            cLog.Write(ex, "Ecotroph.Button4_Click_4")
        End Try
        Dim param_pas2 As String = ",Mul_eff = c(" & mull_eff.Text & "), Beta = " & Replace(beta.Text, ",", ".") & ", TopD = " & Replace(TopD.Text, ",", ".") & ", FormD = " & Replace(formd.Text, ",", ".")

        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas & " et " & param_pas2)


        'Le code R en lui même
        Dim fichier As String = System.IO.Path.GetTempPath() & "\transfert_diag.txt"
        If My.Computer.FileSystem.FileExists(fichier) Then My.Computer.FileSystem.DeleteFile(fichier)

        fichier = Replace(fichier, "\", "\\")
        Dim pathfile As String = Environment.CurrentDirectory()
        ReDim commandes(21)
        commandes(0) = "library(EcoTroph)"
        commandes(1) = "ecopath<-read.ecopath.model('" & Replace(System.IO.Path.GetTempPath() & "\" & fichier_data_transfert, "\", "\\") & "')"
        commandes(2) = "A<-create.ETdiagnosis(create.ETmain(ecopath" & param_pas & ")$ET_Main" & param_pas2 & ")"
        Debug.WriteLine(("A<-create.ETdiagnosis(create.ETmain(ecopath" & param_pas & ")$ET_Main" & param_pas2 & ")"))
        commandes(3) = "write.table(A$ET_Main_diagnose, file ='" & fichier & "', sep = '\t',quote=FALSE)"
        commandes(4) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(5) = "write.table(A$BIOM_MF, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
        commandes(6) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(7) = "write.table(A$Catches, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
        commandes(8) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(9) = "write.table(A$FLOW_MF, file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
        commandes(14) = "pdf(file='" & Replace(System.IO.Path.GetTempPath(), "\", "\\") & fichierpdf & "')"
        commandes(15) = "plot_ETdiagnosis(A)"
        commandes(16) = "dev.off()"
        commandes(17) = " "
        commandes(18) = " "
        commandes(19) = " "
        commandes(20) = " "
        commandes(21) = " quit('yes')"

        'on execute ce code R
        Dim output2 As String = execute_r(commandes)

        If (Len(output2) > 2) Then
            'MsgBox(output2)
        End If

        result_pdf_et_diag.Navigate(System.IO.Path.GetTempPath() & fichierpdf)

        If My.Computer.FileSystem.FileExists(fichier) Then
            Dim recup() As String = File.ReadAllLines(fichier)
            Dim totales As String = Join(recup, vbNewLine)
            Dim matrices() As String = Split(totales, "-----")

            For imat As Integer = 0 To matrices.Count

                If (imat = 0) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_ET_main_diagnose)
                If (imat = 1) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_biom_mf)
                If (imat = 2) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_catches)
                If (imat = 3) Then charge_grid(matrices(imat).Split(New Char() {CChar(cStringUtils.vbNewline)}, StringSplitOptions.RemoveEmptyEntries), grille_flow_mf)

            Next
        Else
            ' ToDo: provide a better error
            ' ToDo: globalize this
            Me.ReportMessage("The procedure did not yield any results")
        End If
        Cursor.Current = Cursors.Default

    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getgraph_diag.CheckedChanged
        If getgraph_diag.Checked = True Then
            result_pdf_et_diag.Visible = True
        Else : result_pdf_et_diag.Visible = False
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles smooth_graph.CheckedChanged
        If smooth_graph.Checked = True Then
            smooth_pdf.Visible = True
        Else : smooth_pdf.Visible = False
        End If
    End Sub

    Private Sub smooth_param_MaskInputRejected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MaskInputRejectedEventArgs) Handles smooth_param.MaskInputRejected

    End Sub

    Private Sub Reset_smooth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Reset_smooth.Click
        smooth_param_1.Text = "0.12"
        decalage.Text = "0.95"
        smooth_param.Text = "0.07"
    End Sub

    Private Sub reset_param_diag_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles reset_param_diag.Click
        TopD.Text = "0.2"
        formd.Text = "0.5"
        beta.Text = "0.1"
        mull_eff.Text = "0.0,0.2,0.4,0.7,1.0,1.5,2.0,2.5,3.0,4.0,5.0"
    End Sub

    ''' <summary>
    ''' Report a warning message to the EwE Core. The message will be shown to the user and will be logged in the status panel.
    ''' </summary>
    Private Sub ReportMessage(strMessage As String)

        Dim core As cCore = cEcotrophPlugin.etCore
        If (core Is Nothing) Then Return
        Try
            core.Messages.SendMessage(New cMessage(strMessage, eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Warning))
        Catch ex As Exception
            ' Whoah
        End Try

    End Sub

End Class