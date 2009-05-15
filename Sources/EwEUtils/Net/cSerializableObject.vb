'==============================================================================
'
' $Log: cSerializableObject.vb,v $
' Revision 1.2  2009/05/15 01:23:16  jeroens
' Added Invalidate, IsValid
'
' Revision 1.1  2008/09/26 07:31:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/09/12 16:44:39  jeroens
' Overridden ToString
'
' Revision 1.5  2008/09/01 15:39:42  jeroens
' ID now provided in MustOverride R/O property
'
' Revision 1.4  2008/08/25 14:33:45  jeroens
' Screw the pre-build solution, it was becoming a headache!
'
' Revision 1.3  2008/08/21 20:08:39  joeb
'  Change deserialization constructor to us correct member name for m_id
'
' Revision 1.2  2008/05/29 22:22:51  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.1  2008/05/20 02:18:58  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Runtime.Serialization
Imports System.Collections
Imports System.Reflection

#End Region ' Imports directive

Namespace NetUtilities

    ''' ===========================================================================
    ''' <summary>
    ''' Implements a base class for a serializable objects.
    ''' </summary>
    ''' ===========================================================================
    <Serializable()> _
    Public MustInherit Class cSerializableObject
        Implements ISerializable

#Region " Private vars "

        ''' <summary>Flag stating that the contents of this object is valid.</summary>
        Private m_bValid As Boolean = True

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            MyBase.New()
            Me.m_bValid = True
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            Me.m_bValid = True
        End Sub

#End Region ' Constructors

#Region " Serialization Implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Serializes the object
        ''' </summary>
        ''' <param name="info"></param>
        ''' <param name="context"></param>
        ''' <remarks>
        ''' This takes care of all objects in the inheritance hierarchy. Derived classes 
        ''' should only override this method to add extra data.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overridable Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) _
            Implements ISerializable.GetObjectData
        End Sub

#End Region ' Serialization Implementation

#Region " Public interfaces "

        Public MustOverride ReadOnly Property ID() As String

        Public Overrides Function ToString() As String
            Return Me.ID()
        End Function

        Public Sub Invalidate()
            Me.m_bValid = False
        End Sub

        Public Function IsValid() As Boolean
            Return Me.m_bValid
        End Function

#End Region ' Public interfaces

    End Class

End Namespace
