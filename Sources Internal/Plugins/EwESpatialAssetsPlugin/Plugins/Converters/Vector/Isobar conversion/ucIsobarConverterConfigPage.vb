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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.SpatialData

Namespace SpatialData

    Public Class ucIsobarConverterConfigPage
        Implements IUIElement
        Implements IOptionsPage

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext

        Public Property Converter As cIsobarConverterPlugin

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Debug.Assert(Me.Converter IsNot Nothing)
            Debug.Assert(Me.Converter.Dataset IsNot Nothing)

            For Each strAttrib As String In Me.Converter.Dataset.GetAttributes
                Me.m_cmbAttribute.Items.Add(strAttrib)
            Next
            Me.m_cmbAttribute.SelectedItem = Me.Converter.AttributeName
        End Sub

        Public Function Apply() As ScientificInterfaceShared.Controls.IOptionsPage.eApplyResultType _
            Implements ScientificInterfaceShared.Controls.IOptionsPage.Apply
            Me.Converter.AttributeName = Me.m_cmbAttribute.Text
            Return IOptionsPage.eApplyResultType.Success
        End Function

        Public Function CanApply() As Boolean _
            Implements ScientificInterfaceShared.Controls.IOptionsPage.CanApply
            Return True
        End Function

        Public Event OnChanged(sender As ScientificInterfaceShared.Controls.IOptionsPage, args As System.EventArgs) _
            Implements ScientificInterfaceShared.Controls.IOptionsPage.OnChanged

        Public Sub SetDefaults() Implements ScientificInterfaceShared.Controls.IOptionsPage.SetDefaults
            ' NOP
        End Sub

    End Class

End Namespace
