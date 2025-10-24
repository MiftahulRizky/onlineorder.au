<%@ WebHandler Language="VB" Class="DowloadPDFOrder" %>

Imports System
Imports System.Web
Imports System.IO

Public Class DowloadPDFOrder : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            Dim fileName As String = context.Request.QueryString("file")
            Dim keyDownload As String = context.Request.QueryString("keyDownload")

            ' Validasi nama file
            If String.IsNullOrEmpty(fileName) OrElse fileName.Contains("..") OrElse fileName.Contains("/") OrElse fileName.Contains("\") Then
                context.Response.StatusCode = 400
                context.Response.Write("Invalid file name.")
                Return
            End If

            If String.IsNullOrEmpty(keyDownload) Then
                context.Response.StatusCode = 400
                context.Response.Write("Missing keyDownload parameter.")
                Return
            End If

            ' Mapping dari symbolic key ke virtual folder
            Dim pathMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"invoice", "~/file/order/preview/"},
                {"jobsheet", "~/file/order/job/"},
                {"mail", "~/file/order/mail/"},
                {"quote", "~/file/order/quote/"}
            }

            If Not pathMap.ContainsKey(keyDownload) Then
                context.Response.StatusCode = 403
                context.Response.Write("Invalid keyDownload value.")
                Return
            End If

            Dim virtualPath As String = pathMap(keyDownload)
            Dim filePath As String = context.Server.MapPath(virtualPath & fileName)

            ' Cek apakah file ada
            If Not File.Exists(filePath) Then
                context.Response.StatusCode = 404
                context.Response.Write("File not found.")
                Return
            End If

            ' Siapkan file untuk di-download
            context.Response.Clear()
            context.Response.ContentType = "application/pdf"
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
            context.Response.TransmitFile(filePath)
            context.Response.Flush()
            context.Response.End()

        Catch ex As Exception
            context.Response.StatusCode = 500
            context.Response.Write("Internal Server Error: " & ex.Message)
        End Try
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class