Partial Class Statistics_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "PPIC & DE" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If
        If Not IsPostBack Then
            Call BackColor()
            Call BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlCompany.SelectedValue = "" Then
                Call MessageError(True, "COMPANY IS REQUIRED !")
                ddlCompany.CssClass= "form-select  is-invalid"
                ddlCompany.Focus()
                Exit Sub
            End If
            If txtFromDate.Text = "" Then
                Call MessageError(True, "FROM DATE IS REQUIRED !")
                txtFromDate.CssClass= "form-control  is-invalid"
                txtFromDate.Focus()
                Exit Sub
            End If

            If txtToDate.Text = "" Then
                Call MessageError(True, "TO DATE IS REQUIRED !")
                txtToDate.CssClass= "form-control  is-invalid"
                txtToDate.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim vFromDate As String = DateTime.Parse(txtFromDate.Text).ToString("yyyy-MM-dd")
                Dim vToDate As String = DateTime.Parse(txtToDate.Text).AddDays(1).ToString("yyyy-MM-dd")

                Dim vOrderTime As String = " AND OrderHeaders.SubmittedDate >= '" + vFromDate + "' AND OrderHeaders.SubmittedDate <= '" + vToDate + "'"

                Dim queryHeader As String = String.Format("SELECT OrderHeaders.*, Stores.Name AS StoreName, Stores.Company AS StoreCompany, SI.SumItem AS SumItem FROM OrderHeaders INNER JOIN Stores ON OrderHeaders.StoreId = Stores.Id INNER JOIN (SELECT HeaderId, SUM(Qty) AS SumItem FROM OrderDetails WHERE Active = 1 GROUP BY HeaderId) AS SI ON SI.HeaderId = OrderHeaders.Id WHERE OrderHeaders.Active=1")

                Dim queryDetail As String = String.Format("SELECT CASE WHEN view_details.BlindName = 'Ven' THEN 'Aluminium' ELSE view_details.BlindName END AS BlindName, COUNT(*) As CountItem FROM view_details INNER JOIN OrderHeaders ON view_details.HeaderId = OrderHeaders.Id WHERE view_details.Active = 1 {0} GROUP BY view_details.BlindName", vOrderTime)

                gvHeader.DataSource = publicCfg.GetListData(queryHeader)
                gvHeader.DataBind()

                gvDetail.DataSource = publicCfg.GetListData(queryDetail)
                gvDetail.DataBind()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataOrder", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.Items.Add(New ListItem("", ""))
            ddlCompany.DataSource = publicCfg.GetListData("SELECT * FROM Company WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()
            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Add(New ListItem("", ""))
            publicCfg.MailError(Session("UserId"), Page.Title, "BindCompany", ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        ddlCompany.CssClass = "form-select "
        txtFromDate.CssClass = "form-control "
        txtToDate.CssClass = "form-control "
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        msgError.InnerText = Msg
        If Show = True Then
            ' divError.Visible = True
            ' msgError.InnerText = Msg
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
