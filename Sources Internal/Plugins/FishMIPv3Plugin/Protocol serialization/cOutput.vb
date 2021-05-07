' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Xml.Serialization

#End Region ' Imports

''' <summary>
''' Configuration class that states which groups contribute to a given output.
''' </summary>
Public Class cOutput

    Private m_groupContribution As New Dictionary(Of Integer, Single)

    <XmlAttribute("name")>
    Public Property Name As String = ""
    <XmlAttribute("description")>
    Public Property Description As String = ""
    <XmlAttribute("comments")>
    Public Property Comments As String = ""
    <XmlAttribute("mandatory")>
    Public Property IsMandatory As Boolean = False
    <XmlAttribute("min-trophic-level")>
    Public Property TLMin As Single = 0.0
    <XmlAttribute("max-trophic-level")>
    Public Property TLMax As Single = 10.0

    Public ReadOnly Property IsBiomass As Boolean
        Get
            If Me.Description.ToLower().Contains("biomass") Then Return True
            If Me.Comments.ToLower().Contains("biomass") Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsCatch As Boolean
        Get
            If Me.Description.ToLower().Contains("catch") Then Return True
            If Me.Comments.ToLower().Contains("catch") Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsConsumer As Boolean
        Get
            If Me.Description.ToLower().Contains("consumer") Then Return True
            If Me.Comments.ToLower().Contains("consumer") Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsPelagic As Boolean
        Get
            If Me.Description.ToLower().Contains("pelagic") Then Return True
            If Me.Comments.ToLower().Contains("pelagic") Then Return True
            Return False
        End Get
    End Property

    Public ReadOnly Property IsDemersal As Boolean
        Get
            If Me.Description.ToLower().Contains("demersal") Then Return True
            If Me.Comments.ToLower().Contains("demersal") Then Return True
            Return False
        End Get
    End Property

    Default Public Property Group(igroup As Integer) As Single
        Get
            If (Not Me.m_groupContribution.ContainsKey(igroup)) Then Return 0
            Return Me.m_groupContribution(igroup)
        End Get
        Set(value As Single)
            If (value > 0) Then
                Me.m_groupContribution(igroup) = value
            ElseIf Me.m_groupContribution.ContainsKey(igroup) Then
                Me.m_groupContribution.Remove(igroup)
            End If
        End Set
    End Property

    Public Sub Clear()
        Me.m_groupContribution.Clear()
    End Sub

    Public ReadOnly Property NumGroups As Integer
        Get
            Return Me.m_groupContribution.Count
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.Name & " (" & CStr(Me.NumGroups) & " groups)"
    End Function

End Class
