#Region " Imports "

Option Strict On
Imports EwECore

#End Region

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Wrapper class for storing a <see cref="cCoreInputOutputBase">core
    ''' input/output item</see> in a Windows control, such as listbox
    ''' or combobox.
    ''' </summary>
    ''' =======================================================================
    Public Class cCoreInputOutputControlItem

        ''' <summary>The object that is wrapped.</summary>
        Private m_source As cCoreInputOutputBase = Nothing
        ''' <summary>Alternative display string if the object is not present.</summary>
        Private m_strLabel As String = ""

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="obj">The object to wrap.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal obj As cCoreInputOutputBase)
            Me.m_source = obj
            Me.m_strLabel = ""
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="strLabel">The label to show for a null-object.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal strLabel As String)
            Me.m_source = Nothing
            Me.m_strLabel = strLabel
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the wrapped core input/output object.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overridable ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_source
            End Get
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Converts the object for display 
        ''' </summary>
        ''' <returns></returns>
        ''' ---------------------------------------------------------------
        Public Overrides Function ToString() As String
            If (Me.m_source Is Nothing) Then
                Return Me.m_strLabel
            End If
            Return String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, _
                                 Me.m_source.Index, _
                                 Me.m_source.Name)
        End Function

    End Class

End Namespace
