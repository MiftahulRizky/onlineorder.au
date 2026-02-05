Partial Class Order_DailyMail
    Inherits Page

    Dim mailCfg As New MailConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        mailCfg.MailProduction()
    End Sub
End Class
