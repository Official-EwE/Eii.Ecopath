#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities
Imports EwEUtils.Database.cEwEDatabase
Imports System.Reflection

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' One single flow diagram.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public Class cFlowDiagram
    : Inherits cOOPStorable

#Region " Private vars "

    Private m_strName As String = "Default"

#End Region ' Private vars

#Region " Properties "

    <Browsable(True), _
       DisplayName("Name"), _
       Description("Name of this diagram"), _
       cPropertySorter.PropertyOrder(1)> _
    Public Overridable Property Name() As String
        Get
            Return Me.m_strName
        End Get
        Set(ByVal strName As String)
            Me.m_strName = strName
        End Set
    End Property

#End Region

End Class
