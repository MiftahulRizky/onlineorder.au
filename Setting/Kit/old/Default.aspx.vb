Partial Class Setting_Kit_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            txtSearch.Text = Session("kitSearch")

            Call BindDataDesign()
            ddlDesign.SelectedValue = Session("kitDesign")

            Call BindDataBlind(ddlDesign.SelectedValue)
            ddlBlind.SelectedValue = Session("kitBlind")

            Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("kitSearch") = txtSearch.Text
        Session("kitDesign") = ddlDesign.SelectedValue
        Session("kitBlind") = ddlBlind.SelectedValue

        Response.Redirect("~/setting/kit/add", False)
    End Sub

    Protected Sub btnBracketType_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/kit/bracket", False)
    End Sub

    Protected Sub btnTubeType_Click(sender As Object, e As EventArgs)
        Session("kitSearch") = txtSearch.Text
        Session("kitDesign") = ddlDesign.SelectedValue
        Session("kitBlind") = ddlBlind.SelectedValue

        Response.Redirect("~/setting/kit/tube", False)
    End Sub

    Protected Sub btnControlType_Click(sender As Object, e As EventArgs)
        Session("kitSearch") = txtSearch.Text
        Session("kitDesign") = ddlDesign.SelectedValue
        Session("kitBlind") = ddlBlind.SelectedValue

        Response.Redirect("~/setting/kit/control", False)
    End Sub

    Protected Sub btnColourType_Click(sender As Object, e As EventArgs)
        Session("kitSearch") = txtSearch.Text
        Session("kitDesign") = ddlDesign.SelectedValue
        Session("kitBlind") = ddlBlind.SelectedValue

        Response.Redirect("~/setting/kit/colour", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindDataBlind(ddlDesign.SelectedValue)

        Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlBlind_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("kitDetail") = row.Cells(1).Text
            Session("kitSearch") = txtSearch.Text
            Session("kitDesign") = ddlDesign.SelectedValue
            Session("kitBlind") = ddlBlind.SelectedValue

            Response.Redirect("~/setting/kit/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE HARDWARE KIT. ID : " & lblId.Text)

            Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(6).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE HARDWARE KIT. ID : " & lblId.Text)

            Call BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(Design As String, Blind As String, SearchText As String)
        Session("kitDetail") = "" : Session("kitSearch") = ""
        Session("kitDesign") = "" : Session("kitBlind") = ""
        Try
            Dim stringQuery As String = ""
            Dim stringText As String = ""
            Dim stringProduct As String = ""

            If Not SearchText = "" Then
                stringText = "(HardwareKits.Id LIKE '%" + SearchText + "%' OR HardwareKits.SoeId LIKE '%" + SearchText + "%' OR HardwareKits.Name LIKE '%" + SearchText + "%' OR Designs.Name LIKE '%" + SearchText + "%' OR Blinds.Name LIKE '%" + SearchText + "%')"
            End If

            If Design = "" Then
                If Not SearchText = "" Then
                    stringQuery = "WHERE HardwareKits.Id LIKE '%" + SearchText + "%' OR HardwareKits.SoeId LIKE '%" + SearchText + "%' OR HardwareKits.Name LIKE '%" + SearchText + "%' OR Designs.Name LIKE '%" + SearchText + "%' OR Blinds.Name LIKE '%" + SearchText + "%'"
                End If
            End If

            If Not Design = "" Then
                stringProduct = " WHERE HardwareKits.DesignId = '" + UCase(Design).ToString() + "'"
                If Not Blind = "" Then
                    stringProduct = " WHERE HardwareKits.DesignId = '" + UCase(Design).ToString() + "' AND HardwareKits.BlindId = '" + UCase(Blind).ToString() + "'"
                End If

                stringQuery = stringProduct
                If Not SearchText = "" Then
                    stringQuery = stringProduct & " AND " & stringText
                End If
            End If
            Dim myQuery As String = String.Format("SELECT HardwareKits.*, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM HardwareKits INNER JOIN Designs ON HardwareKits.DesignId = Designs.Id INNER JOIN Blinds ON HardwareKits.BlindId = Blinds.Id {0} ORDER BY HardwareKits.Name ASC", stringQuery)
            gvList.DataSource = publicCfg.GetListData(myQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Company='SP' ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataBlind(DesignId As String)
        ddlBlind.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlind.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "NameText"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 0 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function IconActiveOnNames(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat me-2 opacity-50 fs-3'></i>Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-arrows-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-arrows-rotate'></i>" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function
End Class
