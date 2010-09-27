#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

Namespace Ecopath.Tools

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for formatting pedigree control content.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cPedigreeStyleGuide

#Region " Private vars "

        ''' <summary>The UI context to format against.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Current active render style.</summary>
        Private m_renderstyle As eRenderStyleTypes = eRenderStyleTypes.Colors

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constjuctoj.
        ''' </summary>
        ''' <param name="uic"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
        End Sub

#Region " Formatting "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the background colour for rendering pedigree information, 
        ''' considering the <paramref name="style">provided</paramref> and 
        ''' <see cref="RenderStyle">default</see> <see cref="eRenderStyleTypes">render styles</see>.
        ''' </summary>
        ''' <param name="clrBack">The default background colour to use if this method
        ''' does if no alternative is found.</param>
        ''' <param name="level">The <see cref="cPedigreeLevel">level</see> to render.</param>
        ''' <param name="style">The <see cref="eRenderStyleTypes">render style</see> to
        ''' use, or <see cref="eRenderStyleTypes.NotSet">NotSet</see> to use the
        ''' <see cref="RenderStyle">present render style</see>.</param>
        ''' <returns>A color.</returns>
        ''' -------------------------------------------------------------------
        Public Function BackgroundColor(ByVal clrBack As Color, _
                                        ByVal level As cPedigreeLevel, _
                                        Optional ByVal style As eRenderStyleTypes = eRenderStyleTypes.NotSet) As Color

            ' Fix up render style
            If (style = eRenderStyleTypes.NotSet) Then style = Me.m_renderstyle

            ' Do colour magic
            Select Case style
                Case eRenderStyleTypes.Colors
                    ' Use colour defined in the style guide for this level.
                    Return Me.m_uic.StyleGuide.PedigreeColor(Me.m_uic.Core, level.VariableName, level.ID)
            End Select

            ' Return provided default
            Return clrBack

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the foreground colour for rendering pedigree information, 
        ''' considering the <paramref name="style">provided</paramref> and 
        ''' <see cref="RenderStyle">default</see> <see cref="eRenderStyleTypes">render styles</see>.
        ''' </summary>
        ''' <param name="clrFore">The default foreground colour to use if this method
        ''' does if no alternative is found.</param>
        ''' <param name="level">The <see cref="cPedigreeLevel">level</see> to render.</param>
        ''' <param name="style">The <see cref="eRenderStyleTypes">render style</see> to
        ''' use, or <see cref="eRenderStyleTypes.NotSet">NotSet</see> to use the
        ''' <see cref="RenderStyle">present render style</see>.</param>
        ''' <returns>A color.</returns>
        ''' -------------------------------------------------------------------
        Public Function ForegroundColor(ByVal clrFore As Color, _
                                           ByVal level As cPedigreeLevel, _
                                           Optional ByVal style As eRenderStyleTypes = eRenderStyleTypes.NotSet) As Color
            ' Hah
            Return clrFore

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns text that represents the given level, considering the
        ''' <paramref name="style">provided</paramref> and 
        ''' <see cref="RenderStyle">default</see> <see cref="eRenderStyleTypes">render styles</see>.
        ''' </summary>
        ''' <param name="level">The <see cref="cPedigreeLevel">level</see> to render.</param>
        ''' <param name="style">The <see cref="eRenderStyleTypes">render style</see> to
        ''' use, or <see cref="eRenderStyleTypes.NotSet">NotSet</see> to use the
        ''' <see cref="RenderStyle">present render style</see>.</param>
        ''' <returns>A text that represents the given level, considering the
        ''' <paramref name="style">provided</paramref> and <see cref="RenderStyle">selected</see> render styles.</returns>
        ''' -------------------------------------------------------------------
        Public Function DisplayText(ByVal level As cPedigreeLevel, _
                                    Optional ByVal style As eRenderStyleTypes = eRenderStyleTypes.NotSet) As String

            ' Fix up render style
            If (style = eRenderStyleTypes.NotSet) Then style = Me.m_renderstyle

            ' Decide on string to display
            Select Case style
                Case eRenderStyleTypes.Colors
                    ' NOP

                Case eRenderStyleTypes.Index
                    ' Represent level by its index (local to its manager)
                    Return Me.m_uic.StyleGuide.FormatNumber(level.ID)

                Case eRenderStyleTypes.IndexValue
                    ' Represent level by its IndexValue
                    Return Me.m_uic.StyleGuide.FormatNumber(level.IndexValue)

                Case eRenderStyleTypes.ConfidenceInterval
                    ' Represent level by its ConfidenceInterval
                    Return Me.m_uic.StyleGuide.FormatNumber(CInt(level.ConfidenceInterval * 100), cStyleGuide.eStyleFlags.OK)

            End Select

            ' Return default
            Return ""

        End Function

#End Region ' Formatting

#Region " Render style "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Different render styles for pedigree information.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eRenderStyleTypes As Integer
            ''' <summary>Render style has not been provided.</summary>
            NotSet = 0
            ''' <summary>Render pedigree cells as colours.</summary>
            Colors
            ''' <summary>Render pedigree cells by <see cref="cPedigreeLevel.ID">level ID</see>.</summary>
            Index
            ''' <summary>Render pedigree cells by <see cref="cPedigreeLevel.IndexValue">index value</see>.</summary>
            IndexValue
            ''' <summary>Render pedigree cells by <see cref="cPedigreeLevel.IndexValue">confidence interval percentages</see>.</summary>
            ConfidenceInterval
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event for responding to a <see cref="RenderStyle">change in default render style</see>.
        ''' </summary>
        ''' <param name="sender">The style guide sending the event.</param>
        ''' -------------------------------------------------------------------
        Public Event OnRenderStyleChanged(ByVal sender As cPedigreeStyleGuide)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the default render style to use.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property RenderStyle() As eRenderStyleTypes
            Get
                Return Me.m_renderstyle
            End Get
            Set(ByVal value As eRenderStyleTypes)
                If (value <> Me.m_renderstyle) Then
                    Me.m_renderstyle = value
                    RaiseEvent OnRenderStyleChanged(Me)
                End If
            End Set
        End Property

#End Region ' Render style

    End Class

End Namespace
