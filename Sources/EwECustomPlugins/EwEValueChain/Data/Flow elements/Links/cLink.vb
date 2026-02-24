' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports System.Runtime.Remoting
Imports EwECore.Database
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

''' ===========================================================================
''' <summary>
''' Base class for holding link information in the flow.
''' </summary>
''' <remarks>
''' Note that this class does not hold the actual references to flow units.
''' This class is a mere holder of shared behaviour between cUnitLinks and
''' cLinkDefaults
''' </remarks>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cLink
    Inherits cLinkDefault

#Region " Helper classes "

    ''' =======================================================================
    ''' <summary>
    ''' Helper class; allows the property grid to show a read-only unit name.
    ''' </summary>
    ''' =======================================================================
    Public Class cStaticUnitConverter
        Inherits TypeConverter

        Public Overrides Function GetStandardValuesSupported(context As ITypeDescriptorContext) As Boolean
            ' Do not show combo
            Return False
        End Function

        Public Overrides Function GetStandardValuesExclusive(context As ITypeDescriptorContext) As Boolean
            ' Do not edit combo
            Return True
        End Function

        ''' <summary>
        ''' Override the GetStandardValues method and return a 
        ''' StandardValuesCollection filled with your standard values
        ''' </summary>
        Public Overrides Function GetStandardValues(context As ITypeDescriptorContext) As TypeConverter.StandardValuesCollection
            Return New StandardValuesCollection(Nothing)
        End Function

        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As System.Type) As Boolean
            ' Can only convert FROM unit
            Return sourceType Is GetType(cUnit)
        End Function

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As System.Type) As Boolean
            ' Can only convert TO unit name
            Return destinationType Is GetType(String)
        End Function

        ''' <summary>
        ''' Convert unit to unit
        ''' </summary>
        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext,
                culture As System.Globalization.CultureInfo,
                value As Object) As Object
            Return MyBase.ConvertFrom(context, culture, value)
        End Function

        ''' <summary>
        ''' Convert unit to unit name
        ''' </summary>
        Public Overrides Function ConvertTo(context As ITypeDescriptorContext,
                culture As System.Globalization.CultureInfo,
                value As Object,
                destinationType As System.Type) As Object

            If TypeOf value Is cUnit Then
                Return DirectCast(value, cUnit).Name
            End If

            Return MyBase.ConvertTo(context, culture, value, destinationType)

        End Function

    End Class

#End Region ' Helper classes

#Region " Private bits "

    ''' <summary>Link name.</summary>
    Private m_strName As String = ""
    Private m_source As cUnit = Nothing
    Private m_target As cUnit = Nothing

#End Region ' Private bits

    Public Sub New()
        MyBase.New()
    End Sub

    <Browsable(True),
        Category(cCATEGORY_GENERIC),
        DisplayName("Name"),
        Description("Name of this link"),
        cPropertySorter.PropertyOrder(1)>
    Public Overrides Property Name() As String
        Get
            If String.IsNullOrWhiteSpace(Me.m_strName) Then
                Try
                    Return String.Format("{0} to {1}", Me.Source.ToString, Me.Target.ToString)
                Catch ex As Exception
                    Return "<unnamed link>"
                End Try
            End If
            Return Me.m_strName
        End Get
        Set(value As String)
            Me.m_strName = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(cCATEGORY_GENERIC),
        DisplayName("Source"),
        Description("Source unit for this link"),
        cPropertySorter.PropertyOrder(2),
        TypeConverter(GetType(cStaticUnitConverter))>
    Public Property Source() As cUnit
        Get
            Return Me.m_source
        End Get
        Set(value As cUnit)
            Debug.Assert(value IsNot Nothing)
            Me.m_source = value
        End Set
    End Property

    <Browsable(True),
        Category(cCATEGORY_GENERIC),
        DisplayName("Target"),
        Description("Target unit for this link"),
        cPropertySorter.PropertyOrder(3),
        TypeConverter(GetType(cStaticUnitConverter))>
    Public Property Target() As cUnit
        Get
            Return Me.m_target
        End Get
        Set(value As cUnit)
            Debug.Assert(value IsNot Nothing)
            Me.m_target = value
        End Set
    End Property

    <Browsable(True),
        Category(cCATEGORY_GENERIC),
        DisplayName("External"),
        Description("True when source and target differ in nationality."),
        cPropertySorter.PropertyOrder(4)>
    Public ReadOnly Property External() As Boolean
        Get
            If Me.Source Is Nothing Then Return False
            If Me.Target Is Nothing Then Return False
            Return Me.Source.Nationality <> Me.Target.Nationality
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the Style for this link
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <Browsable(False)>
    Public Overridable ReadOnly Property Style() As cStyleGuide.eStyleFlags
        Get
            Dim st As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            If (Me.ValuePerTon = 1.0) Then st = st Or cStyleGuide.eStyleFlags.ValueComputed
            Return st
        End Get
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
        If (obj Is Nothing) Then Return False
        If (Not TypeOf obj Is cLink) Then Return False
        Dim l As cLink = DirectCast(obj, cLink)
        Return (Me.Source.DBID = l.Source.DBID) And (Me.Target.DBID = l.Target.DBID)
    End Function

    Public Overrides Function ToString() As String
        Return Me.Name & " " & Me.BiomassRatio.ToString()
    End Function

End Class
