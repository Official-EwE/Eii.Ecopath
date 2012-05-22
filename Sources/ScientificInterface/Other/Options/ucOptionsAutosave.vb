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
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Autosave interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsAutosave
        Implements IOptionsPage

        Private m_uic As cUIContext = Nothing

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            'Me.m_tsddFields.DropDown.Items.Clear()
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_cbAutosaveAll.CheckState = CheckState.Indeterminate
            Me.m_cbEcosim.CheckState = CheckState.Indeterminate
            Me.m_cbEcospace.CheckState = CheckState.Indeterminate

            With Me.m_uic.Core
                Me.m_cbEcosimRun.Checked = .Autosave(eAutosaveTypes.EcosimRun)
                Me.m_cbMonteCarlo.Checked = .Autosave(eAutosaveTypes.MonteCarlo)
                Me.m_cbMSE.Checked = .Autosave(eAutosaveTypes.MSE)
                Me.m_cbSpaceCSV.Checked = .Autosave(eAutosaveTypes.EcospaceCSV)
                Me.m_cbSpaceASCII.Checked = .Autosave(eAutosaveTypes.EcospaceASC)
                Me.m_cbEcotracer.Checked = .Autosave(eAutosaveTypes.Ecotracer)
            End With

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.Apply"/>.</inheritdocs>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success
            Try
                With Me.m_uic.Core
                    .Autosave(eAutosaveTypes.EcosimRun) = Me.m_cbEcosimRun.Checked
                    .Autosave(eAutosaveTypes.MonteCarlo) = Me.m_cbMonteCarlo.Checked
                    .Autosave(eAutosaveTypes.MSE) = Me.m_cbMSE.Checked
                    .Autosave(eAutosaveTypes.EcospaceCSV) = Me.m_cbSpaceCSV.Checked
                    .Autosave(eAutosaveTypes.EcospaceASC) = Me.m_cbSpaceASCII.Checked
                    .Autosave(eAutosaveTypes.Ecotracer) = Me.m_cbEcotracer.Checked
                End With

            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::Apply")
                result = IOptionsPage.eApplyResultType.Failed
            End Try
            Return result

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.SetDefaults"/>.</inheritdocs>
        ''' -------------------------------------------------------------------
        Public Sub SetDefaults() Implements IOptionsPage.SetDefaults

            Try
                Me.m_cbAutosaveAll.Checked = False
            Catch ex As Exception
                cLog.Write(ex, "ucOptionsAutosave::SetDefaults")
            End Try

        End Sub

#End Region ' Overrides

#Region " Event handlers "

        Private Sub SaveAllClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbAutosaveAll.Click
            Me.m_cbEcosim.Checked = Me.m_cbAutosaveAll.Checked
            Me.m_cbEcospace.Checked = Me.m_cbAutosaveAll.Checked
            Me.m_cbEcotracer.Checked = Me.m_cbAutosaveAll.Checked
            Me.EcosimClicked(sender, e)
            Me.EcospaceClicked(sender, e)
        End Sub

        Private Sub EcosimClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbEcosim.Click
            Me.m_cbEcosimRun.Checked = Me.m_cbEcosim.Checked
            Me.m_cbMonteCarlo.Checked = Me.m_cbEcosim.Checked
            Me.m_cbMSE.Checked = Me.m_cbEcosim.Checked
        End Sub

        Private Sub EcospaceClicked(sender As System.Object, e As System.EventArgs) _
            Handles m_cbEcospace.Click
            Me.m_cbSpaceASCII.Checked = Me.m_cbEcospace.Checked
            Me.m_cbSpaceCSV.Checked = Me.m_cbEcospace.Checked
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
