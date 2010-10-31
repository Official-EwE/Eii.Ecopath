#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmFunctionalResponse

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

#If 0 Then

    Private Sub MakeFuncPlot(ByVal picXY As PictureBox, ByVal Wt As Single, ByVal ht As Single, ByVal tp As Single, ByVal lt As Single)
        Dim Cnt As Integer
        Dim color As Object
        Dim MaxBio() As Single
        Dim minX As Single
        Dim maxX As Single
        Dim maxY As Single
        Dim OldX As Single
        Dim OldY As Single
        Dim prey As Integer
        Dim Tm As Integer
        Dim X As Single
        Dim Y As Single

        With picXY
            .Cls()
            .Width = Wt
            .Height = ht
            .Top = tp
            .Left = lt
            .Visible = True
        End With

        'find max x (biomass) value in Simplot(prey, 0, ntimes):
        ReDim MaxBio(NumGroups)
        minX = 0 '10000
        For prey = 1 To NumGroups
            If val(prey) = -10 And StartBiomass(prey) > 0 Then 'it's a prey in the top 20
                For Tm = 1 To Ntimes
                    If SimPlot(prey, 0, Tm) > MaxBio(prey) Then MaxBio(prey) = SimPlot(prey, 0, Tm)
                    If SimPlot(prey, 0, Tm) / StartBiomass(prey) < minX Then minX = SimPlot(prey, 0, Tm) / StartBiomass(prey)
                Next
                'Find the prey that has changed most relative to the Ecopath biomass:
                If MaxBio(prey) / StartBiomass(prey) > maxX Then maxX = MaxBio(prey) / StartBiomass(prey)
            End If
        Next
        'Get the scaling for the one with the max change:
        minX = (CInt((minX * 10 - 0.5))) / 10
        maxX = (CInt((maxX * 10 - 0.5))) / 10 + 0.1 'maxX + 0.1

        'Scaling for y-axis needs to be calculated, the y-axis displays Q of prey / B prey
        'pred = sel; info is saved in
        'SimPlotPred(prey, pred, itime) and SimPlotPrey(prey, pred, itime) as amounts consumed
        maxY = 0
        For prey = 1 To NumGroups : For Tm = 1 To Ntimes
                If SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm) > maxY Then maxY = SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)
            Next : Next
        maxY = maxY * 1.1

        If maxX = 0 Or maxY = 0 Then Exit Sub
    picXY.Scale (minX - 0.1 * maxX, 1.1 * maxY)-(1.1 * maxX, -0.1 * maxY)
    picXY.Line (minX, maxY)-(minX, 0)
    picXY.Line (minX, 0)-(maxX, 0)
    picXY.Line (maxX, 0)-(maxX, 0.01 * maxY)
    picXY.Line (1, 0)-(1, 0.01 * maxY)
    picXY.Line (minX, maxY)-(minX + 0.01 * maxX, maxY)
        PrintSome(picXY, minX + 0.35 * (maxX - minX), -0.04 * maxY, "Prey biomass relative to Ecopath biomass", QBColor(0))
        PrintSome(picXY, minX - 0.005 * maxX, -0.01 * maxY, Format(minX, "0.0"), QBColor(0))
        PrintSome(picXY, 1 - 0.005 * maxX, -0.01 * maxY, "1", QBColor(0))
        PrintSome(picXY, maxX - 0.015 * maxX, -0.01 * maxY, Format(maxX, "0.0"), QBColor(0))

        PrintSome(picXY, minX - 0.07 * maxX, 1.08 * maxY, "Q prey / B pred", QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY, Format(maxY, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY * 2 / 3, Format(maxY * 2 / 3, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 1.02 * maxY / 3, Format(maxY / 3, "0.00"), QBColor(0))
        PrintSome(picXY, minX - 0.07 * maxX, 0.01 * maxY, "0.00", QBColor(0))
        For prey = 1 To NumGroups
            If val(prey) = -10 And StartBiomass(prey) > 0 Then 'it's a prey in the top 20
                OldX = SimPlot(prey, 0, 1) / StartBiomass(prey)
                OldY = SimPlotPred(prey, Sel, 1) / SimPlot(Sel, 0, 1) 'Elect(Sel, prey, 1)
                picXY.PSet(OldX, OldY)
                picXY.DrawWidth = 2
                color = PoolColor(prey)
                For Tm = 2 To Ntimes
                picXY.Line -(SimPlot(prey, 0, Tm) / StartBiomass(prey), SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)), color
                    ' OldX = SimPlot(prey, 0, Tm) / StartBiomass(prey)
                    ' OldY = SimPlotPred(prey, Sel, Tm) / SimPlot(Sel, 0, Tm)
                Next
            End If
        Next

    End Sub

    Private Sub PrintSome(ByVal pic As PictureBox, ByVal X As Single, ByVal Y As Single, ByVal Text As String, ByVal color As Object)
        pic.CurrentX = X
        pic.CurrentY = Y
        pic.ForeColor = color
        pic.Print(Text)
    End Sub

#End If

End Class