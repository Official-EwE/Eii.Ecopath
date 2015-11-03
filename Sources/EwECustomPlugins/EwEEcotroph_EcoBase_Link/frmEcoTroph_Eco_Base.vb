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

Option Strict Off
Imports System.IO
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization
Imports EcoTroph_EcoBase.cEcotroph_Eco_BasePlugIn
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

'not relevent to uncomppress R_ET.zip folder
'Imports Shell32

' ================================================================================
' Ecotroph code audit 1, 21Jun2013, Jeroen Steenbeek
'
' Recommended changes:
' - Replace all message boxes with cMessages or cFeedbackMessages to ensure events 
'   integrate with the EwE UI and are logged in cLog
' - All lengthy operations should provide status feedback via cApplicationStatusNotifier
' - All try/catch blocks should write an entry to cLog
' ================================================================================

Public Class frmEcoTroph_Eco_Base

    Dim num_model() As Integer
    Dim aide As String = "http://sirs.agrocampus-ouest.fr/EcoTroph/index.php?action=examples&lang=uk"
    Private Sub autre_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        smooth_pdf = Nothing
        result_pdf = Nothing
        result_pdf_et_diag = Nothing


    End Sub



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim fmsg As cFeedbackMessage = Nothing
        Dim test() As String
        Dim result() As String
        Dim result_tab() As String
        'Dim repos As String = "http://mirror.ibcp.fr/pub/CRAN/bin/windows/contrib/2.14"
        Dim repos_simple As String = "http://cran.univ-lyon1.fr/"

        Dim repos As String = repos_simple & "bin/windows/contrib/2.14/"



        'We have to test first if R is present in the Ewe directory
        ReDim test(6)
        ' We need to check 1- the version of R 2,3,4- If a new version of the Package exist and if we need to upgrade it
        test(0) = "getRversion()"
        test(1) = "is.element('EcoTroph',installed.packages()[,1])"
        test(2) = "options(timeout=1);summary(packageStatus(repositories=c('" & repos & "')))$inst$Version['EcoTroph']"
        test(3) = "Etat<-summary(packageStatus(repositories=c('" & repos & "')))$inst"
        test(4) = "Etat[Etat$Package=='EcoTroph','Status']"
        test(5) = "installed.packages()['EcoTroph','Version']"

        result = execute_r(test)
        result_tab = Split(result(1), vbCr)

        If (result(0).Contains("R is not")) Then

            ' JS 21Jun13: Really needed to change this
            fmsg = New cFeedbackMessage("You don't have R installed, you won't be able to run Ecotroph! Do you wish to download and install the minimum R for ecotroph directory now?", _
                                        eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.UIContext.Core.Messages.SendMessage(fmsg)

            If (fmsg.Reply = eMessageReply.OK) Then

                cApplicationStatusNotifier.StartProgress(Me.UIContext.Core, "Downloading local copy of R...", -1)
                Try

                    My.Computer.Network.DownloadFile("http://sirs.agrocampus-ouest.fr/EcoTroph/data/R_ET.zip", CurDir() & "\R_ET.zip", "", "", True, 500, True)
                Catch ex As Exception
                    MessageBox.Show(My.Resources.PB_DOWNLOAD & ex.Message)
                    cLog.Write(ex, "frmEcotroph.Load")
                End Try
                cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)

                'If inzip .exe is not here, we have to download it from the EcoTroph website
                Dim toto As String

                cApplicationStatusNotifier.StartProgress(Me.UIContext.Core, "Installing local copy of R...", -1)
                Try
                    toto = System.IO.Path.GetFileName(CurDir() & "\unzip.exe")

                    If Not (File.Exists(CurDir() & "\unzip.exe")) Then My.Computer.Network.DownloadFile("http://sirs.agrocampus-ouest.fr/EcoTroph/data/unzip.exe", CurDir() & "\unzip.exe", "", "", True, 500, True)
                    'This is a way to uncompress R_ET.zip to R folder but it crashs on XP when it's compile on Windows 7 and it 
                    'use a thirs partu dll (interop.shell32.dll) 
                    'Dim mydesktop As String = My.Computer.FileSystem.SpecialDirectories.Desktop
                    'Dim myshell As New Shell32.Shell
                    'Dim myzip As Shell32.Folder = myshell.NameSpace((CurDir() & "\R_ET.zip"))
                    'Dim mydrop As Shell32.Folder = myshell.NameSpace((CurDir()))
                    'mydrop.CopyHere(myzip.Items)
                Catch ex As Exception

                End Try

                'so i prefer to store the unzip.exe file inside the EwEEcoTroph.zip and use it via the system.command
                Dim myProcess As New Process()
                myProcess.StartInfo.UseShellExecute = False ' A remettre à false
                myProcess.StartInfo.FileName = CurDir() & "\unzip.exe "
                myProcess.StartInfo.Arguments = "-o R_ET.zip"
                myProcess.StartInfo.CreateNoWindow = True
                Try
                    myProcess.Start()
                Catch Ex As Exception
                    cLog.Write(Ex, "frmEcotroph::unzip")
                    MessageBox.Show(My.Resources.ERROR_UNZIP & Ex.Message)
                Finally

                End Try
                myProcess.WaitForExit()
                cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)

            End If
        Else
            ecotroph_version.Text = result_tab(6)
        End If
        If (result_tab(4).Contains("upgrade")) Then
            ' JS 21Jun13: Really needed to change this
            fmsg = New cFeedbackMessage("A new version of the EcoTroph R package is available. Do you wish to upgrade now?", _
                                        eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.UIContext.Core.Messages.SendMessage(fmsg)

            If (fmsg.Reply = eMessageReply.OK) Then

                cApplicationStatusNotifier.StartProgress(Me.UIContext.Core, "Upgrading R package...", -1)
                Try
                    test(0) = " install.packages('EcoTroph',repos=c('" & repos_simple & "'))"
                    test(1) = ""
                    test(2) = ""
                    test(3) = ""
                    test(4) = ""
                    result = execute_r(test)
                Catch ex As Exception
                    cLog.Write(ex, "frmEcotroph::upgrade R package")
                End Try
                cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)
            End If
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Load_from_ecopath.Click

        'a retester ou alors tester si les données sont dispo
        EcoTroph_EcoBase.cEcotroph_Eco_BasePlugIn.etCore.RunEcoPath()
        ETgridinput.BringToFront()
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
                DataGrid.Item(6, igrp).Value() = ETinputdatafromEP.habitat_area(igrp + 1)
                DataGrid.Item(7, igrp).Value() = ETinputdatafromEP.ee(igrp + 1)
                DataGrid.Item(8, igrp).Value() = ETinputdatafromEP.biom_acc_rate(igrp + 1)
                DataGrid.Item(9, igrp).Value() = ETinputdatafromEP.flow_to_det(igrp + 1)
                DataGrid.Item(10, igrp).Value() = ETinputdatafromEP.other_mort_rate(igrp + 1)




            Next
            commentaires.Text = ETinputdata.numfleet

            DataGrid.ColumnCount = 11 + ETinputdatafromEP.numfleet
            For ifleet As Integer = 0 To ETinputdatafromEP.numfleet - 1
                DataGrid.Columns(11 + ifleet).Name = ETinputdatafromEP.fleetname(ifleet + 1)
                For igrp As Integer = 0 To ETinputdatafromEP.TL.Length - 2
                    DataGrid.Item(11 + ifleet, igrp).Value() = ETinputdatafromEP.catches(ifleet)(igrp + 1)

                Next
                DataGrid.Columns(4).DefaultCellStyle.BackColor = Drawing.Color.BurlyWood
            Next

            ETinputdata.numfleet = ETinputdatafromEP.numfleet
            If Not (IsNothing(ETinputdata.comments)) Then commentaires.Text = ETinputdata.comments Else commentaires.Text = ""
            If Not (IsNothing(ETinputdata.ModelName)) Then Modelname.Text = ETinputdata.ModelName Else Modelname.Text = ""
            If Not (IsNothing(ETinputdata.Modeldescription)) Then modeldescription.Text = ETinputdata.Modeldescription Else modeldescription.Text = ""
            Button2.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True
        Else
            MsgBox(My.Resources.NO_MODEL_DATA)
        End If

        ' frmET.ETgridinput.DataSource = ETinput
        ' frmET.ETgridinput.Show()
    End Sub


    Private Sub Save_ETdata_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save_ETdata.Click
        Dim saveFileDialog1 As New SaveFileDialog()

        saveFileDialog1.Filter = My.Resources.FILEFILTER_XML
        saveFileDialog1.Title = My.Resources.SAVE_ECOTROPH
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
        Dim myStream As Stream = Nothing
        Dim openFileDialog1 As New OpenFileDialog()
        Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))
        openFileDialog1.InitialDirectory = "c:\"
        openFileDialog1.Filter = My.Resources.FILEFILTER_XML
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True
        ETgridinput.BringToFront()

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try

                Dim file As New System.IO.StreamReader(openFileDialog1.FileName)
                If (openFileDialog1.FileName <> "") Then
                    ETinputdata = CType(reader.Deserialize(file), ETinputtot)
                End If
            Catch Ex As Exception
                MessageBox.Show(My.Resources.ERROR_INPUT_FILE & Ex.Message)
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
            DataGrid.Columns(4).DefaultCellStyle.BackColor = Drawing.Color.BurlyWood
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
                    ETinputdata.groupname(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 1
                    ETinputdata.TL(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 2
                    ETinputdata.B(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 3
                    ETinputdata.PROD(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 4
                    ETinputdata.accessibility(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 5
                    ETinputdata.OI(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 6
                    ETinputdata.habitat_area(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 7
                    ETinputdata.ee(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 8
                    ETinputdata.biom_acc_rate(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 9
                    ETinputdata.flow_to_det(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
                Case 10
                    ETinputdata.other_mort_rate(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value

                    'Then it's fleet catches
                Case Is > 10

                    ETinputdata.catches(e.ColumnIndex - 11)(e.RowIndex + 1) = Me.ETgridinput.Item(e.ColumnIndex, e.RowIndex).Value
            End Select


        End If

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

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

    Public Shared Function execute_r(ByVal code As String()) As String()
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

        myProcess.StartInfo.FileName = CurDir() & "\R\bin\i386\r.exe"



        myProcess.StartInfo.Arguments = "--slave"
        myProcess.StartInfo.CreateNoWindow = True

        Dim output2() As String
        ReDim output2(2)
        If IO.File.Exists(myProcess.StartInfo.FileName) Then
            Try
                myProcess.Start()









                Dim myStreamWriter As StreamWriter = myProcess.StandardInput

                For icod As Integer = 0 To code.Count - 1
                    myStreamWriter.WriteLine(code(icod))
                    Debug.Print(code(icod))
                Next
                myStreamWriter.Close()




                Dim depasse As Boolean = myProcess.WaitForExit(100000)
                If depasse Then
                    output2(1) = myProcess.StandardOutput.ReadToEnd()
                    output2(0) = myProcess.StandardError.ReadToEnd()
                Else
                    MsgBox(My.Resources.EXCEED_TIME_R)
                End If

            Catch ex As Exception
                MsgBox(My.Resources.PB_R)
            End Try
        Else
            output2(0) = My.Resources.NO_R
        End If

        Return (output2)

    End Function
    Public Shared Function execute_rplot(ByVal code As String()) As String
        'Cette fonction execute un code R et renvoie le nom d'un fichier resultat
        Dim myProcess As New Process()
        myProcess.StartInfo.RedirectStandardInput = False


        myProcess.StartInfo.UseShellExecute = True ' A remettre à false
        myProcess.StartInfo.FileName = CurDir() & "\R\bin\i386\r.exe"



        myProcess.StartInfo.Arguments = "--slave"
        myProcess.StartInfo.CreateNoWindow = False



        myProcess.Start()

        'Shell(myProcess.StartInfo.FileName)

        'Dim myStreamWriter As StreamWriter = myProcess.

        For icod As Integer = 0 To code.Count - 1

            My.Computer.Keyboard.SendKeys(code(icod))
            'myStreamWriter.WriteLine(code(icod))
            'MsgBox(myProcess.Threads.Count & "pour " & code(icod))
        Next










        'Dim Output As Object = myProcess.StandardOutput.ReadToEnd()
        'Dim output2 As String = myProcess.StandardError.ReadToEnd()
        'myStreamWriter.Close()


        Return (vbOK)

    End Function

    Public Shared Function sauve_datagrid_xml(ByVal grille As ETinputtot, ByVal filename As String) As Boolean


        Dim writer As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))


        Dim file_data As New System.IO.StreamWriter(filename)


        writer.Serialize(file_data, ETinputdata)
        file_data.Close()
        Return True

    End Function


    Public Shared Function charge_grid(ByVal donnees As String(), ByRef grille As DataGridView) As Integer

        Dim tab_trans(,) As String
        Dim uneligne() As String

        donnees(0) = vbTab & donnees(0)
        Dim nbl As Integer = donnees.Length
        Dim nbcol As Integer = (donnees(0).Split(vbTab).Length)


        ReDim tab_trans(nbcol, nbl)
        ReDim uneligne(nbcol)
        Dim deci_sep As String

        'Une astuce pour obtenir le sep décimal
        deci_sep = Mid$(CStr(1 / 2), 2, 1)


        ' La partie suivante est a mettre en fonction(donnees as data,sheet as )
        grille.ColumnCount = nbcol


        For igrp As Integer = 0 To nbl - 1

            If (grille.Rows.Count < nbl) Then grille.Rows.Add()



            uneligne = donnees(igrp).Split(vbTab)
            If uneligne.Length > 1 Then


                If (uneligne(1).Contains("mE")) Then
                    donnees(igrp) = donnees(igrp).Substring(1, donnees(igrp).Length - 1)
                    uneligne = donnees(igrp).Split(vbTab)
                End If
            End If
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
                output2 = ",sigmaLN_cst=" & Replace(smooth_parameter, ",", ".")
            Case 2
                output2 = ",smooth_type=2,smooth_param=" & Replace(smooth_parameter, ",", ".") & ",shift=" & Replace(decalage, ",", ".")
            Case 3
                output2 = ",smooth_type=3"

        End Select


        Return (output2)

    End Function


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'On commence par sauver le fichier de données 

        Dim commandes() As String
        Dim fichierpdf As String = cFileUtils.MakeTempFile(".pdf")
        Dim fichier_data_transfert As String = cFileUtils.MakeTempFile(".xml")
        Dim fichier As String = cFileUtils.MakeTempFile(".txt")



        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)



        'on charge les différents paramètres du create.smooth
        Dim param_pas As String = ""

        If (type_smooth1.Checked) Then param_pas = get_params(1, smooth_param_1.Text)
        If (type_smooth2.Checked) Then param_pas = get_params(2, smooth_param.Text, decalage.Text)
        If (type_smooth3.Checked) Then param_pas = get_params(3)


        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)
        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)

        'Le code R en lui même





        ReDim commandes(9)
        commandes(0) = ""
        commandes(1) = "library(EcoTroph)"
        commandes(2) = "ecopath<-read.ecopath.model('" & Replace(fichier_data_transfert, "\", "\\") & "')"
        commandes(3) = "A<-create.smooth(ecopath" & param_pas & ")"
        commandes(4) = ""
        commandes(5) = "write.table(A, file ='" & Replace(fichier, "\", "\\") & "', sep = '\t',quote=FALSE)"
        commandes(6) = "pdf(file='" & Replace(fichierpdf, "\", "\\") & "')"

        'commandes(7) = "plot_smooth(A)" modification et utilisation plot générique
        commandes(7) = "plot(A)"
        commandes(8) = "dev.off()"
        commandes(9) = "quit('yes')"

        'on execute ce code R

        Try
            Dim output2() As String = execute_r(commandes)
            ' If Len(output2) > 0 Then MsgBox(output2)

        Catch ex As Exception
            'MessageBox.Show("Problem in R script: " & ex.Message)
        End Try




        smooth_pdf.Navigate(fichierpdf)


        'smooth_pdf.Refresh()
        If My.Computer.FileSystem.FileExists(fichier) Then
            Dim recup() As String = File.ReadAllLines(fichier)
            Try

                charge_grid(recup, datasmooth)
            Catch ex As Exception
                MessageBox.Show("Problem in reading R script output: " & ex.Message)
            End Try
        Else
            MsgBox(My.Resources.NO_OUTPUT_R)
        End If






        Cursor.Current = Cursors.Default


        'Test de la partie graphique, pour voir






    End Sub

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub

    Private Sub pas_MaskInputRejected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MaskInputRejectedEventArgs)

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim commandes() As String



        Dim fichierpdf As String = cFileUtils.MakeTempFile(".pdf")
        Dim fichier_data_transfert As String = cFileUtils.MakeTempFile(".xml")
        Dim fichier As String = cFileUtils.MakeTempFile(".txt")
        Dim log_ech As String



        result_pdf.GoHome()

        Cursor.Current = Cursors.WaitCursor


        'Juste pour attendre que le composant web ne bloque pas le fichier qui doit être mis à jour
        Dim param_pas As String = ""
        If (type_smooth1.Checked) Then param_pas = get_params(1, smooth_param_1.Text)

        If (type_smooth2.Checked) Then param_pas = get_params(2, smooth_param.Text, decalage.Text)
        If (type_smooth3.Checked) Then param_pas = get_params(3)
        ' MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas)
        If (Log_scale.Checked) Then log_ech = ",scale1=log,scale2=log,scale3=log" Else log_ech = ""

        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

        'on charge les différents paramètres du create.smooth


        'Le code R en lui même



        fichier = Replace(fichier, "\", "\\")

        ReDim commandes(21)
        commandes(0) = "options(warn=0)"
        commandes(1) = "library(EcoTroph)"
        commandes(2) = "ecopath<-read.ecopath.model('" & Replace(fichier_data_transfert, "\", "\\") & "')"
        commandes(3) = "A<-create.ETmain(ecopath" & param_pas & ")"
        commandes(4) = "write.table(A$ET_Main[as.numeric(rownames(A$ET_Main))<6,], file ='" & fichier & "', sep = '\t',quote=FALSE)"
        commandes(5) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(6) = "write.table(A$biomass[as.numeric(rownames(A$biomass))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE)"
        commandes(7) = "cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(8) = "write.table(A$biomass_acc[as.numeric(rownames(A$biomass_acc))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(9) = "write.table(A$prod[as.numeric(rownames(A$prod))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(10) = "write.table(A$prod_acc[as.numeric(rownames(A$prod_acc))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(11) = "write.table(as.data.frame(lapply(A$Y,rowSums)),file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(12) = "AY<-Reduce('+',A$Y);write.table(AY[as.numeric(rownames(AY))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)"
        commandes(13) = "for (pecheries in names(A$Y)) {write.table(A$Y[[pecheries]][as.numeric(rownames(A$Y[[pecheries]]))<6,], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)}"

        commandes(14) = ""
        commandes(15) = "pdf(file='" & Replace(fichierpdf, "\", "\\") & "')"
        'commandes(16) = "plot_ETmain(A" & log_ech & ")"
        commandes(16) = "plot(A" & log_ech & ")"
        commandes(17) = "dev.off()"

        commandes(18) = " "
        commandes(19) = " "
        commandes(20) = " "
        commandes(21) = " quit('yes')"


        'on execute ce code R

        Try
            Dim output2() As String = execute_r(commandes)
            ' If Len(output2) > 0 Then MsgBox(output2)

        Catch ex As Exception
            'MessageBox.Show("Problem in R script: " & ex.Message)
        End Try


        result_pdf.Navigate(fichierpdf)

        If My.Computer.FileSystem.FileExists(fichier) Then



            'End If

            Dim recup() As String = File.ReadAllLines(fichier)

            Dim totales As String = Join(recup, vbNewLine)
            Dim matrices() As String = Split(totales, "-----")



            Dim Ctr() As Control = Me.Controls.Find("Catch." & (ETinputdata.fleetname(0)), True)
            Try

                charge_grid(matrices(0).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_ET_main)
                charge_grid(matrices(1).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_biomass)
                charge_grid(matrices(2).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_biomass_acc)
                charge_grid(matrices(3).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_flow_p)
                charge_grid(matrices(4).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_flow_p_acc)
                charge_grid(matrices(5).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_y)
                '    If panel_result.TabPages.Count = 6 Then
                For compteur_fleet As Integer = 0 To ETinputdata.numfleet - 1

                    Dim ctrl() As Control = panel_result.Controls.Find("Catch." & (ETinputdata.fleetname(compteur_fleet)), True)

                    If ctrl.Length = 0 Then

                        Dim myTabPage As New TabPage()
                        myTabPage.Text = "Catch." & (ETinputdata.fleetname(compteur_fleet))
                        myTabPage.Name = "tabCatch." & (ETinputdata.fleetname(compteur_fleet))
                        panel_result.TabPages.Add(myTabPage)
                        Dim dtg As New DataGridView
                        dtg.Name = "Catch." & (ETinputdata.fleetname(compteur_fleet))
                        dtg.Height = 391
                        dtg.Width = 782
                        dtg.Top = 6
                        dtg.Left = 3
                        dtg.Dock = DockStyle.Fill
                        panel_result.TabPages(compteur_fleet + 6).Controls.Add(dtg)
                        charge_grid(matrices(compteur_fleet + 6).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), dtg)
                    Else
                        charge_grid(matrices(compteur_fleet + 6).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ctrl(0))
                    End If


                Next


            Catch ex As Exception
                MessageBox.Show("Problem in reading R script output: " & ex.Message)
            End Try


        Else
            MsgBox(My.Resources.NO_OUTPUT_R)
        End If

        Cursor.Current = Cursors.Default

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)




    End Sub





    Private Sub Process1_Exited(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub getgraphs_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getgraphs.CheckedChanged
        If getgraphs.Checked = True Then
            result_pdf.BringToFront()

            result_pdf.Visible = True
        Else : result_pdf.Visible = False
        End If

    End Sub

    Private Sub Button4_Click_3(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button4_Click_4(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If getgraph_diag.Checked = True Then
            result_pdf_et_diag.Visible = True
        Else : result_pdf_et_diag.Visible = False
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles smooth_graph.CheckedChanged


        If smooth_graph.Checked = True Then
            smooth_pdf.BringToFront()
            smooth_pdf.Visible = True
        Else : smooth_pdf.Visible = False
        End If
    End Sub

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click

    End Sub

    Private Sub GroupBox2_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox2.Enter

    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label4.Click

    End Sub

    Private Sub smooth_param_MaskInputRejected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MaskInputRejectedEventArgs) Handles smooth_param.MaskInputRejected

    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label5.Click

    End Sub

    Private Sub Reset_smooth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Reset_smooth.Click

        smooth_param_1.Text = "0.12"
        decalage.Text = "0.95"
        smooth_param.Text = "0.07"


    End Sub

    Private Sub reset_param_diag_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        TopD.Text = "0.2"
        formd.Text = "0.5"
        beta.Text = "0.1"

    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click, Label13.Click

    End Sub

    Private Sub TabPage1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabPage1.Click

    End Sub

    Private Sub CheckBox1_CheckedChanged_2(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub list_group_diag_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub b_input_check_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles b_input_check.CheckedChanged
        If b_input_check.Checked Then
            beta.Enabled = True
            Forag.Checked = False
        Else
            beta.Enabled = False
        End If
    End Sub

    Private Sub Ponto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles diagnosis_page.Click

    End Sub

    Private Sub getgraph_diag_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getgraph_diag.CheckedChanged
        If getgraph_diag.Checked = True Then
            result_pdf_et_diag.BringToFront()

            result_pdf_et_diag.Visible = True
        Else : result_pdf_et_diag.Visible = False
        End If
    End Sub

    Private Sub Button4_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim commandes() As String
        Dim fichierpdf As String = cFileUtils.MakeTempFile(".pdf")
        Dim fichier_data_transfert As String = cFileUtils.MakeTempFile(".xml")
        Dim fichier As String = cFileUtils.MakeTempFile(".txt")
        Dim log_ech_diag As String

        Cursor.Current = Cursors.WaitCursor


        'result_pdf_et_diag.GoHome()

        sauve_datagrid_xml(ETinputdata, fichier_data_transfert)

        Dim param_iso As String = ""

        'on charge les différents paramètres du create.smooth
        Dim param_pas As String = ""
        If (type_smooth1.Checked) Then param_pas = get_params(1, smooth_param_1.Text)

        If (type_smooth2.Checked) Then param_pas = get_params(2, smooth_param.Text, decalage.Text)
        If (type_smooth3.Checked) Then param_pas = get_params(3)
        If (log_scale_diagnose.Checked) Then log_ech_diag = ",scale=log" Else log_ech_diag = ""
        Dim param_pas2 As String = ", TopD = " & Replace(TopD.Text, ",", ".") & ", FormD = " & Replace(formd.Text, ",", ".")

        If (b_input_check.Checked) Then param_pas2 = param_pas2 & ",B.Input=TRUE, Beta = " & Replace(beta.Text, ",", ".")
        If (Forag.Checked) Then
            param_pas2 = param_pas2 & ",Forag.A=TRUE, Kfeed = " & Replace(Kfeed.Text, ",", ".") & ", Ponto = " & Replace(Ponto.Text, ",", ".")
        Else
            param_pas2 = param_pas2 & ",Forag.A=FALSE"
        End If
        Dim param_EMSY As String = param_pas2

        If (same_mf.Checked) Then param_pas2 = param_pas2 & ",same.mE=TRUE"
        If (Not All_group.Checked) Then

            Dim liste_group As String = ""

            For Each item As Object In list_group_diag.SelectedItems

                liste_group = liste_group & "'" & item.ToString & "',"
            Next
            If liste_group.Length > 0 Then
                param_pas2 = param_pas2 & ",Group=c(" & liste_group.Substring(0, liste_group.Length - 1) & ")"
            End If

        End If

        If (Not same_mf.Checked) Then
            Dim liste_group As String = ""

            If List_fleet1.SelectedItems.Count = 0 Then

                param_pas2 = param_pas2 & ",same.mE=TRUE"
                MsgBox(My.Resources.NO_SELECTED_FLEET)
                same_mf.Checked = True
            Else

                For Each item As Object In List_fleet1.SelectedItems

                    liste_group = liste_group & "'catch." & item.ToString().Replace(" ", ".") & "',"
                Next
                param_iso = param_iso & ",fleet.of.interest=c(" & liste_group.Substring(0, liste_group.Length - 1) & ")"
                liste_group = ""

            End If




        End If

        'MsgBox("Nous allons Lancer la fonction smooth avec les paramètres :" & param_pas & " et " & param_pas2)


        'Le code R en lui même


        fichier = Replace(fichier, "\", "\\")

        ReDim commandes(30)
        Dim liste_tables() As String = {"ET_Main_diagnose", "B", "B_acc", "P", "P_acc", "Kin", "Kin_acc", "Fish_mort", "Fish_mort_acc", "Y"}

        commandes(0) = "library(EcoTroph)"
        commandes(1) = "ecopath<-read.ecopath.model('" & Replace(fichier_data_transfert, "\", "\\") & "')"
        commandes(2) = "ETM<-create.ETmain(ecopath" & param_pas & ");A<-create.ETdiagnosis(ETM" & param_pas2 & param_iso & ")"
        commandes(3) = "B<-convert.list2tab(A)"

        commandes(4) = "write.table(B$" & liste_tables(0) & ", file ='" & fichier & "',col.names=FALSE,row.names=FALSE, sep = '\t',quote=FALSE);" & "cat('-----\n', file ='" & fichier & "',append=TRUE);"
        For compteur_commandes As Integer = 1 To 9
            commandes(compteur_commandes + 4) = "write.table(B$" & liste_tables(compteur_commandes) & ", file ='" & fichier & "', col.names=FALSE,row.names=FALSE,sep = '\t',quote=FALSE,append=TRUE);" & "cat('-----\n', file ='" & fichier & "',append=TRUE);"
        Next
        commandes(14) = ""
        commandes(15) = ""
        commandes(16) = ""

        commandes(17) = "pdf(file='" & Replace(fichierpdf, "\", "\\") & "')"
        'commandes(18) = "plot_ETdiagnosis(A)"
        commandes(18) = "plot(A" & log_ech_diag & ")"
        commandes(19) = ""
        If Not same_mf.Checked Then
            'commandes(19) = "B<-plot_ETdiagnosis_isopleth(A)"
            commandes(19) = "B<-plot_ETdiagnosis_isopleth(A)"
            commandes(20) = "for (pecheries in names(B)) {write.table(B[[pecheries]], file ='" & fichier & "', sep = '\t',append=TRUE,quote=FALSE);cat('-----\n', file ='" & fichier & "',append=TRUE)}"
        Else
            If All_group.Checked Then
                commandes(21) = "A<-E_MSY_0.1(ETM" & param_EMSY & ")"
                commandes(22) = "write.table(A, file ='" & fichier & "', sep = '\t',quote=FALSE,append=TRUE);" & "cat('-----\n', file ='" & fichier & "',append=TRUE);"
                commandes(23) = "par(mar=c(5,4,1,8));plot(row.names(A),A[,'E_0.1'],ylim=range(range(A[,'E_0.1'],na.rm=T,finite=T),range(A[,'E_MSY'],na.rm=T,finite=T),na.rm=T,finite=T),type='l',lwd=2,col='blue',xlab='Trophic levels',ylab='E');abline(h = 1)"
                commandes(24) = "lines(row.names(A),A[,'E_MSY'],type='l',lwd=2,col='red')"
                commandes(25) = "legend(6,range(range(A[,'E_0.1'],na.rm=T,finite=T),range(A[,'E_MSY'],na.rm=T,finite=T),na.rm=T,finite=T)[2],legend=c('E_MSY','E_0.1'),lty=c(1,1),col=c('red','blue'),xpd=NA)"

                commandes(26) = "plot(row.names(A),A[,'F_0.1'],xlim=c(2,5.5),ylim=range(range(A[,'F_0.1'],na.rm=T,finite=T),range(A[,'F_MSY'],na.rm=T,finite=T),na.rm=T,finite=T),type='l',lwd=2,col='blue',xlab='Trophic levels',ylab='F') "

                commandes(27) = "lines(row.names(A),A[,'F_MSY'],type='l',lwd=2,col='red')"
                commandes(28) = "legend(6,range(range(A[,'F_0.1'],na.rm=T,finite=T),range(A[,'F_MSY'],na.rm=T,finite=T),na.rm=T,finite=T)[2],legend=c('F_MSY','F_0.1'),lty=c(1,1),col=c('red','blue'),xpd=NA)"

            End If
        End If



        commandes(29) = "dev.off()"
        commandes(30) = " quit('yes')"

        'on execute ce code R
        Try
            Dim output2() As String = execute_r(commandes)
            ' If Len(output2) > 0 Then MsgBox(output2)

        Catch ex As Exception
            'MessageBox.Show("Problem in R script: " & ex.Message)
        End Try



        result_pdf_et_diag.Navigate(fichierpdf)

        If My.Computer.FileSystem.FileExists(fichier) Then


            Dim recup() As String = File.ReadAllLines(fichier)


            Dim totales As String = Join(recup, vbNewLine)
            Dim matrices() As String = Split(totales, "-----")
            Try

                charge_grid(matrices(0).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), grille_ET_main_diagnose)
                charge_grid(matrices(1).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_B)
                charge_grid(matrices(2).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_B_acc)
                charge_grid(matrices(3).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_FL_P)
                charge_grid(matrices(4).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_FL_P_acc)
                charge_grid(matrices(5).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_Kin)
                charge_grid(matrices(6).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_Kin_acc)
                charge_grid(matrices(7).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_F)
                charge_grid(matrices(8).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_F_acc)
                charge_grid(matrices(9).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_D_Y)
            Catch ex As Exception
                MessageBox.Show("Problem in reading R script output: " & ex.Message)
            End Try
            'Chargement des résultats du plot_ETdianosis_ispoleth

            Dim isopleth_output() As String = {"TOT_biomass", "TOT_biomass_acc", "TOT_P", "TOT_P_acc", "Y", "Y_fleet1", "Y_fleet2", "TL_TOT_biomass", "TL_TOT_biomass_acc", "TL_Catches", "TL_Catches_fleet1", "TL_Catches_fleet2"}

            If Not same_mf.Checked Then

                For compteur_output As Integer = 0 To isopleth_output.Length - 1


                    Dim ctrl() As Control = panel_result_diag.Controls.Find(isopleth_output(compteur_output), True)

                    If ctrl.Length = 0 Then

                        Dim myTabPage As New TabPage()
                        myTabPage.Text = isopleth_output(compteur_output)
                        myTabPage.Name = "Tab" & isopleth_output(compteur_output)
                        panel_result_diag.TabPages.Add(myTabPage)
                        Dim dtg As New DataGridView
                        dtg.Name = isopleth_output(compteur_output)
                        dtg.Height = 391
                        dtg.Width = 782
                        dtg.Top = 6
                        dtg.Left = 3
                        dtg.Dock = DockStyle.Fill
                        panel_result_diag.TabPages(panel_result_diag.TabCount - 1).Controls.Add(dtg)
                        charge_grid(matrices(compteur_output + 10).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), dtg)
                    Else
                        charge_grid(matrices(compteur_output + 10).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ctrl(0))
                    End If


                Next

            Else
                'process of deleting isopleth out if they were created before
                For compteur_output As Integer = 0 To isopleth_output.Length - 1


                    Dim ctrl() As Control = panel_result_diag.Controls.Find(isopleth_output(compteur_output), True)

                    If ctrl.Length > 0 Then
                        ctrl(0).Enabled = False
                        'A voir pour éffacer carrément les tables 
                    End If
                Next


            End If
            If (same_mf.Checked And All_group.Checked) Then
                charge_grid(matrices(10).Split(New Char() {vbNewLine}, StringSplitOptions.RemoveEmptyEntries), ET_M_EMSY)
            Else
                ET_M_EMSY.RowCount = 1

            End If


        Else
            MsgBox(My.Resources.NO_OUTPUT_R)
        End If
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub reset_param_diag_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles reset_param_diag.Click
        TopD.Text = "0.2"
        formd.Text = "0.5"
        beta.Text = "0.1"

        Kfeed.Text = "05"
        Ponto.Text = "0.3"
        same_mf.Checked = True
        Forag.Checked = True


        b_input_check.Checked = False


    End Sub

    Private Sub Forag_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Forag.CheckedChanged
        If Forag.Checked Then
            Kfeed.Enabled = True
            Ponto.Enabled = True
            b_input_check.Checked = False
        Else
            Kfeed.Enabled = False
            Ponto.Enabled = False
        End If
    End Sub

    Private Sub group_param_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub










    Private Sub List_fleet1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles List_fleet1.SelectedIndexChanged

        Dim compteur_fin As Integer = List_fleet1.SelectedItems.Count
        If compteur_fin = List_fleet1.Items.Count Then
            MsgBox(My.Resources.TOO_SELECTED_FLEET)
        End If

    End Sub



    Private Sub mull_eff_EMSY_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub same_mf_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles same_mf.CheckedChanged
        If Not same_mf.Checked Then
            List_fleet1.Enabled = True

            Dim compteur As Integer

            If (ETinputdata.numfleet < 2) Then
                MsgBox(My.Resources.NOT_ENOUGH_FLEET)
                same_mf.Checked = True
                List_fleet1.Enabled = False
            Else


                If (ETinputdata.numfleet > 1 And List_fleet1.Items.Count = 0) Then


                    For compteur = 0 To ETinputdata.numfleet - 1
                        List_fleet1.Items.Add(ETinputdata.fleetname(compteur))

                    Next
                End If
            End If

        Else
            List_fleet1.Enabled = False
        End If


    End Sub

    Private Sub Label19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub



    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click



        Try
            Dim myservice As New getResult()
            Dim myresult As String
            Dim myresult_xml As New XmlDocument()

            Cursor.Current = Cursors.WaitCursor
            panel_webservi.Visible = True
            panel_webservi.BringToFront()


            If models_list.Items.Count = 0 Then


                Try
                    myresult = myservice.list_models("", Nothing)
                    myresult_xml.LoadXml(myresult)

                    Dim nodelist As XmlNodeList = myresult_xml.DocumentElement.ChildNodes
                Catch ex As Exception


                    MessageBox.Show(My.Resources.NO_DB_SERVICES)
                End Try


                ReDim num_model(myresult_xml.GetElementsByTagName("element").Count)
                Dim compteur As Integer = 0

                For Each node As XmlNode In myresult_xml.GetElementsByTagName("element")

                    If Not (IsNothing(node("model_name"))) Then


                        models_list.Items.Add(node("model_name").InnerText)
                        num_model(compteur) = node("model_number").InnerText
                        compteur = compteur + 1
                    End If
                Next

            End If
            Cursor.Current = Cursors.Default
        Catch ex As Exception
            cLog.Write(ex, "Ecotroph::Button7-Click")
            MessageBox.Show(My.Resources.ERROR_NO_WS)
        End Try

    End Sub

    Private Sub models_list_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles models_list.DoubleClick

        Dim myStream As Stream = Nothing
        Dim openFileDialog1 As New OpenFileDialog()
        panel_webservi.Visible = False


        openFileDialog1.InitialDirectory = "c:\"
        openFileDialog1.Filter = My.Resources.FILEFILTER_XML
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True



        Dim url_eco As String

        url_eco = "http://sirs.agrocampus-ouest.fr/EcoTroph/php/extract_model.php?model=" & num_model(models_list.SelectedIndex)


        Try
            Dim myservice As New getResult()

            ' Jerome refonte et utilisation webservice 13/12/2012

            'Dim myresult As String
            'Dim myresult_xml As New XmlDocument()



            ' Good way to get the model from the webservice

            'myresult = myservice.getModel("input_data", num_model(models_list.SelectedIndex))
            'myresult_xml.LoadXml(myresult)

            ' old way to do the same think without webservices

            'Cela devrait être fait avec le myservice getmodel mais les 2 extract_model et get_model ne renvoie pas la même chose
            'et ans le second cas ce qui est renvoyé n'ets pas apprécié. Il faut modifier le web service pour qu'il soit plsu conforme
            'ce qui ne doit pas être grand chose


            'Dim Str As System.IO.Stream
            'Dim srRead As System.IO.StreamReader

            'Try
            ' make a Web request
            'Dim req As System.Net.WebRequest = System.Net.WebRequest.Create(url_eco)
            'Dim resp As System.Net.WebResponse = req.GetResponse
            'Str = resp.GetResponseStream
            'srRead = New System.IO.StreamReader(Str)
            'myresult = srRead.ReadToEnd()


            'myresult_xml.LoadXml(myresult)

            'Catch ex As Exception

            'Finally
            '  Close Stream and StreamReader when done
            '   srRead.Close()
            '  Str.Close()
            'End Try

            'Dim fichier_data_transfert As String = cFileUtils.MakeTempFile(".xml")
            'myresult_xml.Save(fichier_data_transfert)

            'Dim file As New System.IO.StreamReader(fichier_data_transfert)

            Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(ETinputtot))




            ETinputdata = CType(reader.Deserialize(New StringReader(myservice.getModel("input_data", num_model(models_list.SelectedIndex)))), ETinputtot)



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
            DataGrid.Columns(4).DefaultCellStyle.BackColor = Drawing.Color.BurlyWood
            Button2.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True

        Catch Ex As Exception
            cLog.Write(Ex, "Ecotroph::models_list")
            MessageBox.Show(My.Resources.NO_MODEL_DATA & Ex.Message)
        Finally
            ' Check this again, since we need to make sure we didn't throw an exception on open.
            If (myStream IsNot Nothing) Then
                myStream.Close()


            End If
        End Try

    End Sub

    Private Sub models_list_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles models_list.SelectedIndexChanged

        Dim url_eco As String
        url_eco = "http://sirs.agrocampus-ouest.fr/EcoTroph/index.php?ident=base_eco&pass=base_eco&provenance=ecopath&action=base&menu=0&model=" & num_model(models_list.SelectedIndex)
        site_eco.Navigate(New Uri(url_eco))

    End Sub

    Private Sub All_group_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles All_group.CheckedChanged
        If All_group.Checked Then
            list_group_diag.Enabled = False

        Else
            list_group_diag.Enabled = True
            Dim compteur As Integer


            If (ETgridinput.RowCount > 1 And list_group_diag.Items.Count = 0) Then


                For compteur = 1 To ETgridinput.RowCount - 2
                    If (DirectCast(ETgridinput.Item(4, compteur).Value, Single) > 0) Then list_group_diag.Items.Add(ETgridinput.Item(0, compteur).Value)

                Next
            End If


        End If
    End Sub

    Private Sub list_group_diag_old_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub list_group_diag_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles list_group_diag.SelectedIndexChanged

    End Sub

    Private Sub Log_scale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Log_scale.CheckedChanged

    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        panel_webservi.Visible = False


    End Sub







    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        System.Diagnostics.Process.Start(aide & "#smooth1")
    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        System.Diagnostics.Process.Start(aide & "#transpose")
    End Sub

    Private Sub PictureBox5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox5.Click
        System.Diagnostics.Process.Start(aide & "#diagnose")
    End Sub


    Private Sub result_pdf_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles result_pdf.DocumentCompleted

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click


        'Begin to read the csv file with id_ref / name of tha database 
        Dim rep As String = "c://Users//jerome.POLEHAL.000//Dropbox//update_ecobase_03_2015//"

        ' Hold the amount of lines already read in a 'counter-variable'
        Dim fileIn As String = rep & "liste_model.txt"
        Dim fileRows, fileFields() As String
        Label1.Text = String.Empty
        Dim fileStream As StreamReader = File.OpenText(fileIn)

        Dim modelFile As String
        Dim idref As Integer
        ' If the file name is not an empty string open it for saving.
        ' Saves the Image via a FileStream created by the OpenFile method.

        'C'est ici que je dois avoir une nouvelle variable texte avec les données sérialisées




        fileRows = fileStream.ReadLine
        fileFields = fileRows.Split(vbTab)

        Do While (fileStream.Peek <> -1) '  Is -1 when no data exists on the next line of the CSV file
            fileRows = fileStream.ReadLine
            fileFields = fileRows.Split(vbTab)

            idref = fileFields(0)

            modelFile = rep & fileFields(1)
            'MsgBox(modelFile)

            If Core.LoadModel(modelFile) Then
                Debug.Print("Model" & modelFile & "loaded")
                EcoTroph_EcoBase.cEcotroph_Eco_BasePlugIn.etCore.RunEcoPath()
                'Button1_Click(sender, e)

                
                Dim serializer As New XmlSerializer(GetType(ETinputtot))
                Dim writer = New StringWriter()
                serializer.Serialize(writer, ETinputdatafromEP)

                Dim ETinputdata_xml_txt As String = writer.ToString()
                'MsgBox("Insertion modèle" & idref)
                


                Try
                    Dim myresult As String
                    Cursor.Current = Cursors.WaitCursor
                    panel_webservi.Visible = True
                    panel_webservi.BringToFront()
                    Dim myservice As New getResult()
                    Dim test As New XmlDocument
                    test.LoadXml(ETinputdata_xml_txt)
                    test.Save(rep & "result//" & idref & ".xml")

                    'myresult = myservice.Upload_Model(idref, ETinputdata_xml_txt)
                    'MsgBox("Ok pour " & idref)



                Catch ex As Exception
                    cLog.Write(ex, "Ecotroph::Button7-Click")
                    MessageBox.Show("Pour le modele " & idref & "on  " & ex.Message.ToString() & My.Resources.ERROR_NO_WS)
                End Try


            Else
                Debug.Print("Model" & modelFile & "not available ")
            End If
        Loop
    End Sub
End Class