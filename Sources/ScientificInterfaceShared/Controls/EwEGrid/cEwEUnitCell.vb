' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' UnitCell implements a cell that shows a dynamic unit string.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class cEwEUnitCell
        Inherits cEwECell

        Protected m_strUnit As String = ""
        Protected m_strUnitMask As String = ""

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(strUnit As String)
            Me.New("{0}", strUnit)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(strUnitMask As String, strUnit As String)
            MyBase.New(Nothing, GetType(String))

            Me.m_strUnitMask = strUnitMask
            Me.m_strUnit = strUnit
        End Sub

#End Region ' Construction 

#Region " Overrides "

        Public Overrides ReadOnly Property DisplayText() As String
            Get
                Dim strDisplayText As String = ""
                If (Not String.IsNullOrWhiteSpace(Me.m_strUnit)) Then
                    strDisplayText = cStringUtils.Localize(Me.m_strUnitMask, Me.GetUnitString(Me.m_strUnit))
                End If
                Return strDisplayText
            End Get
        End Property

        Private Function GetUnitString(strUnit As String) As String

            If (Me.StyleGuide Is Nothing) Then Return "u1"
            Return Me.StyleGuide.FormatUnitString(strUnit)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that this cell cannot be edited.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Return (MyBase.Style Or cStyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(styleNew As cStyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or cStyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Overrides

    End Class

End Namespace
