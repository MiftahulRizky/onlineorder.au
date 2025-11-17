Partial Class Shipment_Default
    Inherits Page

    Dim shipmentCfg As New ShipmentConfig
    Dim mailCfg As New MailConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" Then
            Response.Redirect("~/", False)
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("shipmentSearch")
            BindDataShipment(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("shipmentSearch") = txtSearch.Text
        Response.Redirect("~/shipment/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        BindDataShipment(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataShipment(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "gvList_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim dataId As String = e.CommandArgument.ToString()

                    Session("shipmentSearch") = txtSearch.Text
                    Session("shipmentId") = dataId

                    Response.Redirect("~/shipment/detail", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkDetail_Click", Session("LoginId"), ex.ToString())
                    End If
                End Try
            End If
        End If
    End Sub

    Private Sub BindDataShipment(Search As String)
        Try
            Dim bySearch As String = String.Empty
            If Not Search = "" Then
                bySearch = "AND OrderShipments.ShipmentNumber LIKE '%" + Search.Trim() + "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT OrderShipments.*, CustomerLogins.FullName AS FullName FROM OrderShipments LEFT JOIN CustomerLogins ON OrderShipments.CreatedBy = CustomerLogins.Id WHERE OrderShipments.Active = 1 {0}", bySearch)

            gvList.DataSource = shipmentCfg.GetListData(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindDataShipment", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
