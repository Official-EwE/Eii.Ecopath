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
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > Map settings interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsMap
        Implements IOptionsPage

#Region " Variables "

        ''' <summary>Only ref to core.</summary>
        Private m_uic As cUIContext = Nothing
        Private m_fpSouth As cEwEFormatProvider = Nothing
        Private m_fpNorth As cEwEFormatProvider = Nothing
        Private m_fpWest As cEwEFormatProvider = Nothing
        Private m_fpEast As cEwEFormatProvider = Nothing

#End Region ' Variables

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)

            Me.m_uic = uic
            Me.InitializeComponent()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                Me.m_fpNorth.Release()
                Me.m_fpSouth.Release()
                Me.m_fpEast.Release()
                Me.m_fpWest.Release()
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub m_btnChoose_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnChoose.Click

            Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Select map reference image", Me.m_tbxFile.Text, "Map image files|*.png;*.gif;*.emf;*.wmf")
            If ofd.ShowDialog = DialogResult.OK Then
                Me.m_tbxFile.Text = ofd.FileName
            End If

        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save colour selections back to the style guide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            ' Apply colors to the style guide
            sg.SuspendEvents()

            sg.MapReferenceLayerFile = Me.m_tbxFile.Text
            sg.MapReferenceLayerTL = New PointF(CSng(Me.m_fpWest.Value), CSng(Me.m_fpNorth.Value))
            sg.MapReferenceLayerBR = New PointF(CSng(Me.m_fpEast.Value), CSng(Me.m_fpSouth.Value))
       
            sg.ResumeEvents()
            Return IOptionsPage.eApplyResultType.Success

        End Function

#End Region ' Public methods

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method to enable and update UI controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            If (Me.m_uic Is Nothing) Then Return

            Me.m_fpNorth = New cEwEFormatProvider(Me.m_uic, Me.m_nudNorth, GetType(Single))
            Me.m_fpSouth = New cEwEFormatProvider(Me.m_uic, Me.m_nudSouth, GetType(Single))
            Me.m_fpEast = New cEwEFormatProvider(Me.m_uic, Me.m_nudEast, GetType(Single))
            Me.m_fpWest = New cEwEFormatProvider(Me.m_uic, Me.m_nudWest, GetType(Single))

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Me.m_tbxFile.Text = sg.MapReferenceLayerFile
            Me.m_fpNorth.Value = sg.MapReferenceLayerTL.Y
            Me.m_fpSouth.Value = sg.MapReferenceLayerBR.Y
            Me.m_fpEast.Value = sg.MapReferenceLayerBR.X
            Me.m_fpWest.Value = sg.MapReferenceLayerTL.X

        End Sub

        Private Sub UpdatePreviewMap()

        End Sub

#End Region ' Helper methods

    End Class


End Namespace


