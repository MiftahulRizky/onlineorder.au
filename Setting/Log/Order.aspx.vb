
Partial Class Setting_Log_Order
    Inherits Page

    Dim logCfg As New LogConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Call MessageError(False, String.Empty)
        Try
            Dim search As String = String.Empty

            If Not SearchText = "" Then
                search = "WHERE Log_Orders.Id LIKE '%" + SearchText + "%' OR Log_Orders.HeaderId LIKE '%" + SearchText + "%' OR Log_Orders.ItemId LIKE '%" + SearchText + "%' OR Log_Orders.ActionBy LIKE '%" + SearchText + "%' OR Log_Orders.Description LIKE '%" + SearchText + "%' OR CustomerLogins.UserName LIKE '%" + SearchText + "%' OR CustomerLogins.Id LIKE '%" + SearchText + "%' OR CustomerLogins.CustomerId LIKE '%" + SearchText + "%' OR CustomerLogins.FullName LIKE '%" + SearchText + "%' OR OrderHeaders.OrderId LIKE '%" + SearchText + "%' OR OrderHeaders.OrderNumber LIKE '%" + SearchText + "%' OR OrderHeaders.OrderName LIKE '%" + SearchText + "%'"
            End If

            Dim stringQuery As String = String.Format("SELECT Log_Orders.*, OrderHeaders.OrderId AS OrderId, CustomerLogins.FullName AS ActionName FROM Log_Orders LEFT JOIN OrderHeaders ON Log_Orders.HeaderId = OrderHeaders.Id LEFT JOIN CustomerLogins ON Log_Orders.ActionBy = CustomerLogins.Id {0} ORDER BY Log_Orders.ActionDate DESC", search)

            gvList.DataSource = logCfg.GetListData(stringQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
