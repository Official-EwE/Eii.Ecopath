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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports EwEUtils.Core
''' <summary>
''' Calculate new diet matrix from preferences
''' </summary>
Public Class cDietCalculator
    Dim m_EcopathData As EwECore.cEcopathDataStructures
    Private m_Core As EwECore.cCore

    Private SumBio As Single

    Private LivingBio As Single
    Private MaxBio As Single

    Private SumR() As Single

    Public Sub New(EwECore As EwECore.cCore, EcopathData As EwECore.cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData
    End Sub

    Public Function DietsFromPreferences(ExternalDietPrefs As cDietPreferences) As Boolean
        Dim igrp As Integer
        Dim Alpha(,) As Single = New Single(nGroups, nGroups) {}

        Me.SumR = New Single(nGroups) {}

        'sum biomass calculated on internal biomasses
        'This is the total biomass abundance in the ecosystem
        'Used to scale to the available biomass
        For igrp = 1 To Me.nGroups
            If Me.m_EcopathData.Binput(igrp) > 0 Then
                SumBio = SumBio + Me.m_EcopathData.B(igrp)
            End If
        Next

        'Alpha() is calcualted on Imported Biomass and Diets
        'Scaled to SumBio (calcualted above) calculated on the internal biomass
        'This is used to scale the imported diets to internal biomass
        For igrp = 1 To Me.nGroups
            CalcChessonAlpha(igrp, Alpha, ExternalDietPrefs.Biomass, ExternalDietPrefs.DietPref)
        Next

        Me.IterateForDiet(Alpha, ExternalDietPrefs)

        Return True

    End Function


    Private ReadOnly Property nGroups As Integer
        Get
            Return Me.m_Core.nGroups
        End Get
    End Property

    Private ReadOnly Property nLiving As Integer
        Get
            Return Me.m_Core.nLivingGroups
        End Get
    End Property


    Private Sub CalcChessonAlpha(i As Integer, ByRef Alpha(,) As Single, B() As Single, DCij(,) As Single)
        'will calculate Chesson's Alpha from the equation si = ri/pi / sum(rn/pn)
        'where ri is the DC and Pn the proportion the biomass of a group constitutes of the total biomass
        Dim j As Integer

        Debug.Assert(SumBio <> 0, "Opps SumBio not set!")
        If SumBio = 0 Then SumBio = 1
        SumR(i) = 0
        For j = 1 To Me.nGroups              'FOLLOWING CHESSON (1983)
            If B(j) > 0.0 Then
                Alpha(i, j) = DCij(i, j) / (B(j) / SumBio)
                SumR(i) = SumR(i) + Alpha(i, j)
            End If
        Next j

        For j = 1 To Me.nGroups
            If SumR(i) > 0 Then
                Alpha(i, j) = Alpha(i, j) / SumR(i)
            End If
        Next j               'THIS ALPHA IS THE SAME AS CHESSONS ALPHA

        Return

    End Sub


    Private Sub IterateForDiet(Alpha(,) As Single, ExternalDiets As cDietPreferences)
        Dim Cnt As Integer
        Dim DClast() As Double
        Dim DietSum As Single
        Dim Diff As Single
        Dim i As Integer
        Dim IsPrey() As Boolean
        Dim iPred As Integer
        Dim nPred As Integer
        Dim Ratio As Single
        Dim bNeedsBalancing As Boolean = False

        Dim Si(,) As Single = New Single(Me.nLiving, Me.nGroups) {}  'is the selection index (alpha) for prey i
        Dim Diet(,) As Single = New Single(Me.nLiving, Me.nGroups) {}

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'For Debugging
        'System.Console.WriteLine("Pred, Prey, Di(0), Di(1)")
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        For iPred = 1 To Me.nLiving
            ReDim DClast(Me.nGroups)
            ReDim IsPrey(Me.nGroups)
            nPred = 0
            DietSum = 0
            Cnt = 0

            For i = 1 To Me.nGroups  'In the dietsum don't consider import: assume import maintained
                Diet(iPred, i) = ExternalDiets.DietPref(iPred, i) 'DC_Old(pred, i)
                DClast(i) = Diet(iPred, i)
                If Diet(iPred, i) > 0 Then
                    DietSum = DietSum + Diet(iPred, i)
                    nPred += 1
                    IsPrey(i) = True
                End If
            Next

            'Remember any import
            Diet(iPred, 0) = ExternalDiets.DietPref(iPred, 0) 'DC_Old(pred, 0)
            If nPred = 1 Then
                For i = 1 To Me.nGroups
                    If IsPrey(i) Then Diet(iPred, i) = 1 - Diet(iPred, 0)
                Next
            Else        'iterate to find diets

                Diff = 1

                'Si() is internal(current model) biomass and eternal diets
                'si = ri/pi / sum(rn/pn) pi and pn calcualted on internal biomass
                CalcChessonAlpha(iPred, Si, Me.m_EcopathData.B, Diet)

                Do While Diff > 10 ^ -6 And Cnt < 30000
                    Diff = 0
                    Cnt = Cnt + 1
                    'Now we have the selection in Si and these should correspond to the Alpha's already calculated (module level variable)
                    For i = 1 To Me.nGroups
                        Diff = Diff + Math.Abs(Alpha(iPred, i) - Si(iPred, i))
                    Next
                    If Diff > 0 Then
                        For i = 1 To Me.nGroups
                            If Alpha(iPred, i) <> Si(iPred, i) Then
                                Ratio = Alpha(iPred, i) / Si(iPred, i)
                                If Ratio <> 1 Then

                                    ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                                    ''For Debugging
                                    'System.Console.Write(iPred.ToString + ", " + i.ToString + ", " + Diet(iPred, i).ToString)
                                    ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                                    Diet(iPred, i) = Diet(iPred, i) * Ratio
                                    Me.RescaleDietsToDietSum(iPred, Diet, DietSum)
                                    CalcChessonAlpha(iPred, Si, Me.m_EcopathData.B, Diet)

                                    ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                                    ''For Debugging
                                    'System.Console.Write(", " + Diet(iPred, i).ToString)
                                    'System.Console.WriteLine()
                                    ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                                End If
                                DClast(i) = Diet(iPred, i) 'store the last dc value
                            End If
                        Next
                    End If
                Loop
            End If
        Next


        Me.m_Core.SetBatchLock(EwECore.cCore.eBatchLockType.Update)

        'So now we know all there is to know about the diets:
        For iPred = 1 To Me.nLiving
            If Me.m_EcopathData.PP(iPred) < 1 Then 'a consumer
                For i = 0 To Me.nGroups
                    If (i <> 0) Then
                        'Normal group diet just update the diets
                        If Me.m_Core.EcoPathGroupInputs(iPred).DietComp(i) <> Diet(iPred, i) Then bNeedsBalancing = True
                        Me.m_Core.EcoPathGroupInputs(iPred).DietComp(i) = Diet(iPred, i)

                    Else '(i <> 0)
                        'Imported diet
                        If Diet(iPred, i) <> 0 Then
                            Me.m_Core.EcoPathGroupInputs(iPred).ImpDiet = Diet(iPred, i)
                        End If 'Diet(iPred, i) <> 0

                    End If '(i <> 0)

                Next i 'i = 0 To Me.nGroups
            End If 'Me.m_EcopathData.PP(iPred) < 1
        Next iPred

        Me.m_Core.ReleaseBatchLock(EwECore.cCore.eBatchChangeLevelFlags.Ecopath)

        If bNeedsBalancing Then
            '"Diets have been imported but not saved. You will need to make sure your model is balanced and save the new values." _
            '                                                    + EwEUtils.Utilities.cStringUtils.vbCrLf + "To revert the imported diets close the model without saving.",
            'Message that the model needs to balancing
            Me.m_Core.Messages.SendMessage(New EwECore.cMessage("Diets have been imported but not saved." + EwEUtils.Utilities.cStringUtils.vbCrLf + "To used the imported diets you will need to make sure your model is balanced then save the new values." _
                                                                + EwEUtils.Utilities.cStringUtils.vbCrLf + "To revert the imported diets close the model without saving.",
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))
        End If

        Return

    End Sub


    Public Sub RescaleDietsToDietSum(pred As Integer, ByRef Diet(,) As Single, OldDietSum As Double)
        Dim i As Integer
        Dim Sum As Double
        Dim newSum As Double
        Sum = 0
        For i = 1 To Me.nGroups
            Sum = Sum + Diet(pred, i)
        Next
        'It sums to Sum now, and should be made to sum to OldDietSum
        If Sum > 0 Then
            For i = 1 To Me.nGroups
                Diet(pred, i) = CSng(Diet(pred, i) * OldDietSum / Sum)
                If Diet(pred, i) < 0.0 Then
                    Diet(pred, i) = 0
                End If
                newSum += Diet(pred, i)
            Next
        End If

        'If newSum <> 1 Then
        '    System.Console.WriteLine(pred.ToString + "," + newSum.ToString)
        'End If
        'Debug.Assert(newSum <> 0, "Diet didn't sum to one...")

    End Sub


#Region "Orginal Code from EwE5"

    'Hide all this crap from the compiler
#If 0 Then
    
        Public Sub GetDietsBiomassCalculatePreference_old(mName As String)
            Dim col As Integer
            Dim i As Integer
            Dim row As Integer
            Dim s As String
            Dim SQL As String
            Dim GrpName As String
            Dim pred As Integer
            Dim RetVal As Variant
            Dim CCY As New clsConnect
            On Local Error GoTo exitSub
            'Species information
            ReDim B_Old(NumGroups)
            ReDim DC_Old(NumGroups, NumGroups)
            CCY.OpenConnection TempMDB

        SQL = "SELECT * from [Group Info] where modelName='" & mName & "'"
        Set y_Recordset = CCY.UpdatableRecords(SQL)
        RetVal = MsgBox("Estimate diet from preference, and overwrite existing diets", vbQuestion + vbYesNo, "Estimating diet composition")
            If RetVal <> vbYes Then Exit Sub

            If y_Recordset.RecordCount > 0 Then y_Recordset.MoveFirst
            Do While Not y_Recordset.EOF
                GrpName = y_Recordset.Fields("groupName").value
                For i = 1 To NumGroups
                    If GrpName = Specie(i) Then Exit For
                Next
                If i <= NumGroups Then  'The group name was found
                    B_Old(i) = y_Recordset!Biomass
                End If
                y_Recordset.MoveNext
            Loop

            For i = 1 To NumGroups
                If B_Old(i) < 0 Then
                    RetVal = InputBox("Enter biomass for #" + CStr(i) + ", " + Specie(i) + " in model you're importing from", "Missing biomass", 0)
                    If RetVal = "" Then
                        MsgBox "Incomplete information, aborting", vbOKOnly
                    GoTo exitSub
                    ElseIf RetVal > 0 Then
                        B_Old(i) = RetVal
                    End If
                End If
            Next
            For i = 1 To NumGroups
                If Bi(i) < 0 Then
                    MsgBox "All biomasses (current model) must be entered to use this routine", vbCritical + vbOKOnly
                GoTo exitSub
                End If
            Next i


            SQL = "SELECT * from [Group x Group] where modelName='" & mName & "'"
        Set y_Recordset = CCY.UpdatableRecords(SQL)
        If y_Recordset.RecordCount > 0 Then y_Recordset.MoveFirst
            Do While Not y_Recordset.EOF
                GrpName = y_Recordset!groupname 'the predator
                For i = 1 To NumGroups
                    If GrpName = Specie(i) Then Exit For
                Next
                If i <= NumGroups Then  'The group name was found
                    pred = i
                End If
                GrpName = y_Recordset!groupColName  'the prey
                For i = 1 To NumGroups
                    If GrpName = Specie(i) Then Exit For
                Next
                If i <= NumGroups Then  'The group name was found
                    'now knows pred and prey
                    DC_Old(pred, i) = y_Recordset!Diet
                    pred = i
                End If
                y_Recordset.MoveNext
            Loop


            ReDim Alpha(NumGroups, NumGroups)
            ReDim SumR(1 To NumGroups)
            SumBio = 0
            For i = 1 To NumGroups
                SumBio = SumBio + B(i)
            Next i
            For i = 1 To NumGroups               'CALCULATION OF PREFERENCE INDEX
                CalcChessonAlpha i, Alpha(), B_Old(), DC_Old()
        Next
            'We now have Alpha(i,j) known, and can estimate the DC's from sets of linear equations (one per predator)
            'InvertForDiet Alpha()
            IterateForDiet Alpha()
        frmInputData.DisplayDietComposition

    exitSub:
            CCY.CloseConnection
        End Sub


    
    Public Sub Chesson()
        Dim LivingBio As Single
        Dim MaxBio As Single
        Dim Alpha(,) As Single = New Single(nGroups, nGroups) {}
        Dim SumR() As Single = New Single(nGroups) {}


        MaxBio = 0
        LivingBio = 0
        SumBio = 0

        For i As Integer = 1 To Me.nLiving
            If Me.m_EcopathData.B(i) > MaxBio Then MaxBio = Me.m_EcopathData.B(i)
            LivingBio = LivingBio + Me.m_EcopathData.B(i)
        Next i
        SumBio = LivingBio

        'Will assume that if there is no me.m_Ecopathdata.b for a detritus box
        'then the me.m_Ecopathdata.b will correspond to the max living me.m_Ecopathdata.b
        'divided by the number of detritus boxes. Thus if all detritus
        'me.m_Ecopathdata.bes are lacking the total detritus me.m_Ecopathdata.b = max living biom.
        For i As Integer = Me.nLiving + 1 To Me.nGroups
            If Me.m_EcopathData.B(i) < 0 Then
                Me.m_EcopathData.B(i) = MaxBio / (nGroups - nLiving)
            End If
            SumBio = SumBio + Me.m_EcopathData.B(i)
        Next i%


        For i = 1 To Me.nGroups               'CALCULATION OF PREFERENCE INDEX
            SumR(i) = 0
            For j = 1 To Me.nGroups                    'FOLLOWING CHESSON (1983)
                Alpha(i, j) = 0
                If B(j) > 0 Then
                    Alpha(i, j) = Me.m_EcopathData.DC(i, j) / (Me.m_EcopathData.B(j) / SumBio)
                End If
                SumR(i) = SumR(i) + Alpha(i, j)
            Next j
        Next i

        For i = 1 To Me.nGroups
            For j = 1 To Me.nGroups
                If SumR(i) > 0 Then
                    Alpha(i, j) = Alpha(i, j) / SumR(i)
                End If
            Next j               'THIS ALPHA IS THE SAME AS CHESSONS ALPHA
        Next i

        For i = 1 To Me.nGroups
            If Me.m_EcopathData.QB(i) > 0 Then
                For j = 1 To Me.nGroups
                    Alpha(i, j) = (Me.nGroups * Alpha(i, j) - 1) / ((Me.nGroups - 2) * Alpha(i, j) + 1)
                Next j
            End If                     'THIS ALPHA EQUALS CHESSONS EPSILON
        Next i

    End Sub

Public Sub IterateForDiet(Alpha() As Single)
    Dim Cnt As Integer
    Dim DClast() As Single
    Dim Diet() As Single
    Dim DietSum As Single
    Dim Diff As Single
    Dim i As Integer
    Dim IsPrey() As Boolean
    Dim j As Integer
    Dim pred As Integer
    Dim prey As Integer
    Dim Ratio As Single
    Dim Si() As Single  'is the selection index (alpha) for prey i
        On Local Error GoTo exitSub
        ReDim Diet(NumLiving, NumGroups)
        ReDim Si(NumLiving, NumGroups)
        For pred = 1 To NumLiving
            ReDim DClast(NumGroups)
            ReDim IsPrey(NumGroups)
            prey = 0
            DietSum = 0
            Cnt = 0
            For i = 1 To NumGroups  'In the dietsum don't consider import: assume import maintained
                Diet(pred, i) = DC_Old(pred, i)
                DClast(i) = Diet(pred, i)
                If Diet(pred, i) > 0 Then
                    DietSum = DietSum + Diet(pred, i)
                    prey = prey + 1
                    IsPrey(i) = True
                End If
            Next
            'Remember any import
            Diet(pred, 0) = DC_Old(pred, 0)
            If prey = 1 Then
                For i = 1 To NumGroups
                    If IsPrey(i) Then Diet(pred, i) = 1 - Diet(pred, 0)
                Next
            Else        'iterate to find diets
                Diff = 1
                CalcChessonAlpha pred, Si(), Bi(), Diet()
                Do While Diff > 10 ^ -6 And Cnt < 30000
                    Diff = 0
                    Cnt = Cnt + 1
                    'Now we have the selection in Si and these should correspond to the Alpha's already calculated (module level variable)
                    For i = 1 To NumGroups
                        Diff = Diff + Abs(Alpha(pred, i) - Si(pred, i))
                    Next
                    If Diff > 0 Then
                        For i = 1 To NumGroups
                            If Alpha(pred, i) <> Si(pred, i) Then
                                Ratio = Alpha(pred, i) / Si(pred, i)
                                If Ratio <> 1 Then
                                    Diet(pred, i) = Diet(pred, i) * Ratio
                                    RescaleDietsToDietSum pred, Diet(), DietSum
                                    CalcChessonAlpha pred, Si(), Bi(), Diet()
                                End If
                                DClast(i) = Diet(pred, i) 'store the last dc value
                            End If
                        Next
                    End If
                Loop
            End If
        Next
        'So now we know all there is to know about the diets:
        For pred = 1 To NumLiving
            If PP(pred) < 1 Then 'a consumer
                For i = 0 To NumGroups  'start at 0 to include import
                    DCi(pred, i) = Diet(pred, i)
                Next
            End If
        Next
    Exit Sub
    exitSub:
        MsgBox "Error in IterateForDiet"
End Sub

Public Sub RescaleDietsToDietSum(pred As Integer, Diet() As Single, OldDietSum As Single)
Dim i As Integer
Dim Sum As Single
    Sum = 0
    For i = 1 To NumGroups
        Sum = Sum + Diet(pred, i)
    Next
    'It sums to Sum now, and should be made to sum to OldDietSum
    If Sum > 0 Then
        For i = 1 To NumGroups
            Diet(pred, i) = Diet(pred, i) * OldDietSum / Sum
        Next
    End If
End Sub
    
#End If


#End Region


End Class
