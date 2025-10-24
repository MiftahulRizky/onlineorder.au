Partial Class jobSheets
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Reprint") <> "" Then
            Dim request As HttpRequest = HttpContext.Current.Request
            Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)
            ' Dim thisString As String = String.Format("https://onlineorder.au/file/order/preview/{0}", Session("Reprint"))
            Dim thisString As String = String.Format(baseUrl & "/file/order/job/{0}", Session("Reprint"))
            embPrint.Attributes.Add("src", thisString)
        End If
    End Sub
End Class