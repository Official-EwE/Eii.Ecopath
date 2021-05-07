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

Imports System.Xml.Serialization

<XmlRoot("run-protocol")>
Public Class cProtocol

    <XmlAttribute("name")>
    Public Property Name As String = ""

    ''' <summary>The earth system models (or global climate models)</summary>
    <XmlArray("gcms")>
    <XmlArrayItem("gcm")>
    Public GCMs As New List(Of cGCM)

    ''' <summary>The ocean regions for this simulation</summary>
    <XmlArray("ocean-regions")>
    <XmlArrayItem("ocean-region")>
    Public OceanRegions As New List(Of cOceanRegion)

    ''' <summary>The run periods defined for the protocol</summary>
    <XmlArray("periods")>
    <XmlArrayItem("period")>
    Public Periods As New List(Of cPeriod)

    ''' <summary>The climate scenarios defined for the protocol</summary>
    <XmlArray("climate-scenarios")>
    <XmlArrayItem("climate-scenario")>
    Public ClimateScenarios As New List(Of cClimateScenario)

    ''' <summary>The socio-economic scenarios defined for the protocol</summary>
    <XmlArray("socio-econ-scenarios")>
    <XmlArrayItem("socio-econ-scenario")>
    Public SocioEconScenarios As New List(Of cSocioEconomicScenario)

    <XmlArray("experiments")>
    <XmlArrayItem("experiment")>
    Public Experiments As New List(Of cExperiment)

    <XmlArray("variables")>
    <XmlArrayItem("variable")>
    Public Variables As New List(Of cVariable)

    <XmlArray("outputs")>
    <XmlArrayItem("output")>
    Public Outputs As New List(Of cOutput)

    <XmlElement("output-file-mask")>
    Public OutputFileMask As String = ""

    <XmlArray("indicators")>
    <XmlArrayItem("indicator")>
    Public Indicators As New List(Of String)

End Class
