'==============================================================================
'
' $Log: cEconomicDataAdapter.vb,v $
' Revision 1.3  2009/01/24 17:44:37  joeb
' Added ProfitByFleet(Fleet) and EmploymentValueByFleet(Fleet) to Economic Adapters
'
' Revision 1.2  2009/01/22 19:04:45  jeroens
' Renamed property
'
' Revision 1.1  2009/01/22 18:36:49  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEPlugin.Data

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

    Public ReadOnly Property EmploymentValue() As Single
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


    ''' <summary>Summary of Profit by Fleet</summary>
    Public ReadOnly Property ProfitByFleet(ByVal FleetIndex As Integer) As Single
        Get
            Dim ecodata As IEconomicData = Me.GetEconomicData()
            If ecodata Is Nothing Then Return Me.m_core.m_EcoSimData.ProfitByFleet(FleetIndex)
            Return ecodata.ProfitByFleet(FleetIndex)
        End Get
    End Property

    ''' <summary>Summary of Jobs by Fleet</summary>
    Public ReadOnly Property EmploymentValueByFleet(ByVal FleetIndex As Integer) As Single
        Get
            Dim ecodata As IEconomicData = Me.GetEconomicData()
            If ecodata Is Nothing Then Return Me.m_core.m_EcoSimData.EmploymentValueByFleet(FleetIndex)
            Return ecodata.EmploymentValueByFleet(FleetIndex)
        End Get
    End Property

#End Region ' Public properties

#Region " Internals "

    Private Function GetEconomicData() As IEconomicData

        Dim adata As IPluginData()

        If Me.m_core.PluginManager IsNot Nothing Then
            adata = DirectCast(Me.m_core.PluginManager.GetData(GetType(IEconomicData)), IPluginData())
        End If

        If (adata Is Nothing) Then Return Nothing
        If (adata.Length = 0) Then Return Nothing

        ' ToDo: figure out how to deal with multiple objects
        If TypeOf adata Is IEconomicData Then Return DirectCast(adata(0), IEconomicData)

        Return Nothing

    End Function

#End Region ' Internals

End Class
