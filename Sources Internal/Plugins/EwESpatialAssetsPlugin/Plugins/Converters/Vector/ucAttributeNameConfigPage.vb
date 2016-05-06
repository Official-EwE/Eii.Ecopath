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

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.SpatialData
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class ucAttributeNameConfigPage
        Implements IUIElement
        Implements IOptionsPage

#Region " Private helper classes "

        Private Class cAttributeItem

            Private m_strAttribute As String = ""
            Private m_type As Type = Nothing

            Public Sub New()
            End Sub

            Public Sub New(ByVal strAttribute As String, t As Type)
                Me.m_strAttribute = strAttribute
                Me.m_type = t
            End Sub

            Public Overrides Function ToString() As String

                If (String.IsNullOrWhiteSpace(Me.m_strAttribute) Or (Me.m_type Is Nothing)) Then
                    Return SharedResources.GENERIC_VALUE_NONE
                End If

                Dim fmt As New cTypeTypeFormatter()
                Return String.Format(SharedResources.GENERIC_LABEL_DETAILED, _
                                     Me.m_strAttribute, _
                                     fmt.GetDescriptor(Me.m_type))
            End Function

            Public ReadOnly Property Attribute As String
                Get
                    Return Me.m_strAttribute
                End Get
            End Property

            Public ReadOnly Property Type As Type
                Get
                    Return Me.m_type
                End Get
            End Property
        End Class

#End Region ' Private helper classes

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext

        Public Property Converter As cSpatialDataConverter

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Debug.Assert(Me.Converter IsNot Nothing)
            Debug.Assert(Me.Converter.Dataset IsNot Nothing)

            Dim astrAttrib() As String = Me.Converter.Dataset.GetAttributes
            Dim atpAttrib() As Type = Me.Converter.Dataset.GetAttributeDataTypes
            Dim item As cAttributeItem = New cAttributeItem()

            Me.m_cmbAttribute.Items.Add(item)
            Me.m_cmbAttribute.SelectedItem = item

            For i As Integer = 0 To astrAttrib.Length - 1
                Dim strName As String = astrAttrib(i)
                item = New cAttributeItem(astrAttrib(i), atpAttrib(i))
                Me.m_cmbAttribute.Items.Add(item)
                If (Me.Converter.AttributeName = strName) Then
                    Me.m_cmbAttribute.SelectedItem = item
                End If
            Next
        End Sub

        Public Function Apply() As ScientificInterfaceShared.Controls.IOptionsPage.eApplyResultType _
            Implements ScientificInterfaceShared.Controls.IOptionsPage.Apply

            Dim item As cAttributeItem = DirectCast(Me.m_cmbAttribute.SelectedItem, cAttributeItem)
            Me.Converter.AttributeName = item.Attribute
            Me.Converter.AttributeFilter = ""

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

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IOptionsPage.CanSetDefaults"/>
        ''' -------------------------------------------------------------------
        Public Function CanSetDefaults() As Boolean _
            Implements IOptionsPage.CanSetDefaults
            Return False
        End Function

    End Class

End Namespace
