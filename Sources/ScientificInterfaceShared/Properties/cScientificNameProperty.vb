' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports ScientificInterfaceShared.Style



Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Local property to format scientific names
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cScientificNameProperty
        Inherits cStringProperty

        Private m_src As cTaxon = Nothing
        Private m_pm As cPropertyManager = Nothing

        Private m_propName As cStringProperty = Nothing
        Private m_propSpecies As cStringProperty = Nothing
        Private m_propGenus As cStringProperty = Nothing

        Public Sub New(pm As cPropertyManager, src As cTaxon)
            MyBase.New()

            ' Do not connect to baseclass PropertyManager; this property is totally superficial!
            Me.m_pm = pm
            Me.m_src = src

            Me.m_propName = Me.Register(eVarNameFlags.Name)
            Me.m_propSpecies = Me.Register(eVarNameFlags.Species)
            Me.m_propGenus = Me.Register(eVarNameFlags.Genus)

        End Sub

        Protected Friend Overrides Sub Dispose(bDisposing As Boolean)
            Me.Unregister(Me.m_propName)
            Me.Unregister(Me.m_propSpecies)
            Me.Unregister(Me.m_propGenus)
            MyBase.Dispose(bDisposing)
        End Sub

        Protected Overrides Property Style As ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags
            Get
                Dim strGenus As String = CStr(Me.m_propGenus.GetValue())
                Dim strSpecies As String = CStr(Me.m_propSpecies.GetValue())

                If String.IsNullOrWhiteSpace(strGenus) Or String.IsNullOrEmpty(strSpecies) Then
                    Return cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                End If
                Return cStyleGuide.eStyleFlags.Taxon Or cStyleGuide.eStyleFlags.NotEditable
            End Get
            Set(value As ScientificInterfaceShared.Style.cStyleGuide.eStyleFlags)
                ' NOP
            End Set
        End Property

        Protected Overrides Property Value(Optional bHonourNull As Boolean = True) As Object
            Get
                ' Do not try to properly capitalize; .NET has no built-in function for this that works under for all languages! Better not try to be too smart
                ' Do not localize the genus + species formatting; keep it fixed here
                Dim strGenus As String = CStr(Me.m_propGenus.GetValue())
                Dim strSpecies As String = CStr(Me.m_propSpecies.GetValue())

                If String.IsNullOrWhiteSpace(strGenus) Or String.IsNullOrEmpty(strSpecies) Then
                    Return CStr(Me.m_propName.GetValue())
                End If
                Return String.Format(My.Resources.GENERIC_LABEL_DOUBLE, strGenus, strSpecies)
            End Get
            Set(value As Object)
                ' NOP
            End Set
        End Property

#Region " Internals "

        Private m_bInUpdate As Boolean = False

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            ' To avoid infinite loops due to poor use
            If (Me.m_bInUpdate) Then Return
            Me.m_bInUpdate = True
            ' Pass it on!
            Me.FireChangeNotification(eChangeFlags.Value)
            Me.m_bInUpdate = False
        End Sub

        Private Function Register(vn As eVarNameFlags) As cStringProperty
            Dim prop As cStringProperty = DirectCast(Me.m_pm.GetProperty(Me.m_src, vn), cStringProperty)
            AddHandler prop.PropertyChanged, AddressOf Me.OnPropertyChanged
            Return prop
        End Function

        Private Sub Unregister(prop As cStringProperty)
            RemoveHandler prop.PropertyChanged, AddressOf Me.OnPropertyChanged
        End Sub

#End Region ' Internals

    End Class

End Namespace
