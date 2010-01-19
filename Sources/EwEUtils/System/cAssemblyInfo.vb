#Region " Imports "

Option Strict On
Imports System.Reflection

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Interface for obtaining assembly information.
''' </summary>
''' <remarks>
''' Code adapted 20 Jan 2010 from "Reading Assembly attributes in VB.NET" by Mihir Patak,
''' http://www.vbdotnetheaven.com/UploadFile/mpathak/ReadingAssembly04112005053044AM/ReadingAssembly.aspx
''' </remarks>
''' ===========================================================================
Public Interface IAssemblyInfo

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Title' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Title() As String

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Description' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Description() As String

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Company' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Company() As String

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Product' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Product() As String

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Copyright' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Copyright() As String

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Trademark' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    ReadOnly Property Trademark() As String

End Interface

''' ===========================================================================
''' <summary>
''' Class that provides easy access to assembly information. 
''' </summary>
''' <remarks>
''' Code adapted 20 Jan 2010 from "Reading Assembly attributes in VB.NET" by Mihir Patak,
''' http://www.vbdotnetheaven.com/UploadFile/mpathak/ReadingAssembly04112005053044AM/ReadingAssembly.aspx
''' </remarks>
''' ===========================================================================
Public Class cAssemblyInfo
    Implements IAssemblyInfo

#Region " Private vars "

    ''' <summary>The assembly to explore.</summary>
    Dim m_ass As System.Reflection.Assembly = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="ass">The assembly to explore.</param>
    ''' ------------------------------------------------------------------------
    Sub New(ByVal ass As Assembly)
        Me.m_ass = ass
    End Sub

#End Region ' Constructor

#Region " Public properties "

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Company' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Company() As String _
        Implements IAssemblyInfo.Company
        Get
            Dim ca As AssemblyCompanyAttribute = Nothing
            ca = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyCompanyAttribute), False)(0), AssemblyCompanyAttribute)
            Return ca.Company.ToString
        End Get
    End Property

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Copyright' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Copyright() As String _
        Implements IAssemblyInfo.Copyright
        Get
            Dim ca As AssemblyCopyrightAttribute = Nothing
            ca = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute)
            Return ca.Copyright.ToCharArray
        End Get
    End Property

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Description' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Description() As String _
         Implements IAssemblyInfo.Description
        Get
            Dim da As AssemblyDescriptionAttribute = Nothing
            da = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyDescriptionAttribute), False)(0), AssemblyDescriptionAttribute)
            Return da.Description.ToString
        End Get
    End Property

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Product' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Product() As String _
            Implements IAssemblyInfo.Product
        Get
            Dim pa As AssemblyProductAttribute = Nothing
            pa = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute)
            Return pa.Product.ToString
        End Get
    End Property

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Title' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Title() As String _
           Implements IAssemblyInfo.Title
        Get
            Dim ta As AssemblyTitleAttribute = Nothing
            ta = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute)
            Return ta.Title.ToString
        End Get
    End Property

    ''' ------------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for an assembly 'Trademark' attribute.
    ''' </summary>
    ''' ------------------------------------------------------------------------
    Public ReadOnly Property Trademark() As String _
         Implements IAssemblyInfo.Trademark
        Get
            Dim ta As AssemblyTrademarkAttribute = Nothing
            ta = DirectCast(m_ass.GetCustomAttributes(GetType(AssemblyTrademarkAttribute), False)(0), AssemblyTrademarkAttribute)
            Return ta.Trademark.ToString
        End Get
    End Property

#End Region ' Public access

End Class
