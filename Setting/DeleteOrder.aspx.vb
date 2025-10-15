Partial Class Setting_DeleteOrder
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            sdsPage.Update()
            Response.Redirect("~/", False)
        Catch ex As Exception
            publicCfg.MailError(Page.Title, "Page_Load", "64310A2F-4DD0-42BB-A72C-3A81ADF8D457", ex.ToString())
        End Try
    End Sub
End Class
