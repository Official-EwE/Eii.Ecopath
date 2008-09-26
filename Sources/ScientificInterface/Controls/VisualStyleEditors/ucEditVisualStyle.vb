'==============================================================================
'
' $Log: ucEditVisualStyle.vb,v $
' Revision 1.1  2008/09/26 07:31:25  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/06/02 00:01:47  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2008/01/01 19:51:18  jeroens
' New and/or moved
'
'==============================================================================

Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls

    Public Class ucEditVisualStyle
        Inherits UserControl

#Region " Factory "

        Public Shared Function GetEditor(ByVal vs As cVisualStyle, ByVal style As cVisualStyle.eVisualStyleTypes) As ucEditVisualStyle
            ' Sanity checks
            Debug.Assert(vs IsNot Nothing)

            If ((style And cVisualStyle.eVisualStyleTypes.Image) = cVisualStyle.eVisualStyleTypes.Image) Then
                Return New ucEditImage(vs, style)
            End If

            If ((style And cVisualStyle.eVisualStyleTypes.Font) = cVisualStyle.eVisualStyleTypes.Image) Then
                Return New ucEditFont(vs, style)
            End If

            Return New ucEditHatch(vs, style)

        End Function

#End Region ' Factory

#Region " Private vars "

        Private m_visualStyle As cVisualStyle = Nothing
        Private m_style As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal vs As cVisualStyle, ByVal style As cVisualStyle.eVisualStyleTypes)
            ' Sanity check
            Debug.Assert(vs IsNot Nothing)

            Me.m_visualStyle = vs
            Me.m_style = style
        End Sub

#End Region ' Constructor

#Region " Event "

        Public Event OnVisualStyleChanged(ByVal sender As ucEditVisualStyle)

        Protected Sub FireStyleChangedEvent()
            RaiseEvent OnVisualStyleChanged(Me)
        End Sub

#End Region

#Region " Properties "

        Public Property VisualStyle() As cVisualStyle
            Get
                Return Me.m_visualStyle
            End Get
            Set(ByVal value As cVisualStyle)
                Me.m_visualStyle = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Property RepresentationStyles() As cVisualStyle.eVisualStyleTypes
            Get
                Return Me.m_style
            End Get
            Set(ByVal value As cVisualStyle.eVisualStyleTypes)
                Me.m_style = value
            End Set
        End Property

#End Region ' Properties

#Region " Overridables "

        Public Overridable Function Apply(ByVal vs As cVisualStyle) As Boolean
            Return True
        End Function

#End Region ' Overridables

    End Class

End Namespace
