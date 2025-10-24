Imports System.Web
Partial Class Order_Preview
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim request As HttpRequest = HttpContext.Current.Request
        Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)
        If Session("printPreview") <> "" Then
            ' Dim thisString As String = String.Format("https://onlineorder.au/file/order/preview/{0}", Session("printPreview"))
            Dim thisString As String = String.Format(baseUrl & "/file/order/preview/{0}", Session("printPreview"))
            embPrint.Attributes.Add("src", thisString)
        End If
    End Sub
End Class