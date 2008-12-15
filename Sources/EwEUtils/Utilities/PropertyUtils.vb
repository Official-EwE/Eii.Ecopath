'==============================================================================
'
' $Log: PropertyUtils.vb,v $
' Revision 1.2  2008/12/15 16:06:33  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/04/15 17:38:41  jeroens
' Moved propery disagnostics from Ecost logic to here
'
' Revision 1.1  2008/04/14 17:31:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.ComponentModel

#End Region ' Imports

Namespace Utilities

    ''' ===========================================================================
    ''' <summary>
    ''' Code taken from "Ordering Items in the Property Grid" by
    ''' Paul T (http://www.codeproject.com/script/Articles/MemberArticles.aspx?amid=126190)
    ''' url: http://www.codeproject.com/KB/cpp/orderedpropertygrid.aspx
    ''' </summary>
    ''' <remarks>
    ''' Usage:
    ''' 
    ''' [TypeConverter(typeof(PropertySorter))]
    ''' [DefaultProperty("Name")]
    ''' Public Class Person
    ''' {
    '''     ..
    '''     ..
    ''' }
    ''' </remarks>
    ''' ===========================================================================
    Public Class cPropertySorter
        Inherits ExpandableObjectConverter

        Public Overloads Overrides Function GetPropertiesSupported(ByVal context As ITypeDescriptorContext) As Boolean
            Return True
        End Function

        ''' <summary>
        ''' This override returns a list of properties in order
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="value"></param>
        ''' <param name="attributes"></param>
        ''' <returns></returns>
        Public Overloads Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, ByVal value As Object, ByVal attributes As Attribute()) As PropertyDescriptorCollection

            Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, attributes)
            Dim alPropsOrdered As New ArrayList()
            For Each pd As PropertyDescriptor In pdc
                Dim attribute As Attribute = pd.Attributes(GetType(PropertyOrderAttribute))
                ' Has an order specifier attribute?
                If attribute IsNot Nothing Then
                    ' #Yes: create an pair object to hold it
                    Dim poa As PropertyOrderAttribute = DirectCast(attribute, PropertyOrderAttribute)
                    alPropsOrdered.Add(New PropertyOrderPair(pd.Name, poa.Order))
                Else
                    ' #No: give a default order of 0
                    alPropsOrdered.Add(New PropertyOrderPair(pd.Name, 0))
                End If
            Next

            ' Perform the actual order using the value PropertyOrderPair classes
            ' implementation of IComparable to sort
            alPropsOrdered.Sort()

            ' Build a string list of the ordered names
            Dim lNames As New List(Of String)
            For Each pop As PropertyOrderPair In alPropsOrdered
                lNames.Add(pop.Name)
            Next

            ' Pass in the ordered list for the PropertyDescriptorCollection to sort by
            Return pdc.Sort(lNames.ToArray())
        End Function

    End Class

#Region "Helper Class - PropertyOrderAttribute"

    <AttributeUsage(AttributeTargets.[Property])> _
    Public Class PropertyOrderAttribute
        Inherits Attribute

        ''' <summary>Simple attribute to allow the order of a property to be specified.</summary>
        Private m_iOrder As Integer = 0

        Public Sub New(ByVal iOrder As Integer)
            m_iOrder = iOrder
        End Sub

        Public ReadOnly Property Order() As Integer
            Get
                Return m_iOrder
            End Get
        End Property
    End Class

#End Region

#Region "Helper Class - PropertyOrderPair"

    Public Class PropertyOrderPair
        Implements IComparable

        Private _order As Integer
        Private _name As String
        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

        Public Sub New(ByVal name As String, ByVal order As Integer)
            _order = order
            _name = name
        End Sub

        Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
            '
            ' Sort the pair objects by ordering by order value
            ' Equal values get the same rank
            '
            Dim otherOrder As Integer = DirectCast(obj, PropertyOrderPair)._order
            If otherOrder = _order Then
                '
                ' If order not specified, sort by name
                '
                Dim otherName As String = DirectCast(obj, PropertyOrderPair)._name
                Return String.Compare(_name, otherName)
            ElseIf otherOrder > _order Then
                Return -1
            End If
            Return 1
        End Function

    End Class

#End Region

    Public Class cPropertyUtils

        Public Shared Function FindOrigPropertyDescriptor(ByVal pi As PropertyInfo) As PropertyDescriptor
            For Each pd As PropertyDescriptor In TypeDescriptor.GetProperties(pi.DeclaringType)
                If pd.Name.Equals(pi.Name) Then
                    Return pd
                End If
            Next
            Return Nothing
        End Function

        Public Shared Function FindOrigPropertyInfo(ByVal t As Type, ByVal pd As PropertyDescriptor) As PropertyInfo
            For Each pi As PropertyInfo In t.GetProperties()
                If pd.Name.Equals(pi.Name) Then
                    Return pi
                End If
            Next
            Return Nothing
        End Function

    End Class

End Namespace
