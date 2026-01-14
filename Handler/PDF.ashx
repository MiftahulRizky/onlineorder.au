<%@ WebHandler Language="VB" Class="PDF" %>

Imports System
Imports System.Web
Imports System.IO
Imports System.Net

Public Class PDF : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim docId As String = context.Request.QueryString("document")
        If String.IsNullOrEmpty(docId) Then
            context.Response.StatusCode = 400
            context.Response.Write("Invalid request: docId is required")
            Return
        End If

        Dim filePath As String = context.Server.MapPath(docId)

        If File.Exists(filePath) Then
            Dim User As WebClient = New WebClient()
            Dim FileBuffer As Byte() = User.DownloadData(filePath)

            If FileBuffer IsNot Nothing Then
                context.Response.ContentType = "application/pdf"
                context.Response.AddHeader("content-length", FileBuffer.Length.ToString())
                context.Response.WriteFile(filePath)
                context.Response.End()

            End If
        Else
            context.Response.StatusCode = 404
            context.Response.Write("File not found")
        End If
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class