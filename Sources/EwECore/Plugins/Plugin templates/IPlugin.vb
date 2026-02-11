' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins

    ''' ===========================================================================
    ''' <summary>
    ''' Base interface for defining an EwE6 plug-in. Plug-ins are detected by the
    ''' presence of this Interface.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the plugin.
        ''' </summary>
        ''' <param name="core">The core this plugin is initialized for.</param>
        ''' -----------------------------------------------------------------------
        Sub Initialize(core As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Uniquely identifies a plugin. This field cannot be left empty!
        ''' </summary>
        ''' <remarks>
        ''' The name field will be used to determine the order of appearance of 
        ''' user interface plug-in elements; user interface elements originating
        ''' from plug-ins will be sorted by this property in ascending order.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Name() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' User-friendly display name of a plug-in.
        ''' </summary>
        ''' <seealso cref="DisplayName"/>
        ''' <seealso cref="Description"/>
        ''' -----------------------------------------------------------------------
        ReadOnly Property DisplayName() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Description of a plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Description() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Describes the author of the plugin.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Author() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Provides contact information about the plugin.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Contact() As String

    End Interface

End Namespace
