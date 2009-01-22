'==============================================================================
'
' $Log: cEconomicDataAdapter.vb,v $
' Revision 1.1  2009/01/22 18:36:49  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEconomicDataAdapter

#Region " Privates "

    Private m_core As cCore = Nothing

#End Region ' Privates

#Region " Constructor "

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Constructor

#Region " Public properties "

    Public ReadOnly Property EmployentValue() As Single
        Get
            Dim ecodata As IEconomicData = Me.GetEconomicData()
            If ecodata Is Nothing Then Return Me.m_core.m_SearchData.Employ
            Return ecodata.EmploymentValue
        End Get
    End Property

    Public ReadOnly Property TotalValue() As Single
        Get
            Dim ecodata As IEconomicData = Me.GetEconomicData()
            If ecodata Is Nothing Then Return Me.m_core.m_SearchData.totval
            Return ecodata.TotalValue
        End Get
    End Property

#End Region ' Public properties

#Region " Internals "

    Private Function GetEconomicData() As IEconomicData

        Dim adata As IEconomicData()

        If Me.m_core.PluginManager IsNot Nothing Then
            adata = DirectCast(Me.m_core.PluginManager.GetData(GetType(IEconomicData)), IEconomicData())
        End If

        If (adata Is Nothing) Then Return Nothing
        If (adata.Length = 0) Then Return Nothing

        ' ToDo: figure out how to deal with multiple objects
        Return adata(0)

    End Function

#End Region ' Internals

End Class
