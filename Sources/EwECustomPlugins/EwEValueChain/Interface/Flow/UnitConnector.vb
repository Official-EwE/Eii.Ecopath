#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class UnitConnector

    Private Shared _iColorRotator_ As Integer = 0

    Private m_strName As String
    Private m_link As cLink
    Private m_color As Color

    Public Sub New(ByVal Link As cLink)
        Me.m_link = Link
        Me.m_color = Me.GetNextColor()
    End Sub

    Public ReadOnly Property Link() As cLink
        Get
            Return Me.m_link
        End Get
    End Property

    Public ReadOnly Property Color() As Color
        Get
            Return Me.m_color
        End Get
    End Property

#Region " Internals "

    Private Function GetNextColor() As Color
        _iColorRotator_ = (_iColorRotator_ + 1) Mod (255 * 255 * 255 - 1)

        Dim iRed As Byte = CByte((_iColorRotator_ >> 16) And &HFF)
        Dim iGreen As Byte = CByte((_iColorRotator_ >> 8) And &HFF)
        Dim iBlue As Byte = CByte(_iColorRotator_ And &HFF)

        Return Color.FromArgb(255, iRed, iGreen, iBlue)
    End Function

#End Region ' Internals

End Class
