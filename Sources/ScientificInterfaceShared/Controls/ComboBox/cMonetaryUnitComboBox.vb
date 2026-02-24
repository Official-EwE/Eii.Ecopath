' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Globalization
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Combo box that allows the user to select a monetary unit.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMonetaryUnitComboBox
        Implements IUIElement

#Region " Helper classes "

        Private Class MonetaryUnitItem

            Private m_strDescription As String

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="m_strISOSymbol"></param>
            ''' <param name="strDescription"></param>
            ''' <remarks></remarks>
            Public Sub New(m_strISOSymbol As String, strDescription As String)
                Me.ISOSymbol = m_strISOSymbol
                Me.m_strDescription = strDescription
            End Sub

            Public ReadOnly Property ISOSymbol() As String = ""

            Public Overrides Function ToString() As String
                Return String.Format(My.Resources.GENERIC_LABEL_DETAILED, Me.ISOSymbol, Me.m_strDescription)
            End Function

        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private m_uic As cUIContext = Nothing

#End Region ' Private vars

        Public Sub New()
            Me.InitializeComponent()
            'Me.DropDownStyle = ComboBoxStyle.DropDownList
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                Me.m_uic = value
                Me.Populate()
            End Set
        End Property

        Private Sub Populate()

            Dim fmt As New cMonetaryTypeFormatter()

            If Me.m_uic Is Nothing Then Return

            Me.SuspendLayout()

            For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                Try
                    Dim ri As New RegionInfo(ci.LCID)
                    Dim strAbbr As String = fmt.ToString(ri, eDescriptorTypes.Abbreviation)
                    Dim strName As String = fmt.ToString(ri, eDescriptorTypes.Name)

                    If Me.GetUnitIndex(strAbbr) = -1 Then
                        Me.Items.Add(New MonetaryUnitItem(strAbbr, strName))
                    End If
                Catch ex As Exception
                    ' Swallow this
                End Try
            Next

            Me.Sorted = True
            Me.ResumeLayout()

        End Sub

        Public Property Unit() As String
            Get
                If TypeOf Me.SelectedItem Is MonetaryUnitItem Then
                    Return DirectCast(Me.SelectedItem, MonetaryUnitItem).ISOSymbol
                Else
                    Return Me.Text
                End If
            End Get
            Set(value As String)
                Dim i As Integer = Me.GetUnitIndex(value)
                If (i < 0) Then
                    Me.Text = value
                Else
                    Me.SelectedIndex = i
                End If
            End Set
        End Property

        Public Function GetUnitIndex(strUnit As String) As Integer
            For iItem As Integer = 0 To Me.Items.Count - 1
                If TypeOf Me.Items(iItem) Is MonetaryUnitItem Then
                    If DirectCast(Me.Items(iItem), MonetaryUnitItem).ISOSymbol = strUnit Then
                        Return iItem
                    End If
                End If
            Next
            Return -1
        End Function
    End Class

End Namespace
